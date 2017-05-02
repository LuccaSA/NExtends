using System;
using System.Globalization;

namespace NExtends.Context
{
	public class CultureContext : IDisposable
	{
		private CultureInfo originalCulture;
		private CultureInfo originalUICulture;

		public CultureContext(CultureInfo culture)
		{
			originalCulture = CultureInfo.CurrentCulture;
			originalUICulture = CultureInfo.CurrentUICulture;

			CultureInfo.CurrentCulture = culture;
			CultureInfo.CurrentUICulture = culture;
		}

		public void Dispose()
		{
			CultureInfo.CurrentCulture = originalCulture;
			CultureInfo.CurrentUICulture = originalUICulture;
		}
	}
}