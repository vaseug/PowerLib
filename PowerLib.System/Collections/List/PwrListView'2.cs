using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerLib.System.Collections
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Type of store item.</typeparam>
	/// <typeparam name="V">Type of view item.</typeparam>
	public class PwrListView<T, V> : PwrListViewBase<T, V>
	{
		private Func<T, V> _selector;

		#region Constructors

		public PwrListView(IList<T> list, Func<T, V> selector)
			: base(list)
		{
			if (selector == null)
				throw new ArgumentNullException("selector");

			_selector = selector;
		}

		#endregion
		#region Methods
		#region Internal methods
 
		protected override V GetItem(T value)
		{
			return _selector(value);
		}

		#endregion
		#endregion
	}
}
