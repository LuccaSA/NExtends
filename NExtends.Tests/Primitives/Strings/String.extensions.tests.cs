using System;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace NExtends.Tests.Primitives.Strings
{
    public class StringExtensionsTest
    {
        [Fact]
        public void StringReplaceExtensionShouldBeRelativelyFastComparerToNativeImplementationAndCaseSensitive()
        {
            string stringToReplace = "foo";
            string source = String.Join(" bar ", Enumerable.Range(0, 10000).Select(i => stringToReplace));

            var sw = new Stopwatch();

            //0 occurrence - Native
            sw.Start();
            var nativeResult = source.Replace("whatever", "");
            var nativeElapsed = sw.ElapsedMilliseconds;

            //0 occurrence - extension
            sw.Restart();
            var extensionResult = NExtends.Primitives.StringExtensions.Replace(source, "whatever", "", StringComparison.InvariantCultureIgnoreCase);
            var extensionElapsed = sw.ElapsedMilliseconds;

            Assert.True(extensionElapsed < 10 || extensionElapsed < nativeElapsed * 10);

            //10000 occurrences - Native
            sw.Restart();
            nativeResult = source.Replace(stringToReplace, "");
            nativeElapsed = sw.ElapsedMilliseconds;

            //10000 occurrences - extension
            sw.Restart();
            extensionResult = NExtends.Primitives.StringExtensions.Replace(source, stringToReplace, "", StringComparison.InvariantCultureIgnoreCase);
            extensionElapsed = sw.ElapsedMilliseconds;

            Assert.True(extensionElapsed < 10 || extensionElapsed < nativeElapsed * 10);

            //0 occurrences due to case sentitiviy - Native
            sw.Restart();
            nativeResult = source.Replace("FOO", "");
            nativeElapsed = sw.ElapsedMilliseconds;

            Assert.Equal(source, nativeResult);

            //10000 occurrences thanks to case sensitivity - extension
            sw.Restart();
            extensionResult = NExtends.Primitives.StringExtensions.Replace(source, "FOO", "", StringComparison.InvariantCultureIgnoreCase);
            extensionElapsed = sw.ElapsedMilliseconds;

            Assert.Equal(String.Empty, extensionResult.Replace("bar", "").Trim());

            Assert.True(extensionElapsed < 10 || extensionElapsed < nativeElapsed * 10);
        }

        [Fact]
        public void StringReplaceExtensionShouldWorkWithXML()
        {
            var source = "<toto>My'Chain@domain.fr - and , \\e {value}</toto>";
            var expected = "<toto>My'Chain@domain.fr - and , \\e valueModified</toto>";

            var result = NExtends.Primitives.StringExtensions.Replace(source, "{VALUE}", "valueModified", StringComparison.InvariantCultureIgnoreCase);

            Assert.Equal(expected, result); //Case insensitive replacement

            result = NExtends.Primitives.StringExtensions.Replace(source, "{VALUE}", "valueModified", StringComparison.InvariantCulture);

            Assert.Equal(source, result); //Case sensitive = no remplacement
        }
    }
}
