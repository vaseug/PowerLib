using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class SelectComparer<T, K> : IComparer<T>
	{
		private Comparison<K> _comparison;
		private Func<T, K> _selector;

		#region Constructors

		public SelectComparer(Func<T, K> selector, Comparison<K> comparison)
    {
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			if (selector == null)
				throw new ArgumentNullException("selector");

			_comparison = comparison;
			_selector = selector;
		}

		public SelectComparer(Func<T, K> selector, IComparer<K> comparer)
    {
			if (comparer == null)
				throw new ArgumentNullException("comparer");
			if (selector == null)
				throw new ArgumentNullException("selector");

			_comparison = comparer.Compare;
			_selector = selector;
		}

    #endregion
    #region Methods

    public int Compare(T first, T second)
		{
			return _comparison(_selector(first), _selector(second));
		}

		#endregion
	}
}
