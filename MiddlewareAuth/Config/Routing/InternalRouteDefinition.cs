using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Net.Http;
using MiddlewareAuth.Config.Claims;

namespace MiddlewareAuth.Config.Routing
{

    internal class InternalRouteDefinition
    {
        public InternalRouteDefinition(RouteDefinition routeDef)
        {
            Method = routeDef.Method();
            RouteTemplate = TemplateParser.Parse(routeDef.RouteTemplate);
            RequestBody = routeDef.RequestBody();
            ClaimsConfig = routeDef.ClaimsConfig;
        }

        internal HttpMethod Method { get; set; }
        internal RouteTemplate RouteTemplate { get; set; }
        internal Type RequestBody { get; set; }
        internal RouteClaimsConfig ClaimsConfig { get; set; }
    }
}
