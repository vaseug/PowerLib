using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PowerLib.System.Collections;

namespace PowerLib.System.IO.Streamed
{
  public abstract class BaseFlaggedStreamedRegularArray<T, F> : BaseFlaggedStreamedArray<T, F>
  {
    #region Constructors

    protected BaseFlaggedStreamedRegularArray(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, leaveOpen, boostAccess)
    {
    }

    #endregion
    #region Properties
    #region Internal properties

    protected abstract RegularArrayInfo ArrayInfo
    {
      get;
    }

    #endregion
    #region Public properties

    public virtual int Rank
    {
      get { return ArrayInfo.Rank; }
    }

    public virtual IReadOnlyList<int> Lengths
    {
      get { return ArrayInfo.Lengths; }
    }

    #endregion
    #endregion
    #region Methods
    #region Internal methods

    protected virtual int CalcFlatIndex(int[] indices)
    {
      return ArrayInfo.CalcFlatIndex(indices);
    }

    protected virtual void CalcDimIndices(int index, int[] indices)
    {
      ArrayInfo.CalcDimIndices(index, indices);
    }

    #endregion
    #region Public methods

    public virtual void ValidateRanges(params Range[] ranges)
    {
      ArrayInfo.ValidateRanges(ranges);
    }

    public virtual int GetFlatIndex(params int[] indices)
    {
      return CalcFlatIndex(indices);
    }

    public virtual int[] GetDimIndices(int index)
    {
      int[] indices = new int[ArrayInfo.Rank];
      CalcDimIndices(index, indices);
      return indices;
    }

    public virtual T GetValue(params int[] indices)
    {
      return this[CalcFlatIndex(indices)];
    }

    public virtual void SetValue(T value, params int[] indices)
    {
      this[CalcFlatIndex(indices)] = value;
    }

    public virtual IEnumerable<int> EnumerateRangeIndex(params Range[] ranges)
    {
      return ArrayInfo.EnumerateFlatIndex(ranges);
    }

    public virtual Array ToRegularArray()
    {
      return ArrayInfo.CreateArray<T>(this);
    }

    #endregion
    #endregion
  }
}
