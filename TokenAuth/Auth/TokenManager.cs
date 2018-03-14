using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using MisturTee.Extensions;

namespace TokenAuth.Auth
{
    public class TokenManager
    {
        private static readonly bool _useJweOverJwt = false;

        public static string CreateToken(TokenIssuancePolicy policy, IList<Claim> claims)
        {
            return _useJweOverJwt ? CreateJwe(policy, claims) : CreateJwt(policy, claims);
        }

        public static TokenValidationResult ValidateToken(string tokenString,
            TokenValidationParameters validationParameters)
        {
            return _useJweOverJwt ? ValidateJwe(tokenString, validationParameters) : ValidateJwt(tokenString, validationParameters);
        }

        public static readonly TokenValidationParameters DefaultValidationParameters = _useJweOverJwt ? new TokenValidationParameters
        {
            IssuerSigningKey = new TokenIssuancePolicy().SecurityKey,
            ValidIssuer = AppDomain.CurrentDomain.FriendlyName,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = false,
            RequireSignedTokens = false,
            TokenDecryptionKey = new EncryptingCredentials(new TokenIssuancePolicy().SecurityKey,
                JwtConstants.DirectKeyUseAlg,
                SecurityAlgorithms.Aes256CbcHmacSha512).Key
        } : new TokenValidationParameters
        {
            IssuerSigningKey = new TokenIssuancePolicy().SecurityKey,
            ValidIssuer = AppDomain.CurrentDomain.FriendlyName,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = false,
            TokenDecryptionKey = new SigningCredentials(new TokenIssuancePolicy().SecurityKey,
                SecurityAlgorithms.HmacSha512).Key
        };

        #region jwt
        private static string CreateJwt(TokenIssuancePolicy policy, IList<Claim> claims)
        {
            var header = new JwtHeader(new SigningCredentials(policy.SecurityKey, SecurityAlgorithms.HmacSha512));
            var payload = new JwtPayload(GetPayloadClaims(claims, policy));
            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static TokenValidationResult ValidateJwt(string tokenString, TokenValidationParameters validationParameters)
        {
            try
            {
                var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(tokenString, validationParameters, out var validatedToken);
                return HandleEncryptedClaims(new TokenValidationResult
                {
                    ClaimsPrincipal = claimsPrincipal,
                    SecurityToken = validatedToken,
                    Successful = true
                });
            }
            catch (Exception ex)
            {
                return new TokenValidationResult
                {
                    Successful = false,
                    FailureReason = ex.Message
                };
            }
        }

        private static TokenValidationResult HandleEncryptedClaims(TokenValidationResult validationResult)
        {
            var claims = new List<Claim>();
            foreach (var claim in validationResult.ClaimsPrincipal.Claims)
            {
                if (claim.Type.StartsWith("encrypted:"))
                {
                    claims.Add(new Claim(claim.Type.Replace("encrypted:", ""), CryptoUtils.SymmetricallyDecryptString(claim.Value)));
                }
                else
                {
                    claims.Add(claim);
                }
            }
            validationResult.ClaimsPrincipal =
                new ClaimsPrincipal(new List<ClaimsIdentity>() { new ClaimsIdentity(claims) }.AsEnumerable());
            return validationResult;
        }

        #endregion

        #region jwe

        private static string CreateJwe(TokenIssuancePolicy policy, IList<Claim> claims)
        {
            var header = new JwtHeader(new EncryptingCredentials(policy.SecurityKey, JwtConstants.DirectKeyUseAlg,
                SecurityAlgorithms.Aes256CbcHmacSha512));
            var payload = new JwtPayload(GetPayloadClaims(claims, policy));
            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static TokenValidationResult ValidateJwe(string tokenString, TokenValidationParameters validationParameters)
        {
            try
            {
                var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(tokenString, validationParameters, out var validatedToken);
                return HandleEncryptedClaims(new TokenValidationResult
                {
                    ClaimsPrincipal = claimsPrincipal,
                    SecurityToken = validatedToken,
                    Successful = true
                });
            }
            catch (Exception ex)
            {
                return new TokenValidationResult
                {
                    Successful = false,
                    FailureReason = ex.Message
                };
            }
        }
        #endregion

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
                        if (claim is EncryptedClaim)
                        {
                            payloadClaims.Add(
                                new Claim($"encrypted:{claim.Type}", CryptoUtils.SymmetricallyEncryptString(claim.Value)));
                        }
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

    public class TokenValidationResult
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public SecurityToken SecurityToken { get; set; }
        public bool Successful { get; set; }
        public string FailureReason { get; set; }
    }
}
