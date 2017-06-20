using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace PowerLib.System.Runtime.InteropServices
{
  public class PtrStream : Stream
  {
    private IntPtr _ptr = IntPtr.Zero;
    private long _position;
    private long _offset;
    private long _length;
    private bool _writable;

    public PtrStream(IntPtr ptr)
      : this(ptr, 0, -1, true)
    {
    }

    public PtrStream(IntPtr ptr, bool writable)
      : this(ptr, 0, -1, writable)
    {
    }

    public PtrStream(IntPtr ptr, int length, bool writable)
      : this(ptr, 0, length, writable)
    {
    }

    public PtrStream(IntPtr ptr, int offset, int length, bool writable)
    {
      if (ptr == IntPtr.Zero)
        throw new ArgumentException("Invalid unmanaged memory pointer", "ptr");

      _ptr = ptr;
      _position = 0L;
      _offset = offset;
      _length = length;
      _writable = writable;
    }

    protected override void Dispose(bool disposing)
    {
      Close();
      base.Dispose(disposing);
    }

    public override void Close()
    {
      base.Close();
      if (_ptr != IntPtr.Zero)
        _ptr = IntPtr.Zero;
    }

    public override long Length
    {
      get
      {
        if (_ptr == IntPtr.Zero)
          throw new ObjectDisposedException(null);
        if (_length < 0)
          throw new NotSupportedException();

        return _length;
      }
    }

    public override long Position
    {
      get
      {
        if (_ptr == IntPtr.Zero)
          throw new ObjectDisposedException(null);

        return _position;
      }
      set
      {
        if (_ptr == IntPtr.Zero)
          throw new ObjectDisposedException(null);
        if (value < 0 || _length >= 0 && value > _length)
          throw new ArgumentOutOfRangeException("value");

        _position = value;
      }
    }

    public override bool CanTimeout
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
        return _ptr != IntPtr.Zero;
      }
    }

    public override bool CanRead
    {
      get
      {
        return _ptr != IntPtr.Zero;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return _ptr != IntPtr.Zero && _writable;
      }
    }

    public override int ReadTimeout
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public override int WriteTimeout
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public override void SetLength(long value)
    {
      if (_ptr == IntPtr.Zero)
        throw new ObjectDisposedException(null);

      throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      if (_ptr == IntPtr.Zero)
        throw new ObjectDisposedException(null);

      switch (origin)
      {
        case SeekOrigin.Begin:
          if (offset < 0 || _length >= 0L && offset > _length)
            throw new ArgumentOutOfRangeException("offset");
          return _position = offset;
        case SeekOrigin.Current:
          if (offset < 0 && _position + offset < 0 || _length >= 0 && offset > _length - _position)
            throw new ArgumentOutOfRangeException("offset");
          return _position += offset;
        case SeekOrigin.End:
          if (_length < 0)
            throw new NotSupportedException();
          else if (offset < 0 && _length + offset < 0 || offset > 0)
            throw new ArgumentOutOfRangeException("offset");
          return _position = _length + offset;
        default:
          throw new ArgumentOutOfRangeException("origin");
      }
    }

    public override void Flush()
    {
      if (_ptr == IntPtr.Zero)
        throw new ObjectDisposedException(null);
    }

    public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
    {
      if (_ptr == IntPtr.Zero)
        throw new ObjectDisposedException(null);
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset < 0 || offset >= buffer.Length)
        throw new ArgumentOutOfRangeException("offset");
      if (count < 0 || count > buffer.Length - offset)
        throw new ArgumentOutOfRangeException("count");

      if (_length >= 0)
        count = (int)Comparable.Min(_length - _position, count);
      if (count > 0)
      {
        Marshal.Copy(new IntPtr(_ptr.ToInt64() + _offset + _position), buffer, offset, count);
        _position += count;
      }
      return count;
    }

    public override void Write(Byte[] buffer, Int32 offset, Int32 count)
    {
      if (_ptr == IntPtr.Zero)
        throw new ObjectDisposedException(null);
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset < 0 || offset > buffer.Length)
        throw new ArgumentOutOfRangeException("offset");
      if (count < 0 || count > buffer.Length - offset)
        throw new ArgumentOutOfRangeException("count");
      if (!_writable)
        throw new NotSupportedException();
      if (_length >= 0 && _position > _length - count)
        throw new InvalidOperationException();

      Marshal.Copy(buffer, offset, new IntPtr(_ptr.ToInt64() + _offset + _position), count);
      _position += count;
    }
  }
}
