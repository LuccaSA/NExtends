using System;
using Newtonsoft.Json;
using System.Globalization;

namespace NExtends.Primitives
{
	public static class DateTimeExtensions
	{
		public static String ToShortUpperDay(this DateTime d)
		{
			return d.ToString("dddd").Substring(0, 3).ToUpper();
		}
		public static String ToFrenchLongText(this DateTime d)
		{
			return d.Day + " " + d.ToString("MMMM") + " " + d.Year;
		}
		public static String ToFrenchLongTextHeure(this DateTime d)
		{
			return d.Day + " " + d.ToString("MMMM") + " " + d.Year + " à " + d.Hour + "h" + d.Minute;
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
			if (step % 7 == 0) throw new Exception("Boucle infinie");

			var d_ = start.Date;
			while (d_.DayOfWeek != day)
			{
				d_ = d_.AddDays(step);
			}
			return d_;
		}

		//Renvoie une date au format JSON, limite à 2099 pour les MaxValue
		public static String ToJSONDate(this DateTime d)
		{
			return "new Date(" + Math.Min(d.Year, 2099) + ", " + (d.Month - 1) + ", " + d.Day + ")";
		}
		public static String ToJSONDateTime(this DateTime d)
		{
			return "new Date(" + Math.Min(d.Year, 2099) + ", " + (d.Month - 1) + ", " + d.Day + ", " + d.Hour + ", " + d.Minute + ", " + d.Second + ")";
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

		public static String ToJQuery(this DateTime d)
		{
			return JsonConvert.SerializeObject(d);
		}

		public static String ToJQuery(this DateTime? d)
		{
			return JsonConvert.SerializeObject(d);
		}

		[Obsolete("Use ToISO instead and parse the date on the client side")]
		public static String ToJSON(this DateTime d)
		{
			return "new Date(" + Math.Min(d.Year, 2099) + ", " + (d.Month - 1) + ", " + d.Day + ", " + d.Hour + ", " + d.Minute + ", " + d.Second + ")";
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

		public static String[] dayText = { "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi", "Dimanche" };
		public static String DayText(int day)
		{
			return dayText[day];
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
	}
}
