using System;
using System.Collections;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Typof item.</typeparam>
	public class PwrListViewBase<T> : PwrReadOnlyListBase<T>, IList<T>, IReadOnlyList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		#region Constructors

		protected PwrListViewBase(IList<T> list)
			: base(list)
		{
		}

		#endregion
		#region Interfaces
		#region ICollection<T> implementation

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		void ICollection<T>.Add(T value)
		{
			throw new NotSupportedException();
		}

		bool ICollection<T>.Remove(T value)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		#endregion
		#region IList<T> interface implementation

		T IList<T>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		void IList<T>.Insert(int index, T value)
		{
			throw new NotSupportedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		#endregion
		#endregion
	}
}
