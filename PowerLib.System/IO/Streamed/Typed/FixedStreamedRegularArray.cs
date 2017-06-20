using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections.Matching;

namespace PowerLib.System.IO.Streamed.Typed
{
  public class FixedStreamedRegularArray : StoredFlaggedStreamedRegularArray<Byte[], Boolean>
  {
    private static readonly IEqualityComparer<Byte[]> EqualityComparer = new ObjectEqualityComparer<Byte[]>(new SequenceEqualityComparer<Byte>(EqualityComparer<Byte>.Default), false);

    #region Constructors

    public FixedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int itemSize, bool compact, Array array, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, itemSizing, itemSize, compact, SizeEncoding.B1, sizeof(Byte), v => v != null, f => f,
        ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, EqualityComparer.Equals, array, leaveOpen, boostAccess)
    {
    }

    public FixedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int itemSize, bool compact, int[] lengths, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, itemSizing, itemSize, compact, SizeEncoding.B1, sizeof(Byte), v => v != null, f => f,
        ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, EqualityComparer.Equals, lengths, null, leaveOpen, boostAccess)
    {
    }

    public FixedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int itemSize, bool compact, int[] lengths, ICollection<Byte[]> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, itemSizing, itemSize, compact, SizeEncoding.B1, sizeof(Byte), v => v != null, f => f,
        ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, EqualityComparer.Equals, lengths, coll, leaveOpen, boostAccess)
    {
    }

    public FixedStreamedRegularArray(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, v => v != null ? v.Length : 0, v => v != null, f => f,
        ReadItemFlag, WriteItemFlag, ReadItemData, WriteItemData, EqualityComparer.Equals, leaveOpen, boostAccess)
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

    public new SizeEncoding ItemSizing
    {
      get { return base.ItemSizing; }
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
      stream.WriteUByte((Byte)(flag ? 1 : 0));
    }

    private static Byte[] ReadItemData(Stream stream, Boolean flag, int size)
    {
      if (!flag)
        return null;
      else
        return stream.ReadBytes(size);
    }

    private static void WriteItemData(Stream stream, Boolean flag, int size, Byte[] value)
    {
      if (value != null)
        stream.WriteBytes(value);
    }

    #endregion
  }
}
