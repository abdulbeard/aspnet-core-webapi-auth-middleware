using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using MiddlewareAuth.Config;
using MiddlewareAuth.Config.Claims;
using MiddlewareAuth.Config.Claims.ExtractionConfigs;
using MiddlewareAuth.Config.Claims.ExtractionConfigs.Valid;
using MiddlewareAuth.Config.Routing;

namespace TokenAuth
{
    public class SerializableRouteDefinition : IRouteDefinitions
    {
        public SerializableRouteDefinition(List<SerializableRouteConfig> routes)
        {
            _routes = routes;
        }

        private List<SerializableRouteConfig> _routes;

        public List<RouteDefinition> GetRouteDefinitions()
        {
            return _routes?.Select(x => (RouteDefinition)x).ToList() ?? new List<RouteDefinition>();
        }
    }

    public class SerializableRouteConfig : RouteDefinition
    {
        public string HttpMethod;
        public override HttpMethod Method()
        {
            return new HttpMethod(HttpMethod);
        }

        public override Type RequestBody()
        {
            return typeof(string);
        }

        public new SerializableRouteClaimsConfig ClaimsConfig { get; set; }
    }

    public class SerializableRouteClaimsConfig : RouteClaimsConfig
    {
        public new IList<SerializableClaimsExtractionConfig> ExtractionConfigs { get; set; }
    }

    public class SerializableClaimsExtractionConfig : ClaimsExtractionConfig
    {
        private Regex _parsedRegex;

        public SerializableExtractionType ExtractionStrategem { get; set; }
        public ClaimLocation ClaimLocation { get; set; }
        public string Path { get; set; }
        public string Regex { get; set; }
        public string KeyName { get; set; }

        public new ExtractionType ExtractionType
        {
            get
            {
                switch (ExtractionStrategem)
                {
                    case SerializableExtractionType.JsonPath:
                        return ExtractionType.JsonPath;
                    case SerializableExtractionType.KeyValue:
                        return ExtractionType.KeyValue;
                    case SerializableExtractionType.RegEx:
                        return ExtractionType.RegEx;
                    default:
                        return ExtractionType.None;
                }
            }
        }

        public SerializableClaimsExtractionConfig(string claimName) : base(claimName)
        {
        }

        public override IValidClaimsExtractionConfig Build()
        {
            switch (ExtractionStrategem)
            {
                case SerializableExtractionType.JsonPath:
                    return new ValidJsonPathClaimExtractionConfig(Path, ExtractionFunctions.JsonPathFunc, ClaimName,
                        ClaimLocation);
                case SerializableExtractionType.KeyValue:
                    return new ValidKeyValueClaimExtractionConfig(ExtractionFunctions.KeyValueFunc, KeyName, ClaimLocation,
                        ClaimName);
                case SerializableExtractionType.RegEx:
                    return new ValidRegexClaimExtractionConfig(ExtractionFunctions.RegexFunc, _parsedRegex, ClaimName,
                        ClaimLocation);
                default:
                    return null;
            }
        }

        public bool IsValid()
        {
            return ExtractionType != ExtractionType.None &&
                   !(string.IsNullOrEmpty(Path) && string.IsNullOrEmpty(Regex) && string.IsNullOrEmpty(KeyName)) &&
                   (!string.IsNullOrEmpty(Regex) && ParseRegex(Regex)) && ClaimLocation != ClaimLocation.None && !string.IsNullOrEmpty(ClaimName);
        }

        private bool ParseRegex(string regexString)
        {
            try
            {
                _parsedRegex = new Regex(regexString);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
