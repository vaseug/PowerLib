using System;
using System.IO;

namespace PowerLib.System.IO
{
  public interface IStreamable
  {
    void Read(Stream stream);

    void Write(Stream stream);
  }
}
