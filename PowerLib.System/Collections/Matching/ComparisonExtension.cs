using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PowerLib.System.Collections.Matching
{
	public static class ComparisonExtension
	{
		public static Comparison<T> Comparison<T>(this IComparer<T> comparer)
		{
			if (comparer == null)
				throw new NullReferenceException();

			return comparer.Compare;
		}

		public static Comparison<object> Comparison(this IComparer comparer)
		{
			if (comparer == null)
				throw new NullReferenceException();

			return comparer.Compare;
		}

		public static IEnumerable<Comparison<T>> Comparisons<T>(this IEnumerable<IComparer<T>> comparers)
		{
			if (comparers == null)
				throw new NullReferenceException();

			return comparers.Select<IComparer<T>, Comparison<T>>(c => c.Compare);
		}

		public static IEnumerable<Comparison<object>> Comparisons(this IEnumerable<IComparer> comparers)
		{
			if (comparers == null)
				throw new NullReferenceException();

			return comparers.Select<IComparer, Comparison<object>>(c => c.Compare);
		}
	}
}
