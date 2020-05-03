using Helpers.General;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Helpers.Extensions
{
    public static class DictionaryExtensions
    {
        public static string ToJsonString(this Dictionary<string, object> dictionary)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new LowercaseContractResolver()
            };

            return JsonConvert.SerializeObject(dictionary, Formatting.Indented, settings);
        }
    }
}