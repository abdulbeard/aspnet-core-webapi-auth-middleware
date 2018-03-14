using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace MisturTee.Utils
{
    public static class JsonUtils
    {
        public static IEnumerable<string> GetValueFromJson(string json, string jsonPath)
        {
            return JToken.Parse(json).SelectTokens(jsonPath).Select(x => x.ToString());
        }
    }
}
