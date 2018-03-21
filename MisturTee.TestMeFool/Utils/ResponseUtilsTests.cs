using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using MisturTee.TestMeFool.Claims;
using MisturTee.Utils;
using Newtonsoft.Json;
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
                    JsonConvert.SerializeObject(new TestingType {Yo = "lo", No = "low"}));
            response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
            response.Headers.Add(new KeyValuePair<string, StringValues>("yolo", new StringValues("solo")));
            response.StatusCode = StatusCodes.Status502BadGateway;
            var methodInfo = typeof(ResponseUtils)
                .GetMethod("BuildResponseFromResponse", BindingFlags.Static | BindingFlags.NonPublic);
            methodInfo.Invoke(this, new object[] {response, httpContext});
            httpContext.Response.Body.Position = 0;
            var responseString = new StreamReader(httpContext.Response.Body).ReadToEnd();
            var responseDeserialized = JsonConvert.DeserializeObject<TestingType>(responseString);
            Assert.Equal(502, httpContext.Response.StatusCode);
            Assert.Equal("lo", responseDeserialized.Yo);
            Assert.Equal("low", responseDeserialized.No);
            Assert.Equal(1, httpContext.Response.Headers.Count);
            Assert.Equal("solo", httpContext.Response.Headers.First().Value);
        }
    }
}
