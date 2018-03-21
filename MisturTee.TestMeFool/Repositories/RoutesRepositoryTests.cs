using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using MisturTee.Config;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using MisturTee.Config.Routing;
using MisturTee.Repositories;
using MisturTee.TestMeFool.Claims;
using Newtonsoft.Json;
using Xunit;

namespace MisturTee.TestMeFool.Repositories
{
    public class RoutesRepositoryTests
    {
        [Fact]
        public void RegisterRoutesAsyncTest_ByRouteDefinitions()
        {
            RoutesRepository.RegisterRoutesAsync(new List<IRouteDefinitions>() {new AnotherRoutes()}).Wait();
            var useProvider = GetPrivateValueFromRoutesRepository<bool>("_useProvider");
            var routeProvider = GetPrivateValueFromRoutesRepository<IValidRouteDefinitionProvider>("_routeDefinitionProvider");
            Assert.False(useProvider);
            Assert.Null(routeProvider);
            var routes = RoutesRepository.GetRoutesAsync();
            Assert.Equal(2, routes.Result.Values.First().Count);
            Assert.Equal(HttpMethod.Put, routes.Result.Values.First().First().Method);
            Assert.Equal(HttpMethod.Put, routes.Result.Values.First().ElementAt(1).Method);
        }

        [Fact]
        public void RegisterRoutesAsyncTest_ByRouteProvider()
        {
            RoutesRepository.RegisterRoutesAsync(new ValidRouteProvider()).Wait();
            var useProvider = GetPrivateValueFromRoutesRepository<bool>("_useProvider");
            var routeProvider = GetPrivateValueFromRoutesRepository<IValidRouteDefinitionProvider>("_routeDefinitionProvider");
            var refreshTimespan = GetPrivateValueFromRoutesRepository<TimeSpan>("_refreshTimespan");
            Assert.Equal(TimeSpan.MinValue, refreshTimespan);
            Assert.True(useProvider);
            Assert.NotNull(routeProvider);
            var routes = RoutesRepository.GetRoutesAsync();
            Assert.Single(routes.Result.Values);
            Assert.Equal(HttpMethod.Post, routes.Result.Values.First().First().Method);
            Assert.Single(routes.Result.Values.First());


        }

        private static T GetPrivateValueFromRoutesRepository<T>(string fieldName)
        {
            var fieldInfo = typeof(RoutesRepository).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            return (T) fieldInfo.GetValue(fieldInfo);
        }
    }

    public class AnotherRoutes : IRouteDefinitions
    {
        public List<RouteDefinition> GetRouteDefinitions()
        {
            return new List<RouteDefinition>()
            {
                new PutRouteDefinition(typeof(TestingType)),
                new PutRouteDefinition(typeof(TestingType))
            };
        }
    }

    public class Routes : IRouteDefinitions
    {
        public List<RouteDefinition> GetRouteDefinitions()
        {
            return new List<RouteDefinition>()
            {
                new PostRouteDefinition(typeof(TestingType))
                {
                    RouteTemplate = "v1/yolo/nolo",
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
                            Response = GetMissingClaimsResponse(),
                            BadRequestResponseOverride = (missingClaims, expectedClaims) =>
                            {
                                return Task.FromResult(GetSampleResponse());
                            }
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
            var response = new DefaultHttpResponse(new DefaultHttpContext())
            {
                Body = new MemoryStream()
            };
            var responseBytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { Yolo = "nolo" }));
            response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
            response.Headers.Add(new KeyValuePair<string, StringValues>("yolo", new StringValues("solo")));
            response.StatusCode = StatusCodes.Status502BadGateway;
            return response;
        }
    }

    public class ValidRouteProvider: IValidRouteDefinitionProvider {
        public Task<IEnumerable<IRouteDefinitions>> GetAsync()
        {
            return Task.FromResult(new List<IRouteDefinitions>() {new Routes()}.AsEnumerable());
        }
    }
}
