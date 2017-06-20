using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PowerLib.System.ComponentModel.DataAnnotations
{
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class FixedLengthAttribute : ValidationAttribute
  {
    private bool _isFixedLength;

    public FixedLengthAttribute(bool isFixedLength)
      : base()
    {
      _isFixedLength = isFixedLength;
    }

    public bool IsFixedLength
    {
      get { return _isFixedLength; }
    }
  }
}
