using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections;

namespace PowerLib.System.IO.Streamed
{
  public class CustomFlaggedStreamedRegularArray<T, F> : BaseFlaggedStreamedRegularArray<T, F>
  {
    private SizeEncoding _countSizing;
    private SizeEncoding _itemSizing;
    private int _flagSize;
    private int? _dataSize;
    private bool _compact;
    private Func<T, int> _dataSizer;
    private Func<T, F> _itemFlag;
    private Func<F, bool> _hasData;
    private Func<Stream, F> _flagReader;
    private Action<Stream, F> _flagWriter;
    private Func<Stream, F, int, T> _dataReader;
    private Action<Stream, F, int, T> _dataWriter;
    private Equality<T> _dataEquality;
    private RegularArrayInfo _arrayInfo;

    #region Constructors

    public CustomFlaggedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      Array array, bool leaveOpen, bool boostAccess, bool initialize)
      : this(stream, countSizing, itemSizing, dataSize, compact, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality,
          array.GetRegularArrayLengths(), initialize ? array.EnumerateAsRegular<T>().Counted(array.Length) : null, leaveOpen, boostAccess, initialize)
    {
    }

    public CustomFlaggedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      Array array, bool leaveOpen, bool boostAccess, bool initialize)
      : this(stream, countSizing, itemSizing, dataSizer, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality,
          array.GetRegularArrayLengths(), initialize ? array.EnumerateAsRegular<T>().Counted(array.Length) : null, leaveOpen, boostAccess, initialize)
    {
    }

    public CustomFlaggedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      int[] lengths, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : this(stream, countSizing, itemSizing, dataSize, compact, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality,
          new RegularArrayInfo(lengths), coll, leaveOpen, boostAccess, initialize)
    {
    }

    public CustomFlaggedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      int[] lengths, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : this(stream, countSizing, itemSizing, dataSizer, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality,
          new RegularArrayInfo(lengths), coll, leaveOpen, boostAccess, initialize)
    {
    }

    protected CustomFlaggedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      RegularArrayInfo arrayInfo, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, leaveOpen, boostAccess && !compact)
    {
      if (arrayInfo == null)
        throw new ArgumentNullException("arrayInfo");
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
      _arrayInfo = arrayInfo;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(ArrayInfo.Length));
      }
    }

    protected CustomFlaggedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      RegularArrayInfo arrayInfo, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, leaveOpen, boostAccess)
    {
      if (arrayInfo == null)
        throw new ArgumentNullException("arrayInfo");
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
      _arrayInfo = arrayInfo;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(ArrayInfo.Length));
      }
    }

    #endregion
    #region Properties

    protected override RegularArrayInfo ArrayInfo
    {
      get { return _arrayInfo; }
    }

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

    protected override int FlagSize
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
      if (ArrayInfo.Length != coll.Count)
        throw new ArgumentException("Inconsistent collection size.", "coll");

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

    protected override F ReadItemFlag(int index)
    {
      return _flagReader(BaseStream);
    }

    protected override void WriteItemFlag(int index, F flag)
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
