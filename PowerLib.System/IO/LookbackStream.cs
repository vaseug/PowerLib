using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using	PowerLib.System.Collections;

namespace PowerLib.System.IO
{
	public sealed class LookbackStream : Stream
	{
		private long _position;
		private int _offset;
		private bool _leaveOpen;
		private PwrList<byte> _buffer;
		private Stream _stream;

		public LookbackStream(Stream stream, int maxSize, bool leaveOpen)
		{
			_stream = stream;
			_leaveOpen = leaveOpen;
			_buffer = new PwrList<byte>(maxSize) { AutoTrim = false };
			_offset = _buffer.Count;
			try
			{
				_position = _stream.Position;
			}
			catch (Exception)
			{
				_position = 0L;
			}
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
				return true;
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
				return _position;
			}
			set
			{
				if (value >= _position - _offset && value <= _position + (_buffer.Count - _offset))
				{
					_offset += (int)(value - _position);
					_position = value ;
				}
				else
				{
					_stream.Position = value ;
					_position = value ;
					_buffer.Clear();
					_offset = _buffer.Count;
				}
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
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long position = 0;
			switch (origin)
			{
				case SeekOrigin.Begin:
					if (offset >= _position - _offset && offset <= _position + (_buffer.Count - _offset))
						position = offset;
					break;
				case SeekOrigin.Current:
					break;
				case SeekOrigin.End:
					position = _stream.Seek(offset, SeekOrigin.End);
					break;
				default:
					throw new ArgumentOutOfRangeException("origin");
			}
			return position;
		}

		public int Peek(byte[] buffer, int offset, int count)
		{
		  return 0;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			try
			{
				long position = _stream.Position;
				if (position != _position - (_buffer.Count - _offset))
				{
					_buffer.Clear();
					_offset = _buffer.Count;
					_position = position;
				}
			}
			catch (NotSupportedException)
			{
			}
			int cached = Comparable.Min(count, _buffer.Count - _offset);
			if (cached> 0)
				//buffer.MoveRange();
				buffer.Fill(i => _buffer[_offset + i], new Range(offset, cached));
			int read = count - cached;
			if (read > 0)
			{
				read = _stream.Read(buffer, offset, read);
				if (read > _buffer.Capacity - _buffer.Count)
					_buffer.RemoveRange(0, Comparable.Min(read - (_buffer.Capacity - _buffer.Count), _buffer.Count));
				_buffer.AddRange(buffer.Skip(offset + cached + Comparable.Max(0, read - (_buffer.Capacity - _buffer.Count))).Take(Comparable.Min(read, _buffer.Capacity - _buffer.Count)));
			}
			return cached + read;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_stream.Write(buffer, offset, count);
			if (count == 0)
				return;
			try
			{
				long position = _stream.Position;
				if (position == _position + count)
				{
					if (count > _buffer.Capacity - _buffer.Count)
						_buffer.RemoveRange(0, Comparable.Min(count - (_buffer.Capacity - _buffer.Count), _buffer.Count));
					_buffer.AddRange(buffer.Skip(offset + Comparable.Max(count - _buffer.Capacity, 0)).Take(Comparable.Min(count, _buffer.Capacity - _buffer.Count)));
					_position = position;
					_offset = _buffer.Count;
				}
			}
			catch (NotSupportedException)
			{
			}
		}
	}
}
