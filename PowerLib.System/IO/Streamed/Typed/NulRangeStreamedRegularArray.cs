using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using PowerLib.System.Collections.Matching;

namespace PowerLib.System.IO.Streamed.Typed
{
  public class NulRangeStreamedRegularArray : StoredFlaggedStreamedRegularArray<Range?, Boolean>
  {
    #region Constructors

    public NulRangeStreamedRegularArray(Stream stream, SizeEncoding countSizing, bool compact, Array array, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, sizeof(int) * 2, compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Range>.Default.Equals, array, leaveOpen, boostAccess)
    {
    }

    public NulRangeStreamedRegularArray(Stream stream, SizeEncoding countSizing, bool compact, int[] lengths, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, sizeof(int) * 2, compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Range>.Default.Equals, lengths, null, leaveOpen, boostAccess)
    {
    }

    public NulRangeStreamedRegularArray(Stream stream, SizeEncoding countSizing, bool compact, int[] lengths, ICollection<Range?> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, sizeof(int) * 2, compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Range>.Default.Equals, lengths, coll, leaveOpen, boostAccess)
    {
    }

    public NulRangeStreamedRegularArray(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, v => v.HasValue, f => f, ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Range>.Default.Equals, leaveOpen, boostAccess)
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

    private static Range? ReadItemData(Stream stream, Boolean flag, int size)
    {
      if (!flag)
        return default(Range?);
      else
        return new Range(stream.ReadInt32(), stream.ReadInt32());
    }

    private static void WriteItemData(Stream stream, Boolean flag, int size, Range? value)
    {
      if (value.HasValue)
      {
        stream.WriteInt32(value.Value.Index);
        stream.WriteInt32(value.Value.Count);
      }
    }

    #endregion
  }
}
