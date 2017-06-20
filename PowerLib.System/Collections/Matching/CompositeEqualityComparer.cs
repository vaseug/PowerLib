using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections.Matching
{
	public sealed class CompositeEqualityComparer : IEqualityComparer
	{
		private Equality<object>[] _equalities;

		#region Constructors

		public CompositeEqualityComparer(IEnumerable<Equality<object>> equalities)
		{
			if (equalities == null)
				throw new ArgumentNullException("equalities");
			Equality<object>[] earray = equalities.Apply(c =>
			{
				if (c == null)
					throw new ArgumentNullException();
			}).ToArray();
			if (earray.Length == 0)
				throw new ArgumentException("Empty equalities", "equalities");

			_equalities = earray;
		}

		public CompositeEqualityComparer(IEnumerable<IEqualityComparer> equalityComparers)
		{
			if (equalityComparers == null)
				throw new ArgumentNullException("equalityComparers");
			Equality<object>[] earray = equalityComparers.Apply(c =>
			{
				if (c == null)
					throw new ArgumentNullException();
			}).Select<IEqualityComparer, Equality<object>>(c => c.Equals).ToArray();
			if (earray.Length == 0)
				throw new ArgumentException("Empty equality comparers", "equalityComparers");

			_equalities = earray;
		}

		public CompositeEqualityComparer(params Equality<object>[] equalities)
			: this((IEnumerable<Equality<object>>)equalities)
		{
		}

		public CompositeEqualityComparer(params IEqualityComparer[] equalityComparers)
			: this((IEnumerable<IEqualityComparer>)equalityComparers)
		{
		}

		#endregion
		#region Methods

		public new bool Equals(object first, object second)
		{
			bool result = true;
			for (int i = 0; result && i < _equalities.Length; i++)
				result = _equalities[i](first, second);
			return result;
		}

		public int GetHashCode(object value)
		{
			return value == null ? 0 : value.GetHashCode();
		}

		#endregion
	}
}
