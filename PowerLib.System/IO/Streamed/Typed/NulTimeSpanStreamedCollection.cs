using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections.Matching;

namespace PowerLib.System.IO.Streamed.Typed
{
  public class NulTimeSpanStreamedCollection : StoredFlaggedStreamedCollection<TimeSpan?, Boolean>
  {
    #region Constructors

    public NulTimeSpanStreamedCollection(Stream stream, SizeEncoding countSizing, bool compact, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, sizeof(Int64), compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<TimeSpan>.Default.Equals, leaveOpen, boostAccess)
    {
    }

    public NulTimeSpanStreamedCollection(Stream stream, SizeEncoding countSizing, bool compact, ICollection<TimeSpan?> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, sizeof(Int64), compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<TimeSpan>.Default.Equals, coll, leaveOpen, boostAccess)
    {
    }

    public NulTimeSpanStreamedCollection(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, v => v.HasValue, f => f, ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<TimeSpan>.Default.Equals, leaveOpen, boostAccess)
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

    private static TimeSpan? ReadItemData(Stream stream, Boolean flag, int size)
    {
      if (!flag)
        return default(TimeSpan?);
      else
        return stream.ReadTimeSpan();
    }

    private static void WriteItemData(Stream stream, Boolean flag, int size, TimeSpan? value)
    {
      if (value.HasValue)
        stream.WriteTimeSpan(value.Value);
    }

    #endregion
  }
}
