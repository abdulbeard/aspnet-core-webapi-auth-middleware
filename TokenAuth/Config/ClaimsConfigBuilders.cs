using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TokenAuth.Config
{
    public static class ClaimsConfigBuilders
    {
        public static ClaimsExtractionConfig ExtractorByPath(this JsonPathClaimExtractionConfig config,
            Func<string, string, Task<Claim>> func)
        {
            return config;
        }

        public static ClaimsExtractionConfig ExtractorByRegularExpression(this RegexClaimExtractionConfig config,
            Func<string, Regex, Task<Claim>> func)
        {
            return config;
        }


        public static ClaimsExtractionConfig ExtractorByType(this TypeClaimExtractionConfig config,
            Func<Type, Task<Claim>> func)
        {
            return config;
        }

        public static ClaimsExtractionConfig Build(this ClaimsExtractionConfig config)
        {
            return config;
        }
    }
}
