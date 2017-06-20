using System;
using System.Collections;

namespace PowerLib.System.Collections.Matching
{
	public sealed class CustomComparer : IComparer
	{
		private Comparison<object> _comparison;

		#region Constructor

		public CustomComparer(Comparison<object> comparison)
		{
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			_comparison = comparison;
		}

		public CustomComparer(IComparer comparer)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			_comparison = comparer.Compare;
		}

		#endregion
		#region Methods

		public int Compare(object first, object second)
		{
			return _comparison(first, second);
		}

		#endregion
	}
}
