using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class InverseComparer : IComparer
	{
		private Comparison<object> _comparison;

		#region Constructors

		public InverseComparer(Comparison<object> comparison)
		{
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			_comparison = comparison;
		}

		public InverseComparer(IComparer comparer)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			_comparison = comparer.Compare;
		}

		#endregion
		#region Methods

		public int Compare(object first, object second)
		{
			return _comparison(second, first);
		}

		#endregion
	}
}
