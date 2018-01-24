using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Net.Http;
using MiddlewareAuth.Config.Claims;
using Newtonsoft.Json;

namespace MiddlewareAuth.Config.Routing
{
    [JsonObject]
    internal class InternalRouteDefinition
    {
        [JsonConstructor]
        internal InternalRouteDefinition() { }
        //[JsonConstructor]
        internal InternalRouteDefinition(RouteDefinition routeDef)
        {
            Method = routeDef.Method();
            RouteTemplate = TemplateParser.Parse(routeDef.RouteTemplate);
            RequestBody = routeDef.RequestBody();
            ClaimsConfig = routeDef.ClaimsConfig;
        }
        [JsonProperty]
        internal HttpMethod Method { get; set; }
        [JsonProperty]
        internal RouteTemplate RouteTemplate { get; set; }
        [JsonProperty]
        internal Type RequestBody { get; set; }
        [JsonProperty]
        internal RouteClaimsConfig ClaimsConfig { get; set; }
    }
}
