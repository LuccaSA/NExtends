using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NExtends.Primitives;
using System.Globalization;

namespace NExtends.Tests
{
	[TestClass]
    public class GuidTests
    {
		[TestMethod]
		public void ShouldChangeTypeFromString()
		{
			var guid = "3d450f16-f5ae-41ff-914e-142b4937ab68".ChangeType(typeof(Guid), CultureInfo.InvariantCulture);

			Assert.AreEqual(typeof(Guid), guid.GetType());
		}
    }
}
