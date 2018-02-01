using System;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;

namespace MisturTee.Config.Claims.ExtractionConfigs
{
    public abstract class ClaimsExtractionConfig
    {
        protected ClaimsExtractionConfig(string claimName)
        {
            if (string.IsNullOrEmpty(claimName))
            {
                throw new ArgumentNullException($"{nameof(claimName)} can't be empty or null.");
            }
            ClaimName = claimName;
        }
        public ClaimLocation Location { get; protected set; }
        protected ExtractionType ExtractionType { get; set; }
        public string ClaimName { get; protected set; }
        public abstract IValidClaimsExtractionConfig Build();
    }
}
