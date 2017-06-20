using System;
using System.IO;

namespace PowerLib.System.IO
{
	public class StreamLoadException : Exception
	{
    private int _total;

    public StreamLoadException(int total, Exception exception)
      : this(exception.Message, total, exception)
    {
    }

    public StreamLoadException(string message, int total, Exception exception)
			: base(message, exception)
		{
      _total = total;
		}

    public int Total
    {
      get { return _total; }
    }
	}
}
