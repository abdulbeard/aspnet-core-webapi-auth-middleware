using MiddlewareAuth.Config.Claims.ExtractionConfigs;

namespace MiddlewareAuth.Config.Claims
{
    public static class ClaimsConfigBuilders
    {
        public static RouteClaimsConfig AddExtractorConfig(this RouteClaimsConfig config, IValidClaimsExtractionConfig extractionConfig)
        {
            //TODO: Add config.ExtractionConfigs null check
            config.ExtractionConfigs.Add(extractionConfig);
            return config;
        }
    }
}
