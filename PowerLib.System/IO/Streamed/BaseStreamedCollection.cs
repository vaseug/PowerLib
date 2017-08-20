using System;
using System.IO;
using System.Linq;

namespace PowerLib.System.IO.Streamed
{
  public abstract class BaseStreamedCollection<T> : StreamedCollection<T>
  {
    #region Constructors

    protected BaseStreamedCollection(Stream stream, bool leaveOpen, bool boostAccess)
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
      get { return HeaderOffset + PwrBitConverter.GetSizeEncodingSize(int.MaxValue, CountSizing); }
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
      if (FixedItemSize.HasValue)
        return FixedItemSize.Value;
      int itemSize = GetDataSize(value);
      return itemSize + PwrBitConverter.GetSizeEncodingSize(itemSize, ItemSizing);
    }

    protected override int ReadItemSize(int index)
    {
      if (FixedItemSize.HasValue)
        return FixedItemSize.Value;
      long itemOffset = BaseStream.Position;
      int itemSize = BaseStream.ReadSize(ItemSizing, Endian);
      itemSize += (int)(BaseStream.Position - itemOffset);
      return itemSize;
    }

    protected abstract T ReadItemData(int size);

    protected abstract void WriteItemData(T value, int size);

    protected override T ReadItem(int index)
    {
      return ReadItemData(FixedDataSize.HasValue ? FixedDataSize.Value : BaseStream.ReadSize(ItemSizing, Endian));
    }

    protected override void WriteItem(int index, T value)
    {
      int itemSize = FixedDataSize.HasValue ? FixedDataSize.Value : GetDataSize(value);
      if (!FixedItemSize.HasValue)
        BaseStream.WriteSize(itemSize, ItemSizing, Endian);
      WriteItemData(value, itemSize);
    }

    #endregion
  }
}
