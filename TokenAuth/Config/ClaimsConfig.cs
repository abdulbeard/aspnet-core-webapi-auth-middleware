using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TokenAuth.Config
{
    public abstract class ClaimsExtractionConfig
    {
        public ClaimLocation Location { get; set; }
        public ExtractionType ExtractionType { get; set; }
        public string ClaimName { get; set; }
        private Func<Type, Task<Claim>> TypeExtraction { get; set; }
        private Func<string, string, Task<Claim>> JsonPathExtraction { get; set; }
        private Func<string, Regex, Task<Claim>> RegExExtraction { get; set; }
        public abstract Task<Claim> GetClaimAsync(Type type = null, string content = null);

        private Task<IList<Claim>> GetClaim(HttpRequestMessage request)
        {
            return null;
        }
    }

    public class RegexClaimExtractionConfig : ClaimsExtractionConfig
    {
        public RegexClaimExtractionConfig()
        {
            base.ExtractionType = ExtractionType.RegEx;
        }

        public override Task<Claim> GetClaimAsync(Type type = null, string content = null)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException();
            }
            throw new NotImplementedException();
        }
    }

    public class JsonPathClaimExtractionConfig : ClaimsExtractionConfig
    {
        public JsonPathClaimExtractionConfig()
        {
            base.ExtractionType = ExtractionType.JsonPath;
        }
    }

    public class TypeClaimExtractionConfig : ClaimsExtractionConfig
    {
        public TypeClaimExtractionConfig()
        {
            base.ExtractionType = ExtractionType.Type;
        }
    }

    public class ClaimsValidationConfig
    {
        public bool IsRequired { get; set; }
        public string ClaimName { get; set; }
    }

    public class RouteClaimsConfig
    {
        public ClaimsExtractionConfig ExtractionConfig { get; set; }
        public ClaimsValidationConfig ValidationConfig { get; set; }
    }

    public enum ClaimLocation
    {
        None = 0,
        Headers = 1,
        Body = 2,
        Uri = 3,
        QueryParameters = 4
    }

    public enum ExtractionType
    {
        None = 0,
        Type = 1,
        JsonPath = 2,
        RegEx = 3
    }
}
