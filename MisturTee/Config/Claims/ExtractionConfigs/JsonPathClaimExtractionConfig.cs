using System;
using System.Threading.Tasks;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;

namespace MisturTee.Config.Claims.ExtractionConfigs
{
    /// <inheritdoc />
    /// <summary>
    /// Defines configuration for extracting claim value from json using jsonPath
    /// </summary>
    public class JsonPathClaimExtractionConfig : ClaimsExtractionConfig
    {
        private ExtractValueByJsonPathAsync _extractionFunc;
        private string _path;

        /// <inheritdoc />
        /// <summary>
        /// creates new <see cref="JsonPathClaimExtractionConfig" />
        /// </summary>
        /// <param name="claimName">name of the claim to be extracted</param>
        public JsonPathClaimExtractionConfig(string claimName) : base(claimName)
        {
            ClaimName = claimName;
            Location = ClaimLocation.Body;
            ExtractionType = ExtractionType.JsonPath;
        }

        /// <summary>
        /// Takes in a delegate of type <see cref="ExtractValueByJsonPathAsync" /> and extractionPath,
        ///  and configures this <see cref="JsonPathClaimExtractionConfig"/> to use these arguments for claim extraction
        /// </summary>
        /// <param name="func"><see cref="ExtractValueByJsonPathAsync"/></param>
        /// <param name="extractionPath">the jsonPath to be used for extraction </param>
        /// <returns></returns>
        public ClaimsExtractionConfig ConfigureExtraction(ExtractValueByJsonPathAsync func, string extractionPath)
        {
            _extractionFunc = func;
            _path = extractionPath;
            return this;
        }

        /// <inheritdoc />
        /// <summary>
        /// Builds <see cref="JsonPathClaimExtractionConfig" /> into a <see cref="IValidClaimsExtractionConfig" /> after checking for validity
        /// </summary>
        /// <returns><see cref="IValidClaimsExtractionConfig" /></returns>
        public override IValidClaimsExtractionConfig Build()
        {
            if (_extractionFunc == null)
            {
                throw new ArgumentException($"Extraction function can't be null. Use {nameof(ConfigureExtraction)} method to configure it first.");
            }
            if (string.IsNullOrEmpty(_path))
            {
                throw new ArgumentException($"{nameof(_path)} can't be null or empty. Use {nameof(ConfigureExtraction)} method to configure it first.");
            }
            return new ValidJsonPathClaimExtractionConfig(_path, _extractionFunc, ClaimName, Location);
        }

        /// <summary>
        /// This delegate takes in json as 1st arg, jsonPath as 2nd arg, and returns the value of json extraction
        /// </summary>
        /// <param name="json">raw json</param>
        /// <param name="jsonPath">the path at which the desired value is found in the json. 
        /// Standard jsonPath syntax used as accepted by JSON.NET i.e. https://www.newtonsoft.com/json/help/html/QueryJsonSelectTokenJsonPath.htm </param>
        /// <returns>string value of the json path extraction</returns>
        public delegate Task<string> ExtractValueByJsonPathAsync(string json, string jsonPath);
    }
}
