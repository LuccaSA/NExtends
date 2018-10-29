using NExtends.Primitives.DateTimes;
using System;
using Xunit;

namespace NExtends.Tests.Primitives.DateTimes
{
    public class DateTimeHelperTest
    {
        [Fact]
        public void TestGetDateOfFirstDayOfWeek()
        {
            Assert.Equal(new DateTime(2017, 01, 02), DateTimeHelper.GetDateOfFirstDayOfWeek(2017, 1));
            Assert.Equal(new DateTime(2017, 07, 10), DateTimeHelper.GetDateOfFirstDayOfWeek(2017, 28));
            Assert.Equal(new DateTime(2018, 03, 12), DateTimeHelper.GetDateOfFirstDayOfWeek(2018, 11));
        }
    }
}
