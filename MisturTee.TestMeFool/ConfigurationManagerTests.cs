using MisturTee.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MisturTee.TestMeFool
{
    public class ConfigurationManagerTests
    {
        [Fact]
        public void Appsettings()
        {
            var loglevelCount = new ConfigurationManager().Appsettings<int>("Logging:Console:LogLevel:Count");
            Assert.Equal(54, loglevelCount);
        }

        [Fact]
        public void Appsettings_ChangeTypeException()
        {
            var loglevelDefault = new ConfigurationManager().Appsettings<int>("Logging:Console:LogLevel:Default");
            Assert.Equal(default(int), loglevelDefault);
        }

        [Fact]
        public void Appsettings_JsonAppsetting()
        {
            var testingType = new TestingType() { Yo = "lo", No = "lo" };
            var testingTypeDeserialized = new ConfigurationManager().JsonAppsetting<TestingType>("testingType");
            Assert.Equal(testingType.Yo, testingTypeDeserialized.Yo);
            Assert.Equal(testingType.No, testingTypeDeserialized.No);
        }

        [Fact]
        public void Appsettings_JsonAppsetting_InvalidJson()
        {
            var testingType = new TestingType() { Yo = "lo", No = "lo" };
            var testingTypeDeserialized = new ConfigurationManager().JsonAppsetting<TestingType>("invalidJson");
            Assert.Null(testingTypeDeserialized);
        }
    }
}
