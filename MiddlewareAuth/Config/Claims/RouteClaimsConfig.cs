using MiddlewareAuth.Config.Claims;
using System.Collections.Generic;

namespace MiddlewareAuth.Config.Routing
{
    public class RouteClaimsConfig
    {
        public IList<ClaimsExtractionConfig> ExtractionConfigs { get; set; }
        public IList<ClaimsValidationConfig> ValidationConfig { get; set; }
    }
}
