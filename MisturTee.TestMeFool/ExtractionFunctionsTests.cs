using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MisturTee.Config;
using Xunit;

namespace MisturTee.TestMeFool
{
    public class ExtractionFunctionsTests
    {
        [Fact]
        public void JsonPathExtractionFunctionTest()
        {
            var json = "{\"title\": \"Person\",\"type\": \"LivingThing\"}";
            var jsonPath = "$.title";
            var jsonExtracted = ExtractionFunctions.JsonPathFunc(json, jsonPath).Result;
            Assert.Equal("Person", jsonExtracted);

            jsonExtracted = ExtractionFunctions.JsonPathFunc(json, "$.Yolo").Result;
            Assert.Equal(string.Empty, jsonExtracted);
        }

        [Fact]
        public void JsonPathExtractionFunctionTest_InvalidJson()
        {
            var json = "title:Person,type:LivingThing";
            var jsonPath = "$.title";
            try
            {
                ExtractionFunctions.JsonPathFunc(json, jsonPath).Wait();
                Assert.True(false);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [Fact]
        public void KeyValueExtractionFunctionTest()
        {
            var kvps = new List<KeyValuePair<string, List<object>>>()
            {
                new KeyValuePair<string, List<object>>("Authorization", new List<object>(){"the", "bauss"}),
                new KeyValuePair<string, List<object>>("whodat", new List<object>(){"its", "me"})
            };
            var key = "Authorization";
            var result = ExtractionFunctions.KeyValueFunc(kvps, key).Result;
            Assert.Equal("the", result);

            key = "whodat";
            result = ExtractionFunctions.KeyValueFunc(kvps, key).Result;
            Assert.Equal("its", result);
        }

        [Fact]
        public void KeyValueExtractionFunctionTest_Null()
        {
            var key = "Authorization";
            try
            {
                ExtractionFunctions.KeyValueFunc(null, key).Wait();
                Assert.True(false);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [Fact]
        public void KeyValueExtractionFunctionTest_NotFound()
        {
            var kvps = new List<KeyValuePair<string, List<object>>>()
            {
                new KeyValuePair<string, List<object>>("Authorization", null)
            };
            var key = "Authorization";
            var result = ExtractionFunctions.KeyValueFunc(kvps, key).Result;
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void RegexExtractionFunctionTest()
        {
            var regex = new Regex(".*/a/(.*)/c/d/e.*");
            var data = "I_Am_Gonna_Go_Through_The_Alphabet_Now_/a/b/c/d/e/f/g/h.......";
            var result = ExtractionFunctions.RegexFunc(data, regex).Result;
            Assert.Equal("b", result);
        }

        [Fact]
        public void RegexExtractionFunctionTest_NoRegex()
        {
            var regex = new Regex("");
            var data = "I_Am_Gonna_Go_Through_The_Alphabet_Now_/a/b/c/d/e/f/g/h.......";
            var result = ExtractionFunctions.RegexFunc(data, regex).Result;
            Assert.Equal(string.Empty, result);
        }


        [Fact]
        public void RegexExtractionFunctionTest_NoMatch()
        {
            var regex = new Regex(".*/a/.*/c/d/e.*");
            var data = "I_Am_Gonna_Go_Through_The_Alphabet_Now_/a/b/c/d/e/f/g/h.......";
            var result = ExtractionFunctions.RegexFunc(data, regex).Result;
            Assert.Equal(string.Empty, result);
        }
    }
}
