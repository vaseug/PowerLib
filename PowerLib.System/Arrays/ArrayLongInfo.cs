using System;

namespace PowerLib.System
{
  /// <summary>
  /// Base class that represents information about long addressing array.
  /// </summary>
  /// <summary xml:lang="ru">
  /// Ѕазовый класс который представл€ет служебную информацию о массиве с адрессацией длинным целым.
  /// </summary>
  public abstract class ArrayLongInfo
  {
    #region Instance properties

    public abstract int Rank
    {
      get;
    }

    public abstract long Length
    {
      get;
    }

    #endregion
    #region Instance methods

    public Array CreateArray<T>()
    {
      return CreateArray(typeof(T));
    }

    public abstract Array CreateArray(Type elementType);

    public virtual object GetValue(Array array, params long[] dimIndices)
    {
      return GetValue(array, false, false, dimIndices);
    }

    public abstract object GetValue(Array array, bool asRanges, bool zeroBased, params long[] dimIndices);

    public virtual void SetValue(Array array, object value, params long[] dimIndices)
    {
      SetValue(array, value, false, false, dimIndices);
    }

    public abstract void SetValue(Array array, object value, bool asRanges, bool zeroBased, params long[] dimIndices);

    public virtual T GetValue<T>(Array array, params long[] dimIndices)
    {
      return GetValue<T>(array, false, false, dimIndices);
    }

    public abstract T GetValue<T>(Array array, bool asRanges, bool zeroBased, params long[] dimIndices);

    public virtual void SetValue<T>(Array array, T value, params long[] dimIndices)
    {
      SetValue<T>(array, value, false, false, dimIndices);
    }

    public abstract void SetValue<T>(Array array, T value, bool asRanges, bool zeroBased, params long[] dimIndices);

    public virtual long CalcFlatIndex(params long[] dimIndices)
    {
      return CalcFlatIndex(false, dimIndices);
    }

    public abstract long CalcFlatIndex(bool zeroBased, params long[] dimIndices);

    public virtual void CalcDimIndices(long flatIndex, long[] dimIndices)
    {
      CalcDimIndices(flatIndex, false, dimIndices);
    }

    public abstract void CalcDimIndices(long flatIndex, bool zeroBased, long[] dimIndices);

    public virtual void GetMinDimIndices(long[] dimIndices)
    {
      GetMinDimIndices(false, dimIndices);
    }

    public abstract void GetMinDimIndices(bool zeroBased, long[] dimIndices);

    public virtual void GetMaxDimIndices(long[] dimIndices)
    {
      GetMaxDimIndices(false, dimIndices);
    }

    public abstract void GetMaxDimIndices(bool zeroBased, long[] dimIndices);

    public virtual bool IncDimIndices(long[] dimIndices)
    {
      return IncDimIndices(false, dimIndices);
    }

    public abstract bool IncDimIndices(bool zeroBased, long[] dimIndices);

    public virtual bool DecDimIndices(long[] dimIndices)
    {
      return DecDimIndices(false, dimIndices);
    }

    public abstract bool DecDimIndices(bool zeroBased, long[] dimIndices);

    #endregion
  }
}
