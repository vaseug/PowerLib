using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections.Matching
{
	public sealed class CompositeComparer : IComparer
	{
		private Comparison<object>[] _comparisons;

		#region Constructors

		public CompositeComparer(IEnumerable<Comparison<object>> comparisons)
		{
			if (comparisons == null)
				throw new ArgumentNullException("comparisons");
			Comparison<object>[] carray = comparisons.Apply(c =>
				{
					if (c == null)
						throw new ArgumentNullException();
				}).ToArray();
			if (carray.Length == 0)
				throw new ArgumentException("Empty comparisons", "comparisons");

			_comparisons = carray;
		}

		public CompositeComparer(IEnumerable<IComparer> comparers)
		{
			if (comparers == null)
				throw new ArgumentNullException("comparers");
			Comparison<object>[] comparisons = comparers.Apply(c =>
			{
				if (c == null)
					throw new ArgumentNullException();
			}).Select<IComparer, Comparison<object>>(c => c.Compare).ToArray();
			if (comparisons.Length == 0)
				throw new ArgumentException("Empty comparers", "comparers");

			_comparisons = comparisons;
		}

		public CompositeComparer(params Comparison<object>[] comparisons)
			: this((IEnumerable<Comparison<object>>)comparisons)
		{
		}

		public CompositeComparer(params IComparer[] comparers)
			: this((IEnumerable<IComparer>)comparers)
		{
		}

		#endregion
		#region Methods

		public int Compare(object first, object second)
		{
			int result = 0;
			for (int i = 0; result == 0 && i < _comparisons.Length; i++)
				result = _comparisons[i](first, second);
			return result;
		}

		#endregion
	}
}
