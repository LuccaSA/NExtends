using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NExtends.Primitives;
using NExtends.Tests.Models;

namespace NExtends.Tests
{
	[TestClass]
	public class EnumExtensionsTests
	{
		[TestMethod]
		public void ShouldSupportCheckOfAttributeOnEnum()
		{
			EnumWithAttribute value = EnumWithAttribute.IHaveNoAttribute;

			Assert.IsNull(value.GetAttributeOfType<OneEnumAttribute>());
			Assert.IsNull(value.GetAttributeOfType<AnotherEnumAttribute>());

			value = EnumWithAttribute.IHaveOneAttribute;

			Assert.IsNotNull(value.GetAttributeOfType<OneEnumAttribute>());
			Assert.IsNull(value.GetAttributeOfType<AnotherEnumAttribute>());
		}
	}
}
