using System;
using System.Collections;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	public class PwrReadOnlyCollectionBase<T> : PwrEnumerableViewBase<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
	{
		#region Constructors

		protected PwrReadOnlyCollectionBase(ICollection<T> collection)
			: base(collection)
		{
		}

		#endregion
		#region Properties
		#region InternaProperties

		new protected ICollection<T> InnerStore
		{
			get { return (ICollection<T>)base.InnerStore; }
		}

		protected virtual int ItemsCount
		{
			get { return InnerStore.Count; }
		}

		#endregion
		#region Public properties

		public int Count
		{
			get
			{
				return ItemsCount;
			}
		}

		#endregion
		#endregion
		#region Methods
		#region Internal methods

		protected virtual bool ContainsItem(T item)
		{
			using (IEnumerator<T> e = GetItemEnumerator())
				while (e.MoveNext())
					if (Equals(e.Current, item))
						return true;
			return false;
		}

		protected virtual void CopyItemsTo(T[] array, int arrayIndex)
		{
			using (IEnumerator<T> e = GetItemEnumerator())
				while (e.MoveNext() && arrayIndex < array.Length)
					array[arrayIndex++] = e.Current;
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
			if (array.Length - arrayIndex < ItemsCount)
				throw new ArgumentException("Noenough spactstore.", "array");
			//
			CopyItemsTo(array, arrayIndex);
		}

		#endregion
		#endregion
	}
}
