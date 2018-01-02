using MiddlewareAuth.Config.Routing;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using MiddlewareAuth.Config.Claims;
using MiddlewareAuth.Config.Claims.ExtractionConfigs;
using MiddlewareAuth.Utils;
using TokenAuth.Models;

namespace TokenAuth.Routes
{
    public class ValuesRoutes : IRouteDefinitions
    {
        public const string Prefix = "values";
        public const string Get = "";
        public const string GetById = "{id}";
        public const string GetByIdGuid = "{id:guid}/yolo/{manafort}/sup/{manaforts:guid}";
        public const string Post = "";
        public const string Put = "{id}";
        public const string Delete = "{id}";
        public const string PostMoralValues = "moralValues/{subscriberId}";
        private const string pathSeparator = "/";

        public List<RouteDefinition> GetRouteDefinitions()
        {
            return new List<RouteDefinition>
            {
                new PostRouteDefinition(typeof(string))
                {
                    RouteTemplate = $"{Prefix}{pathSeparator}{Post}",
                },
                new GetRouteDefinition
                {
                    RouteTemplate = $"{Prefix}{pathSeparator}{GetByIdGuid}"
                },
                new PostRouteDefinition(typeof(MoralValues))
                {
                    RouteTemplate = $"{Prefix}{pathSeparator}{PostMoralValues}",
                    ClaimsConfig = new RouteClaimsConfig()
                    {
                        ExtractionConfigs = new List<IValidClaimsExtractionConfig>()
                        {
                            new JsonPathClaimExtractionConfig("AltruisticAlignment")
                                .ConfigureExtraction((body, path) =>
                                {
                                    var jsonUtils = new JsonUtils();
                                    var extractedValues = jsonUtils.GetValueFromJson(body, path);
                                    return Task.FromResult(extractedValues?.First());
                                }, "$.AltruisticAptitude.AltruisticAlignment").Build(),
                            new JsonPathClaimExtractionConfig(JwtRegisteredClaimNames.Email)
                                .ConfigureExtraction((body, path) =>
                                {
                                    var jsonUtils = new JsonUtils();
                                    var extractedValues = jsonUtils.GetValueFromJson(body, path);
                                    return Task.FromResult(extractedValues?.First());
                                }, "$.ReportViolationEmail").Build()
                        },
                        ValidationConfig = new List<ClaimsValidationConfig>()
                        {
                            new ClaimsValidationConfig()
                            {
                                ClaimName = "AltruisticAlignment",
                                IsRequired = true
                            }
                        }
                    }
                }
            };
        }
    }
}
