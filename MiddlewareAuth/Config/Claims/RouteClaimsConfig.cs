using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MiddlewareAuth.Config.Claims.ExtractionConfigs;

namespace MiddlewareAuth.Config.Claims
{
    public class RouteClaimsConfig
    {
        public RouteClaimsConfig()
        {
            MissingClaimsResponse = new MissingClaimsResponse();
        }
        public IList<IValidClaimsExtractionConfig> ExtractionConfigs { get; set; }
        public IList<ClaimValidationConfig> ValidationConfig { get; set; }
        public MissingClaimsResponse MissingClaimsResponse { get; set; }
    }

    public class MissingClaimsResponse
    {
        public MissingClaimsResponse()
        {
            HttpStatusCode = HttpStatusCode.Forbidden;
            dynamic response = new ExpandoObject();
            response.ErrorCode = 1235;
            response.Message = "The following claims require values";
            Response = response;
        }
        public HttpStatusCode HttpStatusCode { get; set; }
        public ExpandoObject Response { get; set; }
        public Func<IEnumerable<string>, Task<HttpResponse>> MissingClaimsResponseOverride { get; set; }
    }
}
