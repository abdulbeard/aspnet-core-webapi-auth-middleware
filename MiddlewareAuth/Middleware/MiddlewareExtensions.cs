using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MiddlewareAuth.Config.Routing;

namespace MiddlewareAuth.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomRoutingAndClaimsValidation(this IApplicationBuilder app, IEnumerable<IRouteDefinitions> routeDefs)
        {
            var routeDefinitions = routeDefs.SelectMany(x => x.GetRouteDefinitions())
                .Select(x => new InternalRouteDefinition(x))
                .GroupBy(x => x.Method.Method)
                .ToDictionary(x => x.Key, x => x?.ToList() ?? new List<InternalRouteDefinition>());
            CustomRoutingAndClaimsValidationMiddleware.RegisterRoutes(routeDefinitions);
            app.UseMiddleware<CustomRoutingAndClaimsValidationMiddleware>();
            return app;
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
