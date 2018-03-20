using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using MisturTee.Config;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using MisturTee.Config.Routing;
using Xunit;

namespace MisturTee.TestMeFool.Routing
{
    public class InternalRouteDefinitionTests
    {
        [Fact]
        public void InternalRouteDefinition()
        {
            var routeDef = new PostRouteDefinition(typeof(TestingType));
            routeDef.ClaimsConfig = new RouteClaimsConfig()
            {
                ExtractionConfigs = new List<IValidClaimsExtractionConfig>()
                {
                    new JsonPathClaimExtractionConfig("YoloClaim")
                        .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.json").Build()
                }
            };
            routeDef.RouteTemplate = "who/letThe/Dogs/out/{culpritId}";
            var internalRouteDef = new InternalRouteDefinition(routeDef);
            Assert.Equal(HttpMethod.Post, internalRouteDef.Method);
            Assert.Equal(typeof(TestingType), internalRouteDef.RequestBody);
        }

        [Fact]
        public void InternalRouteDefinition_BadRouteTemplate()
        {
            var routeDef = new PostRouteDefinition(typeof(TestingType));
            routeDef.ClaimsConfig = new RouteClaimsConfig()
            {
                ExtractionConfigs = new List<IValidClaimsExtractionConfig>()
                {
                    new JsonPathClaimExtractionConfig("YoloClaim")
                        .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.json").Build()
                }
            };
            routeDef.RouteTemplate = "slkdfjlsdkjf?&lksdflkjsdflk";
            try
            {
                var internalRouteDef = new InternalRouteDefinition(routeDef);
                Assert.False(true);
            }
            catch (Exception) { }
        }

        [Fact]
        public void InternalRouteDefinition_Post()
        {
            var routeDef = new PostRouteDefinition(typeof(TestingType));
            var claimsConfig = new RouteClaimsConfig()
            {
                ExtractionConfigs = new List<IValidClaimsExtractionConfig>()
                {
                    new JsonPathClaimExtractionConfig("YoloClaim")
                        .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.json").Build()
                }
            };
            routeDef.ClaimsConfig = claimsConfig;
            var internalRouteDef = new InternalRouteDefinition(routeDef);
            Assert.Equal(HttpMethod.Post, internalRouteDef.Method);
            Assert.Equal(typeof(TestingType), internalRouteDef.RequestBody);
            Assert.Equal(claimsConfig, internalRouteDef.ClaimsConfig);
        }

        [Fact]
        public void InternalRouteDefinition_Get()
        {
            var routeDef = new GetRouteDefinition();
            var claimsConfig = new RouteClaimsConfig()
            {
                ExtractionConfigs = new List<IValidClaimsExtractionConfig>()
                {
                    new JsonPathClaimExtractionConfig("YoloClaim")
                        .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.json").Build()
                }
            };
            routeDef.ClaimsConfig = claimsConfig;
            var internalRouteDef = new InternalRouteDefinition(routeDef);
            Assert.Equal(HttpMethod.Get, internalRouteDef.Method);
            Assert.Equal(typeof(object), internalRouteDef.RequestBody);
            Assert.Equal(claimsConfig, internalRouteDef.ClaimsConfig);
        }

        [Fact]
        public void InternalRouteDefinition_Put()
        {
            var routeDef = new PutRouteDefinition(typeof(TestingType));
            var claimsConfig = new RouteClaimsConfig()
            {
                ExtractionConfigs = new List<IValidClaimsExtractionConfig>()
                {
                    new JsonPathClaimExtractionConfig("YoloClaim")
                        .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.json").Build()
                }
            };
            routeDef.ClaimsConfig = claimsConfig;
            var internalRouteDef = new InternalRouteDefinition(routeDef);
            Assert.Equal(HttpMethod.Put, internalRouteDef.Method);
            Assert.Equal(typeof(TestingType), internalRouteDef.RequestBody);
            Assert.Equal(claimsConfig, internalRouteDef.ClaimsConfig);
        }


        [Fact]
        public void InternalRouteDefinition_Delete()
        {
            var routeDef = new DeleteRouteDefinition(typeof(TestingType));
            var claimsConfig = new RouteClaimsConfig()
            {
                ExtractionConfigs = new List<IValidClaimsExtractionConfig>()
                {
                    new JsonPathClaimExtractionConfig("YoloClaim")
                        .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.json").Build()
                }
            };
            routeDef.ClaimsConfig = claimsConfig;
            var internalRouteDef = new InternalRouteDefinition(routeDef);
            Assert.Equal(HttpMethod.Delete, internalRouteDef.Method);
            Assert.Equal(typeof(TestingType), internalRouteDef.RequestBody);
            Assert.Equal(claimsConfig, internalRouteDef.ClaimsConfig);
        }


        [Fact]
        public void InternalRouteDefinition_Head()
        {
            var routeDef = new HeadRouteDefinition();
            var claimsConfig = new RouteClaimsConfig()
            {
                ExtractionConfigs = new List<IValidClaimsExtractionConfig>()
                {
                    new JsonPathClaimExtractionConfig("YoloClaim")
                        .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.json").Build()
                }
            };
            routeDef.ClaimsConfig = claimsConfig;
            var internalRouteDef = new InternalRouteDefinition(routeDef);
            Assert.Equal(HttpMethod.Head, internalRouteDef.Method);
            Assert.Equal(typeof(object), internalRouteDef.RequestBody);
            Assert.Equal(claimsConfig, internalRouteDef.ClaimsConfig);
        }


        [Fact]
        public void InternalRouteDefinition_Options()
        {
            var routeDef = new OptionsRouteDefinition();
            var claimsConfig = new RouteClaimsConfig()
            {
                ExtractionConfigs = new List<IValidClaimsExtractionConfig>()
                {
                    new JsonPathClaimExtractionConfig("YoloClaim")
                        .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.json").Build()
                }
            };
            routeDef.ClaimsConfig = claimsConfig;
            var internalRouteDef = new InternalRouteDefinition(routeDef);
            Assert.Equal(HttpMethod.Options, internalRouteDef.Method);
            Assert.Equal(typeof(object), internalRouteDef.RequestBody);
            Assert.Equal(claimsConfig, internalRouteDef.ClaimsConfig);
        }


        [Fact]
        public void InternalRouteDefinition_Trace()
        {
            var routeDef = new TraceRouteDefinition();
            var claimsConfig = new RouteClaimsConfig()
            {
                ExtractionConfigs = new List<IValidClaimsExtractionConfig>()
                {
                    new JsonPathClaimExtractionConfig("YoloClaim")
                        .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.json").Build()
                }
            };
            routeDef.ClaimsConfig = claimsConfig;
            var internalRouteDef = new InternalRouteDefinition(routeDef);
            Assert.Equal(HttpMethod.Trace, internalRouteDef.Method);
            Assert.Equal(typeof(object), internalRouteDef.RequestBody);
            Assert.Equal(claimsConfig, internalRouteDef.ClaimsConfig);
        }
    }
}
