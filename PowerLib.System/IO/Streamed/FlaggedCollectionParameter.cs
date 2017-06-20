using System;
using System.IO;

namespace PowerLib.System.IO.Streamed
{
  public class FlaggedCollectionParameter : CollectionParameter
  {
    public FlaggedCollectionParameter()
      : base()
    {
    }

    public FlaggedCollectionParameter(Endian endian)
      : base(endian)
    {
    }

    public SizeEncoding FlagSizing { get; set; }

    public int? FlagSize { get; set; }

    public bool Compact { get; set; }

    public override void Read(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      byte options = stream.ReadUByte();
      CountSizing = (SizeEncoding)PwrBitConverter.GetMasked(options, 0, 3);
      ItemSizing = (SizeEncoding)PwrBitConverter.GetMasked(options, 3, 3);
      Compact = PwrBitConverter.GetBit(options, 6);
      DataSize = PwrBitConverter.GetBit(options, 7) ? stream.ReadSize(ItemSizing) : default(int?);
      options = stream.ReadUByte();
      FlagSizing = (SizeEncoding)PwrBitConverter.GetMasked(options, 0, 3);
      FlagSize = PwrBitConverter.GetBit(options, 4) ? stream.ReadSize(FlagSizing) : default(int?);
    }

    public override void Write(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.WriteUByte((byte)
        PwrBitConverter.SetBit(
          PwrBitConverter.SetBit(
            PwrBitConverter.SetMasked(
              PwrBitConverter.SetMasked(0, 0, 3, (int)CountSizing), 3, 3, (int)ItemSizing), 6, Compact), 7, DataSize.HasValue));
      if (DataSize.HasValue)
        stream.WriteSize(DataSize.Value, ItemSizing);
      stream.WriteUByte((byte)
        PwrBitConverter.SetBit(
          PwrBitConverter.SetMasked(0, 0, 3, (int)FlagSizing), 4, FlagSize.HasValue));
      if (FlagSize.HasValue)
        stream.WriteSize(FlagSize.Value, FlagSizing);
    }
  }
}
