using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using MisturTee.Utils;
using Xunit;

namespace MisturTee.TestMeFool
{
    public class RouteClaimsConfigTest
    {
        [Fact]
        public void BadRequestResponse()
        {
            dynamic response = new ExpandoObject();
            response.ErrorCode = 1235;
            response.Message = "The following claims require values";

            var badRequestResponse = new BadRequestResponse()
            {
                Headers = new HeaderDictionary(),
                HttpStatusCode = HttpStatusCode.Ambiguous,
                Response = response,
                BadRequestResponseOverride = (claims, invalidClaims) =>
                {
                    return Task.FromResult((HttpResponse)new DefaultHttpResponse(new DefaultHttpContext()
                    {
                        Response =
                        {
                            Body = new MemoryStream(Encoding.UTF8.GetBytes("yo this is awesome")),
                            StatusCode = 500,
                        }
                    }));
                }
            };
            Assert.Equal(HttpStatusCode.Ambiguous, badRequestResponse.HttpStatusCode);
            Assert.Equal(response, badRequestResponse.Response);
            Assert.NotNull(badRequestResponse.BadRequestResponseOverride);
            var badRequestResponseOverrideResponse = badRequestResponse.BadRequestResponseOverride.Invoke(new List<string>(), new List<InvalidClaimResult>());
            Assert.Equal(badRequestResponseOverrideResponse.Result.StatusCode, (int)HttpStatusCode.InternalServerError);
            var responseByteArray = new byte[badRequestResponseOverrideResponse.Result.Body.Length];
            badRequestResponseOverrideResponse.Result.Body.Read(responseByteArray, 0, (int) badRequestResponseOverrideResponse.Result.Body.Length);
            Assert.Equal("yo this is awesome", Encoding.UTF8.GetString(responseByteArray));
        }
    }
}
