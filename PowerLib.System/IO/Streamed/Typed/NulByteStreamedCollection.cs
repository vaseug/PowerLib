using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections.Matching;

namespace PowerLib.System.IO.Streamed.Typed
{
  public class NulByteStreamedCollection : StoredFlaggedStreamedCollection<Byte?, Boolean>
  {
    #region Constructors

    public NulByteStreamedCollection(Stream stream, SizeEncoding countSizing, bool compact, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, sizeof(Byte), compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Byte>.Default.Equals, leaveOpen, boostAccess)
    {
    }

    public NulByteStreamedCollection(Stream stream, SizeEncoding countSizing, bool compact, ICollection<Byte?> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, sizeof(Byte), compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Byte>.Default.Equals, coll, leaveOpen, boostAccess)
    {
    }

    public NulByteStreamedCollection(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, v => v.HasValue, f => f, ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Byte>.Default.Equals, leaveOpen, boostAccess)
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

    private static Byte? ReadItemData(Stream stream, Boolean flag, int size)
    {
      if (!flag)
        return default(Byte?);
      else
        return stream.ReadUByte();
    }

    private static void WriteItemData(Stream stream, Boolean flag, int size, Byte? value)
    {
      if (value.HasValue)
        stream.WriteUByte(value.Value);
    }

    #endregion
  }
}
