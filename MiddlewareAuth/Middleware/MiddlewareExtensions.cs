using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using MiddlewareAuth.Config.Routing;
using MiddlewareAuth.Config;
using System.Threading.Tasks;
using MiddlewareAuth.Repositories;

namespace MiddlewareAuth.Middleware
{
    public static class MiddlewareExtensions
    {
        public static async Task<IApplicationBuilder> UseCustomClaimsValidationAsync(this IApplicationBuilder app,
            IEnumerable<IRouteDefinitions> routeDefs,
            CustomClaimsValidationMiddleware.ClaimValidatorDelegate claimsValidator = null)
        {
            await RoutesRepository.RegisterRoutesAsync(routeDefs).ConfigureAwait(false);
            CustomClaimsValidationMiddleware.RegisterValidationDelegate(claimsValidator);
            return app.UseMiddleware<CustomClaimsValidationMiddleware>();
        }

        public static async Task<IApplicationBuilder> UseCustomClaimsValidationAsync(this IApplicationBuilder app,
            IValidRouteDefinitionProvider validRouteDefinitionProvider,
            CustomClaimsValidationMiddleware.ClaimValidatorDelegate claimsValidator = null)
        {
            await RoutesRepository.RegisterRoutesAsync(validRouteDefinitionProvider).ConfigureAwait(false);
            CustomClaimsValidationMiddleware.RegisterValidationDelegate(claimsValidator);
            return app.UseMiddleware<CustomClaimsValidationMiddleware>();
        }
    }
}
