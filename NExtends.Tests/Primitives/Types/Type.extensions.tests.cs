using NExtends.Primitives.Types;
using NExtends.Tests.Models;
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

        [Fact]
        public void GetRealTypeName_Should_ReturnBaseType()
        {
            var type = typeof(ConcreteClass);
            var result = type.GetRealTypeName();

            Assert.Equal("ConcreteClass", result);
        }
	}
}
