using NExtends.Time;
using System;
using Xunit;

namespace NExtends.Tests.Time
{
    public class StandardTimeServiceTests
    {
        [Fact]
        public void StandardTime_should_beCloseToDateTimeDotNow()
        {
            var standardTimeService = new StandardTimeService();

            var now = DateTime.Now;
            var almostNow = standardTimeService.Now;

            var difference = (almostNow - now).TotalSeconds;

            Assert.InRange<double>(difference, 0, 1);
        }

        [Fact]
        public void StandardTime_should_beCloseToDateTimeDotToday()
        {
            var standardTimeService = new StandardTimeService();

            var today = DateTime.Today;
            var almostToday = standardTimeService.Today;

            //This test can fail if run at midnight !
            Assert.Equal(today, almostToday);
        }
    }
}
