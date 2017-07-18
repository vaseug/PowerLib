using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PowerLib.System.Collections;

namespace PowerLib.System
{
  /// <summary>
  /// Represents information about regular array (rank, total length, dimensional lengths, lower and upper bounds, factors).
  /// </summary>
  public sealed class RegularArrayLongInfo : ArrayLongInfo
  {
    private int _rank;
    private long _length;
    private long[] _lengths;
    private long[] _bases;
    private long[] _factors;
    private IReadOnlyList<long> _lengthsAccessor;
    private IReadOnlyList<long> _lowerBoundsAccessor;
    private IReadOnlyList<long> _upperBoundsAccessor;
    private IReadOnlyList<long> _factorsAccessor;

    #region Constructors

    public RegularArrayLongInfo(params long[] lengths)
    {
      if (lengths == null)
        throw new ArgumentNullException("lengths");
      for (int i = 0; i < lengths.Length; i++)
        if (lengths[i] < 0)
          throw new ArgumentRegularArrayElementException("lengths", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);
      //
      _rank = lengths.Length;
      _bases = new long[_rank];
      _lengths = new long[_rank];
      _factors = new long[_rank];
      _length = 1;
      for (int i = _rank - 1; i >= 0; i--)
      {
        _lengths[i] = lengths[i];
        _bases[i] = 0;
        _factors[i] = (i == _rank - 1) ? 1 : lengths[i + 1] * _factors[i + 1];
        _length *= lengths[i];
      }
    }

    public RegularArrayLongInfo(params ArrayLongDimension[] arrayDims)
    {
      if (arrayDims == null)
        throw new ArgumentNullException("arrayDims");
      //
      _rank = arrayDims.Length;
      _bases = new long[_rank];
      _lengths = new long[_rank];
      _factors = new long[_rank];
      _length = 1;
      for (int i = _rank - 1; i >= 0; i--)
      {
        _lengths[i] = arrayDims[i].Length;
        _bases[i] = arrayDims[i].LowerBound;
        _factors[i] = (i == _rank - 1) ? 1 : arrayDims[i + 1].Length * _factors[i + 1];
        _length *= arrayDims[i].Length;
      }
    }

    public RegularArrayLongInfo(long[] lengths, long[] lowerBounds)
    {
      if (lengths == null)
        throw new ArgumentNullException("lengths");
      if (lowerBounds == null)
        throw new ArgumentNullException("lowerBounds");
      if (lengths.Length != lowerBounds.Length)
        throw new ArgumentException("Inconsistent lengths and lowerBounds arrays.");

      for (int i = 0; i < lengths.Length; i++)
      {
        if (lengths[i] < 0L)
          throw new ArgumentRegularArrayElementException("lengths", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);
        if (lowerBounds[i] < 0L)
          throw new ArgumentRegularArrayElementException("lowerBounds", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);
        if (long.MaxValue - lowerBounds[i] > lengths[i])
          throw new ArgumentRegularArrayElementException(null, ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);
      }
      //
      _rank = lengths.Length;
      _bases = new long[_rank];
      _lengths = new long[_rank];
      _factors = new long[_rank];
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
    public override long Length
    {
      get
      {
        return _length;
      }
    }

    /// <summary>
    /// Array dimensional lengths accessor
    /// </summary>
    public IReadOnlyList<long> Lengths
    {
      get
      {
        if (_lengthsAccessor == null)
          _lengthsAccessor = new ReadOnlyCollection<long>(_lengths);
        return _lengthsAccessor;
      }
    }

    /// <summary>
    /// Array dimensionalowebounds accessor
    /// </summary>
    public IReadOnlyList<long> LowerBounds
    {
      get
      {
        if (_lowerBoundsAccessor == null)
          _lowerBoundsAccessor = new ReadOnlyCollection<long>(_bases);
        return _lowerBoundsAccessor;
      }
    }

    /// <summary>
    /// Array dimensionauppebounds accessor
    /// </summary>
    public IReadOnlyList<long> UpperBounds
    {
      get
      {
        if (_upperBoundsAccessor == null)
          _upperBoundsAccessor = new ReadOnlyCollection<long>(_lengths.Select((length, index) => _bases[index] + length - 1).ToArray());
        return _upperBoundsAccessor;
      }
    }

    /// <summary>
    /// Array dimensionafactors
    /// </summary>
    public IReadOnlyList<long> Factors
    {
      get
      {
        if (_factorsAccessor == null)
          _factorsAccessor = new ReadOnlyCollection<long>(_factors);
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
      if (_bases.Any(t => t != 0))
        throw new InvalidOperationException("Not allowed bounds are zero");
      //
      return Array.CreateInstance(elementType, _lengths);
    }

    public override object GetValue(Array array, bool asRanges, bool zeroBased, params long[] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (array.Rank != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayRank], "array");
      else if (!asRanges && array.LongLength != _length || asRanges && array.LongLength < _length)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "array");
      for (int i = 0; i < _rank; i++)
        if (!asRanges && _bases[i] != 0/*array.GetLowerBound(i) || _range && array.GetLowerBound(i) > _bases[i]*/)
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimBase], i));
        else if (!asRanges && array.GetLongLength(i) != _lengths[i] || asRanges && /*array.GetLowerBound(i) + */array.GetLongLength(i) < _bases[i] + _lengths[i])
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimLength], i));
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      for (int i = 0; i < _rank; i++)
        if (dimIndices[i] < (zeroBased ? 0 : _bases[i]) || dimIndices[i] >= _lengths[i] + (zeroBased ? 0 : _bases[i]))
          throw new ArgumentRegularArrayLongElementException("dimIndices", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);

      if (!zeroBased)
        return array.GetValue(dimIndices);
      else
      {
        long[] indices = new long[_rank];
        for (int i = 0; i < _rank; i++)
          indices[i] = dimIndices[i] + _bases[i];
        return array.GetValue(indices);
      }
    }

    public override void SetValue(Array array, object value, bool asRanges, bool zeroBased, params long[] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (array.Rank != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayRank], "array");
      else if (!asRanges && array.LongLength != _length || asRanges && array.LongLength < _length)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "array");
      for (int i = 0; i < _rank; i++)
        if (!asRanges && _bases[i] != 0 /*array.GetLowerBound(i) || _range && array.GetLowerBound(i) > _bases[i]*/)
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimBase], i));
        else if (!asRanges && array.GetLongLength(i) != _lengths[i] || asRanges && /*array.GetLowerBound(i) + */array.GetLongLength(i) < _bases[i] + _lengths[i])
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
        long[] indices = new long[_rank];
        for (int i = 0; i < _rank; i++)
          indices[i] = dimIndices[i] + _bases[i];
        array.SetValue(value, indices);
      }
    }

    public override T GetValue<T>(Array array, bool asRanges, bool zeroBased, params long[] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (array.Rank != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayRank], "array");
      else if (!asRanges && array.LongLength != _length || asRanges && array.LongLength < _length)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "array");
      for (int i = 0; i < _rank; i++)
        if (!asRanges && _bases[i] != 0/*array.GetLowerBound(i) || _range && array.GetLowerBound(i) > _bases[i]*/)
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimBase], i));
        else if (!asRanges && array.GetLongLength(i) != _lengths[i] || asRanges && /*array.GetLowerBound(i) + */array.GetLongLength(i) < _bases[i] + _lengths[i])
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimLength], i));
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      for (int i = 0; i < _rank; i++)
        if (dimIndices[i] < (zeroBased ? 0 : _bases[i]) || dimIndices[i] >= _lengths[i] + (zeroBased ? 0 : _bases[i]))
          throw new ArgumentRegularArrayLongElementException("dimIndices", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);

      if (!zeroBased)
        return (T)array.GetValue(dimIndices);
      else
      {
        long[] indices = new long[_rank];
        for (int i = 0; i < _rank; i++)
          indices[i] = dimIndices[i] + _bases[i];
        return (T)array.GetValue(indices);
      }
    }

    public override void SetValue<T>(Array array, T value, bool asRanges, bool zeroBased, params long[] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (array.Rank != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayRank], "array");
      else if (!asRanges && array.LongLength != _length || asRanges && array.LongLength < _length)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "array");
      for (int i = 0; i < _rank; i++)
        if (!asRanges && _bases[i] != 0 /*array.GetLowerBound(i) || _range && array.GetLowerBound(i) > _bases[i]*/)
          throw new ArgumentException("array", string.Format(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayDimBase], i));
        else if (!asRanges && array.GetLongLength(i) != _lengths[i] || asRanges && /*array.GetLowerBound(i) + */array.GetLongLength(i) < _bases[i] + _lengths[i])
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
        long[] indices = new long[_rank];
        for (int i = 0; i < _rank; i++)
          indices[i] = dimIndices[i] + _bases[i];
        array.SetValue(value, indices);
      }
    }

    public override long CalcFlatIndex(bool zeroBased, params long[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      for (int i = 0; i < dimIndices.Length; i++)
        if (dimIndices[i] < (zeroBased ? 0 : _bases[i]) || dimIndices[i] >= _lengths[i] + (zeroBased ? 0 : _bases[i]))
          throw new ArgumentCollectionElementException("dimIndices", "Index value is out of range", i);
      //
      long flatIndex = 0;
      for (int i = 0; i < dimIndices.Length; i++)
        flatIndex += (dimIndices[i] - (zeroBased ? 0 : _bases[i])) * _factors[i];
      return flatIndex;
    }

    public override void CalcDimIndices(long flatIndex, bool zeroBased, long[] dimIndices)
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

    public override void GetMinDimIndices(bool zeroBased, long[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      //
      for (int i = 0; i < dimIndices.Length; i++)
        dimIndices[i] = zeroBased ? 0 : _bases[i];
    }

    public override void GetMaxDimIndices(bool zeroBased, long[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      if (dimIndices.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      //
      for (int i = 0; i < dimIndices.Length; i++)
        dimIndices[i] = (zeroBased ? 0 : _bases[i]) + _lengths[i] - 1;
    }

    public override bool IncDimIndices(bool zeroBased, long[] dimIndices)
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
        dimIndices[j] = dimIndices[j] + c < _lengths[j] + (zeroBased ? 0 : _bases[j]) ? dimIndices[j] + c-- : (zeroBased ? 0 : _bases[j]);
      return c > 0;
    }

    public override bool DecDimIndices(bool zeroBased, long[] dimIndices)
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

    public long GetLength(int dimension)
    {
      if (dimension < 0 || dimension >= _rank)
        throw new ArgumentOutOfRangeException("dimension");
      //
      return _lengths[dimension];
    }

    public long GetLowerBound(int dimension)
    {
      if (dimension < 0 || dimension >= _rank)
        throw new ArgumentOutOfRangeException("dimension");
      //
      return _bases[dimension];
    }

    public long GetUpperBound(int dimension)
    {
      if (dimension < 0 || dimension >= _rank)
        throw new ArgumentOutOfRangeException("dimension");
      //
      return _bases[dimension] + _lengths[dimension] - 1;
    }

    public long GetFactor(int dimension)
    {
      if (dimension < 0 || dimension >= _rank)
        throw new ArgumentOutOfRangeException("dimension");
      //
      return _factors[dimension];
    }

    public void GetLengths(long[] lengths)
    {
      if (lengths == null)
        throw new ArgumentNullException("lengths");
      if (lengths.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "lengths");
      //
      for (int i = 0; i < _lengths.Length; i++)
        lengths[i] = _lengths[i];
    }

    public void GetLowerBounds(long[] bounds)
    {
      if (bounds == null)
        throw new ArgumentNullException("bounds");
      if (bounds.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "bounds");
      //
      for (int i = 0; i < _bases.Length; i++)
        bounds[i] = _bases[i];
    }

    public void GetUpperBounds(long[] bounds)
    {
      if (bounds == null)
        throw new ArgumentNullException("bounds");
      if (bounds.Length != _rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "bounds");
      //
      for (int i = 0; i < _bases.Length; i++)
        bounds[i] = _bases[i] + _lengths[i] - 1;
    }

    public void GetFactors(long[] factors)
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
