using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using MiddlewareAuth.Config.Routing;

namespace MiddlewareAuth.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomClaimsValidation(this IApplicationBuilder app, IEnumerable<IRouteDefinitions> routeDefs)
        {
            CustomClaimsValidationMiddleware.RegisterRoutes(GetValidateRouteDefs(routeDefs));
            return app.UseMiddleware<CustomClaimsValidationMiddleware>();
        }

        private static Dictionary<string, List<InternalRouteDefinition>> GetValidateRouteDefs(IEnumerable<IRouteDefinitions> routeDefs)
        {
            var result = new Dictionary<string, List<InternalRouteDefinition>>();
            if (routeDefs != null)
            {
                foreach (var routeDef in routeDefs)
                {
                    var routeDef_definitions = routeDef.GetRouteDefinitions() ?? new List<RouteDefinition>();
                    foreach (var route_routeDef in routeDef_definitions)
                    {
                        var internalRouteDef = new InternalRouteDefinition(route_routeDef);
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
            }
            return result;
        }
    }
}
