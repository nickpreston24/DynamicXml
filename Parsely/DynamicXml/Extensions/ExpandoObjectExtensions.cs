using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace DynamicXml.Extensions
{
    public static class ExpandoObjectExtensions
    {
        public static void Map(this ExpandoObject source, object destination)
        {
            IDictionary<string, object> dictionary = source ?? throw new ArgumentNullException(nameof(source));
            var type = destination?.GetType() ?? throw new ArgumentNullException(nameof(destination));
            string normalizeName(string name) => name.ToLowerInvariant();
            
            var setters = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanWrite && property.GetSetMethod() != null)
                .ToDictionary(property => normalizeName(property.Name));

            foreach (var keyValuePair in dictionary)
            {
                if (!setters.TryGetValue(normalizeName(keyValuePair.Key), out var setter)) 
                    continue;
                var value = setter.PropertyType.ChangeType(keyValuePair.Value);
                setter.SetValue(destination, value);
            }
        }
    }
}