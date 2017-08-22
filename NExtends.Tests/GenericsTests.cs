using System.Collections.Generic;
using System.Linq;
using NExtends.Tests.Models;
using NExtends.Primitives;
using NExtends.Primitives.Types;
using Xunit;

namespace NExtends.Tests
{
	public class GenericsTests
	{
		[Fact]
		public void CastingCollectionOfClassToAnotherThroughtInterfaceShouldWork()
		{
			var t = new GenericTestsClassT() { Id = 1, Name = "T", CustomT = "CustomT", UnexpectedCommonName = "Common" };

			var collection = new HashSet<GenericTestsClassT>() { t };

			var result = collection.Cast<GenericTestsClassT, GenericTestsClassU, GenericTestsInterfaceI>().SingleOrDefault();

			Assert.NotNull(result);
			Assert.Equal(1, result.Id);
			Assert.Equal("T", result.Name);
			Assert.Null(result.CustomU);
			Assert.Null(result.UnexpectedCommonName);
		}

		[Fact]
		public void CastingObjectOfClassToAnotherThroughtInterfaceShouldWork()
		{
			var t = new GenericTestsClassT() { Id = 1, Name = "T", CustomT = "CustomT", UnexpectedCommonName = "Common" };

			var result = t.Cast<GenericTestsClassT, GenericTestsClassU, GenericTestsInterfaceI>();

			Assert.NotNull(result);
			Assert.Equal(1, result.Id);
			Assert.Equal("T", result.Name);
			Assert.Null(result.CustomU);
			Assert.Null(result.UnexpectedCommonName);
		}
	}
}
