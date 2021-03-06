﻿using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections;
using PowerLib.System.Linq;

namespace PowerLib.System.IO.Streamed
{
  public class StoredFlaggedStreamedCollection<T, F> : CustomFlaggedStreamedCollection<T, F>
  {
    private SizeEncoding _flagSizing;

    #region Constructors

    public StoredFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, SizeEncoding flagSizing, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, dataSize, compact, flagSizing, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, true)
    {
    }

    public StoredFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, SizeEncoding flagSizing, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, dataSize, compact, flagSizing, flagSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, true)
    {
    }

    public StoredFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, SizeEncoding flagSizing, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, dataSizer, flagSizing, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, true)
    {
    }

    public StoredFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, SizeEncoding flagSizing, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, dataSizer, flagSizing, flagSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, true)
    {
    }

    public StoredFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, SizeEncoding flagSizing, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      ICollection<T> coll, bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, dataSize, compact, flagSizing, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, coll, leaveOpen, boostAccess, true)
    {
    }

    public StoredFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, SizeEncoding flagSizing, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      ICollection<T> coll, bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, dataSize, compact, flagSizing, flagSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, coll, leaveOpen, boostAccess, true)
    {
    }

    public StoredFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, SizeEncoding flagSizing, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      ICollection<T> coll, bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, dataSizer, flagSizing, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, coll, leaveOpen, boostAccess, true)
    {
    }

    public StoredFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, SizeEncoding flagSizing, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      ICollection<T> coll, bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, dataSizer, flagSizing, flagSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, coll, leaveOpen, boostAccess, true)
    {
    }

    public StoredFlaggedStreamedCollection(Stream stream,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, new FlaggedCollectionParameter().With(h => { stream.Position = 0; h.Read(stream); }), itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, leaveOpen, boostAccess)
    {
    }

    public StoredFlaggedStreamedCollection(Stream stream, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, new FlaggedCollectionParameter().With(h => { stream.Position = 0; h.Read(stream); }), flagSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, leaveOpen, boostAccess)
    {
    }

    public StoredFlaggedStreamedCollection(Stream stream, Func<T, int> dataSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, new FlaggedCollectionParameter().With(h => { stream.Position = 0; h.Read(stream); }), dataSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, leaveOpen, boostAccess)
    {
    }

    public StoredFlaggedStreamedCollection(Stream stream, Func<T, int> dataSizer, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, new FlaggedCollectionParameter().With(h => { stream.Position = 0; h.Read(stream); }), dataSizer, flagSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, leaveOpen, boostAccess)
    {
    }

    protected StoredFlaggedStreamedCollection(Stream stream, FlaggedCollectionParameter header,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, header.CountSizing, header.ItemSizing, header.DataSize.Value, header.Compact, header.FlagSizing, header.FlagSize.Value,
          itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, false)
    {
    }

    protected StoredFlaggedStreamedCollection(Stream stream, FlaggedCollectionParameter header, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, header.CountSizing, header.ItemSizing, header.DataSize.Value, header.Compact, header.FlagSizing,
          flagSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, false)
    {
    }

    protected StoredFlaggedStreamedCollection(Stream stream, FlaggedCollectionParameter header, Func<T, int> dataSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, header.CountSizing, header.ItemSizing, dataSizer, header.FlagSizing, header.FlagSize.Value,
          itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, false)
    {
    }

    protected StoredFlaggedStreamedCollection(Stream stream, FlaggedCollectionParameter header, Func<T, int> dataSizer, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, header.CountSizing, header.ItemSizing, dataSizer, header.FlagSizing,
          flagSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, false)
    {
    }

    protected StoredFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, SizeEncoding flagSizing, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, countSizing, itemSizing, dataSize, compact, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, coll, leaveOpen, boostAccess, false)
    {
      if (flagSize < 0)
        throw new ArgumentOutOfRangeException("flagSize");

      _flagSizing = flagSizing;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(0));
      }
    }

    protected StoredFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, SizeEncoding flagSizing, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, countSizing, itemSizing, dataSize, compact, flagSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, coll, leaveOpen, boostAccess, false)
    {
      if (flagSizer == null)
        throw new ArgumentNullException("flagSizer");

      _flagSizing = flagSizing;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(0));
      }
    }

    protected StoredFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, SizeEncoding flagSizing, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, countSizing, itemSizing, dataSizer, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, coll, leaveOpen, boostAccess, false)
    {
      if (flagSize < 0)
        throw new ArgumentOutOfRangeException("flagSize");

      _flagSizing = flagSizing;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(0));
      }
    }

    protected StoredFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, SizeEncoding flagSizing, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, countSizing, itemSizing, dataSizer, flagSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, coll, leaveOpen, boostAccess, false)
    {
      if (flagSizer == null)
        throw new ArgumentNullException("flagSizer");

      _flagSizing = flagSizing;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(0));
      }
    }

    #endregion
    #region Properties

    protected virtual SizeEncoding FlagSizing
    {
      get { return _flagSizing; }
    }

    protected override int HeaderOffset
    {
      get
      {
        return base.HeaderOffset +
          sizeof(byte) + (FixedDataSize.HasValue ? PwrBitConverter.GetSizeEncodingSize(int.MaxValue, ItemSizing) : 0) +
          sizeof(byte) + (FixedFlagSize.HasValue ? PwrBitConverter.GetSizeEncodingSize(int.MaxValue, FlagSizing) : 0);
      }
    }

    #endregion
    #region Methods

    protected override IStreamable GetInitializer()
    {
      return new FlaggedCollectionParameter()
      {
        CountSizing = CountSizing,
        ItemSizing = ItemSizing,
        FlagSizing = FlagSizing,
        DataSize = FixedDataSize,
        FlagSize = FixedFlagSize,
        Compact = Compact
      };
    }

    #endregion
  }
}
