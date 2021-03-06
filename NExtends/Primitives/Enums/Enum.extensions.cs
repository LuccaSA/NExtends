﻿using NExtends.Primitives.Strings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace NExtends.Primitives.Enums
{
	public static class EnumExtensions
	{
		/// <summary>
		/// Throws an exception if <typeparamref name="T"/> is NOT an Enum
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static void CheckIsEnum<T>() where T : struct { CheckIsEnum(typeof(T)); }

		static void CheckIsEnum(Type t)
		{
			if (!t.GetTypeInfo().IsEnum)
				throw new ArgumentException(string.Format("Type '{0}' is not Enum", t));
		}

		/// <summary>
		/// Convert an TEnum to a List&lt;TEnum&gt; so we can easily iterate on it
		/// No "enum" constraint, so have to settle for struct, and throw exception if not enum
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<T> EnumToList<T>() where T : Enum
		{
			var list = new List<T>();
			var type = typeof(T);
			CheckIsEnum(type);

			var values = Enum.GetValues(type);

			foreach (Object value in values)
			{
				var name = Enum.GetName(type, value);
				list.Add((T)Enum.Parse(type, name));
			}

			return list;
		}

		/// <summary>
		/// <para>Convert an enum to a Dictionnary with Key = enum string value, Value = enum int value</para>
		/// <para>(Useful for JavaScript)</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Dictionary<string, int> EnumToDictionary<T>() where T : Enum
        {
			return EnumToList<T>().ToDictionary(e => e.ToString(), e => Convert.ToInt32(e));
		}

		/// <summary>
		/// Simplifie l'appel à Enum.Parse
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static T Parse<T>(this string value) where T : Enum
        {
			return (T)Enum.Parse(typeof(T), value, true);
		}

		public static bool TryParse<T>(this string value, out T result) where T : struct, Enum
        {
			return Enum.TryParse(value, true, out result);
		}

		/// <summary>
		/// Gets an attribute on an enum field value
		/// </summary>
		/// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
		/// <param name="enumVal">The enum value</param>
		/// <returns>The attribute of type T that exists on the enum value</returns>
		public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
			var type = enumVal.GetType();
			var memInfo = type.GetMember(enumVal.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);

			if (attributes.Any())
			{
				return (T)attributes.ElementAt(0);
			}

			return null;
		}

		/// <summary>
		/// Retourne le DisplayAttribute de l'énumération
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static DisplayAttribute GetDisplayAttribute(this Enum value)
		{
			var type = value.GetType();
			CheckIsEnum(type);

			var members = type.GetMember(value.ToString());
			if (members.Length == 0) throw new ArgumentException(String.Format("Member '{0}' not found in type '{1}'", value, type.Name));

			var member = members[0];
			var attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);
			if (!attributes.Any())
				return null;

            return (DisplayAttribute)attributes[0];
		}

		/// <summary>
		/// Retourne le nom localisé défini par l'attribut '[DisplayAttribute]' sur les valeurs de l'énumération
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetDisplayName(this Enum value)
		{
			var attribute = GetDisplayAttribute(value);
			return attribute == null ? "" : attribute.GetName();
		}

		/// <summary>
		/// Retourne la description localisée définie par l'attribut '[DisplayAttribute]' sur les valeurs de l'énumération
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetDisplayDescription(this Enum value)
		{
			var attribute = GetDisplayAttribute(value);
			return attribute == null ? "" : attribute.GetDescription();
		}

		/// <summary>
		/// Parse string to enum, with default value and optional case insensivity
		/// http://stackoverflow.com/questions/79126/create-generic-method-constraining-t-to-an-enum
		/// Didn't want to go the hardcore way by going through IL - even though it was pretty tempting :)
		/// </summary>
		public static TEnum ParseEnum<TEnum>(this string value) 
            where TEnum : struct, Enum, IComparable, IFormattable, IConvertible
        {
            return ParseEnum<TEnum>(value, true, default(TEnum));
        }

		public static TEnum ParseEnum<TEnum>(this string value, bool ignoreCase, TEnum defaultValue) 
            where TEnum : struct, Enum, IComparable, IFormattable, IConvertible
		{
			CheckIsEnum<TEnum>();
			if (string.IsNullOrEmpty(value)) { return defaultValue; }

			TEnum lResult;
			return Enum.TryParse(value, ignoreCase, out lResult) ? lResult : defaultValue;
		}

		//Courtesy of stackoverflow : http://stackoverflow.com/a/4367868/3535983
		public static T GetValueFromDescription<T>(string description)
		{
			var type = typeof(T);
			if (!type.GetTypeInfo().IsEnum) throw new InvalidOperationException();
			foreach (var field in type.GetFields())
			{
				var attribute = field.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
				if (attribute != null)
				{
					if (attribute.Description == description)
						return (T)field.GetValue(null);
				}
				else
				{
					if (field.Name == description)
						return (T)field.GetValue(null);
				}
			}
			throw new ArgumentException("Not found.", "description");
		}

		public static string GetDescriptionFromValue<T>(T value)
		{
			var info = typeof(T).GetTypeInfo();
			var memInfo = info.GetMember(value.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
			return ((DescriptionAttribute)attributes.ElementAt(0)).Description;
		}
    }
}