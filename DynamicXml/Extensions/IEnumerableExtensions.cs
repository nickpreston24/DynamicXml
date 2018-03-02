using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DynamicXml
{
    public static partial class DynamicExtensions
    {
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> sequence, int startIndex, int count)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            // optimization for anything implementing IList<T>
            return !(sequence is IList<T> list)
                 ? sequence.Skip(startIndex).Take(count)
                 : _(count); IEnumerable<T> _(int countdown)
            {
                int listCount = list.Count;
                int index = startIndex;
                while (index < listCount && countdown-- > 0)
                {
                    yield return list[index++];
                }
            }
        }

        public static IEnumerable<TSource> TakeLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source is ICollection<TSource> col
                ? col.Slice(Math.Max(0, col.Count - count), int.MaxValue)
                : _(); IEnumerable<TSource> _()
            {
                if (count <= 0)
                {
                    yield break;
                }

                var q = new Queue<TSource>(count);

                foreach (var item in source)
                {
                    if (q.Count == count)
                    {
                        q.Dequeue();
                    }

                    q.Enqueue(item);
                }

                foreach (var item in q)
                {
                    yield return item;
                }
            }
        }

        //From: https://github.com/morelinq/MoreLINQ/blob/master/MoreLinq/MaxBy.cs#L43
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
           Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, null);
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
           Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            comparer = comparer ?? Comparer<TKey>.Default;
            return ExtremumBy(source, selector, (x, y) => comparer.Compare(x, y));
        }

        static TSource ExtremumBy<TSource, TKey>(IEnumerable<TSource> source,
            Func<TSource, TKey> selector, Func<TKey, TKey, int> comparer)
        {
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }

                var extremum = sourceIterator.Current;
                var key = selector(extremum);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer(candidateProjected, key) > 0)
                    {
                        extremum = candidate;
                        key = candidateProjected;
                    }
                }

                return extremum;
            }
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
           Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, null);
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
           Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            comparer = comparer ?? Comparer<TKey>.Default;
            return ExtremumBy(source, selector, (x, y) => -Math.Sign(comparer.Compare(x, y)));
        }

        public static DataTable ToDatatable<T>(this IEnumerable<T> collection)
        {
            var table = new DataTable();

            if (collection.IsNullOrEmpty())
            {
                return table;
            }

            var properties = typeof(T).GetProperties();

            if (properties.Length == 0)
            {
                return table;
            }

            var values = new object[properties.Length];

            try
            {
                foreach (var item in collection ?? Enumerable.Empty<T>())
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = properties[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }
            }
            catch (Exception)
            {
            }

            return table;
        }

        public static IEnumerable<T> AsEmpty<T>(this IEnumerable<T> source) => Enumerable.Empty<T>();

        public static IEnumerable<T> Interweave<T>(params T[] inputs) => Interweave<T>(inputs);

        public static IEnumerable<T> Interweave<T>(this IEnumerable<IEnumerable<T>> inputs)
        {
            var enumerators = new List<IEnumerator<T>>();
            try
            {
                foreach (var input in inputs)
                {
                    enumerators.Add(input.GetEnumerator());
                }

                while (true)
                {
                    enumerators.RemoveAll(enumerator =>
                    {
                        if (enumerator.MoveNext())
                        {
                            return false;
                        }

                        enumerator.Dispose();
                        return true;
                    });

                    if (enumerators.Count == 0)
                    {
                        yield break;
                    }

                    foreach (var enumerator in enumerators)
                    {
                        yield return enumerator.Current;
                    }
                }
            }
            finally
            {
                if (enumerators != null)
                {
                    foreach (var e in enumerators)
                    {
                        e.Dispose();
                    }
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> query, int batchSize)
        {
            using (var enumerator = query.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.EnumerateSome(batchSize);
                }
            }
        }

        internal static IEnumerable<T> EnumerateSome<T>(this IEnumerator<T> enumerator, int count)
        {
            var list = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(enumerator.Current);
                if (!enumerator.MoveNext())
                {
                    break;
                }
            }

            foreach (var item in list)
            {
                yield return item;
            }
        }

        //public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int maxBatchSize)
        //{
        //    return items.Select((item, index) => new { item, index })
        //        .GroupBy(pairs => pairs.index / maxBatchSize)
        //        .Select(mapped => mapped.Select(pair => pair.item));
        //}

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection == null || !collection.Any();

    }
}
