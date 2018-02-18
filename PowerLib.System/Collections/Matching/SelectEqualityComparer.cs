using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class SelectEqualityComparer<T, K> : IEqualityComparer<T>
	{
		private Equality<K> _equality;
		private Func<K, int> _hasher;
		private Func<T, K> _selector;

		#region Constructors

		public SelectEqualityComparer(Func<T, K> selector, Equality<K> equality, Func<K, int> hasher)
    {
			if (equality == null)
				throw new ArgumentNullException("equality");
			if (hasher == null)
				throw new ArgumentNullException("hasher");
			if (selector == null)
				throw new ArgumentNullException("selector");

			_equality = equality;
			_hasher = hasher;
			_selector = selector;
		}

		public SelectEqualityComparer(Func<T, K> selector, IEqualityComparer<K> equalityComparer)
    {
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");
			if (selector == null)
				throw new ArgumentNullException("selector");

			_equality = equalityComparer.Equals;
			_hasher = equalityComparer.GetHashCode;
			_selector = selector;
		}

    #endregion
    #region Methods

    public bool Equals(T first, T second)
		{
			return _equality(_selector(first), _selector(second));
		}

		public int GetHashCode(T value)
		{
			return _hasher(_selector(value));
		}

		#endregion
	}
}
