using NExtends.Time;
using System;
using System.Threading;
using Xunit;

namespace NExtends.Tests.Time
{
    public class WaybackTimeServiceTests
    {
        [Fact]
        public void WaybackTime_should_beCloseToASpecifiedTime()
        {
            var reference = new DateTime(2018, 01, 01, 14, 00, 00);
            var waybackTimeService = new WaybackTimeService(reference);

            Thread.Sleep(100); //100 milliseconds

            var almostThen = waybackTimeService.Now;

            var difference = (almostThen - reference).TotalMilliseconds;

            Assert.InRange<double>(difference, 100, 200);
        }

        [Fact]
        public void WaybackTime_should_beCloseToASpecifiedDate()
        {
            var reference = new DateTime(2018, 01, 01, 14, 00, 00);
            var waybackTimeService = new WaybackTimeService(reference);

            var thatDay = waybackTimeService.Today;

            Assert.Equal(reference.Date, thatDay);
        }
    }
}
