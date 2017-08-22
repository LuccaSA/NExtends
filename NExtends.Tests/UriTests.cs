using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NExtends.Primitives;
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
