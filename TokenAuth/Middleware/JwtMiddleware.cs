using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TokenAuth.Auth;

namespace TokenAuth.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"];
            var token = authHeader.Count > 0 ? authHeader.First().Replace("Bearer ", string.Empty) : string.Empty;
            if (!string.IsNullOrEmpty(token))
            {
                var jwtValidationResult = TokenManager.ValidateJwt(token, TokenManager.DefaultValidationParameters);
                if (jwtValidationResult.Key == null || jwtValidationResult.Value == null)
                {
                    context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    var bytesToWrite = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                    {
                        YouShallNotPass = "SaysGandalf"
                    }));
                    await context.Response.Body.WriteAsync(bytesToWrite, 0, bytesToWrite.Length).ConfigureAwait(false);
                }
                else
                {
                    var claims = jwtValidationResult.Key.Claims;
                    claims = claims.Select(x =>
                    {
                        if (string.Equals(x.Type, "supersecretid", StringComparison.OrdinalIgnoreCase))
                        {
                            return new Claim(x.Type, TokenManager.SymmetricallyDecryptString(x.Value));
                        }
                        return x;
                    });
                    context.Items.Add("ExpectedClaims", claims);
                }
            }
            await _next(context).ConfigureAwait(false);
        }
    }
}
