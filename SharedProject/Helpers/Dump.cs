using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Utilities.Shared.Extensions
{
    public static class ObjectExtensions
    {
        public static T Dump<T>(this T obj, string displayName = null, bool ignoreNulls = false)
        {
            if (obj != null)
            {
                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = obj.GetType().Name;

                string prettyJson = JsonConvert.SerializeObject(
                    obj,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter> { new Newtonsoft.Json.Converters.StringEnumConverter() },
                        NullValueHandling = (ignoreNulls) ? NullValueHandling.Ignore : NullValueHandling.Include,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                Debug.WriteLine($"{displayName}:\n{prettyJson}");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    Debug.WriteLine($"'{displayName}' is null.");
                    return obj;
                }

                return obj;
            }

            return obj;
        }
    }
}