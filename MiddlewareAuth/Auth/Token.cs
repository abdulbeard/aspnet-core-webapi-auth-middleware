using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using MiddlewareAuth.Extensions;

namespace MiddlewareAuth.Auth
{
    public class TokenManager
    {
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
                var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(tokenString, validationParameters, out SecurityToken validatedToken);
                return new KeyValuePair<ClaimsPrincipal, SecurityToken>(claimsPrincipal, validatedToken);
            }
            catch (Exception)
            {
                return new KeyValuePair<ClaimsPrincipal, SecurityToken>(null, null);
            }
        }

        private static IList<Claim> GetPayloadClaims(IList<Claim> claims, TokenIssuancePolicy policy)
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
                if (claim.Type == JwtRegisteredClaimNames.Iss)
                {
                    payloadClaims.Add(!string.IsNullOrEmpty(claim.Value) ? claim : GetClaimFromPolicy_Iss(policy));
                    useDefaultIss = false;
                }
                else if (claim.Type == JwtRegisteredClaimNames.Nbf)
                {
                    payloadClaims.Add(!string.IsNullOrEmpty(claim.Value) ? claim : GetClaimFromPolicy_Nbf(policy));
                    useDefaultNbf = false;
                }
                else if (claim.Type == JwtRegisteredClaimNames.Exp)
                {
                    payloadClaims.Add(!string.IsNullOrEmpty(claim.Value) ? claim : GetClaimFromPolicy_Exp(policy));
                    useDefaultExp = false;
                }
                else if (claim.Type == JwtRegisteredClaimNames.Iat)
                {
                    payloadClaims.Add(!string.IsNullOrEmpty(claim.Value) ? claim : GetClaimFromPolicy_Iat(policy));
                    useDefaultIat = false;
                }
                else if (claim.Type == JwtRegisteredClaimNames.Jti)
                {
                    payloadClaims.Add(!string.IsNullOrEmpty(claim.Value) ? claim : GetClaimFromPolicy_Jti(policy));
                    useDefaultJti = false;
                }
                else
                {
                    payloadClaims.Add(claim);
                }
            }
            if (useDefaultExp)
            {
                payloadClaims.Add(GetClaimFromPolicy_Exp(defaultPolicy));
            }
            if (useDefaultIat)
            {
                payloadClaims.Add(GetClaimFromPolicy_Iat(defaultPolicy));
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
        private static Claim GetClaimFromPolicy_Iat(TokenIssuancePolicy policy)
        {
            return new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.TotalSecondsSinceEpoch().ToString());
        }
    }
}
