using MiddlewareAuth.Config.Routing;
using System;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiddlewareAuth.Config
{
    public static class ClaimsConfigBuilders
    {
        public static ClaimsExtractionConfig ExtractorByPath(this JsonPathClaimExtractionConfig config,
            Func<string, string, Task<Claim>> func, string extractionPath)
        {
            if (string.IsNullOrEmpty(extractionPath))
            {
                throw new ArgumentNullException(extractionPath);
            }
            config.ConfigureExtractionFunction(func, extractionPath);
            return config;
        }

        public static ClaimsExtractionConfig ExtractorByRegularExpression(this RegexClaimExtractionConfig config,
            Func<string, Regex, Task<Claim>> func, Regex extractionRegex)
        {
            return config;
        }


        public static ClaimsExtractionConfig ExtractorByType<T>(this TypeClaimExtractionConfig<T> config,
            Func<T, Task<Claim>> func, Type type)
        {
            return config;
        }

        public static RouteClaimsConfig AddExtractorConfig(this RouteClaimsConfig config, ClaimsExtractionConfig extractionConfig)
        {
            //TODO: Add config.ExtractionConfigs null check
            config.ExtractionConfigs.Add(extractionConfig);
            return config;
        }

        public static ClaimsExtractionConfig Build(this ClaimsExtractionConfig config)
        {
            return config;
        }
    }
}
