using System;
using System.IO;

namespace PowerLib.System.IO
{
	public class StreamWriteException : Exception
	{
    private int _total;

    public StreamWriteException(int total, Exception exception)
      : this(exception.Message, total, exception)
    {
    }

    public StreamWriteException(string message, int total, Exception exception)
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
