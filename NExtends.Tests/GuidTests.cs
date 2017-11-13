using NExtends.Primitives.Strings;
using System;
using System.Globalization;
using Xunit;

namespace NExtends.Tests
{
    public class GuidTests
    {
		[Fact]
		public void ShouldChangeTypeFromString()
		{
			var guid = "3d450f16-f5ae-41ff-914e-142b4937ab68".ChangeType(typeof(Guid), CultureInfo.InvariantCulture);

			Assert.Equal(typeof(Guid), guid.GetType());
		}
    }
}
