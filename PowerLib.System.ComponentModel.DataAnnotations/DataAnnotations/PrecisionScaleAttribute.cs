using System;
using System.ComponentModel.DataAnnotations;

namespace PowerLib.System.ComponentModel.DataAnnotations
{
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class PrecisionScaleAttribute : ValidationAttribute
  {
    private byte _precision;
    private byte _scale;

    public PrecisionScaleAttribute(byte precision)
      : this(precision, 0)
    {
    }

    public PrecisionScaleAttribute(byte precision, byte scale)
      : base()
    {
      if (scale > precision)
        throw new ArgumentException("Scale is greater than precision.", "scale");

      _precision = precision;
      _scale = scale;
    }

    public byte Precision
    {
      get { return _precision; }
    }

    public byte Scale
    {
      get { return _scale; }
    }
  }
}
