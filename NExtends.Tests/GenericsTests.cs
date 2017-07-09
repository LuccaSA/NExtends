using Microsoft.VisualStudio.TestTools.UnitTesting;
using NExtends.Primitives;
using NExtends.Primitives.Types;
using NExtends.Tests.Models;
using System.Collections.Generic;
using System.Linq;

namespace NExtends.Tests
{
	[TestClass]
	public class GenericsTests
	{
		[TestMethod]
		public void CastingCollectionOfClassToAnotherThroughtInterfaceShouldWork()
		{
			var t = new GenericTestsClassT() { Id = 1, Name = "T", CustomT = "CustomT", UnexpectedCommonName = "Common" };

			var collection = new HashSet<GenericTestsClassT>() { t };

			var result = collection.Cast<GenericTestsClassT, GenericTestsClassU, GenericTestsInterfaceI>().SingleOrDefault();

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Id);
			Assert.AreEqual("T", result.Name);
			Assert.IsNull(result.CustomU);
			Assert.IsNull(result.UnexpectedCommonName);
		}

		[TestMethod]
		public void CastingObjectOfClassToAnotherThroughtInterfaceShouldWork()
		{
			var t = new GenericTestsClassT() { Id = 1, Name = "T", CustomT = "CustomT", UnexpectedCommonName = "Common" };

			var result = t.Cast<GenericTestsClassT, GenericTestsClassU, GenericTestsInterfaceI>();

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Id);
			Assert.AreEqual("T", result.Name);
			Assert.IsNull(result.CustomU);
			Assert.IsNull(result.UnexpectedCommonName);
		}
	}
}
