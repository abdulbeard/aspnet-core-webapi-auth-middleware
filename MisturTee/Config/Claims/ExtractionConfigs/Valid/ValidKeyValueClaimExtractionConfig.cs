using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MisturTee.Config.Claims.ExtractionConfigs.Valid
{
    /// <summary>
    /// Valid version of <see cref="KeyValueClaimExtractionConfig"/>
    /// </summary>
    public class ValidKeyValueClaimExtractionConfig : IValidClaimsExtractionConfig
    {
        private KeyValueClaimExtractionConfig.KeyValueExtractionAsync _extract;
        private string _keyName;
        private string _claimName;

        /// <summary>
        /// creates a new <see cref="ValidKeyValueClaimExtractionConfig"/>
        /// </summary>
        /// <param name="func">extraction function</param>
        /// <param name="key">key</param>
        /// <param name="location"><see cref="ClaimLocation"/> location of claim</param>
        /// <param name="claimName">name of claim</param>
        public ValidKeyValueClaimExtractionConfig(KeyValueClaimExtractionConfig.KeyValueExtractionAsync func, string key, ClaimLocation location, string claimName)
        {
            _extract = func;
            _keyName = key;
            ClaimLocation = location;
            _claimName = claimName;
        }

        public ExtractionType ExtractionType => ExtractionType.KeyValue;
        public ClaimLocation ClaimLocation { get; }

        /// <summary>
        /// extracts and returns claim
        /// </summary>
        /// <param name="content"><see cref="List{KeyValuePair{string, List{object}}}"/> as JSON</param>
        /// <returns></returns>
        public async Task<Claim> GetClaimAsync(string content)
        {
            var contentDict = JsonConvert.DeserializeObject<List<KeyValuePair<string, List<object>>>>(content);
            var value = await _extract(contentDict, _keyName)
                .ConfigureAwait(false);
            return new Claim(_claimName, value);
        }
    }
}
