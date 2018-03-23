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
            IEnumerable<IRouteDefinitions> routeDefs, RoutesRepository routesRepository,
            ClaimValidatorDelegate claimsValidator = null)
        {
            await routesRepository.RegisterRoutesAsync(routeDefs).ConfigureAwait(false);
            Sentinel.RegisterValidationDelegate(claimsValidator);
            return app.UseMiddleware<Sentinel>();
        }

        public static async Task<IApplicationBuilder> UseCustomClaimsValidationAsync(this IApplicationBuilder app,
            IValidRouteDefinitionProvider validRouteDefinitionProvider, RoutesRepository routesRepository,
            ClaimValidatorDelegate claimsValidator = null)
        {
            await routesRepository.RegisterRoutesAsync(validRouteDefinitionProvider).ConfigureAwait(false);
            Sentinel.RegisterValidationDelegate(claimsValidator);
            return app.UseMiddleware<Sentinel>();
        }
    }
}
