using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PowerLib.System.IO.Streamed
{
  public abstract class BaseStreamedArray<T> : StreamedCollection<T>
  {
    #region Constructors

    protected BaseStreamedArray(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, leaveOpen, boostAccess)
    {
    }

    #endregion
    #region Properties
    #region Internal properties

    protected virtual int HeaderOffset
    {
      get { return 0; }
    }

    protected override int DataOffset
    {
      get { return HeaderOffset + PwrBitConverter.GetSizeEncodingSize(int.MaxValue, CountSizing) + AllocItemSize * Count; }
    }

    protected virtual int AllocItemSize
    {
      get { return FixedItemSize.HasValue ? 0 : PwrBitConverter.GetSizeEncodingSize(int.MaxValue, ItemSizing); }
    }

    protected abstract int? FixedDataSize { get; }

    protected override int? FixedItemSize
    {
      get { return FixedDataSize.HasValue ? FixedDataSize.Value : default(int?); }
    }

    protected virtual Endian Endian
    {
      get { return Endian.Default; }
    }

    protected abstract SizeEncoding CountSizing { get; }

    protected abstract SizeEncoding ItemSizing { get; }

    #endregion
    #endregion
    #region Methods
    #region Internal methods

    protected override int ReadCount()
    {
      int countSize = PwrBitConverter.GetSizeEncodingSize(int.MaxValue, CountSizing);
      long countOffset = BaseStream.Position = HeaderOffset;
      int count = BaseStream.ReadSize(CountSizing, Endian);
      int rest = countSize - (int)(BaseStream.Position - countOffset);
      if (rest > 0)
        BaseStream.Position += rest;
      return count;
    }

    protected override void WriteCount(int count)
    {
      int countSize = PwrBitConverter.GetSizeEncodingSize(int.MaxValue, CountSizing);
      long countOffset = BaseStream.Position = HeaderOffset;
      BaseStream.WriteSize(count, CountSizing, Endian);
      int rest = countSize - (int)(BaseStream.Position - countOffset);
      if (rest > 0)
        BaseStream.WriteCollection(Enumerable.Repeat((byte)0, rest), (s, v) => s.WriteUByte(v));
    }

    protected virtual int GetDataSize(T value)
    {
      if (FixedDataSize.HasValue)
        return FixedDataSize.Value;
      throw new NotImplementedException();
    }

    protected override int GetItemSize(T value)
    {
      return FixedDataSize.HasValue ? FixedDataSize.Value : GetDataSize(value);
    }

    protected override int ReadItemSize(int index)
    {
      int itemSize = FixedItemSize.HasValue ? FixedItemSize.Value : 0;
      if (!FixedItemSize.HasValue)
      {
        long itemOffset = BaseStream.Position;
        try
        {
          int countSize = PwrBitConverter.GetSizeEncodingSize(int.MaxValue, CountSizing);
          BaseStream.Position = HeaderOffset + countSize + AllocItemSize * index;
          itemSize = BaseStream.ReadSize(ItemSizing, Endian);
        }
        finally
        {
          BaseStream.Position = itemOffset;
        }
      }
      return itemSize;
    }

    protected override T ReadItem(int index)
    {
      int itemSize = FixedItemSize.HasValue ? FixedItemSize.Value : 0;
      if (!FixedItemSize.HasValue)
      {
        long itemOffset = BaseStream.Position;
        try
        {
          int countSize = PwrBitConverter.GetSizeEncodingSize(int.MaxValue, CountSizing);
          BaseStream.Position = HeaderOffset + countSize + AllocItemSize * index;
          itemSize = BaseStream.ReadSize(ItemSizing, Endian);
        }
        finally
        {
          BaseStream.Position = itemOffset;
        }
      }
      return ReadItemData(itemSize);
    }

    protected override void WriteItem(int index, T value)
    {
      int itemSize = FixedItemSize.HasValue ? FixedItemSize.Value : GetItemSize(value);
      if (!FixedItemSize.HasValue)
      {
        long itemOffset = BaseStream.Position;
        try
        {
          int countSize = PwrBitConverter.GetSizeEncodingSize(int.MaxValue, CountSizing);
          long allocOffset = BaseStream.Position = HeaderOffset + countSize + AllocItemSize * index;
          int allocSize = PwrBitConverter.GetSizeEncodingSize(int.MaxValue, ItemSizing);
          BaseStream.WriteSize(itemSize, ItemSizing, Endian);
          int rest = allocSize - (int)(BaseStream.Position - allocOffset);
          if (rest > 0)
            BaseStream.WriteCollection(Enumerable.Repeat((byte)0, rest), (s, t) => s.WriteUByte(t));
        }
        finally
        {
          BaseStream.Position = itemOffset;
        }
      }
      WriteItemData(value, itemSize);
    }

    protected abstract T ReadItemData(int size);

    protected abstract void WriteItemData(T value, int size);

    #endregion
    #region Public methods

    public sealed override void Clear()
    {
      throw new NotImplementedException();
    }

    public sealed override void Add(T value)
    {
      throw new NotImplementedException();
    }

    public sealed override void AddRange(IEnumerable<T> coll)
    {
      throw new NotImplementedException();
    }

    public sealed override void Insert(int index, T value)
    {
      throw new NotImplementedException();
    }

    public sealed override void InsertRange(int index, IEnumerable<T> coll)
    {
      throw new NotImplementedException();
    }

    public sealed override void InsertRepeat(int index, T value, int count)
    {
      throw new NotImplementedException();
    }

    public sealed override bool Remove(T value)
    {
      throw new NotImplementedException();
    }

    public sealed override void RemoveAt(int index)
    {
      throw new NotImplementedException();
    }

    public sealed override void RemoveRange(int index, int count)
    {
      throw new NotImplementedException();
    }

    #endregion
    #endregion
  }
}
