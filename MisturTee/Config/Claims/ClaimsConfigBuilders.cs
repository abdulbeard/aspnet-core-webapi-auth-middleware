using MisturTee.Config.Claims.ExtractionConfigs.Valid;

namespace MisturTee.Config.Claims
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
