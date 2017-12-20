using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenAuth.Config.Routing;

namespace TokenAuth.Middleware
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public Task Invoke(HttpContext context)
        {
            var routeData = context.GetRouteData();
            System.Diagnostics.Debug.WriteLine((context.Request.Path));
            var templateMatcher = new TemplateMatcher(ValuesRoutes.GetByIdGuidTemplate, ValuesRoutes.GetDefaults(ValuesRoutes.GetByIdGuidTemplate));
            var sdfdsf = templateMatcher.TryMatch(context.Request.Path, ValuesRoutes.GetDefaults(ValuesRoutes.GetByIdGuidTemplate));
            //System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(routeData.DataTokens));
            return _next(context);
        }
    }
}
