using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using MisturTee.Extensions;

namespace TokenAuth.Auth
{
    public class TokenManager
    {
        private const string Key = "jApKAyYRMOQW1lilWun6RKEmn7914LmXlz0Oa2q5wCg=";
        private const string Iv = "H1Ykk45i/L66XaXv6Hh6Qg==";

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
            //var header = new JwtHeader(new SigningCredentials(
            //    policy.SecurityKey,
            //    SecurityAlgorithms.HmacSha512Signature));
            var header = new JwtHeader(new EncryptingCredentials(policy.SecurityKey, JwtConstants.DirectKeyUseAlg,
                SecurityAlgorithms.Aes256CbcHmacSha512));
            var payload = new JwtPayload(GetPayloadClaims(claims, policy));
            var token = new JwtSecurityToken(header, payload);
            //token.EncryptingCredentials = new EncryptingCredentials(policy.SecurityKey, JwtConstants.DirectKeyUseAlg,
            //    SecurityAlgorithms.Aes256CbcHmacSha512);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static TokenValidationResult ValidateJwt(string tokenString, TokenValidationParameters validationParameters)
        {
            try
            {
                var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(tokenString, validationParameters, out var validatedToken);
                return new TokenValidationResult{
                    ClaimsPrincipal = claimsPrincipal,
                    SecurityToken = validatedToken,
                    Successful = true
                };
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

        public static string SymmetricallyEncryptString(string plainText)
        {
            {
                // Check arguments.
                if (plainText == null || plainText.Length <= 0)
                    throw new ArgumentNullException(nameof(plainText));
                if (Key.Length <= 0)
                    throw new ArgumentNullException(nameof(Key));
                if (Iv.Length <= 0)
                    throw new ArgumentNullException(nameof(Iv));
                var encrypted = new List<byte>();
                // Create an Aes object
                // with the specified key and IV.
                using (var aesAlg = Aes.Create())
                {
                    if (aesAlg != null)
                    {
                        aesAlg.Key = Convert.FromBase64String(Key);
                        aesAlg.IV = Convert.FromBase64String(Iv);

                        // Create a decrytor to perform the stream transform.
                        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                        // Create the streams used for encryption.
                        using (var msEncrypt = new MemoryStream())
                        {
                            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            {
                                using (var swEncrypt = new StreamWriter(csEncrypt))
                                {

                                    //Write all data to the stream.
                                    swEncrypt.Write(plainText);
                                }
                                encrypted = msEncrypt.ToArray().ToList();
                            }
                        }
                    }
                }


                // Return the encrypted bytes from the memory stream.
                return Convert.ToBase64String(encrypted.ToArray());

            }
        }

        public static string SymmetricallyDecryptString(string cipherText)
        {
            {
                // Check arguments.
                if (cipherText == null || cipherText.Length <= 0)
                    throw new ArgumentNullException(nameof(cipherText));
                if (Key.Length <= 0)
                    throw new ArgumentNullException(nameof(Key));
                if (Iv.Length <= 0)
                    throw new ArgumentNullException(nameof(Iv));

                // Declare the string used to hold
                // the decrypted text.
                string plaintext = string.Empty;

                // Create an Aes object
                // with the specified key and IV.
                using (var aesAlg = Aes.Create())
                {
                    if (aesAlg != null)
                    {
                        aesAlg.Key = Convert.FromBase64String(Key);
                        aesAlg.IV = Convert.FromBase64String(Iv);

                        // Create a decrytor to perform the stream transform.
                        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                        // Create the streams used for decryption.
                        using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {

                                    // Read the decrypted bytes from the decrypting stream
                                    // and place them in a string.
                                    plaintext = srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }

                }

                return plaintext;

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

    public class TokenValidationResult
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public SecurityToken SecurityToken { get; set; }
        public bool Successful { get; set; }
        public string FailureReason { get; set; }
    }
}
