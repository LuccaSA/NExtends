using System;
using System.Linq;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace NExtends.Attributes
{
	public class CulturedDescriptionAttribute : Attribute
	{
		string TermName { get; set; }
		ResourceManager ResxManager { get; set; }

		public string Description => ResxManager.GetString(TermName, CultureInfo.CurrentCulture);

		public CulturedDescriptionAttribute(ResourceManager resxManager, string resxName)
		{
			ResxManager = resxManager;
			TermName = resxName;
		}

		/// <summary>
		/// Gets an attribute on an enum field value
		/// </summary>
		/// <param name="enumValue">The enum value</param>
		/// <returns>The enum Description if it exists, else an empty string</returns>
		public static string GetName(Enum enumValue)
		{
			var type = enumValue.GetType();
			var memInfo = type.GetMember(enumValue.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(CulturedDescriptionAttribute), false);
			return (attributes.Count() > 0) ? ((CulturedDescriptionAttribute)attributes.ElementAt(0)).Description : String.Empty;
		}
	}
}
