using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public static class EqualityExtension
	{
		public static Equality<T> Equality<T>(this IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
				throw new NullReferenceException();

			return equalityComparer.Equals;
		}

		public static Equality<object> Equality(this IEqualityComparer equalityComparer)
		{
			if (equalityComparer == null)
				throw new NullReferenceException();

			return equalityComparer.Equals;
		}

		public static Func<T, int> Hasher<T>(this IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
				throw new NullReferenceException();

			return equalityComparer.GetHashCode;
		}

		public static Func<object, int> Hasher(this IEqualityComparer equalityComparer)
		{
			if (equalityComparer == null)
				throw new NullReferenceException();

			return equalityComparer.GetHashCode;
		}
	}
}
