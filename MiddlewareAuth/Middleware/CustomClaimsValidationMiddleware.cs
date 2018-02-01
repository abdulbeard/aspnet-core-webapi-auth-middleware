using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using MiddlewareAuth.Utils;

namespace MiddlewareAuth.Middleware
{
    public class CustomClaimsValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private static ClaimValidatorDelegate validationDelegate;

        public CustomClaimsValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        internal static void RegisterValidationDelegate(ClaimValidatorDelegate func)
        {
            validationDelegate = func;
        }

        public async Task Invoke(HttpContext context)
        {
            if (validationDelegate != null)
            {
                if (!await validationDelegate(context).ConfigureAwait(false))
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
        /// <returns>true if claims are valid and all is good with the request</returns>
        public delegate Task<bool> ClaimValidatorDelegate(HttpContext context);
    }
}
