using Newtonsoft.Json;
using System.Collections.Generic;

namespace HiperTrip.Extensions
{
    public static class DictionaryExtension
    {
        public static string ToJsonString(this Dictionary<string, object> dictionary)
        {
            return JsonConvert.SerializeObject(dictionary);
        }
    }
}