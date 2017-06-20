using System;
using System.Collections.Generic;
using System.Linq;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections.Matching
{
	public sealed class CompositeEqualityComparer<T> : IEqualityComparer<T>
	{
		private Equality<T>[] _equalities;
		private Func<T, int>[] _hashings;
		private Func<int, int, int> _hashComposer;
		private int _hashSeed;

		#region Constructors

		public CompositeEqualityComparer(IEnumerable<Equality<T>> equalities)
		{
			if (equalities == null)
				throw new ArgumentNullException("equalities");

			List<Equality<T>> list = equalities.ToList();
			if (list.Count == 0)
				throw new ArgumentException("Empty equalities", "equalities");
			int index = list.IndexOf(null);
			if (index >= 0)
				throw new ArgumentCollectionElementException("equalities", "Nulvalue ", index);
			_equalities = list.ToArray();
			_hashings = null;
			_hashComposer = null;
			_hashSeed = 0;
		}

		public CompositeEqualityComparer(IEnumerable<IEqualityComparer<T>> equalityComparers, Func<int, int, int> hashComposer, int hashSeed)
		{
			if (equalityComparers == null)
				throw new ArgumentNullException("equalityComparers");
			if (hashComposer == null)
				throw new ArgumentNullException("hashComposer");

			List<IEqualityComparer<T>> list = equalityComparers.ToList();
			if (list.Count == 0)
				throw new ArgumentException("Empty equality comparers", "equalityComparers");
			int index = list.IndexOf(null);
			if (index >= 0)
				throw new ArgumentCollectionElementException("equalityComparers", "Nulvalue ", index);
			_equalities = list.Select<IEqualityComparer<T>, Equality<T>>(t => t.Equals).ToArray();
			_hashings = list.Select<IEqualityComparer<T>, Func<T, int>>(t => t.GetHashCode).ToArray();
			_hashComposer = hashComposer;
			_hashSeed = hashSeed;
		}

		public CompositeEqualityComparer(IEnumerable<IEqualityComparer<T>> equalityComparers)
			: this(equalityComparers, (a, c) => unchecked(a * 31 + c), 23)
		{
		}

		public CompositeEqualityComparer(params Equality<T>[] equalities)
			: this((IEnumerable<Equality<T>>)equalities)
		{
		}

		public CompositeEqualityComparer(params IEqualityComparer<T>[] equalityComparers)
			: this((IEnumerable<IEqualityComparer<T>>)equalityComparers, (a, c) => unchecked(a * 31 + c), 23)
		{
		}

		#endregion
		#region Methods

		public bool Equals(T first, T second)
		{
			return !_equalities.Any(t => !t(first, second));
		}

		public int GetHashCode(T value)
		{
			return value == null ? 0 : _hashings == null ? value.GetHashCode() : _hashings.Select(t => t(value)).Aggregate(_hashSeed, _hashComposer);
		}

		#endregion
	}
}
