using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MiddlewareAuth.Auth;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace TokenAuth.Controllers
{
    [Route("login")]
    public class LoginController : Controller
    {
        public string Get()
        {
            return "lksdjflksjdflksdf";
        }

        [HttpGet]
        public string GetToken([FromQuery] string username, [FromQuery] string password)
        {
            if (username == "validUser" && password == "validPassword")
            {
                return TokenManager.CreateJwt(new TokenIssuancePolicy(), new List<Claim>
                {
                    new Claim("CustomerId", Guid.NewGuid().ToString()),
                    new Claim("username", "totallyValidUserBro")
                });
                
            }
            return "no-abla-Engles";
        }

        [HttpPost("validate")]
        public bool Validate([FromHeader] string token)
        {
            var policy = new TokenIssuancePolicy();
            var validationResult = TokenManager.ValidateJwt(token, new TokenValidationParameters
            {
                IssuerSigningKey = policy.SecurityKey,
                ValidIssuer = AppDomain.CurrentDomain.FriendlyName,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateAudience = false
            });
            return validationResult.Key != null && validationResult.Value != null;
        }
    }
}
