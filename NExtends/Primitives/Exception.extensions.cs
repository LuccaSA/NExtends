using System;

namespace NExtends.Primitives
{
	public static class ExceptionExtensions
	{
		/// <summary>
		/// Utilisé par timmi.web/areas/timmi/views/oneclickactions/index.cshtml
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static string ExtractFriendlyCompleteMessage(this Exception ex)
		{
			return ex == null ? null : (ex.Message + Environment.NewLine + ExtractCompleteMessage(ex.InnerException));
		}

		public static string ExtractCompleteMessage(this Exception ex)
		{
			return ex == null ? null :
				("(" + ex.GetType() + ") " + ex.Message + Environment.NewLine +
				ExtractCompleteMessage(ex.InnerException));
		}

		public static string ExtractCompleteDescription(this Exception ex)
		{
			return ex == null ? null :
				("(" + ex.GetType() + ") " +
				ex.Message + Environment.NewLine +
				ex.StackTrace + Environment.NewLine + Environment.NewLine +
				ExtractCompleteDescription(ex.InnerException));
		}
	}
}