using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace OnlineOrderingAPI.Authorization
{
    public class AccessTokenHandler : DelegatingHandler
    {
        //private readonly IAuthorizationConfigManager _authorizationConfigManager;
        //private readonly IEncryptor _accessTokenEncryptor;
        //private readonly ISettingsUtils _settingsUtils;

        public AccessTokenHandler(
            //IAuthorizationConfigManager authorizationConfigManager,
            //IEncryptor accessTokenEncryptor, ISettingsUtils settingsUtils
            )
        {
            //_authorizationConfigManager = authorizationConfigManager.ThrowIfNull(nameof(authorizationConfigManager));
            //_accessTokenEncryptor = accessTokenEncryptor.ThrowIfNull(nameof(accessTokenEncryptor));
            //_settingsUtils = settingsUtils;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var route = GetRoute(request);
            //var headerUtils = new HeaderUtils();
            var skipTokenValidation = false;
            if (skipTokenValidation)
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(true);
            }

            //var accessTokenHeader = headerUtils.GetAccessTokenHeader(request);
            //if (await SkipAccessTokenValidationAsync(route, accessTokenHeader).ConfigureAwait(true))
            //{
            //    return await base.SendAsync(request, cancellationToken).ConfigureAwait(true);
            //}
            //if (!RequestIsJson(request))//means that useAccessToken is true, but the request isn't json - this scenario is not supported
            //{
            //    return CreateUnacceptableContentTypeResponse(request);
            //}

            //if (string.IsNullOrEmpty(accessTokenHeader))
            //{
            //    return CreateFailedResponse("You are unauthorized to make this call.", request);
            //}

            //var accessToken = GetAccessToken(accessTokenHeader);
            //if (accessToken == null || !IsAuthenticated(accessToken, request))
            //{
            //    return CreateFailedResponse("You are unauthorized to make this call.", request);
            //}

            //var authorizationResult = await AuthorizeAsync(accessToken, request, route).ConfigureAwait(true);
            //if (authorizationResult != null && !authorizationResult.Success)
            //{
            //    return CreateFailedResponse(authorizationResult.Message, request);
            //}

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(true);
        }

        //public async Task<bool> SkipAccessTokenValidationAsync(string route, string accessTokenHeader)
        //{
        //    var useAccessToken = await CompanyUsesAccessTokenAsync().ConfigureAwait(true) ||
        //                         !string.IsNullOrEmpty(accessTokenHeader);
        //    return !useAccessToken || string.IsNullOrEmpty(route) ||
        //         !await IsRouteMonitoredAsync(route).ConfigureAwait(true);
        //}

        public virtual string GetRoute(HttpRequestMessage request)
        {
            return "";
            //var routeDataValues = request.GetConfiguration()?.Routes?.GetRouteData(request)?.Values;
            //var result = "";
            //if (routeDataValues != null)
            //{
            //    var routeKey = ((IHttpRouteData[])routeDataValues[routeDataValues.Keys.FirstOrDefault()]);
            //    if (RequestThatMapsToSecondRoute(request))
            //    {
            //        result = routeKey?.ElementAt(1)?.Route?.RouteTemplate ?? string.Empty;
            //    }
            //    else if (routeKey?.FirstOrDefault()?.Values?.Any(rd => rd.Key == "id" && string.Equals(rd.Value?.ToString(), "LoginUpdate", StringComparison.OrdinalIgnoreCase)) == true)
            //    {
            //        result = "v1/Customers/LoginUpdate";
            //    }
            //    else if (routeKey?.FirstOrDefault()?.Values?.Any(rd => rd.Key == "id" && string.Equals(rd.Value?.ToString(), "PasswordReset", StringComparison.OrdinalIgnoreCase)) == true)
            //    {
            //        result = "v1/Customers/PasswordReset";
            //    }
            //    else
            //    {
            //        result = routeKey?.FirstOrDefault()?.Route?.RouteTemplate ?? string.Empty;
            //    }
            //}
            //return string.IsNullOrEmpty(result) ? result : Routes.GetRoute(request.Method, result);
        }

        protected async Task<bool> IsRouteMonitoredAsync(string route)
        {
            return true;
            //return (await GetValidationConfigAsync().ConfigureAwait(true)).IsMonitored(route);
        }

        //protected Task<IAccessTokenValidationConfig> GetValidationConfigAsync()
        //{
        //    return _authorizationConfigManager.GetConfigurationAsync();
        //}

    //    public AccessToken GetAccessToken(string accessToken)
    //    {
    //        return AccessToken.FromEncryptedString(accessToken, _accessTokenEncryptor);
    //    }

    //    protected bool IsAuthenticated(AccessToken token, HttpRequestMessage request)
    //    {
    //        return string.Equals(token.Data.AuthenticationSignature, GetAuthenticationSignature(request),
    //            StringComparison.Ordinal);
    //    }

    //    protected string GetAuthenticationSignature(HttpRequestMessage request)
    //    {
    //        var authHeader = request.Headers?.Authorization?.Parameter;
    //        return string.IsNullOrEmpty(authHeader)
    //            ? string.Empty
    //            : System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authHeader)).Split(':').FirstOrDefault();
    //    }

    //    public HttpResponseMessage CreateFailedResponse(string message, HttpRequestMessage request)
    //    {
    //        return request.CreateResponse(HttpStatusCode.Unauthorized,
    //            APIErrorUtils.CreateAPIError(Common.Models.v1.ErrorCode.AccessDenied, message));
    //    }

    //    public HttpResponseMessage CreateUnacceptableContentTypeResponse(HttpRequestMessage request)
    //    {
    //        return request.CreateResponse(HttpStatusCode.BadRequest,
    //            APIErrorUtils.CreateAPIError(ErrorCode.ContentTypeInvalid,
    //                "Only application/json Content-Type is supported."));
    //    }

    //    public async Task<AccessTokenValidationResult> AuthorizeAsync(AccessToken token, HttpRequestMessage request,
    //        string route)
    //    {
    //        var customerInfo = await GetCustomerInfoAsync(route, request).ConfigureAwait(true);
    //        return await ValidateAccessTokenAsync(customerInfo, token, route).ConfigureAwait(true);
    //    }

    //    public async Task<CustomerInfo> GetCustomerInfoAsync(string route, HttpRequestMessage httpRequest)
    //    {
    //        var extractionType = (await GetValidationConfigAsync().ConfigureAwait(true)).GetExtractionType(route);
    //        switch (extractionType)
    //        {
    //            case ExtractionType.None:
    //                {
    //                    return null;
    //                }
    //            case ExtractionType.Path:
    //                {
    //                    var configForThisRoute =
    //                        (await GetValidationConfigAsync().ConfigureAwait(true)).GetExtractionConfigForRoute(route);
    //                    return await GetCustomerInfoFromRequestAsync(httpRequest, configForThisRoute)
    //                        .ConfigureAwait(true);
    //                }
    //            case ExtractionType.Type:
    //                {
    //                    return await TypeRouteConfig.GetCustomerInfoAsync(route, httpRequest).ConfigureAwait(true);
    //                }
    //        }
    //        return null;
    //    }

    //    public async Task<CustomerInfo> GetCustomerInfoFromRequestAsync(HttpRequestMessage request,
    //        RouteMonitoringObjectFromConfig config)
    //    {
    //        var result = new CustomerInfo();
    //        if (!string.IsNullOrEmpty(config.PathToCustomerId))
    //        {
    //            var id =
    //                await GetIdFromLocationAsync(config.LocationOfCustomerId, config.PathToCustomerId, request)
    //                    .ConfigureAwait(true);

    //            var customerId = Guid.Empty;
    //            result.CustomerId = !Guid.TryParse(id, out customerId) ? Guid.Empty : customerId;

    //        }
    //        if (!string.IsNullOrEmpty(config.PathToLoyaltyId))
    //        {
    //            result.LoyaltyId =
    //                await GetIdFromLocationAsync(config.LocationOfLoyaltyId, config.PathToLoyaltyId, request)
    //                    .ConfigureAwait(true);
    //        }
    //        return result;
    //    }

    //    public async Task<string> GetIdFromLocationAsync(ClaimLocation location, string pathToId,
    //        HttpRequestMessage request)
    //    {
    //        switch (location)
    //        {
    //            case ClaimLocation.Body:
    //                {
    //                    var requestString = await request.Content.ReadAsStringAsync().ConfigureAwait(true);
    //                    return GetIdFromJson(requestString, pathToId);
    //                }
    //            case ClaimLocation.Headers:
    //                {
    //                    var headersJson = JsonConvert.SerializeObject(request.Headers);
    //                    return GetIdFromJson(headersJson, pathToId, true);
    //                }
    //            case ClaimLocation.Uri:
    //                {
    //                    var idmatch = Regex.Match(request.RequestUri.PathAndQuery, pathToId);
    //                    return idmatch.Success ? idmatch.Groups[1].Value : string.Empty;
    //                }
    //            case ClaimLocation.QueryParameters:
    //                {
    //                    var queryParams = request.RequestUri.ParseQueryString();
    //                    var queryParameterKey =
    //                        queryParams.AllKeys.FirstOrDefault(
    //                            x => string.Equals(x, pathToId, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
    //                    return
    //                            !string.IsNullOrEmpty(queryParameterKey)
    //                            ? queryParams[queryParameterKey]
    //                            : string.Empty;
    //                }
    //            default:
    //                {
    //                    return string.Empty;
    //                }
    //        }
    //    }

    //    public bool RequestIsJson(HttpRequestMessage request)
    //    {
    //        return ((request?.Content?.Headers?.ContentType?.MediaType ?? string.Empty) == string.Empty) ||
    //               string.Equals(request?.Content?.Headers?.ContentType?.MediaType ?? string.Empty, "application/json",
    //                   StringComparison.OrdinalIgnoreCase);
    //    }

    //    public string GetIdFromJson(string json, string jsonPath, bool jsonIsArray = false)
    //    {
    //        if (jsonIsArray)
    //        {
    //            var jobject = JArray.Parse(json);
    //            return jobject.SelectToken(jsonPath).ToString();
    //        }
    //        else
    //        {
    //            var jobject = JObject.Parse(json);
    //            return jobject.SelectToken(jsonPath).ToString();
    //        }
    //    }

    //    public async Task<AccessTokenValidationResult> ValidateAccessTokenAsync(CustomerInfo customerInfo, AccessToken token, string route)
    //    {
    //        if (token.Data.Expiration <= DateTime.UtcNow)
    //        {
    //            return new AccessTokenValidationResult
    //            {
    //                Success = false,
    //                Message = "Your token has expired"
    //            };
    //        }
    //        var idsToValidate = (await GetValidationConfigAsync().ConfigureAwait(true)).GetIdTypeToValidate(route);

    //        if (!IdsAreValid(idsToValidate.HasFlag(IdValidationType.CustomerId),
    //            idsToValidate.HasFlag(IdValidationType.LoyaltyId), token, route, customerInfo))
    //        {
    //            return new AccessTokenValidationResult
    //            {
    //                Success = false,
    //                Message = "You are not authorized to make requests for this customer."
    //            };
    //        }
    //        return new AccessTokenValidationResult
    //        {
    //            Success = true,
    //            Message = "Bon Voyage!"
    //        };
    //    }

    //    public bool IdsAreValid(bool customerIdValidationRequested, bool loyaltyIdValidationRequested, AccessToken token,
    //        string route, CustomerInfo customerInfo)
    //    {
    //        if (customerIdValidationRequested && customerInfo != null && customerInfo.CustomerId != Guid.Empty)
    //        {
    //            if (token.Data.CustomerId != customerInfo.CustomerId)
    //            {
    //                return false;
    //            }
    //        }
    //        if (loyaltyIdValidationRequested && !string.IsNullOrEmpty(customerInfo?.LoyaltyId))
    //        {
    //            if (!string.Equals(token.Data.LoyaltyId, customerInfo.LoyaltyId, StringComparison.Ordinal))
    //            {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }
    }

    //public class AccessTokenValidationResult
    //{
    //    public bool Success { get; set; }
    //    public string Message { get; set; }
    //}
}