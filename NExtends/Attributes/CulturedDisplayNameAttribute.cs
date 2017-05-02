using System;
using System.Globalization;
using System.Resources;

namespace NExtends.Attributes
{
	public class CulturedDisplayNameAttribute : Attribute
	{
		string TermName { get; set; }
		ResourceManager ResxManager { get; set; }

		public string DisplayName
		{
			get { return ResxManager.GetString(TermName, CultureInfo.CurrentCulture); }
		}

		public CulturedDisplayNameAttribute(ResourceManager resxManager, string resxName)
		{
			ResxManager = resxManager;
			TermName = resxName;
		}
	}
}
