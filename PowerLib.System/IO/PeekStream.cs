using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using	PowerLib.System.Collections;

namespace PowerLib.System.IO
{
	public sealed class PeekStream : Stream
	{
    private Stream _stream;
    private bool _leaveOpen;
    private byte? _peek;

    public PeekStream(Stream stream)
      : this(stream, false)
    {
    }

    public PeekStream(Stream stream, bool leaveOpen)
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
				return _stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return _stream.CanSeek;
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
        _stream.Position = value;
        _peek = null;
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
      _stream.SetLength(value);
      _peek = null;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long position = _stream.Seek(offset, origin);
      _peek = null;
      return position;
		}

    public override int Read(byte[] buffer, int offset, int count)
		{
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset < 0)
        throw new ArgumentOutOfRangeException("offset");
      if (count < 0)
        throw new ArgumentOutOfRangeException("offset");
      if (offset > buffer.Length || count > buffer.Length - offset)
        throw new ArgumentException("Inconsistent offset and count");

      int read = 0;
      if (_peek.HasValue)
      {
        buffer[offset++] = _peek.Value;
        count--;
        read++;
        _peek = null;
      }
      if (count > 0)
			  read += _stream.Read(buffer, offset, count);
			return read;
		}

    public override void Write(byte[] buffer, int offset, int count)
    {
      _stream.Write(buffer, offset, count);
      _peek = null;
    }

    public override int ReadByte()
    {
      if (!_peek.HasValue)
      {
        int read = _stream.ReadByte();
        if (read < 0)
          return read;
        _peek = (byte)read;
      }
      return (int)_peek.Value;
    }

    public int Peek()
    {
      if (!_peek.HasValue)
      {
        byte[] buff = new byte[1];
        if (_stream.Read(buff, 0, 1) != 1)
          return -1;
        _peek = buff[0];
      }
      return (int)_peek.Value;
    }
  }
}
