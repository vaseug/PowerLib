using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class CustomPredicate<T> : IPredicate<T>
	{
		private Func<T, bool> _predicate;

		#region Constructors

		public CustomPredicate(Func<T, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			_predicate = predicate;
		}

		public CustomPredicate(IPredicate<T> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			_predicate = predicate.Match;
		}

		#endregion
		#region Methods

		public bool Match(T value)
		{
			return _predicate(value);
		}

		#endregion
	}
}
