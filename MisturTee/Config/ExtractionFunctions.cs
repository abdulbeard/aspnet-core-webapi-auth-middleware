using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MisturTee.Utils;

namespace MisturTee.Config
{
    public class ExtractionFunctions
    {
        public static Task<string> JsonPathFunc(string body, string path)
        {
            var extractedValues = new JsonUtils().GetValueFromJson(body, path);
            return Task.FromResult(extractedValues?.FirstOrDefault() ?? string.Empty);
        }

        public static Task<string> KeyValueFunc(List<KeyValuePair<string, List<object>>> listKvp, string keyName)
        {
            return Task.FromResult(listKvp.FirstOrDefault(x => x.Key == keyName).Value?.First().ToString() ?? string.Empty);
        }

        public static Task<string> RegexFunc(string data, Regex regex)
        {
            var match = regex.Match(data);
            if (match.Success && match.Groups.Count > 1 && match.Groups[1].Captures?.Count > 0)
            {
                return Task.FromResult(match.Groups[1].Captures[0].ToString());
            }
            return Task.FromResult(string.Empty);
        }
    }
}
