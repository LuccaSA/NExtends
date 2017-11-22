using System;
using Xunit;

namespace NExtends.Tests.Primitives.TimeSpans
{
    public class TimeSpanExtensionsTests
    {
        [Fact]
        public void TimespanMultiply()
        {
            var time = new TimeSpan(1, 0, 0);
            var multiplicator = 1.1M;

            var result = NExtends.Primitives.TimeSpans.TimeSpanExtensions.Multiply(time, multiplicator);

            Assert.Equal(time.Ticks * multiplicator, Convert.ToDecimal(result.Ticks));
        }
    }
}
