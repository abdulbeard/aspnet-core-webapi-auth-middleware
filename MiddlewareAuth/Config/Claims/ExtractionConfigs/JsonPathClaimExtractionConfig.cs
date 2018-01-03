using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiddlewareAuth.Config.Claims.ExtractionConfigs
{
    public class JsonPathClaimExtractionConfig : ClaimsExtractionConfig
    {
        private Func<string, string, Task<string>> _jsonPathExtraction;
        private string _path;
        public JsonPathClaimExtractionConfig(string claimName) : base(claimName)
        {
            ClaimName = claimName;
            Location = ClaimLocation.Body;
            ExtractionType = ExtractionType.JsonPath;
        }

        /// <summary>
        /// Takes in a function and extractionPath and configures this <see cref="JsonPathClaimExtractionConfig"/> to use these arguments for claim extraction
        /// </summary>
        /// <param name="func">takes in json as 1st arg, extraction path as 2nd arg, and returns the value of json extraction</param>
        /// <param name="extractionPath">the path at which the desired value is found in the json. Standard jsonPath syntax used as accepted by JSON.NET i.e. https://www.newtonsoft.com/json/help/html/QueryJsonSelectTokenJsonPath.htm </param>
        /// <returns></returns>
        public ClaimsExtractionConfig ConfigureExtraction(Func<string, string, Task<string>> func, string extractionPath)
        {
            _jsonPathExtraction = func;
            _path = extractionPath;
            return this;
        }

        public override IValidClaimsExtractionConfig Build()
        {
            if (_jsonPathExtraction == null)
            {
                throw new ArgumentException($"Extraction function can't be null. Use {nameof(ConfigureExtraction)} method to configure it first.");
            }
            if (string.IsNullOrEmpty(_path))
            {
                throw new ArgumentException($"{nameof(_path)} can't be null or empty. Use {nameof(ConfigureExtraction)} method to configure it first.");
            }
            return new ValidJsonPathClaimExtractionConfig(_path, _jsonPathExtraction, ClaimName, Location);
        }
    }

    public class ValidJsonPathClaimExtractionConfig : IValidClaimsExtractionConfig
    {
        private readonly Func<string, string, Task<string>> _jsonPathExtraction;
        private readonly string _path;
        private readonly string _claimName;
        public ValidJsonPathClaimExtractionConfig(string path, Func<string, string, Task<string>> jsonPathExtraction, string claimName, ClaimLocation location)
        {
            _path = path;
            _jsonPathExtraction = jsonPathExtraction;
            _claimName = claimName;
            ClaimLocation = location;
        }

        public ExtractionType ExtractionType => ExtractionType.JsonPath;
        public ClaimLocation ClaimLocation { get; }

        public async Task<Claim> GetClaimAsync(string content)
        {
            if (content == null)
            {
                return null;
            }
            var claimValue = await _jsonPathExtraction(content, _path).ConfigureAwait(false);
            return new Claim(_claimName, claimValue);
        }
    }
}
