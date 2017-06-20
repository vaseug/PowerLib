using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PowerLib.System.Collections;
using PowerLib.System.Linq;

namespace PowerLib.System.IO.Streamed
{
  public class StoredStreamedRegularArray<T> : CustomStreamedRegularArray<T>
  {
    #region Constructors

    public StoredStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality,
      Array array, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, itemSizing, dataSize, dataReader, dataWriter, itemEquality, array, leaveOpen, boostAccess, true)
    {
    }

    public StoredStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality,
      Array array, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, itemSizing, dataSizer, dataReader, dataWriter, itemEquality, array, leaveOpen, boostAccess, true)
    {
    }

    public StoredStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality,
      int[] lengths, ICollection<T> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, itemSizing, dataSize, dataReader, dataWriter, itemEquality, lengths, coll, leaveOpen, boostAccess, true)
    {
    }

    public StoredStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality,
      int[] lengths, ICollection<T> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, itemSizing, dataSizer, dataReader, dataWriter, itemEquality, lengths, coll, leaveOpen, boostAccess, true)
    {
    }

    public StoredStreamedRegularArray(Stream stream,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality, bool leaveOpen, bool boostAccess)
      : this(stream, new RegularArrayParameter().With(h => { stream.Position = 0; h.Read(stream); }), dataReader, dataWriter, itemEquality, leaveOpen, boostAccess)
    {
    }

    public StoredStreamedRegularArray(Stream stream, Func<T, int> dataSizer,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality, bool leaveOpen, bool boostAccess)
      : this(stream, new RegularArrayParameter().With(h => { stream.Position = 0; h.Read(stream); }), dataSizer, dataReader, dataWriter, itemEquality, leaveOpen, boostAccess)
    {
    }

    protected StoredStreamedRegularArray(Stream stream, RegularArrayParameter header,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality, bool leaveOpen, bool boostAccess)
      : base(stream, header.CountSizing, header.ItemSizing, header.DataSize.Value, dataReader, dataWriter, itemEquality, header.Lengths, null, leaveOpen, boostAccess, false)
    {
    }

    protected StoredStreamedRegularArray(Stream stream, RegularArrayParameter header, Func<T, int> dataSizer,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality, bool leaveOpen, bool boostAccess)
      : base(stream, header.CountSizing, header.ItemSizing, dataSizer, dataReader, dataWriter, itemEquality, header.Lengths, null, leaveOpen, boostAccess, false)
    {
    }

    protected StoredStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality,
      RegularArrayInfo arrayInfo, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, countSizing, itemSizing, dataSize, dataReader, dataWriter, itemEquality, arrayInfo, coll, leaveOpen, boostAccess, initialize)
    {
    }

    protected StoredStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality,
      RegularArrayInfo arrayInfo, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, countSizing, itemSizing, dataSizer, dataReader, dataWriter, itemEquality, arrayInfo, coll, leaveOpen, boostAccess, initialize)
    {
    }

    #endregion
    #region Properties

    protected override int HeaderOffset
    {
      get
      {
        return sizeof(byte) + (FixedDataSize.HasValue ? PwrBitConverter.GetSizeEncodingSize(ItemSizing, int.MaxValue) : 0) +
          sizeof(byte) + PwrBitConverter.GetSizeEncodingSize(CountSizing, int.MaxValue) * ArrayInfo.Rank;
      }
    }

    #endregion
    #region Methods

    protected override IStreamable GetInitializer()
    {
      return new RegularArrayParameter(Endian)
      {
        CountSizing = CountSizing,
        ItemSizing = ItemSizing,
        DataSize = FixedDataSize,
        Lengths = ArrayInfo.Lengths.ToArray()
      };
    }

    #endregion
  }
}
