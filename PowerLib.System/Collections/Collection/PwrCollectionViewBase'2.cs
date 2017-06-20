using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PowerLib.System.Collections
{
	public abstract class PwrCollectionViewBase<S, T> : PwrReadOnlyCollectionBase<S, T>, ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
	{
		#region Constructors

		public PwrCollectionViewBase(ICollection<S> coll)
			: base(coll)
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
		#endregion
	}
}
