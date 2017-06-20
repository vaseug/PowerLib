using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class EqualityPredicate<T> : IPredicate<T>
	{
		private Equality<T> _equality;
		private T _value ;

		#region Constructors

		public EqualityPredicate(Equality<T> equality, T value)
		{
			if (equality == null)
				throw new ArgumentNullException("equality");

			_value = value ;
			_equality = equality;
		}

		public EqualityPredicate(IEqualityComparer<T> equalityComparer, T value)
		{
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			_value = value ;
			_equality = equalityComparer.Equals;
		}

		#endregion
		#region Methods

		public bool Match(T value)
		{
			return _equality(_value , value);
		}

		#endregion
	}
}
