using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
