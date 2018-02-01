using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MisturTee.Config.Claims.ExtractionConfigs.Valid
{
    /// <summary>
    /// Valid version of <see cref="RegexClaimExtractionConfig"/>
    /// </summary>
    public class ValidRegexClaimExtractionConfig : IValidClaimsExtractionConfig
    {
        private RegexClaimExtractionConfig.ExtractValueByRegexAsync _extract;
        private Regex _regex;
        private string _claimName;

        /// <summary>
        /// creats a new <see cref="ValidRegexClaimExtractionConfig"/>
        /// </summary>
        /// <param name="func">function used for extraction</param>
        /// <param name="regex"><see cref="Regex"/> used for pattern matching</param>
        /// <param name="claim">name of the claim</param>
        /// <param name="location"><see cref="ClaimLocation"/> location of the claim</param>
        public ValidRegexClaimExtractionConfig(RegexClaimExtractionConfig.ExtractValueByRegexAsync func, Regex regex, string claim, ClaimLocation location)
        {
            _extract = func;
            _regex = regex;
            _claimName = claim;
            ClaimLocation = location;
        }

        public ExtractionType ExtractionType => ExtractionType.RegEx;
        public ClaimLocation ClaimLocation { get; }

        /// <summary>
        /// extracts and returns the claim using <see cref="Regex"/> on the provided content
        /// </summary>
        /// <param name="content">content</param>
        /// <returns></returns>
        public async Task<Claim> GetClaimAsync(string content)
        {
            var regexValue = await _extract(content, _regex).ConfigureAwait(false);
            return new Claim(_claimName, regexValue);
        }
    }
}
