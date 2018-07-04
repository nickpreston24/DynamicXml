using System;

namespace DynamicXml
{
    public static class FunctionalExtensions
    {
        /// <summary>
        /// Chain an action to an object T
        /// </summary>
        public static T Tee<T>(this T @this, Action<T> action)
        {
            action(@this);
            return @this;
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

        public static TResult Map<TSource, TResult>(this TSource @this, Func<TSource, TResult> function)
        {
            return function(@this);
        }
    }
}
