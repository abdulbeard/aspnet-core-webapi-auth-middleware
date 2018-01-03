using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Template;
using MiddlewareAuth.Config.Routing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MiddlewareAuth.Config.Claims;
using MiddlewareAuth.Config.Claims.ExtractionConfigs;
using Newtonsoft.Json;
using TokenAuth.Utils;

namespace MiddlewareAuth.Middleware
{
    public class CustomRoutingAndClaimsValidationMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomRoutingAndClaimsValidationMiddleware(RequestDelegate next)
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
                await CreateMissingClaimsResponseAsync(missingClaims, context).ConfigureAwait(false);
                return false;
            }
            return true;
        }

        private async Task CreateMissingClaimsResponseAsync(List<string> missingClaims, HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                ErrorCode = 1235,
                Message = "The following claims require values",
                Data = missingClaims
            })).ConfigureAwait(false);
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
                        return extType == ExtractionType.KeyValue ? JsonConvert.SerializeObject(req.Headers) : string.Empty;
                    }
                case ClaimLocation.Uri:
                    {
                        if (extType == ExtractionType.KeyValue)
                        {
                            return routeValues == null ? "" : JsonConvert.SerializeObject(routeValues);
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
                            return JsonConvert.SerializeObject(req.Query);
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
    }
}
