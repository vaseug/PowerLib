using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PowerLib.System.Collections
{
	public class PwrEnumerableView<T, V> : PwrEnumerableViewBase<T, V>
	{
		private Func<T, V> _selector;

		#region Constructors

		public PwrEnumerableView(IEnumerable<T> coll, Func<T, V> selector)
			: base(coll)
		{
			if (selector == null)
				throw new ArgumentNullException("selector");

			_selector = selector;
		}

		#endregion
		#region Internal methods

		protected override V GetItem(T value)
		{
			return _selector(value);
		}

		#endregion
	}
}
