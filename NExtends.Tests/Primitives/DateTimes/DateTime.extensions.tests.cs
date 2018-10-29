using NExtends.Context;
using NExtends.Primitives.DateTimes;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace NExtends.Tests.Primitives.DateTimes
{
    public class DateTimeExtensionsTest
    {
        [Fact]
        public void TestLastDayOfMonth()
        {
            //date random
            Assert.Equal(new DateTime(2015, 06, 30), new DateTime(2015, 06, 10).LastOfMonth());

            //année bissextile
            Assert.Equal(new DateTime(2012, 02, 29), new DateTime(2012, 02, 1).LastOfMonth());

            //changement d'année
            Assert.Equal(new DateTime(2015, 12, 31), new DateTime(2015, 12, 31).LastOfMonth());
            Assert.Equal(new DateTime(2015, 12, 31), new DateTime(2015, 12, 1).LastOfMonth());
            Assert.Equal(new DateTime(2016, 1, 31), new DateTime(2016, 1, 1).LastOfMonth());
        }

        [Fact]
        public void TestAddDaySafe()
        {
            var min = DateTime.MinValue;
            var max = DateTime.MaxValue;

            Assert.Equal(min.Ticks, min.AddDaysSafe(-1).Ticks);
            Assert.Equal(min.AddDays(1).Ticks, min.AddDaysSafe(1).Ticks);

            Assert.Equal(max.AddDaysSafe(-1).Ticks, max.AddDaysSafe(-1).Ticks);
            Assert.Equal(max.Ticks, max.AddDaysSafe(1).Ticks);
        }

        [Fact]
        public void TestGetDifferenceInFullYearsWithNothingSpecific()
        {
            var date1 = new DateTime(2014, 10, 1);
            var date2 = new DateTime(2010, 3, 23);

            Assert.Equal(4, date1.GetDifferenceInFullYearsWith(date2));
        }

        [Fact]
        public void TestGetDifferenceInFullYearsWithJustADayMissing()
        {
            var date1 = new DateTime(2014, 10, 1);
            var date2 = new DateTime(2010, 10, 2);

            Assert.Equal(3, date1.GetDifferenceInFullYearsWith(date2));
        }

        [Fact]
        public void TestGetDifferenceInFullYearsWithLessThanAYear()
        {
            var date1 = new DateTime(2014, 10, 1);
            var date2 = new DateTime(2014, 7, 16);

            Assert.Equal(0, date1.GetDifferenceInFullYearsWith(date2));
        }

        [Fact]
        public void TestGetDifferenceInFullYearsWithZeroDay()
        {
            var date1 = new DateTime(2014, 10, 1);

            Assert.Equal(0, date1.GetDifferenceInFullYearsWith(date1));
        }

        [Fact]
        public void TestGetDifferenceInFullYearsWithLessThanAYearWithOverlappingYears()
        {
            var date1 = new DateTime(2014, 3, 1);
            var date2 = new DateTime(2013, 11, 30);

            Assert.Equal(0, date1.GetDifferenceInFullYearsWith(date2));
        }

        [Fact]
        public void TestGetDifferenceInFullYearsExactlySameDateButDifferentYear()
        {
            var date1 = new DateTime(2014, 3, 1);
            var date2 = new DateTime(2013, 3, 1);

            Assert.Equal(1, date1.GetDifferenceInFullYearsWith(date2));
        }

        [Fact]
        public void TestGetDifferenceInFullYearsEdgeCase1()
        {
            // 31/05/2013-01/06/1995 donnait 19 ans alors qu'il nous faut 18 ans ! (probablement dû aux années bisextiles)
            var date1 = new DateTime(2014, 5, 31);
            var date2 = new DateTime(1995, 6, 1);
            Assert.Equal(18, date1.GetDifferenceInFullYearsWith(date2));

        }
        [Fact]
        public void PreviousOrCurrentShoudWork()
        {
            var monday = new DateTime(2015, 09, 28, 11, 26, 00);
            var tuesday = new DateTime(2015, 09, 29, 11, 26, 00);
            var nextMonday = new DateTime(2015, 10, 05, 11, 26, 00);
            var nextTuesday = new DateTime(2015, 10, 06, 11, 26, 00);

            Assert.Equal(monday.Date, monday.PreviousOrCurrent(DayOfWeek.Monday));
            Assert.Equal(monday.Date, tuesday.PreviousOrCurrent(DayOfWeek.Monday));
            Assert.NotEqual(monday.Date, nextMonday.PreviousOrCurrent(DayOfWeek.Monday));
            Assert.NotEqual(monday.Date, nextTuesday.PreviousOrCurrent(DayOfWeek.Monday));

            Assert.Equal(tuesday.Date, tuesday.PreviousOrCurrent(DayOfWeek.Tuesday));
            Assert.Equal(tuesday.Date, nextMonday.PreviousOrCurrent(DayOfWeek.Tuesday));
            Assert.NotEqual(tuesday.Date, nextTuesday.PreviousOrCurrent(DayOfWeek.Tuesday));
        }
        [Fact]
        public void PreviousShoudWork()
        {
            var monday = new DateTime(2015, 09, 28, 11, 26, 00);
            var tuesday = new DateTime(2015, 09, 29, 11, 26, 00);
            var nextMonday = new DateTime(2015, 10, 05, 11, 26, 00);
            var nextTuesday = new DateTime(2015, 10, 06, 11, 26, 00);

            Assert.NotEqual(monday.Date, monday.Previous(DayOfWeek.Monday));
            Assert.Equal(monday.Date, tuesday.Previous(DayOfWeek.Monday));
            Assert.Equal(monday.Date, nextMonday.Previous(DayOfWeek.Monday));
            Assert.Equal(nextMonday.Date, nextTuesday.Previous(DayOfWeek.Monday));
            Assert.NotEqual(tuesday.Date, tuesday.Previous(DayOfWeek.Tuesday));
            Assert.Equal(tuesday.Date, nextMonday.Previous(DayOfWeek.Tuesday));
            Assert.Equal(tuesday.Date, nextTuesday.Previous(DayOfWeek.Tuesday));
        }

        [Fact]
        public void NextOrCurrentShoudWork()
        {
            var monday = new DateTime(2015, 09, 28, 11, 26, 00);
            var tuesday = new DateTime(2015, 09, 29, 11, 26, 00);
            var nextMonday = new DateTime(2015, 10, 05, 11, 26, 00);
            var nextTuesday = new DateTime(2015, 10, 06, 11, 26, 00);

            Assert.Equal(monday.Date, monday.NextOrCurrent(DayOfWeek.Monday));
            Assert.Equal(nextMonday.Date, tuesday.NextOrCurrent(DayOfWeek.Monday));
            Assert.NotEqual(monday.Date, tuesday.NextOrCurrent(DayOfWeek.Monday));

            Assert.Equal(tuesday.Date, monday.NextOrCurrent(DayOfWeek.Tuesday));
            Assert.Equal(tuesday.Date, tuesday.NextOrCurrent(DayOfWeek.Tuesday));
            Assert.NotEqual(tuesday.Date, nextMonday.NextOrCurrent(DayOfWeek.Tuesday));
            Assert.Equal(nextTuesday.Date, nextMonday.NextOrCurrent(DayOfWeek.Tuesday));
        }
        [Fact]
        public void NextShoudWork()
        {
            var monday = new DateTime(2015, 09, 28, 11, 26, 00);
            var tuesday = new DateTime(2015, 09, 29, 11, 26, 00);
            var nextMonday = new DateTime(2015, 10, 05, 11, 26, 00);
            var nextTuesday = new DateTime(2015, 10, 06, 11, 26, 00);

            Assert.Equal(nextMonday.Date, monday.Next(DayOfWeek.Monday));
            Assert.Equal(nextMonday.Date, tuesday.Next(DayOfWeek.Monday));

            Assert.Equal(tuesday.Date, monday.Next(DayOfWeek.Tuesday));
            Assert.Equal(nextTuesday.Date, tuesday.Next(DayOfWeek.Tuesday));
            Assert.Equal(nextTuesday.Date, nextMonday.Next(DayOfWeek.Tuesday));
        }

        [Fact]
        public void TestDayOrdinalUSEnglish()
        {
            // Ça ferait un beau property-based test :)
            var usCultureInfo = CultureInfo.GetCultureInfo(1033);
            var expectedCases = new Dictionary<int, string>() {
                {1, "st"}, {2, "nd"}, {3, "rd"}, {4, "th"}, {5, "th"}, {6, "th"}, {7, "th"}, {8, "th"}, {9, "th"}, {10, "th"},
                {11, "th"}, {12, "th"}, {13, "th"}, {14, "th"}, {15, "th"}, {16, "th"}, {17, "th"}, {18, "th"}, {19, "th"}, {20, "th"},
                {21, "st"}, {22, "nd"}, {23, "rd"}, {24, "th"}, {25, "th"}, {26, "th"}, {27, "th"}, {28, "th"}, {29, "th"}, {30, "th"},
                {31, "st"},
            };

            foreach (var kvp in expectedCases)
            {
                var testDt = new DateTime(2016, 07, kvp.Key);
                Assert.Equal(kvp.Value, testDt.GetDayOrdinalSuffix(usCultureInfo));//, String.Format("The day with value {0} should have {1} after the number, e.g. June {0}{1}", kvp.Key, kvp.Value));
            }
        }

        [Fact]
        public void TestDayOrdinalGBEnglish()
        {
            // Ça ferait un beau property-based test :)
            var gbCultureInfo = CultureInfo.GetCultureInfo(2057);
            var expectedCases = new Dictionary<int, string>() {
                {1, "st"}, {2, "nd"}, {3, "rd"}, {4, "th"}, {5, "th"}, {6, "th"}, {7, "th"}, {8, "th"}, {9, "th"}, {10, "th"},
                {11, "th"}, {12, "th"}, {13, "th"}, {14, "th"}, {15, "th"}, {16, "th"}, {17, "th"}, {18, "th"}, {19, "th"}, {20, "th"},
                {21, "st"}, {22, "nd"}, {23, "rd"}, {24, "th"}, {25, "th"}, {26, "th"}, {27, "th"}, {28, "th"}, {29, "th"}, {30, "th"},
                {31, "st"},
            };

            foreach (var kvp in expectedCases)
            {
                var testDt = new DateTime(2016, 07, kvp.Key);
                Assert.Equal(kvp.Value, testDt.GetDayOrdinalSuffix(gbCultureInfo));//, String.Format("The day with value {0} should have {1} after the number, e.g. July {0}{1}", kvp.Key, kvp.Value));
            }
        }

        [Fact]
        public void TestDayOrdinalFrench()
        {
            // Ça ferait un beau property-based test :)
            var frenchCultureInfo = CultureInfo.GetCultureInfo(1036);
            var expectedCases = new Dictionary<int, string>() {
                {1, "er"}, {2, ""}, {3, ""}, {4, ""}, {5, ""}, {6, ""}, {7, ""}, {8, ""}, {9, ""}, {10, ""},
                {11, ""}, {12, ""}, {13, ""}, {14, ""}, {15, ""}, {16, ""}, {17, ""}, {18, ""}, {19, ""}, {20, ""},
                {21, ""}, {22, ""}, {23, ""}, {24, ""}, {25, ""}, {26, ""}, {27, ""}, {28, ""}, {29, ""}, {30, ""},
                {31, ""},
            };

            foreach (var kvp in expectedCases)
            {
                var testDt = new DateTime(2016, 07, kvp.Key);
                Assert.Equal(kvp.Value, testDt.GetDayOrdinalSuffix(frenchCultureInfo));//, String.Format("The day with value {0} should have {1} after the number, e.g. {0}{1} Juillet", kvp.Key, kvp.Value));
            }
        }

        [Fact]
        public void TestDayOrdinalGerman()
        {
            // Ça ferait un beau property-based test :)
            var germanCultureInfo = CultureInfo.GetCultureInfo(1031);
            for (var day = 1; day <= 31; day++)
            {
                var testDt = new DateTime(2016, 07, day);
                Assert.Equal(".", testDt.GetDayOrdinalSuffix(germanCultureInfo));//, String.Format("The day with value {0} should have . after the number, e.g. {0}. Juli", day));
            }
        }

        [Fact]
        public void TestDayOrdinalDutch()
        {
            // Ça ferait un beau property-based test :)
            var dutchCultureInfo = CultureInfo.GetCultureInfo(2067);

            for (var day = 1; day <= 31; day++)
            {
                var testDt = new DateTime(2016, 07, day);
                Assert.Equal("", testDt.GetDayOrdinalSuffix(dutchCultureInfo));//, String.Format("The day with value {0} should have . after the number, e.g. {0} juli", day));
            }
        }

        [Fact]
        public void TestBirthDateFuture()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                // On doit tester que l'exception est bien repéré
                var presentDate = new DateTime(2014, 5, 31);
                var futureDate = new DateTime(2014, 10, 31);
                presentDate.GetDifferenceInFullYearsWith(futureDate);
            });
        }

        [Fact]
        public void TestSameDate()
        {
            // On test avec deux dates identiques
            var date = new DateTime(2014, 5, 31);
            Assert.Equal(0, date.GetDifferenceInFullYearsWith(date));
        }

        [Fact]
        public void TestLastSundayOf2006ISO8601()
        {
            Assert.Equal(DateTimeExtensions.GetLastSundayOfYearISO8601(2006), new DateTime(2006, 12, 31));
        }

        [Fact]
        public void TestLastSundayOf2008ISO8601()
        {
            Assert.Equal(DateTimeExtensions.GetLastSundayOfYearISO8601(2008), new DateTime(2008, 12, 28));
        }

        [Fact]
        public void TestLastSundayOf2009ISO8601()
        {
            Assert.Equal(DateTimeExtensions.GetLastSundayOfYearISO8601(2009), new DateTime(2010, 1, 3));
        }

        [Fact]
        public void TestToFrenchLongTextHeureShouldAlwaysWriteMinutesWithTwoNumbers()
        {
            var d = new DateTime(2018, 6, 1, 10, 9, 0);
            Assert.Contains("09", d.ToFrenchLongTextHeure());
        }

        [Fact]
        public void TestToFrenchLongTextHeureShouldAlwaysWriteMonthInFrench()
        {
            var d = new DateTime(2018, 6, 1, 10, 0, 0);
            using (new CultureContext("en-US"))
            {
                Assert.Contains("juin", d.ToFrenchLongTextHeure().ToLower());
            }
        }
        [Fact]
        public void TestToFrenchLongTextShouldAlwaysWriteMonthInFrench()
        {
            var d = new DateTime(2018, 6, 1, 10, 0, 0);
            using (new CultureContext("en-US"))
            {
                Assert.Contains("juin", d.ToFrenchLongText().ToLower());
            }
        }

        [Theory]
        [InlineData(2018, 11, 5, 22, 30, "fr-FR", "05/11/2018 22:30")]
        [InlineData(2018, 11, 5, 22, 30, "en-US", "11/5/2018 10:30 PM")]
        public void ToShortDateTimeStringShouldWork(int year, int month, int day, int hour, int minute, string cultureName, string expected)
        {
            var d = new DateTime(year, month, day, hour, minute, 00);

            Assert.Equal(expected, d.ToShortDateTimeString(CultureInfo.GetCultureInfo(cultureName)));
        }
    }
}
