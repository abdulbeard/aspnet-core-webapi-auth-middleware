using System;
using System.Net.Http;
using MisturTee.Config.Claims;

namespace MisturTee.Config.Routing
{
    public abstract class RouteDefinition
    {
        public abstract HttpMethod Method();
        public string RouteTemplate { get; set; }
        public abstract Type RequestBody();
        public RouteClaimsConfig ClaimsConfig { get; set; }
    }
}
