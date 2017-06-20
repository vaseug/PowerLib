using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using	PowerLib.System.Collections;

namespace PowerLib.System.IO
{
	public sealed class RangeStream : Stream
	{
    private Stream _stream;
    private bool _leaveOpen;
    private long _rangeOffset;
    private long _rangeLength;
    private long _position;

    public RangeStream(Stream stream)
      : this(stream, false)
    {
    }

    public RangeStream(Stream stream, bool leaveOpen)
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

    public long RangeOffset
    {
      get
      {
        return _rangeOffset;
      }
      set
      {
        if (value < 0L)
          throw new ArgumentOutOfRangeException();

        _rangeOffset = value;
      }
    }

    public long RangeLength
    {
      get
      {
        return _rangeLength;
      }
      set
      {
        if (value < 0L || value > long.MaxValue - _rangeOffset)
          throw new ArgumentOutOfRangeException();

        _rangeLength = value;
      }
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

    public override int ReadTimeout
    {
      get
      {
        return _stream.ReadTimeout;
      }
      set
      {
        _stream.ReadTimeout = value;
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
        _stream.WriteTimeout = value;
      }
    }

    public override long Position
		{
			get
			{
				return _stream.Position - _rangeOffset;
			}
			set
			{
        if (value < 0)
          throw new IOException("Position before stream.");

        _stream.Position = _rangeOffset + value;
			}
		}

		public override long Length
		{
			get
			{
				return _rangeLength;
			}
		}

		public override void Flush()
		{
      _stream.Flush();
		}

		public override void SetLength(long value)
		{
      if (value > _rangeLength)
      {
        long delta = value - _rangeLength;
        long position = _rangeOffset + _rangeLength;
        long count = _stream.Length - position;
        _stream.SetLength(_stream.Length + delta);
        _stream.Position = position;
        _stream.Move(position + delta, count, 1024);
      }
      else if (value < _rangeLength)
      {
        long delta = _rangeLength - value;
        long position = _rangeOffset + _rangeLength;
        long count = _stream.Length - position;
        _stream.Position = position;
        _stream.Move(position - delta, count, 1024);
        _stream.SetLength(_stream.Length - delta);
      }
    }

		public override long Seek(long offset, SeekOrigin origin)
		{
      switch (origin)
      {
        case SeekOrigin.Begin:
          if (offset < 0)
            throw new IOException("New position before stream.");
          return _stream.Seek(_rangeOffset + offset, SeekOrigin.Begin) - _rangeOffset;
        case SeekOrigin.End:
          if (offset < -_rangeLength)
            throw new IOException("New position before stream.");
          return _stream.Seek(_rangeOffset + _rangeLength + offset, SeekOrigin.End);
        case SeekOrigin.Current:
          long postion = _stream.Position - _rangeOffset;
          if (postion < 0)
            throw new IOException("Current position before stream.");
          if (offset < -postion)
            throw new IOException("New position before stream.");
          return _stream.Seek(_rangeOffset + postion + offset, SeekOrigin.Begin);
        default:
          throw new ArgumentOutOfRangeException();
      }
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
			return read;
		}

    public override void Write(byte[] buffer, int offset, int count)
    {
      _stream.Write(buffer, offset, count);
    }

    public override int ReadByte()
    {
      return 0;
    }
  }
}
