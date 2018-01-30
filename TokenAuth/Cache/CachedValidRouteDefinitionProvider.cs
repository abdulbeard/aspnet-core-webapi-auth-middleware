using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MiddlewareAuth.Config;
using MiddlewareAuth.Config.Claims;
using MiddlewareAuth.Config.Routing;

namespace TokenAuth.Cache
{
    public class CachedValidRouteDefinitionProvider : IValidRouteDefinitionProvider
    {
        private IMemoryCache objectCache;
        public CachedValidRouteDefinitionProvider(IMemoryCache memoryCache)
        {
            objectCache = memoryCache;
        }

        public async Task<IEnumerable<IRouteDefinitions>> GetAsync()
        {
            return await GetRouteDefinitions().ConfigureAwait(false);
        }

        private async Task<string> RefreshCache(ICacheEntry cacheEntry)
        {
            cacheEntry.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(1);
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(1);
            cacheEntry.RegisterPostEvictionCallback((key, value, evictionReason, state) =>
            {

            });
            return await GetCacheString().ConfigureAwait(false);
        }

        private async Task<IEnumerable<IRouteDefinitions>> GetRouteDefinitions()
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
                        MissingClaimsResponse =  new MissingClaimsResponse
                        {
                            HttpStatusCode = System.Net.HttpStatusCode.Forbidden,
                            Response =  response,
                            Headers = new HeaderDictionary()
                        },
                        ExtractionConfigs = new List<SerializableClaimsExtractionConfig>
                        {
                            new SerializableClaimsExtractionConfig("AltruisticAlignment")
                        },
                        ValidationConfig = new List<ClaimValidationConfig>
                        {
                            new ClaimValidationConfig()
                            {
                                ClaimName = "AltruisticAlignment",
                                IsRequired = true
                            }                        
                        }
                    }
                }
            }));
            return routeDefs;
        }

        public async Task<string> GetCacheString()
        {
            var result = await objectCache.GetOrCreateAsync("yolo", (entry) =>
            {
                entry.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(1);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                entry.SlidingExpiration = TimeSpan.FromMinutes(1);
                return Task.FromResult("superDuperExpensiveToCreateString");
            });
            return result;
        }
    }
}
