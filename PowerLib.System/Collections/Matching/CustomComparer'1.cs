using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class CustomComparer<T> : IComparer<T>
	{
		private Comparison<T> _comparison;

		#region Constructor

		public CustomComparer(Comparison<T> comparison)
		{
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			_comparison = comparison;
		}

		public CustomComparer(IComparer<T> comparer)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			_comparison = comparer.Compare;
		}

		#endregion
		#region Methods

		public int Compare(T first, T second)
		{
			return _comparison(first, second);
		}

		#endregion
	}
}
