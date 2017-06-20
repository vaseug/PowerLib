using System;
using System.Collections;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	public class PwrReadOnlyCollection<T> : PwrReadOnlyCollectionBase<T>
	{
		private Func<T, int, bool> _predicate;
		private Func<ICollection<T>, int> _counter;

		#region Constructors

		public PwrReadOnlyCollection(ICollection<T> collection)
			: this(collection, (t, i) => true, coll => coll.Count)
		{
		}

		public PwrReadOnlyCollection(ICollection<T> collection, Func<T, int, bool> predicate, Func<ICollection<T>, int> counter)
			: base(collection)
		{
			_predicate = predicate;
			_counter = counter;
		}

		#endregion
		#region Properties

		protected override int ItemsCount
		{
			get
			{
				return _counter(InnerStore);
			}
		}

		#endregion
		#region Methods
		#region Internal methods

		protected override bool PredicateItem(T value, int index)
		{
			return _predicate(value, index);
		}

		#endregion
		#endregion
	}
}
