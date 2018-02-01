using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MisturTee.Config.Routing;
using MisturTee.Utils;

namespace MisturTee.Middleware
{
    public class CustomClaimsValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private static ClaimValidatorDelegate _validationDelegate;

        public CustomClaimsValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        internal static void RegisterValidationDelegate(ClaimValidatorDelegate func)
        {
            _validationDelegate = func;
        }

        public static async Task<KeyValuePair<RouteDefinition, RouteValueDictionary>> GetMatchingRoute(HttpContext context)
        {
            return await RoutesUtils.MatchRouteAsync(context).ConfigureAwait(false);
        }

        public static async Task<bool> ValidateClaimsAsync(RouteDefinition routeDef, HttpContext context,
            RouteValueDictionary routeValues)
        {
            return await ValidationUtils.ValidateClaimsAsync(routeDef, context, routeValues).ConfigureAwait(false);
        }

        public async Task Invoke(HttpContext context)
        {
            if (_validationDelegate != null)
            {
                if (!await _validationDelegate(context).ConfigureAwait(false))
                {
                    return;
                }
            }
            else
            {
                var matchedRouteResult = await RoutesUtils.MatchRouteAsync(context).ConfigureAwait(false);
                if (matchedRouteResult.Key != null)
                {
                    if (!await ValidationUtils
                        .ValidateClaimsAsync(matchedRouteResult.Key, context, matchedRouteResult.Value)
                        .ConfigureAwait(false))
                    {
                        return;
                    }
                }
            }

            await _next(context).ConfigureAwait(false);
        }

        /// <summary>
        /// Delegate that takes in <see cref="HttpContext"/> and evaluates if all is good with the request
        /// </summary>
        /// <param name="context">current http context</param>
        /// <returns>
        /// true if claims are valid and all is good with the request. 
        /// false if errors were found with the request, and the request will be cut short and the response returned
        /// </returns>
        public delegate Task<bool> ClaimValidatorDelegate(HttpContext context);
    }
}
