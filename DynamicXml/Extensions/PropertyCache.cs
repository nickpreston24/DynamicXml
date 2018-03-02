using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace DynamicXml
{
    public class PropertyCache
    {
        public static ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
        public PropertyInfo[] this[Type type] => _propertyCache.TryGetValue(type, out PropertyInfo[] properties) ? properties : type.Cache()[type];
        public int Count => _propertyCache.Count;
    }
}
