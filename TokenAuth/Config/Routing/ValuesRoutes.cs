using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using TokenAuth.Utils;

namespace TokenAuth.Config.Routing
{
    public class ValuesRoutes
    {
        public const string Prefix = "values";
        public const string Get = "";
        public const string GetById = "{id}";
        public const string GetByIdGuid = "{id:guid}/yolo/{manafort}/sup/{manaforts:guid}";
        public const string Post = "";
        public const string Put = "{id}";
        public const string Delete = "{id}";

        public static readonly Dictionary<string, List<RouteDefinition>> RouteDefinitions = new List<RouteDefinition>
        {
            new RouteDefinition
            {
                Method = HttpMethod.Post,
                RequestBody = typeof(string),
                RouteTemplate = TemplateParser.Parse(Prefix + "/" + Post)
            },
            new RouteDefinition
            {
                Method = HttpMethod.Get,
                RouteTemplate = TemplateParser.Parse(Prefix + "/" + GetByIdGuid)
            }
        }.GroupBy(x => x.Method.Method).ToDictionary(x => x.Key, x => x?.ToList() ?? new List<RouteDefinition>());
    }

    public class RouteDefinition
    {
        public HttpMethod Method { get; set; }
        public RouteTemplate RouteTemplate { get; set; }
        public Type RequestBody { get; set; }
        public RouteClaimsConfig ClaimsConfig { get; set; }
    }
}
