using MiddlewareAuth.Config.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiddlewareAuth.Config
{
    public interface IValidRouteDefinitionProvider
    {
        Task<IEnumerable<IRouteDefinitions>> GetAsync();
    }
}
