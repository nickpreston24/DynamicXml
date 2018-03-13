using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

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
            Debug.WriteLine(type.ToString());

            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(type);
            var instance = Activator.CreateInstance(constructedListType);
            Debug.WriteLine(listType.ToString());
            Debug.WriteLine(constructedListType.ToString());
            return (IList)instance;
        }
    }
}
