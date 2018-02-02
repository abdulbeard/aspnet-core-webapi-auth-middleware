using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MisturTee.Config
{
    public class ConfigurationManager
    {
        public ConfigurationManager()
        {
            BuildAppsettings();
        }
        private IConfigurationRoot _configuration;

        public T Appsettings<T>(string appsettingsPath)
        {
            try
            {
                var value = _configuration[appsettingsPath];
                return value == null ? default(T) : (T) Convert.ChangeType(value, typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public T JsonAppsetting<T>(string appsettingsPath)
        {
            //var instance = Activator.CreateInstance<T>();
            //var section = _configuration.GetSection(appsettingsPath);
            //section.Bind(instance);
            //return instance;



            try
            {
                var value = _configuration[appsettingsPath];
                return string.IsNullOrEmpty(value) ? default(T) : JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public void BuildAppsettings()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            _configuration = builder.Build();
        }
    }
}