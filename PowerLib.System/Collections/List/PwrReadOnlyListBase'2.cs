using System;
using System.Collections;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="S">Type of store item.</typeparam>
	/// <typeparam name="T">Type of view item.</typeparam>
	public abstract class PwrReadOnlyListBase<S, T> : PwrReadOnlyCollectionBase<S, T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
	{
		#region Constructors

		protected PwrReadOnlyListBase(IList<S> list)
			: base(list)
		{
		}

		#endregion
		#region Properties
		#region Internal properties

		new protected IList<S> InnerStore
		{
			get
			{
				return (IList<S>)base.InnerStore;
			}
		}

		#endregion
		#region Public properties

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= InnerStore.Count)
					throw new ArgumentOutOfRangeException("index");

				return GetItem(InnerStore[index]);
			}
		}

		#endregion
		#endregion
		#region Methods
		#region Internal methods

		protected virtual int IndexOfItem(T value)
		{
			for (int i = 0; i < InnerStore.Count; i++)
				if (Equals(GetItem(InnerStore[i]), value))
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
