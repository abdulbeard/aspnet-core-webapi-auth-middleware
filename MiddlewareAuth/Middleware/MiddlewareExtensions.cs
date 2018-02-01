using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using MisturTee.Config;
using MisturTee.Config.Routing;
using MisturTee.Repositories;

namespace MisturTee.Middleware
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
