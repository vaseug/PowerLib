using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class ObjectEqualityComparer<T> : IEqualityComparer<T>
		where T : class
	{
		private Equality<T> _equality;
		private bool _nullInequal;
    private static Lazy<ObjectEqualityComparer<T>> @default = new Lazy<ObjectEqualityComparer<T>>(() => new ObjectEqualityComparer<T>(EqualityComparer<T>.Default, false));

		#region Constructors

		public ObjectEqualityComparer(Equality<T> equality, bool nullInequal)
		{
			if (equality == null)
				throw new ArgumentNullException("equality");

			_equality = equality;
			_nullInequal = nullInequal;
		}

		public ObjectEqualityComparer(IEqualityComparer<T> equalityComparer, bool nullInequal)
		{
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			_equality = equalityComparer.Equals;
			_nullInequal = nullInequal;
		}

    #endregion
    #region Static properties

    public static IEqualityComparer<T> Default
    {
      get { return @default.Value; }
    }

    #endregion
    #region Methods

    public bool Equals(T x, T y)
		{
			return x == null && y == null && !_nullInequal || x != null && y != null && _equality(x, y);
		}

		public int GetHashCode(T v)
		{
			return v == null ? 0 : v.GetHashCode();
		}

		#endregion
	}
}
