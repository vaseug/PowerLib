using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections.Matching;

namespace PowerLib.System.IO.Streamed.Typed
{
  public class NulLongRangeStreamedArray : StoredFlaggedStreamedArray<LongRange?, Boolean>
  {
    #region Constructors

    public NulLongRangeStreamedArray(Stream stream, SizeEncoding countSizing, bool compact, ICollection<LongRange?> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, sizeof(int) * 2, compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<LongRange>.Default.Equals, coll, leaveOpen, boostAccess)
    {
    }

    public NulLongRangeStreamedArray(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, v => v.HasValue, f => f, ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<LongRange>.Default.Equals, leaveOpen, boostAccess)
    {
    }

    #endregion
    #region Properties

    protected override ExpandMethod ExpandMethod
    {
      get { return ExpandMethod.WriteZero; }
    }

    public new SizeEncoding CountSizing
    {
      get { return base.CountSizing; }
    }

    public new bool Compact
    {
      get { return base.Compact; }
    }

    #endregion
    #region Methods

    private static Boolean ReadItemFlag(Stream stream)
    {
      switch (stream.ReadUByte())
      {
        case 0:
          return false;
        case 1:
          return true;
        default:
          throw new FormatException();
      }
    }

    private static void WriteItemFlag(Stream stream, Boolean flag)
    {
      if (!flag)
        stream.WriteUByte(0);
      else
        stream.WriteUByte(1);
    }

    private static LongRange? ReadItemData(Stream stream, Boolean flag, int size)
    {
      if (!flag)
        return default(LongRange?);
      else
        return new LongRange(stream.ReadInt64(), stream.ReadInt64());
    }

    private static void WriteItemData(Stream stream, Boolean flag, int size, LongRange? value)
    {
      if (value.HasValue)
      {
        stream.WriteInt64(value.Value.Index);
        stream.WriteInt64(value.Value.Count);
      }
    }

    #endregion
  }
}
