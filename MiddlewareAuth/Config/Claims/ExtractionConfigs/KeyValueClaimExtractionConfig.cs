using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiddlewareAuth.Config.Claims.ExtractionConfigs
{
    public class KeyValueClaimExtractionConfig : ClaimsExtractionConfig
    {
        private Func<List<KeyValuePair<object, object>>, string, Task<string>> _keyValueExtraction;
        private string _keyName;

        public KeyValueClaimExtractionConfig(string claimName, ClaimLocation location) : base(claimName)
        {
            ClaimName = claimName;
            Location = location;
            ExtractionType = ExtractionType.KeyValue;
        }

        public ClaimsExtractionConfig ConfigureExtraction(Func<List<KeyValuePair<object, object>>, string, Task<string>> func, string key)
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
            return new ValidKeyValueClaimExtractionConfig(_keyValueExtraction, _keyName);
        }
    }

    public class ValidKeyValueClaimExtractionConfig : IValidClaimsExtractionConfig
    {
        private Func<List<KeyValuePair<object, object>>, string, Task<string>> _keyValueExtraction;
        private string _keyName;

        public ValidKeyValueClaimExtractionConfig(Func<List<KeyValuePair<object, object>>, string, Task<string>> func, string key)
        {
            _keyValueExtraction = func;
            _keyName = key;
        }
        public Task<Claim> GetClaimAsync(Type type = null, string content = null)
        {
            throw new NotImplementedException();
        }
    }
}
