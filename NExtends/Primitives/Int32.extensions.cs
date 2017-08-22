using System;

namespace NExtends.Primitives
{
	public static class Int32Extensions
	{
		public static String ToJSON(this int i)
		{
			return i.ToString().Replace(",", ".");
		}
	}
}
