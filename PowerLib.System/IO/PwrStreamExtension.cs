using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PowerLib.System.Collections.Matching;
using PowerLib.System.Linq;

namespace PowerLib.System.IO
{
  public static class PwrStreamExtension
  {
    private readonly static byte[] filler = new byte[1024];

    public static S Locate<S>(this S stream, long offset)
      where S : Stream
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Position = offset;
      return stream;
    }

    public static S Locate<S>(this S stream, long offset, SeekOrigin origin)
      where S : Stream
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Seek(offset, origin);
      return stream;
    }

    public static Stream Resize(this Stream stream, long length)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.SetLength(length);
      return stream;
    }

    //public static S Resize<S>(this S stream, long length)
    //  where S : Stream
    //{
    //  if (stream == null)
    //    throw new ArgumentNullException("stream");

    //  stream.SetLength(length);
    //  return stream;
    //}

    public static S Read<S, T>(this S stream, out T value, Func<S, T> reader)
      where S : Stream
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (reader == null)
        throw new ArgumentNullException("reader");

      value = reader(stream);
      return stream;
    }

    public static S ReadAt<S, T>(this S stream, long offset, out T value, Func<S, T> reader)
      where S : Stream
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (reader == null)
        throw new ArgumentNullException("reader");

      stream.Position = offset;
      value = reader(stream);
      return stream;
    }

    public static S ReadAt<S, T>(this S stream, long offset, SeekOrigin origin, out T value, Func<S, T> reader)
      where S : Stream
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (reader == null)
        throw new ArgumentNullException("reader");

      stream.Seek(offset, origin);
      value = reader(stream);
      return stream;
    }

    public static S Write<S, T>(this S stream, T value, Action<S, T> writer)
      where S : Stream
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer(stream, value);
      return stream;
    }

    public static S WriteAt<S, T>(this S stream, long offset, T value, Action<S, T> writer)
      where S : Stream
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (writer == null)
        throw new ArgumentNullException("writer");

      stream.Position = offset;
      writer(stream, value);
      return stream;
    }

    public static S WriteAt<S, T>(this S stream, long offset, SeekOrigin origin, T value, Action<S, T> writer)
      where S : Stream
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (writer == null)
        throw new ArgumentNullException("writer");

      stream.Seek(offset, origin);
      writer(stream, value);
      return stream;
    }

    public static long Find(this Stream stream, long length, IList<byte> search, int index, int count, bool reverse)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (search == null)
        throw new ArgumentNullException("search");
      if (index < 0 && index > search.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 && count > search.Count - index)
        throw new ArgumentOutOfRangeException("count");

      long offset = 0L;
      int search_index = index + (reverse ? count - 1 : 0), search_count = 0, matched = 0;
      byte value = 0;
      while (matched < count && count <= length)
      {
        if (matched >= search_count)
        {
          int read = stream.ReadByte();
          if (read == -1)
            break;
          else
            value = (byte)read;
        }
        if (search[index + (reverse ? count - 1 - matched : matched)] == (matched < search_count && search_count > 1 ? search[search_index + (reverse ? -matched : matched)] : value))
        {
          matched++;
          if (matched == search_count)
            search_index = index + (reverse ? count - 1 : 0);
        }
        else
        {
          if (matched >= search_count)
            search_count = matched;
          else if (search_count > 0)
            search_count--;
          if (search_count == 0)
            search_index = index + (reverse ? count - 1 : 0);
          else if (reverse)
            search_index--;
          else
            search_index++;
          matched = 0;
          offset += 1;
          length -= 1;
        }
      }
      return matched == count ? offset : -1L;
    }

    public static long FindLast(this Stream stream, long length, IList<byte> search, int index, int count, bool reverse)
    {
      long offset = Find(new ReverseSearchStream(stream, true), length, search, index, count, !reverse);
      return offset < 0 || search.Count == 0 ? offset : offset + search.Count - 1;
    }

    public static long Find(this Stream stream, long length, int count, Func<int, int, byte, bool> match)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (match == null)
        throw new ArgumentNullException("match");
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");

      long offset = 0L;
      int search_index = 0, search_count = 0, matched = 0;
      byte value = 0;
      while (matched < count && count <= length)
      {
        if (matched >= search_count)
        {
          int read = stream.ReadByte();
          if (read == -1)
            break;
          else
            value = (byte)read;
        }
        if (match(matched, matched < search_count && search_count > 1 ? search_index + matched : -1, value))
        {
          matched++;
          if (matched == search_count)
            search_index = 0;
        }
        else
        {
          if (matched >= search_count)
            search_count = matched;
          else if (search_count > 0)
            search_count--;
          if (search_count == 0)
            search_index = 0;
          else
            search_index++;
          matched = 0;
          offset++;
          length--;
        }
      }
      return matched == count ? offset : -1L;
    }

    public static long FindLast(this Stream stream, long length, int count, Func<int, int, byte, bool> match)
    {
      long offset = Find(new ReverseSearchStream(stream, true), length, count, match);
      return offset < 0 || count == 0 ? offset : offset + count - 1;
    }

    public static long Find(this Stream stream, long length, long count, Func<long, long, byte, bool> match)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (match == null)
        throw new ArgumentNullException("match");
      if (count < 0L)
        throw new ArgumentOutOfRangeException("count");

      long offset = 0L;
      long search_index = 0L, search_count = 0L, matched = 0L;
      byte value = 0;
      while (matched < count && count <= length)
      {
        if (matched >= search_count)
        {
          int read = stream.ReadByte();
          if (read == -1)
            break;
          else
            value = (byte)read;
        }
        if (match(matched, matched < search_count && search_count > 1? search_index + matched : -1L, value))
        {
          matched++;
          if (matched == search_count)
            search_index = 0L;
        }
        else
        {
          if (matched >= search_count)
            search_count = matched;
          else if (search_count > 0L)
            search_count--;
          if (search_count == 0L)
            search_index = 0L;
          else
            search_index++;
          matched = 0L;
          offset += 1L;
          length -= 1L;
        }
      }
      return matched == count ? offset : -1L;
    }

    public static long FindLast(this Stream stream, long length, long count, Func<long, long, byte, bool> match)
    {
      long offset = Find(new ReverseSearchStream(stream, true), length, count, match);
      return offset < 0 || count == 0 ? offset : offset + count - 1;
    }

    public static int Compare(this Stream xStream, Stream yStream, long count, int buffSize, Comparison<byte> comparison)
    {
      if (xStream == null)
        throw new ArgumentNullException("xStream");
      if (yStream == null)
        throw new ArgumentNullException("yStream");
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      if (buffSize <= 0)
        throw new ArgumentOutOfRangeException("buffSize");
      if (comparison == null)
        comparison = Comparer<byte>.Default.Compare;

      byte[] xBuffer = new byte[buffSize];
      byte[] yBuffer = new byte[buffSize];
      long total = 0;
      int xRead, yRead, result;
      do
      {
        xRead = xStream.Read(xBuffer, 0, (int)Comparable.Min(xBuffer.Length, count - total));
        yRead = yStream.Read(yBuffer, 0, (int)Comparable.Min(yBuffer.Length, count - total));
        result = xBuffer.Take(xRead).SequenceCompare(yBuffer.Take(yRead), comparison);
        if (result == 0)
          total += Comparable.Max(xRead, yRead);
      }
      while (result == 0 && xRead > 0 && yRead > 0 && total < count);
      return result;
    }

    public static int Compare(this Stream xStream, Stream yStream, long count, int buffSize, IComparer<byte> comparer)
    {
      return Compare(xStream, yStream, count, buffSize, comparer != null ? comparer.Compare : (Comparison<byte>)null);
    }

    public static int Compare(this Stream xStream, Stream yStream, long count, int buffSize)
    {
      return Compare(xStream, yStream, count, buffSize, (Comparison<byte>)null);
    }

    public static long Copy(this Stream srcStream, Stream dstStream, long count, int buffSize, IProgress<long> progress, long unitSize)
    {
      if (srcStream == null)
        throw new ArgumentNullException("srcStream");
      if (dstStream == null)
        throw new ArgumentNullException("dstStream");
      if (srcStream == dstStream)
        throw new ArgumentException("Both streams are same");
      if (buffSize <= 0)
        throw new ArgumentOutOfRangeException("buffSize");
      if (unitSize < 0)
        throw new ArgumentOutOfRangeException("unitSize");
      if (!srcStream.CanRead)
        if (!srcStream.CanWrite)
          throw new ObjectDisposedException("srcStream");
        else
          throw new NotSupportedException();
      if (!dstStream.CanWrite)
        if (!dstStream.CanRead)
          throw new ObjectDisposedException(null);
        else
          throw new NotSupportedException("dstStream");

      long total = 0;
      long before = 0;
      int read;
      byte[] buffer = new byte[buffSize];
      while (total < count && (read = srcStream.Read(buffer, 0, (int)Comparable.Min(count - total, buffer.Length))) != 0)
      {
        dstStream.Write(buffer, 0, read);
        total += read;
        if (progress != null && total - before >= unitSize)
        {
          before = total;
          progress.Report(total);
        }
      }
      return total;
    }

    public static long Copy(this Stream srcStream, Stream dstStream, long count, int buffSize, IProgress<double> progress, double unitPart)
    {
      if (unitPart < 0 || unitPart > 1)
        throw new ArgumentOutOfRangeException("unitPart");

      return Copy(srcStream, dstStream, count, buffSize, progress != null ? new Progress<long>(t => progress.Report((double)t / (double)count)) : null, (long)(count * unitPart));
    }

    public static long Copy(this Stream srcStream, Stream dstStream, long count, int buffSize)
    {
      return Copy(srcStream, dstStream, count, buffSize, null, 0L);
    }

    public static long Move(this Stream stream, long destination, long count, int buffSize, ExpandMethod expandMethod, IProgress<long> progress, long unitSize)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      long source = stream.Position;
      if (destination < 0)
        throw new ArgumentOutOfRangeException("destination");
      if (count < 0 || count > stream.Length - source)
        throw new ArgumentOutOfRangeException("count");
      if (buffSize <= 0)
        throw new ArgumentOutOfRangeException("bufferSize");
      if (unitSize < 0)
        throw new ArgumentOutOfRangeException("unitSize");

      bool reverse = source < destination;
      long total = 0L;
      long before = 0L;
      int read;
      byte[] buffer = new byte[buffSize];
      if (reverse)
      {
        if (stream.Length < destination + count)
        {
          switch (expandMethod)
          {
            case ExpandMethod.SetLength:
              stream.SetLength(destination + count);
              break;
            case ExpandMethod.WriteZero:
              {
                stream.Position = stream.Length;
                long length = destination + count - stream.Length;
                while (length > 0L)
                {
                  int block = (int)Comparable.Min(length, (long)filler.Length);
                  stream.Write(filler, 0, block);
                  length -= block;
                }
              }
              break;
          }
        }
        stream.Position = source + count - Comparable.Min(count, buffer.Length);
      }
      while (total < count && (read = stream.Read(buffer, 0, (int)Comparable.Min(count - total, buffer.Length))) > 0)
      {
        stream.Position = destination + (reverse ? count - total - read : total);
        stream.Write(buffer, 0, read);
        stream.Position = source + (reverse ? count - total - read : total);
        total += read;
        if (progress != null && total - before >= unitSize)
        {
          before = total;
          progress.Report(total);
        }
      }
      return total;
    }

    public static void Move(this Stream stream, long destination, long count, int buffSize, ExpandMethod expandMethod, IProgress<double> progress, double unitPart)
    {
      if (unitPart < 0 || unitPart > 1)
        throw new ArgumentOutOfRangeException("unitPart");

      Move(stream, destination, count, buffSize, expandMethod, progress != null ? new Progress<long>(t => progress.Report((double)t / (double)count)) : null, (long)(count * unitPart));
    }

    public static long Move(this Stream stream, long destination, long count, int buffSize, IProgress<long> progress, long unitSize)
    {
      return Move(stream, destination, count, buffSize, ExpandMethod.None, progress, unitSize);
    }

    public static void Move(this Stream stream, long destination, long count, int buffSize, IProgress<double> progress, double unitPart)
    {
      if (unitPart < 0 || unitPart > 1)
        throw new ArgumentOutOfRangeException("unitPart");

      Move(stream, destination, count, buffSize, progress != null ? new Progress<long>(t => progress.Report((double)t / (double)count)) : null, (long)(count * unitPart));
    }

    public static long Move(this Stream stream, long destination, long count, int buffSize, ExpandMethod expandMethod)
    {
      return Move(stream, destination, count, buffSize, expandMethod, null, 0L);
    }

    public static long Move(this Stream stream, long destination, long count, int buffSize)
    {
      return Move(stream, destination, count, buffSize, null, 0L);
    }

    public static void Skip(this Stream stream, long count, int buffSize, bool readMethod)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      if (buffSize <= 0)
        throw new ArgumentOutOfRangeException("bufferSize");
      if (count == 0)
        return;
      if (!readMethod)
      {
        stream.Position += count;
        return;
      }
      byte[] buffer = new byte[Comparable.Min(count, buffSize)];
      while (count > 0)
      {
        int read = stream.Read(buffer, 0, (int)Comparable.Min(count, buffer.Length));
        if (read == 0)
          break;
        count -= read;
      }
    }

    public static IEnumerable<byte> Enumerate(this Stream stream, long offset, long count, int buffSize)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (offset < 0 || offset > stream.Length)
        throw new ArgumentOutOfRangeException("offset");
      if (count < 0 || count > stream.Length - offset)
        throw new ArgumentOutOfRangeException("count");

      int read;
      byte[] buffer = new byte[buffSize];
      stream.Position = offset;
      while ((read = stream.Read(buffer, 0, (int)Comparable.Min(count, buffer.Length))) > 0)
      {
        for (int i = 0; i < read; i++)
          yield return buffer[i];
        count -= read;
      }
    }
  }

  public enum ExpandMethod
  {
    None = 0,
    SetLength = 1,
    WriteZero = 2
  }
}
