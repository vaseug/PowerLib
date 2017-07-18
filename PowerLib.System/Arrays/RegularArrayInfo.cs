using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PowerLib.System.Collections;

namespace PowerLib.System
{
  public sealed class RegularArrayInfo : ArrayInfo
  {
    private int _rank;
    private int _length;
    private int[] _lengths;
    private int[] _bases;
    private int[] _factors;
    private IReadOnlyList<int> _lengthsAccessor;
    private IReadOnlyList<int> _lowerBoundsAccessor;
    private IReadOnlyList<int> _upperBoundsAccessor;
    private IReadOnlyList<int> _factorsAccessor;

    #region Constructors

    public RegularArrayInfo(params int[] lengths)
    {
      if (lengths == null)
        throw new ArgumentNullException("lengths");
      for (int i = 0; i < lengths.Length; i++)
        if (lengths[i] < 0)
          throw new ArgumentRegularArrayElementException("lengths", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);
      //
      _rank = lengths.Length;
      _bases = new int[_rank];
      _lengths = new int[_rank];
      _factors = new int[_rank];
      _length = 1;
      for (int i = _rank - 1; i >= 0; i--)
      {
        _lengths[i] = lengths[i];
        _bases[i] = 0;
        _factors[i] = (i == _rank - 1) ? 1 : lengths[i + 1] * _factors[i + 1];
        _length *= lengths[i];
      }
    }

    public RegularArrayInfo(params ArrayDimension[] dimensions)
    {
      if (dimensions == null)
        throw new ArgumentNullException("arrayDims");
      //
      _rank = dimensions.Length;
      _bases = new int[_rank];
      _lengths = new int[_rank];
      _factors = new int[_rank];
      _length = 1;
      for (int i = _rank - 1; i >= 0; i--)
      {
        _lengths[i] = dimensions[i].Length;
        _bases[i] = dimensions[i].LowerBound;
        _factors[i] = (i == _rank - 1) ? 1 : dimensions[i + 1].Length * _factors[i + 1];
        _length *= dimensions[i].Length;
      }
    }

    public RegularArrayInfo(int[] lengths, int[] lowerBounds)
    {
      if (lengths == null)
        throw new ArgumentNullException("lengths");
      if (lowerBounds == null)
        throw new ArgumentNullException("lowerBounds");
      if (lengths.Length != lowerBounds.Length)
        throw new ArgumentException("Inconsistent lengths and lowerBounds arrays.");

      for (int i = 0; i < lengths.Length; i++)
      {
        if (lengths[i] < 0)
          throw new ArgumentRegularArrayElementException("lengths", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);
        if (lowerBounds[i] < 0)
          throw new ArgumentRegularArrayElementException("lowerBounds", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);
        if (int.MaxValue - lowerBounds[i] > lengths[i])
          throw new ArgumentRegularArrayElementException(null, ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);
      }
      //
      _rank = lengths.Length;
      _bases = new int[_rank];
      _lengths = new int[_rank];
      _factors = new int[_rank];
      _length = 1;
      for (int i = _rank - 1; i >= 0; i--)
      {
        _lengths[i] = lengths[i];
        _bases[i] = lowerBounds[i];
        _factors[i] = (i == _rank - 1) ? 1 : lengths[i + 1] * _factors[i + 1];
        _length *= lengths[i];
      }
    }

    #endregion
    #region Instance properties

    /// <summary>
    /// Array rank
    /// </summary>
    public override int Rank
    {
      get
      {
        return _rank;
      }
    }

    /// <summary>
    /// Array total length
    /// </summary>
    public override int Length
    {
      get
      {
        return _length;
      }
    }

    /// <summary>
    /// Array dimensional lengths accessor
    /// </summary>
    public IReadOnlyList<int> Lengths
    {
      get
      {
        if (_lengthsAccessor == null)
          _lengthsAccessor = new ReadOnlyCollection<int>(_lengths);
        return _lengthsAccessor;
      }
    }

    /// <summary>
    /// Array dimensionalowebounds accessor
    /// </summary>
    public IReadOnlyList<int> LowerBounds
    {
      get
      {
        if (_lowerBoundsAccessor == null)
          _lowerBoundsAccessor = new ReadOnlyCollection<int>(_bases);
        return _lowerBoundsAccessor;
      }
    }

    /// <summary>
    /// Array dimensionauppebounds accessor
    /// </summary>
    public IReadOnlyList<int> UpperBounds
    {
      get
      {
        if (_upperBoundsAccessor == null)
          _upperBoundsAccessor = new ReadOnlyCollection<int>(_lengths.Select((length, index) => _bases[index] + length - 1).ToArray());
        return _upperBoundsAccessor;
      }
    }

    /// <summary>
    /// Array dimensionafactors
    /// </summary>
    public IReadOnlyList<int> Factors
    {
      get
      {
        if (_factorsAccessor == null)
          _factorsAccessor = new ReadOnlyCollection<int>(_factors);
        return null;
      }
    }

    #endregion
    #region Instance methods

    /// <summary>
    /// Create new array specified by this ArrayInfo
    /// </summary>
    /// <param name="elementType">Element type of created array.</param>
    /// <returns>Created array.</returns>
    public override Array CreateArray(Type elementType)
    {
      if (elementType == null)
        throw new ArgumentNullException("elementType");
      //
      return Array.CreateInstance(elementType, _lengths, _bases);
    }

    public override object GetValue(Array array, bool asRanges, bool zeroBased, params int[] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (array.Rank != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayRank], "array");
      else if (!asRanges && array.Length != _length || asRanges && array.Length < _length)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "array");
      for (int i = 0; i < _rank; i++)
        if (!asRanges && array.GetLowerBound(i) != _bases[i] || asRanges && array.GetLowerBound(i) > _bases[i])
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimBase], i));
        else if (!asRanges && array.GetLength(i) != _lengths[i] || asRanges && array.GetLowerBound(i) + array.GetLength(i) < _bases[i] + _lengths[i])
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimLength], i));
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      for (int i = 0; i < _rank; i++)
        if (dimIndices[i] < (zeroBased ? 0 : _bases[i]) || dimIndices[i] >= _lengths[i] + (zeroBased ? 0 : _bases[i]))
          throw new ArgumentRegularArrayElementException("dimIndices", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);

      if (!zeroBased)
        return array.GetValue(dimIndices);
      else
      {
        int[] indices = new int[_rank];
        for (int i = 0; i < _rank; i++)
          indices[i] = dimIndices[i] + _bases[i];
        return array.GetValue(indices);
      }
    }

    public override void SetValue(Array array, object value, bool asRanges, bool zeroBased, params int[] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (array.Rank != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayRank], "array");
      else if (!asRanges && array.Length != _length || asRanges && array.Length < _length)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "array");
      for (int i = 0; i < _rank; i++)
        if (!asRanges && array.GetLowerBound(i) != _bases[i] || asRanges && array.GetLowerBound(i) > _bases[i])
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimBase], i));
        else if (!asRanges && array.GetLength(i) != _lengths[i] || asRanges && array.GetLowerBound(i) + array.GetLength(i) < _bases[i] + _lengths[i])
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimLength], i));
      if (!array.GetType().GetElementType().IsValueAssignable(value))
        throw new ArgumentException("Inassignable value", "value");
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      for (int i = 0; i < _rank; i++)
        if (dimIndices[i] < (zeroBased ? 0 : _bases[i]) || dimIndices[i] >= _lengths[i] + (zeroBased ? 0 : _bases[i]))
          throw new ArgumentRegularArrayElementException("dimIndices", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);

      if (!zeroBased)
        array.SetValue(value, dimIndices);
      else
      {
        int[] indices = new int[_rank];
        for (int i = 0; i < _rank; i++)
          indices[i] = dimIndices[i] + _bases[i];
        array.SetValue(value, indices);
      }
    }

    public override T GetValue<T>(Array array, bool asRanges, bool zeroBased, params int[] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (array.Rank != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayRank], "array");
      else if (!asRanges && array.Length != _length || asRanges && array.Length < _length)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "array");
      for (int i = 0; i < _rank; i++)
        if (!asRanges && array.GetLowerBound(i) != _bases[i] || asRanges && array.GetLowerBound(i) > _bases[i])
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimBase], i));
        else if (!asRanges && array.GetLength(i) != _lengths[i] || asRanges && array.GetLowerBound(i) + array.GetLength(i) < _bases[i] + _lengths[i])
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimLength], i));
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      for (int i = 0; i < _rank; i++)
        if (dimIndices[i] < (zeroBased ? 0 : _bases[i]) || dimIndices[i] >= _lengths[i] + (zeroBased ? 0 : _bases[i]))
          throw new ArgumentRegularArrayElementException("dimIndices", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);

      if (!zeroBased)
        return (T)array.GetValue(dimIndices);
      else
      {
        int[] indices = new int[_rank];
        for (int i = 0; i < _rank; i++)
          indices[i] = dimIndices[i] + _bases[i];
        return (T)array.GetValue(indices);
      }
    }

    public override void SetValue<T>(Array array, T value, bool asRanges, bool zeroBased, params int[] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (array.Rank != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayRank], "array");
      else if (!asRanges && array.Length != _length || asRanges && array.Length < _length)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "array");
      for (int i = 0; i < _rank; i++)
        if (!asRanges && array.GetLowerBound(i) != _bases[i] || asRanges && array.GetLowerBound(i) > _bases[i])
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimBase], i));
        else if (!asRanges && array.GetLength(i) != _lengths[i] || asRanges && array.GetLowerBound(i) + array.GetLength(i) < _bases[i] + _lengths[i])
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimLength], i));
      if (!array.GetType().GetElementType().IsValueAssignable(value))
        throw new ArgumentException("Inassignable value", "value");
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      for (int i = 0; i < _rank; i++)
        if (dimIndices[i] < (zeroBased ? 0 : _bases[i]) || dimIndices[i] >= _lengths[i] + (zeroBased ? 0 : _bases[i]))
          throw new ArgumentRegularArrayElementException("dimIndices", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);

      if (!zeroBased)
        array.SetValue(value, dimIndices);
      else
      {
        int[] indices = new int[_rank];
        for (int i = 0; i < _rank; i++)
          indices[i] = dimIndices[i] + _bases[i];
        array.SetValue(value, indices);
      }
    }

    public override int CalcFlatIndex(bool zeroBased, params int[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      for (int i = 0; i < dimIndices.Length; i++)
        if (dimIndices[i] < (zeroBased ? 0 : _bases[i]) || dimIndices[i] >= _lengths[i] + (zeroBased ? 0 : _bases[i]))
          throw new ArgumentCollectionElementException("dimIndices", "Index value is out of range", i);
      //
      int flatIndex = 0;
      for (int i = 0; i < dimIndices.Length; i++)
        flatIndex += (dimIndices[i] - (zeroBased ? 0 : _bases[i])) * _factors[i];
      return flatIndex;
    }

    public override void CalcDimIndices(int flatIndex, bool zeroBased, int[] dimIndices)
    {
      if (flatIndex < 0 || flatIndex >= _length)
        throw new ArgumentOutOfRangeException("flatIndex");
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      //
      for (int i = 0; i < dimIndices.Length; i++)
      {
        dimIndices[i] = flatIndex / _factors[i] + (zeroBased ? 0 : _bases[i]);
        flatIndex %= _factors[i];
      }
    }

    public override void GetMinDimIndices(bool zeroBased, int[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      //
      for (int i = 0; i < dimIndices.Length; i++)
        dimIndices[i] = zeroBased ? 0 : _bases[i];
    }

    public override void GetMaxDimIndices(bool zeroBased, int[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      //
      for (int i = 0; i < dimIndices.Length; i++)
        dimIndices[i] = (zeroBased ? 0 : _bases[i]) + _lengths[i] - 1;
    }

    public override bool IncDimIndices(bool zeroBased, int[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      if (_length == 0)
        return true;
      for (int i = 0; i < dimIndices.Length; i++)
        if (dimIndices[i] < (zeroBased ? 0 : _bases[i]) || dimIndices[i] >= _lengths[i] + (zeroBased ? 0 : _bases[i]))
          throw new ArgumentRegularArrayElementException("dimIndices", "Index value is out of range", i);
      //
      int c = 1;
      for (int j = _rank - 1; j >= 0 && c > 0; j--)
        dimIndices[j] = dimIndices[j] + c < _lengths[j] + (zeroBased ? 0 : _bases[j]) ? dimIndices[j] + c-- : (zeroBased ? 0 : _bases[j]);
      return c > 0;
    }

    public override bool DecDimIndices(bool zeroBased, int[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      if (_length == 0)
        return true;
      for (int i = 0; i < dimIndices.Length; i++)
        if (dimIndices[i] < (zeroBased ? 0 : _bases[i]) || dimIndices[i] >= _lengths[i] + (zeroBased ? 0 : _bases[i]))
          throw new ArgumentCollectionElementException("dimIndices", "Index value is out of range", i);
      //
      int c = 1;
      for (int j = _rank - 1; j >= 0 && c > 0; j--)
        dimIndices[j] = dimIndices[j] - c >= (zeroBased ? 0 : _bases[j]) ? dimIndices[j] - c-- : _lengths[j] + (zeroBased ? 0 : _bases[j]) - 1;
      return c > 0;
    }

    public int GetLength(int dimension)
    {
      if (dimension < 0 || dimension >= _rank)
        throw new ArgumentOutOfRangeException("dimension");
      //
      return _lengths[dimension];
    }

    public int GetLowerBound(int dimension)
    {
      if (dimension < 0 || dimension >= _rank)
        throw new ArgumentOutOfRangeException("dimension");
      //
      return _bases[dimension];
    }

    public int GetUpperBound(int dimension)
    {
      if (dimension < 0 || dimension >= _rank)
        throw new ArgumentOutOfRangeException("dimension");
      //
      return _bases[dimension] + _lengths[dimension] - 1;
    }

    public int GetFactor(int dimension)
    {
      if (dimension < 0 || dimension >= _rank)
        throw new ArgumentOutOfRangeException("dimension");
      //
      return _factors[dimension];
    }

    public void GetLengths(int[] lengths)
    {
      if (lengths == null)
        throw new ArgumentNullException("lengths");
      if (lengths.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "lengths");
      //
      for (int i = 0; i < _lengths.Length; i++)
        lengths[i] = _lengths[i];
    }

    public void GetLowerBounds(int[] bounds)
    {
      if (bounds == null)
        throw new ArgumentNullException("bounds");
      if (bounds.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "bounds");
      //
      for (int i = 0; i < _bases.Length; i++)
        bounds[i] = _bases[i];
    }

    public void GetUpperBounds(int[] bounds)
    {
      if (bounds == null)
        throw new ArgumentNullException("bounds");
      if (bounds.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "bounds");
      //
      for (int i = 0; i < _bases.Length; i++)
        bounds[i] = _bases[i] + _lengths[i] - 1;
    }

    public void GetFactors(int[] factors)
    {
      if (factors == null)
        throw new ArgumentNullException("factors");
      if (factors.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "factors");
      //
      for (int i = 0; i < _factors.Length; i++)
        factors[i] = _factors[i];
    }

    #endregion
  }
}
