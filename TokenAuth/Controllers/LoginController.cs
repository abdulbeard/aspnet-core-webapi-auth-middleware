using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using TokenAuth.Auth;

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
                return TokenManager.CreateToken(new TokenIssuancePolicy(), new List<Claim>
                {
                    new Claim("CustomerId", Guid.NewGuid().ToString()),
                    new Claim("username", "totallyValidUserBro"),
                    new Claim("ReportViolationEmail", "yolo@nolo.com"),
                    new EncryptedClaim("superSecretId", "whooaaaaaaaaaaaa")
                });
                
            }
            return "no-abla-Engles";
        }

        [HttpPost("validate")]
        public bool Validate([FromHeader] string authorization)
        {
            authorization = authorization.Replace("Bearer ", "");
            var validationResult = TokenManager.ValidateToken(authorization, TokenManager.DefaultValidationParameters);
            return validationResult.Successful;
        }
    }
}
