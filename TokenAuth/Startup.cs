using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MiddlewareAuth.Auth;
using MiddlewareAuth.Config.Routing;
using MiddlewareAuth.Middleware;
using TokenAuth.Routes;

namespace TokenAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(typeof(MiddlewareAuth.Config.ConfigurationManager), new MiddlewareAuth.Config.ConfigurationManager());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCustomRoutingAndClaimsValidation(new List<IRouteDefinitions> { new ValuesRoutes() });
            //app.UseLoggingMiddleware();
            //app.UseCustomRouting();
            app.UseMvc();
            //app.UseAuthentication();
        }
    }
}
