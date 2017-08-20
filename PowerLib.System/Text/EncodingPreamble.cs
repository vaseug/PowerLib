using System;
using PowerLib.System.Collections;

namespace PowerLib.System.Text
{
  public class EncodingPreamble
  {
    private const int UTF16LE = 1200;
    private const int UTF16BE = 1201;
    private const int UTF32LE = 12000;
    private const int UTF32BE = 12001;
    private const int UTF7 = 65000;
    private const int UTF8 = 65001;

    private static EncodingPreamble[] preambles = new[]
    {
      new EncodingPreamble(65001, new byte[] { 0xef, 0xbb, 0xbf }),
      new EncodingPreamble(12000, new byte[] { 0xff, 0xfe, 0x00, 0x00 }),
      new EncodingPreamble(12001, new byte[] { 0x00, 0x00, 0xfe, 0xff }),
      new EncodingPreamble(1200, new byte[] { 0xff, 0xfe }),
      new EncodingPreamble(1201, new byte[] { 0xfe, 0xff }),
    };

    private EncodingPreamble(int codepage, byte[] bom)
    {
      if (bom == null)
        throw new ArgumentNullException("bom");

      CodePage = codepage;
      BOM = bom;
    }

    public int CodePage { get; private set; }

    public byte[] BOM { get; private set; }

    public static EncodingPreamble Detect(byte[] buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      if (buffer.Length > 0)
        foreach (var preamble in preambles)
          if (buffer.Length >= preamble.BOM.Length && preamble.BOM.SequenceEqual(0, buffer, 0, preamble.BOM.Length))
            return preamble;
      return null;
    }
  }
}
