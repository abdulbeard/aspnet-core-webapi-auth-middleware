using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace MiddlewareAuth.Config.Claims.ExtractionConfigs
{
    public class KeyValueClaimExtractionConfig : ClaimsExtractionConfig
    {
        private Func<List<KeyValuePair<string, List<object>>>, string, Task<string>> _keyValueExtraction;
        private string _keyName;

        public KeyValueClaimExtractionConfig(string claimName, ClaimLocation location) : base(claimName)
        {
            ClaimName = claimName;
            Location = location;
            ExtractionType = ExtractionType.KeyValue;
        }

        public ClaimsExtractionConfig ConfigureExtraction(Func<List<KeyValuePair<string, List<object>>>, string, Task<string>> func, string key)
        {
            _keyValueExtraction = func;
            _keyName = key;
            return this;
        }

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
    }

    public class ValidKeyValueClaimExtractionConfig : IValidClaimsExtractionConfig
    {
        private Func<List<KeyValuePair<string, List<object>>>, string, Task<string>> _keyValueExtraction;
        private string _keyName;
        private string _claimName;

        public ValidKeyValueClaimExtractionConfig(Func<List<KeyValuePair<string, List<object>>>, string, Task<string>> func, string key, ClaimLocation location, string claimName)
        {
            _keyValueExtraction = func;
            _keyName = key;
            ClaimLocation = location;
            _claimName = claimName;
        }

        public ExtractionType ExtractionType => ExtractionType.KeyValue;
        public ClaimLocation ClaimLocation { get; }

        public async Task<Claim> GetClaimAsync(string content)
        {
            var contentDict = JsonConvert.DeserializeObject<List<KeyValuePair<string, List<object>>>>(content);
            var value = await _keyValueExtraction(contentDict, _keyName)
                .ConfigureAwait(false);
            return new Claim(_claimName, value);
        }
    }
}
