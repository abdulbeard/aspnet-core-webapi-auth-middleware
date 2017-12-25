using System;
using System.Net.Http;

namespace MiddlewareAuth.Config.Routing
{
    public abstract class RouteDefinition
    {
        public abstract HttpMethod Method();
        public string RouteTemplate { get; set; }
        public abstract Type RequestBody();
        public RouteClaimsConfig ClaimsConfig { get; set; }
    }
}
