using System;

namespace NExtends.Primitives
{
	public static class ExceptionExtensions
	{
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