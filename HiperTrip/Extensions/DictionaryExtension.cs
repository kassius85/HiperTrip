using HiperTrip.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HiperTrip.Extensions
{
    public static class DictionaryExtension
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