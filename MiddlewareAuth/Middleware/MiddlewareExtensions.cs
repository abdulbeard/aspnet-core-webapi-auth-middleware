using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using MiddlewareAuth.Config.Routing;
using MiddlewareAuth.Config;
using System.Threading.Tasks;
using MiddlewareAuth.Repositories;
using MiddlewareAuth.Utils;

namespace MiddlewareAuth.Middleware
{
    public static class MiddlewareExtensions
    {
        public static async Task<IApplicationBuilder> UseCustomClaimsValidationAsync(this IApplicationBuilder app,
            IEnumerable<IRouteDefinitions> routeDefs)
        {
            await RoutesRepository.RegisterRoutesAsync(routeDefs).ConfigureAwait(false);
            return app.UseMiddleware<CustomClaimsValidationMiddleware>();
        }

        public static async Task<IApplicationBuilder> UseCustomClaimsValidationAsync(this IApplicationBuilder app,
            IValidRouteDefinitionProvider validRouteDefinitionProvider)
        {
            await RoutesRepository.RegisterRoutesAsync(validRouteDefinitionProvider).ConfigureAwait(false);
            return app.UseMiddleware<CustomClaimsValidationMiddleware>();
        }
    }
}
