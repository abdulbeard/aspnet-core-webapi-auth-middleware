using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;

namespace TokenAuth.Extractors
{
    public class ValidatedRegExExtractor : IValidClaimsExtractionConfig
    {
        private readonly RegexClaimExtractionConfig.ExtractValueByRegexAsync _extract;
        private readonly Regex _regex;
        private readonly string _claimName;

        /// <inheritdoc />
        public ValidatedRegExExtractor(RegexClaimExtractionConfig.ExtractValueByRegexAsync func, Regex regex, string claim, ClaimLocation location)
        {
            _extract = func;
            _regex = regex;
            _claimName = claim;
            ClaimLocation = location;
        }

        /// <inheritdoc />
        public ExtractionType ExtractionType => ExtractionType.RegEx;

        /// <inheritdoc />
        public ClaimLocation ClaimLocation { get; }

        /// <inheritdoc />
        public async Task<Claim> GetClaimAsync(string content)
        {
            var regexValue = await _extract(content, _regex).ConfigureAwait(false);
            return new Claim(_claimName, regexValue);
        }
    }
}
