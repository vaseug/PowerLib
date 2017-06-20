using System;
using System.Globalization;

namespace PowerLib.System.Text
{
  public interface INumberParser<T>
  {
    T Parse(string s, NumberStyles styles, IFormatProvider formatProvider);

    bool TryParse(string s, NumberStyles styles, IFormatProvider formatProvider, out T result);
  }
}
