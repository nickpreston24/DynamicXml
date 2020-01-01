using System;

namespace Parsely
{
    public static class FunctionalExtensions
    {
        /// <summary>
        /// Chain an action to an object T
        /// </summary>
        public static T With<T>(this T item, Action<T> action)
        {
            action(item);
            return item;
        }

        /// <summary>
        /// Allow disposable chained calls
        /// </summary>
        public static class Disposable
        {
            public static TResult Using<TDisposable, TResult>(
                Func<TDisposable> factory,
                Func<TDisposable, TResult> function) where TDisposable : IDisposable
            {
                using (var disposable = factory())
                    return function(disposable);
            }
        }

        public static TResult Map<TSource, TResult>(this TSource source, Func<TSource, TResult> map) => map(source);
    }
}