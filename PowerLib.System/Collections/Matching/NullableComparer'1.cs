using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class NullableComparer<T> : IComparer<T?>
		where T : struct
	{
		private Comparison<T> _comparison;
		private bool _nullOrder;
    private static Lazy<NullableComparer<T>> @default = new Lazy<NullableComparer<T>>(() => new NullableComparer<T>(Comparer<T>.Default, false));

    #region Constructors

    public NullableComparer(Comparison<T> comparison, bool nullOrder)
		{
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			_comparison = comparison;
			_nullOrder = nullOrder;
		}

		public NullableComparer(IComparer<T> comparer, bool nullOrder)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			_comparison = comparer.Compare;
			_nullOrder = nullOrder;
		}

    #endregion
    #region Static properties

    public static IComparer<T?> Default
    {
      get { return @default.Value; }
    }

    #endregion
    #region Methods

    public int Compare(T? first, T? second)
		{
			return first.HasValue? second.HasValue? _comparison(first.Value, second.Value) : _nullOrder ? -1 : 1 : first.HasValue? _nullOrder ? 1 : -1 : 0;
		}

		#endregion
	}
}
