using System;
using System.Collections;

namespace PowerLib.System.Collections.Matching
{
	public sealed class ObjectComparer : IComparer
	{
		private Comparison<object> _comparison;
		private bool _nullOrder;

		#region Constructors

		public ObjectComparer(Comparison<object> comparison, bool nullOrder)
		{
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			_comparison = comparison;
			_nullOrder = nullOrder;
		}

		public ObjectComparer(IComparer comparer, bool nullOrder)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			_comparison = comparer.Compare;
			_nullOrder = nullOrder;
		}

    #endregion
    #region Methods

    public int Compare(object first, object second)
		{
			return first != null ? second != null ? _comparison(first, second) : _nullOrder ? -1 : 1 : second != null ? _nullOrder ? 1 : -1 : 0;
		}

		#endregion
	}
}
