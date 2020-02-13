using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Parsely
{
    public static partial class DynamicExtensions
    {
        static PropertyCache _properties { get; } = new PropertyCache();

        public static bool HasNullProperties(this object instance)
        {
            var properties = instance.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(instance, null);

                if (value == null)
                    return true;
            }

            return false;
        }

        public static IEnumerable<PropertyInfo> GetNullProperties(this object @object)
        {
            return @object.GetType()
                .GetProperties()
                .Where(property => property.GetValue(@object, null) == null);
        }

        public static bool Implements(this Type type, Type contract)
        {
            var interfaces = type.GetInterfaces();
            return contract.IsGenericTypeDefinition
                ? interfaces.Any(interfaceType => interfaceType.GetGenericTypeDefinition() == contract)
                : interfaces.Any(interfaceType => interfaceType == contract);
        }

        public static bool IsIEnumerable(this Type type)
        {
            return type.Implements(typeof(IEnumerable<>));
        }
    }
}