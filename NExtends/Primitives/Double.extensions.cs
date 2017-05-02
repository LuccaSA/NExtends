using System;
using System.Globalization;

namespace NExtends.Primitives
{
	public static class DoubleExtensions
	{
		public static String ToJSON(this Double d)
		{
			return d.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// <para>Round number to given precision using mode MidpointRounding.ToEven</para>
		/// <para>
		///		If the one's digit is odd, it is changed to an even digit. Otherwise, it is left unchanged. 
		///		This behavior follows IEEE Standard 754, section 4. It is sometimes called rounding to nearest, or banker's rounding. 
		///		It minimizes rounding errors that result from consistently rounding a midpoint value in a single direction.
		///	</para>
		///	<para>For example: Round(0.165, 2) returns 0.16 and NOT 0.17</para>
		///	<para>More info on http://stackoverflow.com/questions/977796/in-c-math-round2-5-result-is-2-instead-of-3-are-you-kidding-me </para>
		/// </summary>
		/// <param name="d"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		public static Double Round(this Double d, int precision)
		{
			return Math.Round(d, precision);
		}

		/// <summary>
		/// <para>Round number to given precision using mode MidpointRounding.AwayFromZero</para>
		/// <para>ie: the normal maths rounding</para>
		///	<para>For example: RealRound(0.165, 2) returns 0.17 and NOT 0.16</para>
		/// </summary>
		/// <param name="d"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		public static Double RealRound(this Double d, int precision)
		{
			return Math.Round(d, precision, MidpointRounding.AwayFromZero);
		}

		public static Double Floor(this Double d, int precision)
		{
			var multiplier = Convert.ToDecimal(Math.Pow(10, precision));

			return (double)(Math.Floor(Convert.ToDecimal(d) * multiplier) / multiplier);
		}

		public static Double Ceiling(this Double d, int precision)
		{
			var multiplier = Math.Pow(10, precision);

			return Math.Ceiling(d * multiplier) / multiplier;
		}

		public static double tronquerDecimales(this double value)
		{
			//on passe par des decimals pour faire le floor
			//sinon bug
			//Ex: Math.Floor(10.03 * 100) / 100 => 10.02 au lieu de 10.03
			return (double)Math.Floor(Convert.ToDecimal(value) * 100) / 100;
		}
	}
}
