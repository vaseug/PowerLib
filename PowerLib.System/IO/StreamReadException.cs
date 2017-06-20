using System;
using System.IO;

namespace PowerLib.System.IO
{
	public class StreamReadException : Exception
	{
    private Array _buffer;
    private int _total;

    public StreamReadException(Array buffer, int total, Exception exception)
      : this(exception.Message, buffer, total, exception)
    {
    }

    public StreamReadException(string message, Array buffer, int total, Exception exception)
			: base(message, exception)
		{
			_buffer = buffer;
      _total = total;
		}

		public Array Buffer
		{
			get { return _buffer; }
		}

    public int Total
    {
      get { return _total; }
    }
	}
}
