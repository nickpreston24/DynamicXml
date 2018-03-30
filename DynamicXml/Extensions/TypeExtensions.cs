using System;
using System.Collections;
using System.Collections.Generic;

namespace DynamicXml.Extensions
{
    public static partial class DynamicExtensions
    {
        public static T ConvertTo<T>(this object value) where T : class
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static object ConvertTo(this object value, Type type)
        {
            return Convert.ChangeType(value, type);
        }

        public static object ToClassInstance(this Type type)
        {
            if (type == null && type.IsClass)
            {
                return default(object);
            }

            var constructedType = type.MakeGenericType(type);
            return Activator.CreateInstance(constructedType);
        }

        public static IList ToClassList(this Type type)
        {
            if (type == null && type.IsClass)
            {
                return default(IList);
            }

            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(type);
            object instance = Activator.CreateInstance(constructedListType);
            return (IList)instance;
        }
    }
}
