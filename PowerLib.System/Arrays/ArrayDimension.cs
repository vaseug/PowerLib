using System;

namespace PowerLib.System
{
  /// <summary>
  /// Represents inforamtion about regular array dimension (length, lower and upper bound).
  /// </summary>
  public struct ArrayDimension
  {
    private int _length;
    private int _lowerBound;

    #region Constructors

    public ArrayDimension(int length)
      : this(length, 0)
    {
    }

    public ArrayDimension(int length, int lowerBound)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");
      if (lowerBound < 0)
        throw new ArgumentOutOfRangeException("lowerBound");
      if (lowerBound > int.MaxValue - length)
        throw new ArgumentException("Inconsistent array dimensiolength anlowerBound");
      //
      _length = length;
      _lowerBound = lowerBound;
    }

    #endregion
    #region Properties

    public int Length
    {
      get
      {
        return _length;
      }
    }

    public int LowerBound
    {
      get
      {
        return _lowerBound;
      }
    }

    public int UpperBound
    {
      get
      {
        return Length == 0 ? LowerBound : LowerBound + Length - 1;
      }
    }

    #endregion
  }
}
