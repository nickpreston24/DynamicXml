using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Shared
{
    public static class Extensions
    {
        public static T Dump<T>(this T obj, string displayName = null, bool ignoreNulls = false)
        {
            if (obj == null)
            {
                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    Debug.WriteLine(string.Format("Object '{0}'{1}", displayName, " is null."));
                }
            }
            else if (obj != null)
            {
                if (string.IsNullOrWhiteSpace(displayName))
                {
                    displayName = obj.GetType().Name;
                }

                string prettyJson = JsonConvert.SerializeObject(
                    obj,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter> { new Newtonsoft.Json.Converters.StringEnumConverter() },
                        NullValueHandling = (ignoreNulls) ? NullValueHandling.Ignore : NullValueHandling.Include,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                Debug.WriteLine(string.Format("{0}:\n{1}", displayName, prettyJson));
            }

            return obj;
        }
    }
}
