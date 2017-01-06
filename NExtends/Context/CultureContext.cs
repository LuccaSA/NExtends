using System;
using System.Globalization;
using System.Threading;

namespace NExtends.Context
{
	public class CultureContext : IDisposable
	{
		private CultureInfo originalCulture;
		private CultureInfo originalUICulture;

		public CultureContext(CultureInfo culture)
		{
			originalCulture = Thread.CurrentThread.CurrentCulture;
			originalUICulture = Thread.CurrentThread.CurrentUICulture;

			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}

		public void Dispose()
		{
			Thread.CurrentThread.CurrentCulture = originalCulture;
			Thread.CurrentThread.CurrentUICulture = originalUICulture;
		}
	}
}