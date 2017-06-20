using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class QuantifyPredicate<T> : IPredicate<IEnumerable<T>>
	{
		private Func<T, bool> _predicate;
		private QuantifyCriteria _criteria;

		#region Constructors

		public QuantifyPredicate(Func<T, bool> predicate, QuantifyCriteria criteria)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			_predicate = predicate;
			_criteria = criteria;
		}

		public QuantifyPredicate(IPredicate<T> predicate, QuantifyCriteria criteria)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			_predicate = predicate.Match;
			_criteria = criteria;
		}

    #endregion
    #region Methods

    public bool Match(IEnumerable<T> coll)
		{
			if (coll == null)
				throw new ArgumentNullException();

			using (IEnumerator<T> e = coll.GetEnumerator())
			{
				bool result = _criteria == QuantifyCriteria.All;
				while ((!result && _criteria == QuantifyCriteria.Any || result && _criteria == QuantifyCriteria.All) && e.MoveNext())
					result = _predicate(e.Current);
				return result;
			}
		}

		#endregion
	}
}
