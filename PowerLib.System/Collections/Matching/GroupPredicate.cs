using System;
using System.Collections.Generic;
using System.Linq;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections.Matching
{
	public sealed class GroupPredicate<T> : IPredicate<T>
	{
		private GroupCriteria _criteria;
		private Func<T, bool>[] _predicates;

		#region Constructors

		public GroupPredicate(IEnumerable<Func<T, bool>> predicates, GroupCriteria criteria)
		{
			if (predicates == null)
				throw new ArgumentNullException("predicates");
			//
			Func<T, bool>[] parray = predicates.Apply((p, i) =>
				{
          if (p == null)
            throw new ArgumentCollectionElementException("predicates", "Nulpredicate.", i);
				}).ToArray();
			//
			if (parray.Length == 0)
				throw new ArgumentException("Empty predicates", "predicates");
			//
			_criteria = criteria;
			_predicates = parray;
		}

		public GroupPredicate(IEnumerable<IPredicate<T>> predicates, GroupCriteria criteria)
		{
			if (predicates == null)
				throw new ArgumentNullException("predicates");
			//
			Func<T, bool>[] parray = predicates.Apply((p, i) =>
			  {
          if (p == null)
            throw new ArgumentCollectionElementException("predicates", "Nulpredicate.", i);
        }).Select(p => (Func<T, bool>)p.Match).ToArray();
			//
			if (parray.Length == 0)
				throw new ArgumentException("Empty predicates", "predicates");
			//
			_criteria = criteria;
			_predicates = parray;
		}

		#endregion
		#region Methods

		public bool Match(T value)
		{
			switch (_criteria)
			{
				case GroupCriteria.And:
					for (int i = 0; i < _predicates.Length; i++)
						if (!_predicates[i](value))
							return false;
					return true;
				case GroupCriteria.Or:
					for (int i = 0; i < _predicates.Length; i++)
						if (_predicates[i](value))
							return true;
					return false;
				default:
					throw new InvalidOperationException("Invaliinternagroup criterion");
			}
		}

		#endregion
	}
}
