using System.Security.Claims;
using System.Threading.Tasks;

namespace MisturTee.Config.Claims.ExtractionConfigs.Valid
{
    /// <inheritdoc />
    internal class ValidJsonPathClaimExtractionConfig : IValidClaimsExtractionConfig
    {
        private readonly JsonPathClaimExtractionConfig.ExtractValueByJsonPathAsync _extract;
        private readonly string _path;
        private readonly string _claimName;

        /// <inheritdoc />
        internal ValidJsonPathClaimExtractionConfig(string path, JsonPathClaimExtractionConfig.ExtractValueByJsonPathAsync jsonPathExtraction, string claimName, ClaimLocation location)
        {
            _path = path;
            _extract = jsonPathExtraction;
            _claimName = claimName;
            ClaimLocation = location;
        }

        /// <inheritdoc />
        public ExtractionType ExtractionType => ExtractionType.JsonPath;

        /// <inheritdoc />
        public ClaimLocation ClaimLocation { get; }

        /// <inheritdoc />
        public async Task<Claim> GetClaimAsync(string json)
        {
            if (json == null)
            {
                return null;
            }
            var claimValue = await _extract(json, _path).ConfigureAwait(false);
            return new Claim(_claimName, claimValue);
        }
    }
}
