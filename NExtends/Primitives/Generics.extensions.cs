using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace NExtends.Primitives
{
	public static class GenericExtensions
	{
		/// <summary>
		/// Ajout d'une méthode pour supprimer le premier élément d'une List
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T Shift<T>(this List<T> list)
		{
			if (list.Count > 0)
			{
				var first = list[0];
				list.RemoveAt(0);

				return first;
			}
			else
			{
				return default(T);
			}
		}

		public static String ToJSON<T>(this IEnumerable<T> list)
		{
			return "[" + String.Join(",", list.Select(o => (string)o.GetType().GetMethod("ToJSON", new Type[] { }).Invoke(o, null)).ToArray()) + "]";
		}


		/// <summary>
		/// Pour reconstruire un Dictionnaire après avoir faire un .Where dessus
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="ienum"></param>
		/// <returns></returns>
		public static Dictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> ienum)
		{
			return ienum.ToDictionary(K => K.Key, K => K.Value);
		}

		/// <summary>
		/// Permet d'avoir les collections liées aux clés étrangères compatible entre le DBML et l'EDMX
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <param name="elements"></param>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> elements)
		{
			foreach (var element in elements)
			{
				collection.Add(element);
			}
		}

		/// <summary>
		/// Pour supprimer plusieurs éléments d'un coup
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <param name="elements"></param>
		public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> elements)
		{
			foreach (var element in elements)
			{
				collection.Remove(element);
			}
		}

		//Pour pouvoir comparer rapidement 2 objets de la même Classe
		public class GenericEqualityComparer<T> : IEqualityComparer<T>
		{
			private Func<T, T, Boolean> _comparer;
			public GenericEqualityComparer(Func<T, T, Boolean> comparer)
			{
				_comparer = comparer;
			}

			public bool Equals(T x, T y)
			{
				return _comparer(x, y);
			}

			public int GetHashCode(T obj)
			{
				return obj.ToString().ToLower().GetHashCode();
			}
		}

		/// <summary>
		/// Indicates whether the specified IEnumerable is null or empty.
		/// </summary>
		/// <param name="iEnumerable"></param>
		/// <returns></returns>
		public static bool isNullOrEmpty(this IEnumerable<object> IEnumerable)
		{
			if (IEnumerable != null)
			{
				return !IEnumerable.Any();
			}
			return true;
		}

		//Pour pouvoir écrire dans une NameValueCollection, il faut la convertir en Dictionnary
		public static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
		{
			Dictionary<string, string> dico = new Dictionary<string, string>();

			foreach (string key in collection.AllKeys)
			{
				dico.Add(key, collection[key]);
			}

			return dico;
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
		/// Pour pouvoir ajouter plusieurs éléments d'un coup à une List (ex : a, b, c ...)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="objs"></param>
		public static void AddMany<T>(this ICollection<T> list, params T[] objs)
		{
			list.AddRange(objs);
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
		/// Evite de devoir mettre des .ToString() partout
		/// </summary>
		/// <param name="dic"></param>
		/// <param name="enumKey"></param>
		/// <returns></returns>
		public static bool ContainsKey<T>(this Dictionary<string, T> dic, Enum enumKey)
		{
			return dic.ContainsKey(enumKey.ToString());
		}

		/// <summary>
		/// Permet d'ajouter une clé de type Enum sans devoir mettre un .ToString()
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dic"></param>
		/// <param name="enumValue"></param>
		/// <returns></returns>
		public static void Add<T>(this Dictionary<string, T> dic, Enum enumKey, T value)
		{
			dic.Add(enumKey.ToString(), value);
		}

		/// <summary>
		/// Permet de renommer une clé
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dict"></param>
		/// <param name="oldKey"></param>
		/// <param name="newKey"></param>
		public static void UpdateKey<T>(this Dictionary<string, T> dict, string oldKey, string newKey)
		{
			T value;
			if(dict.TryGetValue(newKey, out value))
			{
				throw new Exception("The new key is already present in the dictionary");
			}
			if(dict.TryGetValue(oldKey, out value))
			{
				dict.Remove(oldKey);
				dict.Add(newKey, value);
			}
			else
			{
				throw new Exception("The key '" + oldKey + "' does not exist in the dictionary");
			}
		}

		public static Dictionary<T, U> Concat<T, U>(this Dictionary<T, U> first, Dictionary<T, U> second)
		{
			return first.ToList().Concat(second.ToList()).ToDictionary(k => k.Key, k => k.Value);
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
	}
}