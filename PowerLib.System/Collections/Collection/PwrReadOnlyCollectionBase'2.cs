using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PowerLib.System.Collections
{
	public abstract class PwrReadOnlyCollectionBase<S, T> : PwrEnumerableViewBase<S, T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
	{
		#region Constructors

		public PwrReadOnlyCollectionBase(ICollection<S> coll)
			: base(coll)
		{
		}

		#endregion
		#region Properties
		#region Internal properties

		new protected ICollection<S> InnerStore
		{
			get
			{
				return (ICollection<S>)base.InnerStore;
			}
		}

		#endregion
		#region Public properties

		public int Count
		{
			get
			{
				return InnerStore.Count;
			}
		}

		#endregion
		#endregion
		#region Methods
		#region Innemethods

		protected virtual bool ContainsItem(T value)
		{
			using (IEnumerator<S> e = InnerStore.GetEnumerator())
				while (e.MoveNext())
					if (Equals(GetItem(e.Current), value))
						return true;
			return false;
		}

		protected virtual void CopyItemsTo(T[] array, int arrayIndex)
		{
			using (IEnumerator<S> e = InnerStore.GetEnumerator())
			{
				while (e.MoveNext() && arrayIndex < array.Length)
					array[arrayIndex++] = GetItem(e.Current);
			}
		}

		#endregion
		#region Public methods

		public bool Contains(T value)
		{
			return ContainsItem(value);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (arrayIndex < 0 || arrayIndex > array.Length)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (array.Length - arrayIndex < InnerStore.Count)
				throw new ArgumentException("Noenough spactstore.", "array");

			CopyItemsTo(array, arrayIndex);
		}

		#endregion
		#endregion
	}
}
