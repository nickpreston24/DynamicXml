using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DynamicXml.Extensions
{
    public static partial class DynamicExtensions
    {
        public static bool IsNullable(this Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            return type.IsGenericType 
                   && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNullAssignable(this Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            return type.IsNullable() || !type.IsValueType;
        }

        public static object ChangeType(this Type type, object instance)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            
            if (!type.IsNullAssignable())
                throw new InvalidCastException($"{type.FullName} is not null-assignable");
            
            if (instance == null)
                return null;

            if (!type.IsNullable()) 
                return Convert.ChangeType(instance, type);
            
            type = Nullable.GetUnderlyingType(type);
            return Convert.ChangeType(instance, type);
        }
        
        public static T ConvertTo<T>(this object value) where T : class
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static object ConvertTo(this object value, Type type)
        {
            return Convert.ChangeType(value, type);
        }

        public static IEnumerable ToEnumerableType(this Type type)
        {
            if (type == null || !type.IsClass)
                return default;

            var enumerableType = typeof(IEnumerable<>);
            var constructedListType = enumerableType.MakeGenericType(type);
            
            var instance = Activator.CreateInstance(constructedListType);
            return (IList)instance;
        }

        public static IList ToListType(this Type type)
        {
            if (type == null || !type.IsClass)
                return default;

            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(type);
            var instance = Activator.CreateInstance(constructedListType);
            return (IList)instance;
        }

        /// <summary>
        /// Creates a Default Expression that has the Type property set to the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetDefault<T>()
        {
            var expression = Expression.Lambda<Func<T>>(Expression.Default(typeof(T)));
            return expression.Compile()();
        }

        public static object GetDefault(this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            var expression = Expression.Lambda<Func<object>>(Expression.Convert(Expression.Default(type), type));
            return expression.Compile()();
        }
    }
}