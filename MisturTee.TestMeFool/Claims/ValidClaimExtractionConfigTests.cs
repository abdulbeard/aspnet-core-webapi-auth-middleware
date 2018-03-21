using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MisturTee.Config;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using Newtonsoft.Json;
using Xunit;

namespace MisturTee.TestMeFool.Claims
{
    public class ValidClaimExtractionConfigTests
    {
        [Theory]
        [InlineData("$.title", "{\"title\": \"Person\",\"type\": \"LivingThing\"}", nameof(ExtractionFunctions.JsonPathFunc))]
        [InlineData("", "{'returnsTheFullThing':'withoutHesitation'}", nameof(ExtractionFunctions.JsonPathFunc))]
        [InlineData("$.title", null, nameof(ExtractionFunctions.JsonPathFunc))]
        public void JsonPathExtraction_Theory(string jsonPath, string json, string extractionFunction)
        {
            const string claimName = "PityTheFoolClaim";
            JsonPathClaimExtractionConfig.ExtractValueByJsonPathAsync extractionFunc = null;
            if (extractionFunction?.Equals(nameof(ExtractionFunctions.JsonPathFunc)) ?? false)
            {
                extractionFunc = ExtractionFunctions.JsonPathFunc;
            }
            var config = new ValidJsonPathClaimExtractionConfig(jsonPath, extractionFunc, claimName, ClaimLocation.Body);
            Assert.True(config.ClaimLocation.Equals(ClaimLocation.Body));
            Assert.True(config.ExtractionType.Equals(ExtractionType.JsonPath));
            var result = config.GetClaimAsync(json).Result;
            if (string.IsNullOrEmpty(jsonPath) && result != null)
            {
                Assert.Equal(result.Type, claimName);
                Assert.Equal("{\r\n  \"returnsTheFullThing\": \"withoutHesitation\"\r\n}", result.Value);
            }
            else if (string.IsNullOrEmpty(json))
            {
                Assert.Null(result);
            }
            else if(result != null)
            {
                Assert.Equal(result.Type, claimName);
                Assert.Equal("Person", result.Value);
            }
        }

        [Fact]
        public void JsonPathExtraction_NoExtractionFunc()
        {
            const string claimName = "PityTheFoolClaim";
            var config = new ValidJsonPathClaimExtractionConfig("$.title", null,
                claimName, ClaimLocation.Body);
            Assert.True(config.ClaimLocation.Equals(ClaimLocation.Body));
            Assert.True(config.ExtractionType.Equals(ExtractionType.JsonPath));
            try
            {
                config.GetClaimAsync("{\"title\": \"Person\",\"type\": \"LivingThing\"}").Wait();
                Assert.True(false);
            }
            catch (AggregateException) { }
        }

        [Theory]
        [InlineData("1", "[{\"Key\":\"1\",\"Value\":[\"one\"]},{\"Key\":\"2\",\"Value\":[\"two\"]}]", nameof(ExtractionFunctions.KeyValueFunc))]
        [InlineData("2", "[{\"Key\":\"1\",\"Value\":[\"one\"]},{\"Key\":\"2\",\"Value\":[\"two\"]}]", nameof(ExtractionFunctions.KeyValueFunc))]
        [InlineData("fourSeventyFive", "[{\"Key\":\"1\",\"Value\":[\"one\"]},{\"Key\":\"2\",\"Value\":[\"two\"]}]", nameof(ExtractionFunctions.KeyValueFunc))]
        public void KeyValuePairExtraction_Theory(string key, string data, string extractionFunction)
        {
            const string claimName = "PityTheFoolClaim";
            KeyValueClaimExtractionConfig.KeyValueExtractionAsync extractionFunc = null;
            if (extractionFunction?.Equals(nameof(ExtractionFunctions.KeyValueFunc)) ?? false)
            {
                extractionFunc = ExtractionFunctions.KeyValueFunc;
            }
            var config = new ValidKeyValueClaimExtractionConfig(extractionFunc, key, ClaimLocation.Body, claimName);
            Assert.True(config.ClaimLocation.Equals(ClaimLocation.Body));
            Assert.True(config.ExtractionType.Equals(ExtractionType.KeyValue));
            var result = config.GetClaimAsync(data).Result;
            switch (key)
            {
                case "1":
                {
                    Assert.Equal(claimName, result.Type);
                    Assert.Equal("one", result.Value);
                    break;
                }
                case "2":
                {
                    Assert.Equal(claimName, result.Type);
                    Assert.Equal("two", result.Value);
                    break;
                }
                case "fourSeventyFive":
                {
                    Assert.Equal(string.Empty, result.Value);
                    break;
                }
            }
        }

        [Fact]
        public void KeyValuePairExtraction_NoExtractionFunction()
        {
            const string claimName = "PityTheFoolClaim";
            var config = new ValidKeyValueClaimExtractionConfig(null, "1", ClaimLocation.Body, claimName);
            Assert.True(config.ClaimLocation.Equals(ClaimLocation.Body));
            Assert.True(config.ExtractionType.Equals(ExtractionType.KeyValue));
            try
            {
                config.GetClaimAsync("{\"title\": \"Person\",\"type\": \"LivingThing\"}").Wait();
                Assert.True(false);
            }
            catch (AggregateException) { }
        }

        [Theory]
        [InlineData("/a/b/c/(.*)/e", "/a/b/c/d/e", nameof(ExtractionFunctions.RegexFunc))]
        [InlineData("/a/(.*)/c/d/e", "/a/b/c/d/e", nameof(ExtractionFunctions.RegexFunc))]
        [InlineData("", "/a/b/c/d/e", nameof(ExtractionFunctions.RegexFunc))]
        [InlineData("/a/b/c/(.*)/e", "", nameof(ExtractionFunctions.RegexFunc))]
        public void RegExExtraction_Theory(string regexString, string content, string extractionFunction)
        {
            const string claimName = "PityTheFoolClaim";
            RegexClaimExtractionConfig.ExtractValueByRegexAsync extractionFunc = null;
            if (extractionFunction?.Equals(nameof(ExtractionFunctions.RegexFunc)) ?? false)
            {
                extractionFunc = ExtractionFunctions.RegexFunc;
            }
            var regex = new Regex(regexString);
            var config = new ValidRegexClaimExtractionConfig(extractionFunc, regex, claimName, ClaimLocation.Body);
            Assert.True(config.ClaimLocation.Equals(ClaimLocation.Body));
            Assert.True(config.ExtractionType.Equals(ExtractionType.RegEx));
            var result = config.GetClaimAsync(content).Result;
            switch (regexString)
            {
                case "/a/b/c/(.*)/e":
                {
                    if (string.IsNullOrEmpty(content))
                    {
                        if (content == null)
                        {
                            Assert.Equal(claimName, result.Type);
                            Assert.Equal(string.Empty, result.Value);
                        }
                        else
                        {
                            Assert.Equal(claimName, result.Type);
                            Assert.Equal(string.Empty, result.Value);
                        }
                    }
                    else
                    {
                        Assert.Equal(claimName, result.Type);
                        Assert.Equal("d", result.Value);
                    }
                    break;
                }
                case "/a/(.*)/c/d/e":
                {
                    Assert.Equal(claimName, result.Type);
                    Assert.Equal("b", result.Value);
                    break;
                }
                case "":
                {
                    Assert.Equal(claimName, result.Type);
                    Assert.Equal(string.Empty, result.Value);
                    break;
                }
            }
        }

        [Fact]
        public void RegexExtraction_NullExtractionFunction()
        {
            const string claimName = "PityTheFoolClaim";
            var regex = new Regex("/a/(.*)/c/d/e");
            var config = new ValidRegexClaimExtractionConfig(null, regex, claimName, ClaimLocation.Body);
            Assert.True(config.ClaimLocation.Equals(ClaimLocation.Body));
            Assert.True(config.ExtractionType.Equals(ExtractionType.RegEx));
            try
            {
                config.GetClaimAsync("/a/b/c/d/e").Wait();
                Assert.True(false);
            }
            catch (AggregateException) { }
        }

        [Fact]
        public void RegexExtraction_NullData()
        {
            const string claimName = "PityTheFoolClaim";
            var regex = new Regex("/a/(.*)/c/d/e");
            var config = new ValidRegexClaimExtractionConfig(ExtractionFunctions.RegexFunc, regex, claimName, ClaimLocation.Body);
            Assert.True(config.ClaimLocation.Equals(ClaimLocation.Body));
            Assert.True(config.ExtractionType.Equals(ExtractionType.RegEx));
            try
            {
                config.GetClaimAsync(null).Wait();
                Assert.True(false);
            }
            catch (AggregateException) { }
        }

        [Fact]
        public void TypeExtraction()
        {
            const string claimName = "PityTheFoolClaim";
            TypeClaimExtractionConfig<TestingType>.ExtractClaimForTypeAsync extractionFunc = (testingType) => Task.FromResult(testingType.No);
            var config = new ValidTypeClaimExtractionConfig<TestingType>(extractionFunc, claimName, ClaimLocation.Body);
            Assert.True(config.ClaimLocation.Equals(ClaimLocation.Body));
            Assert.True(config.ExtractionType.Equals(ExtractionType.Type));
            var instance = new TestingType() {Yo = "lo", No = "lo"};
            var result = config.GetClaimAsync(JsonConvert.SerializeObject(instance)).Result;
            Assert.Equal(instance.No, result.Value);
            Assert.Equal(claimName, result.Type);
        }

        [Fact]
        public void TypeExtraction_NullExtractionFunction()
        {
            const string claimName = "PityTheFoolClaim";
            var config = new ValidTypeClaimExtractionConfig<TestingType>(null, claimName, ClaimLocation.Body);
            Assert.True(config.ClaimLocation.Equals(ClaimLocation.Body));
            Assert.True(config.ExtractionType.Equals(ExtractionType.Type));
            var instance = new TestingType() { Yo = "lo", No = "lo" };
            try
            {
                config.GetClaimAsync(JsonConvert.SerializeObject(instance)).Wait();
            }
            catch (AggregateException) { }
        }

        [Fact]
        public void TypeExtraction_NullEntity()
        {
            const string claimName = "PityTheFoolClaim";
            TypeClaimExtractionConfig<TestingType>.ExtractClaimForTypeAsync extractionFunc = (testingType) => Task.FromResult(testingType.No);
            var config = new ValidTypeClaimExtractionConfig<TestingType>(extractionFunc, claimName, ClaimLocation.Body);
            Assert.True(config.ClaimLocation.Equals(ClaimLocation.Body));
            Assert.True(config.ExtractionType.Equals(ExtractionType.Type));
            try
            {
                config.GetClaimAsync(null).Wait();
            }
            catch (AggregateException) { }
        }
    }

    public class TestingType
    {
        public string Yo { get; set; }
        public string No { get; set; }
    }
}
