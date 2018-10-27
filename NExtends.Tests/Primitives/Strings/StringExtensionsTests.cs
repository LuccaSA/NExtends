using NExtends.Primitives.Strings;
using System.Globalization;
using System.Threading;
using Xunit;

namespace NExtends.Tests.Primitives.Strings
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("00000000", false)]
        [InlineData("00000000-0000-0000-0000-00000000000012345", false)]
        [InlineData("{{00000000-0000-0000-0000-000000000000}}", false)]
        [InlineData("00000000000000000000000000000000", true)]
        [InlineData("00000000-0000-0000-0000-000000000000", true)]
        [InlineData("{00000000-0000-0000-0000-000000000000}", true)]
        [InlineData("(00000000-0000-0000-0000-000000000000)", true)]
        [InlineData("{0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}", true)]
        public void IsGuidTest(string testData, bool isGuid)
        {
            Assert.Equal(isGuid, testData.IsGuid());
        }

        [Fact]
        public void TestToDouble()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Assert.Equal(1234.567, "1234.567".ToDouble());
            Assert.Equal(1234.567, "1234,567".ToDouble());
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            Assert.Equal(1234.567, "1234.567".ToDouble());
            Assert.Equal(1234.567, "1234,567".ToDouble());
        }

        [Fact]
        public void ToFileNameProducesAdequatesName()
        {
            string input = null;
            Assert.Null(input.ToFileName());
            Assert.Equal("", "".ToFileName());
            Assert.Equal("lOlé.3.4", "lOlé 3.4".ToFileName());
            Assert.Equal("lOlé.3.4", @"<l?O%l/é\ *3:.>4|".ToFileName());
        }

        [Fact]
        public void ChangeTypeShouldConvertDecimalValues()
        {
            var result = (decimal)StringExtensions.ChangeType("12.054", typeof(decimal), CultureInfo.InvariantCulture);
            Assert.Equal(12.054m, result);
        }

        [Fact]
        public void ChangeTypeShouldConvertDecimalScientificValues()
        {
            var result = (decimal)StringExtensions.ChangeType("3.6E-05", typeof(decimal), CultureInfo.InvariantCulture);
            Assert.Equal(0.000036m, result);
        }
    }
}
