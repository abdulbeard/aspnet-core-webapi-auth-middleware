using MiddlewareAuth.Config.Routing;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using MiddlewareAuth.Config.Claims;
using MiddlewareAuth.Config.Claims.ExtractionConfigs;
using MiddlewareAuth.Utils;
using TokenAuth.Models;
using MiddlewareAuth.Config;
using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using System.IO;

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
        public const string PostMoralValues = "moralValues/{subscriberId}/{kiwiChant}";
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
                                .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.AltruisticAptitude.AltruisticAlignment").Build(),
                            new JsonPathClaimExtractionConfig(JwtRegisteredClaimNames.Email)
                                .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.ReportViolationEmail").Build(),
                            new KeyValueClaimExtractionConfig("hukaChaka1", ClaimLocation.QueryParameters).ConfigureExtraction(ExtractionFunctions.KeyValueFunc, "chandKara").Build(),
                            new KeyValueClaimExtractionConfig("hukaChaka2", ClaimLocation.Uri).ConfigureExtraction(ExtractionFunctions.KeyValueFunc, "kiwiChant").Build(),
                            new KeyValueClaimExtractionConfig("hukaChaka3", ClaimLocation.Headers).ConfigureExtraction(ExtractionFunctions.KeyValueFunc, "hukaChaka").Build(),
                            //make regex for the path only, not including the query parameters. For query params, use KeyValueClaimExtractionConfig instead
                            new RegexClaimExtractionConfig("hookaRegex", ClaimLocation.Uri).ConfigureExtraction(ExtractionFunctions.RegexFunc,
                                new System.Text.RegularExpressions.Regex("/values/moralvalues/ca413986-f096-11e7-8c3f-9a214cf093ae/(.*)")).Build()
                        },
                        ValidationConfig = new List<ClaimValidationConfig>()
                        {
                            new ClaimValidationConfig()
                            {
                                ClaimName = "AltruisticAlignment",
                                IsRequired = true
                            },
                            new ClaimValidationConfig()
                            {
                                ClaimName = JwtRegisteredClaimNames.Email,
                                IsRequired = true,
                                AllowNullOrEmpty = false
                            }
                        },
                        MissingClaimsResponse = new MissingClaimsResponse
                        {
                            HttpStatusCode = System.Net.HttpStatusCode.Forbidden,
                            Response = GetMissingClaimsResponse(),
                            //MissingClaimsResponseOverride = (missingClaims) =>
                            //{
                            //    return Task.FromResult(GetSampleResponse());
                            //}
                        }
                    }
                }
            };
        }

        private dynamic GetMissingClaimsResponse()
        {
            dynamic result = new ExpandoObject();
            result.ErrorCode = 2500;
            result.Message = "Somebody gonna get hurt real bad";
            return result;
        }

        private HttpResponse GetSampleResponse()
        {
            var response = new DefaultHttpResponse(new DefaultHttpContext());
            response.Body = new MemoryStream();
            var responseBytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { Yolo = "nolo" }));
            response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
            response.Headers.Add(new KeyValuePair<string, StringValues>("yolo", new StringValues("solo")));
            response.StatusCode = StatusCodes.Status502BadGateway;
            return response;
        }
    }
}
