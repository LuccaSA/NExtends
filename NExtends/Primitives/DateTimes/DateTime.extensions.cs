using NExtends.Context;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NExtends.Primitives.DateTimes
{
	public static class DateTimeExtensions
	{
        private static CultureInfo _FrenchCultureInfo = new CultureInfo("fr-FR");

		public static String ToShortUpperDay(this DateTime d)
		{
			return d.ToString("dddd").Substring(0, 3).ToUpper();
		}
		public static String ToFrenchLongText(this DateTime d)
		{
			return d.Day + " " + d.ToString("MMMM", _FrenchCultureInfo) + " " + d.Year;
		}
		public static String ToFrenchLongTextHeure(this DateTime d)
		{
			return String.Format(_FrenchCultureInfo, "{0:d MMMM yyyy à hh'h'mm}", d);
		}
		public static String ToShortUpperMonth(this DateTime d)
		{
			return d.ToString("MMMM").Substring(0, 3).ToUpper();
		}
		public static String ToShortFrenchTimeString(this DateTime d)
		{
			String time = d.Hour + "h";

			if (d.Minute > 0)
			{
				time += d.Minute;
			}

			return time;
		}

		public static DateTime Min(this DateTime a, DateTime b) { return a > b ? b : a; }
		public static DateTime Max(this DateTime a, DateTime b) { return a > b ? a : b; }

		//Format RFC1123 correspondant à ce qui est renvoyé par une requête HTTP dans le Header Last-Modified
		public static string ToHttpDate(this DateTime d)
		{
			return d.ToString("R");
		}

		/// <summary>
		/// Minutes écoulées depuis minuit pour cette date : entre 0 et 1440
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static int Minutes(this DateTime d)
		{
			return d.Minute + d.Hour * 60;
		}

		public static bool IsFirstOfMonth(DateTime d)
		{
			return d == FirstOfMonth(d);
		}

		public static DateTime FirstOfMonth(this DateTime d)
		{
			return new DateTime(d.Year, d.Month, 1);
		}

		public static bool IsLastOfMonth(DateTime d)
		{
			return d == LastOfMonth(d);
		}
		public static DateTime LastOfMonth(this DateTime d)
		{
			return new DateTime(d.Year, d.Month, DateTime.DaysInMonth(d.Year, d.Month));
		}

		public static DateTime Previous(this DateTime d, DayOfWeek day) { return LookFor(d.AddDaysSafe(-1), day, -1); }
		public static DateTime Next(this DateTime d, DayOfWeek day) { return LookFor(d.AddDaysSafe(1), day, 1); }
		public static DateTime PreviousOrCurrent(this DateTime d, DayOfWeek day) { return LookFor(d, day, -1); }
		public static DateTime NextOrCurrent(this DateTime d, DayOfWeek day) { return LookFor(d, day, 1); }
		static DateTime LookFor(DateTime start, DayOfWeek day, int step)
		{
			if (step % 7 == 0) throw new ArgumentException("Step should be less than 7");

			var d = start.Date;
			while (d.DayOfWeek != day)
			{
				d = d.AddDays(step);
			}
			return d;
		}

		public static String ToShortISO(this DateTime d)
		{
			return d.ToString("yyyy-MM-dd");
		}

		// eg: 2010-10-21T18:38:35 => no bad surprise when parsing
		public static String ToISO(this DateTime d)
		{
			return d.ToString("s");
		}

		/// <summary>
		/// Prend en charge le TimeZone envoyé par le client embed dans la date/time exprimée en UTC
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static String ToISOz(this DateTime d)
		{
			return d.ToString("o");
		}

		public static int weekNumber(this DateTime d)
		{
			CultureInfo ciCurr = CultureInfo.CurrentCulture;
			int weekNum = ciCurr.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
			return weekNum;
		}

		//https://github.com/dotnet/coreclr/issues/2317
		public static String ToShortDateTimeString(this DateTime d)
		{
			return d.ToString("d") + " " + d.ToString("t");
		}
		/// <summary>
		///  new DateTime(2008, 8, 29, 19, 27, 15) ==> août 2008
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static String ToShortDate(this DateTime d)
		{
			return d.ToString("MMM", new CultureInfo("en-US")) + " " + d.Year;
		}

		/// <summary>
		/// Permet d'affecter un time à 23:59:59.999 à une date qui n'a pas de time
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime ToMidnightTimeIfEmpty(this DateTime date)
		{
			if (date.TimeOfDay.Ticks == 0)
			{
				return date.AddDays(1).AddMilliseconds(-1);
			}
			else
			{
				return date.AddDays(0); //Permet de renvoyer une "copie" de la date et pas un pointeur vers la date elle-même
			}
		}

		public static int GetDifferenceInFullYearsWith(this DateTime firstDate, DateTime secondDate)
		{
			//   http://stackoverflow.com/questions/4127363/date-difference-in-years-c-sharp
			// + http://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
			DateTime zeroTime = new DateTime(1, 1, 1);

			var timeSpan = firstDate - secondDate;
            if (timeSpan < System.TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(secondDate), "secondDate should be lower than the current DateTime");
            }
            var yearResult = (zeroTime + timeSpan).Year - 1; // because we start at year 1 for the Gregorian calendar, we must subtract a year here.
			// Il peut y avoir des cas à la con : 31/05/2014-01/06/1995 donne 19 ans alors qu'il nous faut 18 ans ! (probablement dû aux années bisextiles)

			if (firstDate.AddYears(-yearResult) < secondDate)
			{
				yearResult--;
			}
			return yearResult;
		}

		public static int GetDifferenceInFullMonths(this DateTime firstDate, DateTime secondDate)
		{
			var deltaYear = firstDate.Year - secondDate.Year;
			var deltaMonth = firstDate.Month - secondDate.Month;
			if (firstDate.Day - secondDate.Day < 0)
			{
				deltaMonth--;
			}

			return deltaMonth + 12 * deltaYear;
		}

		public static DateTime AddDaysSafe(this DateTime date, double value)
		{
			if ((DateTime.MaxValue - date).TotalDays > value)
			{
				if ((date - DateTime.MinValue).TotalDays > -value)
				{
					return date.AddDays(value);
				}
				else
				{
					return DateTime.MinValue;
				}
			}
			else
			{
				return DateTime.MaxValue;
			}
		}

        public static string GetDayOrdinalSuffix(this DateTime date, CultureInfo culture)
        {
            switch (culture.LCID)
            {
                case 1033:
                case 2057:
                    return GetEnglishDayOrdinalSuffix(date);
                case 1036:
                    return GetFrenchDayOrdinalSuffix(date);
                case 1031:
                    return GetGermanDayOrdinalSuffix();
                case 2067:
                    return GetDutchDayOrdinalSuffix();
                default:
                    return "";
            }
        }

        public static DateTime GetLastSundayOfYearISO8601(int year)
        {
            var lastDayOfYear = new DateTime(year, 12, 31);
            if (lastDayOfYear.DayOfWeek == DayOfWeek.Sunday)
                return lastDayOfYear;

            var gapDaysNumberToSunday = (lastDayOfYear.DayOfWeek >= DayOfWeek.Thursday ? 7 - (int)lastDayOfYear.DayOfWeek : -(int)lastDayOfYear.DayOfWeek);
            return lastDayOfYear.AddDays(gapDaysNumberToSunday);
        }

        public static DateTime GetStartOfWeek(this DateTime value)
        {
            var date = value.Date;
            //DayOfWeek starts on Sunday but we want weeks starting on monday
            int daysIntoWeek = (int)date.DayOfWeek == 0 ? 6 : (int)date.DayOfWeek - 1;
            return date.AddDays(-daysIntoWeek);
        }

        public static string ToUserCultureFormat(this DateTime date)
        {
            return date.ToString(System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern);
        }

        // http://stackoverflow.com/a/20175
        private static string GetEnglishDayOrdinalSuffix(DateTime date)
        {
            switch (date.Day % 100)
            {
                case 11:
                case 12:
                case 13:
                    return "th";
            }

            switch (date.Day % 10)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }

        private static string GetFrenchDayOrdinalSuffix(DateTime date)
        {
            if (date.Day == 1)
            {
                return "er";
            }
            else
            {
                return "";
            }
        }

        private static string GetGermanDayOrdinalSuffix()
        {
            return ".";
        }

        private static string GetDutchDayOrdinalSuffix()
        {
            return "";
        }

        public static DateTime TrimMilliseconds(this DateTime dt)
        {
            // Source: http://stackoverflow.com/a/11558076
            return dt.AddTicks(-dt.Ticks % TimeSpan.TicksPerSecond);
        }

        public static Period GetQuarterRange(this DateTime d)
        {
            var firstMonthOfQuarter = Math.Floor((double)(d.Month - 1) / 3) * 3 + 1;

            var quarterStart = new DateTime(d.Year, (int)firstMonthOfQuarter, 1);
            var quarterEnd = quarterStart.AddMonths(2).LastOfMonth();

            return new Period(quarterStart, quarterEnd);
        }

        public static int GetISO8601WeekNumber(this DateTime d)
        {
            DateTime date = d;
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(d);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                date = date.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static Period GetIncludingPeriod(this DateTime date, TimeSliceMode timeSliceMode)
        {
            DateTime start, end;

            switch (timeSliceMode)
            {
                default:
                case TimeSliceMode.Week:
                    start = date.PreviousOrCurrent(DayOfWeek.Monday);
                    end = date.NextOrCurrent(DayOfWeek.Sunday);
                    break;

                case TimeSliceMode.Fortnight:
                    if (date.Day <= 15)
                    {
                        start = new DateTime(date.Year, date.Month, 1);
                        end = new DateTime(date.Year, date.Month, 15);
                    }
                    else
                    {
                        start = new DateTime(date.Year, date.Month, 16);
                        end = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                    }
                    break;

                case TimeSliceMode.Month:
                    start = new DateTime(date.Year, date.Month, 1);
                    end = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                    break;
            }

            return new Period(start, end);
        }

        public static List<Period> GetPreviousPeriods(this DateTime date, TimeSliceMode timeSliceMode, int nbPeriod, bool includeDate)
        {
            // we want to include dtStart => add one day
            DateTime startOfPeriodSeek = includeDate ? date.AddDays(1) : date;

            var periods = new List<Period>();

            for (int i = 0; i < nbPeriod; i++)
            {
                // Date we look => last date of previous range
                var period = GetIncludingPeriod(startOfPeriodSeek.AddDays(-1), timeSliceMode);

                periods.Add(new Period(period.Start, period.End));

                // for next iteration
                startOfPeriodSeek = period.Start;
            }

            return periods;
        }
    }
}
