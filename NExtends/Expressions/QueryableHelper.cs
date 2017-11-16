using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NExtends.Expressions
{
	public static class QueryableHelper
	{
		/// <summary>
		/// Inspiration : http://stackoverflow.com/questions/3631547/select-right-generic-method-with-reflection
		/// </summary>
		/// <param name="TSource"></param>
		/// <param name="TResult"></param>
		/// <returns></returns>
		public static MethodInfo GetSelectMethod()
		{
			var methods = typeof(Enumerable).GetMethods()
				 .Where(x => x.Name == "Select");

			var subs = methods
				 .Select(x => new { M = x, P = x.GetParameters() })
				 .Where(x => x.P.Length == 2
							 && x.P[0].ParameterType.IsGenericType
							 && x.P[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
							 && x.P[1].ParameterType.IsGenericType
							 && x.P[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));

			return subs
				 .Select(x => x.M)
				 .SingleOrDefault();
		}
	}
}
