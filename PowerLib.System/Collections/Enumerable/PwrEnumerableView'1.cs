using System;
using System.Collections;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	public class PwrEnumerableView<T> : PwrEnumerableViewBase<T>
	{
		private Func<T, int, bool> _predicate;

		#region Constructors

		public PwrEnumerableView(IEnumerable<T> coll)
			: this(coll, null)
		{
		}

		public PwrEnumerableView(IEnumerable<T> coll, Func<T, int, bool> predicate)
			: base(coll)
		{
			_predicate = predicate;
		}

		#endregion
		#region Internal methods

		protected override bool PredicateItem(T value, int index)
		{
			return _predicate == null || _predicate(value, index);
		}

		#endregion
	}
}
