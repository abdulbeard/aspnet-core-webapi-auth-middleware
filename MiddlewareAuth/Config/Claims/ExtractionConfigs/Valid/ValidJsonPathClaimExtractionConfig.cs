using System.Security.Claims;
using System.Threading.Tasks;
using static MiddlewareAuth.Config.Claims.ExtractionConfigs.JsonPathClaimExtractionConfig;

namespace MiddlewareAuth.Config.Claims.ExtractionConfigs.Valid
{
    /// <summary>
    /// Validated version of <see cref="JsonPathClaimExtractionConfig"/> used to actually extract the claim
    /// </summary>
    public class ValidJsonPathClaimExtractionConfig : IValidClaimsExtractionConfig
    {
        private readonly ExtractValueByJsonPathAsync _extract;
        private readonly string _path;
        private readonly string _claimName;

        /// <summary>
        /// creates a new instance
        /// </summary>
        /// <param name="path">jsonPath</param>
        /// <param name="jsonPathExtraction"><see cref="ExtractValueByJsonPathAsync"/></param>
        /// <param name="claimName">name of the claim to extract</param>
        /// <param name="location"><see cref="ClaimLocation"/> location of the claim value</param>
        public ValidJsonPathClaimExtractionConfig(string path, ExtractValueByJsonPathAsync jsonPathExtraction, string claimName, ClaimLocation location)
        {
            _path = path;
            _extract = jsonPathExtraction;
            _claimName = claimName;
            ClaimLocation = location;
        }

        public ExtractionType ExtractionType => ExtractionType.JsonPath;
        public ClaimLocation ClaimLocation { get; }

        /// <summary>
        /// Extracts claim using the json and jsonPath
        /// </summary>
        /// <param name="json">JSON</param>
        /// <returns></returns>
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
