using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Shared
{
    public static class Extensions
    {
        public static T Dump<T>(this T obj, string displayName = null, bool ignoreNulls = false)
        {
            if (obj != null)
            {
                if (string.IsNullOrWhiteSpace(displayName)) displayName = obj.GetType().Name;

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
                if (string.IsNullOrWhiteSpace(displayName))
                    return obj;

                Debug.WriteLine($"'{displayName}' is null.");
                return obj;
            }

            return obj;
        }

        public static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return Convert.ToString(((attributes.Length > 0) ? attributes[0].Description : value.ToString()));
        }
    }

    public static class PrintHelpers
    {
        public static void Print<T>(this IEnumerable<T> collection) => Print(collection.ToList());

        private static void Print<T>(this IReadOnlyCollection<T> list)
        {
            foreach (var obj in list)
                Print(obj);
        }

        public static void Print(params object[] collection)
        {
            foreach (var item in collection)
            {
                if (item is List<object> list)
                    Print(list);
                if (item is object[] array)
                    Print(array);

                Print(item);
            }
        }

        public static void Print<T>(params T[] collection) where T : class
        {
            foreach (T item in collection)
            {
                if (item is List<T> list)
                    Print(list);
                if (item is T[] array)
                    Print(array);

                Print(item);
            }
        }

        private static void Print<T>(this T item, bool debug = false)
        {
            string text = item?.ToString();
            Console.WriteLine(text);
            Debug.WriteLineIf(debug, text);
        }
    }
}