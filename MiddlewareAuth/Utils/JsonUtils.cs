using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace MisturTee.Utils
{
    public class JsonUtils
    {
        //public string GetValueFromJson(string json, string jsonPath, bool jsonIsArray = false)
        //{
        //    var jobject = jsonIsArray ? JArray.Parse(json) : (JContainer)JObject.Parse(json);
        //    return jobject.SelectToken(jsonPath).ToString();
        //}

        public IEnumerable<string> GetValueFromJson(string json, string jsonPath)
        {
            return JToken.Parse(json).SelectTokens(jsonPath).Select(x => x.ToString());
        }
    }
}
