using NExtends.Primitives.DateTimes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NExtends.Tests.Primitives.DateTimes
{
    public class PeriodTests
    {
        [Fact]
        public void PeriodShouldWorkOnDates()
        {
            var startsOn = new DateTime(2000, 01, 01);
            var endsOn = new DateTime(2001, 12, 31);

            var period = new Period(startsOn, endsOn);

            Assert.Equal(startsOn, period.Start);
            Assert.Equal(endsOn, period.End);
        }

        [Fact]
        public void PeriodShouldWorkOnDateTimes()
        {
            var startsAt = new DateTime(2000, 01, 01, 10, 30, 00);
            var endsAt = new DateTime(2001, 12, 31, 14, 10, 20);

            var period = new Period(startsAt, endsAt);

            Assert.Equal(startsAt, period.Start);
            Assert.Equal(endsAt, period.End);
        }
    }
}
