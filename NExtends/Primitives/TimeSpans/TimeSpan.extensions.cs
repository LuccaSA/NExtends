using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public static string ToHours(this TimeSpan span, bool showSign = false)
		{
			if (span == TimeSpan.Zero)
			{
				return "0h";
			}
			var absSpan = new TimeSpan(Math.Abs(span.Ticks));

			var sb = new StringBuilder();
			if (showSign && span > TimeSpan.Zero)
			{
				sb.Append("+");
			}
			if (showSign && span < TimeSpan.Zero)
			{
				sb.Append("-");
			}
			if (absSpan.Hours > 0)
			{
				sb.AppendFormat("{0}h", Math.Floor(absSpan.TotalHours));
			}
			if (absSpan.Minutes > 0 && absSpan.Hours == 0)
			{
				sb.AppendFormat("{0}min", absSpan.Minutes);
			}
			if (absSpan.Minutes > 0 && absSpan.Hours > 0)
			{
				sb.AppendFormat("{0}", absSpan.Minutes);
			}
			return sb.ToString();
		}
		public static string ToDays(this TimeSpan span, bool showSign = false)
		{
			if (span == TimeSpan.Zero)
			{
				return "0";
			}
			var absSpan = new TimeSpan(Math.Abs(span.Ticks));
			var sb = new StringBuilder();
			if (showSign && span > TimeSpan.Zero)
			{
				sb.Append("+");
			}
			if (showSign && span < TimeSpan.Zero)
			{
				sb.Append("-");
			}
			sb.AppendFormat("{0}", absSpan.TotalDays);
			return sb.ToString();
		}
	}
}
