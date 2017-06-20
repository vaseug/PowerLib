using System;
using System.IO;
using System.Text;

namespace PowerLib.System.IO.Streamed
{
  public class StringCollectionParameter : FlaggedCollectionParameter
  {
    public StringCollectionParameter()
      : base()
    {
    }

    public StringCollectionParameter(Endian endian)
      : base(endian)
    {
    }

    public Encoding Encoding { get; set; }

    public override void Read(Stream stream)
    {
      base.Read(stream);
      Encoding = Encoding.GetEncoding(stream.ReadUInt16());
    }

    public override void Write(Stream stream)
    {
      base.Write(stream);
      stream.WriteUInt16((ushort)(Encoding == null ? 0 : Encoding.CodePage));
    }
  }
}
