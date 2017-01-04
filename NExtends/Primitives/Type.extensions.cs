using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NExtends.Attributes;
using System.Linq.Expressions;

namespace NExtends.Primitives
{
	public static class TypeExtensions
	{
		//http://stackoverflow.com/questions/36032555/compare-propertyinfo-name-to-an-existing-property-in-a-safe-way
		public static bool IsEqual<T>(this PropertyInfo prop, Expression<Func<T, object>> propertyExpression)
		{
			var mbody = propertyExpression.Body as MemberExpression;

			if (mbody == null)
			{
				//This will handle Nullable<T> properties.
				var ubody = propertyExpression.Body as UnaryExpression;

				if (ubody != null)
				{
					mbody = ubody.Operand as MemberExpression;
				}

				if (mbody == null)
				{
					throw new ArgumentException("Expression is not a MemberExpression", "propertyExpression");
				}
			}
			return mbody.Member.Name == prop.Name;
		}

		/// <summary>
		/// Renvoie le nom c# d'un type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetOriginalName(this Type type)
		{
			var typeName = type.FullName.Replace(type.Namespace + ".", "");

			//CSharp ou VisualBasic
			var niceName = typeName == "Void" ? "void" : CodeDomProvider.CreateProvider("CSharp").GetTypeOutput(new CodeTypeReference(typeName));

			if (type.GetGenericArguments().Length == 0)
			{
				return niceName;
			}
			else
			{
				niceName = niceName.Split("<")[0];
				return niceName + "<" + string.Join(",", type.GetGenericArguments().Select(GetOriginalName)) + ">";
			}
		}

		public static T GetAttribute<T>(this MemberInfo member) where T : Attribute
		{
			return Attribute.IsDefined(member, typeof(T)) ? (T)member.GetCustomAttributes(typeof(T), false)[0] : null;
		}

		public static bool IsEnumerableOrArray(this Type T)
		{
			return (typeof(IEnumerable).IsAssignableFrom(T) && T != typeof(string)) || T.IsArray;
		}

		public static Type GetEnumerableOrArrayElementType(this Type T)
		{
			if (!IsEnumerableOrArray(T))
			{
				return T;
			}
			// On est sûr que c'est un type Enumerable<> ou un Array
			// Donc si c'est pas un Array, on retourne le type generique
			if (T.IsArray)
			{
				return T.GetElementType();
			}
			else
			{
				//gère aussi IEnumerable<T>
				return (T.GetInterface("IEnumerable`1") ?? T).GetGenericArguments().FirstOrDefault();
			}
		}

		/// <summary>
		/// Given Fun&lt;T&gt; we return T
		/// </summary>
		/// <param name="T"></param>
		/// <returns></returns>
		public static Type GetNeastedFuncType(this Type T)
		{
			if (!T.IsGenericType || T.GetGenericTypeDefinition() != typeof(Func<>))
			{
				return T;
			}

			return T.GetGenericArguments().FirstOrDefault();
		}

		/// <summary>
		/// Pour savoir si un Type dérive d'un Type générique, genre APIResource&lt;T&gt; par exemple
		/// http://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
		/// </summary>
		/// <param name="generic"></param>
		/// <param name="toCheck"></param>
		/// <returns></returns>
		public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
		{
			while (toCheck != null && toCheck != typeof(object))
			{
				var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
				if (generic == cur)
				{
					return true;
				}
				toCheck = toCheck.BaseType;
			}
			return false;
		}

		/// <summary>
		/// Pour savoir si une classe ou un de ses parents implémente une interface
		/// </summary>
		/// <param name="toCheck"></param>
		/// <param name="generic"></param>
		/// <returns></returns>
		public static bool IsSubclassOfInterface(this Type toCheck, Type interfaceType)
		{
			var interfaces = toCheck.GetInterfaces();

			foreach (var interfaceOfCheck in interfaces)
			{
				if (interfaceOfCheck.IsGenericType)
				{
					if (interfaceOfCheck.GetGenericTypeDefinition() == interfaceType)
					{
						return true;
					}
				}
				else if (interfaceOfCheck == interfaceType)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Si c'est une entité de l'EDMX, le type va être Utilisateur_DFDKKDFHVBJTHERU : Utilisateur => Utilisateur
		/// Et parfois sera tronqué !! PublicHolidaysCalenda_SDOIDFOIDF : PublicHolidaysCalendar => PublicHolidaysCalendar !!
		/// Ou alors des types hérité, comme FiggoApplication : Application => Application
		/// Si c'est une APICollection, le type va être APICollection`1 => APICollection
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetRealTypeName(this Type type)
		{
			if (!Attribute.IsDefined(type, typeof(JsonObjectAttribute), false))
			{
				return (type.BaseType ?? type).Name; //Au cas où BaseType est null, on prend le type lui même (cas du type Object par ex)
			}
			else
			{
				return type.Name.Split('`')[0];
			}
		}

		/// <summary>
		/// [ <c>public static Type GetNullableType(Type TypeToConvert)</c> ]
		/// <para></para>
		/// Convert any Type to its Nullable&lt;T&gt; form, if possible
		/// </summary>
		/// <param name="TypeToConvert">The Type to convert</param>
		/// <returns>
		/// The Nullable&lt;T&gt; converted from the original type, the original type if it was already nullable, or null 
		/// if either <paramref name="TypeToConvert"/> could not be converted or if it was null.
		/// </returns>
		/// <remarks>
		/// To qualify to be converted to a nullable form, <paramref name="TypeToConvert"/> must contain a non-nullable value 
		/// type other than System.Void.  Otherwise, this method will return a null.
		/// </remarks>
		/// <seealso cref="Nullable&lt;T&gt;"/>
		public static Type GetNullableType(this Type TypeToConvert)
		{
			// Abort if no type supplied
			if (TypeToConvert == null)
				return null;

			// If the given type is already nullable, just return it
			if (TypeToConvert.IsTypeNullable())
				return TypeToConvert;

			// If the type is a ValueType and is not System.Void, convert it to a Nullable<Type>
			if (TypeToConvert.IsValueType && TypeToConvert != typeof(void))
				return typeof(Nullable<>).MakeGenericType(TypeToConvert);

			// Done - no conversion
			return null;
		}

		public static Type GetNonNullableType(this Type TypeToConvert)
		{
			// Abort if no type supplied
			if (TypeToConvert == null)
				return null;

			// If the given type is non nullable, just return it
			if (!TypeToConvert.IsNullableValueType())
			{
				return TypeToConvert;
			}
			else
			{
				return TypeToConvert.GetGenericArguments()[0];
			}
		}

		/// <summary>
		/// [ <c>public static bool IsTypeNullable(Type TypeToTest)</c> ]
		/// <para></para>
		/// Reports whether a given Type is nullable (Nullable&lt; Type &gt;)
		/// </summary>
		/// <param name="TypeToTest">The Type to test</param>
		/// <returns>
		/// true = The given Type is a Nullable&lt; Type &gt; or can be assigned null (reference type); false = The type is not nullable, or <paramref name="TypeToTest"/> 
		/// is null.
		/// </returns>
		/// <remarks>
		/// This method tests <paramref name="TypeToTest"/> and reports whether it is nullable (i.e. whether it is either a 
		/// reference type or a form of the generic Nullable&lt; T &gt; type).
		/// Do not confuse with IsNullableValueType !
		/// </remarks>
		/// <seealso cref="GetNullableType"/>
		public static bool IsTypeNullable(this Type TypeToTest)
		{
			// Abort if no type supplied
			if (TypeToTest == null)
				return false;

			// If this is not a value type, it is a reference type, so it is automatically nullable
			//  (NOTE: All forms of Nullable<T> are value types)
			if (!TypeToTest.IsValueType)
				return true;

			// Report whether TypeToTest is a form of the Nullable<> type
			return TypeToTest.IsGenericType && TypeToTest.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		/// <summary>
		/// [ <c>public static bool IsNullableValueType(Type TypeToTest)</c> ]
		/// <para></para>
		/// Reports whether a given Type is of type Nullable&lt; Type &gt; (or equivalent shorthand form such as int?, string?, ...)
		/// </summary>
		/// <param name="TypeToTest">The Type to test</param>
		/// <returns>
		/// true = The given Type is a Nullable&lt; Type &gt;; false = The type is not of the form Nullable&lt; Type &gt;;, or <paramref name="TypeToTest"/> 
		/// is null.
		/// </returns>
		/// <remarks>
		/// Do not confuse with IsTypeNullable !
		/// 
		/// </remarks>
		/// <seealso cref="GetNullableType"/>
		public static bool IsNullableValueType(this Type TypeToTest)
		{
			// Abort if no type supplied
			if (TypeToTest == null)
				return false;

			// Report whether TypeToTest is a form of the Nullable<> type
			return TypeToTest.IsGenericType && TypeToTest.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		/// <summary>
		/// Permet d'aller chercher les propriétés hérités pour les Interfaces => IUser : IEntityBase
		/// http://stackoverflow.com/questions/358835/getproperties-to-return-all-properties-for-an-interface-inheritance-hierarchy
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static PropertyInfo[] GetPublicProperties(this Type type)
		{
			if (type.IsInterface)
			{
				var propertyInfos = new List<PropertyInfo>();

				var considered = new List<Type>();
				var queue = new Queue<Type>();
				considered.Add(type);
				queue.Enqueue(type);
				while (queue.Count > 0)
				{
					var subType = queue.Dequeue();
					foreach (var subInterface in subType.GetInterfaces())
					{
						if (considered.Contains(subInterface)) continue;

						considered.Add(subInterface);
						queue.Enqueue(subInterface);
					}

					var typeProperties = subType.GetProperties(
						BindingFlags.FlattenHierarchy
						| BindingFlags.Public
						| BindingFlags.Instance);

					var newPropertyInfos = typeProperties
						.Where(x => !propertyInfos.Contains(x));

					propertyInfos.InsertRange(0, newPropertyInfos);
				}

				return propertyInfos.ToArray();
			}

			return type.GetProperties(BindingFlags.FlattenHierarchy
				| BindingFlags.Public | BindingFlags.Instance);
		}

		/// <summary>
		/// type.IsValueType || type == typeof(String)
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsValueType(this Type type)
		{
			return type.IsValueType || type == typeof(String);
		}

		/// <summary>
		/// Renvoie le nom le plus friendly possible (displayName, description, puis nom machine) du type donné
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetFriendlyName(this Type type)
		{
			var name = type.GetRealTypeName();
			var displayNameAttr = type.GetAttribute<CulturedDisplayNameAttribute>();
			if (displayNameAttr != null)
			{
				name = displayNameAttr.DisplayName;
			}
			else
			{
				var descriptionAttr = type.GetAttribute<DescriptionAttribute>();
				if (descriptionAttr != null)
				{
					name = descriptionAttr.Description;
				}
			}

			return name;
		}

		public static PropertyInfo[] GetFlattenProperties(this Type type, BindingFlags flags)
		{
			if (type.IsInterface)
			{
				var propertyInfos = new List<PropertyInfo>();

				var considered = new HashSet<Type> { type };
				var stack = new Stack<Type>(new[] { type });

				while (stack.Any())
				{
					var subType = stack.Pop();
					foreach (var subInterface in subType.GetInterfaces())
					{
						if (considered.Add(subInterface))
						{
							stack.Push(subInterface);
						}
					}

					propertyInfos.AddRange(subType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | flags).Where(x => !propertyInfos.Contains(x)));
				}

				return propertyInfos.ToArray();
			}
			else
			{
				return type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | flags);
			}
		}

		public static PropertyInfo[] GetPublicPropertiesIncludingExplicitInterfaceImplementations(this Type type)
		{
			var classProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var interfacesProperties = type.GetInterfaces().SelectMany(i => i.GetProperties());

			return classProperties.Union(interfacesProperties).Distinct(new GenericEqualityComparer<PropertyInfo>((p1, p2) => p1.Name == p2.Name)).ToArray();
		}

		public static PropertyInfo[] GetPublicOrInternalPropertiesIncludingExplicitInterfaceImplementations(this Type type)
		{
			var classProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var interfacesProperties = type.GetInterfaces().SelectMany(i => i.GetProperties());

			return classProperties.Union(interfacesProperties).Distinct(new GenericEqualityComparer<PropertyInfo>((p1, p2) => p1.Name == p2.Name)).ToArray();
		}

		public static object Convert<T>(object o)
		{
			if (typeof(T) == typeof(int))
			{
				return Int32.Parse(o.ToString());
			}
			else if (typeof(T) == typeof(long))
			{
				return long.Parse(o.ToString());
			}
			else if (typeof(T) == typeof(string))
			{
				return o.ToString();
			}

			throw new NotImplementedException();
		}

		public static TResult Cast<TSource, TResult, ICommonInterface>(this TSource sourceObject)
			where TSource : class, ICommonInterface
			where TResult : class, ICommonInterface, new()
		{
			var resultObject = new TResult();

			var sourceProps = (from prop in typeof(TSource).GetProperties() select prop).ToList();
			var resultProps = (from prop in typeof(TResult).GetProperties() select prop).ToList();
			var properties = (from prop in typeof(ICommonInterface).GetProperties()
							  select new
							  {
								  source = sourceProps.Where(p => p.Name == prop.Name).FirstOrDefault(),
								  result = resultProps.Where(p => p.Name == prop.Name).FirstOrDefault()
							  })
							 .ToList();

			foreach (var property in properties)
			{
				property.result.SetValue(resultObject, property.source.GetValue(sourceObject, null), null);
			}

			return resultObject;
		}
	}
}
