using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TokenAuth.Config
{
    internal class ConfigurationManager
    {

        private static IConfigurationRoot Configuration;
        public static T Appsettings<T>(string appsettingsPath)
        {
            var sdfsdf = Configuration[appsettingsPath];
            return (T)Convert.ChangeType(sdfsdf, typeof(T));
        }

        public static void BuildAppsettings()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }
    }
}