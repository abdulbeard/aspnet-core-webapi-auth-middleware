using MiddlewareAuth.Config.Claims.ExtractionConfigs.Valid;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiddlewareAuth.Config.Claims.ExtractionConfigs
{
    /// <summary>
    /// Uses <see cref="Regex"/> to extract claim value
    /// </summary>
    public class RegexClaimExtractionConfig : ClaimsExtractionConfig
    {
        private ExtractValueByRegexAsync _regExExtraction;
        private Regex _regex;

        /// <summary>
        /// creates a new <see cref="RegexClaimExtractionConfig"/>
        /// </summary>
        /// <param name="claimName">name of claim</param>
        /// <param name="location"><see cref="ClaimLocation"/> location of the claim</param>
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
        /// configures this <see cref="RegexClaimExtractionConfig"/> for extraction
        /// </summary>
        /// <param name="func"><see cref="ExtractValueByRegexAsync"/></param>
        /// <param name="extractionRegex">the <see cref="Regex"/> used to extract the claim value</param>
        /// <returns></returns>
        public ClaimsExtractionConfig ConfigureExtraction(ExtractValueByRegexAsync func, Regex extractionRegex)
        {
            _regExExtraction = func;
            _regex = extractionRegex;
            return this;
        }

        /// <summary>
        /// returns <see cref="IValidClaimsExtractionConfig"/> after validating this <see cref="RegexClaimExtractionConfig"/>
        /// </summary>
        /// <returns></returns>
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
            return new ValidRegexClaimExtractionConfig(_regExExtraction, _regex, ClaimName, Location);
        }

        /// <summary>
        /// extracts and returns claim value by applying the provided regex to the provided content
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="regex"><see cref="Regex"/></param>
        /// <returns></returns>
        public delegate Task<string> ExtractValueByRegexAsync(string content, Regex regex);
    }    
}
