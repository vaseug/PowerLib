using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerLib.System.Text
{
  public interface IFormatter<T>
  {
    string Format(T value, string format, IFormatProvider formatProvider);
  }
}
