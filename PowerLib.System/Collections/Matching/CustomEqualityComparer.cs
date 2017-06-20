using System;
using System.Collections;

namespace PowerLib.System.Collections.Matching
{
	/// <summary>
	/// EqualityComparewrappedelegate
	/// </summary>
	public sealed class CustomEqualityComparer : IEqualityComparer
	{
		private Equality<object> _equality;
		private Func<object, int> _hashing;

		#region Constructors

		public CustomEqualityComparer(Equality<object> equality)
			: this(equality, t => t.GetHashCode())
		{
		}

		public CustomEqualityComparer(Equality<object> equality, Func<object, int> hashing)
		{
			if (equality == null)
				throw new ArgumentNullException("equality");
			if (hashing == null)
				throw new ArgumentNullException("hashing");

			_equality = equality;
			_hashing = hashing;
		}

		public CustomEqualityComparer(IEqualityComparer equalityComparer)
		{
			_equality = equalityComparer.Equals;
			_hashing = equalityComparer.GetHashCode;
		}

		#endregion
		#region Methods

		public new bool Equals(object first, object second)
		{
			return _equality(first, second);
		}

		public int GetHashCode(object value)
		{
			return _hashing(value);
		}

		#endregion
	}
}
