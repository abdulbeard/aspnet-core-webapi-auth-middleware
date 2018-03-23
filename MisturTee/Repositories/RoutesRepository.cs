using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MisturTee.Config;
using MisturTee.Config.Routing;
using MisturTee.Utils;

namespace MisturTee.Repositories
{
    public class RoutesRepository
    {
        private Dictionary<string, List<InternalRouteDefinition>> _routes;
        private readonly object _lockObject = new object();
        private readonly object _refreshLockObject = new object();
        private bool _useProvider;
        private IValidRouteDefinitionProvider _routeDefinitionProvider;
        private DateTime _lastRetrievedAt;
        private TimeSpan _refreshTimespan = TimeSpan.MinValue;
        private const int DefaultRefreshFrequencyInSeconds = 60;


        private const string AppSettingForRefreshFrequency = "MrT:RefreshFrequencyInSeconds";

        internal async Task<Dictionary<string, List<InternalRouteDefinition>>> GetRoutesAsync()
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
                lock (_refreshLockObject)
                {
                    _routes = routesDict;
                    _lastRetrievedAt = DateTime.Now;
                }
            }
            return _routes;
        }

        private void LoadRefreshTimestamp()
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

        internal Task RegisterRoutesAsync(IEnumerable<IRouteDefinitions> routeDefs)
        {
            lock (_lockObject)
            {
                _routes = RoutesUtils.GetValidRouteDefs(routeDefs);
                _routeDefinitionProvider = null;
                _useProvider = false;
            }
            return Task.CompletedTask;
        }

        internal Task RegisterRoutesAsync(IValidRouteDefinitionProvider routesDefinitionProvider)
        {
            lock (_lockObject)
            {
                _routeDefinitionProvider = routesDefinitionProvider;
                _useProvider = true;
            }
            return Task.CompletedTask;
        }
    }
}
