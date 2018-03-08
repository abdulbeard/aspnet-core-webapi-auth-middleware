using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MisturTee.Utils;
using static MisturTee.Config.Claims.ClaimValidationConfig;

namespace MisturTee.Middleware
{
    public class Sentinel
    {
        private readonly RequestDelegate _next;
        private static ClaimValidatorDelegate _validationDelegate;

        public Sentinel(RequestDelegate next)
        {
            _next = next;
        }

        internal static void RegisterValidationDelegate(ClaimValidatorDelegate func)
        {
            _validationDelegate = func;
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
                        .InternalValidateClaimsAsync(matchedRouteResult.Key, context, matchedRouteResult.Value)
                        .ConfigureAwait(false))
                    {
                        return;
                    }
                }
            }

            await _next(context).ConfigureAwait(false);
        }
    }
}
