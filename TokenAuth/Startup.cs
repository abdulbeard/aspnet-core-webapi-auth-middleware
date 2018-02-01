using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MisturTee.Config;
using MisturTee.Middleware;
using TokenAuth.Cache;
using TokenAuth.Middleware;

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
            services.AddSingleton(typeof(ConfigurationManager), new ConfigurationManager());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMemoryCache memoryCache)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<JwtMiddleware>();
            //app.UseCustomClaimsValidationAsync(new List<IRouteDefinitions> { new ValuesRoutes() }).Wait();
            app.UseCustomClaimsValidationAsync(new CachedValidRouteDefinitionProvider(memoryCache), async (context) =>
            {
                var matchedRouteResult = await CustomClaimsValidationMiddleware.GetMatchingRoute(context).ConfigureAwait(false);
                if (matchedRouteResult.Key != null)
                {
                    return await CustomClaimsValidationMiddleware
                        .ValidateClaimsAsync(matchedRouteResult.Key, context, matchedRouteResult.Value)
                        .ConfigureAwait(false);
                }
                return true;
            }).Wait();
            app.UseMvc();
        }
    }
}
