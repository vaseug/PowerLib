using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections;

namespace PowerLib.System.IO.Streamed
{
  public class CustomFlaggedStreamedCollection<T, F> : BaseFlaggedStreamedCollection<T, F>
  {
    private SizeEncoding _countSizing;
    private SizeEncoding _itemSizing;
    private int? _flagSize;
    private int? _dataSize;
    private bool _compact;
    private Func<F, int> _flagSizer;
    private Func<T, int> _dataSizer;
    private Func<T, F> _itemFlag;
    private Func<F, bool> _hasData;
    private Func<Stream, F> _flagReader;
    private Action<Stream, F> _flagWriter;
    private Func<Stream, F, int, T> _dataReader;
    private Action<Stream, F, int, T> _dataWriter;
    private Equality<T> _dataEquality;

    #region Constructors

    public CustomFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess, bool initialize)
      : this(stream, countSizing, itemSizing, dataSize, compact, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, initialize)
    {
    }

    public CustomFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess, bool initialize)
      : this(stream, countSizing, itemSizing, dataSize, compact, flagSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, initialize)
    {
    }

    public CustomFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess, bool initialize)
      : this(stream, countSizing, itemSizing, dataSizer, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, initialize)
    {
    }

    public CustomFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess, bool initialize)
      : this(stream, countSizing, itemSizing, dataSizer, flagSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, null, leaveOpen, boostAccess, initialize)
    {
    }

    public CustomFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, leaveOpen, boostAccess && !compact)
    {
      if (dataSize < 0)
        throw new ArgumentOutOfRangeException("dataSize");
      if (flagSize < 0)
        throw new ArgumentOutOfRangeException("flagSize");
      if (itemFlag == null)
        throw new ArgumentNullException("itemFlag");
      if (hasData == null)
        throw new ArgumentNullException("hasData");
      if (flagReader == null)
        throw new ArgumentNullException("flagReader");
      if (flagWriter == null)
        throw new ArgumentNullException("flagWriter");
      if (dataReader == null)
        throw new ArgumentNullException("dataReader");
      if (dataWriter == null)
        throw new ArgumentNullException("dataWriter");

      _countSizing = countSizing;
      _itemSizing = itemSizing;
      _flagSize = flagSize;
      _flagSizer = t => flagSize;
      _dataSize = dataSize;
      _dataSizer = t => dataSize;
      _compact = compact;
      _itemFlag = itemFlag;
      _hasData = hasData;
      _flagReader = flagReader;
      _flagWriter = flagWriter;
      _dataReader = dataReader;
      _dataWriter = dataWriter;
      _dataEquality = itemEquality != null ? itemEquality : EqualityComparer<T>.Default.Equals;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(0));
      }
    }

    public CustomFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, leaveOpen, boostAccess)
    {
      if (dataSize < 0)
        throw new ArgumentOutOfRangeException("dataSize");
      if (flagSizer == null)
        throw new ArgumentNullException("flagSizer");
      if (itemFlag == null)
        throw new ArgumentNullException("itemFlag");
      if (hasData == null)
        throw new ArgumentNullException("hasData");
      if (flagReader == null)
        throw new ArgumentNullException("flagReader");
      if (flagWriter == null)
        throw new ArgumentNullException("flagWriter");
      if (dataReader == null)
        throw new ArgumentNullException("dataReader");
      if (dataWriter == null)
        throw new ArgumentNullException("dataWriter");

      _countSizing = countSizing;
      _itemSizing = itemSizing;
      _flagSize = null;
      _flagSizer = flagSizer;
      _dataSize = dataSize;
      _dataSizer = t => dataSize;
      _compact = compact;
      _itemFlag = itemFlag;
      _hasData = hasData;
      _flagReader = flagReader;
      _flagWriter = flagWriter;
      _dataReader = dataReader;
      _dataWriter = dataWriter;
      _dataEquality = itemEquality != null ? itemEquality : EqualityComparer<T>.Default.Equals;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(0));
      }
    }

    public CustomFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, leaveOpen, boostAccess)
    {
      if (dataSizer == null)
        throw new ArgumentNullException("dataSizer");
      if (flagSize < 0)
        throw new ArgumentOutOfRangeException("flagSize");
      if (itemFlag == null)
        throw new ArgumentNullException("itemFlag");
      if (hasData == null)
        throw new ArgumentNullException("hasData");
      if (flagReader == null)
        throw new ArgumentNullException("flagReader");
      if (flagWriter == null)
        throw new ArgumentNullException("flagWriter");
      if (dataReader == null)
        throw new ArgumentNullException("dataReader");
      if (dataWriter == null)
        throw new ArgumentNullException("dataWriter");

      _countSizing = countSizing;
      _itemSizing = itemSizing;
      _flagSize = flagSize;
      _flagSizer = t => flagSize;
      _dataSize = null;
      _dataSizer = dataSizer;
      _compact = false;
      _itemFlag = itemFlag;
      _hasData = hasData;
      _flagReader = flagReader;
      _flagWriter = flagWriter;
      _dataReader = dataReader;
      _dataWriter = dataWriter;
      _dataEquality = itemEquality != null ? itemEquality : EqualityComparer<T>.Default.Equals;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(0));
      }
    }

    public CustomFlaggedStreamedCollection(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, Func<F, int> flagSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, leaveOpen, boostAccess)
    {
      if (dataSizer == null)
        throw new ArgumentNullException("dataSizer");
      if (flagSizer == null)
        throw new ArgumentNullException("flagSizer");
      if (itemFlag == null)
        throw new ArgumentNullException("itemFlag");
      if (hasData == null)
        throw new ArgumentNullException("hasData");
      if (flagReader == null)
        throw new ArgumentNullException("flagReader");
      if (flagWriter == null)
        throw new ArgumentNullException("flagWriter");
      if (dataReader == null)
        throw new ArgumentNullException("dataReader");
      if (dataWriter == null)
        throw new ArgumentNullException("dataWriter");

      _countSizing = countSizing;
      _itemSizing = itemSizing;
      _flagSize = null;
      _flagSizer = flagSizer;
      _dataSize = null;
      _dataSizer = dataSizer;
      _compact = false;
      _itemFlag = itemFlag;
      _hasData = hasData;
      _flagReader = flagReader;
      _flagWriter = flagWriter;
      _dataReader = dataReader;
      _dataWriter = dataWriter;
      _dataEquality = itemEquality != null ? itemEquality : EqualityComparer<T>.Default.Equals;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(0));
      }
    }

    #endregion
    #region Properties

    protected override SizeEncoding CountSizing
    {
      get { return _countSizing; }
    }

    protected override SizeEncoding ItemSizing
    {
      get { return _itemSizing; }
    }

    protected override bool Compact
    {
      get { return _compact; }
    }

    protected override int? FixedFlagSize
    {
      get { return _flagSize; }
    }

    protected override int? FixedDataSize
    {
      get { return _dataSize; }
    }

    #endregion
    #region Methods

    protected virtual IStreamable GetInitializer()
    {
      return null;
    }

    protected virtual void Initialize(ICollection<T> coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      var initializer = GetInitializer();
      if (initializer != null)
        initializer.Write(BaseStream);
      WriteCount(coll.Count);
      BaseStream.SetLength(DataOffset);
      BaseStream.Position = DataOffset;
      int i = 0;
      foreach (var item in coll)
        WriteItem(i++, item);
    }

    protected override bool EqualItem(T x, T y)
    {
      return _dataEquality(x, y);
    }

    protected override F GetItemFlag(T value)
    {
      return _itemFlag(value);
    }

    protected override bool HasItemData(F flag)
    {
      return _hasData(flag);
    }

    protected override int GetDataSize(T value)
    {
      return _dataSizer != null ? _dataSizer(value) : base.GetDataSize(value);
    }

    protected override int GetFlagSize(F flag)
    {
      return _flagSizer != null ? _flagSizer(flag) : base.GetFlagSize(flag);
    }

    protected override F ReadItemFlag()
    {
      return _flagReader(BaseStream);
    }

    protected override void WriteItemFlag(F flag)
    {
      _flagWriter(BaseStream, flag);
    }

    protected override T ReadItemData(F flag, int size)
    {
      using (var s = new ReadRestrictiveStream(BaseStream, size, true))
      {
        T value = _dataReader(s, flag, size);
        BaseStream.Position += s.Available;
        return value;
      }
    }

    protected override void WriteItemData(T value, F flag, int size)
    {
      using (var s = new WriteRestrictiveStream(BaseStream, size, true))
      {
        _dataWriter(s, flag, size, value);
        BaseStream.Position += s.Available;
      }
    }

    #endregion
  }
}
