﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerLib.System.Collections
{
	public class PwrReadOnlyCollection<T, V> : PwrReadOnlyCollectionBase<T, V>
	{
		private Func<T, V> _selector;

		#region Constructors

		public PwrReadOnlyCollection(ICollection<T> collection, Func<T, V> selector)
			: base(collection)
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
