using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections.Matching;

namespace PowerLib.System.IO.Streamed.Typed
{
  public class NulBooleanStreamedArray : StoredStreamedArray<Boolean?>
  {
    #region Constructors

    public NulBooleanStreamedArray(Stream stream, SizeEncoding countSizing, ICollection<Boolean?> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, sizeof(byte), ReadItemData, WriteItemData, NullableEqualityComparer<Boolean>.Default.Equals, coll, leaveOpen, boostAccess)
    {
    }

    public NulBooleanStreamedArray(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, ReadItemData, WriteItemData, NullableEqualityComparer<Boolean>.Default.Equals, leaveOpen, boostAccess)
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

    #endregion
    #region Methods

    private static Boolean? ReadItemData(Stream stream)
    {
      switch (stream.ReadUByte())
      {
        case 0:
          return default(Boolean?);
        case 1:
          return false;
        case 2:
          return true;
        default:
          throw new FormatException();
      }
    }

    private static void WriteItemData(Stream stream, Boolean? value)
    {
      if (!value.HasValue)
        stream.WriteUByte(0);
      else if (!value.Value)
        stream.WriteUByte(1);
      else
        stream.WriteUByte(2);
    }

    #endregion
  }
}
