using System;
using System.IO;
using System.Text;

namespace PowerLib.System.IO.Streamed
{
  public class StringArrayParameter : FlaggedRegularArrayParameter
  {
    #region Constructors

    public StringArrayParameter()
      : base()
    {
    }

    public StringArrayParameter(Endian endian)
      : base(endian)
    {
    }

    #endregion
    #region Properties

    public Encoding Encoding { get; set; }

    #endregion
    #region Methods

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

    #endregion
  }
}
