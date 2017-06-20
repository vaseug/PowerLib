using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerLib.System.Collections
{
	public class PwrEnumerableViewBase<T> : IEnumerable<T>, IEnumerable
	{
		private IEnumerable<T> _innerStore;

		#region Constructors

		protected PwrEnumerableViewBase(IEnumerable<T> coll)
		{
			if (coll == null)
				throw new ArgumentNullException("coll");

			_innerStore = coll;
		}

		#endregion
		#region Properties
		#region Internal properties

		protected IEnumerable<T> InnerStore
		{
			get
			{
				return _innerStore;
			}
		}

		#endregion
		#endregion
		#region Internal methods

		protected virtual bool PredicateItem(T item, int index)
		{
			return true;
		}

		protected virtual IEnumerator<T> GetItemEnumerator()
		{
			using (IEnumerator<T> e = InnerStore.GetEnumerator())
				for (int i = 0; e.MoveNext(); i++)
					if (PredicateItem(e.Current, i))
						yield return e.Current;
		}

		#endregion
		#region Public methods

		public IEnumerator<T> GetEnumerator()
		{
			return GetItemEnumerator();
		}

		#endregion
		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetItemEnumerator();
		}

		#endregion
	}
}
