using NExtends.Primitives.DateTimes;
using System;
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

            Assert.Equal(startsOn, period.StartsAt);
            Assert.Equal(endsOn, period.EndsAt);
        }

        [Fact]
        public void PeriodShouldWorkOnDateTimes()
        {
            var startsAt = new DateTime(2000, 01, 01, 10, 30, 00);
            var endsAt = new DateTime(2001, 12, 31, 14, 10, 20);

            var period = new Period(startsAt, endsAt);

            Assert.Equal(startsAt, period.StartsAt);
            Assert.Equal(endsAt, period.EndsAt);
        }

        [Fact]
        public void PeriodCannotBeNegative()
        {
            Assert.Throws<NegativeDurationException>(() =>
            {
                var period = new Period(new DateTime(2018, 10, 30), new DateTime(2018, 10, 28));
            });
        }

        [Fact]
        public void PeriodCanBeZeroSize()
        {
            var startAndEnd = new DateTime(2018, 10, 30);

            var period = new Period(startAndEnd, startAndEnd);
        }

        [Fact]
        public void ITimeBlockEndCannotBeModifiedToBeNegative()
        {
            var startAndEnd = new DateTime(2018, 10, 30);

            ITimeBlock period = new Period(startAndEnd, startAndEnd);

            Assert.Throws<NegativeDurationException>(() =>
            {
                period.ChangeEndsAt(new DateTime(2018, 10, 28));
            });
        }

        [Fact]
        public void ITimeBlockStartCannotBeModifiedToBeNegative()
        {
            var startAndEnd = new DateTime(2018, 10, 28);

            ITimeBlock period = new Period(startAndEnd, startAndEnd);

            Assert.Throws<NegativeDurationException>(() =>
            {
                period.ChangeStartsAt(new DateTime(2018, 10, 30));
            });
        }

        [Fact]
        public void ITimeBlockDurationCannotBeModifiedToBeNegative()
        {
            var startAndEnd = new DateTime(2018, 10, 28);

            ITimeBlock period = new Period(startAndEnd, startAndEnd);

            Assert.Throws<NegativeDurationException>(() =>
            {
                period.ChangeDuration(TimeSpan.FromSeconds(-1));
            });
        }

        [Fact]
        public void ITimeBlockDurationCanBeDifferentFromEndMinusStart()
        {
            var startAndEnd = new DateTime(2018, 10, 28);

            ITimeBlock period = new Period(startAndEnd, startAndEnd);

            period = period.ChangeDuration(TimeSpan.FromSeconds(1));

            Assert.Equal(TimeSpan.FromSeconds(1), period.Duration);
            Assert.Equal(TimeSpan.FromSeconds(0), period.EndsAt - period.StartsAt);
        }
    }
}
