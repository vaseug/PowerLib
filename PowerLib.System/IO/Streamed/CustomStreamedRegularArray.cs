using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections;

namespace PowerLib.System.IO.Streamed
{
  public class CustomStreamedRegularArray<T> : BaseStreamedRegularArray<T>
  {
    private SizeEncoding _countSizing;
    private SizeEncoding _itemSizing;
    private int? _dataSize;
    private Func<T, int> _dataSizer;
    private Func<Stream, int, T> _dataReader;
    private Action<Stream, int, T> _dataWriter;
    private Equality<T> _dataEquality;
    private RegularArrayInfo _arrayInfo;

    #region Constructors

    public CustomStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality,
      Array array, bool leaveOpen, bool boostAccess, bool initialize)
      : this(stream, countSizing, itemSizing, dataSize, dataReader, dataWriter, itemEquality,
          array.GetRegularArrayLengths(), initialize ? array.EnumerateAsRegular<T>().Counted(array.Length) : null, leaveOpen, boostAccess, initialize)
    {
    }

    public CustomStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality,
      Array array, bool leaveOpen, bool boostAccess, bool initialize)
      : this(stream, countSizing, itemSizing, dataSizer, dataReader, dataWriter, itemEquality,
          array.GetRegularArrayLengths(), initialize ? array.EnumerateAsRegular<T>().Counted(array.Length) : null, leaveOpen, boostAccess, initialize)
    {
    }

    public CustomStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality,
      int[] lengths, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : this(stream, countSizing, itemSizing, dataSize, dataReader, dataWriter, itemEquality,
          lengths != null ? new RegularArrayInfo(lengths) : null, coll, leaveOpen, boostAccess, initialize)
    {
    }

    public CustomStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality,
      int[] lengths, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : this(stream, countSizing, itemSizing, dataSizer, dataReader, dataWriter, itemEquality,
          lengths != null ? new RegularArrayInfo(lengths) : null, coll, leaveOpen, boostAccess, initialize)
    {
    }

    protected CustomStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality,
      RegularArrayInfo arrayInfo, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, leaveOpen, boostAccess)
    {
      if (arrayInfo == null)
        throw new ArgumentNullException("arrayInfo");
      if (dataSize < 0)
        throw new ArgumentOutOfRangeException("dataSize");
      if (dataReader == null)
        throw new ArgumentNullException("dataReader");
      if (dataWriter == null)
        throw new ArgumentNullException("dataWriter");

      _countSizing = countSizing;
      _itemSizing = itemSizing;
      _dataSize = dataSize;
      _dataSizer = t => dataSize;
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

    protected CustomStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer,
      Func<Stream, int, T> dataReader, Action<Stream, int, T> dataWriter, Equality<T> itemEquality,
      RegularArrayInfo arrayInfo, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, leaveOpen, boostAccess)
    {
      if (arrayInfo == null)
        throw new ArgumentNullException("arrayInfo");
      if (dataSizer == null)
        throw new ArgumentNullException("dataSizer");
      if (dataReader == null)
        throw new ArgumentNullException("dataReader");
      if (dataWriter == null)
        throw new ArgumentNullException("dataWriter");

      _countSizing = countSizing;
      _itemSizing = itemSizing;
      _dataSize = null;
      _dataSizer = dataSizer;
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

    protected override int GetDataSize(T value)
    {
      return _dataSize.HasValue ? _dataSize.Value : _dataSizer(value);
    }

    protected override T ReadItemData(int size)
    {
      using (var s = new ReadRestrictiveStream(BaseStream, size, true))
      {
        T value = _dataReader(s, size);
        BaseStream.Position += s.Available;
        return value;
      }
    }

    protected override void WriteItemData(T value, int size)
    {
      using (var s = new WriteRestrictiveStream(BaseStream, size, true))
      {
        _dataWriter(s, size, value);
        BaseStream.Position += s.Available;
      }
    }

    #endregion
  }
}
