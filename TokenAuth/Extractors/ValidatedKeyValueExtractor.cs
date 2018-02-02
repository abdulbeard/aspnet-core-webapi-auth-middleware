using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using Newtonsoft.Json;

namespace TokenAuth.Extractors
{
    public class ValidatedKeyValueExtractor : IValidClaimsExtractionConfig
    {
        private KeyValueClaimExtractionConfig.KeyValueExtractionAsync _extract;
        private string _keyName;
        private string _claimName;

        /// <inheritdoc />
        public ValidatedKeyValueExtractor(KeyValueClaimExtractionConfig.KeyValueExtractionAsync func, string key, ClaimLocation location, string claimName)
        {
            _extract = func;
            _keyName = key;
            ClaimLocation = location;
            _claimName = claimName;
        }

        /// <inheritdoc />
        public ExtractionType ExtractionType => ExtractionType.KeyValue;

        /// <inheritdoc />
        public ClaimLocation ClaimLocation { get; }

        /// <inheritdoc />
        public async Task<Claim> GetClaimAsync(string content)
        {
            var contentDict = JsonConvert.DeserializeObject<List<KeyValuePair<string, List<object>>>>(content);
            var value = await _extract(contentDict, _keyName)
                .ConfigureAwait(false);
            return new Claim(_claimName, value);
        }
    }
}
