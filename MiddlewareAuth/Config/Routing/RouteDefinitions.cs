using System;
using System.Net.Http;

namespace MisturTee.Config.Routing
{
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
}
