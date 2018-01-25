using Microsoft.Extensions.Caching.Memory;
using MiddlewareAuth.Config;
using MiddlewareAuth.Config.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareAuth.Cache
{
    public class ValidRouteDefinitionProvider : IValidRouteDefinitionProvider
    {
        private IMemoryCache objectCache;
        public ValidRouteDefinitionProvider(IMemoryCache memoryCache)
        {
            objectCache = memoryCache;
        }

        public async Task<IEnumerable<IRouteDefinitions>> GetAsync()
        {
            return null;
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

        private Task<string> getString()
        {
            return Task.FromResult("superDuperExpensiveToCreateString");
        }
    }
}
