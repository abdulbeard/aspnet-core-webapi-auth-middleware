using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenAuth.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogMiddleware>();
        }

        public static IApplicationBuilder UseCustomRouting(this IApplicationBuilder app)
        {
            var routeBuilder = new RouteBuilder(app);
            routeBuilder.MapGet("shabbargullah/{name}", x =>
            {
                var name = x.GetRouteValue("name");
                // This is the route handler when HTTP GET "hello/<anything>"  matches
                // To match HTTP GET "hello/<anything>/<anything>,
                // use routeBuilder.MapGet("hello/{*name}"
                x.Response.Redirect("values/");
                return Task.CompletedTask;
                //return x.Response.WriteAsync($"Hi, {name}!");
            });
            routeBuilder.MapGet("shabbargullah/{id:guid}/yolo/{manafort}/sup/{manaforts:guid}", y =>
            {
                var id = y.GetRouteValue("id");
                var manafort = y.GetRouteValue("manafort");
                var manaforts = y.GetRouteValue("manaforts");
                y.Response.Redirect("values/");
                return Task.CompletedTask;
            });
            app.UseRouter(routeBuilder.Build());
            return app;
        }
    }
}
