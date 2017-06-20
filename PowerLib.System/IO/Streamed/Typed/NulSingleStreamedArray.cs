using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections.Matching;

namespace PowerLib.System.IO.Streamed.Typed
{
  public class NulSingleStreamedArray : StoredFlaggedStreamedArray<Single?, Boolean>
  {
    #region Constructors

    public NulSingleStreamedArray(Stream stream, SizeEncoding countSizing, bool compact, ICollection<Single?> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, sizeof(Single), compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Single>.Default.Equals, coll, leaveOpen, boostAccess)
    {
    }

    public NulSingleStreamedArray(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, v => v.HasValue, f => f, ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Single>.Default.Equals, leaveOpen, boostAccess)
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

    private static Single? ReadItemData(Stream stream, Boolean flag, int size)
    {
      if (!flag)
        return default(Single?);
      else
        return stream.ReadSingle();
    }

    private static void WriteItemData(Stream stream, Boolean flag, int size, Single? value)
    {
      if (value.HasValue)
        stream.WriteSingle(value.Value);
    }

    #endregion
  }
}
