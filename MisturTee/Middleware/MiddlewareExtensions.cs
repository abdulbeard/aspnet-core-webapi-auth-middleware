using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using MisturTee.Config;
using MisturTee.Config.Routing;
using MisturTee.Repositories;
using static MisturTee.Config.Claims.ClaimValidationConfig;

namespace MisturTee.Middleware
{
    public static class MiddlewareExtensions
    {
        public static async Task<IApplicationBuilder> UseCustomClaimsValidationAsync(this IApplicationBuilder app,
            IEnumerable<IRouteDefinitions> routeDefs,
            ClaimValidatorDelegate claimsValidator = null)
        {
            await RoutesRepository.RegisterRoutesAsync(routeDefs).ConfigureAwait(false);
            Sentinel.RegisterValidationDelegate(claimsValidator);
            return app.UseMiddleware<Sentinel>();
        }

        public static async Task<IApplicationBuilder> UseCustomClaimsValidationAsync(this IApplicationBuilder app,
            IValidRouteDefinitionProvider validRouteDefinitionProvider,
            ClaimValidatorDelegate claimsValidator = null)
        {
            await RoutesRepository.RegisterRoutesAsync(validRouteDefinitionProvider).ConfigureAwait(false);
            Sentinel.RegisterValidationDelegate(claimsValidator);
            return app.UseMiddleware<Sentinel>();
        }
    }
}
