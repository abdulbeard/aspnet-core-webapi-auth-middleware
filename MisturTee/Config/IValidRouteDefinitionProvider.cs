using System.Collections.Generic;
using System.Threading.Tasks;
using MisturTee.Config.Routing;

namespace MisturTee.Config
{
    public interface IValidRouteDefinitionProvider
    {
        Task<IEnumerable<IRouteDefinitions>> GetAsync();
    }
}
