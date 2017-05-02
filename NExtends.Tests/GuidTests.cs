using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NExtends.Primitives;
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
