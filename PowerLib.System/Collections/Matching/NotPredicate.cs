using System;

namespace PowerLib.System.Collections.Matching
{
	public sealed class NotPredicate<T> : IPredicate<T>
	{
		private Func<T, bool> _predicate;

		#region Constructors

		public NotPredicate(Func<T, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException();

			_predicate = predicate;
		}

		public NotPredicate(IPredicate<T> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException();

			_predicate = predicate.Match;
		}

		#endregion
		#region Methods

		public bool Match(T value)
		{
			return !_predicate(value);
		}

		#endregion
	}
}
