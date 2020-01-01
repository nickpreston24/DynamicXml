using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared
{
    public static class PrintHelpers
    {
        public static void Print<T>(this IEnumerable<T> collection) => Print(collection.ToList());

        public static Action<string> PrintAction { get; set; } = Console.WriteLine;

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

        private static void Print<T>(T item, Action<string> customPrint = null)
        {
            if (customPrint != null) PrintAction = customPrint;
            string text = item?.ToString();
            PrintAction(text);
        }
    }
}