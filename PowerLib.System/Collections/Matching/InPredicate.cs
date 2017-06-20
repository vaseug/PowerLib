using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerLib.System.Collections.Matching
{
	public sealed class InPredicate<T> : IPredicate<T>
	{
		private Equality<T> _equality;
		private IEnumerable<T> _collection;

		#region Constructors

		public InPredicate(Equality<T> equality, IEnumerable<T> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");
			if (equality == null)
				throw new ArgumentNullException("equality");

			_collection = collection;
			_equality = equality;
		}

		public InPredicate(IEqualityComparer<T> equalityComparer, IEnumerable<T> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			_collection = collection;
			_equality = equalityComparer.Equals;
		}

		#endregion
		#region Methods

		public bool Match(T value)
		{
			return _collection.Any(v => _equality(v, value));
		}

		#endregion
	}
}
