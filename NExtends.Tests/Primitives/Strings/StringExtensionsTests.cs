using NExtends.Primitives.Strings;
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
    }
}
