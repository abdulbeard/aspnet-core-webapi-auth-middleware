using MiddlewareAuth.Config.Routing;
using System.Collections.Generic;

namespace TokenAuth.Routes
{
    public class ValuesRoutes : IRouteDefinitions
    {
        public const string Prefix = "values";
        public const string Get = "";
        public const string GetById = "{id}";
        public const string GetByIdGuid = "{id:guid}/yolo/{manafort}/sup/{manaforts:guid}";
        public const string Post = "";
        public const string Put = "{id}";
        public const string Delete = "{id}";
        private const string pathSeparator = "/";

        public List<RouteDefinition> GetRouteDefinitions()
        {
            return new List<RouteDefinition> {
                new PostRouteDefinition(typeof(string))
                {
                    RouteTemplate = $"{Prefix}{pathSeparator}{Post}"
                },
                new GetRouteDefinition
                {
                    RouteTemplate = $"{Prefix}{pathSeparator}{GetByIdGuid}"
                }
            };
        }
    }
}
