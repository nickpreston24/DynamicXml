using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Shared.Helpers
{
    public static class PrintHelpers
    {
        public static void Print<T>(this IEnumerable<T> collection) => Print(collection.ToList());

        private static void Print<T>(List<T> list)
        {
            foreach (var obj in list)
                Print(obj);
        }

        public static void Print(params object[] collection)
        {
            foreach (var item in collection)
            {
                if (item is List<object> list)
                    Print(list);
                if (item is object[] array)
                    Print(array);

                Print(item);
            }
        }

        public static void Print<T>(params T[] collection) where T : class
        {
            foreach (T item in collection)
            {
                if (item is List<T> list)
                    Print(list);
                if (item is T[] array)
                    Print(array);

                Print(item);
            }
        }

        private static void Print<T>(T item, bool debug = false)
        {
            string text = item?.ToString();
            Console.WriteLine(text);
            Debug.WriteLineIf(debug, text);
        }
    }
}