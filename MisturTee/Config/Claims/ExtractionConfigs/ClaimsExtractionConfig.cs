using System;
using System.Security.Claims;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;

namespace MisturTee.Config.Claims.ExtractionConfigs
{
    /// <summary>
    /// Defines the configuration needed to extract a claim from an http request
    /// </summary>
    public abstract class ClaimsExtractionConfig
    {
        /// <summary>
        /// Is only for Json purposes
        /// </summary>
        protected ClaimsExtractionConfig(){}

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="claimName">what to name the claim once it's extracted - not be null or empty</param>
        protected ClaimsExtractionConfig(string claimName)
        {
            if (string.IsNullOrEmpty(claimName))
            {
                throw new ArgumentNullException($"{nameof(claimName)} can't be empty or null.");
            }
            ClaimName = claimName;
        }

        /// <summary>
        /// Defines where the claim is in the http request. <see cref="ClaimLocation"/>
        /// </summary>
        protected ClaimLocation Location { get; set; }

        /// <summary>
        /// Defines the method to use when extracting the claim. <see cref="Claims.ExtractionType"/>
        /// </summary>
        protected ExtractionType ExtractionType { get; set; }

        /// <summary>
        /// The value of <see cref="Claim.Type"/> for the extracted claim
        /// </summary>
        protected string ClaimName { get; set; }

        /// <summary>
        /// Builds and returns a validated claims extraction config.
        /// </summary>
        /// <returns>an instance of <see cref="IValidClaimsExtractionConfig"/> which has been checked for validity, and will execute successfully</returns>
        public abstract IValidClaimsExtractionConfig Build();
    }
}
