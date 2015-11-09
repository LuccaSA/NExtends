using System;

namespace NExtends.Primitives
{
	public static class ObjectExtensions
	{
		public static object ToXMLAttribute(this object o)
		{
			throw new NotImplementedException();
		}

		public static String ToXML(this object o)
		{
			if (o.GetType() == typeof(int))
			{
				return ((int)o).ToXML();
			}
			else if (o.GetType() == typeof(double))
			{
				return ((double)o).ToXML();
			}
			else if (o.GetType() == typeof(DateTime))
			{
				return ((DateTime)o).ToISO().ToXML();
			}
			else if (o.GetType() == typeof(bool))
			{
				return ((bool)o).ToXML();
			}
			else if (o.GetType() == typeof(Uri))
			{
				return ((Uri)o).ToXML();
			}
			else if (o.GetType() == typeof(string))
			{
				return ((string)o).ToXML();
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}