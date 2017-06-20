using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerLib.System.Collections
{
	public abstract class PwrEnumerableViewBase<S, T> : IEnumerable<T>, IEnumerable
	{
		private IEnumerable<S> _innerStore;

		#region Constructors

		protected PwrEnumerableViewBase(IEnumerable<S> coll)
		{
			if (coll == null)
				throw new ArgumentNullException("coll");

			_innerStore = coll;
		}

		#endregion
		#region Properties
		#region Internal properties

		protected IEnumerable<S> InnerStore
		{
			get
			{
				return _innerStore;
			}
		}

		#endregion
		#endregion
		#region Methods
		#region Internal methods

		protected abstract T GetItem(S item);

		#endregion
		#region Public methods

		public IEnumerator<T> GetEnumerator()
		{
			using (IEnumerator<S> e = InnerStore.GetEnumerator())
				while (e.MoveNext())
					yield return GetItem(e.Current);
		}

		#endregion
		#endregion
		#region Interfaces
		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
		#endregion
	}
}
