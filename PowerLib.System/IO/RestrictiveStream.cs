using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using	PowerLib.System.Collections;

namespace PowerLib.System.IO
{
	public sealed class RestrictiveStream : Stream
	{
    private Stream _stream;
    private bool _leaveOpen;
		private long _readAvailable;
    private long _writeAvailable;

    public RestrictiveStream(Stream stream)
      : this(stream, false)
    {
    }

    public RestrictiveStream(Stream stream, bool leaveOpen)
		{
      if (stream == null)
        throw new ArgumentNullException("stream");

      _stream = stream;
      _leaveOpen = leaveOpen;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				if (!_leaveOpen)
					_stream.Dispose();
			base.Dispose(disposing);
		}

		public long ReadAvailable
		{
			get { return _readAvailable; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException();

        _readAvailable = value;
      }
		}

    public long WriteAvailable
    {
      get { return _writeAvailable; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException();

        _writeAvailable = value;
      }
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
				return _stream.CanRead && _readAvailable > 0;
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
				return _stream.CanWrite && _writeAvailable > 0;
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
      if (_readAvailable == 0 || count == 0)
        return 0;

			int read = _stream.Read(buffer, offset, (int)Comparable.Min(_readAvailable, count));
			_readAvailable -= read;
			return read;
		}

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (count > _writeAvailable)
        throw new InvalidOperationException("Exceeded write operation limit.");

      if (count > 0)
      {
        int write = (int)Comparable.Min(_writeAvailable, count);
        _stream.Write(buffer, offset, write);
        _writeAvailable -= write;
      }
    }
  }
}
