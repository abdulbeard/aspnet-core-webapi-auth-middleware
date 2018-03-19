using System;
using System.Text.RegularExpressions;
using MisturTee.Config;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using Xunit;

namespace MisturTee.TestMeFool
{
    public class ClaimExtractionConfigTests
    {
        [Fact]
        public void JsonPathClaimExtractionConfig()
        {
            var jsonPathClaimExtractionConfig = new JsonPathClaimExtractionConfig("PityTheFoolClaim");
            try
            {
                jsonPathClaimExtractionConfig.Build();
                Assert.True(true);
            }
            catch (ArgumentException) { }
            jsonPathClaimExtractionConfig.ConfigureExtraction(ExtractionFunctions.JsonPathFunc, null);
            try
            {
                jsonPathClaimExtractionConfig.Build();
                Assert.True(true);
            }
            catch (ArgumentException) { }
            jsonPathClaimExtractionConfig.ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.json");
            var validJsonPathClaimExtractionConfig = jsonPathClaimExtractionConfig.Build();
            Assert.Equal(ExtractionType.JsonPath, validJsonPathClaimExtractionConfig.ExtractionType);
            Assert.Equal(ClaimLocation.Body, validJsonPathClaimExtractionConfig.ClaimLocation);
        }

        [Fact]
        public void KeyValueClaimExtractionConfig()
        {
            var keyValueClaimExtractionConfig = new KeyValueClaimExtractionConfig("PityTheFoolClaim", ClaimLocation.Headers);
            try
            {
                keyValueClaimExtractionConfig.Build();
                Assert.True(true);
            }
            catch (ArgumentException) { }
            keyValueClaimExtractionConfig.ConfigureExtraction(ExtractionFunctions.KeyValueFunc, null);
            try
            {
                keyValueClaimExtractionConfig.Build();
                Assert.True(true);
            }
            catch (ArgumentException) { }
            keyValueClaimExtractionConfig.ConfigureExtraction(ExtractionFunctions.KeyValueFunc, "key");
            var validJsonPathClaimExtractionConfig = keyValueClaimExtractionConfig.Build();
            Assert.Equal(ExtractionType.KeyValue, validJsonPathClaimExtractionConfig.ExtractionType);
            Assert.Equal(ClaimLocation.Headers, validJsonPathClaimExtractionConfig.ClaimLocation);
        }

        [Fact]
        public void RegExClaimExtractionConfig()
        {
            var regexClaimExtractionConfig = new RegexClaimExtractionConfig("PityTheFoolClaim", ClaimLocation.Uri);
            try
            {
                regexClaimExtractionConfig.Build();
                Assert.True(true);
            }
            catch (ArgumentException) { }
            regexClaimExtractionConfig.ConfigureExtraction(ExtractionFunctions.RegexFunc, null);
            try
            {
                regexClaimExtractionConfig.Build();
                Assert.True(true);
            }
            catch (ArgumentException) { }
            regexClaimExtractionConfig.ConfigureExtraction(ExtractionFunctions.RegexFunc, new Regex("yolo/(.*)/solo"));
            var validJsonPathClaimExtractionConfig = regexClaimExtractionConfig.Build();
            Assert.Equal(ExtractionType.RegEx, validJsonPathClaimExtractionConfig.ExtractionType);
            Assert.Equal(ClaimLocation.Uri, validJsonPathClaimExtractionConfig.ClaimLocation);
        }

        [Fact]
        public void TypeClaimExtractionConfig()
        {
            var typeClaimExtractionConfig = new TypeClaimExtractionConfig<TestingType>("PityTheFoolClaim");
            try
            {
                typeClaimExtractionConfig.Build();
                Assert.True(true);
            }
            catch (ArgumentException) { }
            typeClaimExtractionConfig.ConfigureExtraction(x => null);
            try
            {
                typeClaimExtractionConfig.Build();
                Assert.True(true);
            }
            catch (ArgumentException) { }
            typeClaimExtractionConfig.ConfigureExtraction(x => null);
            var validJsonPathClaimExtractionConfig = typeClaimExtractionConfig.Build();
            Assert.Equal(ExtractionType.Type, validJsonPathClaimExtractionConfig.ExtractionType);
            Assert.Equal(ClaimLocation.Body, validJsonPathClaimExtractionConfig.ClaimLocation);
        }
    }
}
