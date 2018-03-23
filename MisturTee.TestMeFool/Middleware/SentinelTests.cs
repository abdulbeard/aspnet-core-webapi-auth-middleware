using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using MisturTee.Config;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using MisturTee.Config.Routing;
using MisturTee.Middleware;
using MisturTee.Repositories;
using MisturTee.TestMeFool.Claims;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static MisturTee.Config.Claims.ClaimValidationConfig;

namespace MisturTee.TestMeFool.Middleware
{
    public class SentinelTests
    {
        [Fact]
        public void SentinelTest_ValidationDelegateFailsValidation()
        {
            TestingUtils.ResetSentinel();
            var sentinel = new Sentinel(reqDelegate, new RoutesRepository());
            Sentinel.RegisterValidationDelegate(failingClaimValidationDelegate);
            var httpContext = new DefaultHttpContext();
            var result = sentinel.Invoke(httpContext);
            Assert.Equal((int)HttpStatusCode.BadRequest, httpContext.Response.StatusCode);
        }

        [Fact]
        public void SentinelTest_ValidationDelegatePasses()
        {
            TestingUtils.ResetSentinel();
            var sentinel = new Sentinel(reqDelegate, new RoutesRepository());
            Sentinel.RegisterValidationDelegate(claimValidationDelegate);
            var httpContext = new DefaultHttpContext();
            var result = sentinel.Invoke(httpContext);
            Assert.Equal((int)HttpStatusCode.Created, httpContext.Response.StatusCode);
        }

        [Fact]
        public void SentinelTest()
        {
            TestingUtils.ResetSentinel();
            TestingUtils.ResetRoutesRepository();

            var routesRepository = new RoutesRepository();
            routesRepository.RegisterRoutesAsync(new List<IRouteDefinitions>() { new SpecialRouteTemplateRoutes() }).Wait();
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var request = new DefaultHttpRequest(httpContext)
            {
                Method = HttpMethod.Post.Method,
                Path = "/v1/yolo/nolo/1",
                Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>() { { "No", "true" } }),
                Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new TestingType() { Yo = "lo", No = "low" })))
            };
            httpContext.Items.Add("ExpectedClaims", new List<Claim>()
            {
                new Claim("PityTheFoolType", "low"),
                new Claim("PityTheFoolJsonPath", "low"),
                new Claim("PityTheFoolKeyValue", "true"),
                new Claim("PityTheFoolRegex", "yolo"),
            });
            var sentinel = new Sentinel(reqDelegate, routesRepository);
            var result = sentinel.Invoke(httpContext);
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Created, httpContext.Response.StatusCode);
        }

        [Fact]
        public void SentinelTest_ThisOneShouldntPassButDoes()
        {
            TestingUtils.ResetSentinel();
            TestingUtils.ResetRoutesRepository();

            var routesRepository = new RoutesRepository();
            routesRepository.RegisterRoutesAsync(new List<IRouteDefinitions>() { new SpecialRouteTemplateRoutes() }).Wait();
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var request = new DefaultHttpRequest(httpContext)
            {
                Method = HttpMethod.Post.Method,
                Path = "/v1/yolo/nolo/thisIsDefinitelyNotAnInteger",
                Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>() { { "No", "true" } }),
                Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new TestingType() { Yo = "lo", No = "low" })))
            };
            httpContext.Items.Add("ExpectedClaims", new List<Claim>()
            {
                new Claim("PityTheFoolType", "low"),
                new Claim("PityTheFoolJsonPath", "low"),
                new Claim("PityTheFoolKeyValue", "true"),
                new Claim("PityTheFoolRegex", "yolo"),
            });
            var sentinel = new Sentinel(reqDelegate, routesRepository);
            var result = sentinel.Invoke(httpContext);
            Assert.Equal((int)HttpStatusCode.Created, httpContext.Response.StatusCode);
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
                            new TypeClaimExtractionConfig<TestingType>("PityTheFoolType")
                                .ConfigureExtraction((testingType) => Task.FromResult(testingType.No)).Build(),
                            new JsonPathClaimExtractionConfig("PityTheFoolJsonPath")
                                .ConfigureExtraction(ExtractionFunctions.JsonPathFunc,
                                    "$.No").Build(),
                            new KeyValueClaimExtractionConfig("PityTheFoolKeyValue", ClaimLocation.QueryParameters)
                                .ConfigureExtraction(ExtractionFunctions.KeyValueFunc, "No").Build(),
                            //make regex for the path only, not including the query parameters. For query params, use KeyValueClaimExtractionConfig instead
                            new RegexClaimExtractionConfig("PityTheFoolRegex", ClaimLocation.Uri).ConfigureExtraction(
                                ExtractionFunctions.RegexFunc,
                                new System.Text.RegularExpressions.Regex(
                                    "v1/(.*)/nolo")).Build()
                        },
                        ValidationConfigs = new List<ClaimValidationConfig>()
                        {
                            new ClaimValidationConfig()
                            {
                                ClaimName = "PityTheFoolType",
                                IsRequired = true,
                                AllowNullOrEmpty = false,
                                ValueMustBeExactMatch = true
                            },
                            new ClaimValidationConfig()
                            {
                                ClaimName = "PityTheFoolJsonPath",
                                IsRequired = true,
                                AllowNullOrEmpty = false,
                                ValueMustBeExactMatch = true
                            },
                            new ClaimValidationConfig()
                            {
                                ClaimName = "PityTheFoolKeyValue",
                                IsRequired = true,
                                AllowNullOrEmpty = false,
                                ValueMustBeExactMatch = true
                            },
                            new ClaimValidationConfig()
                            {
                                ClaimName = "PityTheFoolRegex",
                                IsRequired = true,
                                AllowNullOrEmpty = false,
                                ValueMustBeExactMatch = true
                            }
                        },
                        BadRequestResponse = new BadRequestResponse
                        {
                            HttpStatusCode = System.Net.HttpStatusCode.Forbidden,
                            BadRequestResponseOverride = null,
                        }
                    }
                }
            };
            }
        }

        private RequestDelegate reqDelegate = (httpContext) =>
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            return Task.CompletedTask;
        };
        private ClaimValidatorDelegate claimValidationDelegate = (httpContext) =>
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadGateway;
            return Task.FromResult(true);
        };
        private ClaimValidatorDelegate failingClaimValidationDelegate = (httpContext) =>
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Task.FromResult(false);
        };
    }
}
