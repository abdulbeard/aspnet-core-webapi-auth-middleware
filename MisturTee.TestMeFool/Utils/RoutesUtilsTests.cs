using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using MisturTee.Config;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using MisturTee.Config.Routing;
using MisturTee.Repositories;
using MisturTee.TestMeFool.Claims;
using MisturTee.Utils;
using Xunit;

namespace MisturTee.TestMeFool.Utils
{
    public class RoutesUtilsTests
    {
        [Fact]
        public void GetValidRouteDefsTest()
        {
            var result = RoutesUtils.GetValidRouteDefs(new List<IRouteDefinitions>() { new Routes() });
            Assert.Equal(7, result.Keys.Count);
            Assert.Equal(2, result["PUT"].Count);
        }

        [Fact]
        public void GetValidRouteDefsTest_InvalidRouteTemplate()
        {
            try
            {
                RoutesUtils.GetValidRouteDefs(new List<IRouteDefinitions>() { new InvalidRouteTemplateRoutes() });
                Assert.True(false);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [Fact]
        public void GetValidRouteDefsTest_NullRouteDefs()
        {
            Assert.Equal(new Dictionary<string, List<InternalRouteDefinition>>(), RoutesUtils.GetValidRouteDefs(null));
        }

        [Fact]
        public void MatchRouteAsyncTest()
        {
            RoutesRepository.RegisterRoutesAsync(new List<IRouteDefinitions>() { new SpecialRouteTemplateRoutes() }).Wait();
            var httpContext = new DefaultHttpContext();
            var request = new DefaultHttpRequest(httpContext)
            {
                Method = HttpMethod.Post.Method,
                Path = "/v1/yolo/nolo/1"
            };
            var result = RoutesUtils.MatchRouteAsync(httpContext).Result;
            Assert.NotNull(request);
            Assert.Equal(4, result.Key.ClaimsConfig.ExtractionConfigs.Count);
            Assert.Equal(1, result.Key.ClaimsConfig.ValidationConfigs.Count);
            Assert.NotNull(result.Key.ClaimsConfig.BadRequestResponse);
            Assert.Equal("1", result.Value["id"]);
        }


        [Fact]
        public void MatchRouteAsyncTest_NoMatch()
        {
            RoutesRepository.RegisterRoutesAsync(new List<IRouteDefinitions>() { new SpecialRouteTemplateRoutes() }).Wait();
            var httpContext = new DefaultHttpContext();
            var request = new DefaultHttpRequest(httpContext)
            {
                Method = HttpMethod.Post.Method,
                Path = "/v1/yolo/nolo/asdf"
            };
            var result = RoutesUtils.MatchRouteAsync(httpContext).Result;
            Assert.NotNull(request);
            Assert.Null(result.Key);
            Assert.Null(result.Value);
        }

        [Fact]
        public void GetMatchingRouteTest()
        {
            RoutesRepository.RegisterRoutesAsync(new List<IRouteDefinitions>() { new SpecialRouteTemplateRoutes() }).Wait();
            var httpContext = new DefaultHttpContext();
            var request = new DefaultHttpRequest(httpContext)
            {
                Method = HttpMethod.Post.Method,
                Path = "/v1/yolo/nolo/1"
            };
            var result = RoutesUtils.GetMatchingRoute(httpContext).Result;
            Assert.NotNull(request);
            Assert.Equal(4, result.Route.ClaimsConfig.ExtractionConfigs.Count);
            Assert.Equal(1, result.Route.ClaimsConfig.ValidationConfigs.Count);
            Assert.NotNull(result.Route.ClaimsConfig.BadRequestResponse);
            Assert.Equal("1", result.RouteValues["id"]);
        }

        [Fact]
        public void GetMatchingRouteTest_NoMatch()
        {
            RoutesRepository.RegisterRoutesAsync(new List<IRouteDefinitions>() { new SpecialRouteTemplateRoutes() }).Wait();
            var httpContext = new DefaultHttpContext();
            var request = new DefaultHttpRequest(httpContext)
            {
                Method = HttpMethod.Post.Method,
                Path = "/v1/yolo/nolo/asdf"
            };
            var result = RoutesUtils.GetMatchingRoute(httpContext).Result;
            Assert.NotNull(request);
            Assert.NotNull(result.Route);
            Assert.NotNull(result.RouteValues);
        }
    }

    public class SpecialRouteTemplateRoutes : IRouteDefinitions
    {
        public List<RouteDefinition> GetRouteDefinitions()
        {
            return new List<RouteDefinition>
            {
                new PostRouteDefinition(typeof(TestingType))
                {
                    RouteTemplate = "v1/yolo/nolo/{id:int}",
                    ClaimsConfig = new RouteClaimsConfig()
                    {
                        ExtractionConfigs = new List<IValidClaimsExtractionConfig>()
                        {
                            new TypeClaimExtractionConfig<TestingType>("PityTheFool")
                                .ConfigureExtraction((testingType) => Task.FromResult(testingType.No)).Build(),
                            new JsonPathClaimExtractionConfig("PityTheFool")
                                .ConfigureExtraction(ExtractionFunctions.JsonPathFunc,
                                    "$.No").Build(),
                            new KeyValueClaimExtractionConfig("PityTheFool", ClaimLocation.QueryParameters)
                                .ConfigureExtraction(ExtractionFunctions.KeyValueFunc, "No").Build(),
                            //make regex for the path only, not including the query parameters. For query params, use KeyValueClaimExtractionConfig instead
                            new RegexClaimExtractionConfig("PityTheFool", ClaimLocation.Uri).ConfigureExtraction(
                                ExtractionFunctions.RegexFunc,
                                new System.Text.RegularExpressions.Regex(
                                    "v1/(.*)/nolo")).Build()
                        },
                        ValidationConfigs = new List<ClaimValidationConfig>()
                        {
                            new ClaimValidationConfig()
                            {
                                ClaimName = "PityTheFool",
                                IsRequired = true,
                                AllowNullOrEmpty = false
                            }
                        },
                        BadRequestResponse = new BadRequestResponse
                        {
                            HttpStatusCode = System.Net.HttpStatusCode.Forbidden,
                            Response = null,
                            BadRequestResponseOverride = null
                        }
                    }
                }
            };
        }
    }

    public class InvalidRouteTemplateRoutes : IRouteDefinitions
    {
        public List<RouteDefinition> GetRouteDefinitions()
        {
            return new List<RouteDefinition>()
            {
                new PutRouteDefinition(typeof(TestingType)){RouteTemplate = "/yolo/nolo/solo"}
            };
        }
    }

    public class Routes : IRouteDefinitions
    {
        public List<RouteDefinition> GetRouteDefinitions()
        {
            return new List<RouteDefinition>()
            {
                new PutRouteDefinition(typeof(TestingType)){RouteTemplate = "yolo/nolo/solo"},
                new PutRouteDefinition(typeof(TestingType)){RouteTemplate = "yolo/nolo/solo"},
                new PostRouteDefinition(typeof(TestingType)){RouteTemplate = "yolo/nolo/solo"},
                new GetRouteDefinition(){RouteTemplate = "yolo/nolo/solo"},
                new HeadRouteDefinition(){RouteTemplate = "yolo/nolo/solo"},
                new DeleteRouteDefinition(typeof(TestingType)){RouteTemplate = "yolo/nolo/solo"},
                new OptionsRouteDefinition(){RouteTemplate = "yolo/nolo/solo"},
                new TraceRouteDefinition(){RouteTemplate = "yolo/nolo/solo"},
            };
        }
    }
}
