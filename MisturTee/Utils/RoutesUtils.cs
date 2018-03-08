using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using MisturTee.Config.Routing;
using MisturTee.Repositories;

namespace MisturTee.Utils
{
    public static class RoutesUtils
    {
        public static async Task<RouteMatchResult> GetMatchingRoute(HttpContext context)
        {
            var routeMatchResult = await RoutesUtils.MatchRouteAsync(context).ConfigureAwait(false);
            return new RouteMatchResult() { Route = routeMatchResult.Key, RouteValues = routeMatchResult.Value };
        }

        internal static async Task<KeyValuePair<RouteDefinition, RouteValueDictionary>> MatchRouteAsync(
            HttpContext context)
        {
            var routes = (await RoutesRepository.GetRoutesAsync().ConfigureAwait(false));
            if (routes.ContainsKey(context.Request.Method))
            {
                foreach (var route in routes[context.Request.Method])
                {
                    var templateMatcher = new TemplateMatcher(route.RouteTemplate, GetDefaults(route.RouteTemplate));
                    var routeValues = GetDefaults(route.RouteTemplate);
                    if (templateMatcher.TryMatch(context.Request.Path, routeValues))
                    {
                        return new KeyValuePair<RouteDefinition, RouteValueDictionary>(route.ToRouteDefinition(),
                            routeValues);
                    }
                }
            }
            return new KeyValuePair<RouteDefinition, RouteValueDictionary>(null, null);
        }

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
