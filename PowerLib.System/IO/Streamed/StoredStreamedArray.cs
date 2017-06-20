using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections;
using PowerLib.System.Linq;

namespace PowerLib.System.IO.Streamed
{
  public class StoredStreamedArray<T> : CustomStreamedArray<T>
  {
    #region Constructors

    public StoredStreamedArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize,
      Func<Stream, T> dataReader, Action<Stream, T> dataWriter, Equality<T> itemEquality, ICollection<T> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, itemSizing, dataSize, dataReader, dataWriter, itemEquality, coll, leaveOpen, boostAccess, true)
    {
    }

    public StoredStreamedArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality, ICollection<T> coll, bool leaveOpen, bool boostAccess)
      : base(stream, countSizing, itemSizing, dataSizer, dataReader, dataWriter, itemEquality, coll, leaveOpen, boostAccess, true)
    {
    }

    public StoredStreamedArray(Stream stream,
      Func<Stream, T> dataReader, Action<Stream, T> dataWriter, Equality<T> itemEquality, bool leaveOpen, bool boostAccess)
      : this(stream, new CollectionParameter().With(h => { stream.Position = 0; h.Read(stream); }), dataReader, dataWriter, itemEquality, leaveOpen, boostAccess)
    {
    }

    public StoredStreamedArray(Stream stream, Func<T, int> dataSizer,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality, bool leaveOpen, bool boostAccess)
      : this(stream, new CollectionParameter().With(h => { stream.Position = 0; h.Read(stream); }), dataSizer, dataReader, dataWriter, itemEquality, leaveOpen, boostAccess)
    {
    }

    protected StoredStreamedArray(Stream stream, CollectionParameter header,
      Func<Stream, T> dataReader, Action<Stream, T> dataWriter, Equality<T> itemEquality, bool leaveOpen, bool boostAccess)
      : base(stream, header.CountSizing, header.ItemSizing, header.DataSize.Value, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, false)
    {
    }

    protected StoredStreamedArray(Stream stream, CollectionParameter header, Func<T, int> dataSizer,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality, bool leaveOpen, bool boostAccess)
      : base(stream, header.CountSizing, header.ItemSizing, dataSizer, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, false)
    {
    }

    protected StoredStreamedArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize,
      Func<Stream, T> dataReader, Action<Stream, T> dataWriter, Equality<T> itemEquality, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, countSizing, itemSizing, dataSize, dataReader, dataWriter, itemEquality, coll, leaveOpen, boostAccess, initialize)
    {
    }

    protected StoredStreamedArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, countSizing, itemSizing, dataSizer, dataReader, dataWriter, itemEquality, coll, leaveOpen, boostAccess, initialize)
    {
    }

    #endregion
    #region Properties

    protected override int HeaderOffset
    {
      get
      {
        return sizeof(byte) + (FixedDataSize.HasValue ? PwrBitConverter.GetSizeEncodingSize(ItemSizing, int.MaxValue) : 0);
      }
    }

    #endregion
    #region Methods

    protected override IStreamable GetInitializer()
    {
      return new CollectionParameter(Endian)
      {
        CountSizing = CountSizing,
        ItemSizing = ItemSizing,
        DataSize = FixedDataSize
      };
    }

    #endregion
  }
}
