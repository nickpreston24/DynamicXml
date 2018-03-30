using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DynamicXml.Extensions
{
    public static partial class DynamicExtensions
    {
        private static ConcurrentDictionary<Type, object> typeDefaults = new ConcurrentDictionary<Type, object>();

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
            {
                return default(IEnumerable);
            }

            var enumerableType = typeof(IEnumerable<>);
            var constructedListType = enumerableType.MakeGenericType(type);
            object instance = Activator.CreateInstance(constructedListType);
            return (IList)instance;
        }

        public static IList ToListType(this Type type)
        {
            if (type == null || !type.IsClass)
            {
                return default(IList);
            }

            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(type);
            object instance = Activator.CreateInstance(constructedListType);
            return (IList)instance;
        }

        public static T GetDefault<T>()
        {
            // We want an Func<T> which returns the default.
            // Create that expression here.
            var e = Expression.Lambda<Func<T>>(
                // The default value, always get what the *code* tells us.
                Expression.Default(typeof(T))
            );

            // Compile and return the value.
            return e.Compile()();
        }

        //todo: still not working, returns null for a class type!
        public static object GetDefault(this Type type)
        {
            // Validate parameters.
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            // We want an Func<object> which returns the default.
            // Create that expression here.
            var e = Expression.Lambda<Func<object>>(
                // Have to convert to object.
                Expression.Convert(
                    // The default value, always get what the *code* tells us.
                    Expression.Default(type), typeof(object)
                )
            );

            // Compile and return the value.
            return e.Compile()();
        }

        #region Mine
        //todo: got error:  "MakeGenericType may only be called on a type for which Type.IsGenericTypeDefinition is true."
        //public static object GetDefault(this Type type)
        //{
        //    return type.IsValueType
        //      ? typeDefaults.GetOrAdd(type, Activator.CreateInstance)
        //      : null;

        //    //return (type.IsValueType) ? Activator.CreateInstance(type) : null;

        //    //if (type == null || !type.IsClass)
        //    //{
        //    //    return default(object);
        //    //}

        //    //var constructedType = type.MakeGenericType(type);
        //    //return Activator.CreateInstance(constructedType);
        //}

        #endregion Mine

        #region SO's
        //https://stackoverflow.com/questions/325426/programmatic-equivalent-of-defaulttype
        //public static object GetDefault(this Type type)
        //{
        //    Func<object> func = GetDefaultGeneric<object>;
        //    return func.Method.GetGenericMethodDefinition().MakeGenericMethod(type).Invoke(null, null);
        //}

        //public static object GetDefault(this Type type)
        //{
        //    return type.GetType().GetMethod(nameof(GetDefaultGeneric)).MakeGenericMethod(type).Invoke(null, null);
        //}

        //public static T GetDefaultGeneric<T>()
        //{
        //    return default(T);
        //}
        #endregion SO's

    }
}
