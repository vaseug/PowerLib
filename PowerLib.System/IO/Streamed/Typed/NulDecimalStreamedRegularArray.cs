using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections.Matching;

namespace PowerLib.System.IO.Streamed.Typed
{
  public class NulDecimalStreamedRegularArray : StoredFlaggedStreamedRegularArray<Decimal?, Boolean>
  {
    #region Constructors

    public NulDecimalStreamedRegularArray(Stream stream, SizeEncoding countSizing, bool compact, Array array, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, 16, compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Decimal>.Default.Equals, array, leaveOpen, boostAccess)
    {
    }

    public NulDecimalStreamedRegularArray(Stream stream, SizeEncoding countSizing, bool compact, int[] lenghts, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, 16, compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Decimal>.Default.Equals, lenghts, null, leaveOpen, boostAccess)
    {
    }

    public NulDecimalStreamedRegularArray(Stream stream, SizeEncoding countSizing, bool compact, int[] lenghts, ICollection<Decimal?> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, 16, compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Decimal>.Default.Equals, lenghts, coll, leaveOpen, boostAccess)
    {
    }

    public NulDecimalStreamedRegularArray(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, v => v.HasValue, f => f, ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Decimal>.Default.Equals, leaveOpen, boostAccess)
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

    private static Decimal? ReadItemData(Stream stream, Boolean flag, int size)
    {
      if (!flag)
        return default(Decimal?);
      else
        return stream.ReadDecimal();
    }

    private static void WriteItemData(Stream stream, Boolean flag, int size, Decimal? value)
    {
      if (value.HasValue)
        stream.WriteDecimal(value.Value);
    }

    #endregion
  }
}
