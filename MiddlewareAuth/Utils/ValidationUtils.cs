using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MiddlewareAuth.Config.Claims;
using MiddlewareAuth.Config.Routing;

namespace MiddlewareAuth.Utils
{
    internal static class ValidationUtils
    {
        internal static async Task<bool> ValidateClaimsAsync(RouteDefinition routeDef, HttpContext context,
            RouteValueDictionary routeValues)
        {
            if (routeDef?.ClaimsConfig?.ValidationConfigs == null) return true;
            var claims = await ExtractionUtils.GetClaimsAsync(routeDef, context.Request, routeValues)
                .ConfigureAwait(false);
            var expectedClaims = (IEnumerable<Claim>) context.Items["ExpectedClaims"] ?? Enumerable.Empty<Claim>();
            var validationResult = Validate(routeDef.ClaimsConfig.ValidationConfigs, claims, expectedClaims);
            if (!validationResult.Success)
            {
                await ResponseUtils
                    .CreateResponse(validationResult, context, routeDef.ClaimsConfig.BadRequestResponse)
                    .ConfigureAwait(false);
                return false;
            }
            return true;
        }

        private static ValidationResult Validate(IEnumerable<ClaimValidationConfig> validationConfig,
            IEnumerable<Claim> extractedClaims, IEnumerable<Claim> expectedClaims)
        {
            var result = new ValidationResult();
            var extClaimsDict = extractedClaims.ToDictionary(x => x.Type, x => x.Value);
            var expectedClaimsDict = expectedClaims?.ToDictionary(x => x.Type, x => x.Value) ??
                                     new Dictionary<string, string>();
            foreach (var config in validationConfig)
                if (config.IsRequired)
                {
                    if (!extClaimsDict.ContainsKey(config.ClaimName))
                    {
                        result.MissingClaims.Add(config.ClaimName);
                        continue;
                    }
                    var extractedClaimValue = extClaimsDict[config.ClaimName];
                    if (!extClaimsDict.ContainsKey(config.ClaimName) ||
                        !config.AllowNullOrEmpty &&
                        string.IsNullOrEmpty(extractedClaimValue))
                    {
                        result.MissingClaims.Add(config.ClaimName);
                    }
                    else if (config.ValueMustBeExactMatch &&
                             (expectedClaimsDict.Keys?.Count ?? 0) > 0 &&
                             expectedClaimsDict.ContainsKey(config.ClaimName) &&
                             !string.Equals(extractedClaimValue, expectedClaimsDict[config.ClaimName],
                                 StringComparison.Ordinal))
                    {
                        result.InvalidClaims.Add(new InvalidClaimResult
                        {
                            ClaimName = config.ClaimName,
                            ActualValue = extractedClaimValue,
                            ExpectedValue = expectedClaimsDict[config.ClaimName]
                        });
                    }
                }
            return result;
        }
    }

    public class ValidationResult
    {
        public ValidationResult()
        {
            MissingClaims = new List<string>();
            InvalidClaims = new List<InvalidClaimResult>();
        }

        public List<string> MissingClaims { get; set; }
        public List<InvalidClaimResult> InvalidClaims { get; set; }
        public bool Success => (MissingClaims?.Count ?? 0) + (InvalidClaims?.Count ?? 0) == 0;
    }

    public class InvalidClaimResult
    {
        public string ClaimName { get; set; }
        public string ExpectedValue { get; set; }
        public string ActualValue { get; set; }
    }
}
