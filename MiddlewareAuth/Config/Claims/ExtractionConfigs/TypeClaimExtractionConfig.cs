using System;
using System.Threading.Tasks;
using MiddlewareAuth.Config.Claims.ExtractionConfigs.Valid;

namespace MiddlewareAuth.Config.Claims.ExtractionConfigs
{
    /// <summary>
    /// Helps you extract a claim from an object of type <see cref="{T}"/>
    /// </summary>
    /// <typeparam name="T">type of request entity</typeparam>
    public class TypeClaimExtractionConfig<T> : ClaimsExtractionConfig
    {
        private ExtractClaimForTypeAsync _typeExtraction;

        /// <summary>
        /// creates a new <see cref="TypeClaimExtractionConfig{T}"/>
        /// </summary>
        /// <param name="claimName">name of the claim to be extracted</param>
        public TypeClaimExtractionConfig(string claimName) : base(claimName)
        {
            ClaimName = claimName;
            Location = ClaimLocation.Body;
            ExtractionType = ExtractionType.Type;
        }

        /// <summary>
        /// Configures <see cref="this"/> by setting its extraction function />
        /// </summary>
        /// <param name="func">function used to extract claim value</param>
        public void ConfigureExtraction(ExtractClaimForTypeAsync func)
        {
            _typeExtraction = func;
        }

        /// <summary>
        /// Builds and returns a <see cref="IValidClaimsExtractionConfig"/> to be used for claim extraction
        /// </summary>
        /// <returns></returns>
        public override IValidClaimsExtractionConfig Build()
        {
            if (_typeExtraction == null)
            {
                throw new ArgumentException($"Extraction function can't be null. Use {nameof(ConfigureExtraction)} method to configure it first.");
            }
            return new ValidTypeClaimExtractionConfig<T>(_typeExtraction, ClaimName, Location);
        }

        /// <summary>
        /// This function takes in a <see cref="{T}"/> and returns the value of the extracted claim. <see cref="{T}"/> could be e.g. deserialized http request body
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public delegate Task<string> ExtractClaimForTypeAsync(T entity);
    }    
}
