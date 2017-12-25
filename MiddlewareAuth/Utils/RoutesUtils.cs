using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System.Linq;

namespace TokenAuth.Utils
{
    public class RoutesUtils
    {
        public static RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();

            foreach (var parameter in parsedTemplate.Parameters.Where(x => x.DefaultValue != null))
            {
                result.Add(parameter.Name, parameter.DefaultValue);
            }

            return result;
        }
    }
}
