using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using MiddlewareAuth.Config.Routing;

namespace MiddlewareAuth.Utils
{
    public static class RoutesUtils
    {
        public static RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();

            foreach (var parameter in parsedTemplate.Parameters.Where(x => x.DefaultValue != null))
            {
                result.Add(parameter.Name, parameter.DefaultValue);
            }

            return result;
        }

        internal static Dictionary<string, List<InternalRouteDefinition>> GetValidRouteDefs(
            IEnumerable<IRouteDefinitions> routeDefs)
        {
            var result = new Dictionary<string, List<InternalRouteDefinition>>();
            if (routeDefs == null) return result;
            foreach (var routeDef in routeDefs)
            {
                var routeDefinitions = routeDef.GetRouteDefinitions() ?? new List<RouteDefinition>();
                foreach (var internalRouteDef in routeDefinitions.Select(x => new InternalRouteDefinition(x)))
                {
                    if (internalRouteDef.Method == null) continue;
                    if (!result.ContainsKey(internalRouteDef.Method.Method))
                    {
                        result.Add(internalRouteDef.Method.Method, new List<InternalRouteDefinition>());
                    }
                    result[internalRouteDef.Method.Method].Add(internalRouteDef);
                }
            }
            return result;
        }
    }
}
