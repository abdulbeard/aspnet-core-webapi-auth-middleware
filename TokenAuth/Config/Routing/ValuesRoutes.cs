using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace TokenAuth.Config.Routing
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

    public interface IRouteDefinitions
    {
        List<RouteDefinition> GetRouteDefinitions();
    }

    public class GetRouteDefinition : RouteDefinition
    {
        public GetRouteDefinition()
        {
            _method = HttpMethod.Get;
            _requestBody = typeof(object);
        }

        private HttpMethod _method;
        private Type _requestBody;

        public override HttpMethod Method()
        {
            return _method;
        }

        public override Type RequestBody()
        {
            return _requestBody;
        }
    }

    public class PostRouteDefinition : RouteDefinition
    {
        public PostRouteDefinition(Type requestBody)
        {
            _method = HttpMethod.Post;
            _requestBody = requestBody;
        }

        private HttpMethod _method;
        private Type _requestBody;

        public override HttpMethod Method()
        {
            return _method;
        }

        public override Type RequestBody()
        {
            return _requestBody;
        }
    }

    public class PutRouteDefinition : RouteDefinition
    {
        public PutRouteDefinition(Type requestBody)
        {
            _method = HttpMethod.Put;
            _requestBody = requestBody;
        }

        private HttpMethod _method;
        private Type _requestBody;

        public override HttpMethod Method()
        {
            return _method;
        }

        public override Type RequestBody()
        {
            return _requestBody;
        }
    }

    public class HeadRouteDefinition : RouteDefinition
    {
        public HeadRouteDefinition()
        {
            _method = HttpMethod.Head;
            _requestBody = typeof(object);
        }

        private HttpMethod _method;
        private Type _requestBody;

        public override HttpMethod Method()
        {
            return _method;
        }

        public override Type RequestBody()
        {
            return _requestBody;
        }
    }

    public class OptionsRouteDefinition : RouteDefinition
    {
        public OptionsRouteDefinition()
        {
            _method = HttpMethod.Options;
            _requestBody = typeof(object);
        }

        private HttpMethod _method;
        private Type _requestBody;

        public override HttpMethod Method()
        {
            return _method;
        }

        public override Type RequestBody()
        {
            return _requestBody;
        }
    }

    public class DeleteRouteDefinition : RouteDefinition
    {
        public DeleteRouteDefinition(Type requestBody)
        {
            _method = HttpMethod.Delete;
            _requestBody = requestBody;
        }

        private HttpMethod _method;
        private Type _requestBody;

        public override HttpMethod Method()
        {
            return _method;
        }

        public override Type RequestBody()
        {
            return _requestBody;
        }
    }

    public class TraceRouteDefinition : RouteDefinition
    {
        public TraceRouteDefinition()
        {
            _method = HttpMethod.Trace;
            _requestBody = typeof(object);
        }

        private HttpMethod _method;
        private Type _requestBody;

        public override HttpMethod Method()
        {
            return _method;
        }

        public override Type RequestBody()
        {
            return _requestBody;
        }
    }

    public abstract class RouteDefinition
    {
        public abstract HttpMethod Method();
        public string RouteTemplate { get; set; }
        public abstract Type RequestBody();
        public RouteClaimsConfig ClaimsConfig { get; set; }
    }

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
