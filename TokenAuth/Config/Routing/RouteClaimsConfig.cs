using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenAuth.Config.Routing
{
    public class RouteClaimsConfig
    {
        public IList<ClaimsExtractionConfig> ExtractionConfigs { get; set; }
        public ClaimsValidationConfig ValidationConfig { get; set; }
    }
}
