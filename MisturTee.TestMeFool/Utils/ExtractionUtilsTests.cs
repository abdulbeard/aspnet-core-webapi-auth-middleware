using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Primitives;
using MisturTee.Config;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using MisturTee.Config.Routing;
using MisturTee.TestMeFool.Claims;
using MisturTee.Utils;
using Newtonsoft.Json;
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

        [Fact]
        public void GetContentTest_ClaimBody()
        {
            var requestBytes = Encoding.UTF8.GetBytes("{'IAm':'AFool', 'YouAreNotAFool':true}");
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("GetContent", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (string) methodInfo.Invoke(this,
                new object[]
                {
                    ClaimLocation.Body, ExtractionType.JsonPath, new DefaultHttpRequest(new DefaultHttpContext())
                    {
                        Body = new MemoryStream(requestBytes),
                        Method = HttpMethod.Post.ToString(),
                        Headers =
                        {
                            new KeyValuePair<string, StringValues>("Authorization",
                                new StringValues("Bearer hollaaaaaa"))
                        },
                        Path = "/yolo/solo/nolo/12345",
                        Query = new QueryCollection(
                            new Dictionary<string, StringValues>() {{"areYou", new StringValues("YesIAm")}}),
                        ContentType = "application/json",
                        ContentLength = requestBytes.Length
                    },
                    new RouteValueDictionary() {{"culpritId", "12345"}}, null
                });
            Assert.Equal("{'IAm':'AFool', 'YouAreNotAFool':true}", result);
        }

        [Fact]
        public void GetContentTest_ClaimHeaders_ExtractionJsonPath()
        {
            var requestBytes = Encoding.UTF8.GetBytes("{'IAm':'AFool', 'YouAreNotAFool':true}");
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("GetContent", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (string)methodInfo.Invoke(this,
                new object[]
                {
                    ClaimLocation.Headers, ExtractionType.JsonPath, new DefaultHttpRequest(new DefaultHttpContext())
                    {
                        Body = new MemoryStream(requestBytes),
                        Method = HttpMethod.Post.ToString(),
                        Headers =
                        {
                            new KeyValuePair<string, StringValues>("Authorization",
                                new StringValues("Bearer hollaaaaaa"))
                        },
                        Path = "/yolo/solo/nolo/12345",
                        Query = new QueryCollection(
                            new Dictionary<string, StringValues>() {{"areYou", new StringValues("YesIAm")}}),
                        ContentType = "application/json",
                        ContentLength = requestBytes.Length
                    },
                    new RouteValueDictionary() {{"culpritId", "12345"}}, null
                });
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetContentTest_ClaimHeaders_ExtractionKeyValue()
        {
            var requestBytes = Encoding.UTF8.GetBytes("{'IAm':'AFool', 'YouAreNotAFool':true}");
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("GetContent", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (string)methodInfo.Invoke(this,
                new object[]
                {
                    ClaimLocation.Headers, ExtractionType.KeyValue, new DefaultHttpRequest(new DefaultHttpContext())
                    {
                        Body = new MemoryStream(requestBytes),
                        Method = HttpMethod.Post.ToString(),
                        Headers =
                        {
                            new KeyValuePair<string, StringValues>("Authorization",
                                new StringValues("Bearer hollaaaaaa")),
                            new KeyValuePair<string, StringValues>("x-access-token",
                                new StringValues("just kidding"))
                        },
                        Path = "/yolo/solo/nolo/12345",
                        Query = new QueryCollection(
                            new Dictionary<string, StringValues>() {{"areYou", new StringValues("YesIAm")}}),
                        ContentType = "application/json",
                        ContentLength = requestBytes.Length
                    },
                    new RouteValueDictionary() {{"culpritId", "12345"}}, null
                });
            Assert.Equal(
                "[{\"Key\":\"Authorization\",\"Value\":[\"Bearer hollaaaaaa\"]},{\"Key\":\"x-access-token\",\"Value\":[\"just kidding\"]},{\"Key\":\"Content-Type\",\"Value\":[\"application/json\"]},{\"Key\":\"Content-Length\",\"Value\":[\"38\"]}]",
                result);
        }

        [Fact]
        public void GetContentTest_ClaimUri_ExtractionKeyValue_NullRouteValues()
        {
            var requestBytes = Encoding.UTF8.GetBytes("{'IAm':'AFool', 'YouAreNotAFool':true}");
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("GetContent", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (string)methodInfo.Invoke(this,
                new object[]
                {
                    ClaimLocation.Uri, ExtractionType.KeyValue, new DefaultHttpRequest(new DefaultHttpContext())
                    {
                        Body = new MemoryStream(requestBytes),
                        Method = HttpMethod.Post.ToString(),
                        Headers =
                        {
                            new KeyValuePair<string, StringValues>("Authorization",
                                new StringValues("Bearer hollaaaaaa")),
                            new KeyValuePair<string, StringValues>("x-access-token",
                                new StringValues("just kidding"))
                        },
                        Path = "/yolo/solo/nolo/12345",
                        Query = new QueryCollection(
                            new Dictionary<string, StringValues>() {{"areYou", new StringValues("YesIAm")}}),
                        ContentType = "application/json",
                        ContentLength = requestBytes.Length
                    },
                    null, null
                });
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetContentTest_ClaimUri_ExtractionKeyValue()
        {
            var requestBytes = Encoding.UTF8.GetBytes("{'IAm':'AFool', 'YouAreNotAFool':true}");
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("GetContent", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (string)methodInfo.Invoke(this,
                new object[]
                {
                    ClaimLocation.Uri, ExtractionType.KeyValue, new DefaultHttpRequest(new DefaultHttpContext())
                    {
                        Body = new MemoryStream(requestBytes),
                        Method = HttpMethod.Post.ToString(),
                        Headers =
                        {
                            new KeyValuePair<string, StringValues>("Authorization",
                                new StringValues("Bearer hollaaaaaa")),
                            new KeyValuePair<string, StringValues>("x-access-token",
                                new StringValues("just kidding"))
                        },
                        Path = "/yolo/solo/nolo/12345",
                        Query = new QueryCollection(
                            new Dictionary<string, StringValues>() {{"areYou", new StringValues("YesIAm")}}),
                        ContentType = "application/json",
                        ContentLength = requestBytes.Length
                    },
                    new RouteValueDictionary() {{"culpritId", "12345"}, {"whodat", "itsme"}}, null
                });
            Assert.Equal("[{\"Key\":\"culpritId\",\"Value\":[\"12345\"]},{\"Key\":\"whodat\",\"Value\":[\"itsme\"]}]", result);
        }

        [Fact]
        public void GetContentTest_ClaimUri_ExtractionRegex()
        {
            var requestBytes = Encoding.UTF8.GetBytes("{'IAm':'AFool', 'YouAreNotAFool':true}");
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("GetContent", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (string)methodInfo.Invoke(this,
                new object[]
                {
                    ClaimLocation.Uri, ExtractionType.RegEx, new DefaultHttpRequest(new DefaultHttpContext())
                    {
                        Body = new MemoryStream(requestBytes),
                        Method = HttpMethod.Post.ToString(),
                        Headers =
                        {
                            new KeyValuePair<string, StringValues>("Authorization",
                                new StringValues("Bearer hollaaaaaa")),
                            new KeyValuePair<string, StringValues>("x-access-token",
                                new StringValues("just kidding"))
                        },
                        Path = "/yolo/solo/nolo/12345",
                        Query = new QueryCollection(
                            new Dictionary<string, StringValues>() {{"areYou", new StringValues("YesIAm")}}),
                        ContentType = "application/json",
                        ContentLength = requestBytes.Length
                    },
                    new RouteValueDictionary() {{"culpritId", "12345"}}, null
                });
            Assert.Equal("/yolo/solo/nolo/12345", result);
        }

        [Fact]
        public void GetContentTest_ClaimUri_ExtractionType()
        {
            var requestBytes = Encoding.UTF8.GetBytes("{'IAm':'AFool', 'YouAreNotAFool':true}");
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("GetContent", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (string)methodInfo.Invoke(this,
                new object[]
                {
                    ClaimLocation.Uri, ExtractionType.Type, new DefaultHttpRequest(new DefaultHttpContext())
                    {
                        Body = new MemoryStream(requestBytes),
                        Method = HttpMethod.Post.ToString(),
                        Headers =
                        {
                            new KeyValuePair<string, StringValues>("Authorization",
                                new StringValues("Bearer hollaaaaaa")),
                            new KeyValuePair<string, StringValues>("x-access-token",
                                new StringValues("just kidding"))
                        },
                        Path = "/yolo/solo/nolo/12345",
                        Query = new QueryCollection(
                            new Dictionary<string, StringValues>() {{"areYou", new StringValues("YesIAm")}}),
                        ContentType = "application/json",
                        ContentLength = requestBytes.Length
                    },
                    new RouteValueDictionary() {{"culpritId", "12345"}}, null
                });
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetContentTest_ClaimQuery_ExtractionKeyValue()
        {
            var requestBytes = Encoding.UTF8.GetBytes("{'IAm':'AFool', 'YouAreNotAFool':true}");
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("GetContent", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (string) methodInfo.Invoke(this,
                new object[]
                {
                    ClaimLocation.QueryParameters, ExtractionType.KeyValue,
                    new DefaultHttpRequest(new DefaultHttpContext())
                    {
                        Body = new MemoryStream(requestBytes),
                        Method = HttpMethod.Post.ToString(),
                        Headers =
                        {
                            new KeyValuePair<string, StringValues>("Authorization",
                                new StringValues("Bearer hollaaaaaa")),
                            new KeyValuePair<string, StringValues>("x-access-token",
                                new StringValues("just kidding"))
                        },
                        Path = "/yolo/solo/nolo/12345",
                        Query = new QueryCollection(
                            new Dictionary<string, StringValues>()
                            {
                                {"areYou", new StringValues("YesIAm")},
                                {"spitItAllOut", new StringValues("YES")}
                            }),
                        ContentType = "application/json",
                        ContentLength = requestBytes.Length
                    },
                    new RouteValueDictionary() {{"culpritId", "12345"}}, null
                });
            Assert.Equal("[{\"Key\":\"areYou\",\"Value\":[\"YesIAm\"]},{\"Key\":\"spitItAllOut\",\"Value\":[\"YES\"]}]", result);
        }

        [Fact]
        public void GetContentTest_ClaimQuery_ExtractionRegex()
        {
            var requestBytes = Encoding.UTF8.GetBytes("{'IAm':'AFool', 'YouAreNotAFool':true}");
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("GetContent", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (string)methodInfo.Invoke(this,
                new object[]
                {
                    ClaimLocation.QueryParameters, ExtractionType.RegEx,
                    new DefaultHttpRequest(new DefaultHttpContext())
                    {
                        Body = new MemoryStream(requestBytes),
                        Method = HttpMethod.Post.ToString(),
                        Headers =
                        {
                            new KeyValuePair<string, StringValues>("Authorization",
                                new StringValues("Bearer hollaaaaaa")),
                            new KeyValuePair<string, StringValues>("x-access-token",
                                new StringValues("just kidding"))
                        },
                        Path = "/yolo/solo/nolo/12345",
                        Query = new QueryCollection(
                            new Dictionary<string, StringValues>()
                            {
                                {"areYou", new StringValues("YesIAm")},
                                {"spitItAllOut", new StringValues("YES")}
                            }),
                        ContentType = "application/json",
                        ContentLength = requestBytes.Length
                    },
                    new RouteValueDictionary() {{"culpritId", "12345"}}, null
                });
            Assert.Equal("?areYou=YesIAm&spitItAllOut=YES", result);
        }

        [Fact]
        public void GetContentTest_ClaimQuery_ExtractionType()
        {
            var requestBytes = Encoding.UTF8.GetBytes("{'IAm':'AFool', 'YouAreNotAFool':true}");
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("GetContent", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (string)methodInfo.Invoke(this,
                new object[]
                {
                    ClaimLocation.QueryParameters, ExtractionType.Type,
                    new DefaultHttpRequest(new DefaultHttpContext())
                    {
                        Body = new MemoryStream(requestBytes),
                        Method = HttpMethod.Post.ToString(),
                        Headers =
                        {
                            new KeyValuePair<string, StringValues>("Authorization",
                                new StringValues("Bearer hollaaaaaa")),
                            new KeyValuePair<string, StringValues>("x-access-token",
                                new StringValues("just kidding"))
                        },
                        Path = "/yolo/solo/nolo/12345",
                        Query = new QueryCollection(
                            new Dictionary<string, StringValues>()
                            {
                                {"areYou", new StringValues("YesIAm")},
                                {"spitItAllOut", new StringValues("YES")}
                            }),
                        ContentType = "application/json",
                        ContentLength = requestBytes.Length
                    },
                    new RouteValueDictionary() {{"culpritId", "12345"}}, null
                });
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetContentTest_ClaimNone_ExtractionType()
        {
            var requestBytes = Encoding.UTF8.GetBytes("{'IAm':'AFool', 'YouAreNotAFool':true}");
            var methodInfo = typeof(ExtractionUtils)
                .GetMethod("GetContent", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (string)methodInfo.Invoke(this,
                new object[]
                {
                    ClaimLocation.None, ExtractionType.Type,
                    new DefaultHttpRequest(new DefaultHttpContext())
                    {
                        Body = new MemoryStream(requestBytes),
                        Method = HttpMethod.Post.ToString(),
                        Headers =
                        {
                            new KeyValuePair<string, StringValues>("Authorization",
                                new StringValues("Bearer hollaaaaaa")),
                            new KeyValuePair<string, StringValues>("x-access-token",
                                new StringValues("just kidding"))
                        },
                        Path = "/yolo/solo/nolo/12345",
                        Query = new QueryCollection(
                            new Dictionary<string, StringValues>()
                            {
                                {"areYou", new StringValues("YesIAm")},
                                {"spitItAllOut", new StringValues("YES")}
                            }),
                        ContentType = "application/json",
                        ContentLength = requestBytes.Length
                    },
                    new RouteValueDictionary() {{"culpritId", "12345"}}, null
                });
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetClaimsAsyncTest()
        {
            var routeDef = new PostRouteDefinition(typeof(TestingType))
            {
                ClaimsConfig = new RouteClaimsConfig()
                {
                    ExtractionConfigs = new List<IValidClaimsExtractionConfig>()
                    {
                        new JsonPathClaimExtractionConfig("PityTheFoolJsonPathNo")
                            .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.No").Build(),
                        new JsonPathClaimExtractionConfig("PityTheFoolJsonPathYo")
                            .ConfigureExtraction(ExtractionFunctions.JsonPathFunc, "$.Yo").Build(),
                        new KeyValueClaimExtractionConfig("PityTheFoolKeyValueHeaders", ClaimLocation.Headers)
                            .ConfigureExtraction(ExtractionFunctions.KeyValueFunc, "Authorization").Build(),
                        new KeyValueClaimExtractionConfig("PityTheFoolKeyValueQuery", ClaimLocation.QueryParameters)
                            .ConfigureExtraction(ExtractionFunctions.KeyValueFunc, "areYou").Build(),
                        new RegexClaimExtractionConfig("PityTheFoolRegex", ClaimLocation.Uri)
                            .ConfigureExtraction(ExtractionFunctions.RegexFunc, new Regex("/yolo/nolo/(.*)/bolo"))
                            .Build(),
                        new TypeClaimExtractionConfig<TestingType>("PityTheFoolType")
                            .ConfigureExtraction((testingType) => { return Task.FromResult(testingType.No); }).Build()
                    }
                }
            };
            var requestBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new TestingType()
            {
                Yo = "Lo",
                No = "low"
            }));
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = new MemoryStream(requestBytes),
                Method = HttpMethod.Post.ToString(),
                Headers =
                {
                    new KeyValuePair<string, StringValues>("Authorization",
                        new StringValues("Bearer hollaaaaaa")),
                    new KeyValuePair<string, StringValues>("x-access-token",
                        new StringValues("just kidding"))
                },
                Path = "/yolo/nolo/12345/bolo",
                Query = new QueryCollection(
                    new Dictionary<string, StringValues>()
                    {
                        {"areYou", new StringValues("YesIAm")},
                        {"spitItAllOut", new StringValues("YES")}
                    }),
                ContentType = "application/json",
                ContentLength = requestBytes.Length
            };
            var claims = ExtractionUtils.GetClaimsAsync(routeDef, request, new RouteValueDictionary(TemplateParser.Parse("yolo/nolo/{culpridId}/bolo"))).Result;
            Assert.Equal(6, claims.Count);
            Assert.Equal("low", claims.First(x => x.Type == "PityTheFoolJsonPathNo").Value);
            Assert.Equal("Lo", claims.First(x => x.Type == "PityTheFoolJsonPathYo").Value);
            Assert.Equal("Bearer hollaaaaaa", claims.First(x => x.Type == "PityTheFoolKeyValueHeaders").Value);
            Assert.Equal("YesIAm", claims.First(x => x.Type == "PityTheFoolKeyValueQuery").Value);
            Assert.Equal("12345", claims.First(x => x.Type == "PityTheFoolRegex").Value);
            Assert.Equal("low", claims.First(x => x.Type == "PityTheFoolType").Value);
        }
    }
}
