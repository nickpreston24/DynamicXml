using System;
using System.Collections.Generic;
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
