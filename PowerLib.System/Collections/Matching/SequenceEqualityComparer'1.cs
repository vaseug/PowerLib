using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerLib.System.Collections.Matching
{
	public sealed class SequenceEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
	{
		private IEqualityComparer<T> _equalityComparer;
		private Func<int, int, int> _hashComposer;
		private int _hashSeed;

		#region Constructors

		public SequenceEqualityComparer(IEqualityComparer<T> equalityComparer)
			: this(equalityComparer, (a, c) => unchecked(a * 31 + c), 23)
		{
		}

		public SequenceEqualityComparer(IEqualityComparer<T> equalityComparer, Func<int, int, int> hashComposer, int hashSeed)
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

		public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
		{
			if (x == null)
				throw new ArgumentNullException("x");
			if (y == null)
				throw new ArgumentNullException("y");

			return x.SequenceEqual(y);
		}

		public int GetHashCode(IEnumerable<T> seq)
		{
			if (seq == null)
				throw new ArgumentNullException("seq");

			return seq
				.Select(t => t == null ? 0 : t.GetHashCode())
				.Aggregate(_hashSeed, (accumHash, itemHash) => _hashComposer(accumHash, itemHash));
		}

		#endregion
	}
}
