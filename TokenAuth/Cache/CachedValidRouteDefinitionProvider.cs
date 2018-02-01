using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MisturTee.Config;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using MisturTee.Config.Routing;

namespace TokenAuth.Cache
{
    public class CachedValidRouteDefinitionProvider : IValidRouteDefinitionProvider
    {
        private readonly IMemoryCache _objectCache;
        public CachedValidRouteDefinitionProvider(IMemoryCache memoryCache)
        {
            _objectCache = memoryCache;
        }

        public async Task<IEnumerable<IRouteDefinitions>> GetAsync()
        {
            return await GetRouteDefinitionsFromCacheAsync().ConfigureAwait(false);
        }

        private Task<IEnumerable<IRouteDefinitions>> GetRouteDefinitionsFromCacheAsync()
        {
            var result = _objectCache.GetOrCreate("supahDupahExpensiveStuff", (entry) =>
            {
                entry.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(1);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                entry.SlidingExpiration = TimeSpan.FromMinutes(1);
                return GetRouteDefinitions();
            });
            return Task.FromResult(result);
        }

        private IEnumerable<IRouteDefinitions> GetRouteDefinitions()
        {
            var routeDefs = new List<SerializableRouteDefinition>();
            dynamic response = new ExpandoObject();
            response.ErrorCode = 2500;
            response.Message = "Somebody gonna get hurt real bad";
            routeDefs.Add(new SerializableRouteDefinition(new List<SerializableRouteConfig>
            {
                new SerializableRouteConfig
                {
                    HttpMethod = "POST",
                    RouteTemplate = "values/moralValues/{subscriberId}/{kiwiChant}",
                    ClaimsConfig = new SerializableRouteClaimsConfig
                    {
                        BadRequestResponse = new BadRequestResponse
                        {
                            HttpStatusCode = System.Net.HttpStatusCode.Forbidden,
                            Response = response,
                            Headers = new HeaderDictionary()
                        },
                        ExtractionConfigs = new List<IValidClaimsExtractionConfig>
                        {
                            new SerializableClaimsExtractionConfig("ReportViolationEmail")
                            {
                                ClaimLocation = ClaimLocation.Body,
                                ExtractionStrategem = SerializableExtractionType.JsonPath,
                                Path = "$.ReportViolationEmail"
                            }.Build(),
                            new SerializableClaimsExtractionConfig("superSecretId")
                            {
                                ClaimLocation = ClaimLocation.Body,
                                ExtractionStrategem = SerializableExtractionType.JsonPath,
                                Path = "$.SuperSecretId"
                            }.Build()
                        },
                        ValidationConfigs = new List<ClaimValidationConfig>
                        {
                            new ClaimValidationConfig()
                            {
                                ClaimName = "ReportViolationEmail",
                                IsRequired = true,
                                AllowNullOrEmpty = false,
                                ValueMustBeExactMatch = true
                            },
                            new ClaimValidationConfig()
                            {
                                ClaimName = "superSecretId",
                                IsRequired = true,
                                AllowNullOrEmpty = false,
                                ValueMustBeExactMatch = true
                            }
                        }
                    }
                }
            }));
            return routeDefs;
        }
    }
}
