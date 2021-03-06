﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PowerLib.System.Collections;
using PowerLib.System.Linq;

namespace PowerLib.System.IO.Streamed.Typed
{
  public class StringStreamedRegularArray : StoredFlaggedStreamedRegularArray<String, Boolean>
  {
    #region Constructors

    public StringStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Encoding encoding, Equality<String> equality, Array array, bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, encoding, equality, array.GetRegularArrayLengths(), array.EnumerateAsRegular<String>().Counted(array.Length), leaveOpen, boostAccess)
    {
    }

    public StringStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Encoding encoding, Equality<String> equality, int[] lengths, bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, encoding, equality, lengths, null, leaveOpen, boostAccess)
    {
    }

    public StringStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Encoding encoding, Equality<String> equality, int[] lengths, ICollection<String> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, itemSizing, v => GetDataSize(encoding, v), SizeEncoding.B1, sizeof(Byte), v => v != null, f => f,
          ReadItemFlag, WriteItemFlag, (s, f, l) => ReadItemData(s, encoding, f, l), (s, f, l, v) => WriteItemData(s, encoding, f, l, v), equality,
          new RegularArrayInfo(lengths), null, leaveOpen, boostAccess, false)
    {
      Encoding = encoding;
      BaseStream.Position = 0;
      Initialize(coll ?? default(String).Repeat(ArrayInfo.Length));
    }

    public StringStreamedRegularArray(Stream stream, Equality<String> equality, bool leaveOpen, bool boostAccess)
      : this(stream, new StringArrayParameter().With(h => { stream.Position = 0; h.Read(stream); }), equality, leaveOpen, boostAccess)
    {
    }

    protected StringStreamedRegularArray(Stream stream, StringArrayParameter header, Equality<String> equality, bool leaveOpen, bool boostAccess)
      : base(stream, header, v => GetDataSize(header.Encoding, v), v => v != null, f => f,
          ReadItemFlag, WriteItemFlag, (s, f, l) => ReadItemData(s, header.Encoding, f, l), (s, f, l, v) => WriteItemData(s, header.Encoding, f, l, v), equality,
          leaveOpen, boostAccess)
    {
      Encoding = header.Encoding;
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

    public Encoding Encoding
    {
      get;
      private set;
    }

    protected override int HeaderOffset
    {
      get
      {
        return base.HeaderOffset + sizeof(ushort);
      }
    }

    #endregion
    #region Methods

    protected override IStreamable GetInitializer()
    {
      return new StringArrayParameter()
      {
        CountSizing = CountSizing,
        ItemSizing = ItemSizing,
        FlagSizing = FlagSizing,
        DataSize = FixedDataSize,
        FlagSize = FlagSize,
        Compact = Compact,
        Lengths = ArrayInfo.Lengths.ToArray(),
        Encoding = Encoding
      };
    }

    private static int GetDataSize(Encoding encoding, String value)
    {
      return value != null ? encoding.GetByteCount(value) : 0;
    }

    private static Boolean ReadItemFlag(Stream stream)
    {
      return stream.ReadBoolean(TypeCode.Byte);
    }

    private static void WriteItemFlag(Stream stream, Boolean flag)
    {
      stream.WriteBoolean(flag, TypeCode.Byte, false);
    }

    private static String ReadItemData(Stream stream, Encoding encoding, Boolean flag, int size)
    {
      return flag ? encoding.GetString(stream.ReadBytes(size)) : null;
    }

    private static void WriteItemData(Stream stream, Encoding encoding, Boolean flag, int size, String value)
    {
      if (flag && value != null)
        stream.WriteBytes(encoding.GetBytes(value));
    }

    #endregion
  }
}
