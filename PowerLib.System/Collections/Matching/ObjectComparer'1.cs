using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class ObjectComparer<T> : IComparer<T>
		where T : class
	{
		private Comparison<T> _comparison;
		private bool _nullOrder;
    private static Lazy<ObjectComparer<T>> @default = new Lazy<ObjectComparer<T>>(() => new ObjectComparer<T>(Comparer<T>.Default, false));

    #region Constructors

    public ObjectComparer(Comparison<T> comparison, bool nullOrder)
		{
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			_comparison = comparison;
			_nullOrder = nullOrder;
		}

		public ObjectComparer(IComparer<T> comparer, bool nullOrder)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			_comparison = comparer.Compare;
			_nullOrder = nullOrder;
		}

    #endregion
    #region Static properties

    public static IComparer<T> Default
    {
      get { return @default.Value; }
    }

    #endregion
    #region Methods

    public int Compare(T first, T second)
		{
			return first != null ? second != null ? _comparison(first, second) : _nullOrder ? -1 : 1 : second != null ? _nullOrder ? 1 : -1 : 0;
		}

		#endregion
	}
}
