using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerLib.System.Collections
{
	public struct RegularArrayEnumerator<T> : IEnumerator<T>
	{
		private Array _array;
		private ArrayIndex _arrayIndex;
		private Func<int> _getStamp;
		private int _stamp;
		private T _current;

		#region Constructors

		public RegularArrayEnumerator(Array array)
			: this(array, false, null)
		{
		}

		public RegularArrayEnumerator(Array array, bool zeroBased)
			: this(array, zeroBased, null)
		{
		}

		public RegularArrayEnumerator(Array array, Func<int> getStamp)
			: this(array, false, getStamp)
		{
		}

		public RegularArrayEnumerator(Array array, bool zeroBased, Func<int> getStamp)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			//
			_arrayIndex = new ArrayIndex(new RegularArrayInfo(array.GetRegularArrayDimensions())) { ZeroBased = zeroBased };
			_array = array;
			_getStamp = getStamp;
			_stamp = getStamp != null ? getStamp() : 0;
			_current = default(T);
		}

		#endregion
		#region Properties

		public int FlatIndex
		{
			get
			{
				return _arrayIndex.FlatIndex;
			}
		}

		public T Current
		{
			get
			{
				if (_arrayIndex.Carry != 0)
					throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.EnumeratorPositionAfterLast]);
				else if (_array.Length == 0 || _arrayIndex.FlatIndex == 0)
					throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.EnumeratorPositionBeforeFirst]);
				//
				return _current;
			}
		}

		#endregion
		#region Methods

		public bool MoveNext()
		{
			if (_getStamp != null && _stamp != _getStamp())
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionWasModified]);
			else if (_array.Length == 0 || _arrayIndex.Carry != 0)
				return false;
			else
			{
				_current = _arrayIndex.GetValue<T>(_array);
				_arrayIndex++;
				return true;
			}
		}

		public void Reset()
		{
			_current = default(T);
			_arrayIndex.SetMin();
		}

		#endregion
		#region IEnumeratoimplementation

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		#endregion
		#region IDisposablimplementation

		void IDisposable.Dispose()
		{
		}

		#endregion
	}
}
