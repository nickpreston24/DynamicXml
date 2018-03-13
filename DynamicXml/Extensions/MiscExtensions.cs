using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace DynamicXml
{
    public static partial class DynamicExtensions
    {
        private static PropertyCache _properties { get; } = new PropertyCache();
        public static bool HasNullProperties(this object @object)
        {
            var properties = @object.GetType().GetProperties();

            foreach (var property in properties)
            {
                object value = property.GetValue(@object, null);

                if (value == null)
                {
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<PropertyInfo> GetNullProperties(this object @object)
        {
            return @object.GetType().GetProperties().Where(property => property.GetValue(@object, null) == null);
        }

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


        //public static bool IsIEnumerableOfT(this Type type)
        //{
        //    return type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        //    //return type.GetInterfaces().Any(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        //}
        public static bool Implements(this Type type, Type contract)
        {
            return contract.IsGenericTypeDefinition
            ? type.GetInterfaces().Any(i => i.GetGenericTypeDefinition().Equals(contract))
            : type.GetInterfaces().Any(i => i.Equals(contract));
        }

        public static bool IsIEnumerableOfT(this Type type)
        {
            return type.Implements(typeof(IEnumerable<>));
        }

    }
}
