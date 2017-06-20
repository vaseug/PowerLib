using System;
using System.Collections;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	public class PwrCollectionViewBase<T> : PwrReadOnlyCollectionBase<T>, ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
	{
		#region Constructors

		protected PwrCollectionViewBase(ICollection<T> collection)
			: base(collection)
		{
		}

		#endregion
		#region Inerfaces
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
		#endregion
	}
}
