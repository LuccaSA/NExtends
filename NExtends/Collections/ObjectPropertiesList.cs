using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace NExtends.Collections
{
	/// <summary>
	/// <para>
	/// Helper class for getting and setting class properties via an indexable list.
	/// If no class attributes are specified for class <typeparamref name="K"/>, all properties will be indexed in order of definition.
	/// </para>
	/// 
	/// <para>
	/// Use custom attributes <see cref="ObjectPropertiesListProperties"/> and <see cref="ObjectPropertiesListNumberedProperties"/>
	/// if you want to change order or only use selected properties. <seealso cref="IObjectPropertiesListParams"/>.
	/// </para>
	/// 
	/// <para>
	/// See example section for usage example.
	/// </para>
	/// </summary>
	/// 
	/// <typeparam name="K">Type of class to map properties</typeparam>
	/// <typeparam name="T">Type of properties</typeparam>
	/// 
	///<example>
	///<code>
	///
	///public class ObjectPropertiesListTest
	///{		
	///	public void ObjectFieldArrayTest1()
	///	{
	///		var t = new Test();
	///		var i = new ObjectPropertiesList&lt;Test, string&gt;(t);
	///
	///		i[0] = "test";
	///		i[1] = "test0";
	///		i[2] = "test1";
	///		i[3] = "test2";
	///
	///		Assert.IsTrue(t.test == "test");
	///		Assert.IsTrue(t.field0 == "test0");
	///		Assert.IsTrue(t.field1 == "test1");
	///		Assert.IsTrue(t.field2 == "test2");
	///	}
	///}
	///
	///[ObjectPropertiesListProperties(0, "test")]
	///[ObjectPropertiesListNumberedProperties(1, "field", 3, 0)]
	///class Test
	///{
	///		public string test { get; set; }
	///		public string field0 { get; set; }
	///		public string field1 { get; set; }
	///		public string field2 { get; set; }
	///}
	///</code>
	///</example>
	public class ObjectPropertiesList<K, T> : IList<T>
	{
		public static IEnumerable<string> IndexableProperties;
		public static Func<K, T>[] Getters;
		public static Action<K, T>[] Setters;
		public static string[] PropertyNames;

		K ObjectToIndex;

		static ObjectPropertiesList()
		{
			var getters = new List<Func<K, T>>();
			var setters = new List<Action<K, T>>();
			var propertyNames = new List<string>();

			var Attributes = typeof(K).GetTypeInfo().GetCustomAttributes(typeof(IObjectPropertiesListParams), true).OfType<IObjectPropertiesListParams>();

			if (Attributes.Count() == 0)
			{
				//in case we don't have any specified attributes we use all public properties
				IndexableProperties = typeof(K).GetProperties().Where(p => p.PropertyType == typeof(T)).Select(p => p.Name).ToList();
			}
			else
			{
				IndexableProperties = Attributes.OrderBy(a => a.Order).SelectMany(a => a.IndexableProperties).ToList();
			}

			foreach (var pName in IndexableProperties)
			{
				var pInfo = typeof(K).GetProperty(pName);

				if (pInfo == null)
					throw new Exception(string.Format("Class {0} doesn't have a property named {1}", typeof(K).Name, pName));

				var getter = pInfo.GetValueGetter<K, T>();
				var setter = pInfo.GetValueSetter<K, T>();

				getters.Add(getter);
				setters.Add(setter);
				propertyNames.Add(pName);
			}

			Getters = getters.ToArray();
			Setters = setters.ToArray();
			PropertyNames = propertyNames.ToArray();
		}

		public ObjectPropertiesList(K objectToIndex)
		{
			this.ObjectToIndex = objectToIndex;
		}

		public T this[int key]
		{
			get
			{
				return Getters[key](ObjectToIndex);
			}
			set
			{
				Setters[key](ObjectToIndex, value);
			}
		}

		public void Add(T item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(T item)
		{
			bool contains = false;
			for (int i = 0; i < Count && !contains; i++)
				contains = this[i].Equals(item);

			return contains;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (object.ReferenceEquals(array, null))
			{
				throw new ArgumentNullException(
					"Null array reference",
					"array"
					);
			}

			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException(
					"Index is out of range",
					"index"
					);
			}

			if (array.Rank > 1)
			{
				throw new ArgumentException(
					"Array is multi-dimensional",
					"array"
					);
			}

			foreach (T o in this)
			{
				array.SetValue(o, arrayIndex);
				arrayIndex++;
			}
		}

		public int Count
		{
			get { return IndexableProperties.Count(); }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new ObjectFieldArrayEnumerator<K, T>(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public static string PropertyNameOfIndex(int idx)
		{
			return PropertyNames[idx];
		}
	}

	public class ObjectFieldArrayEnumerator<K, T> : IEnumerator<T>
	{
		ObjectPropertiesList<K, T> Of;
		int CurIndex = -1;

		public ObjectFieldArrayEnumerator(ObjectPropertiesList<K, T> of)
		{
			this.Of = of;
		}
		public T Current
		{
			get { return Of[CurIndex]; }
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
			if (++CurIndex >= Of.Count)
				return false;

			return true;
		}

		public void Reset()
		{
			CurIndex = -1;
		}
	}

	/// <summary>
	/// Interface for attributes decorating objects which will be able to be indexed via <see cref="ObjectFieldArray&lt;K,T&gt;" />.
	/// </summary>
	public interface IObjectPropertiesListParams
	{
		int Order { get; }
		IEnumerable<string> IndexableProperties { get; }
	}


	/// <summary>
	/// Use this attribute to index numbered properties (e.g. field0, field1, field2, ...).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ObjectPropertiesListNumberedPropertiesAttribute : Attribute, IObjectPropertiesListParams
	{
		public IEnumerable<string> IndexableProperties { get; protected set; }
		public int Order { get; protected set; }

		/// <summary>
		/// Attribute to index numbered properties (e.g. field0, field1, field2, ...).
		/// </summary>
		/// <param name="order">In case you use multiple attributes, design the order of the properties in the resulting list</param>
		/// <param name="fieldPrefix">Prefix of properties to index (e.g. "field")</param>
		/// <param name="length">Number of properties with specified prefix</param>
		/// <param name="offset">Couting offset, (e.g. 1 if properties are named field1, field, etc.</param>
		public ObjectPropertiesListNumberedPropertiesAttribute(int order, string fieldPrefix, int length, int offset)
		{
			List<string> propertiesNames = new List<string>();

			for (int i = offset; i < length + offset; i++)
				propertiesNames.Add(string.Format("{0}{1}", fieldPrefix, i));

			this.IndexableProperties = propertiesNames;
			this.Order = order;
		}
	}

	/// <summary>
	/// Use this attribute to index selected properties.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ObjectPropertiesListPropertiesAttribute : Attribute, IObjectPropertiesListParams
	{
		public IEnumerable<string> IndexableProperties { get; protected set; }
		public int Order { get; protected set; }

		/// <summary>
		/// Attribute to index selected properties 
		/// </summary>
		/// <param name="order">In case you use multiple attributes, design the order of the properties in the resulting list</param>
		/// <param name="properties">Name of properties to index</param>
		public ObjectPropertiesListPropertiesAttribute(int order, params string[] properties)
		{
			this.IndexableProperties = properties;
			this.Order = order;
		}
	}

	/// <summary>
	/// Use expression trees to get property getter and setters
	/// http://weblogs.asp.net/marianor/archive/2009/04/10/using-expression-trees-to-get-property-getter-and-setters.aspx
	/// 
	/// There are times when you need get and set property values and you do not know the type of the properties.
	/// One option is use reflection through GetValue and SetValue from PropertyInfo class, but this wears to poor performance in our code.
	/// In order to do that we can use an Expression Tree to generate delegates that allow to get and set the value of the required property, 
	/// for example building a couple of extensions methods applying to PropertyInfo:
	/// 
	/// Modification done to the code online: The getter and setter delegates are strictly typed, by simply adding a second generic parameter T and replacing "object" by T.
	/// </summary>
	public static class PropertyInfoExtensions
	{
		public static Func<K, T> GetValueGetter<K, T>(this PropertyInfo propertyInfo)
		{
			if (typeof(K) != propertyInfo.DeclaringType)
			{
				throw new ArgumentException();
			}

			var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
			var property = Expression.Property(instance, propertyInfo);
			var convert = Expression.TypeAs(property, typeof(T));
			return (Func<K, T>)Expression.Lambda(convert, instance).Compile();
		}

		public static Action<K, T> GetValueSetter<K, T>(this PropertyInfo propertyInfo)
		{
			if (typeof(K) != propertyInfo.DeclaringType)
			{
				throw new ArgumentException();
			}

			var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
			var argument = Expression.Parameter(typeof(object), "a");
			var setterCall = Expression.Call(
				instance,
				propertyInfo.GetSetMethod(),
				Expression.Convert(argument, propertyInfo.PropertyType));
			return (Action<K, T>)Expression.Lambda(setterCall, instance, argument)
												.Compile();
		}
	}
}
