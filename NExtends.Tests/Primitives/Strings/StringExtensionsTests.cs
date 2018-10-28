﻿using NExtends.Primitives.Strings;
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

        [Theory]
        [InlineData("/api/", "https://client.ilucca.net/api/v3/users")]
        [InlineData("/API/", "https://client.ilucca.net/api/v3/users")]
        [InlineData("/api/", "https://client.ilucca.net/API/v3/users")]
        public void ContainsIgnoreCaseShouldWork(string match, string chain)
        {
            var result = chain.ContainsIgnoreCase(match);

            Assert.True(result);
        }
    }
}
