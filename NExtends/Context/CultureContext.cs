using System;
using System.Globalization;

namespace NExtends.Context
{
	public class CultureContext : IDisposable
	{
		private readonly CultureInfo _originalCulture;
		private readonly CultureInfo _originalUiCulture;

        public CultureContext(string culture)
            : this(new CultureInfo(culture))
        {
        }

        public CultureContext(CultureInfo culture)
		{
			_originalCulture = CultureInfo.CurrentCulture;
			_originalUiCulture = CultureInfo.CurrentUICulture;

			CultureInfo.CurrentCulture = culture;
			CultureInfo.CurrentUICulture = culture;
		}

		public void Dispose()
		{
			CultureInfo.CurrentCulture = _originalCulture;
			CultureInfo.CurrentUICulture = _originalUiCulture;
		}
	}
}