using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PowerLib.System
{
  /// <summary>
  ///	ArrayIndex is allow addressing any dimensional arrays even with non zero lower boundaries.
  /// ArrayIndex contains consistent values of flat index and dimensional indices.
  /// </summary>
  /// <summary xml:lang="ru">
  /// ArrayIndex позволяет производить адресацию массивов любой размерности, даже с ненулевой нижней границей.
  /// ArrayIndex содержит согласованные значения сквозного индекса и индексов каждой из размерности массива.
  /// </summary>
  public sealed class ArrayLongIndex
  {
    private ArrayLongInfo _arrayInfo;
    private bool _asRanges = false;
    private bool _zeroBased = false;
    private bool _checkOut = false;
    private long _carry = 0;
    private long _flatIndex = 0;
    private long[] _dimIndices;
    private IReadOnlyList<long> _dimIndicesAccessor;

    #region Constructors

    /// <summary>
    /// Construct array index
    /// </summary>
    /// <param name="arrayInfo">Array information class.</param>
    public ArrayLongIndex(ArrayLongInfo arrayInfo)
      : this(arrayInfo, false)
    {
    }

    /// <summary>
    /// Construct array index
    /// </summary>
    /// <param name="arrayInfo">Array information class.</param>
    /// <param name="bound">If truthen.</param>
    public ArrayLongIndex(ArrayLongInfo arrayInfo, bool bound)
    {
      _arrayInfo = arrayInfo;
      _dimIndices = new long[_arrayInfo.Rank];
      _flatIndex = bound && _arrayInfo.Length != 0 ? _arrayInfo.Length - 1 : 0;
      if (!bound)
        _arrayInfo.GetMinDimIndices(_dimIndices);
      else
        _arrayInfo.GetMaxDimIndices(_dimIndices);
    }

    #endregion
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public bool AsRanges
    {
      get
      {
        return _asRanges;
      }
      set
      {
        _asRanges = value;
      }
    }

    /// <summary>
    /// Dimensional indices would be represented as zerbase(if its nones).
    /// </summary>
    public bool ZeroBased
    {
      get
      {
        return _zeroBased;
      }
      set
      {
        if (_arrayInfo.Length == 0)
          _arrayInfo.GetMinDimIndices(value, _dimIndices);
        else
          _arrayInfo.CalcDimIndices(_flatIndex, value, _dimIndices);
        _zeroBased = value;
      }
    }

    /// <summary>
    /// Check out range of dimensional index
    /// </summary>
    public bool CheckOut
    {
      get
      {
        return _checkOut;
      }
      set
      {
        _checkOut = value;
      }
    }

    /// <summary>
    /// Carry value from previous flat index operation
    /// </summary>
    public long Carry
    {
      get
      {
        return _carry;
      }
    }

    /// <summary>
    /// Array info
    /// </summary>
    public ArrayLongInfo ArrayInfo
    {
      get
      {
        return _arrayInfo;
      }
    }

    public long FlatIndex
    {
      get
      {
        return _flatIndex;
      }
      set
      {
        if (value == FlatIndex)
          return;
        _arrayInfo.CalcDimIndices(value, _zeroBased, _dimIndices);
        _flatIndex = value;
        _carry = 0;
      }
    }

    public IReadOnlyList<long> DimIndices
    {
      get
      {
        if (_dimIndicesAccessor == null)
          _dimIndicesAccessor = new ReadOnlyCollection<long>(_dimIndices);
        return _dimIndicesAccessor;
      }
    }

    #endregion
    #region Methods
    #region Internal methods

    /// <summary>
    /// Adds value to the value of the flat delta index. 
    /// If the value of property Module false and indices out of range then throwing an exception.
    /// Otherwise, the addition is performed by module the length of the array and exception is not throw.
    /// </summary>
    /// <param name="delta"></param>
    /// <returns></returns>
    private long AddFlatIndexCore(long delta)
    {
      long carry = 0;
      bool checkOut = _checkOut;
      if (!checkOut && (delta > _arrayInfo.Length || delta < -_arrayInfo.Length))
        delta %= _arrayInfo.Length;
      else if (_arrayInfo.Length == 0)
        throw new InvalidOperationException("Operatiois out of range");
      if (delta > 0 && _arrayInfo.Length - _flatIndex < delta + 1)
      {
        if (!checkOut)
          carry = delta - (_arrayInfo.Length - _flatIndex - 1);
        else
          throw new InvalidOperationException("Operatiois out of range");
      }
      else if (delta < 0 && _flatIndex < -delta)
      {
        if (!checkOut)
          carry = -delta - _flatIndex;
        else
          throw new InvalidOperationException("Operatiois out of range");
      }
      _flatIndex +=
        delta > 0 && _arrayInfo.Length - _flatIndex < delta + 1 ? delta - _arrayInfo.Length :
        delta < 0 && _flatIndex < -delta ? delta + _arrayInfo.Length :
        delta;
      if (delta == 1)
        _arrayInfo.IncDimIndices(_zeroBased, _dimIndices);
      else if (delta == -1)
        _arrayInfo.DecDimIndices(_zeroBased, _dimIndices);
      else
        _arrayInfo.CalcDimIndices(_flatIndex, _zeroBased, _dimIndices);
      return _carry = carry;
    }

    #endregion
    #region Public methods

    public object GetValue(Array array)
    {
      return _arrayInfo.GetValue(array, _asRanges, _zeroBased, _dimIndices);
    }

    public void SetValue(Array array, object value)
    {
      _arrayInfo.SetValue(array, value, _asRanges, _zeroBased, _dimIndices);
    }

    public T GetValue<T>(Array array)
    {
      return _arrayInfo.GetValue<T>(array, _asRanges, _zeroBased, _dimIndices);
    }

    public void SetValue<T>(Array array, T value)
    {
      _arrayInfo.SetValue<T>(array, value, _asRanges, _zeroBased, _dimIndices);
    }

    public void GetDimIndices(long[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("indices");
      else if (dimIndices.Length != ArrayInfo.Rank)
        throw new ArgumentException("Invalid array size", "indices");
      //
      _dimIndices.CopyTo(dimIndices, 0);
    }

    public void SetDimIndices(params long[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("indices");
      else if (dimIndices.Length != ArrayInfo.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "indices");
      //
      _flatIndex = _arrayInfo.CalcFlatIndex(dimIndices);
      _carry = 0;
      dimIndices.CopyTo(_dimIndices, 0);
    }

    public long GetLongDimIndex(int dimension)
    {
      if (dimension < 0 || dimension >= _dimIndices.Length)
        throw new ArgumentOutOfRangeException("dimension");
      //
      return _dimIndices[dimension];
    }

    public long Add(long delta)
    {
      return AddFlatIndexCore(delta);
    }

    public long Sub(long delta)
    {
      return AddFlatIndexCore(-delta);
    }

    public bool Inc()
    {
      return AddFlatIndexCore(1) != 0;
    }

    public bool Dec()
    {
      return AddFlatIndexCore(-1) != 0;
    }

    public void SetMin()
    {
      _carry = 0;
      _flatIndex = 0;
      _arrayInfo.GetMinDimIndices(_zeroBased, _dimIndices);
    }

    public void SetMax()
    {
      _carry = 0;
      _flatIndex = _arrayInfo.Length == 0 ? 0 : _arrayInfo.Length - 1;
      _arrayInfo.GetMaxDimIndices(_zeroBased, _dimIndices);
    }

    public void SetFrom(ArrayLongIndex arrayIndex)
    {
      if (arrayIndex == null)
        throw new ArgumentNullException("arrayIndex");
      else if (arrayIndex._arrayInfo != _arrayInfo || arrayIndex._zeroBased != _zeroBased)
        throw new ArgumentException("Diffrent array specifications", "arrayIndex");
      //
      _carry = 0;
      _flatIndex = arrayIndex._flatIndex;
      arrayIndex._dimIndices.CopyTo(_dimIndices, 0);
    }

    public bool IsMin
    {
      get
      {
        return _arrayInfo.Length > 0 && _flatIndex == 0;
      }
    }

    public bool IsMax
    {
      get
      {
        return _arrayInfo.Length > 0 && _flatIndex == _arrayInfo.Length - 1;
      }
    }

    #endregion
    #endregion
    #region Operators

    /// <summary>
    /// Conversation operator to flat index value.
    /// </summary>
    /// <param name="arrayIndex">Array index.</param>
    /// <returns>Flaindex value.</returns>
    public static explicit operator long(ArrayLongIndex arrayIndex)
    {
      if (arrayIndex == null)
        throw new NullReferenceException();
      //
      return arrayIndex.FlatIndex;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrayIndex"></param>
    /// <returns></returns>
    public static ArrayLongIndex operator ++(ArrayLongIndex arrayIndex)
    {
      if (arrayIndex == null)
        throw new NullReferenceException();
      //
      arrayIndex.Inc();
      return arrayIndex;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrayIndex"></param>
    /// <returns></returns>
    public static ArrayLongIndex operator --(ArrayLongIndex arrayIndex)
    {
      if (arrayIndex == null)
        throw new NullReferenceException();
      //
      arrayIndex.Dec();
      return arrayIndex;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrayIndex"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public static ArrayLongIndex operator +(ArrayLongIndex arrayIndex, long delta)
    {
      if (arrayIndex == null)
        throw new NullReferenceException();
      //
      arrayIndex.Add(delta);
      return arrayIndex;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrayIndex"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public static ArrayLongIndex operator -(ArrayLongIndex arrayIndex, long delta)
    {
      if (arrayIndex == null)
        throw new NullReferenceException();
      //
      arrayIndex.Sub(delta);
      return arrayIndex;
    }

    #endregion
  }
}
