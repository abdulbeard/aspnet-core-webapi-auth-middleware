using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using MisturTee.Config;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using MisturTee.Config.Routing;
using TokenAuth.Extractors;

namespace TokenAuth
{
    public class SerializableRouteDefinition : IRouteDefinitions
    {
        public SerializableRouteDefinition(List<SerializableRouteConfig> routes)
        {
            _routes = routes;
        }

        public List<RouteDefinition> Routes => GetRouteDefinitions();

        private List<SerializableRouteConfig> _routes;

        public List<RouteDefinition> GetRouteDefinitions()
        {
            return _routes?.Select(x => (RouteDefinition)x).ToList() ?? new List<RouteDefinition>();
        }
    }

    public class SerializableRouteConfig : RouteDefinition
    {
        private SerializableRouteClaimsConfig _serializableRouteClaimsConfig;
        public string HttpMethod;
        public override HttpMethod Method()
        {
            return new HttpMethod(HttpMethod);
        }

        public override Type RequestBody()
        {
            return typeof(string);
        }

        public new SerializableRouteClaimsConfig ClaimsConfig
        {
            get => _serializableRouteClaimsConfig;
            set
            {
                _serializableRouteClaimsConfig = value;
                base.ClaimsConfig = value;
            }
        }
    }

    public class SerializableRouteClaimsConfig : RouteClaimsConfig
    {
        private IList<SerializableClaimsExtractionConfig> _validClaimsExtractionConfigs;

        public new IList<SerializableClaimsExtractionConfig> ExtractionConfigs
        {
            get => _validClaimsExtractionConfigs;
            set
            {
                _validClaimsExtractionConfigs = value;
                base.ExtractionConfigs = value.Select(x => x.Build()).ToList();
            }
        }
    }

    public class SerializableClaimsExtractionConfig : ClaimsExtractionConfig
    {
        private Regex _parsedRegex;

        public new string ClaimName
        {
            get => base.ClaimName;
            set => base.ClaimName = value;
        }

        public SerializableExtractionType ExtractionStrategem { get; set; }
        public ClaimLocation ClaimLocation { get; set; }
        public string Path { get; set; }
        public string Regex { get; set; }
        public string KeyName { get; set; }

        private new ExtractionType ExtractionType
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
            if (!IsValid())
            {
                throw new ArgumentException($"This instance of {nameof(SerializableClaimsExtractionConfig)} is invalid");
            }
            switch (ExtractionStrategem)
            {
                case SerializableExtractionType.JsonPath:
                    return new ValidatedJsonPathExtractor(Path, ExtractionFunctions.JsonPathFunc, ClaimName,
                        ClaimLocation);
                case SerializableExtractionType.KeyValue:
                    return new ValidatedKeyValueExtractor(ExtractionFunctions.KeyValueFunc, KeyName, ClaimLocation,
                        ClaimName);
                case SerializableExtractionType.RegEx:
                    return new ValidatedRegExExtractor(ExtractionFunctions.RegexFunc, _parsedRegex, ClaimName,
                        ClaimLocation);
                default:
                    return null;
            }
        }

        private bool IsValid()
        {
            if (string.IsNullOrEmpty(ClaimName))
            {
                return false;
            }
            if (ExtractionType == ExtractionType.None)
            {
                return false;
            }
            if (string.IsNullOrEmpty(Path) && string.IsNullOrEmpty(Regex) && string.IsNullOrEmpty(KeyName))
            {
                return false;
            }
            if (ClaimLocation == ClaimLocation.None)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(KeyName))
            {
                ExtractionStrategem = SerializableExtractionType.KeyValue;
                return true;
            }
            if (!string.IsNullOrEmpty(Path))
            {
                ExtractionStrategem = SerializableExtractionType.JsonPath;
                return true;
            }
            if (!string.IsNullOrEmpty(Regex))
            {
                if (!ParseRegex(Regex) || ClaimLocation == ClaimLocation.Headers)
                {
                    return false;
                }
                ExtractionStrategem = SerializableExtractionType.RegEx;
            }
            return true;
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
