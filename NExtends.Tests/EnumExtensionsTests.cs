using NExtends.Primitives.Enums;
using NExtends.Tests.Models;
using Xunit;

namespace NExtends.Tests
{
	public class EnumExtensionsTests
	{
		[Fact]
		public void ShouldSupportCheckOfAttributeOnEnum()
		{
			EnumWithAttribute value = EnumWithAttribute.IHaveNoAttribute;

			Assert.Null(value.GetAttributeOfType<OneEnumAttribute>());
			Assert.Null(value.GetAttributeOfType<AnotherEnumAttribute>());

			value = EnumWithAttribute.IHaveOneAttribute;

			Assert.NotNull(value.GetAttributeOfType<OneEnumAttribute>());
			Assert.Null(value.GetAttributeOfType<AnotherEnumAttribute>());
		}
	}
}
