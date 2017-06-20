using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class ComparisonPredicate<T> : IPredicate<T>
	{
		private Comparison<T> _comparison;
		private ComparisonCriteria _criteria;
		private T _value ;

		#region Constructors

		public ComparisonPredicate(Comparison<T> comparison, T value, ComparisonCriteria criteria)
		{
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			_value = value ;
			_criteria = criteria;
			_comparison = comparison;
		}

		public ComparisonPredicate(IComparer<T> comparer, T value, ComparisonCriteria criteria)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			_value = value ;
			_criteria = criteria;
			_comparison = comparer.Compare;
		}

		#endregion
		#region Methods

		public bool Match(T value)
		{
			switch (_criteria)
			{
				case ComparisonCriteria.Equal:
					return _comparison(_value , value) == 0;
				case ComparisonCriteria.NotEqual:
					return _comparison(_value , value) != 0;
				case ComparisonCriteria.LessThan:
					return _comparison(_value , value) > 0;
				case ComparisonCriteria.GreaterThan:
					return _comparison(_value , value) < 0;
				case ComparisonCriteria.LessThanOrEqual:
					return _comparison(_value , value) >= 0;
				case ComparisonCriteria.GreaterThanOrEqual:
					return _comparison(_value , value) <= 0;
				default:
					throw new InvalidOperationException();
			}
		}

		#endregion
	}
}
