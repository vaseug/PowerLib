using System;
using System.IO;

namespace PowerLib.System.IO.Streamed
{
  public abstract class BaseFlaggedStreamedCollection<T, F> : BaseStreamedCollection<T>
  {
    #region Constructors

    protected BaseFlaggedStreamedCollection(Stream stream, bool leaveOpen, bool boostAccess)
      : base(stream, leaveOpen, boostAccess)
    {
    }

    #endregion
    #region Properties

    protected abstract bool Compact { get; }

    protected abstract int? FixedFlagSize { get; }

    protected override int? FixedItemSize
    {
      get { return FixedFlagSize.HasValue && FixedDataSize.HasValue && !Compact ? FixedFlagSize.Value + FixedDataSize.Value : default(int?); }
    }

    #endregion
    #region Methods

    protected virtual int GetFlagSize(F flags)
    {
      if (FixedFlagSize.HasValue)
        return FixedFlagSize.Value;
      throw new NotImplementedException();
    }

    protected abstract bool HasItemData(F flags);

    protected abstract F GetItemFlag(T value);

    protected abstract F ReadItemFlag();

    protected abstract void WriteItemFlag(F flags);

    protected override int GetItemSize(T value)
    {
      F flag = GetItemFlag(value);
      bool hasValue = HasItemData(flag);
      int size = FixedDataSize.HasValue && (hasValue || !Compact) ? FixedDataSize.Value : !FixedDataSize.HasValue && hasValue ? GetDataSize(value) : 0;
      if (!FixedDataSize.HasValue && hasValue)
        size += PwrBitConverter.GetSizeEncodingSize(size, ItemSizing);
      return size + (FixedFlagSize.HasValue ? FixedFlagSize.Value : GetFlagSize(flag));
    }

    protected override int ReadItemSize(int index)
    {
      long offset = BaseStream.Position;
      F flag = ReadItemFlag();
      bool hasValue = HasItemData(flag);
      int size = FixedDataSize.HasValue && (hasValue || !Compact) ? FixedDataSize.Value : !FixedDataSize.HasValue && hasValue ? BaseStream.ReadSize(ItemSizing, Endian) : 0;
      return size + (int)(BaseStream.Position - offset);
    }

    protected override T ReadItem(int index)
    {
      F flag = ReadItemFlag();
      if (HasItemData(flag))
        return ReadItemData(flag, FixedDataSize.HasValue ? FixedDataSize.Value : BaseStream.ReadSize(ItemSizing, Endian));
      else if (FixedDataSize.HasValue && !Compact)
        BaseStream.Position += FixedDataSize.Value;
      return default(T);
    }

    protected override void WriteItem(int index, T value)
    {
      F flag = GetItemFlag(value);
      WriteItemFlag(flag);
      if (HasItemData(flag))
      {
        int size = FixedDataSize.HasValue ? FixedDataSize.Value : GetDataSize(value);
        if (!FixedDataSize.HasValue)
          BaseStream.WriteSize(size, ItemSizing, Endian);
        WriteItemData(value, flag, size);
      }
      else if (FixedDataSize.HasValue && !Compact)
        BaseStream.Position += FixedDataSize.Value;
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
