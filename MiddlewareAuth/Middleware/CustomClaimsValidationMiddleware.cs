using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Template;
using MiddlewareAuth.Config.Routing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using MiddlewareAuth.Config.Claims;
using MiddlewareAuth.Config.Claims.ExtractionConfigs;
using Newtonsoft.Json;
using TokenAuth.Utils;

namespace MiddlewareAuth.Middleware
{
    public class CustomClaimsValidationMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomClaimsValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private static Dictionary<string, List<InternalRouteDefinition>> _routes;
        internal static void RegisterRoutes(Dictionary<string, List<InternalRouteDefinition>> routeDefs)
        {
            _routes = routeDefs;
        }

        public async Task Invoke(HttpContext context)
        {
            var matchedRouteResult = MatchRoute(context);
            if (matchedRouteResult.Key != null)
            {
                if (!(await ValidateClaimsAsync(matchedRouteResult.Key, context, matchedRouteResult.Value)
                    .ConfigureAwait(false)))
                {
                    return;
                }
            }

            await _next(context).ConfigureAwait(false);
        }

        private KeyValuePair<InternalRouteDefinition, RouteValueDictionary> MatchRoute(HttpContext context)
        {
            foreach (var route in _routes[context.Request.Method])
            {
                var templateMatcher = new TemplateMatcher(route.RouteTemplate, RoutesUtils.GetDefaults(route.RouteTemplate));
                var routeValues = RoutesUtils.GetDefaults(route.RouteTemplate);
                if (templateMatcher.TryMatch(context.Request.Path, routeValues))
                {
                    return new KeyValuePair<InternalRouteDefinition, RouteValueDictionary>(route, routeValues);
                }
            }
            return new KeyValuePair<InternalRouteDefinition, RouteValueDictionary>(null, null);
        }

        private async Task<bool> ValidateClaimsAsync(InternalRouteDefinition routeDef, HttpContext context, RouteValueDictionary routeValues)
        {
            var claims = await GetClaimsAsync(routeDef, context.Request, routeValues).ConfigureAwait(false);
            var missingClaims = GetMissingClaims(routeDef.ClaimsConfig.ValidationConfig, claims);
            if (missingClaims?.Count > 0)
            {
                await CreateResponse(missingClaims, context, routeDef.ClaimsConfig.MissingClaimsResponse).ConfigureAwait(false);
                return false;
            }
            return true;
        }

        private async Task CreateResponse(List<string> missingClaims, HttpContext context, MissingClaimsResponse missingClaimsResponse)
        {
            if (missingClaimsResponse.MissingClaimsResponseOverride == null)
            {
                context.Response.StatusCode = (int)missingClaimsResponse.HttpStatusCode;
                dynamic dynamicMissingClaimsResponseResponse = missingClaimsResponse.Response;
                dynamicMissingClaimsResponseResponse.Data = missingClaims;
                missingClaimsResponse.Response = dynamicMissingClaimsResponseResponse;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(missingClaimsResponse.Response)).ConfigureAwait(false);
            }
            else
            {
                var overrideResponse = await missingClaimsResponse.MissingClaimsResponseOverride(missingClaims).ConfigureAwait(false);
                await BuildResponseFromResponse(overrideResponse, context).ConfigureAwait(false);
            }
        }

        private async Task BuildResponseFromResponse(HttpResponse overrideResponse, HttpContext context)
        {
            if (overrideResponse != null)
            {
                context.Response.ContentLength = overrideResponse.ContentLength;
                context.Response.ContentType = overrideResponse.ContentType;
                context.Response.Headers.Clear();
                overrideResponse.Headers.ToList().ForEach(x => context.Response.Headers.AppendList(x.Key, x.Value));
                context.Response.StatusCode = overrideResponse.StatusCode;
                overrideResponse.Body.Position = 0;//resetting stream position
                var responseBytes = new byte[overrideResponse.Body.Length];
                var bytesRead = await overrideResponse.Body.ReadAsync(responseBytes, 0, (int)overrideResponse.Body.Length).ConfigureAwait(false);
                await context.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length).ConfigureAwait(false);
            }
        }

        private List<string> GetMissingClaims(IEnumerable<ClaimValidationConfig> validationConfig, IEnumerable<Claim> extractedClaims)
        {
            var extClaimsDict = extractedClaims.ToDictionary(x => x.Type, x => x.Value);
            return validationConfig
                .Where(x => x.IsRequired && (!extClaimsDict.ContainsKey(x.ClaimName) ||
                                             (!x.AllowNullOrEmpty && string.IsNullOrEmpty(extClaimsDict[x.ClaimName]))))
                .Select(x => x.ClaimName).ToList();
        }

        private async Task<List<Claim>> GetClaimsAsync(InternalRouteDefinition routeDef, HttpRequest req, RouteValueDictionary routeValues)
        {
            var dict = new Dictionary<ClaimLocation, Dictionary<ExtractionType, string>>();
            var taskClaims = new List<Task<Claim>>();

            foreach (var extractionConfig in routeDef.ClaimsConfig?.ExtractionConfigs ?? new List<IValidClaimsExtractionConfig>())
            {
                if (!dict.ContainsKey(extractionConfig.ClaimLocation))
                {
                    dict.Add(extractionConfig.ClaimLocation, new Dictionary<ExtractionType, string> { { extractionConfig.ExtractionType, string.Empty } });
                }
                else if (!dict[extractionConfig.ClaimLocation].ContainsKey(extractionConfig.ExtractionType))
                {
                    dict[extractionConfig.ClaimLocation].Add(extractionConfig.ExtractionType, string.Empty);
                }

                if (string.IsNullOrEmpty(dict[extractionConfig.ClaimLocation][extractionConfig.ExtractionType]))
                {
                    dict[extractionConfig.ClaimLocation][extractionConfig.ExtractionType] = GetContent(
                        extractionConfig.ClaimLocation, extractionConfig.ExtractionType, req, routeValues);
                }
                taskClaims.Add(extractionConfig.GetClaimAsync(dict[extractionConfig.ClaimLocation][extractionConfig.ExtractionType]));
            }

            return (await Task.WhenAll(taskClaims).ConfigureAwait(false)).ToList();
        }

        private string GetContent(ClaimLocation location, ExtractionType extType, HttpRequest req, RouteValueDictionary routeValues)
        {
            switch (location)
            {
                case ClaimLocation.Body:
                    {
                        var bodyMemStream = new MemoryStream();
                        req.Body.CopyTo(bodyMemStream);
                        var stringBody = System.Text.Encoding.UTF8.GetString(bodyMemStream.ToArray());
                        return stringBody;
                    }
                case ClaimLocation.Headers:
                    {
                        return extType == ExtractionType.KeyValue ? JsonConvert.SerializeObject(IHeaderDictToKvp(req.Headers, req.Headers)) : string.Empty;
                    }
                case ClaimLocation.Uri:
                    {
                        if (extType == ExtractionType.KeyValue)
                        {
                            return routeValues == null ? "" : JsonConvert.SerializeObject(RouteValueDictionaryToKvp(routeValues));
                        }
                        if (extType == ExtractionType.RegEx)
                        {
                            return req.Path;
                        }
                        break;
                    }
                case ClaimLocation.QueryParameters:
                    {
                        if (extType == ExtractionType.KeyValue)
                        {
                            return JsonConvert.SerializeObject(IQueryCollectionToKvp(req.Query));
                        }
                        if (extType == ExtractionType.RegEx)
                        {
                            return req.QueryString.ToString();
                        }
                        break;
                    }
                case ClaimLocation.None:
                default:
                    return string.Empty;
            }
            return string.Empty;
        }

        private List<KeyValuePair<string, List<object>>> IQueryCollectionToKvp(IQueryCollection iqc)
        {
            var result = new List<KeyValuePair<string, List<object>>>();
            foreach (var key in iqc.Keys)
            {
                result.Add(new KeyValuePair<string, List<object>>(key,
                    iqc[key].ToArray().Select(x => (object)x).ToList()));
            }
            return result;
        }

        private List<KeyValuePair<string, List<object>>> IHeaderDictToKvp(IEnumerable<KeyValuePair<string, StringValues>> arg, IHeaderDictionary dict)
        {
            return arg.Select(x =>
                new KeyValuePair<string, List<object>>(x.Key,
                    dict.GetCommaSeparatedValues(x.Key).Select(y => (object)y).ToList())).ToList();
        }

        private List<KeyValuePair<string, List<object>>> RouteValueDictionaryToKvp(IDictionary<string, object> arg)
        {
            return arg.Select(x => new KeyValuePair<string, List<object>>(x.Key, new List<object> { x.Value })).ToList();
        }
    }
}
