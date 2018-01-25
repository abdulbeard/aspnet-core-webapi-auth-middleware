using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using System.Linq;
using MiddlewareAuth.Config.Routing;
using MiddlewareAuth.Config;
using System.Threading.Tasks;

namespace MiddlewareAuth.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomClaimsValidation(this IApplicationBuilder app, IEnumerable<IRouteDefinitions> routeDefs)
        {
            CustomClaimsValidationMiddleware.RegisterRoutes(GetValidRouteDefs(routeDefs));
            return app.UseMiddleware<CustomClaimsValidationMiddleware>();
        }

        public static async Task<IApplicationBuilder> UseCustomClaimsValidationAsync(this IApplicationBuilder app, IValidRouteDefinitionProvider validRouteDefinitionProvider)
        {
            CustomClaimsValidationMiddleware.RegisterRoutes(GetValidRouteDefs(await validRouteDefinitionProvider.GetAsync().ConfigureAwait(false)));
            return app.UseMiddleware<CustomClaimsValidationMiddleware>();
        }

        private static Dictionary<string, List<InternalRouteDefinition>> GetValidRouteDefs(
            IEnumerable<IRouteDefinitions> routeDefs)
        {
            var result = new Dictionary<string, List<InternalRouteDefinition>>();
            if (routeDefs == null) return result;
            foreach (var routeDef in routeDefs)
            {
                var routeDefinitions = routeDef.GetRouteDefinitions() ?? new List<RouteDefinition>();
                foreach (var internalRouteDef in routeDefinitions.Select(x => new InternalRouteDefinition(x)))
                {
                    if (internalRouteDef.Method != null)
                    {
                        if (!result.ContainsKey(internalRouteDef.Method.Method))
                        {
                            result.Add(internalRouteDef.Method.Method, new List<InternalRouteDefinition>());
                        }
                        result[internalRouteDef.Method.Method].Add(internalRouteDef);
                    }
                }
            }
            return result;
        }
    }
}
