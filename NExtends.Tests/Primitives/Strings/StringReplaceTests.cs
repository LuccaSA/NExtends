using System;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace NExtends.Tests.Primitives.Strings
{
    public class StringReplaceTests
    {
        [Fact]
        public void CaseSensitiveStringReplace()
        {
            var source = "The cat is in the kitchen";
            var expected = "@ cat is in the kitchen";

            var result = NExtends.Primitives.StringExtensions.Replace(source, "The", "@", StringComparison.InvariantCulture);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CaseInsensitiveStringReplace()
        {
            var source = "The cat is in the kitchen";
            var expected = "@ cat is in @ kitchen";

            var result = NExtends.Primitives.StringExtensions.Replace(source, "THE", "@", StringComparison.InvariantCultureIgnoreCase);

            Assert.Equal(expected, result);
        }

        private long StringReplaceWithStopWatch(string source, string oldValue, string newValue, StringComparison stringComparison)
        {
            var sw = new Stopwatch();
            sw.Start();

            var extensionResult = NExtends.Primitives.StringExtensions.Replace(source, oldValue, newValue, stringComparison);

            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        [Fact]
        public void StringReplaceShouldBeFastWhenNoMatchButLongChain()
        {
            string stringToReplace = "foo";
            string source = String.Join(" bar ", Enumerable.Range(0, 10000).Select(i => stringToReplace));

            var result = StringReplaceWithStopWatch(source, "whatever", String.Empty, StringComparison.InvariantCultureIgnoreCase);

            Assert.True(result < 200);
        }

        [Fact]
        public void StringReplaceShouldBeFastWhenCaseSensitiveReplacementBut10000CaseInsentitiveMatches()
        {
            string stringToReplace = "foo";
            string source = String.Join(" bar ", Enumerable.Range(0, 10000).Select(i => stringToReplace));

            var result = StringReplaceWithStopWatch(source, "FOO", String.Empty, StringComparison.InvariantCulture);

            Assert.True(result < 200);
        }

        [Fact]
        public void StringReplaceShouldBeFastWhen10000Matches()
        {
            string stringToReplace = "foo";
            string source = String.Join(" bar ", Enumerable.Range(0, 10000).Select(i => stringToReplace));

            var result = StringReplaceWithStopWatch(source, "foo", String.Empty, StringComparison.InvariantCultureIgnoreCase);

            Assert.True(result < 200);
        }

        [Fact]
        public void StringReplaceExtensionShouldWorkWithXML()
        {
            var source = "<toto>My'Chain@domain.fr - and , \\e {value}</toto>";
            var expected = "<toto>My'Chain@domain.fr - and , \\e valueModified</toto>";

            var result = NExtends.Primitives.StringExtensions.Replace(source, "{VALUE}", "valueModified", StringComparison.InvariantCultureIgnoreCase);

            Assert.Equal(expected, result);
        }
    }
}
