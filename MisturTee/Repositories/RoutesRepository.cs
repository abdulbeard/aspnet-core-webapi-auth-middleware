using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MisturTee.Config;
using MisturTee.Config.Routing;
using MisturTee.Utils;

namespace MisturTee.Repositories
{
    internal static class RoutesRepository
    {
        private static Dictionary<string, List<InternalRouteDefinition>> _routes;
        private static readonly object LockObject = new object();
        private static readonly object RefreshLockObject = new object();
        private static bool _useProvider;
        private static IValidRouteDefinitionProvider _routeDefinitionProvider;
        private static DateTime _lastRetrievedAt;
        private static TimeSpan _refreshTimespan = TimeSpan.MinValue;
        private const int DefaultRefreshFrequencyInSeconds = 60;


        private const string AppSettingForRefreshFrequency = "MrT:RefreshFrequencyInSeconds";

        internal static async Task<Dictionary<string, List<InternalRouteDefinition>>> GetRoutesAsync()
        {
            if (!_useProvider || _routeDefinitionProvider == null)
            {
                return _routes;
            }
            LoadRefreshTimestamp();
            if (DateTime.Now - _lastRetrievedAt > _refreshTimespan)
            {
                var routes = await _routeDefinitionProvider.GetAsync().ConfigureAwait(false);
                var routesDict = RoutesUtils.GetValidRouteDefs(routes);
                lock (RefreshLockObject)
                {
                    _routes = routesDict;
                    _lastRetrievedAt = DateTime.Now;
                }
            }
            return _routes;
        }

        private static void LoadRefreshTimestamp()
        {
            if (_refreshTimespan == TimeSpan.MinValue)
            {
                var refreshFrequency = new ConfigurationManager().Appsettings<int>(AppSettingForRefreshFrequency);
                refreshFrequency = refreshFrequency == default(int)
                    ? DefaultRefreshFrequencyInSeconds
                    : refreshFrequency;
                _refreshTimespan = TimeSpan.FromSeconds(refreshFrequency);
            }
        }

        internal static Task RegisterRoutesAsync(IEnumerable<IRouteDefinitions> routeDefs)
        {
            lock (LockObject)
            {
                _routes = RoutesUtils.GetValidRouteDefs(routeDefs);
                _routeDefinitionProvider = null;
                _useProvider = false;
            }
            return Task.CompletedTask;
        }

        internal static Task RegisterRoutesAsync(IValidRouteDefinitionProvider routesDefinitionProvider)
        {
            lock (LockObject)
            {
                _routeDefinitionProvider = routesDefinitionProvider;
                _useProvider = true;
            }
            return Task.CompletedTask;
        }
    }
}
