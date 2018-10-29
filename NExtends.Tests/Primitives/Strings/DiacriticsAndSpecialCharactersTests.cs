using NExtends.Primitives.Strings;
using Xunit;

namespace NExtends.Tests.Primitives.Strings
{
    public class DiacriticsAndSpecialCharactersTests
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

        [Fact]
        public void ShouldSanitizeSpecialCharacters()
        {
            var text = "ØøæÆœŒ";

            var result = text.SanitizeSpecialCaracters();

            var expected = "OoaeAEoeOE";

            Assert.Equal(expected, result);
        }
    }
}
