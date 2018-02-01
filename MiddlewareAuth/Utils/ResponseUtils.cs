using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MiddlewareAuth.Config.Claims;
using Newtonsoft.Json;

namespace MiddlewareAuth.Utils
{
    internal class ResponseUtils
    {
        internal static async Task CreateResponse(ValidationResult validationResult, HttpContext context,
            BadRequestResponse badRequestResponse)
        {
            if (badRequestResponse.BadRequestResponseOverride == null)
            {
                badRequestResponse.Headers.ToList().ForEach(x => context.Response.Headers.AppendList(x.Key, x.Value));
                context.Response.StatusCode = (int) badRequestResponse.HttpStatusCode;
                dynamic dynamicMissingClaimsResponseResponse = badRequestResponse.Response;
                dynamicMissingClaimsResponseResponse.MissingClaims = validationResult.MissingClaims;
                dynamicMissingClaimsResponseResponse.InvalidClaims =
                    validationResult.InvalidClaims.Select(x => new
                    {
                        ClaimName = x.ClaimName,
                        Value = x.ActualValue
                    });
                badRequestResponse.Response = dynamicMissingClaimsResponseResponse;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(badRequestResponse.Response))
                    .ConfigureAwait(false);
            }
            else
            {
                var overrideResponse = await badRequestResponse
                    .BadRequestResponseOverride(validationResult.MissingClaims, validationResult.InvalidClaims)
                    .ConfigureAwait(false);
                await BuildResponseFromResponse(overrideResponse, context).ConfigureAwait(false);
            }
        }

        private static async Task BuildResponseFromResponse(HttpResponse overrideResponse, HttpContext context)
        {
            if (overrideResponse != null)
            {
                context.Response.ContentLength = overrideResponse.ContentLength;
                context.Response.ContentType = overrideResponse.ContentType;
                context.Response.Headers.Clear();
                overrideResponse.Headers.ToList().ForEach(x => context.Response.Headers.AppendList(x.Key, x.Value));
                context.Response.StatusCode = overrideResponse.StatusCode;
                overrideResponse.Body.Position = 0;//resetting stream position
                var responseBytes = new byte[overrideResponse.Body.Length];
                await overrideResponse.Body.ReadAsync(responseBytes, 0, (int)overrideResponse.Body.Length).ConfigureAwait(false);
                await context.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length).ConfigureAwait(false);
            }
        }
    }
}
