using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using PowerLib.System.Collections;

namespace PowerLib.System.IO.Streamed
{
  public class CustomStreamedArray<T> : BaseStreamedArray<T>
  {
    private SizeEncoding _countSizing;
    private SizeEncoding _itemSizing;
    private int? _dataSize;
    private Func<T, int> _dataSizer;
    private Func<Stream, int, T> _dataReader;
    private Action<Stream, int, T> _dataWriter;
    private Equality<T> _dataEquality;

    #region Constructors

    public CustomStreamedArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, Func<Stream, T> dataReader,
      Action<Stream, T> dataWriter, Equality<T> itemEquality, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, leaveOpen, boostAccess)
    {
      if (dataReader == null)
        throw new ArgumentNullException("dataReader");
      if (dataWriter == null)
        throw new ArgumentNullException("dataWriter");

      _countSizing = countSizing;
      _itemSizing = itemSizing;
      _dataSize = dataSize;
      _dataSizer = t => dataSize;
      _dataReader = (s, l) => dataReader(s);
      _dataWriter = (s, l, t) => dataWriter(s, t);
      _dataEquality = itemEquality != null ? itemEquality : EqualityComparer<T>.Default.Equals;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(0));
      }
    }

    public CustomStreamedArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, Func<Stream, int, T> dataReader,
      Action<Stream, int, T> dataWriter, Equality<T> itemEquality, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, leaveOpen, boostAccess)
    {
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
