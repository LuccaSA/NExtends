using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace NExtends.Primitives
{
	//Pour pouvoir comparer rapidement 2 objets de la même Classe
	public class GenericEqualityComparer<T> : IEqualityComparer<T>
	{
		private Func<T, T, Boolean> _comparer;
		public GenericEqualityComparer(Func<T, T, Boolean> comparer)
		{
			_comparer = comparer;
		}

		public bool Equals(T x, T y)
		{
			return _comparer(x, y);
		}

		public int GetHashCode(T obj)
		{
			return obj.ToString().ToLower().GetHashCode();
		}
	}
}