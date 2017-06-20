using System;
using System.Collections.Generic;
using System.IO;

namespace PowerLib.System.IO
{
	public class StreamReadException<T> : StreamReadException
	{
    public StreamReadException(T[] buffer, int total, Exception exception)
      : this(exception.Message, buffer, total, exception)
    {
    }

    public StreamReadException(string message, T[] buffer, int total, Exception exception)
			: base(message, buffer, total, exception)
		{
		}

		public new T[] Buffer
		{
			get { return (T[])base.Buffer; }
		}
	}
}
