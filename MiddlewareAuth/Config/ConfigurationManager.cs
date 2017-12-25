using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MiddlewareAuth.Config
{
    public class ConfigurationManager
    {
        public ConfigurationManager()
        {
            BuildAppsettings();
        }
        private IConfigurationRoot Configuration;
        public T Appsettings<T>(string appsettingsPath)
        {
            var sdfsdf = Configuration[appsettingsPath];
            return (T)Convert.ChangeType(sdfsdf, typeof(T));
        }

        public void BuildAppsettings()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }
    }
}