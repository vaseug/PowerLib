using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	/// <summary>
	/// EqualityComparewrappedelegate
	/// </summary>
	/// <typeparam name="T">Comparing type</typeparam>
	public sealed class CustomEqualityComparer<T> : IEqualityComparer<T>
	{
		private Equality<T> _equality;
		private Func<T, int> _hashing;

		#region Constructors

		public CustomEqualityComparer(Equality<T> equality)
			: this(equality, t => t.GetHashCode())
		{
		}

		public CustomEqualityComparer(Equality<T> equality, Func<T, int> hashing)
		{
			if (equality == null)
				throw new ArgumentNullException("equality");
			if (hashing == null)
				throw new ArgumentNullException("hashing");

			_equality = equality;
			_hashing = hashing;
		}

		public CustomEqualityComparer(IEqualityComparer<T> equalityComparer)
		{
			_equality = equalityComparer.Equals;
			_hashing = equalityComparer.GetHashCode;
		}

		#endregion
		#region Methods

		public bool Equals(T first, T second)
		{
			return _equality(first, second);
		}

		public int GetHashCode(T value)
		{
			return _hashing(value);
		}

		#endregion
	}
}
