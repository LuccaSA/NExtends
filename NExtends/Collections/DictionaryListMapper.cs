using System;
using System.Collections.Generic;
using System.Linq;

namespace NExtends.Collections
{
	/// <summary>
	/// Helper class mapping entries of an IList&lt;<typeparamref name="V"/>&gt; to an IDictionary with configurable key values
	/// </summary>
	/// <example>
	/// <code>
	///		var test = new string[] { "entry1", "entry2" };
	///
	///		var entries = new DictionaryListMapEntry[] {
	///			new  DictionaryListMapEntry(0, "lala"),
	///			new  DictionaryListMapEntry(1, "lulu")
	///		};
	///		var m = new DictionaryListMapper&lt;string&gt;(test, entries);
	///
	///		Assert.IsTrue(m["lala"] == "entry1");
	///		Assert.IsTrue(m["lulu"] == "entry2");
	/// </code>
	/// </example>
	/// <typeparam name="V"></typeparam>
	public class DictionaryListMapper<V> : IDictionary<string, V>
	{
		IList<V> CollectionToMap;
		IEnumerable<IDictionaryListMapEntry> MapEntries;
		ICollection<string> MapEntriesKeys;
		ICollection<int> MapEntriesIndexes;

		Dictionary<string, IDictionaryListMapEntry> Map;

		public DictionaryListMapper(IList<V> collectionToMap, IEnumerable<IDictionaryListMapEntry> mapEntries)
			: this(collectionToMap)
		{
			SetMapEntries(mapEntries);
		}

		protected DictionaryListMapper(IList<V> collectionToMap)
		{
			this.CollectionToMap = collectionToMap;
		}

		protected void SetMapEntries(IEnumerable<IDictionaryListMapEntry> mapEntries)
		{
			this.MapEntries = mapEntries;
			this.MapEntriesKeys = this.MapEntries.Select(me => me.Key).ToList();
			this.MapEntriesIndexes = this.MapEntries.Select(me => me.Index).ToList();

			Map = new Dictionary<string, IDictionaryListMapEntry>();
			foreach (var entry in mapEntries)
				Map.Add(entry.Key, entry);
		}

		public void Add(string key, V value)
		{
			throw new NotImplementedException();
		}

		public bool ContainsKey(string key)
		{
			return MapEntriesKeys.Contains(key);
		}

		public ICollection<string> Keys
		{
			get { return MapEntriesKeys; }
		}

		public bool Remove(string key)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(string key, out V value)
		{
			bool succes = false;

			value = default(V);
			try
			{
				int idx = Map[key].Index;
				value = CollectionToMap[idx];

				succes = true;
			}
			catch
			{

			}
			return succes;
		}

		public ICollection<V> Values
		{
			get
			{
				List<V> values = new List<V>();
				foreach (var idx in MapEntriesIndexes)
					values.Add(CollectionToMap[idx]);
				return values;
			}
		}

		public V this[string key]
		{
			get
			{
				int idx = Map[key].Index;
				return CollectionToMap[idx];
			}
			set
			{
				int idx = Map[key].Index;
				CollectionToMap[idx] = value;
			}
		}

		public void Add(KeyValuePair<string, V> item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<string, V> item)
		{
			if (!ContainsKey(item.Key))
				return false;

			return this[item.Key].Equals(item.Value);
		}

		public void CopyTo(KeyValuePair<string, V>[] array, int arrayIndex)
		{
			foreach (var mapEntry in MapEntries)
			{
				var kvp = new KeyValuePair<string, V>(mapEntry.Key, this[mapEntry.Key]);
				array.SetValue(kvp, arrayIndex++);
			}
		}

		public int Count
		{
			get { return MapEntries.Count(); }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<string, V> item)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<string, V>> GetEnumerator()
		{
			return new ListDictionaryMapperEnumerator(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public class ListDictionaryMapperEnumerator : IEnumerator<KeyValuePair<string, V>>
		{
			DictionaryListMapper<V> Mapper;
			IEnumerator<IDictionaryListMapEntry> It;

			public ListDictionaryMapperEnumerator(DictionaryListMapper<V> mapper)
			{
				this.Mapper = mapper;
				Reset();
			}

			public KeyValuePair<string, V> Current
			{
				get
				{
					var mapEntry = It.Current;
					return new KeyValuePair<string, V>(mapEntry.Key, Mapper[mapEntry.Key]);
				}
			}

			public void Dispose()
			{
			}

			object System.Collections.IEnumerator.Current
			{
				get { return Current; }
			}

			public bool MoveNext()
			{
				return It.MoveNext();
			}

			public void Reset()
			{
				It = Mapper.MapEntries.GetEnumerator();
			}
		}
	}

	/// <summary>
	/// Parameter for <see cref="DictionaryListMapper(IList&lt;V&gt; collectionToMap, IEnumerable&lt;IDictionaryListMapEntry&gt; mapEntries)"/> 
	/// Map Index of a list to Key 
	/// </summary>
	public interface IDictionaryListMapEntry
	{
		int Index { get; set; }
		string Key { get; set; }
	}

	public class DictionaryListMapEntry : IDictionaryListMapEntry
	{
		public DictionaryListMapEntry(int idx, string key)
		{
			this.Index = idx;
			this.Key = key;
		}

		public int Index { get; set; }
		public string Key { get; set; }
	}
}
