using System.Linq;
using MisturTee.Utils;
using Xunit;

namespace MisturTee.TestMeFool.Utils
{
    public class JsonUtilsTests
    {
        /// <summary>
        /// Test built from: https://www.newtonsoft.com/json/help/html/QueryJsonSelectTokenJsonPath.htm
        /// </summary>
        [Fact]
        public void RegularTest()
        {
            var json =
                @"{'Stores': ['Lambton Quay','Willis Street'],'Manufacturers': [{'Name': 'Acme Co','Products': [{'Name': 'Anvil','Price': 50}]},{'Name': 'Contoso','Products': [{'Name': 'Elbow Grease','Price': 99.95},{'Name': 'Headlight Fluid','Price': 4}]}]}";
            var jsonPath = "$.Manufacturers[?(@.Name == 'Acme Co')]";
            var jsonToken = JsonUtils.GetValueFromJson(json, jsonPath);
            Assert.Equal(
                "{\r\n  \"Name\": \"Acme Co\",\r\n  \"Products\": [\r\n    {\r\n      \"Name\": \"Anvil\",\r\n      \"Price\": 50\r\n    }\r\n  ]\r\n}",
                jsonToken.First());
            var productsMoreThan50 = JsonUtils.GetValueFromJson(json, "$..Products[?(@.Price >= 50)].Name").ToList();
            Assert.Equal("Anvil", productsMoreThan50.First());
            Assert.Equal("Elbow Grease", productsMoreThan50.ElementAt(1));
        }
    }
}
