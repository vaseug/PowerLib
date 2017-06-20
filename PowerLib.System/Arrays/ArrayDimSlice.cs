using System;
using System.Collections;
using System.Collections.Generic;
using PowerLib.System.Collections;

namespace PowerLib.System
{
	public struct ArrayDimSlice<T> : IEnumerable<T>, IEnumerable
	{
		private ArrayInfo _arrayInfo;
		private Array _array;
		private Range[] _ranges;

		#region Constructors

		public ArrayDimSlice(Array array)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (!typeof(T).Equals(array.GetType().GetElementType()))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType], "array");

			_array = array;
			_ranges = new Range[_array.Rank];
			for (int i = 0; i < array.Rank; i++)
				_ranges[i] = new Range(0, array.GetLength(i));
			_arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions());
		}

		public ArrayDimSlice(Array array, Range[] ranges)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (!typeof(T).Equals(array.GetType().GetElementType()))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType], "array");
			if (ranges == null)
				throw new ArgumentNullException("ranges");
			if (ranges.Length != array.Rank)
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "ranges");
			for (int i = 0; i < ranges.Length; i++ )
				try
				{
					if (ranges[i].Index < 0 || ranges[i].Index > array.GetLength(i))
						throw new ArgumentOutOfRangeException("range.Index");
					if (ranges[i].Count < 0 || ranges[i].Count > array.GetLength(i) - ranges[i].Index)
						throw new ArgumentOutOfRangeException("range.Count");
				}
				catch (Exception ex)
				{
					throw new ArgumentRegularArrayElementException("ranges", ex, i);
				}

			_array = array;
			_ranges = (Range[])ranges.Clone();
			_arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions());
		}

		#endregion
		#region Properties

		public Array Array
		{
			get { return _array; }
		}

		public Range[] Ranges
		{
			get { return (Range[])_ranges.Clone(); }
		}

		public T this[params int[] indices]
		{
			get
			{
				if (_array == null)
					throw new InvalidOperationException("Array is null");
				if (indices == null)
					throw new ArgumentNullException("indices");
				if (indices.Length != _array.Rank)
					throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "indices");

				return (T)_array.GetValue(indices);
			}
			set
			{
				if (_array == null)
					throw new InvalidOperationException("Array is null");
				if (indices == null)
					throw new ArgumentNullException("indices");
				if (indices.Length != _array.Rank)
					throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "indices");

				_array.SetValue(value, indices);
			}
		}

		#endregion
		#region Methods

		public Array ToArray()
		{
			if (_array == null)
				throw new InvalidOperationException("Array is null");

      return _array.RangeAsRegular(null, null, false, null, _ranges);
		}

		#endregion
		#region Interface IEnumerable<T> implementation

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			if (_array == null)
				throw new InvalidOperationException("Array is null");

			foreach (T item in _array.EnumerateAsRegular<T>(false, null, _ranges))
				yield return item;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		#endregion
	}
}
