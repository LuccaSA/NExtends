using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NExtends.Primitives;

namespace NExtends.Tests
{
	[TestClass]
	public class UriTests
	{
		[TestMethod]
		public void ShouldChangeTypeFromString()
		{
			var uri = "https://twitter.com".ChangeType(typeof(Uri), CultureInfo.InvariantCulture);

			Assert.AreEqual(typeof(Uri), uri.GetType());
		}
	}
}
