using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MiddlewareAuth.Config.Claims.ExtractionConfigs
{
    public class TypeClaimExtractionConfig<T> : ClaimsExtractionConfig
    {
        private Func<T, Task<string>> _typeExtraction;

        public TypeClaimExtractionConfig(string claimName) : base(claimName)
        {
            ClaimName = claimName;
            Location = ClaimLocation.Body;
            ExtractionType = ExtractionType.Type;
        }

        public void ConfigureExtraction(Func<T, Task<string>> func)
        {
            _typeExtraction = func;
        }

        public override IValidClaimsExtractionConfig Build()
        {
            if (_typeExtraction == null)
            {
                throw new ArgumentException($"Extraction function can't be null. Use {nameof(ConfigureExtraction)} method to configure it first.");
            }
            return new ValidTypeClaimExtractionConfig<T>(_typeExtraction, ClaimName);
        }
    }

    public class ValidTypeClaimExtractionConfig<T> : IValidClaimsExtractionConfig
    {
        private readonly Func<T, Task<string>> _typeExtraction;
        private readonly string _claimName;

        public ValidTypeClaimExtractionConfig(Func<T, Task<string>> func, string claim)
        {
            _typeExtraction = func;
            _claimName = claim;
        }

        public async Task<Claim> GetClaimAsync(Type type = null, string content = null)
        {
            if (type == null || _typeExtraction == null)
            {
                return null;
            }
            var value = await _typeExtraction(JsonConvert.DeserializeObject<T>(content)).ConfigureAwait(false);
            return new Claim(_claimName, value);
        }
    }
}
