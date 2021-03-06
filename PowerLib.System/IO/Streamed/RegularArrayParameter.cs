﻿using System;
using System.IO;

namespace PowerLib.System.IO.Streamed
{
  public class RegularArrayParameter : CollectionParameter
  {
    #region Constructors

    public RegularArrayParameter()
      : base()
    {
    }

    public RegularArrayParameter(Endian endian)
      : base(endian)
    {
    }

    #endregion
    #region Properties

    public int[] Lengths { get; set; }

    #endregion
    #region Methods

    public override void Read(Stream stream)
    {
      base.Read(stream);
      Lengths = stream.ReadArray(s => s.ReadSize(CountSizing, Endian), stream.ReadSize(SizeEncoding.B1, Endian));
    }

    public override void Write(Stream stream)
    {
      base.Write(stream);
      stream.WriteSize(Lengths.Length, SizeEncoding.B1, Endian);
      stream.WriteArray(Lengths, (s, t) => s.WriteSize(t, CountSizing, Endian));
    }

    #endregion
  }
}
