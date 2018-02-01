using System;
using System.Net.Http;
using Microsoft.AspNetCore.Routing.Template;
using MisturTee.Config.Claims;

namespace MisturTee.Config.Routing
{
    internal class InternalRouteDefinition
    {
        internal InternalRouteDefinition(RouteDefinition routeDef)
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

        internal RouteDefinition ToRouteDefinition()
        {
            if (Method == HttpMethod.Get)
            {
                return new GetRouteDefinition
                {
                    ClaimsConfig = ClaimsConfig,
                    RouteTemplate = RouteTemplate.TemplateText
                };
            }
            if (Method == HttpMethod.Delete)
            {
                return new DeleteRouteDefinition(RequestBody)
                {
                    ClaimsConfig = ClaimsConfig,
                    RouteTemplate = RouteTemplate.TemplateText
                };
            }
            if (Method == HttpMethod.Head)
            {
                return new HeadRouteDefinition
                {
                    ClaimsConfig = ClaimsConfig,
                    RouteTemplate = RouteTemplate.TemplateText
                };
            }
            if (Method == HttpMethod.Options)
            {
                return new OptionsRouteDefinition
                {
                    ClaimsConfig = ClaimsConfig,
                    RouteTemplate = RouteTemplate.TemplateText
                };
            }
            if (Method == HttpMethod.Post)
            {
                return new PostRouteDefinition(RequestBody)
                {
                    ClaimsConfig = ClaimsConfig,
                    RouteTemplate = RouteTemplate.TemplateText
                };
            }
            if (Method == HttpMethod.Put)
            {
                return new PutRouteDefinition(RequestBody)
                {
                    ClaimsConfig = ClaimsConfig,
                    RouteTemplate = RouteTemplate.TemplateText
                };
            }
            if (Method == HttpMethod.Trace)
            {
                return new TraceRouteDefinition
                {
                    ClaimsConfig = ClaimsConfig,
                    RouteTemplate = RouteTemplate.TemplateText
                };
            }
            return null;
        }
    }
}
