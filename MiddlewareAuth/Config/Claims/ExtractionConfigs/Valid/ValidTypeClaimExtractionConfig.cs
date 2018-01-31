using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiddlewareAuth.Config.Claims.ExtractionConfigs.Valid
{
    public class ValidTypeClaimExtractionConfig<T> : IValidClaimsExtractionConfig
    {
        private readonly TypeClaimExtractionConfig<T>.ExtractClaimForTypeAsync _typeExtraction;
        private readonly string _claimName;

        /// <summary>
        /// Used by <see cref="TypeClaimExtractionConfig{T}"/> after having validated itself
        /// </summary>
        /// <param name="func">function that returns claim value</param>
        /// <param name="claim">name of the claim</param>
        /// <param name="location"><see cref="ClaimLocation"/>, location of the claim</param>
        public ValidTypeClaimExtractionConfig(TypeClaimExtractionConfig<T>.ExtractClaimForTypeAsync func, string claim, ClaimLocation location)
        {
            _typeExtraction = func;
            _claimName = claim;
            ClaimLocation = location;
        }

        public ExtractionType ExtractionType => ExtractionType.Type;
        public ClaimLocation ClaimLocation { get; }

        /// <summary>
        /// runs the extraction function and returns <see cref="Claim"/>
        /// </summary>
        /// <param name="content"><see cref="T"/> represented in JSON</param>
        /// <returns></returns>
        public async Task<Claim> GetClaimAsync(string content)
        {
            var value = await _typeExtraction(JsonConvert.DeserializeObject<T>(content)).ConfigureAwait(false);
            return new Claim(_claimName, value);
        }
    }
}
