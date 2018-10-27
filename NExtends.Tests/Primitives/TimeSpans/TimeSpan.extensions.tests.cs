using NExtends.Primitives.TimeSpans;
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

            var result = TimeSpanExtensions.Multiply(time, multiplicator);

            Assert.Equal(time.Ticks * multiplicator, Convert.ToDecimal(result.Ticks));
        }

        [Theory]
        [InlineData(0, true, "-")]
        [InlineData(0, false, "-")]
        [InlineData(9, true, "+09mn")]
        [InlineData(9, false, "09mn")]
        [InlineData(-9, true, "-09mn")]
        [InlineData(-9, false, "-09mn")]
        [InlineData(69, true, "+1h09")]
        [InlineData(69, false, "1h09")]
        [InlineData(-69, true, "-1h09")]
        [InlineData(-69, false, "-1h09")]
        [InlineData(1449, false, "24h09")]
        public void TimeSpanToHoursShouldWork(int timeInMinutes, bool showSign, string expected)
        {
            var timespan = TimeSpan.FromMinutes(timeInMinutes);
            var initials = new TimeInitials("mn", "h", "j");
            var result = TimeSpanExtensions.ToHours(timespan, initials, showSign);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, true, "-")]
        [InlineData(0, false, "-")]
        [InlineData(9, true, "+0.00625 j")]
        [InlineData(9, false, "0.00625 j")]
        [InlineData(-9, true, "-0.00625 j")]
        [InlineData(-9, false, "-0.00625 j")]
        [InlineData(69, true, "+0.04792 j")]
        [InlineData(69, false, "0.04792 j")]
        [InlineData(-69, true, "-0.04792 j")]
        [InlineData(-69, false, "-0.04792 j")]
        [InlineData(1449, false, "1.00625 j")]
        public void TimeSpanToDaysShouldWork(int timeInMinutes, bool showSign, string expected)
        {
            var timespan = TimeSpan.FromMinutes(timeInMinutes);
            var initials = new TimeInitials("mn", "h", "j");
            var result = TimeSpanExtensions.ToDays(timespan, initials, showSign);

            Assert.Equal(expected, result);
        }
    }
}
