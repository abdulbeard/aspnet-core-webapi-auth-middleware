using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using MisturTee.TestMeFool.Claims;
using MisturTee.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MisturTee.TestMeFool.Utils
{
    public class ResponseUtilsTests
    {
        [Fact]
        public void BuildResponseFromResponseTest()
        {
            //BuildResponseFromResponse(HttpResponse overrideResponse, HttpContext context)
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var response = new DefaultHttpResponse(new DefaultHttpContext())
            {
                Body = new MemoryStream()
            };
            var responseBytes =
                System.Text.Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(new TestingType { Yo = "lo", No = "low" }));
            response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
            response.Headers.Add(new KeyValuePair<string, StringValues>("yolo", new StringValues("solo")));
            response.StatusCode = StatusCodes.Status502BadGateway;
            var methodInfo = typeof(ResponseUtils)
                .GetMethod("BuildResponseFromResponse", BindingFlags.Static | BindingFlags.NonPublic);
            methodInfo.Invoke(this, new object[] { response, httpContext });
            httpContext.Response.Body.Position = 0;
            var responseString = new StreamReader(httpContext.Response.Body).ReadToEnd();
            var responseDeserialized = JsonConvert.DeserializeObject<TestingType>(responseString);
            Assert.Equal(502, httpContext.Response.StatusCode);
            Assert.Equal("lo", responseDeserialized.Yo);
            Assert.Equal("low", responseDeserialized.No);
            Assert.Equal(1, httpContext.Response.Headers.Count);
            Assert.Equal("solo", httpContext.Response.Headers.First().Value);
        }

        [Fact]
        public void CreateResponseTest()
        {
            var validationResult = new ValidationResult()
            {
                InvalidClaims = new List<InvalidClaimResult>()
                {
                    new InvalidClaimResult()
                    {
                        ActualValue = "NO",
                        ClaimName = "PityTheFool",
                        ExpectedValue = "Always"
                    }
                },
                MissingClaims = new List<string>() { "You_Cross_Me_Im_Going_To_Hurt_You", "Whatever_Role_I_Play_Is_A_Positive_Role"}
            };
            var badRequestResponse = new Config.Claims.BadRequestResponse()
            {
                BadRequestResponseOverride = null,
                Headers = new HeaderDictionary(new Dictionary<string, StringValues>() { { "I_Dont_Do_Shakespeare", new StringValues("I_Dont_Talk_In_That_Kind_Of_Broken_English") } }),
                HttpStatusCode = System.Net.HttpStatusCode.Conflict,
                Response = new System.Dynamic.ExpandoObject()
            };
            ((dynamic)badRequestResponse.Response).Yo = "Lo";
            ((dynamic)badRequestResponse.Response).No = "Low";
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            ResponseUtils.CreateResponse(validationResult, httpContext, badRequestResponse).Wait();
            var response = httpContext.Response;
            Assert.Equal((int)System.Net.HttpStatusCode.Conflict, response.StatusCode);
            Assert.Single(response.Headers.Where(x => x.Key == "I_Dont_Do_Shakespeare" && x.Value == new StringValues("I_Dont_Talk_In_That_Kind_Of_Broken_English")));
            Assert.Single(response.Headers.Where(x => x.Key == "Content-Type" && x.Value == new StringValues("application/json")));
            httpContext.Response.Body.Position = 0;
            var responseString = new StreamReader(httpContext.Response.Body).ReadToEnd();
            dynamic responseDeserialized = JsonConvert.DeserializeObject<dynamic>(responseString);
            Assert.Equal("Lo", responseDeserialized.Yo.Value);
            Assert.Equal("Low", responseDeserialized.No.Value);
            Assert.Equal("You_Cross_Me_Im_Going_To_Hurt_You", ((JArray) responseDeserialized.MissingClaims)[0].Value<string>());
            Assert.Equal("Whatever_Role_I_Play_Is_A_Positive_Role", (((JArray) responseDeserialized.MissingClaims)[1]).Value<string>());
            var invalidClaim = ((JArray)responseDeserialized.InvalidClaims)[0].Value<dynamic>();
            Assert.Equal("PityTheFool", invalidClaim.ClaimName.Value);
            Assert.Equal("NO", invalidClaim.Value.Value);
        }

        [Fact]
        public void CreateResponseTest_BadRequestResponseOverride()
        {
            var validationResult = new ValidationResult()
            {
                InvalidClaims = new List<InvalidClaimResult>()
                {
                    new InvalidClaimResult()
                    {
                        ActualValue = "NO",
                        ClaimName = "PityTheFool",
                        ExpectedValue = "Always"
                    }
                },
                MissingClaims = new List<string>() { "You_Cross_Me_Im_Going_To_Hurt_You", "Whatever_Role_I_Play_Is_A_Positive_Role" }
            };
            var badRequestResponse = new Config.Claims.BadRequestResponse()
            {
                BadRequestResponseOverride = (missingClaims, invalidClaims) => 
                {
                    var response = new DefaultHttpResponse(new DefaultHttpContext())
                    {
                        Body = new MemoryStream()
                    };
                    var responseBytes =
                        System.Text.Encoding.UTF8.GetBytes(
                            JsonConvert.SerializeObject(new TestingType { Yo = "lo", No = "low" }));
                    response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
                    response.Headers.Add(new KeyValuePair<string, StringValues>("yolo", new StringValues("solo")));
                    response.StatusCode = StatusCodes.Status502BadGateway;
                    return Task.FromResult((HttpResponse) response);
                },
                Headers = new HeaderDictionary(new Dictionary<string, StringValues>() { { "I_Dont_Do_Shakespeare", new StringValues("I_Dont_Talk_In_That_Kind_Of_Broken_English") } }),
                HttpStatusCode = System.Net.HttpStatusCode.Conflict,
                Response = new System.Dynamic.ExpandoObject()
            };
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            ResponseUtils.CreateResponse(validationResult, httpContext, badRequestResponse).Wait();
            httpContext.Response.Body.Position = 0;
            Assert.Equal((int)System.Net.HttpStatusCode.BadGateway, httpContext.Response.StatusCode);
            Assert.Single(httpContext.Response.Headers.Where(x=>x.Key == "yolo"));
            Assert.Equal("solo", httpContext.Response.Headers.First(x => x.Key == "yolo").Value);
            var responseDeserialized = JsonConvert.DeserializeObject<TestingType>((new StreamReader(httpContext.Response.Body).ReadToEnd()));
            Assert.Equal("lo", responseDeserialized.Yo);
            Assert.Equal("low", responseDeserialized.No);
        }
    }
}
