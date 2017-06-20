using System;
using System.Collections;

namespace PowerLib.System.Collections.Matching
{
	public sealed class ObjectEqualityComparer : IEqualityComparer
	{
		private Equality<object> _equality;
		private bool _nullInequal;

		#region Constructors

		public ObjectEqualityComparer(Equality<object> equality, bool nullInequal)
		{
			if (equality == null)
				throw new ArgumentNullException("equality");

			_equality = equality;
			_nullInequal = nullInequal;
		}

		public ObjectEqualityComparer(IEqualityComparer equalityComparer, bool nullInequal)
		{
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			_equality = equalityComparer.Equals;
			_nullInequal = nullInequal;
		}

    #endregion
    #region Methods

    public new bool Equals(object first, object second)
		{
			return first == null && second == null && !_nullInequal || first != null && second != null && _equality(first, second);
		}

		public int GetHashCode(object value)
		{
			return value == null ? 0 : value.GetHashCode();
		}

		#endregion
	}
}
