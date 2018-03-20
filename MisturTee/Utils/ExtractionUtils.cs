using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using MisturTee.Config.Claims;
using MisturTee.Config.Claims.ExtractionConfigs.Valid;
using MisturTee.Config.Routing;
using Newtonsoft.Json;

namespace MisturTee.Utils
{
    internal class ExtractionUtils
    {
        internal static async Task<List<Claim>> GetClaimsAsync(RouteDefinition routeDef, HttpRequest req, RouteValueDictionary routeValues)
        {
            var dict = new Dictionary<ClaimLocation, Dictionary<ExtractionType, string>>();
            var taskClaims = new List<Task<Claim>>();

            foreach (var extractionConfig in routeDef.ClaimsConfig?.ExtractionConfigs ?? new List<IValidClaimsExtractionConfig>())
            {
                if (!dict.ContainsKey(extractionConfig.ClaimLocation))
                {
                    dict.Add(extractionConfig.ClaimLocation, new Dictionary<ExtractionType, string>
                    {
                        { extractionConfig.ExtractionType, string.Empty }
                    });
                }
                else if (!dict[extractionConfig.ClaimLocation].ContainsKey(extractionConfig.ExtractionType))
                {
                    dict[extractionConfig.ClaimLocation].Add(extractionConfig.ExtractionType, string.Empty);
                }

                if (string.IsNullOrEmpty(dict[extractionConfig.ClaimLocation][extractionConfig.ExtractionType]))
                {
                    dict[extractionConfig.ClaimLocation][extractionConfig.ExtractionType] = GetContent(
                        extractionConfig.ClaimLocation, extractionConfig.ExtractionType, req, routeValues);
                }
                taskClaims.Add(extractionConfig.GetClaimAsync(dict[extractionConfig.ClaimLocation][extractionConfig.ExtractionType]));
            }

            return (await Task.WhenAll(taskClaims).ConfigureAwait(false)).ToList();
        }

        private static string GetContent(ClaimLocation location, ExtractionType extType, HttpRequest req, RouteValueDictionary routeValues)
        {
            switch (location)
            {
                case ClaimLocation.Body:
                    {
                        var bodyMemStream = new MemoryStream();
                        req.Body.CopyTo(bodyMemStream);
                        var stringBody = System.Text.Encoding.UTF8.GetString(bodyMemStream.ToArray());
                        return stringBody;
                    }
                case ClaimLocation.Headers:
                    {
                        return extType == ExtractionType.KeyValue ? JsonConvert.SerializeObject(HeaderDictToKvp(req.Headers, req.Headers)) : string.Empty;
                    }
                case ClaimLocation.Uri:
                    {
                        if (extType == ExtractionType.KeyValue)
                        {
                            return routeValues == null ? "" : JsonConvert.SerializeObject(RouteValueDictionaryToKvp(routeValues));
                        }
                        if (extType == ExtractionType.RegEx)
                        {
                            return req.Path;
                        }
                        break;
                    }
                case ClaimLocation.QueryParameters:
                    {
                        switch (extType)
                        {
                            case ExtractionType.KeyValue:
                                return JsonConvert.SerializeObject(QueryCollectionToKvp(req.Query));
                            case ExtractionType.RegEx:
                                return req.QueryString.ToString();
                        }
                        break;
                    }
                default:
                    return string.Empty;
            }
            return string.Empty;
        }

        private static List<KeyValuePair<string, List<object>>> QueryCollectionToKvp(IQueryCollection iqc)
        {
            return iqc?.Keys?.Select(key =>
                       new KeyValuePair<string, List<object>>(key,
                           iqc[key].ToArray()?.Select(x => (object) x).ToList()))?.ToList() ??
                   new List<KeyValuePair<string, List<object>>>();
        }

        private static List<KeyValuePair<string, List<object>>> HeaderDictToKvp(IEnumerable<KeyValuePair<string, StringValues>> arg, IHeaderDictionary dict)
        {
            return arg?.Select(x =>
                       new KeyValuePair<string, List<object>>(x.Key,
                           dict.GetCommaSeparatedValues(x.Key).Select(y => (object) y).ToList())).ToList() ??
                   new List<KeyValuePair<string, List<object>>>();
        }

        private static List<KeyValuePair<string, List<object>>> RouteValueDictionaryToKvp(IDictionary<string, object> arg)
        {
            return arg?.Select(x => new KeyValuePair<string, List<object>>(x.Key, new List<object> {x.Value}))
                       .ToList() ?? new List<KeyValuePair<string, List<object>>>();
        }
    }
}
