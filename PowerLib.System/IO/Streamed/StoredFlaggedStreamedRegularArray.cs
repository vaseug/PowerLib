using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PowerLib.System.Collections;
using PowerLib.System.Linq;

namespace PowerLib.System.IO.Streamed
{
  public class StoredFlaggedStreamedRegularArray<T, F> : CustomFlaggedStreamedRegularArray<T, F>
  {
    private SizeEncoding _flagSizing;

    #region Constructors

    public StoredFlaggedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, SizeEncoding flagSizing, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      Array array, bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, dataSize, compact, flagSizing, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality,
          array.GetRegularArrayLengths(), array.EnumerateAsRegular<T>().Counted(array.Length), leaveOpen, boostAccess)
    {
    }

    public StoredFlaggedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, SizeEncoding flagSizing, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      Array array, bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, dataSizer, flagSizing, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality,
          array.GetRegularArrayLengths(), array.EnumerateAsRegular<T>().Counted(array.Length), leaveOpen, boostAccess)
    {
    }

    public StoredFlaggedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, SizeEncoding flagSizing, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      int[] lengths, ICollection<T> coll, bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, dataSize, compact, flagSizing, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality,
          new RegularArrayInfo(lengths), coll, leaveOpen, boostAccess, true)
    {
    }

    public StoredFlaggedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, SizeEncoding flagSizing, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      int[] lengths, ICollection<T> coll, bool leaveOpen, bool boostAccess)
      : this(stream, countSizing, itemSizing, dataSizer, flagSizing, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality,
          new RegularArrayInfo(lengths), coll, leaveOpen, boostAccess, true)
    {
    }

    public StoredFlaggedStreamedRegularArray(Stream stream,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, new FlaggedRegularArrayParameter().With(h => { stream.Position = 0; h.Read(stream); }), itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, leaveOpen, boostAccess)
    {
    }

    public StoredFlaggedStreamedRegularArray(Stream stream, Func<T, int> dataSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, new FlaggedRegularArrayParameter().With(h => { stream.Position = 0; h.Read(stream); }), dataSizer, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, leaveOpen, boostAccess)
    {
    }

    protected StoredFlaggedStreamedRegularArray(Stream stream, FlaggedRegularArrayParameter header,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, header.CountSizing, header.ItemSizing, header.DataSize.Value, header.Compact, header.FlagSizing, header.FlagSize.Value,
          itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, new RegularArrayInfo(header.Lengths), null, leaveOpen, boostAccess, false)
    {
    }

    protected StoredFlaggedStreamedRegularArray(Stream stream, FlaggedRegularArrayParameter header, Func<T, int> dataSizer,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      bool leaveOpen, bool boostAccess)
      : this(stream, header.CountSizing, header.ItemSizing, dataSizer, header.FlagSizing, header.FlagSize.Value,
          itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, new RegularArrayInfo(header.Lengths), null, leaveOpen, boostAccess, false)
    {
    }

    protected StoredFlaggedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, int dataSize, bool compact, SizeEncoding flagSizing, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      RegularArrayInfo arrayInfo, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, countSizing, itemSizing, dataSize, compact, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, arrayInfo, coll, leaveOpen, boostAccess, false)
    {
      if (flagSize < 0)
        throw new ArgumentOutOfRangeException("flagSize");

      _flagSizing = flagSizing;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(ArrayInfo.Length));
      }
    }

    protected StoredFlaggedStreamedRegularArray(Stream stream, SizeEncoding countSizing, SizeEncoding itemSizing, Func<T, int> dataSizer, SizeEncoding flagSizing, int flagSize,
      Func<T, F> itemFlag, Func<F, bool> hasData, Func<Stream, F> flagReader, Action<Stream, F> flagWriter, Func<Stream, F, int, T> dataReader, Action<Stream, F, int, T> dataWriter, Equality<T> itemEquality,
      RegularArrayInfo arrayInfo, ICollection<T> coll, bool leaveOpen, bool boostAccess, bool initialize)
      : base(stream, countSizing, itemSizing, dataSizer, flagSize, itemFlag, hasData, flagReader, flagWriter, dataReader, dataWriter, itemEquality, arrayInfo, coll, leaveOpen, boostAccess, false)
    {
      if (flagSize < 0)
        throw new ArgumentOutOfRangeException("flagSize");

      _flagSizing = flagSizing;
      if (initialize)
      {
        BaseStream.Position = 0;
        Initialize(coll ?? default(T).Repeat(ArrayInfo.Length));
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
          sizeof(byte) + (FixedDataSize.HasValue ? PwrBitConverter.GetSizeEncodingSize(ItemSizing, int.MaxValue) : 0) +
          sizeof(byte) + PwrBitConverter.GetSizeEncodingSize(FlagSizing, int.MaxValue) +
          sizeof(byte) + PwrBitConverter.GetSizeEncodingSize(CountSizing, int.MaxValue) * ArrayInfo.Rank;
      }
    }

    #endregion
    #region Methods

    protected override IStreamable GetInitializer()
    {
      return new FlaggedRegularArrayParameter()
      {
        CountSizing = CountSizing,
        ItemSizing = ItemSizing,
        FlagSizing = FlagSizing,
        DataSize = FixedDataSize,
        FlagSize = FlagSize,
        Compact = Compact,
        Lengths = ArrayInfo.Lengths.ToArray()
      };
    }

    #endregion
  }
}
