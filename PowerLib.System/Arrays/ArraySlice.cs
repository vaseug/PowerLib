using System;
using System.Collections;
using System.Collections.Generic;
using PowerLib.System.Collections;

namespace PowerLib.System
{
  public struct ArraySlice<T> : IList<T>, IEnumerable<T>, IEnumerable
	{
		private readonly T[] _array;
		private readonly Range _range;

		#region Constructors

		public ArraySlice(T[] array)
      : this(array, 0, array != null ? array.Length : 0)
		{
		}

		public ArraySlice(T[] array, int index, int count)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > array.Length - index)
				throw new ArgumentOutOfRangeException("count");

			_array = array;
			_range = new Range(index, count);
		}

		public ArraySlice(T[] array, Range range)
			: this(array, range.Index, range.Count)
		{
		}

		#endregion
		#region Properties

		public T[] Array
		{
			get { return _array; }
		}

    public Range Range
    {
      get { return _range; }
    }

		public T this[int index]
		{
			get
			{
        if (_array == null)
          throw new InvalidOperationException("Array is null");
        if (index < 0 || index >= _range.Count)
					throw new ArgumentOutOfRangeException("index");

        return _array[_range.Index + index];
			}
			set
			{
        if (_array == null)
          throw new InvalidOperationException("Array is null");
        if (index < 0 || index >= _range.Count)
					throw new ArgumentOutOfRangeException();

				_array[_range.Index + index] = value ;
			}
		}

		#endregion
		#region IEnumerable<T> implementation

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			if (_array == null)
				throw new InvalidOperationException("Array is null");

			for (int i = _range.Index, c = _range.Index + _range.Count; i < c; i++)
				yield return _array[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		#endregion
    #region ICollection<T> implementation

    int ICollection<T>.Count
    {
      get { return _range.Count; }
    }

    bool ICollection<T>.IsReadOnly
    {
      get
      {
        if (_array == null)
          throw new InvalidOperationException("Array is null");

        return _array.IsReadOnly;
      }
    }

   void ICollection<T>.Add(T item)
    {
      throw new NotSupportedException();
    }

    bool ICollection<T>.Remove(T item)
    {
      throw new NotSupportedException();
    }

   void ICollection<T>.Clear()
    {
      throw new NotSupportedException();
    }

    bool ICollection<T>.Contains(T item)
    {
      if (_array == null)
        throw new InvalidOperationException("Array is null");

      return _array.FindIndex(_range.Index, _range.Count, t => EqualityComparer<T>.Default.Equals(item, t)) >= 0;
    }

   void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
      _array.CopyTo(array, arrayIndex, _range.Index, _range.Count);
    }

    #endregion
    #region IList<T> implementation

   void IList<T>.Insert(int index, T item)
    {
      throw new NotSupportedException();
    }

   void IList<T>.RemoveAt(int index)
    {
      throw new NotSupportedException();
    }

    int IList<T>.IndexOf(T item)
    {
      if (_array == null)
        throw new InvalidOperationException("Array is null");

      return _array.FindIndex(_range.Index, _range.Count, t => EqualityComparer<T>.Default.Equals(item, t));
    }

    #endregion
  }
}
