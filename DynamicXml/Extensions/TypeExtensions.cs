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

        public static IList ToTypedList(this Type type)
        {
            if (type == null)
            {
                return default(IList);
            }

            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(type);
            var instance = Activator.CreateInstance(constructedListType);
            return (IList)instance;
        }
    }
}
