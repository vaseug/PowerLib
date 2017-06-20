using System;
using System.Collections.Generic;
using System.Linq;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections.Matching
{
	public sealed class CompositeComparer<T> : IComparer<T>
	{
		private Comparison<T>[] _comparisons;

		#region Constructors

		public CompositeComparer(IEnumerable<Comparison<T>> comparisons)
		{
			if (comparisons == null)
				throw new ArgumentNullException("comparisons");
			Comparison<T>[] carray = comparisons.Apply(c =>
				{
					if (c == null)
						throw new ArgumentNullException();
				}).ToArray();
			if (carray.Length == 0)
				throw new ArgumentException("Empty comparisons", "comparisons");

			_comparisons = carray;
		}

		public CompositeComparer(IEnumerable<IComparer<T>> comparers)
		{
			if (comparers == null)
				throw new ArgumentNullException("comparers");
			Comparison<T>[] carray = comparers.Apply(c =>
			{
				if (c == null)
					throw new ArgumentNullException();
			}).Select<IComparer<T>, Comparison<T>>(c => c.Compare).ToArray();
			if (carray.Length == 0)
				throw new ArgumentException("Empty comparers", "comparers");

			_comparisons = carray;
		}

		public CompositeComparer(params Comparison<T>[] comparisons)
			: this((IEnumerable<Comparison<T>>)comparisons)
		{
		}

		public CompositeComparer(params IComparer<T>[] comparers)
			: this((IEnumerable<IComparer<T>>)comparers)
		{
		}

		#endregion
		#region Methods

		public int Compare(T first, T second)
		{
			int result = 0;
			for (int i = 0; result == 0 && i < _comparisons.Length; i++)
				result = _comparisons[i](first, second);
			return result;
		}

		#endregion
	}
}
