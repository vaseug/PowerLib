using System;
using System.IO;

namespace PowerLib.System.IO.Streamed
{
  public class CollectionParameter : IStreamable
  {
    private Endian _endian;

    #region Constructors

    public CollectionParameter()
      : this(Endian.Default)
    {
    }

    public CollectionParameter(Endian endian)
    {
      _endian = endian;
    }

    #endregion
    #region Properties

    protected Endian Endian { get { return _endian; } }

    public SizeEncoding CountSizing { get; set; }

    public SizeEncoding ItemSizing { get; set; }

    public int? DataSize { get; set; }

    #endregion
    #region Methods

    public virtual void Read(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      byte options = stream.ReadUByte();
      CountSizing = (SizeEncoding)PwrBitConverter.GetMasked(options, 0, 3);
      ItemSizing = (SizeEncoding)PwrBitConverter.GetMasked(options, 3, 3);
      DataSize = PwrBitConverter.GetBit(options, 7) ? stream.ReadSize(ItemSizing) : default(int?);
    }

    public virtual void Write(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.WriteUByte((byte)
        PwrBitConverter.SetBit(
          PwrBitConverter.SetMasked(
            PwrBitConverter.SetMasked(0, 0, 3, (int)CountSizing), 3, 3, (int)ItemSizing), 7, DataSize.HasValue));
      if (DataSize.HasValue)
        stream.WriteSize(DataSize.Value, ItemSizing);
    }

    #endregion
  }
}
