using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using MisturTee.Utils;

namespace MisturTee.Config.Claims
{
    public class RouteClaimsConfig
    {
        public RouteClaimsConfig()
        {
            BadRequestResponse = new BadRequestResponse();
        }
        public IList<IValidClaimsExtractionConfig> ExtractionConfigs { get; set; }
        public IList<ClaimValidationConfig> ValidationConfigs { get; set; }
        public BadRequestResponse BadRequestResponse { get; set; }
    }

    public class BadRequestResponse
    {
        public BadRequestResponse()
        {
            HttpStatusCode = HttpStatusCode.Forbidden;
            dynamic response = new ExpandoObject();
            response.ErrorCode = 1235;
            response.Message = "The following claims require values";
            Response = response;
        }
        public HeaderDictionary Headers { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public ExpandoObject Response { get; set; }
        public BadRequestResponseOverride BadRequestResponseOverride { get; set; }
    }

    /// <summary>
    /// Function that returns <see cref="HttpResponse"/> which overrides the response defined by <see cref="BadRequestResponse"/>
    /// </summary>
    /// <param name="missingClaims">list of missing claims</param>
    /// <returns></returns>
    public delegate Task<HttpResponse> BadRequestResponseOverride(IEnumerable<string> missingClaims, IEnumerable<InvalidClaimResult> invalidClaims);
}
