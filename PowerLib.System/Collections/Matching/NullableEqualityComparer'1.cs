using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class NullableEqualityComparer<T> : IEqualityComparer<T?>
		where T : struct
	{
		private Equality<T> _equality;
		private bool _nullInequal;
    private static Lazy<NullableEqualityComparer<T>> @default = new Lazy<NullableEqualityComparer<T>>(() => new NullableEqualityComparer<T>(EqualityComparer<T>.Default, false));

		#region Constructors

		public NullableEqualityComparer(Equality<T> equality, bool nullInequal)
		{
			if (equality == null)
				throw new ArgumentNullException("equality");

			_equality = equality;
			_nullInequal = nullInequal;
		}

		public NullableEqualityComparer(IEqualityComparer<T> equalityComparer, bool nullInequal)
		{
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			_equality = equalityComparer.Equals;
			_nullInequal = nullInequal;
		}

    #endregion
    #region Static properties

    public static IEqualityComparer<T?> Default
    {
      get { return @default.Value; }
    }

    #endregion
    #region Methods

    public bool Equals(T? xvalue, T? yvalue)
		{
			return !xvalue.HasValue && !yvalue.HasValue && !_nullInequal || xvalue.HasValue && yvalue.HasValue&& _equality(xvalue.Value, yvalue.Value);
		}

		public int GetHashCode(T? value)
		{
			return !value.HasValue? 0 : value.Value.GetHashCode();
		}

		#endregion
	}
}
