using System.Collections.Generic;
using MiddlewareAuth.Config.Claims.ExtractionConfigs;

namespace MiddlewareAuth.Config.Claims
{
    public class RouteClaimsConfig
    {
        public IList<IValidClaimsExtractionConfig> ExtractionConfigs { get; set; }
        public IList<ClaimValidationConfig> ValidationConfig { get; set; }
    }
}
