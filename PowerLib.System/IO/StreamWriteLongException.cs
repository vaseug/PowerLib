using System;
using System.IO;

namespace PowerLib.System.IO
{
	public class StreamWriteLongException : Exception
	{
    private long _total;

    public StreamWriteLongException(long total, Exception exception)
      : this(exception.Message, total, exception)
    {
    }

    public StreamWriteLongException(string message, long total, Exception exception)
			: base(message, exception)
		{
      _total = total;
		}

    public long Total
    {
      get { return _total; }
    }
	}
}
