using System;
using System.Collections;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Typof item.</typeparam>
	public class PwrReadOnlyListBase<T> : PwrReadOnlyCollectionBase<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
	{
		#region Constructors

		protected PwrReadOnlyListBase(IList<T> list)
			: base(list)
		{
		}

		#endregion
		#region Properties
		#region Internal properties

		new protected IList<T> InnerStore
		{
			get
			{
				return (IList<T>)base.InnerStore;
			}
		}

		#endregion
		#region Public properties

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= ItemsCount)
					throw new ArgumentOutOfRangeException("index");

				return InnerStore[GetStoreIndex(index)];
			}
		}

		#endregion
		#endregion
		#region Methods
		#region Internal methods

		protected virtual int GetStoreIndex(int index)
		{
			return index;
		}

		protected override IEnumerator<T> GetItemEnumerator()
		{
			for (int i = 0; i < ItemsCount; i++)
				yield return InnerStore[GetStoreIndex(i)];
		}

		protected virtual int IndexOfItem(T value)
		{
			for (int i = 0; i < ItemsCount; i++)
				if (Equals(InnerStore[GetStoreIndex(i)], value))
					return i;
			return -1;
		}

		#endregion
		#region Public methods

		public int IndexOf(T value)
		{
			return IndexOfItem(value);
		}

		#endregion
		#endregion
	}
}
