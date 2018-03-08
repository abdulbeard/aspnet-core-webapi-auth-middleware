using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TokenAuth.Auth;

namespace TokenAuth.Middleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"];
            var token = authHeader.Count > 0 ? authHeader.First().Replace("Bearer ", string.Empty) : string.Empty;
            if (!string.IsNullOrEmpty(token))
            {
                var jwtValidationResult = TokenManager.ValidateToken(token, TokenManager.DefaultValidationParameters);
                if (!jwtValidationResult.Successful)
                {
                    context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    var bytesToWrite = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                    {
                        YouShallNotPass = "SaysGandalf",
                        Reason = jwtValidationResult.FailureReason
                    }));
                    await context.Response.Body.WriteAsync(bytesToWrite, 0, bytesToWrite.Length).ConfigureAwait(false);
                    return;
                }
                else
                {
                    context.Items.Add("ExpectedClaims", jwtValidationResult.ClaimsPrincipal.Claims);
                }
            }
            await _next(context).ConfigureAwait(false);
        }
    }
}
