using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder.Internal;
using MisturTee.Config;
using MisturTee.Config.Routing;
using MisturTee.Middleware;
using Xunit;

namespace MisturTee.TestMeFool.Middleware
{
    public class MiddlewareExtensionsTests
    {
        [Fact]
        public void UseCustomClaimsValidationAsync_RouteDefinitions()
        {
            var applicationBuilder = new ApplicationBuilder(new ServiceContainer());
            var appBuilder = applicationBuilder.UseCustomClaimsValidationAsync(new List<IRouteDefinitions>(), new MisturTee.Repositories.RoutesRepository())
                .Result;
            Assert.NotNull(appBuilder);
        }


        [Fact]
        public void UseCustomClaimsValidationAsync_RoutesProvider()
        {
            var applicationBuilder = new ApplicationBuilder(new ServiceContainer());
            var appBuilder = applicationBuilder
                .UseCustomClaimsValidationAsync(new TestValidRouteDefinitionProvider(), new MisturTee.Repositories.RoutesRepository()).Result;
            Assert.NotNull(appBuilder);
        }

        private class TestValidRouteDefinitionProvider : IValidRouteDefinitionProvider
        {
            public Task<IEnumerable<IRouteDefinitions>> GetAsync()
            {
                return null;
            }
        }
    }
}
