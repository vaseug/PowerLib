using System;
using System.IO;

namespace PowerLib.System.IO
{
	public class StreamReadLongException : Exception
	{
    private Array _buffer;
    private long _total;

    public StreamReadLongException(Array buffer, long total, Exception exception)
      : this(exception.Message, buffer, total, exception)
    {
    }

    public StreamReadLongException(string message, Array buffer, long total, Exception exception)
			: base(message, exception)
		{
			_buffer = buffer;
      _total = total;
		}

		public Array Buffer
		{
			get { return _buffer; }
		}

    public long Total
    {
      get { return _total; }
    }
	}
}
