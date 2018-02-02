using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MisturTee.Config.Claims.ExtractionConfigs.Valid
{
    /// <inheritdoc />
    public class ValidTypeClaimExtractionConfig<T> : IValidClaimsExtractionConfig
    {
        private readonly TypeClaimExtractionConfig<T>.ExtractClaimForTypeAsync _typeExtraction;
        private readonly string _claimName;

        /// <inheritdoc />
        public ValidTypeClaimExtractionConfig(TypeClaimExtractionConfig<T>.ExtractClaimForTypeAsync func, string claim, ClaimLocation location)
        {
            _typeExtraction = func;
            _claimName = claim;
            ClaimLocation = location;
        }

        /// <inheritdoc />
        public ExtractionType ExtractionType => ExtractionType.Type;

        /// <inheritdoc />
        public ClaimLocation ClaimLocation { get; }

        /// <inheritdoc />
        public async Task<Claim> GetClaimAsync(string content)
        {
            var value = await _typeExtraction(JsonConvert.DeserializeObject<T>(content)).ConfigureAwait(false);
            return new Claim(_claimName, value);
        }
    }
}
