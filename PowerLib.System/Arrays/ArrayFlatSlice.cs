using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerLib.System
{
  public struct ArrayFlatSlice<T> : IEnumerable<T>, IEnumerable
  {
    private ArrayInfo _arrayInfo;
    private Array _array;
    private Range _range;

    #region Constructors

    public ArrayFlatSlice(Array array)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (!typeof(T).Equals(array.GetType().GetElementType()))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      _array = array;
      _range = new Range(0, array.Length);
      _arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions());
    }

    public ArrayFlatSlice(Array array, Range range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (!typeof(T).Equals(array.GetType().GetElementType()))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
      if (range.Index < 0 || range.Index > array.Length)
        throw new ArgumentOutOfRangeException("range.Index");
      if (range.Count < 0 || range.Count > array.Length - range.Index)
        throw new ArgumentOutOfRangeException("range.Count");

      _array = array;
      _range = range;
      _arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions());
    }

    #endregion
    #region Properties

    public Array Array
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

        int[] dimIndices = new int[_arrayInfo.Rank];
        _arrayInfo.CalcDimIndices(_range.Index + index, dimIndices);
        return (T)_array.GetValue(dimIndices);
      }
      set
      {
        if (_array == null)
          throw new InvalidOperationException("Array is null");
        if (index < 0 || index >= _range.Count)
          throw new ArgumentOutOfRangeException("index");

        int[] dimIndices = new int[_arrayInfo.Rank];
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

      return (T[])_array.RangeAsRegular(null, null, false, _range);
    }

    #endregion
    #region Interface IEnumerable<T> implementation

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      if (_array == null)
        throw new InvalidOperationException("Array is null");

      foreach (T item in _array.EnumerateAsRegular<T>(false, _range))
        yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<T>)this).GetEnumerator();
    }

    #endregion
  }
}
