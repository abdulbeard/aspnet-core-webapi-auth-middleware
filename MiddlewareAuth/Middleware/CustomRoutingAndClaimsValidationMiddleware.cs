using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Template;
using MiddlewareAuth.Config.Routing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            //var appTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.ExportedTypes);

            var httpAction = context.Request.Method;
            InternalRouteDefinition matchingRouteDefinition = null;
            foreach (var route in _routes[httpAction])
            {
                var templateMatcher = new TemplateMatcher(route.RouteTemplate, RoutesUtils.GetDefaults(route.RouteTemplate));
                var routeValues = RoutesUtils.GetDefaults(route.RouteTemplate);
                var routeIsAMatch = templateMatcher.TryMatch(context.Request.Path, routeValues);
                if (routeIsAMatch)
                {
                    matchingRouteDefinition = route;
                    break;
                }
            }

            var bodyMemStream = new MemoryStream();
            context.Request.Body.CopyTo(bodyMemStream);
            //context.Request.Body.Position = 0;
            var stringBody = System.Text.Encoding.UTF8.GetString(bodyMemStream.ToArray());
            var extractedClaims = (await Task.WhenAll(matchingRouteDefinition.ClaimsConfig.ExtractionConfigs.Select(x => x.GetClaimAsync())).ConfigureAwait(false)).ToList();
            var requiredClaims = matchingRouteDefinition.ClaimsConfig.ValidationConfig.Where(x => x.IsRequired);
            var missingRequiredClaims = requiredClaims.Select(x => x.ClaimName).Where(x => !extractedClaims.Select(y => y.Type).Contains(x)).Select(x => x);
            // if required claims are present, match claims to confirm all is good


            await _next(context).ConfigureAwait(false);
            return;
        }
    }
}
