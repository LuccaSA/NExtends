using Microsoft.VisualStudio.TestTools.UnitTesting;
using NExtends.Primitives.Types;

namespace NExtends.Tests.Primitives.Types
{
	[TestClass]
	public class TypeExtensionsTest
	{
		[TestMethod]
		public void IsSubclassOfInterfaceShouldHandleNonGenericInterfaces()
		{
			Assert.IsTrue(typeof(System.Collections.ArrayList).IsSubclassOfInterface(typeof(System.Collections.IEnumerable)));
		}
	}
}
