using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace DynamicXml
{
    public static partial class DynamicExtensions
    {
        public static ConcurrentDictionary<Type, PropertyInfo[]> Cache(this Type type)
        {
            PropertyCache._propertyCache.TryAdd(type, type.GetProperties());
            return PropertyCache._propertyCache;
        }
    }
}
