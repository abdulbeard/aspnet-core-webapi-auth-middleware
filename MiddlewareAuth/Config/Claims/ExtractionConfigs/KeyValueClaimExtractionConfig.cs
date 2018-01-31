using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MiddlewareAuth.Config.Claims.ExtractionConfigs.Valid;

namespace MiddlewareAuth.Config.Claims.ExtractionConfigs
{
    /// <summary>
    /// Used on a list of <see cref="KeyValuePair{string, List{object}}"/> to extract a claim
    /// </summary>
    public class KeyValueClaimExtractionConfig : ClaimsExtractionConfig
    {
        private KeyValueExtractionAsync _keyValueExtraction;
        private string _keyName;

        /// <summary>
        /// creates a new <see cref="KeyValueClaimExtractionConfig"/>
        /// </summary>
        /// <param name="claimName">name of the claim</param>
        /// <param name="location"><see cref="ClaimLocation"/> location of the claim</param>
        public KeyValueClaimExtractionConfig(string claimName, ClaimLocation location) : base(claimName)
        {
            ClaimName = claimName;
            Location = location;
            ExtractionType = ExtractionType.KeyValue;
        }

        /// <summary>
        /// configures this <see cref="KeyValueClaimExtractionConfig"/> with what's needed for extraction
        /// </summary>
        /// <param name="func"><see cref="KeyValueExtractionAsync"/> function that extracts claim value</param>
        /// <param name="key">key to be used to get the value from <see cref="KeyValuePair{string, List{object}}"/></param>
        /// <returns></returns>
        public ClaimsExtractionConfig ConfigureExtraction(KeyValueExtractionAsync func, string key)
        {
            _keyValueExtraction = func;
            _keyName = key;
            return this;
        }

        /// <summary>
        /// checks for validity and returns a valid <see cref="IValidClaimsExtractionConfig"/>
        /// </summary>
        /// <returns></returns>
        public override IValidClaimsExtractionConfig Build()
        {
            if (_keyValueExtraction == null)
            {
                throw new ArgumentException($"Extraction function can't be null. Use {nameof(ConfigureExtraction)} method to configure it first.");
            }
            if (string.IsNullOrEmpty(_keyName))
            {
                throw new ArgumentException($"{nameof(_keyName)} can't be null or empty. Use {nameof(ConfigureExtraction)} method to configure it first.");
            }
            return new ValidKeyValueClaimExtractionConfig(_keyValueExtraction, _keyName, Location, ClaimName);
        }

        /// <summary>
        /// Delegate that takes in a <see cref="List{KeyValuePair{string, List{object}}"/> and a <see cref="string"/> key and returns the extracted claim value
        /// </summary>
        /// <param name="dictionary"><see cref="List{KeyValuePair{string, List{object}}"/></param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public delegate Task<string> KeyValueExtractionAsync(List<KeyValuePair<string, List<object>>> dictionary, string key);
    }
}
