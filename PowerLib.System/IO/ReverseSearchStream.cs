using System;
using System.IO;

namespace PowerLib.System.IO
{
  public sealed class ReverseSearchStream : Stream
  {
    private Stream _stream;
    private bool _leaveOpen;
    private bool _endOf;

    public ReverseSearchStream(Stream stream, bool leaveOpen)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      _stream = stream;
      _leaveOpen = leaveOpen;
      _endOf = false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        if (!_leaveOpen)
          _stream.Dispose();
      base.Dispose(disposing);
    }

    public override bool CanRead
    {
      get { return _stream.CanRead; }
    }

    public override bool CanSeek
    {
      get { return _stream.CanSeek; }
    }

    public override bool CanTimeout
    {
      get { return _stream.CanTimeout; }
    }

    public override bool CanWrite
    {
      get { return false; }
    }

    public override long Length
    {
      get { return _stream.Length; }
    }

    public override long Position
    {
      get { return _stream.Position; }
      set { _stream.Position = value ; }
    }

    public override int ReadTimeout
    {
      get { return _stream.ReadTimeout; }
      set { _stream.ReadTimeout = value ; }
    }

    public override int WriteTimeout
    {
      get { return _stream.WriteTimeout; }
      set { _stream.WriteTimeout = value ; }
    }

    public override void Flush()
    {
      throw new NotSupportedException();
    }

    public override void Close()
    {
      if (!_leaveOpen)
        _stream.Close();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return _stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    public override int ReadByte()
    {
      if (_endOf)
        return -1;
      int result = base.ReadByte();
      long postion = _stream.Position;
      if (result < 0 || postion == 1L)
        _endOf = true;
      if (result >= 0)
        _stream.Seek(-1, SeekOrigin.Current);
      return result;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset < 0 || offset > buffer.Length)
        throw new ArgumentOutOfRangeException("offset");
      if (count < 0 || count > buffer.Length - offset)
        throw new ArgumentOutOfRangeException("count");
      if (_endOf || count == 0)
        return 0;

      long position = _stream.Position;
      if (count - 1 > position)
        count = (int)position + 1;
      _stream.Seek(-count + 1, SeekOrigin.Current);
      int read = _stream.Read(buffer, offset, count);
      position = _stream.Position;
      if (position == read)
        _endOf = true;
      else
        _stream.Seek(-read, SeekOrigin.Current);
      return read;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }
  }
}
