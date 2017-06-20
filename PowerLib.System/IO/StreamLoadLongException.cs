using System;
using System.IO;

namespace PowerLib.System.IO
{
	public class StreamLoadLongException : Exception
	{
    private long _total;

    public StreamLoadLongException(long total, Exception exception)
      : this(exception.Message, total, exception)
    {
    }

    public StreamLoadLongException(string message, long total, Exception exception)
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
