using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace Shared
{
    public static class Dumper
    {
        public static T Dump<T>(this T obj, string displayName = null, bool ignoreNulls = false, Action<string> print = null)
        {
            if (print == null) print = str => Debug.WriteLine(str);

            if (obj != null)
            {
                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = obj.GetType().Name;

                string prettyJson = JsonConvert.SerializeObject(
                    obj,
                    (Newtonsoft.Json.Formatting) Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter> { new Newtonsoft.Json.Converters.StringEnumConverter() },
                        NullValueHandling = (ignoreNulls) ? NullValueHandling.Ignore : NullValueHandling.Include,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                print($"{displayName}:\n{prettyJson}");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(displayName))
                    print($"'{displayName}' is null.");
            }

            return obj;
        }
    }
}