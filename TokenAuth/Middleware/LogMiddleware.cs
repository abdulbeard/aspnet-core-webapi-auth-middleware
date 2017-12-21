using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TokenAuth.Config.Routing;
using TokenAuth.Utils;

namespace TokenAuth.Middleware
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task Invoke(HttpContext context)
        {
            var httpAction = context.Request.Method;
            RouteDefinition matchingRouteDefinition = null;
            foreach (var action in ValuesRoutes.RouteDefinitions[httpAction])
            {
                var templateMatcher = new TemplateMatcher(action.RouteTemplate, RoutesUtils.GetDefaults(action.RouteTemplate));
                var routeValues = RoutesUtils.GetDefaults(action.RouteTemplate);
                var routeIsAMatch = templateMatcher.TryMatch(context.Request.Path, routeValues);
                if (routeIsAMatch)
                {
                    matchingRouteDefinition = action;
                    break;
                }
            }

            var bodyMemStream = new MemoryStream();
            context.Request.Body.CopyTo(bodyMemStream);
            context.Request.Body.Position = 0;
            var stringBody = System.Text.Encoding.UTF8.GetString(bodyMemStream.ToArray());
            var extractedClaims = (await Task.WhenAll(matchingRouteDefinition.ClaimsConfig.ExtractionConfigs.Select(x => x.GetClaimAsync())).ConfigureAwait(false)).ToList();
            var requiredClaims = matchingRouteDefinition.ClaimsConfig.ValidationConfig.Where(x => x.IsRequired);
            var missingRequiredClaims = requiredClaims.Select(x => x.ClaimName).Where(x => !extractedClaims.Select(y => y.Type).Contains(x)).Select(x => x);
            // if required claims are present, match claims to confirm all is good


            return _next(context);
        }
    }
}
