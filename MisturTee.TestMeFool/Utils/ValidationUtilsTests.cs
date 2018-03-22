using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
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
using Newtonsoft.Json.Linq;
using Xunit;

namespace MisturTee.TestMeFool.Utils
{
    public class ValidationUtilsTests
    {
        [Fact]
        public void ValidateTests()
        {
            var validationConfigs = new List<ClaimValidationConfig>()
            {
                new ClaimValidationConfig()
                {
                    AllowNullOrEmpty = true,
                    ClaimName = "PityTheFool",
                    IsRequired = true,
                    ValueMustBeExactMatch = true
                },
                new ClaimValidationConfig()
                {
                    ValueMustBeExactMatch = true,
                    ClaimName = "MamasBoy",
                    IsRequired = true,
                    AllowNullOrEmpty = false
                }
            };
            var extractedClaims = new List<Claim>() {new Claim("PityTheFool", "I_Do"), new Claim("MamasBoy", "I_Am")};
            var expectedClaims = new List<Claim>() {new Claim("PityTheFool", "I_Do"), new Claim("MamasBoy", "I_Am")};

            var methodInfo = typeof(ValidationUtils)
                .GetMethod("Validate", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (ValidationResult) methodInfo.Invoke(this,
                new object[] { validationConfigs, extractedClaims, expectedClaims});
            Assert.Empty(result.InvalidClaims);
            Assert.Empty(result.MissingClaims);
            Assert.True(result.Success);
        }

        [Fact]
        public void ValidateTests_InvalidClaimValue()
        {
            var validationConfigs = new List<ClaimValidationConfig>()
            {
                new ClaimValidationConfig()
                {
                    AllowNullOrEmpty = true,
                    ClaimName = "PityTheFool",
                    IsRequired = true,
                    ValueMustBeExactMatch = true
                },
                new ClaimValidationConfig()
                {
                    ValueMustBeExactMatch = true,
                    ClaimName = "MamasBoy",
                    IsRequired = true,
                    AllowNullOrEmpty = false
                }
            };
            var extractedClaims = new List<Claim>() { new Claim("PityTheFool", "I_do"), new Claim("MamasBoy", "I_Am") };
            var expectedClaims = new List<Claim>() { new Claim("PityTheFool", "I_Do"), new Claim("MamasBoy", "I_Am") };

            var methodInfo = typeof(ValidationUtils)
                .GetMethod("Validate", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (ValidationResult)methodInfo.Invoke(this,
                new object[] { validationConfigs, extractedClaims, expectedClaims });
            Assert.Single(result.InvalidClaims);
            Assert.Empty(result.MissingClaims);
            Assert.False(result.Success);
        }
        
        [Fact]
        public void ValidateTests_RequiredClaimMissing()
        {
            var validationConfigs = new List<ClaimValidationConfig>()
            {
                new ClaimValidationConfig()
                {
                    AllowNullOrEmpty = true,
                    ClaimName = "PityTheFool",
                    IsRequired = true,
                    ValueMustBeExactMatch = true
                },
                new ClaimValidationConfig()
                {
                    ValueMustBeExactMatch = true,
                    ClaimName = "MamasBoy",
                    IsRequired = true,
                    AllowNullOrEmpty = false
                }
            };
            var extractedClaims = new List<Claim>() { new Claim("MamasBoy", "I_Am") };
            var expectedClaims = new List<Claim>() { new Claim("PityTheFool", "I_Do"), new Claim("MamasBoy", "I_Am") };

            var methodInfo = typeof(ValidationUtils)
                .GetMethod("Validate", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (ValidationResult)methodInfo.Invoke(this,
                new object[] { validationConfigs, extractedClaims, expectedClaims });
            Assert.Empty(result.InvalidClaims);
            Assert.Single(result.MissingClaims);
            Assert.Contains("PityTheFool", result.MissingClaims);
            Assert.False(result.Success);
        }


        [Fact]
        public void ValidateTests_ClaimMissingButNotRequired()
        {
            var validationConfigs = new List<ClaimValidationConfig>()
            {
                new ClaimValidationConfig()
                {
                    AllowNullOrEmpty = true,
                    ClaimName = "PityTheFool",
                    IsRequired = false,
                    ValueMustBeExactMatch = true
                },
                new ClaimValidationConfig()
                {
                    ValueMustBeExactMatch = true,
                    ClaimName = "MamasBoy",
                    IsRequired = true,
                    AllowNullOrEmpty = false
                }
            };
            var extractedClaims = new List<Claim>() { new Claim("MamasBoy", "I_Am") };
            var expectedClaims = new List<Claim>() { new Claim("PityTheFool", "I_Do"), new Claim("MamasBoy", "I_Am") };

            var methodInfo = typeof(ValidationUtils)
                .GetMethod("Validate", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (ValidationResult)methodInfo.Invoke(this,
                new object[] { validationConfigs, extractedClaims, expectedClaims });
            Assert.Empty(result.InvalidClaims);
            Assert.Empty(result.MissingClaims);
            Assert.True(result.Success);
        }

        [Fact]
        public void ValidateTests_ClaimPresentButNotExactMatch()
        {
            var validationConfigs = new List<ClaimValidationConfig>()
            {
                new ClaimValidationConfig()
                {
                    AllowNullOrEmpty = true,
                    ClaimName = "PityTheFool",
                    IsRequired = true,
                    ValueMustBeExactMatch = false
                },
                new ClaimValidationConfig()
                {
                    ValueMustBeExactMatch = true,
                    ClaimName = "MamasBoy",
                    IsRequired = true,
                    AllowNullOrEmpty = false
                }
            };
            var extractedClaims = new List<Claim>() { new Claim("PityTheFool", "yabbadabba"), new Claim("MamasBoy", "I_Am") };
            var expectedClaims = new List<Claim>() { new Claim("PityTheFool", "I_Do"), new Claim("MamasBoy", "I_Am") };

            var methodInfo = typeof(ValidationUtils)
                .GetMethod("Validate", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (ValidationResult)methodInfo.Invoke(this,
                new object[] { validationConfigs, extractedClaims, expectedClaims });
            Assert.Empty(result.InvalidClaims);
            Assert.Empty(result.MissingClaims);
            Assert.True(result.Success);
        }

        [Fact]
        public void ValidateTests_ClaimAllowNulOrEmpty()
        {
            var validationConfigs = new List<ClaimValidationConfig>()
            {
                new ClaimValidationConfig()
                {
                    AllowNullOrEmpty = true,
                    ClaimName = "PityTheFool",
                    IsRequired = true,
                    ValueMustBeExactMatch = false
                },
                new ClaimValidationConfig()
                {
                    ValueMustBeExactMatch = true,
                    ClaimName = "MamasBoy",
                    IsRequired = true,
                    AllowNullOrEmpty = false
                }
            };
            var extractedClaims = new List<Claim>() { new Claim("PityTheFool", ""), new Claim("MamasBoy", "I_Am") };
            var expectedClaims = new List<Claim>() { new Claim("PityTheFool", "I_Do"), new Claim("MamasBoy", "I_Am") };

            var methodInfo = typeof(ValidationUtils)
                .GetMethod("Validate", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (ValidationResult)methodInfo.Invoke(this,
                new object[] { validationConfigs, extractedClaims, expectedClaims });
            Assert.Empty(result.InvalidClaims);
            Assert.Empty(result.MissingClaims);
            Assert.True(result.Success);
        }

        [Fact]
        public void ValidateTests_ClaimRequiredButNotExpected()
        {
            var validationConfigs = new List<ClaimValidationConfig>()
            {
                new ClaimValidationConfig()
                {
                    AllowNullOrEmpty = true,
                    ClaimName = "PityTheFool",
                    IsRequired = true,
                    ValueMustBeExactMatch = false
                },
                new ClaimValidationConfig()
                {
                    ValueMustBeExactMatch = true,
                    ClaimName = "MamasBoy",
                    IsRequired = true,
                    AllowNullOrEmpty = false
                }
            };
            var extractedClaims = new List<Claim>() { new Claim("PityTheFool", "I_Do"), new Claim("MamasBoy", "I_Am") };
            var expectedClaims = new List<Claim>() { new Claim("MamasBoy", "I_Am") };

            var methodInfo = typeof(ValidationUtils)
                .GetMethod("Validate", BindingFlags.Static | BindingFlags.NonPublic);
            var result = (ValidationResult)methodInfo.Invoke(this,
                new object[] { validationConfigs, extractedClaims, expectedClaims });
            Assert.Empty(result.InvalidClaims);
            Assert.Empty(result.MissingClaims);
            Assert.True(result.Success);
        }

        [Fact]
        public void InternalValidateClaimsAsyncTest_NullRouteDef()
        {
            Assert.True(ValidationUtils
                .InternalValidateClaimsAsync(null, new DefaultHttpContext(), new RouteValueDictionary()).Result);
            var routeDefinition = new SpecialRouteTemplateRoutes().GetRouteDefinitions().First();
            routeDefinition.ClaimsConfig.ValidationConfigs = null;
            Assert.True(ValidationUtils
                .InternalValidateClaimsAsync(routeDefinition, new DefaultHttpContext(), new RouteValueDictionary())
                .Result);
            routeDefinition.ClaimsConfig = null;
            Assert.True(ValidationUtils
                .InternalValidateClaimsAsync(null, new DefaultHttpContext(), new RouteValueDictionary()).Result);
        }

        [Fact]
        public void InternalValidateClaimsAsyncTest()
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
                    },
                    ValidationConfigs = new List<ClaimValidationConfig>()
                    {
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolJsonPathNo",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolJsonPathYo",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolKeyValueHeaders",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolKeyValueQuery",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolRegex",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolType",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        }
                    }
                },
                RouteTemplate = "/yolo/nolo/{id:int}/bolo"
            };
            var requestBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new TestingType()
            {
                Yo = "Lo",
                No = "low"
            }));
            var httpContext = new DefaultHttpContext();
            var request = new DefaultHttpRequest(httpContext)
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
            httpContext.Items.Add("ExpectedClaims", new List<Claim>()
            {
                new Claim("PityTheFoolJsonPathNo", "low"),
                new Claim("PityTheFoolJsonPathYo", "Lo"),
                new Claim("PityTheFoolKeyValueHeaders", "Bearer hollaaaaaa"),
                new Claim("PityTheFoolKeyValueQuery", "YesIAm"),
                new Claim("PityTheFoolRegex", "12345"),
                new Claim("PityTheFoolType", "low"),
            });
            Assert.NotNull(request);
            Assert.True(ValidationUtils.InternalValidateClaimsAsync(routeDef, httpContext, new RouteValueDictionary(TemplateParser.Parse("yolo/nolo/{id:int}/bolo"))).Result);
        }

        [Fact]
        public void InternalValidateClaimsAsyncTest_Invalid()
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
                    },
                    ValidationConfigs = new List<ClaimValidationConfig>()
                    {
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolJsonPathNo",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolJsonPathYo",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolKeyValueHeaders",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolKeyValueQuery",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolRegex",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolType",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "Wildcard",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        }
                    },
                    BadRequestResponse = new BadRequestResponse()
                    {
                        Response = new ExpandoObject(),
                        Headers = new HeaderDictionary() { {"worked","true"} },
                        HttpStatusCode = HttpStatusCode.Forbidden
                    }
                },
                RouteTemplate = "/yolo/nolo/{id:int}/bolo"
            };
            var requestBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new TestingType()
            {
                Yo = "Lo",
                No = "low"
            }));
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var request = new DefaultHttpRequest(httpContext)
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
            httpContext.Items.Add("ExpectedClaims", new List<Claim>()
            {
                new Claim("PityTheFoolJsonPathNo", "low"),
                new Claim("PityTheFoolJsonPathYo", "Lo"),
                new Claim("PityTheFoolKeyValueHeaders", "Bearer hollaaaaaa"),
                new Claim("PityTheFoolKeyValueQuery", "YesIAm"),
                new Claim("PityTheFoolRegex", "12345"),
                new Claim("PityTheFoolType", "low"),
                new Claim("Wildcard", "whoooooooooo")
            });
            Assert.NotNull(request);
            Assert.False(ValidationUtils.InternalValidateClaimsAsync(routeDef, httpContext, new RouteValueDictionary(TemplateParser.Parse("yolo/nolo/{id:int}/bolo"))).Result);
            Assert.Equal((int)HttpStatusCode.Forbidden, httpContext.Response.StatusCode);
            Assert.Single(httpContext.Response.Headers.Where(x => x.Key == "worked"));
            httpContext.Response.Body.Position = 0;
            var responseString = new StreamReader(httpContext.Response.Body).ReadToEnd();
            var responseDeserialized = JsonConvert.DeserializeObject<dynamic>(responseString);
            Assert.Equal("Wildcard", ((JArray)responseDeserialized.MissingClaims)[0].Value<string>());
        }

        [Fact]
        public void ValidateClaimsAsyncTest()
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
                    },
                    ValidationConfigs = new List<ClaimValidationConfig>()
                    {
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolJsonPathNo",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolJsonPathYo",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolKeyValueHeaders",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolKeyValueQuery",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolRegex",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolType",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        }
                    }
                },
                RouteTemplate = "/yolo/nolo/{id:int}/bolo"
            };
            var requestBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new TestingType()
            {
                Yo = "Lo",
                No = "low"
            }));
            var httpContext = new DefaultHttpContext();
            var request = new DefaultHttpRequest(httpContext)
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
            httpContext.Items.Add("ExpectedClaims", new List<Claim>()
            {
                new Claim("PityTheFoolJsonPathNo", "low"),
                new Claim("PityTheFoolJsonPathYo", "Lo"),
                new Claim("PityTheFoolKeyValueHeaders", "Bearer hollaaaaaa"),
                new Claim("PityTheFoolKeyValueQuery", "YesIAm"),
                new Claim("PityTheFoolRegex", "12345"),
                new Claim("PityTheFoolType", "low"),
            });
            Assert.NotNull(request);
            Assert.True(ValidationUtils.ValidateClaimsAsync(routeDef, httpContext, new RouteValueDictionary(TemplateParser.Parse("yolo/nolo/{id:int}/bolo"))).Result);
        }

        [Fact]
        public void ValidateClaimsAsyncTest_Invalid()
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
                    },
                    ValidationConfigs = new List<ClaimValidationConfig>()
                    {
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolJsonPathNo",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolJsonPathYo",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolKeyValueHeaders",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolKeyValueQuery",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolRegex",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "PityTheFoolType",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        },
                        new ClaimValidationConfig()
                        {
                            AllowNullOrEmpty = false,
                            ClaimName = "Wildcard",
                            IsRequired = true,
                            ValueMustBeExactMatch = true
                        }
                    },
                    BadRequestResponse = new BadRequestResponse()
                    {
                        Response = new ExpandoObject(),
                        Headers = new HeaderDictionary() { { "worked", "true" } },
                        HttpStatusCode = HttpStatusCode.Forbidden
                    }
                },
                RouteTemplate = "/yolo/nolo/{id:int}/bolo"
            };
            var requestBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new TestingType()
            {
                Yo = "Lo",
                No = "low"
            }));
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var request = new DefaultHttpRequest(httpContext)
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
            httpContext.Items.Add("ExpectedClaims", new List<Claim>()
            {
                new Claim("PityTheFoolJsonPathNo", "low"),
                new Claim("PityTheFoolJsonPathYo", "Lo"),
                new Claim("PityTheFoolKeyValueHeaders", "Bearer hollaaaaaa"),
                new Claim("PityTheFoolKeyValueQuery", "YesIAm"),
                new Claim("PityTheFoolRegex", "12345"),
                new Claim("PityTheFoolType", "low"),
                new Claim("Wildcard", "whoooooooooo")
            });
            Assert.NotNull(request);
            Assert.False(ValidationUtils.ValidateClaimsAsync(routeDef, httpContext, new RouteValueDictionary(TemplateParser.Parse("yolo/nolo/{id:int}/bolo"))).Result);
            Assert.Equal((int)HttpStatusCode.Forbidden, httpContext.Response.StatusCode);
            Assert.Single(httpContext.Response.Headers.Where(x => x.Key == "worked"));
            httpContext.Response.Body.Position = 0;
            var responseString = new StreamReader(httpContext.Response.Body).ReadToEnd();
            var responseDeserialized = JsonConvert.DeserializeObject<dynamic>(responseString);
            Assert.Equal("Wildcard", ((JArray)responseDeserialized.MissingClaims)[0].Value<string>());
        }

    }
}
