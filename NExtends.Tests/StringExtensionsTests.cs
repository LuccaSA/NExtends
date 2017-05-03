using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NExtends.Primitives;
using Xunit;

namespace NExtends.Tests
{
	public class StringExtensionsTests
	{
		[Fact]
		public void ShouldRemoveDiacritics()
		{
			var text = "ÀÁÂÃÄÅàáâãäåÒÓÔÕÖòóôõöÈÉÊËèéêëÌÍÎÏìíîïÙÚÛÜùúûüÿÑñÇç";

			var result = text.RemoveDiacritics();

			var expected = "AAAAAAaaaaaaOOOOOoooooEEEEeeeeIIIIiiiiUUUUuuuuyNnCc";

			Assert.Equal(expected, result);
		}

		[Fact]
		public void ShouldRemoveSpecialCharacters()
		{
			var text = @"Jean d'Anois-Dumesnil _[]|./""@{}()";

			var result = text.RemoveSpecialCaracters();

			var expected = "JeandAnoisDumesnil";

			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData("toto")]
		[InlineData("toto@l")]
		[InlineData("toto@lucca")]
		public void ShouldReturnFalseWhenTextIsNotEmail(string value)
		{
			Assert.False(value.isEmail());
		}

		[Theory]
		[InlineData("toto@lucca.fr")]
		public void ShouldReturnTrueWhenTextIsEmail(string value)
		{
			Assert.True(value.isEmail());
		}
	}
}
