using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class BetweenPredicate<T> : IPredicate<T>
	{
		private Comparison<T> _comparison;
		private BetweenCriteria _criteria;
		private T _lowerBound;
		private T _upperBound;

		#region Constructors

		public BetweenPredicate(Comparison<T> comparison, T lowerBound, T upperBound)
			: this(comparison, lowerBound, upperBound, BetweenCriteria.IncludeBoth)
		{
		}

		public BetweenPredicate(Comparison<T> comparison, T lowerBound, T upperBound, BetweenCriteria excludedBounds)
		{
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			_lowerBound = lowerBound;
			_upperBound = upperBound;
			_criteria = excludedBounds;
			_comparison = comparison;
		}

		public BetweenPredicate(IComparer<T> comparer, T lowerBound, T upperBound)
			: this(comparer, lowerBound, upperBound, BetweenCriteria.IncludeBoth)
		{
		}

		public BetweenPredicate(IComparer<T> comparer, T lowerBound, T upperBound, BetweenCriteria excludedBounds)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			_lowerBound = lowerBound;
			_upperBound = upperBound;
			_criteria = excludedBounds;
			_comparison = comparer.Compare;
		}

		#endregion
		#region Methods

		public bool Match(T value)
		{
			switch (_criteria)
			{
				case BetweenCriteria.IncludeBoth:
					return _comparison(value, _lowerBound) >= 0 && _comparison(value, _upperBound) <= 0;
				case BetweenCriteria.ExcludeLower:
					return _comparison(value, _lowerBound) > 0 && _comparison(value, _upperBound) <= 0;
				case BetweenCriteria.ExcludeUpper:
					return _comparison(value, _lowerBound) >= 0 && _comparison(value, _upperBound) < 0;
				case BetweenCriteria.ExcludeBoth:
					return _comparison(value, _lowerBound) > 0 && _comparison(value, _upperBound) < 0;
				default:
					throw new InvalidOperationException();
			}
		}

		#endregion
	}
}
