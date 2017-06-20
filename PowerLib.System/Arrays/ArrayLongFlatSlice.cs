using System;
using System.Collections;
using System.Collections.Generic;
using PowerLib.System.Collections;

namespace PowerLib.System
{
	public struct ArrayLongFlatSlice<T> : IEnumerable<T>, IEnumerable
	{
		private ArrayLongInfo _arrayInfo;
		private Array _array;
		private LongRange _range;

		#region Constructors

		public ArrayLongFlatSlice(Array array)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (!typeof(T).Equals(array.GetType().GetElementType()))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

			_array = array;
			_range = new LongRange(0L, array.LongLength);
			_arrayInfo = new RegularArrayLongInfo (array.GetRegularArrayLongDimensions());
		}

		public ArrayLongFlatSlice(Array array, LongRange range)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (!typeof(T).Equals(array.GetType().GetElementType()))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			if (range.Index < 0 || range.Index > array.LongLength)
				throw new ArgumentOutOfRangeException("range.Index");
			if (range.Count < 0 || range.Count > array.LongLength - range.Index)
				throw new ArgumentOutOfRangeException("range.Count");

			_array = array;
			_range = range;
			_arrayInfo = new RegularArrayLongInfo (array.GetRegularArrayLongDimensions());
		}

		#endregion
		#region Properties

		public Array Array
		{
			get { return _array; }
		}

		public LongRange Range
		{
			get { return _range; }
		}

		public T this[long index]
		{
			get
			{
				if (_array == null)
					throw new InvalidOperationException("Array is null");
				if (index < 0|| index >= _range.Count)
					throw new ArgumentOutOfRangeException();

				long[] dimIndices = new long[_arrayInfo.Rank];
				_arrayInfo.CalcDimIndices(_range.Index + index, dimIndices);
				return (T)_array.GetValue(dimIndices);
			}
			set
			{
				if (_array == null)
					throw new InvalidOperationException("Array is null");
				if (index < 0|| index >= _range.Count)
					throw new ArgumentOutOfRangeException();

				long[] dimIndices = new long[_arrayInfo.Rank];
				_arrayInfo.CalcDimIndices(_range.Index + index, dimIndices);
				_array.SetValue(value, dimIndices);
			}
		}

		#endregion
		#region Methods

		public T[] ToArray()
		{
			if (_array == null)
				throw new InvalidOperationException("Array is null");

			return (T[])_array.RangeAsLongRegular(null, _range);
		}

		#endregion
		#region Interface IEnumerable<T> implementation

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			if (_array == null)
				throw new InvalidOperationException("Array is null");

			foreach (T item in _array.EnumerateAsLongRegular<T>(false, _range))
				yield return item;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		#endregion
	}
}
