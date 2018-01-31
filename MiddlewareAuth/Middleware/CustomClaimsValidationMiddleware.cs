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
using Newtonsoft.Json;
using MiddlewareAuth.Config.Claims.ExtractionConfigs.Valid;
using MiddlewareAuth.Repositories;
using MiddlewareAuth.Utils;

namespace MiddlewareAuth.Middleware
{
    public class CustomClaimsValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomClaimsValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var matchedRouteResult = await MatchRouteAsync(context).ConfigureAwait(false);
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

        private static async Task<KeyValuePair<InternalRouteDefinition, RouteValueDictionary>> MatchRouteAsync(HttpContext context)
        {
            foreach (var route in (await RoutesRepository.GetRoutesAsync().ConfigureAwait(false))[context.Request.Method])
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

        private static async Task<bool> ValidateClaimsAsync(InternalRouteDefinition routeDef, HttpContext context, RouteValueDictionary routeValues)
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

        private static async Task CreateResponse(IReadOnlyCollection<string> missingClaims, HttpContext context, MissingClaimsResponse missingClaimsResponse)
        {
            if (missingClaimsResponse.MissingClaimsResponseOverride == null)
            {
                missingClaimsResponse.Headers.ToList().ForEach(x => context.Response.Headers.AppendList(x.Key, x.Value));
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

        private static async Task BuildResponseFromResponse(HttpResponse overrideResponse, HttpContext context)
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
                await overrideResponse.Body.ReadAsync(responseBytes, 0, (int)overrideResponse.Body.Length).ConfigureAwait(false);
                await context.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length).ConfigureAwait(false);
            }
        }

        private static List<string> GetMissingClaims(IEnumerable<ClaimValidationConfig> validationConfig, IEnumerable<Claim> extractedClaims)
        {
            var extClaimsDict = extractedClaims.ToDictionary(x => x.Type, x => x.Value);
            return validationConfig
                .Where(x => x.IsRequired && (!extClaimsDict.ContainsKey(x.ClaimName) ||
                                             (!x.AllowNullOrEmpty && string.IsNullOrEmpty(extClaimsDict[x.ClaimName]))))
                .Select(x => x.ClaimName).ToList();
        }

        private static async Task<List<Claim>> GetClaimsAsync(InternalRouteDefinition routeDef, HttpRequest req, RouteValueDictionary routeValues)
        {
            var dict = new Dictionary<ClaimLocation, Dictionary<ExtractionType, string>>();
            var taskClaims = new List<Task<Claim>>();

            foreach (var extractionConfig in routeDef.ClaimsConfig?.ExtractionConfigs ?? new List<IValidClaimsExtractionConfig>())
            {
                if (!dict.ContainsKey(extractionConfig.ClaimLocation))
                {
                    dict.Add(extractionConfig.ClaimLocation, new Dictionary<ExtractionType, string>
                    {
                        { extractionConfig.ExtractionType, string.Empty }
                    });
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

        private static string GetContent(ClaimLocation location, ExtractionType extType, HttpRequest req, RouteValueDictionary routeValues)
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
                        return extType == ExtractionType.KeyValue ? JsonConvert.SerializeObject(HeaderDictToKvp(req.Headers, req.Headers)) : string.Empty;
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
                        switch (extType)
                        {
                            case ExtractionType.KeyValue:
                                return JsonConvert.SerializeObject(QueryCollectionToKvp(req.Query));
                            case ExtractionType.RegEx:
                                return req.QueryString.ToString();
                        }
                        break;
                    }
                default:
                    return string.Empty;
            }
            return string.Empty;
        }

        private static List<KeyValuePair<string, List<object>>> QueryCollectionToKvp(IQueryCollection iqc)
        {
            return iqc.Keys.Select(key => new KeyValuePair<string, List<object>>(key, iqc[key].ToArray().Select(x => (object)x).ToList())).ToList();
        }

        private static List<KeyValuePair<string, List<object>>> HeaderDictToKvp(IEnumerable<KeyValuePair<string, StringValues>> arg, IHeaderDictionary dict)
        {
            return arg.Select(x =>
                new KeyValuePair<string, List<object>>(x.Key,
                    dict.GetCommaSeparatedValues(x.Key).Select(y => (object)y).ToList())).ToList();
        }

        private static List<KeyValuePair<string, List<object>>> RouteValueDictionaryToKvp(IDictionary<string, object> arg)
        {
            return arg.Select(x => new KeyValuePair<string, List<object>>(x.Key, new List<object> { x.Value })).ToList();
        }
    }
}
