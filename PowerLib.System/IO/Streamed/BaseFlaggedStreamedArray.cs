using System;
using System.IO;

namespace PowerLib.System.IO.Streamed
{
  public abstract class BaseFlaggedStreamedArray<T, F> : BaseStreamedArray<T>
  {
    #region Constructors

    protected BaseFlaggedStreamedArray(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, leaveOpen, boostAccess)
    {
    }

    #endregion
    #region Properties

    protected abstract bool Compact { get; }

    protected abstract int FlagSize { get; }

    protected override int AllocItemSize
    {
      get { return FlagSize + (FixedDataSize.HasValue ? 0 : PwrBitConverter.GetSizeEncodingSize(ItemSizing, int.MaxValue)); }
    }

    protected override int? FixedItemSize
    {
      get { return FixedDataSize.HasValue && !Compact ? FixedDataSize.Value : default(int?); }
    }

    #endregion
    #region Methods

    protected virtual long GetAllocOffset(int index)
    {
      return HeaderOffset + AllocItemSize * index + PwrBitConverter.GetSizeEncodingSize(CountSizing, int.MaxValue);
    }

    protected abstract bool HasItemData(F flags);

    protected abstract F GetItemFlag(T value);

    protected abstract F ReadItemFlag(int index);

    protected abstract void WriteItemFlag(int index, F flags);

    protected override int GetItemSize(T value)
    {
      bool hasValue = HasItemData(GetItemFlag(value));
      return FixedDataSize.HasValue && (hasValue || !Compact) ? FixedDataSize.Value : !FixedDataSize.HasValue && hasValue ? GetDataSize(value) : 0;
    }

    protected override int ReadItemSize(int index)
    {
      if (FixedItemSize.HasValue)
        return FixedItemSize.Value;
      long itemOffset = BaseStream.Position;
      try
      {
        long allocOffset = BaseStream.Position = GetAllocOffset(index);
        if (!HasItemData(ReadItemFlag(index)))
          return 0;
        else if (FixedDataSize.HasValue)
          return FixedDataSize.Value;
        else
        {
          BaseStream.Position += (allocOffset + FlagSize - BaseStream.Position);
          return BaseStream.ReadSize(ItemSizing, Endian);
        }
      }
      finally
      {
        BaseStream.Position = itemOffset;
      }
    }

    protected override T ReadItem(int index)
    {
      F flag;
      int itemSize = FixedItemSize.HasValue ? FixedItemSize.Value : 0;
      long itemOffset = BaseStream.Position;
      try
      {
        long allocOffset = BaseStream.Position = GetAllocOffset(index);
        flag = ReadItemFlag(index);
        if (!HasItemData(flag))
          return default(T);
        if (!FixedDataSize.HasValue)
        {
          BaseStream.Position += (allocOffset + FlagSize - BaseStream.Position);
          itemSize = BaseStream.ReadSize(ItemSizing, Endian);
        }
        else if (itemSize == 0)
          itemSize = FixedDataSize.Value;
      }
      finally
      {
        BaseStream.Position = itemOffset;
      }
      return ReadItemData(flag, itemSize);
    }

    protected override void WriteItem(int index, T value)
    {
      F flag = GetItemFlag(value);
      int itemSize = FixedItemSize.HasValue ? FixedItemSize.Value : GetItemSize(value);
      long itemOffset = BaseStream.Position;
      try
      {
        long allocOffset = BaseStream.Position = GetAllocOffset(index);
        WriteItemFlag(index, flag);
        if (!HasItemData(flag))
          return;
        if (!FixedDataSize.HasValue)
        {
          BaseStream.Position += (allocOffset + FlagSize - BaseStream.Position);
          BaseStream.WriteSize(GetDataSize(value), ItemSizing, Endian);
        }
        else if (itemSize == 0)
          itemSize = FixedDataSize.Value;
      }
      finally
      {
        BaseStream.Position = itemOffset;
      }
      WriteItemData(value, flag, itemSize);
    }

    protected abstract T ReadItemData(F flags, int size);

    protected abstract void WriteItemData(T value, F flags, int size);

    protected sealed override T ReadItemData(int size)
    {
      throw new NotImplementedException();
    }

    protected sealed override void WriteItemData(T value, int size)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
