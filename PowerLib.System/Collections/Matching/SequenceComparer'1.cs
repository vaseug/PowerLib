using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections.Matching
{
	public sealed class SequenceComparer<T> : IComparer<IEnumerable<T>>
	{
		private IComparer<T> _сomparer;

		#region Constructors

		public SequenceComparer(IComparer<T> сomparer)
		{
			if (сomparer == null)
				throw new ArgumentNullException("сomparer");

			_сomparer = сomparer;
		}

    #endregion
    #region Methods

    public int Compare(IEnumerable<T> first, IEnumerable<T> second)
		{
			if (first == null)
				throw new ArgumentNullException("first");
			if (second == null)
				throw new ArgumentNullException("second");

			return first.SequenceCompare(second);
		}

		#endregion
	}
}
