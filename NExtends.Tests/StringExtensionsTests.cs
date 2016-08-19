using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NExtends.Primitives;

namespace NExtends.Tests
{
	[TestClass]
	public class StringExtensionsTests
	{
		[TestMethod]
		public void ShouldRemoveDiacritics()
		{
			var text = "ÀÁÂÃÄÅàáâãäåÒÓÔÕÖòóôõöÈÉÊËèéêëÌÍÎÏìíîïÙÚÛÜùúûüÿÑñÇç";

			var result = text.RemoveDiacritics();

			var expected = "AAAAAAaaaaaaOOOOOoooooEEEEeeeeIIIIiiiiUUUUuuuuyNnCc";

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void ShouldRemoveSpecialCharacters()
		{
			var text = @"Jean d'Anois-Dumesnil _[]|./""@{}()";

			var result = text.RemoveSpecialCaracters();

			var expected = "JeandAnoisDumesnil";

			Assert.AreEqual(expected, result);
		}
	}
}
