﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NExtends.Primitives
{
	public static class GenericExtensions
	{
		
        /// <summary>
        /// Build a Dictionary 
        /// </summary>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {

#if NETCOREAPP2_0
            return new Dictionary<TKey, TValue>(source);
#else
            return source.ToDictionary(item => item.Key, item => item.Value);
#endif
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
		/// Pour savoir si une liste contient tous les éléments d'une autre liste => int, string, ..
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static bool Contains<T>(this IEnumerable<T> first, IEnumerable<T> second)
		{
			return second.All(el => first.Contains(el));
		}

		public static bool ContainsIgnoreCase(this IEnumerable<string> collection, string target)
		{
			return collection.Contains(target, StringComparer.OrdinalIgnoreCase);
		}

		public static void AddOrUpdate<K, V>(this ConcurrentDictionary<K, V> dictionary, K key, V value)
		{
			dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
		}

		public static IEnumerable<TResult> Cast<TSource, TResult, ICommonInterface>(this IEnumerable<TSource> collection)
			where TSource : class, ICommonInterface
			where TResult : class, ICommonInterface, new()
		{
			var sourceProps = (from prop in typeof(TSource).GetProperties() select prop).ToList();
			var resultProps = (from prop in typeof(TResult).GetProperties() select prop).ToList();
			var properties = (from prop in typeof(ICommonInterface).GetProperties()
							 select new
							 {
								 source = sourceProps.Where(p => p.Name == prop.Name).FirstOrDefault(),
								 result = resultProps.Where(p => p.Name == prop.Name).FirstOrDefault()
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
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> collection)
		{
			return new HashSet<T>(collection);
		}

		public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
		{
			return source == null || !source.Any();
		}
    }
}