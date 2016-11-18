using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NExtends.Tests.Models
{
	public enum EnumWithAttribute
	{
		IHaveNoAttribute,
		[OneEnumAttribute]
		IHaveOneAttribute,
		[AnotherEnumAttribute]
		IHaveAnotherAttribute
	}

	public class OneEnumAttribute : Attribute { }
	public class AnotherEnumAttribute : Attribute { }
}
