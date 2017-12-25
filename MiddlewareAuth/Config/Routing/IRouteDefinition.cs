using System.Collections.Generic;

namespace MiddlewareAuth.Config.Routing
{
    public interface IRouteDefinitions
    {
        List<RouteDefinition> GetRouteDefinitions();
    }
}
