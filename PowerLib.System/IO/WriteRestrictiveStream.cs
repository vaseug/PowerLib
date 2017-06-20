using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using	PowerLib.System.Collections;

namespace PowerLib.System.IO
{
	public sealed class WriteRestrictiveStream : Stream
	{
    private Stream _stream;
    private bool _leaveOpen;
		private long _available;

    public WriteRestrictiveStream(Stream stream, long available)
      : this(stream, available, false)
    {
    }

    public WriteRestrictiveStream(Stream stream, long available, bool leaveOpen)
		{
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (available < 0)
        throw new ArgumentOutOfRangeException("available");

      _stream = stream;
      _leaveOpen = leaveOpen;
			_available = available;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				if (!_leaveOpen)
					_stream.Dispose();
			base.Dispose(disposing);
		}

    public long Available
    {
      get { return _available; }
      set { _available = value ; }
    }

    public override void Close()
		{
			if (!_leaveOpen)
				_stream.Close();
			base.Close();
		}

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return _stream.CanTimeout;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return _stream.CanWrite;
			}
		}

		public override long Position
		{
			get
			{
				return _stream.Position;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override long Length
		{
			get
			{
				return _stream.Length;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return _stream.ReadTimeout;
			}
			set
			{
				_stream.ReadTimeout = value ;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return _stream.WriteTimeout;
			}
			set
			{
				_stream.WriteTimeout = value ;
			}
		}

		public override void Flush()
		{
      _stream.Flush();
		}

		public override void SetLength(long value)
		{
      throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (count > _available)
        throw new InvalidOperationException("Exceeded write operation limit.");

      if (_available > 0&& count > 0)
      {
        int write = (int)Comparable.Min(_available, count);
        _stream.Write(buffer, offset, write);
        _available -= write;
      }
    }
	}
}
