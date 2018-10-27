using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NExtends.Primitives.Doubles;

namespace NExtends.Primitives.TimeSpans
{
	public static class TimeSpanExtensions
	{
		public static TimeSpan Max(TimeSpan t1, TimeSpan t2)
		{
			return t1.Ticks > t2.Ticks ? t1 : t2;
		}

		public static TimeSpan Min(TimeSpan t1, TimeSpan t2)
		{
			return t1.Ticks < t2.Ticks ? t1 : t2;
		}

		public static TimeSpan Multiply(this TimeSpan t1, decimal multiplicator)
		{
			//do not try reusing the long version of it => decimal get truncated
			return new TimeSpan((long)(t1.Ticks * multiplicator));
		}

		public static TimeSpan Multiply(this TimeSpan t1, long multiplicator)
		{
			return new TimeSpan(t1.Ticks * multiplicator);
		}

		public static TimeSpan DividedBy(this TimeSpan t1, int divider)
		{
			return new TimeSpan(t1.Ticks / divider);
		}

		public static TimeSpan Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, TimeSpan> selector)
		{
			return new TimeSpan(source.Sum(t => selector(t).Ticks));
		}

		public static bool IsPositive(this TimeSpan t1) { return t1.Ticks > 0; }
		public static bool IsPositiveOrZero(this TimeSpan t1) { return t1.Ticks >= 0; }
		public static bool IsNegativeOrZero(this TimeSpan t1) { return t1.Ticks <= 0; }
		public static bool IsNegative(this TimeSpan t1) { return t1.Ticks < 0; }

        public static string Humanize(this TimeSpan timeSpan, TimeUnit timeUnit, TimeInitials initials, bool showSign = false)
        {
            switch (timeUnit)
            {
                case TimeUnit.Day:
                    return ToDays(timeSpan, initials, showSign);
                case TimeUnit.Duration:
                case TimeUnit.Time:
                    return ToHours(timeSpan, initials, showSign);
                case TimeUnit.NotApplicable:
                default:
                    throw new InvalidEnumArgumentException(nameof(timeUnit));
            }
        }

        public static string ToHours(this TimeSpan timeSpan, TimeInitials initials, bool showSign = false)
        {
            if (timeSpan == TimeSpan.Zero)
            {
                return "-";
            }
            var absSpan = new TimeSpan(Math.Abs(timeSpan.Ticks));
            var totalHours = Math.Floor(absSpan.TotalHours);

            var sb = new StringBuilder();
            if (showSign && timeSpan > TimeSpan.Zero)
            {
                sb.Append("+");
            }
            if (timeSpan < TimeSpan.Zero)
            {
                sb.Append("-");
            }
            if (totalHours > 0)
            {
                sb.Append(totalHours + initials.HoursInitial);
            }
            if (absSpan.Minutes > 0)
            {
                sb.Append(absSpan.Minutes.ToString("D2"));
            }
            if (absSpan.Minutes > 0 && totalHours == 0)
            {
                sb.Append(initials.MinutesInitial);
            }
            return sb.ToString();
        }

        public static string ToDays(this TimeSpan span, TimeInitials initials, bool showSign = false)
        {
            if (span == TimeSpan.Zero)
            {
                return "-";
            }
            var absSpan = new TimeSpan(Math.Abs(span.Ticks));
            var sb = new StringBuilder();
            if (showSign && span > TimeSpan.Zero)
            {
                sb.Append("+");
            }
            if (span < TimeSpan.Zero)
            {
                sb.Append("-");
            }
            sb.AppendFormat("{0} " + initials.DaysInitial, absSpan.TotalDays.RealRound(5));
            return sb.ToString();
        }
    }
}
