using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerLib.System.Collections
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Typof storitem.</typeparam>
	/// <typeparam name="V">Typof view item.</typeparam>
	public class PwrReadOnlyList<T, V> : PwrReadOnlyListBase<T, V>
	{
		private Func<T, V> _selector;

		#region Constructors

		public PwrReadOnlyList(IList<T> list, Func<T, V> selector)
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
