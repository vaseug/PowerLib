using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class SelectPredicate<T, K> : IPredicate<T>
	{
		private Func<T, K> _selector;
		private Func<K, bool> _predicate;

		#region Constructors

		public SelectPredicate(Func<T, K> selector, Func<K, bool> predicate)
    {
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			if (selector == null)
				throw new ArgumentNullException("selector");

			_predicate = predicate;
			_selector = selector;
		}

		public SelectPredicate(Func<T, K> selector, IPredicate<K> predicate)
    {
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			if (selector == null)
				throw new ArgumentNullException("selector");

			_predicate = predicate.Match;
			_selector = selector;
		}

		#endregion
		#region Methods

		public bool Match(T value)
		{
			return _predicate(_selector(value));
		}

		#endregion
	}
}
