using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using MisturTee.Utils;
using Xunit;

namespace MisturTee.TestMeFool.Utils
{
    public class ExtractionUtilsTests
    {
        [Fact]
        public void RouteValueDictionaryToKvpTest()
        {
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("RouteValueDictionaryToKvp", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (List<KeyValuePair<string, List<object>>>) methodInfo.Invoke(this,
                new object[] {new Dictionary<string, object>() {{"yo", "lo"}}});
            Assert.Equal("yo", result.First().Key);
            Assert.Equal("lo", result.First().Value.First());

            var routeValueDict = new RouteValueDictionary(new Dictionary<string, string>
            {
                {"yolo", "nolo"}
            });
            result = (List<KeyValuePair<string, List<object>>>) methodInfo.Invoke(this,
                new object[] {routeValueDict});
            Assert.Equal("yolo", result.First().Key);
            Assert.Equal("nolo", result.First().Value.First());
        }

        [Fact]
        public void HeaderDictToKvpTest()
        {
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("HeaderDictToKvp", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (List<KeyValuePair<string, List<object>>>) methodInfo.Invoke(this,
                new object[]
                {
                    new List<KeyValuePair<string, StringValues>>()
                    {
                        {new KeyValuePair<string, StringValues>("yolo", new StringValues("nolo"))}
                    },
                    new HeaderDictionary()
                    {
                        {new KeyValuePair<string, StringValues>("yolo", new StringValues("nolo"))}
                    }
                });
            Assert.Equal("yolo", result.First().Key);
            Assert.Equal("nolo", result.First().Value.First());
        }

        [Fact]
        public void QueryCollectionToKvpTest()
        {
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("QueryCollectionToKvp", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (List<KeyValuePair<string, List<object>>>) methodInfo.Invoke(this,
                new object[]
                {
                    new QueryCollection(new Dictionary<string, StringValues>()
                    {
                        {"yolo", new StringValues("nolo")}
                    })
                });
            Assert.Equal("yolo", result.First().Key);
            Assert.Equal("nolo", result.First().Value.First());
        }
    }
}
