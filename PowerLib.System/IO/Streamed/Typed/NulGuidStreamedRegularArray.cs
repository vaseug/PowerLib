﻿using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections.Matching;

namespace PowerLib.System.IO.Streamed.Typed
{
  public class NulGuidStreamedRegularArray : StoredFlaggedStreamedRegularArray<Guid?, Boolean>
  {
    #region Constructors

    public NulGuidStreamedRegularArray(Stream stream, SizeEncoding countSizing, bool compact, Array array, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, 16, compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Guid>.Default.Equals, array, leaveOpen, boostAccess)
    {
    }

    public NulGuidStreamedRegularArray(Stream stream, SizeEncoding countSizing, bool compact, int[] lengths, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, 16, compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Guid>.Default.Equals, lengths, null, leaveOpen, boostAccess)
    {
    }

    public NulGuidStreamedRegularArray(Stream stream, SizeEncoding countSizing, bool compact, int[] lengths, ICollection<Guid?> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, SizeEncoding.B1, 16, compact, SizeEncoding.B1, sizeof(Byte), v => v.HasValue, f => f,
          ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Guid>.Default.Equals, lengths, coll, leaveOpen, boostAccess)
    {
    }

    public NulGuidStreamedRegularArray(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, v => v.HasValue, f => f, ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, NullableEqualityComparer<Guid>.Default.Equals, leaveOpen, boostAccess)
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

    private static Guid? ReadItemData(Stream stream, Boolean flag, int size)
    {
      if (!flag)
        return default(Guid?);
      else
        return stream.ReadGuid();
    }

    private static void WriteItemData(Stream stream, Boolean flag, int size, Guid? value)
    {
      if (value.HasValue)
        stream.WriteGuid(value.Value);
    }

    #endregion
  }
}
