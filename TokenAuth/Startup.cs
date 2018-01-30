using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiddlewareAuth.Config.Routing;
using MiddlewareAuth.Middleware;
using TokenAuth.Cache;
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
            services.AddMemoryCache();
            services.AddSingleton(typeof(MiddlewareAuth.Config.ConfigurationManager), new MiddlewareAuth.Config.ConfigurationManager());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMemoryCache memoryCache)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCustomClaimsValidation(new List<IRouteDefinitions> { new ValuesRoutes() });
            app.UseCustomClaimsValidationAsync(new CachedValidRouteDefinitionProvider(memoryCache)).Wait();
            app.UseMvc();
        }
    }
}
