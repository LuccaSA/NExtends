using System;

namespace NExtends.Primitives.Decimals
{
	public static class DecimalExtensions
	{
		/// <summary>
		/// Does the Mathematical rounding and not the banker's rounding
		/// </summary>
		/// <param name="d"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		public static Decimal MathRound(this Decimal d, int precision)
		{
			return Math.Round(d, precision, MidpointRounding.AwayFromZero);
		}
	}
}
