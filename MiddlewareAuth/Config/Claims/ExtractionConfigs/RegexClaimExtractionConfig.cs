using System;
using System.ComponentModel;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiddlewareAuth.Config.Claims.ExtractionConfigs
{
    public class RegexClaimExtractionConfig : ClaimsExtractionConfig
    {
        private Func<string, Regex, Task<string>> _regExExtraction;
        private Regex _regex;
        public RegexClaimExtractionConfig(string claimName, ClaimLocation location) : base(claimName)
        {
            if (!(location == ClaimLocation.Uri || location == ClaimLocation.Body))
            {
                throw new InvalidEnumArgumentException(
                    $"{nameof(location)} must either be {nameof(ClaimLocation.Uri)} or {nameof(ClaimLocation.Body)}");
            }
            ClaimName = claimName;
            Location = location;
            ExtractionType = ExtractionType.RegEx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func">takes in json as 1st arg, a <see cref="Regex"/> as 2nd arg, and returns the string value of the regex match</param>
        /// <param name="extractionRegex">the <see cref="Regex"/> used to extract the claim value</param>
        /// <returns></returns>
        public void ConfigureExtraction(Func<string, Regex, Task<string>> func, Regex extractionRegex)
        {
            _regExExtraction = func;
            _regex = extractionRegex;
        }

        public override IValidClaimsExtractionConfig Build()
        {
            if (_regExExtraction == null)
            {
                throw new ArgumentException($"Extraction function can't be null. Use {nameof(ConfigureExtraction)} method to configure it first.");
            }
            if (_regex == null)
            {
                throw new ArgumentException($"Extraction regex can't be null. Use {nameof(ConfigureExtraction)} method to configure it first.");
            }
            return new ValidRegexClaimExtractionConfig(_regExExtraction, _regex, ClaimName);
        }
    }

    public class ValidRegexClaimExtractionConfig : IValidClaimsExtractionConfig
    {
        private Func<string, Regex, Task<string>> _regExExtraction;
        private Regex _regex;
        private string _claimName;

        public ValidRegexClaimExtractionConfig(Func<string, Regex, Task<string>> func, Regex regex, string claim)
        {
            _regExExtraction = func;
            _regex = regex;
            _claimName = claim;
        }

        public async Task<Claim> GetClaimAsync(Type type = null, string content = null)
        {
            var regexValue = await _regExExtraction(content, _regex).ConfigureAwait(false);
            return new Claim(_claimName, regexValue);
        }
    }
}
