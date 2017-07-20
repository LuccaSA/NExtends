using NExtends.Primitives.Types;
using Xunit;

namespace NExtends.Tests.Primitives.Types
{
	public class TypeExtensionsTest
	{
		[Fact]
		public void IsSubclassOfInterfaceShouldHandleNonGenericInterfaces()
		{
			Assert.True(typeof(System.Collections.ArrayList).IsSubclassOfInterface(typeof(System.Collections.IEnumerable)));
		}
	}
}
