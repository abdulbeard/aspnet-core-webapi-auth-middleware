using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using MiddlewareAuth.Extensions;

namespace TokenAuth.Auth
{
    public class TokenManager
    {
        public static readonly TokenValidationParameters DefaultValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new TokenIssuancePolicy().SecurityKey,
            ValidIssuer = AppDomain.CurrentDomain.FriendlyName,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = false
        };

        public static string CreateJwt(TokenIssuancePolicy policy, IList<Claim> claims)
        {
            var header = new JwtHeader(new SigningCredentials(
                policy.SecurityKey,
                SecurityAlgorithms.HmacSha512Signature));
            var payload = new JwtPayload(GetPayloadClaims(claims, policy));
            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static KeyValuePair<ClaimsPrincipal, SecurityToken> ValidateJwt(string tokenString, TokenValidationParameters validationParameters)
        {
            try
            {
                var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(tokenString, validationParameters, out var validatedToken);
                return new KeyValuePair<ClaimsPrincipal, SecurityToken>(claimsPrincipal, validatedToken);
            }
            catch (Exception)
            {
                return new KeyValuePair<ClaimsPrincipal, SecurityToken>(null, null);
            }
        }

        private static IEnumerable<Claim> GetPayloadClaims(IEnumerable<Claim> claims, TokenIssuancePolicy policy)
        {
            var useDefaultIss = true;
            var useDefaultNbf = true;
            var useDefaultExp = true;
            var useDefaultIat = true;
            var useDefaultJti = true;

            var defaultPolicy = TokenIssuancePolicy.Default;

            var payloadClaims = new List<Claim>();

            foreach (var claim in claims)
            {
                switch (claim.Type)
                {
                    case JwtRegisteredClaimNames.Iss:
                        payloadClaims.Add(!string.IsNullOrEmpty(claim.Value) ? claim : GetClaimFromPolicy_Iss(policy));
                        useDefaultIss = false;
                        break;
                    case JwtRegisteredClaimNames.Nbf:
                        payloadClaims.Add(!string.IsNullOrEmpty(claim.Value) ? claim : GetClaimFromPolicy_Nbf(policy));
                        useDefaultNbf = false;
                        break;
                    case JwtRegisteredClaimNames.Exp:
                        payloadClaims.Add(!string.IsNullOrEmpty(claim.Value) ? claim : GetClaimFromPolicy_Exp(policy));
                        useDefaultExp = false;
                        break;
                    case JwtRegisteredClaimNames.Iat:
                        payloadClaims.Add(!string.IsNullOrEmpty(claim.Value) ? claim : GetClaimFromPolicy_Iat());
                        useDefaultIat = false;
                        break;
                    case JwtRegisteredClaimNames.Jti:
                        payloadClaims.Add(!string.IsNullOrEmpty(claim.Value) ? claim : GetClaimFromPolicy_Jti(policy));
                        useDefaultJti = false;
                        break;
                    default:
                        payloadClaims.Add(claim);
                        break;
                }
            }
            if (useDefaultExp)
            {
                payloadClaims.Add(GetClaimFromPolicy_Exp(defaultPolicy));
            }
            if (useDefaultIat)
            {
                payloadClaims.Add(GetClaimFromPolicy_Iat());
            }
            if (useDefaultIss)
            {
                payloadClaims.Add(GetClaimFromPolicy_Iss(defaultPolicy));
            }
            if (useDefaultJti)
            {
                payloadClaims.Add(GetClaimFromPolicy_Jti(defaultPolicy));
            }
            if (useDefaultNbf)
            {
                payloadClaims.Add(GetClaimFromPolicy_Nbf(defaultPolicy));
            }
            return payloadClaims;
        }

        private static Claim GetClaimFromPolicy_Iss(TokenIssuancePolicy policy)
        {
            return new Claim(JwtRegisteredClaimNames.Iss, policy.Issuer);
        }
        private static Claim GetClaimFromPolicy_Nbf(TokenIssuancePolicy policy)
        {
            return new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.Add(policy.NotBefore).TotalSecondsSinceEpoch().ToString());
        }
        private static Claim GetClaimFromPolicy_Jti(TokenIssuancePolicy policy)
        {
            return new Claim(JwtRegisteredClaimNames.Jti, policy.TokenId.ToString());
        }
        private static Claim GetClaimFromPolicy_Exp(TokenIssuancePolicy policy)
        {
            return new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.Add(policy.Expiration).TotalSecondsSinceEpoch().ToString());
        }
        private static Claim GetClaimFromPolicy_Iat()
        {
            return new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.TotalSecondsSinceEpoch().ToString());
        }
    }
}
