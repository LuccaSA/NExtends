using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NExtends.Attributes
{
	public class CulturedDisplayNameAttribute : Attribute
	{
		string TermName { get; set; }
		ResourceManager ResxManager { get; set; }

		public string DisplayName
		{
			get { return ResxManager.GetString(TermName, Thread.CurrentThread.CurrentCulture); }
		}

		public CulturedDisplayNameAttribute(ResourceManager resxManager, string resxName)
		{
			ResxManager = resxManager;
			TermName = resxName;
		}
	}
}
