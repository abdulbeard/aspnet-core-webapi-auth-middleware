using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TokenAuth.Config
{
    public abstract class ClaimsExtractionConfig
    {
        public ClaimLocation Location { get; set; }
        protected ExtractionType ExtractionType { get; set; }
        public string ClaimName { get; set; }
        public abstract Task<Claim> GetClaimAsync(Type type = null, string content = null);
    }

    public class RegexClaimExtractionConfig : ClaimsExtractionConfig
    {
        protected Func<string, Regex, Task<Claim>> RegExExtraction;
        private Regex regex;
        public RegexClaimExtractionConfig()
        {
            ExtractionType = ExtractionType.RegEx;
        }

        public void ConfigureExtractionFunction(Func<string, Regex, Task<Claim>> func, Regex extractionRegex)
        {
            RegExExtraction = func;
            regex = extractionRegex;
        }

        public override Task<Claim> GetClaimAsync(Type type = null, string content = null)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            return RegExExtraction(content, regex);
        }
    }

    public class JsonPathClaimExtractionConfig : ClaimsExtractionConfig
    {
        private Func<string, string, Task<Claim>> JsonPathExtraction;
        private string path;
        public JsonPathClaimExtractionConfig()
        {
            base.ExtractionType = ExtractionType.JsonPath;
        }

        public void ConfigureExtractionFunction(Func<string, string, Task<Claim>> func, string jsonPath)
        {
            JsonPathExtraction = func;
            this.path = jsonPath;
        }

        public override Task<Claim> GetClaimAsync(Type type = null, string content = null)
        {
            if (content == null)
            {
                return null;
            }
            return JsonPathExtraction(content, path);
        }
    }

    public class TypeClaimExtractionConfig<T> : ClaimsExtractionConfig
    {
        private Func<T, Task<Claim>> TypeExtraction;

        public TypeClaimExtractionConfig()
        {
            base.ExtractionType = ExtractionType.Type;
        }

        public void ConfigureExtractionFunction(Func<T, Task<Claim>> func)
        {
            TypeExtraction = func;
        }

        public override Task<Claim> GetClaimAsync(Type type = null, string content = null)
        {
            if (type == null || TypeExtraction == null)
            {
                return null;
            }
            return TypeExtraction(JsonConvert.DeserializeObject<T>(content));
        }
    }

    public class ClaimsValidationConfig
    {
        public bool IsRequired { get; set; }
        public string ClaimName { get; set; }
    }

    public class RouteClaimsConfig
    {
        public IList<ClaimsExtractionConfig> ExtractionConfigs { get; set; }
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
