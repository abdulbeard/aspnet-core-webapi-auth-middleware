using Microsoft.AspNetCore.Routing;

namespace MisturTee.Config.Routing
{
    /// <summary>
    /// Contains the result of a route-matching operation
    /// </summary>
    public class RouteMatchResult
    {
        /// <summary>
        /// The definition of the matched route. Null if no match found
        /// </summary>
        public RouteDefinition Route { get; set; }
        /// <summary>
        /// Route values from the current HttpContext for the matched route
        /// </summary>
        public RouteValueDictionary RouteValues { get; set; }
    }
}
