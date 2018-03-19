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
            routeDef.RouteTemplate = "/who/letThe/Dogs/out/{culpritId}";
            var internalRouteDef = new InternalRouteDefinition(routeDef);
            Assert.Equal(HttpMethod.Post, internalRouteDef.Method);
            Assert.Equal(typeof(TestingType), internalRouteDef.RequestBody);
        }
    }
}
