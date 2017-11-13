using NExtends.Primitives.Strings;
using System;
using System.Globalization;
using Xunit;

namespace NExtends.Tests
{
	public class UriTests
	{
		[Fact]
		public void ShouldChangeTypeFromString()
		{
			var uri = "https://twitter.com".ChangeType(typeof(Uri), CultureInfo.InvariantCulture);

			Assert.Equal(typeof(Uri), uri.GetType());
		}
	}
}
