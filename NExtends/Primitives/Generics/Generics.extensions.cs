using NExtends.Primitives.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NExtends.Primitives.Generics
{
    public static class GenericExtensions
    {
        /// <summary>
        /// Build a Dictionary 
        /// </summary>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return source.ToDictionary(item => item.Key, item => item.Value);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end
        /// </summary>
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> elements)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            foreach (T element in elements)
            {
                source.Add(element);
            }
        }

        /// <summary>
        /// Remove the elements of the specified collection
        /// </summary>
        public static void RemoveRange<T>(this ICollection<T> source, IEnumerable<T> elements)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            foreach (T element in elements)
            {
                source.Remove(element);
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end
        /// </summary>
        public static void AddMany<T>(this ICollection<T> list, params T[] objs)
        {
            list.AddRange(objs);
        }

        public enum SortOrder { Ascending, Descending };

        public static IOrderedEnumerable<T> OrderByEnum<T>(this IQueryable<T> query, Func<T, object> orderDelegate, SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Ascending)
            {
                return query.OrderBy(orderDelegate);
            }
            else
            {
                return query.OrderByDescending(orderDelegate);
            }
        }

        /// <summary>
        /// Permet d'envoyer l'élément ET son indice dans un ForEach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this List<T> list, Action<T, int> action)
        {
            for (int i = 0; i < list.Count; i++)
            {
                action(list[i], i);
            }
        }

        /// <summary>
        /// Permet de transformer une List de string du type "user.id,user.name" en Dictionary du type (user, (id, name)), ...
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> ToLowerDotDictionary(this List<string> list)
        {
            return list.Where(el => !String.IsNullOrEmpty(el)).Select(el => el.ToLower()).GroupBy(el => el.Split('.')[0]).ToDictionary(g => g.Key, g => g.All(s => s.Contains('.')) ? g.Select(s => String.Join(".", s.Split('.').Skip(1).ToArray())).ToList() : (List<string>)null, StringComparer.OrdinalIgnoreCase);
        }
        
        /// <summary>
        /// Swap specifi key
        /// </summary>
        public static void UpdateKey<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey oldKey, TKey newKey)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source.TryGetValue(newKey, out TValue value))
            {
                throw new ArgumentException("The new key is already present in the dictionary");
            }
            if (source.TryGetValue(oldKey, out value))
            {
                source.Remove(oldKey);
                source.Add(newKey, value);
            }
            else
            {
                throw new KeyNotFoundException("The key '" + oldKey + "' does not exist in the dictionary");
            }
        }

        public static Dictionary<TKey, TValue> Concat<TKey, TValue>(this Dictionary<TKey, TValue> first, Dictionary<TKey, TValue> second)
        {
            IEnumerable<KeyValuePair<TKey, TValue>> firstDictionary = first;
            IEnumerable<KeyValuePair<TKey, TValue>> secondDictionary = second;
            return firstDictionary.Concat(secondDictionary).ToDictionary(k => k.Key, k => k.Value);
        }

        /// <summary>
        /// Check if a IEnumerable contains all values present in the other, with the same number of occurences
        /// </summary>
        public static bool Contains<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            ICollection<T> actual = first as ICollection<T> ?? first?.ToList();
            ICollection<T> expected = second as ICollection<T> ?? second?.ToList();

            return actual.IsEquivalentTo(expected);
        }
          
        public static bool ContainsIgnoreCase(this IEnumerable<string> collection, string target)
        {
            return collection.Contains(target, StringComparer.OrdinalIgnoreCase);
        }

        public static void AddOrUpdate<K, V>(this ConcurrentDictionary<K, V> dictionary, K key, V value)
        {
            dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
        }

        public static IEnumerable<TResult> Cast<TSource, TResult>(this IEnumerable<TSource> collection, Type commonInterface)
            where TSource : class
            where TResult : class, new()
        {
            if (!typeof(TSource).IsSubclassOfInterface(commonInterface))
            {
                throw new ArgumentException($"TSource should implement {commonInterface}");
            }
            if (!typeof(TResult).IsSubclassOfInterface(commonInterface))
            {
                throw new ArgumentException($"TSource should implement {commonInterface}");
            }

            var sourceProps = (from prop in typeof(TSource).GetProperties() select prop).ToList();
            var resultProps = (from prop in typeof(TResult).GetProperties() select prop).ToList();
            var properties = (from prop in commonInterface.GetProperties()
                             select new
                             {
                                 source = sourceProps.FirstOrDefault(p => p.Name == prop.Name),
                                 result = resultProps.FirstOrDefault(p => p.Name == prop.Name)
                             })
                             .ToList();

            foreach (var sourceObject in collection)
            {
                var resultObject = new TResult();

                foreach (var property in properties)
                {
                    property.result.SetValue(resultObject, property.source.GetValue(sourceObject, null), null);
                }

                yield return resultObject;
            }
        }
        
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> collection) => new HashSet<TSource>(collection);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source) => source == null || !source.Any();

        public static bool IsEquivalentTo<T>(this ICollection<T> expected, ICollection<T> actual, IEqualityComparer<T> comparer = null)
        {
            if (expected == null != (actual == null))
            {
                return false;
            }
            if (expected == actual)
            {
                return true;
            }
            if (expected.Count != actual.Count)
            {
                return false;
            }
            if (expected.Count != 0 && FindMismatchedElement(expected, actual, comparer ?? EqualityComparer<T>.Default))
            {
                return false;
            }
            return true;
        }

        private static bool FindMismatchedElement<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T> comparer)
        {
            Dictionary<T, int> elementCounts = GetElementCounts(expected, out int num, comparer);
            Dictionary<T, int> dictionary2 = GetElementCounts(actual, out int num2, comparer);
            if (num2 != num)
            {
                return true;
            }
            foreach (T obj2 in elementCounts.Keys)
            {
                elementCounts.TryGetValue(obj2, out int expectedNullCount);
                dictionary2.TryGetValue(obj2, out int actualNullCount);
                if (expectedNullCount != actualNullCount)
                {
                    return true;
                }
            }
            return false;
        }

        private static Dictionary<T, int> GetElementCounts<T>(IEnumerable<T> collection, out int nullCount, IEqualityComparer<T> comparer)
        {
            var dictionary = new Dictionary<T, int>(comparer);
            nullCount = 0;
            foreach (T obj2 in collection)
            {
                if (object.Equals(obj2, default(T)))
                {
                    nullCount++;
                }
                else
                {
                    dictionary.TryGetValue(obj2, out int num);
                    num++;
                    dictionary[obj2] = num;
                }
            }
            return dictionary;
        }

        /// <summary>
        /// See https://stackoverflow.com/a/489421
        /// </summary>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Returns the only element of a sequence, or a default value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable`1 to return the single element of.</param>
        /// <returns>The single element of the input sequence, or default(TSource)</returns>
        public static TSource UniqueOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            var matches = source.Take(2).ToList();
            return matches.Count == 1 ? matches[0] : default(TSource);
        }
        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition or 
        /// a default value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable`1 to return a single element from.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <returns>The single element of the input sequence that satisfies the condition, or default(TSource)</returns>
        public static TSource UniqueOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => source.Where(predicate).UniqueOrDefault();
    }
}