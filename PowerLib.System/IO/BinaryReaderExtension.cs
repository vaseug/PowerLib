using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PowerLib.System.Linq;

namespace PowerLib.System.IO
{
	public static class BinaryReaderExtension
	{
    public static bool ThrowDataInfo = false;

    #region Read size methods

    public static int ReadSize(this BinaryReader reader, SizeEncoding sizeEncoding, Endian? endian = null)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      if (endian.HasValue)
        return reader.BaseStream.ReadSize(sizeEncoding, endian.Value);
      else
        switch (sizeEncoding)
        {
          case SizeEncoding.NO:
            return -1;
          case SizeEncoding.B1:
            return reader.ReadSByte();
          case SizeEncoding.B2:
            return reader.ReadInt16();
          case SizeEncoding.B4:
            return reader.ReadInt32();
          case SizeEncoding.B8:
            return Convert.ToInt32(reader.ReadInt64());
          case SizeEncoding.VB:
            return reader.ReadInt32Mb();
          default:
            throw new ArgumentOutOfRangeException("sizeEncoding");
        }
    }

    public static long ReadLongSize(this BinaryReader reader, SizeEncoding sizeEncoding, Endian? endian = null)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      if (endian.HasValue)
        return reader.BaseStream.ReadLongSize(sizeEncoding, endian.Value);
      else
        switch (sizeEncoding)
        {
          case SizeEncoding.NO:
            return -1L;
          case SizeEncoding.B1:
            return reader.ReadSByte();
          case SizeEncoding.B2:
            return reader.ReadInt16();
          case SizeEncoding.B4:
            return reader.ReadInt32();
          case SizeEncoding.B8:
            return reader.ReadInt64();
          case SizeEncoding.VB:
            return reader.ReadInt64Mb();
          default:
            throw new ArgumentOutOfRangeException("sizeEncoding");
        }
    }

    #endregion
    #region Read boolean

    public static Boolean ReadBoolean(this BinaryReader reader, TypeCode typeCode)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.BaseStream.ReadBoolean(typeCode);
    }

    public static Boolean ReadBoolean(this BinaryReader reader, TypeCode typeCode, Endian endian)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.BaseStream.ReadBoolean(typeCode, endian);
    }

    #endregion
    #region Read integers methods

    public static Int16 ReadInt16(this BinaryReader reader, Endian endian)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return reader.BaseStream.ReadInt16(endian);
		}

		public static Int32 ReadInt32(this BinaryReader reader, Endian endian)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return reader.BaseStream.ReadInt32(endian);
		}

		public static Int64 ReadInt64(this BinaryReader reader, Endian endian)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return reader.BaseStream.ReadInt64(endian);
		}

		public static UInt16 ReadUInt16(this BinaryReader reader, Endian endian)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return reader.BaseStream.ReadUInt16(endian);
		}

		public static UInt32 ReadUInt32(this BinaryReader reader, Endian endian)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return reader.BaseStream.ReadUInt32(endian);
		}

		public static UInt64 ReadUInt64(this BinaryReader reader, Endian endian)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return reader.BaseStream.ReadUInt64(endian);
		}

		public static Int16 ReadInt16Mb(this BinaryReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return reader.BaseStream.ReadInt16Vb();
		}

		public static Int32 ReadInt32Mb(this BinaryReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return reader.BaseStream.ReadInt32Vb();
		}

		public static Int64 ReadInt64Mb(this BinaryReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return reader.BaseStream.ReadInt64Vb();
		}

		public static UInt16 ReadUInt16Mb(this BinaryReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return reader.BaseStream.ReadUInt16Vb();
		}

		public static UInt32 ReadUInt32Mb(this BinaryReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return reader.BaseStream.ReadUInt32Vb();
		}

		public static UInt64 ReadUInt64Mb(this BinaryReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return reader.BaseStream.ReadUInt64Vb();
		}

    #endregion
    #region Read real methods

    public static Single ReadSingle(this BinaryReader reader, Endian endian)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.BaseStream.ReadSingle(endian);
    }

    public static Double ReadDouble(this BinaryReader reader, Endian endian)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.BaseStream.ReadDouble(endian);
    }

    #endregion
    #region Read string

    public static Char[] ReadChars(this BinaryReader reader, Encoding encoding, int length)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (encoding == null)
        throw new ArgumentNullException("encoding");
      if (length <= 0)
        throw new ArgumentOutOfRangeException("count");

      return encoding.GetChars(reader.ReadBytes(length));
    }

    public static Char[] ReadChars(this BinaryReader reader, Encoding encoding, SizeEncoding sizeEncoding)
    {
      return reader.ReadChars(encoding, reader.ReadSize(sizeEncoding));
    }

    public static String ReadString(this BinaryReader reader, Encoding encoding, int length)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      return encoding.GetString(reader.ReadBytes(length));
    }

    public static String ReadString(this BinaryReader reader, Encoding encoding, SizeEncoding sizeEncoding)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      return encoding.GetString(reader.ReadBytes(reader.ReadSize(sizeEncoding)));
    }

    public static String ReadString(this BinaryReader reader, Encoding encoding, string terminator, bool omitTerm)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (encoding == null)
        throw new ArgumentNullException("encoding");
      if (string.IsNullOrEmpty(terminator))
        throw new ArgumentException("Value is not specified", "terminator");

      return encoding.GetString(reader.BaseStream.ReadBytes(encoding.GetBytes(terminator), omitTerm));
    }

    public static IEnumerable<String> ReadLines(this BinaryReader reader, Encoding encoding, IList<String> terminatorsList, String terminatorStub, int maxCount)
    {
      return reader.BaseStream.ReadLines(encoding, terminatorsList, terminatorStub, maxCount);
    }

    #endregion
    #region Read enum methods

    public static T ReadEnum<T>(this BinaryReader reader)
      where T : struct, IComparable, IFormattable, IConvertible
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (!typeof(T).IsEnum)
        throw new InvalidOperationException(string.Format("Type '{0}' is not enum.", typeof(T).FullName));

      switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))))
      {
        case TypeCode.Byte:
          return (T)Enum.ToObject(typeof(T), reader.ReadByte());
        case TypeCode.UInt16:
          return (T)Enum.ToObject(typeof(T), reader.ReadUInt16());
        case TypeCode.UInt32:
          return (T)Enum.ToObject(typeof(T), reader.ReadUInt32());
        case TypeCode.UInt64:
          return (T)Enum.ToObject(typeof(T), reader.ReadUInt64());
        case TypeCode.SByte:
          return (T)Enum.ToObject(typeof(T), reader.ReadSByte());
        case TypeCode.Int16:
          return (T)Enum.ToObject(typeof(T), reader.ReadInt16());
        case TypeCode.Int32:
          return (T)Enum.ToObject(typeof(T), reader.ReadInt32());
        case TypeCode.Int64:
          return (T)Enum.ToObject(typeof(T), reader.ReadInt64());
        default:
          throw new InvalidOperationException("Invalid enum underlying type");
      }
    }

    public static T ReadEnum<T>(this BinaryReader reader, Endian endian)
      where T : struct, IComparable, IFormattable, IConvertible
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (!typeof(T).IsEnum)
        throw new InvalidOperationException(string.Format("Type '{0}' is not enum.", typeof(T).FullName));

      switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))))
      {
        case TypeCode.Byte:
          return (T)Enum.ToObject(typeof(T), reader.ReadByte());
        case TypeCode.UInt16:
          return (T)Enum.ToObject(typeof(T), reader.ReadUInt16(endian));
        case TypeCode.UInt32:
          return (T)Enum.ToObject(typeof(T), reader.ReadUInt32(endian));
        case TypeCode.UInt64:
          return (T)Enum.ToObject(typeof(T), reader.ReadUInt64(endian));
        case TypeCode.SByte:
          return (T)Enum.ToObject(typeof(T), reader.ReadSByte());
        case TypeCode.Int16:
          return (T)Enum.ToObject(typeof(T), reader.ReadInt16(endian));
        case TypeCode.Int32:
          return (T)Enum.ToObject(typeof(T), reader.ReadInt32(endian));
        case TypeCode.Int64:
          return (T)Enum.ToObject(typeof(T), reader.ReadInt64(endian));
        default:
          throw new InvalidOperationException("Invalid enum underlying type");
      }
    }

    #endregion
    #region Read others methods

    public static Guid ReadGuid(this BinaryReader reader)
    {
      return new Guid(reader.ReadBytes(16));
    }

    public static DateTime ReadDateTime(this BinaryReader reader)
    {
      return new DateTime(reader.ReadInt64());
    }

    public static DateTime ReadDateTime(this BinaryReader reader, Endian endian)
    {
      return new DateTime(reader.ReadInt64(endian));
    }

    public static TimeSpan ReadTimeSpan(this BinaryReader reader)
    {
      return new TimeSpan(reader.ReadInt64());
    }

    public static TimeSpan ReadTimeSpan(this BinaryReader reader, Endian endian)
    {
      return new TimeSpan(reader.ReadInt64(endian));
    }

    #endregion
    #region Read structured methods

    public static T? ReadNullable<T>(this BinaryReader reader, Func<BinaryReader, T> typeReader)
      where T : struct
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (typeReader == null)
        throw new ArgumentNullException("typeReader");

      return reader.ReadBoolean() ? typeReader(reader) : default(T?);
    }

    public static T ReadObject<T>(this BinaryReader reader, Func<BinaryReader, T> typeReader)
      where T : class
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (typeReader == null)
        throw new ArgumentNullException("typeReader");

      return reader.ReadBoolean() ? typeReader(reader) : default(T);
    }

    public static T? ReadNullable<T>(this BinaryReader reader, Func<BinaryReader, T> typeReader, Func<BinaryReader, Boolean> flagReader)
      where T : struct
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (typeReader == null)
        throw new ArgumentNullException("typeReader");
      if (flagReader == null)
        throw new ArgumentNullException("flagReader");

      return flagReader(reader) ? typeReader(reader) : default(T?);
    }

    public static T ReadObject<T>(this BinaryReader reader, Func<BinaryReader, T> typeReader, Func<BinaryReader, Boolean> flagReader)
      where T : class
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (typeReader == null)
        throw new ArgumentNullException("typeReader");
      if (flagReader == null)
        throw new ArgumentNullException("flagReader");

      return flagReader(reader) ? typeReader(reader) : default(T);
    }

    #endregion
    #region Read bytes methods

    public static byte[] ReadBytes(this BinaryReader reader, SizeEncoding sizeEncoding)
    {
      return reader.ReadBytes(reader.ReadSize(sizeEncoding));
    }

    #endregion
    #region Read collection methods

    public static IEnumerable<T> ReadCollection<T>(this BinaryReader reader, Func<BinaryReader, T> itemReader, int count)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      return reader.ReadCollection((rd, i) => itemReader(reader), count);
    }

    public static IEnumerable<T> ReadCollection<T>(this BinaryReader reader, Func<BinaryReader, int, T> itemReader, int count)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      int total = 0;
      try
      {
        return PwrEnumerable.Produce(count, i => itemReader(reader, i));
      }
      catch (Exception ex) when (ThrowDataInfo)
      {
        throw new StreamLoadException(total, ex);
      }
    }

    #endregion
    #region Read array methods

    public static T[] ReadArray<T>(this BinaryReader reader, Func<BinaryReader, T> itemReader, int length)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      return reader.ReadArray((rd, i) => itemReader(rd), length);
    }

    public static T[] ReadArray<T>(this BinaryReader reader, Func<BinaryReader, int, T> itemReader, int length)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      T[] array = new T[length];
      int total = 0;
      try
      {
        array.Fill(i => { T v = itemReader(reader, i); total = i + 1; return v; });
      }
      catch (Exception ex) when (ThrowDataInfo)
      {
        throw new StreamReadException(array, total, ex);
      }
      return array;
    }

    #endregion
    #region Read regular array methods

    public static Array ReadRegularArray<T>(this BinaryReader reader, Func<BinaryReader, T> itemReader, int[] lengths, int[] lowerBounds = null)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      return reader.ReadRegularArray((rd, fi, di) => itemReader(rd), null, lengths, lowerBounds);
    }

    public static Array ReadRegularArray<T>(this BinaryReader reader, Func<BinaryReader, int, int[], T> itemReader, int[] indices, int[] lengths, int[] lowerBounds = null)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      Array array = PwrArray.CreateAsRegular<T>(lengths, lowerBounds);
      int total = 0;
      try
      {
        array.FillAsRegular((fi, di) => { var v = itemReader(reader, fi, di); total = fi + 1; return v; }, indices);
      }
      catch (Exception ex) when (ThrowDataInfo)
      {
        throw new StreamReadException(array, total, ex);
      }
      return array;
    }

    public static Array ReadRegularArray<T>(this BinaryReader reader, Func<BinaryReader, T> itemReader, ArrayDimension[] dimensions)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      return reader.ReadRegularArray<T>((rd, fi, di) => itemReader(rd), null, dimensions);
    }

    public static Array ReadRegularArray<T>(this BinaryReader reader, Func<BinaryReader, int, int[], T> itemReader, int[] indices, ArrayDimension[] dimensions)
    {
      if (dimensions == null)
        throw new ArgumentNullException("dimensions");

      return reader.ReadRegularArray(itemReader, indices, dimensions.Select(t => t.Length).ToArray(), dimensions.Select(t => t.LowerBound).ToArray());
    }

    #endregion
    #region Read long regular array methods

    public static Array ReadLongRegularArray<T>(this BinaryReader reader, Func<BinaryReader, T> itemReader, long[] lengths)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      return reader.ReadLongRegularArray<T>((rd, fi, di) => itemReader(rd), null, lengths);
    }

    public static Array ReadLongRegularArray<T>(this BinaryReader reader, Func<BinaryReader, long, long[], T> itemReader, long[] indices, long[] lengths)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      Array array = PwrArray.CreateAsLongRegular<T>(lengths);
      long total = 0L;
      try
      {
        array.FillAsLongRegular((fi, di) => { var v = itemReader(reader, fi, di); total = fi + 1L; return v; }, indices);
      }
      catch (Exception ex) when (ThrowDataInfo)
      {
        throw new StreamReadLongException(array, total, ex);
      }
      return array;
    }

    #endregion
    #region Load array methods

    public static void LoadArray<T>(this BinaryReader reader, T[] array, Func<BinaryReader, T> itemReader)
    {
      reader.LoadArray(array, itemReader, 0, array != null ? array.Length : 0);
    }

    public static void LoadArray<T>(this BinaryReader reader, T[] array, Func<BinaryReader, int, T> itemReader)
    {
      reader.LoadArray(array, itemReader, 0, array != null ? array.Length : 0);
    }

    public static void LoadArray<T>(this BinaryReader reader, T[] array, Func<BinaryReader, T> itemReader, Range range)
    {
      reader.LoadArray(array, itemReader, range.Index, range.Count);
    }

    public static void LoadArray<T>(this BinaryReader reader, T[] array, Func<BinaryReader, int, T> itemReader, Range range)
    {
      reader.LoadArray(array, itemReader, range.Index, range.Count);
    }

    public static void LoadArray<T>(this BinaryReader reader, T[] array, Func<BinaryReader, T> itemReader, int index, int count)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      reader.LoadArray(array, itemReader, index, count);
    }

    public static void LoadArray<T>(this BinaryReader reader, T[] array, Func<BinaryReader, int, T> itemReader, int index, int count)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      int total = 0;
      try
      {
        array.Fill(i => { var v = itemReader(reader, i); total = i + 1; return v; }, index, count);
      }
      catch (Exception ex) when (ThrowDataInfo)
      {
        throw new StreamLoadException(total, ex);
      }
    }

    #endregion
    #region Load regular array methods

    public static void LoadRegularArray<T>(this BinaryReader reader, Array array, Func<BinaryReader, T> itemReader, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      reader.LoadRegularArray<T>(array, (rd, fi, di) => itemReader(rd), null, zeroBased, range, ranges);
    }

    public static void LoadRegularArray<T>(this BinaryReader reader, Array array, Func<BinaryReader, int, int[], T> itemReader, int[] indices, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      int total = 0;
      try
      {
        array.FillAsRegular((fi, di) => { var v = itemReader(reader, fi, di); total = fi + 1; return v; }, indices, zeroBased, range, ranges);
      }
      catch (Exception ex) when (ThrowDataInfo)
      {
        throw new StreamLoadException(total, ex);
      }
    }

    #endregion
    #region Load long regular array methods

    public static void LoadLongRegularArray<T>(this BinaryReader reader, Array array, Func<BinaryReader, T> itemReader, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      reader.LoadLongRegularArray<T>(array, (rd, fi, di) => itemReader(rd), null, zeroBased, range, ranges);
    }

    public static void LoadLongRegularArray<T>(this BinaryReader reader, Array array, Func<BinaryReader, long, long[], T> itemReader, long[] indices, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      long total = 0;
      try
      {
        array.FillAsLongRegular((fi, di) => { var v = itemReader(reader, fi, di); total = fi + 1L; return v; }, indices, zeroBased, range, ranges);
      }
      catch (Exception ex) when (ThrowDataInfo)
      {
        throw new StreamLoadLongException(total, ex);
      }
    }

    #endregion
  }
}
