using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerLib.System.Collections.Matching
{
	public sealed class RegularArrayEqualityComparer<T> : IEqualityComparer<Array>
	{
		private IEqualityComparer<T> _equalityComparer;
		private Func<int, int, int> _hashComposer;
		private int _hashSeed;

		#region Constructors

		public RegularArrayEqualityComparer(IEqualityComparer<T> equalityComparer)
			: this(equalityComparer, (a, c) => unchecked(a * 31 + c), 23)
		{
		}

		public RegularArrayEqualityComparer(IEqualityComparer<T> equalityComparer, Func<int, int, int> hashComposer, int hashSeed)
		{
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");
			if (hashComposer == null)
				throw new ArgumentNullException("hashComposer");

			_equalityComparer = equalityComparer;
			_hashComposer = hashComposer;
			_hashSeed = hashSeed;
		}

    #endregion
    #region Methods

    public bool Equals(Array first, Array second)
		{
			if (first == null)
				throw new ArgumentNullException("first");
			if (second == null)
				throw new ArgumentNullException("second");
			if (!typeof(T).IsAssignableFrom(first.GetType().GetElementType()))
				throw new ArgumentException("Invalid array element type", "first");
			if (!typeof(T).IsAssignableFrom(second.GetType().GetElementType()))
				throw new ArgumentException("Invalid array element type", "second");

			return first.LongLength == second.LongLength && first.Rank == second.Rank &&
				first.GetRegularArrayLongLengths().SequenceEqual(second.GetRegularArrayLongLengths()) && first.EnumerateAsRegular<T>().SequenceEqual(second.EnumerateAsRegular<T>());
		}

		public int GetHashCode(Array array)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (!typeof(T).IsAssignableFrom(array.GetType().GetElementType()))
				throw new ArgumentException("Invalid array element type", "array");

			return array.EnumerateAsRegular<T>()
				.Select(t => t == null ? 0 : t.GetHashCode())
				.Aggregate(_hashSeed, (accumHash, itemHash) => _hashComposer(accumHash, itemHash));
		}

		#endregion
	}
}
