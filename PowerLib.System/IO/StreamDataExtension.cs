using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PowerLib.System.Collections;
using PowerLib.System.Linq;
using PowerLib.System.Text;

namespace PowerLib.System.IO
{
	using Math = global::System.Math;

	public static class StreamDataExtension
  {
    private const int GuidSize = 16;

    #region Option handlers

    public static Func<Stream, bool> ThrowData { private get; set; }

    public static Func<Stream, bool> StreamExpand { private get; set; }

    #endregion
    #region Constants

    public const int ReadBufferMinSize = 128;
		public const int ReadBufferMaxSize = 65536;
		public const int InitBufferMinSize = 128;
		public const int NoCheckEndMinSize = 16;

		private static int readBufferDefaultSize = 1024;
		private static int initBufferDefaultSize = InitBufferMinSize;
		private static int noCheckEndDefaultSize = NoCheckEndMinSize;

		#endregion
		#region Properties

		public static int ReadBufferDefaultSize
		{
			get { return readBufferDefaultSize; }
			set
			{
				if (value < ReadBufferMinSize || value > ReadBufferMaxSize)
					throw new ArgumentOutOfRangeException();
				readBufferDefaultSize = value ;
			}
		}

		public static int InitBufferDefaultSize
		{
			get { return initBufferDefaultSize; }
			set
			{
				if (value < InitBufferMinSize)
					throw new ArgumentOutOfRangeException();
				initBufferDefaultSize = value ;
			}
		}

		public static int NoCheckEndDefaultSize
		{
			get { return noCheckEndDefaultSize; }
			set
			{
				if (value < NoCheckEndMinSize)
					throw new ArgumentOutOfRangeException();
				noCheckEndDefaultSize = value ;
			}
		}

    #endregion
    #region General methods

    public static int ReadMost(this Stream stream, byte[] buffer, int offset, int count)
		{
			if (stream == null)
				throw new NullReferenceException("stream");
			if (offset < 0 || offset > buffer.Length)
				throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || count > buffer.Length - offset)
				throw new ArgumentOutOfRangeException("count");

			if (count == 0)
				return 0;
			int total = 0;
			for (int read = 0; count > 0; total += read, offset += read, count -= read)
				if ((read = stream.Read(buffer, offset, count)) == 0)
					break;
			return total;
		}

		public static bool ReadExact(this Stream stream, byte[] buffer, int offset, int count, bool noAcceptEnd)
		{
			int read = ReadMost(stream, buffer, offset, count);
			if (read == 0 && noAcceptEnd || read > 0 && read < count)
				throw new EndOfStreamException();
			return read == count;
		}

		public static byte[] ReadBytesMost(this Stream stream, int initSize, int maxCount)
		{
			return stream.ReadBytes(initSize, maxCount, initSize <= noCheckEndDefaultSize ? StreamReadOptions.NoCheckEnd : 0);
		}

		public static byte[] ReadBytesExact(this Stream stream, int count, bool noAcceptEnd)
		{
			return stream.ReadBytes(count, count, StreamReadOptions.ExactSize | (noAcceptEnd ? StreamReadOptions.NoAcceptEnd : 0) | (count <= noCheckEndDefaultSize ? StreamReadOptions.NoCheckEnd : 0));
		}

		public static byte[] ReadBytesMost(this Stream stream, int initSize, int maxCount, byte[] terminator, bool omitTerm)
		{
			return stream.ReadBytes(initSize, maxCount, terminator, (omitTerm ? StreamReadOptions.OmitTerminator : 0) | (initSize <= noCheckEndDefaultSize ? StreamReadOptions.NoCheckEnd : 0));
		}

		public static byte[] ReadBytesExact(this Stream stream, int count, bool noAcceptEnd, byte[] terminator, bool omitTerm)
		{
			return stream.ReadBytes(count, count, terminator, (omitTerm ? StreamReadOptions.OmitTerminator : 0) | (noAcceptEnd ? StreamReadOptions.NoAcceptEnd : 0) | (count <= noCheckEndDefaultSize ? StreamReadOptions.NoCheckEnd : 0));
		}

		public static byte[] ReadBytes(this Stream stream, int initSize, int maxCount, StreamReadOptions readOptions)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (initSize < 0)
				throw new ArgumentOutOfRangeException("initSize");
			if (maxCount < 0)
				throw new ArgumentOutOfRangeException("maxCount");
			if (initSize > maxCount)
				throw new ArgumentException("Inconsistent initial size and max count values");

			if (maxCount == 0)
				return new byte[0];
			int total = 0;
			byte[] buffer = new byte[(readOptions & StreamReadOptions.NoCheckEnd) != 0 ? initSize : 1];
			try
			{
				total = stream.Read(buffer, 0, buffer.Length);
				for (int read = 0; total > 0 && total < maxCount; total += read)
				{
					if (buffer.Length == total)
						buffer = buffer.Resize(buffer.Length < initSize ? initSize : (buffer.Length > maxCount - buffer.Length) ? maxCount : buffer.Length * 2);
					if ((read = stream.Read(buffer, total, buffer.Length - total)) == 0)
						break;
				}
				if (total == maxCount)
					return buffer;
				else if (total == 0 && (readOptions & StreamReadOptions.NoAcceptEnd) == 0)
					return null;
				else if (total != 0 && (readOptions & StreamReadOptions.ExactSize) == 0)
					return buffer.Resize(total);
				else
					throw new EndOfStreamException();
			}
			catch (Exception ex) when ((readOptions & StreamReadOptions.ThrowBuffer) != 0)
			{
			  throw new StreamReadException(buffer, total, ex);
			}
		}

		public static byte[] ReadBytes(this Stream stream, int initSize, int maxCount, byte[] terminator, StreamReadOptions readOptions)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (initSize < 0)
				throw new ArgumentOutOfRangeException("initSize");
			if (maxCount < 0)
				throw new ArgumentOutOfRangeException("maxCount");
			if (initSize > maxCount)
				throw new ArgumentException("Inconsistent initial size and max count values");
			if (terminator == null)
				throw new ArgumentNullException("terminator");
			if (terminator.Length == 0)
				throw new ArgumentException("Empty array", "terminator");

			if (maxCount == 0)
				return new byte[0];
			int total = 0;
			byte[] buffer = new byte[(readOptions & StreamReadOptions.NoCheckEnd) != 0 ? initSize : 1];
			try
			{
				total = stream.Read(buffer, 0, 1);
				int match = total > 0 && buffer[0] == terminator[0] ? 1 : 0;
				for (int read = 0; total > 0 && total < maxCount && match != terminator.Length; total += read)
				{
					if (buffer.Length == total)
						buffer = buffer.Resize(buffer.Length < initSize ? initSize : buffer.Length > maxCount - buffer.Length ? maxCount : buffer.Length * 2);
					if ((read = stream.Read(buffer, total, 1)) == 0)
						break;
					match = buffer[total] == terminator[match] ? match + 1 : 0;
				}
				if (match == terminator.Length)
				{
					if (total == maxCount && (readOptions & StreamReadOptions.OmitTerminator) == 0)
						return buffer;
					else if (total != maxCount && (readOptions & StreamReadOptions.ExactSize) != 0)
						throw new InvalidDataException("Unexpected data size");
					else
						return buffer.Resize(total - match);
				}
				else if (total == 0 && (readOptions & StreamReadOptions.NoAcceptEnd) == 0)
					return null;
				else if (total == maxCount)
					throw new InternalBufferOverflowException();
				else
					throw new EndOfStreamException();
			}
			catch (Exception ex) when ((readOptions & StreamReadOptions.ThrowBuffer) != 0)
			{
			  throw new StreamReadException(buffer, total, ex);
			}
		}

    public static void WriteBytes(this Stream stream, byte[] buffer, StreamWriteOptions writeOptions)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      if ((writeOptions & StreamWriteOptions.Expand) != 0 && buffer.Length > stream.Length - stream.Position)
        stream.SetLength(stream.Position + buffer.Length);
      stream.Write(buffer, 0, buffer.Length);
    }

    public static void WriteBytes(this Stream stream, byte[] buffer)
		{
      //      stream.WriteBytes(buffer, StreamExpand != null && StreamExpand(stream) ? StreamWriteOptions.Expand : 0);
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      stream.Write(buffer, 0, buffer.Length);
    }

    #endregion
    #region Read methods
    #region Read size methods

    public static Int32 ReadSize(this Stream stream, SizeEncoding sizeEncoding, Endian endian = Endian.Default)
		{
			switch (sizeEncoding)
			{
				case SizeEncoding.NO:
					return 0;
				case SizeEncoding.B1:
					return stream.ReadUByte();
				case SizeEncoding.B2:
					return stream.ReadUInt16(endian);
				case SizeEncoding.B4:
					return stream.ReadInt32(endian);
				case SizeEncoding.B8:
					return Convert.ToInt32(stream.ReadInt64(endian));
				case SizeEncoding.VB:
					return stream.ReadInt32Vb();
				default:
					throw new ArgumentOutOfRangeException("sizeEncoding");
			}
		}

		public static Int64 ReadLongSize(this Stream stream, SizeEncoding sizeEncoding, Endian endian = Endian.Default)
		{
			switch (sizeEncoding)
			{
				case SizeEncoding.NO:
					return 0L;
				case SizeEncoding.B1:
					return stream.ReadUByte();
				case SizeEncoding.B2:
					return stream.ReadUInt16(endian);
				case SizeEncoding.B4:
					return stream.ReadUInt32(endian);
				case SizeEncoding.B8:
					return stream.ReadInt64(endian);
				case SizeEncoding.VB:
					return stream.ReadInt64Vb();
				default:
					throw new ArgumentOutOfRangeException("sizeEncoding");
			}
		}

    #endregion
    #region Read encoding methods

    #endregion
    #region Read base types methods

    public static SByte ReadSByte(this Stream stream)
		{
			return (SByte)stream.ReadBytes(sizeof(SByte), sizeof(SByte), StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0))[0];
		}

		public static Byte ReadUByte(this Stream stream)
		{
			return stream.ReadBytes(sizeof(Byte), sizeof(Byte), StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0))[0];
		}

		public static Int16 ReadInt16(this Stream stream, Endian endian = Endian.Default)
		{
			byte[] buffer = stream.ReadBytes(sizeof(Int16), sizeof(Int16), StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
			if (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert)
				Array.Reverse(buffer);
			return BitConverter.ToInt16(buffer, 0);
		}

		public static Int32 ReadInt32(this Stream stream, Endian endian = Endian.Default)
		{
			byte[] buffer = stream.ReadBytes(sizeof(Int32), sizeof(Int32), StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd | 
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
			if (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert)
				Array.Reverse(buffer);
			return BitConverter.ToInt32(buffer, 0);
		}

		public static Int64 ReadInt64(this Stream stream, Endian endian = Endian.Default)
		{
			byte[] buffer = stream.ReadBytes(sizeof(Int64), sizeof(Int64), StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
			if (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert)
				Array.Reverse(buffer);
			return BitConverter.ToInt64(buffer, 0);
		}

		public static UInt16 ReadUInt16(this Stream stream, Endian endian = Endian.Default)
		{
			byte[] buffer = stream.ReadBytes(sizeof(UInt16), sizeof(UInt16), StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
			if (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert)
				Array.Reverse(buffer);
			return BitConverter.ToUInt16(buffer, 0);
		}

		public static UInt32 ReadUInt32(this Stream stream, Endian endian = Endian.Default)
		{
			byte[] buffer = stream.ReadBytes(sizeof(UInt32), sizeof(UInt32), StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
			if (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert)
				Array.Reverse(buffer);
			return BitConverter.ToUInt32(buffer, 0);
		}

		public static UInt64 ReadUInt64(this Stream stream, Endian endian = Endian.Default)
		{
			byte[] buffer = stream.ReadBytes(sizeof(UInt64), sizeof(UInt64), StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
			if (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert)
				Array.Reverse(buffer);
			return BitConverter.ToUInt64(buffer, 0);
		}

		public static Int16 ReadInt16Vb(this Stream stream)
		{
			Int16 value = 0;
			for (int i = 0, r = sizeof(Int16) * 8 - 1; r > 0; i += 7)
			{
				byte read = stream.ReadUByte();
				if (read < 0x80)
					r = 0;
				else if (r > 7)
					r -= 7;
				else
					throw new InvalidOperationException("Bad multi-byte integer format");
				value |= (Int16)((read & ~0x80) << i);
			}
			return value ;
		}

		public static Int32 ReadInt32Vb(this Stream stream)
		{
			Int32 value = 0;
			for (int i = 0, r = sizeof(Int32) * 8 - 1; r > 0; i += 7)
			{
				byte read = stream.ReadUByte();
				if (read < 0x80)
					r = 0;
				else if (r > 7)
					r -= 7;
				else
					throw new InvalidOperationException("Bad multi-byte integer format");
				value |= (read & ~0x80) << i;
			}
			return value ;
		}

		public static Int64 ReadInt64Vb(this Stream stream)
		{
			Int64 value = 0;
			for (int i = 0, r = sizeof(Int64) * 8 - 1; r > 0; i += 7)
			{
				byte read = stream.ReadUByte();
				if (read < 0x80)
					r = 0;
				else if (r > 7)
					r -= 7;
				else
					throw new InvalidOperationException("Bad multi-byte integer format");
				value |= (read & ~0x80L) << i;
			}
			return value ;
		}

		public static UInt16 ReadUInt16Vb(this Stream stream)
		{
			UInt16 value = 0;
			for (int i = 0, r = sizeof(UInt16) * 8; r > 0; i += 7)
			{
				byte read = stream.ReadUByte();
				if (read < 0x80)
					r = 0;
				else if (r > 7)
					r -= 7;
				else
					throw new InvalidOperationException("Bad multi-byte integer format");
				value |= (UInt16)((read & ~0x80) << i);
			}
			return value ;
		}

		public static UInt32 ReadUInt32Vb(this Stream stream)
		{
			UInt32 value = 0U;
			for (int i = 0, r = sizeof(UInt32) * 8; r > 0; i += 7)
			{
				byte read = stream.ReadUByte();
				if (read < 0x80)
					r = 0;
				else if (r > 7)
					r -= 7;
				else
					throw new InvalidOperationException("Bad multi-byte integer format");
				value |= (read & ~0x80U) << i;
			}
			return value ;
		}

		public static UInt64 ReadUInt64Vb(this Stream stream)
		{
			UInt64 value = 0UL;
			for (int i = 0, r = sizeof(UInt64) * 8; r > 0; i += 7)
			{
				byte read = stream.ReadUByte();
				if (read < 0x80)
					r = 0;
				else if (r > 7)
					r -= 7;
				else
					throw new InvalidOperationException("Bad multi-byte integer format");
				value |= (read & ~0x80UL) << i;
			}
			return value ;
		}

		public static Single ReadSingle(this Stream stream, Endian endian = Endian.Default)
		{
      byte[] buffer = stream.ReadBytes(sizeof(Single), sizeof(Single), StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
			if (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert)
				Array.Reverse(buffer);
			return BitConverter.ToSingle(buffer, 0);
		}

		public static Double ReadDouble(this Stream stream, Endian endian = Endian.Default)
		{
      byte[] buffer = stream.ReadBytes(sizeof(Double), sizeof(Double), StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
			if (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert)
				Array.Reverse(buffer);
			return BitConverter.ToDouble(buffer, 0);
		}

		public static Decimal ReadDecimal(this Stream stream)
		{
			byte[] buffer = stream.ReadBytes(sizeof(int) * 4, sizeof(int) * 4, StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
			return new Decimal(Enumerable.Range(0, 4).Select(i => BitConverter.ToInt32(buffer, i * sizeof(int))).ToArray());
		}

		public static Guid ReadGuid(this Stream stream)
		{
			return new Guid(stream.ReadBytes(16, 16, StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0)));
		}

		public static DateTime ReadDateTime(this Stream stream, Endian endian = Endian.Default)
		{
			return new DateTime(stream.ReadInt64(endian));
		}

		public static TimeSpan ReadTimeSpan(this Stream stream, Endian endian = Endian.Default)
		{
			return new TimeSpan(stream.ReadInt64(endian));
		}

		public static Boolean ReadBoolean(this Stream stream, TypeCode typeCode, Endian endian = Endian.Default)
		{
			switch (typeCode)
			{
				case TypeCode.Byte:
					return stream.ReadUByte() != 0;
				case TypeCode.UInt16:
					return stream.ReadUInt16(endian) != 0;
				case TypeCode.UInt32:
					return stream.ReadUInt32(endian) != 0U;
				case TypeCode.UInt64:
					return stream.ReadUInt64(endian) != 0UL;
				case TypeCode.SByte:
					return stream.ReadSByte() != 0;
				case TypeCode.Int16:
					return stream.ReadInt16(endian) != 0;
				case TypeCode.Int32:
					return stream.ReadInt32(endian) != 0;
				case TypeCode.Int64:
					return stream.ReadInt64(endian) != 0L;
				default:
					throw new ArgumentOutOfRangeException("Invalid argument TypeCode value ", "typeCode");
			}
		}

    public static Char ReadChar(this Stream stream, Encoding encoding)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      var decoder = encoding.GetDecoder();
      var bytes = new byte[1];
      var chars = new char[1];
      do
        if (stream.Read(bytes, 0, bytes.Length) == 0)
          throw new EndOfStreamException();
      while (decoder.GetChars(bytes, 0, bytes.Length, chars, 0) == 0);
      return chars[0];
    }

    public static T ReadEnum<T>(this Stream stream, Endian endian = Endian.Default)
			where T : struct, IComparable, IFormattable, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new InvalidOperationException(string.Format("Type '{0}' is not enum.", typeof(T).FullName));

			switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))))
			{
				case TypeCode.Byte:
					return (T)Enum.ToObject(typeof(T), stream.ReadUByte());
				case TypeCode.UInt16:
					return (T)Enum.ToObject(typeof(T), stream.ReadUInt16(endian));
				case TypeCode.UInt32:
					return (T)Enum.ToObject(typeof(T), stream.ReadUInt32(endian));
				case TypeCode.UInt64:
					return (T)Enum.ToObject(typeof(T), stream.ReadUInt64(endian));
				case TypeCode.SByte:
					return (T)Enum.ToObject(typeof(T), stream.ReadSByte());
				case TypeCode.Int16:
					return (T)Enum.ToObject(typeof(T), stream.ReadInt16(endian));
				case TypeCode.Int32:
					return (T)Enum.ToObject(typeof(T), stream.ReadInt32(endian));
				case TypeCode.Int64:
					return (T)Enum.ToObject(typeof(T), stream.ReadInt64(endian));
				default:
					throw new InvalidOperationException("Invalid enum underlying type");
			}
		}

    public static Char[] ReadChars(this Stream stream, Encoding encoding, int length)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (encoding == null)
				throw new ArgumentNullException("encoding");
			if (length <= 0)
				throw new ArgumentOutOfRangeException("count");

			return encoding.GetChars(stream.ReadBytes(length));
		}

    public static Char[] ReadChars(this Stream stream, Encoding encoding, SizeEncoding sizeEncoding, Endian endian = Endian.Default)
    {
      return stream.ReadChars(encoding, stream.ReadSize(sizeEncoding, endian));
    }

    public static Char[] ReadChars(this Stream stream, Encoding encoding, string terminator, bool omitTerm)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");
      if (string.IsNullOrEmpty(terminator))
        throw new ArgumentException("Value is not specified", "terminator");

      return encoding.GetChars(stream.ReadBytes(encoding.GetBytes(terminator), omitTerm));
    }

    public static String ReadString(this Stream stream, Encoding encoding, int length)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (encoding == null)
				throw new ArgumentNullException("encoding");

			return encoding.GetString(stream.ReadBytes(length));
		}

    public static String ReadString(this Stream stream, Encoding encoding, SizeEncoding sizeEncoding, Endian endian = Endian.Default)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      return encoding.GetString(stream.ReadBytes(stream.ReadSize(sizeEncoding, endian)));
    }

    public static String ReadString(this Stream stream, Encoding encoding, string terminator, bool omitTerm)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (encoding == null)
				throw new ArgumentNullException("encoding");
			if (string.IsNullOrEmpty(terminator))
				throw new ArgumentException("Value is not specified", "terminator");

			return encoding.GetString(stream.ReadBytes(encoding.GetBytes(terminator), omitTerm));
		}

		public static byte[] ReadBytes(this Stream stream, int length)
		{
			return stream.ReadBytes(length, length, StreamReadOptions.ExactSize | StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
		}

		public static byte[] ReadBytes(this Stream stream, SizeEncoding sizeEncoding)
		{
			return stream.ReadBytes(stream.ReadSize(sizeEncoding));
		}

    public static byte[] ReadBytes(this Stream stream, byte[] terminator, bool omitTerm)
    {
      return stream.ReadBytes(initBufferDefaultSize, int.MaxValue, terminator, StreamReadOptions.NoAcceptEnd | StreamReadOptions.NoCheckEnd | (omitTerm ? StreamReadOptions.OmitTerminator : 0) |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
    }

    public static T ReadObject<T>(this Stream stream, Func<Stream, T> typeReader, Func<Stream, Boolean> flagReader)
      where T : class
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (typeReader == null)
        throw new ArgumentNullException("typeReader");
      if (flagReader == null)
        throw new ArgumentNullException("flagReader");

      return flagReader(stream) ? typeReader(stream) : default(T);
    }

    public static T? ReadNullable<T>(this Stream stream, Func<Stream, T> typeReader, Func<Stream, Boolean> flagReader)
      where T : struct
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (typeReader == null)
        throw new ArgumentNullException("typeReader");
      if (flagReader == null)
        throw new ArgumentNullException("flagReader");

      return flagReader(stream) ? typeReader(stream) : default(T?);
    }

    public static T ReadVoid<T>(this Stream stream, T value)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			stream.Read(new byte[0], 0, 0);
      return value;
		}

    public static T ReadAt<T>(this Stream stream, long position, Func<Stream, T> typeReader)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (typeReader == null)
        throw new ArgumentNullException("typeReader");

      var temp = stream.Position;
      stream.Position = position;
      try
      {
        return typeReader(stream);
      }
      finally
      {
        stream.Position = temp;
      }
    }

    #endregion
    #region Read array methods

    public static T[] ReadArray<T>(this Stream stream, Func<Stream, T> itemReader, int length)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      return stream.ReadArray((s, i) => itemReader(s), length);
    }

    public static T[] ReadArray<T>(this Stream stream, Func<Stream, int, T> itemReader, int length)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      T[] array = new T[length];
      int total = 0;
      try
      {
        array.Fill(i => { T v = itemReader(stream, i); total = i + 1; return v; });
      }
      catch (Exception ex) when (ThrowData != null && ThrowData(stream))
      {
        throw new StreamReadException(array, total, ex);
      }
      return array;
    }

    #endregion
    #region Read regular array methods

    public static Array ReadRegularArray<T>(this Stream stream, Func<Stream, T> itemReader, int[] lengths, int[] lowerBounds = null)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      return stream.ReadRegularArray<T>((s, fi, di) => itemReader(s), null, lengths, lowerBounds);
    }

    public static Array ReadRegularArray<T>(this Stream stream, Func<Stream, int, int[], T> itemReader, int[] indices, int[] lengths, int[] lowerBounds = null)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      Array array = PwrArray.CreateAsRegular<T>(lengths, lowerBounds);
      int total = 0;
      try
      {
        array.FillAsRegular((fi, di) => { var v = itemReader(stream, fi, di); total = fi + 1; return v; }, indices);
      }
      catch (Exception ex) when (ThrowData != null && ThrowData(stream))
      {
        throw new StreamReadException(array, total, ex);
      }
      return array;
    }

    public static Array ReadRegularArray<T>(this Stream stream, Func<Stream, T> itemReader, ArrayDimension[] dimensions)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      return stream.ReadRegularArray<T>((s, fi, di) => itemReader(s), null, dimensions);
    }

    public static Array ReadRegularArray<T>(this Stream stream, Func<Stream, int, int[], T> itemReader, int[] indices, ArrayDimension[] dimensions)
    {
      if (dimensions == null)
        throw new ArgumentNullException("dimensions");

      return stream.ReadRegularArray(itemReader, indices, dimensions.Select(t => t.Length).ToArray(), dimensions.Select(t => t.LowerBound).ToArray());
    }

    #endregion
    #region Read long regular array methods

    public static Array ReadLongRegularArray<T>(this Stream stream, Func<Stream, T> itemReader, long[] lengths)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      return stream.ReadLongRegularArray<T>((s, fi, di) => itemReader(s), null, lengths);
    }

    public static Array ReadLongRegularArray<T>(this Stream stream, Func<Stream, long, long[], T> itemReader, long[] indices, long[] lengths)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      Array array = PwrArray.CreateAsLongRegular<T>(lengths);
      long total = 0;
      try
      {
        array.FillAsLongRegular((fi, di) => { var v = itemReader(stream, fi, di); total = fi + 1; return v; }, indices);
      }
      catch (Exception ex) when (ThrowData != null && ThrowData(stream))
      {
        throw new StreamReadLongException(array, total, ex);
      }
      return array;
    }

    #endregion
    #endregion
    #region Try read methods
    #region Try read size methods

    public static bool TryReadSize(this Stream stream, SizeEncoding sizeEncoding, out Int32 value, Endian endian = Endian.Default)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      switch (sizeEncoding)
      {
        case SizeEncoding.NO:
          value = 0;
          return true;
        case SizeEncoding.B1:
          {
            Byte temp;
            if (stream.TryReadUByte(out temp))
            {
              value = temp;
              return true;
            }
            break;
          }
        case SizeEncoding.B2:
          {
            UInt16 temp;
            if (stream.TryReadUInt16(out temp))
            {
              value = temp;
              return true;
            }
            break;
          }
        case SizeEncoding.B4:
          {
            Int32 temp;
            if (stream.TryReadInt32(out temp))
            {
              value = temp;
              return true;
            }
            break;
          }
        case SizeEncoding.B8:
          {
            Int64 temp;
            if (stream.TryReadInt64(out temp))
            {
              value = (Int32)temp;
              return true;
            }
            break;
          }
        case SizeEncoding.VB:
          {
            Int32 temp;
            if (stream.TryReadInt32Vb(out temp))
            {
              value = temp;
              return true;
            }
            break;
          }
        default:
          throw new ArgumentOutOfRangeException("sizeEncoding");
      }
      value = default(Int32);
      return false;
    }

    public static bool TryReadLongSize(this Stream stream, SizeEncoding sizeEncoding, out Int64 value, Endian endian = Endian.Default)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      switch (sizeEncoding)
      {
        case SizeEncoding.NO:
          value = 0L;
          return true;
        case SizeEncoding.B1:
          {
            Byte temp;
            if (stream.TryReadUByte(out temp))
            {
              value = temp;
              return true;
            }
            break;
          }
        case SizeEncoding.B2:
          {
            UInt16 temp;
            if (stream.TryReadUInt16(out temp))
            {
              value = temp;
              return true;
            }
            break;
          }
        case SizeEncoding.B4:
          {
            UInt32 temp;
            if (stream.TryReadUInt32(out temp))
            {
              value = temp;
              return true;
            }
            break;
          }
        case SizeEncoding.B8:
          {
            Int64 temp;
            if (stream.TryReadInt64(out temp))
            {
              value = temp;
              return true;
            }
            break;
          }
        case SizeEncoding.VB:
          {
            Int64 temp;
            if (stream.TryReadInt64Vb(out temp))
            {
              value = temp;
              return true;
            }
            break;
          }
        default:
          throw new ArgumentOutOfRangeException("sizeEncoding");
      }
      value = default(Int64);
      return false;
    }

    public static TryOut<Int32> TryReadSize(this Stream stream, SizeEncoding sizeEncoding, Endian endian = Endian.Default)
    {
      Int32 value;
      return TryOut.Create(stream.TryReadSize(sizeEncoding, out value, endian), value);
    }

    public static TryOut<Int64> TryReadLongSize(this Stream stream, SizeEncoding sizeEncoding, Endian endian = Endian.Default)
    {
      Int64 value;
      return TryOut.Create(stream.TryReadLongSize(sizeEncoding, out value, endian), value);
    }

    #endregion
    #region Try read base types methods

    public static bool TryReadSByte(this Stream stream, out SByte value)
    {
      byte[] buffer = stream.ReadBytes(sizeof(SByte), sizeof(SByte), StreamReadOptions.ExactSize | StreamReadOptions.NoCheckEnd);
      value = buffer != null ? (SByte)buffer[0] : default(SByte);
      return buffer != null;
    }

    public static bool TryReadUByte(this Stream stream, out Byte value)
    {
      byte[] buffer = stream.ReadBytes(sizeof(Byte), sizeof(Byte), StreamReadOptions.ExactSize | StreamReadOptions.NoCheckEnd);
      value = buffer != null ? buffer[0] : default(Byte);
      return buffer != null;
    }

    public static bool TryReadInt16(this Stream stream, out Int16 value, Endian endian = Endian.Default)
    {
      byte[] buffer = stream.ReadBytes(sizeof(Int16), sizeof(Int16), StreamReadOptions.ExactSize | StreamReadOptions.NoCheckEnd);
      if (buffer != null && (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert))
        Array.Reverse(buffer);
      value = buffer != null ? BitConverter.ToInt16(buffer, 0) : default(Int16);
      return buffer != null;
    }

    public static bool TryReadInt32(this Stream stream, out Int32 value, Endian endian = Endian.Default)
    {
      byte[] buffer = stream.ReadBytes(sizeof(Int32), sizeof(Int32), StreamReadOptions.ExactSize | StreamReadOptions.NoCheckEnd);
      if (buffer != null && (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert))
        Array.Reverse(buffer);
      value = buffer != null ? BitConverter.ToInt32(buffer, 0) : default(Int32);
      return buffer != null;
    }

    public static bool TryReadInt64(this Stream stream, out Int64 value, Endian endian = Endian.Default)
    {
      byte[] buffer = stream.ReadBytes(sizeof(Int64), sizeof(Int64), StreamReadOptions.ExactSize | StreamReadOptions.NoCheckEnd);
      if (buffer != null && (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert))
        Array.Reverse(buffer);
      value = buffer != null ? BitConverter.ToInt64(buffer, 0) : default(Int64);
      return buffer != null;
    }

    public static bool TryReadUInt16(this Stream stream, out UInt16 value, Endian endian = Endian.Default)
    {
      byte[] buffer = stream.ReadBytes(sizeof(UInt16), sizeof(UInt16), StreamReadOptions.ExactSize | StreamReadOptions.NoCheckEnd);
      if (buffer != null && (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert))
        Array.Reverse(buffer);
      value = buffer != null ? BitConverter.ToUInt16(buffer, 0) : default(UInt16);
      return buffer != null;
    }

    public static bool TryReadUInt32(this Stream stream, out UInt32 value, Endian endian = Endian.Default)
    {
      byte[] buffer = stream.ReadBytes(sizeof(UInt32), sizeof(UInt32), StreamReadOptions.ExactSize | StreamReadOptions.NoCheckEnd);
      if (buffer != null && (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert))
        Array.Reverse(buffer);
      value = buffer != null ? BitConverter.ToUInt32(buffer, 0) : default(UInt32);
      return buffer != null;
    }

    public static bool TryReadUInt64(this Stream stream, out UInt64 value, Endian endian = Endian.Default)
    {
      byte[] buffer = stream.ReadBytes(sizeof(UInt64), sizeof(UInt64), StreamReadOptions.ExactSize | StreamReadOptions.NoCheckEnd);
      if (buffer != null && (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert))
        Array.Reverse(buffer);
      value = buffer != null ? BitConverter.ToUInt64(buffer, 0) : default(UInt64);
      return buffer != null;
    }

    public static bool TryReadInt16Vb(this Stream stream, out Int16 value)
    {
      Byte part;
      if (!stream.TryReadUByte(out part))
      {
        value = default(Int16);
        return false;
      }
      Int16 temp = (Int16)part;
      for (int i = 7, r = temp >= 0x80 ? sizeof(Int16) * 8 - 8 : 0; r > 0; i += 7)
      {
        part = stream.ReadUByte();
        if (part < 0x80)
          r = 0;
        else if (r > 7)
          r -= 7;
        else
          throw new InvalidOperationException("Bad multi-byte integer format");
        temp |= (Int16)((part & ~0x80) << i);
      }
      value = temp;
      return true;
    }

    public static bool TryReadInt32Vb(this Stream stream, out Int32 value)
    {
      Byte part;
      if (!stream.TryReadUByte(out part))
      {
        value = default(Int32);
        return false;
      }
      Int32 temp = (Int32)part;
      for (int i = 7, r = temp >= 0x80 ? sizeof(Int32) * 8 - 8 : 0; r > 0; i += 7)
      {
        part = stream.ReadUByte();
        if (part < 0x80)
          r = 0;
        else if (r > 7)
          r -= 7;
        else
          throw new InvalidOperationException("Bad multi-byte integer format");
        temp |= (part & ~0x80) << i;
      }
      value = temp;
      return true;
    }

    public static bool TryReadInt64Vb(this Stream stream, out Int64 value)
    {
      Byte part;
      if (!stream.TryReadUByte(out part))
      {
        value = default(Int64);
        return false;
      }
      Int64 temp = (Int64)part;
      for (int i = 7, r = temp >= 0x80 ? sizeof(Int64) * 8 - 8 : 0; r > 0; i += 7)
      {
        part = stream.ReadUByte();
        if (part < 0x80)
          r = 0;
        else if (r > 7)
          r -= 7;
        else
          throw new InvalidOperationException("Bad multi-byte integer format");
        temp |= (part & ~0x80L) << i;
      }
      value = temp;
      return true;
    }

    public static bool TryReadUInt16Vb(this Stream stream, out UInt16 value)
    {
      Byte part;
      if (!stream.TryReadUByte(out part))
      {
        value = default(UInt16);
        return false;
      }
      UInt16 temp = (UInt16)part;
      for (int i = 7, r = temp >= 0x80 ? sizeof(UInt16) * 8 - 8 : 0; r > 0; i += 7)
      {
        part = stream.ReadUByte();
        if (part < 0x80)
          r = 0;
        else if (r > 7)
          r -= 7;
        else
          throw new InvalidOperationException("Bad multi-byte integer format");
        temp |= (UInt16)((part & ~0x80) << i);
      }
      value = temp;
      return true;
    }

    public static bool TryReadUInt32Vb(this Stream stream, out UInt32 value)
    {
      Byte part;
      if (!stream.TryReadUByte(out part))
      {
        value = default(UInt32);
        return false;
      }
      UInt32 temp = (UInt32)part;
      for (int i = 7, r = temp >= 0x80 ? sizeof(UInt32) * 8 - 8 : 0; r > 0; i += 7)
      {
        part = stream.ReadUByte();
        if (part < 0x80)
          r = 0;
        else if (r > 7)
          r -= 7;
        else
          throw new InvalidOperationException("Bad multi-byte integer format");
        temp |= (UInt32)((part & ~0x80) << i);
      }
      value = temp;
      return true;
    }

    public static bool TryReadUInt64Vb(this Stream stream, out UInt64 value)
    {
      Byte part;
      if (!stream.TryReadUByte(out part))
      {
        value = default(UInt64);
        return false;
      }
      UInt64 temp = (UInt64)part;
      for (int i = 7, r = temp >= 0x80 ? sizeof(UInt64) * 8 - 8 : 0; r > 0; i += 7)
      {
        part = stream.ReadUByte();
        if (part < 0x80)
          r = 0;
        else if (r > 7)
          r -= 7;
        else
          throw new InvalidOperationException("Bad multi-byte integer format");
        temp |= (UInt64)((part & ~0x80L) << i);
      }
      value = temp;
      return true;
    }

    public static bool TryReadSingle(this Stream stream, out Single value, Endian endian = Endian.Default)
    {
      byte[] buffer = stream.ReadBytes(sizeof(Single), sizeof(Single), StreamReadOptions.ExactSize | StreamReadOptions.NoCheckEnd);
      if (buffer != null && (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert))
        Array.Reverse(buffer);
      value = buffer != null ? BitConverter.ToSingle(buffer, 0) : default(Single);
      return buffer != null;
    }

    public static bool TryReadDouble(this Stream stream, out Double value, Endian endian = Endian.Default)
    {
      byte[] buffer = stream.ReadBytes(sizeof(Double), sizeof(Double), StreamReadOptions.ExactSize | StreamReadOptions.NoCheckEnd);
      if (buffer != null && (endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian || endian == Endian.Invert))
        Array.Reverse(buffer);
      value = buffer != null ? BitConverter.ToDouble(buffer, 0) : default(Double);
      return buffer != null;
    }

    public static bool TryReadDecimal(this Stream stream, out Decimal value)
    {
      byte[] buffer = stream.ReadBytes(sizeof(Int32) * 4, sizeof(Int32) * 4, StreamReadOptions.ExactSize | StreamReadOptions.NoCheckEnd);
      value = buffer != null ? new Decimal(Enumerable.Range(0, 4).Select(i => BitConverter.ToInt32(buffer, i * sizeof(int))).ToArray()) : default(Decimal);
      return buffer != null;
    }

    public static bool TryReadGuid(this Stream stream, out Guid value)
    {
      byte[] buffer = stream.ReadBytes(GuidSize, GuidSize, StreamReadOptions.ExactSize | StreamReadOptions.NoCheckEnd);
      value = buffer != null ? new Guid(buffer) : default(Guid);
      return buffer != null;
    }

    public static bool TryReadDateTime(this Stream stream, out DateTime value, Endian endian = Endian.Default)
    {
      Int64 ticks;
      if (stream.TryReadInt64(out ticks, endian))
      {
        value = new DateTime(ticks);
        return true;
      }
      else
      {
        value = default(DateTime);
        return false;
      }
    }

    public static bool TryReadTimeSpan(this Stream stream, out TimeSpan value, Endian endian = Endian.Default)
    {
      Int64 ticks;
      if (stream.TryReadInt64(out ticks, endian))
      {
        value = new TimeSpan(ticks);
        return true;
      }
      else
      {
        value = default(TimeSpan);
        return false;
      }
    }

    public static bool TryReadBoolean(this Stream stream, TypeCode typeCode, out Boolean value, Endian endian = Endian.Default)
    {
      switch (typeCode)
      {
        case TypeCode.Byte:
          {
            Byte temp;
            if (stream.TryReadUByte(out temp))
            {
              value = temp != 0;
              return true;
            }
            else
            {
              value = default(Boolean);
              return false;
            }
          }
        case TypeCode.UInt16:
          {
            UInt16 temp;
            if (stream.TryReadUInt16(out temp, endian))
            {
              value = temp != 0;
              return true;
            }
            else
            {
              value = default(Boolean);
              return false;
            }
          }
        case TypeCode.UInt32:
          {
            UInt32 temp;
            if (stream.TryReadUInt32(out temp, endian))
            {
              value = temp != 0;
              return true;
            }
            else
            {
              value = default(Boolean);
              return false;
            }
          }
        case TypeCode.UInt64:
          {
            UInt64 temp;
            if (stream.TryReadUInt64(out temp, endian))
            {
              value = temp != 0;
              return true;
            }
            else
            {
              value = default(Boolean);
              return false;
            }
          }
        case TypeCode.SByte:
          {
            SByte temp;
            if (stream.TryReadSByte(out temp))
            {
              value = temp != 0;
              return true;
            }
            else
            {
              value = default(Boolean);
              return false;
            }
          }
        case TypeCode.Int16:
          {
            Int16 temp;
            if (stream.TryReadInt16(out temp, endian))
            {
              value = temp != 0;
              return true;
            }
            else
            {
              value = default(Boolean);
              return false;
            }
          }
        case TypeCode.Int32:
          {
            Int32 temp;
            if (stream.TryReadInt32(out temp, endian))
            {
              value = temp != 0;
              return true;
            }
            else
            {
              value = default(Boolean);
              return false;
            }
          }
        case TypeCode.Int64:
          {
            Int64 temp;
            if (stream.TryReadInt64(out temp, endian))
            {
              value = temp != 0;
              return true;
            }
            else
            {
              value = default(Boolean);
              return false;
            }
          }
        default:
          throw new ArgumentOutOfRangeException("Invalid argument TypeCode value ", "typeCode");
      }
    }

    public static bool TryReadChar(this Stream stream, Encoding encoding, out Char value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      var bytes = new byte[1];
      if (stream.Read(bytes, 0, bytes.Length) == 0)
      {
        value = default(Char);
        return false;
      }
      else
      {
        var chars = new char[1];
        var decoder = encoding.GetDecoder();
        while (decoder.GetChars(bytes, 0, bytes.Length, chars, 0) == 0)
          if (stream.Read(bytes, 0, bytes.Length) == 0)
            throw new EndOfStreamException();
        value = chars[0];
        return true;
      }
    }

    public static bool TryReadEnum<T>(this Stream stream, out T value, Endian endian = Endian.Default)
      where T : struct, IComparable, IFormattable, IConvertible
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (!typeof(T).IsEnum)
        throw new InvalidOperationException(string.Format("Type '{0}' is not enum.", typeof(T).FullName));

      switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))))
      {
        case TypeCode.Byte:
          {
            Byte temp;
            if (stream.TryReadUByte(out temp))
            {
              value = (T)Enum.ToObject(typeof(T), temp);
              return true;
            }
            else
            {
              value = default(T);
              return false;
            }
          }
        case TypeCode.UInt16:
          {
            UInt16 temp;
            if (stream.TryReadUInt16(out temp, endian))
            {
              value = (T)Enum.ToObject(typeof(T), temp);
              return true;
            }
            else
            {
              value = default(T);
              return false;
            }
          }
        case TypeCode.UInt32:
          {
            UInt32 temp;
            if (stream.TryReadUInt32(out temp, endian))
            {
              value = (T)Enum.ToObject(typeof(T), temp);
              return true;
            }
            else
            {
              value = default(T);
              return false;
            }
          }
        case TypeCode.UInt64:
          {
            UInt64 temp;
            if (stream.TryReadUInt64(out temp, endian))
            {
              value = (T)Enum.ToObject(typeof(T), temp);
              return true;
            }
            else
            {
              value = default(T);
              return false;
            }
          }
        case TypeCode.SByte:
          {
            SByte temp;
            if (stream.TryReadSByte(out temp))
            {
              value = (T)Enum.ToObject(typeof(T), temp);
              return true;
            }
            else
            {
              value = default(T);
              return false;
            }
          }
        case TypeCode.Int16:
          {
            Int16 temp;
            if (stream.TryReadInt16(out temp, endian))
            {
              value = (T)Enum.ToObject(typeof(T), temp);
              return true;
            }
            else
            {
              value = default(T);
              return false;
            }
          }
        case TypeCode.Int32:
          {
            Int32 temp;
            if (stream.TryReadInt32(out temp, endian))
            {
              value = (T)Enum.ToObject(typeof(T), temp);
              return true;
            }
            else
            {
              value = default(T);
              return false;
            }
          }
        case TypeCode.Int64:
          {
            Int64 temp;
            if (stream.TryReadInt64(out temp, endian))
            {
              value = (T)Enum.ToObject(typeof(T), temp);
              return true;
            }
            else
            {
              value = default(T);
              return false;
            }
          }
        default:
          throw new InvalidOperationException("Invalid enum underlying type");
      }
    }

    public static bool TryReadChars(this Stream stream, Encoding encoding, int length, out Char[] value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      byte[] bytes;
      if (stream.TryReadBytes(length, out bytes))
      {
        value = encoding.GetChars(bytes);
        return true;
      }
      else
      {
        value = default(Char[]);
        return false;
      }
    }

    public static bool TryReadChars(this Stream stream, Encoding encoding, SizeEncoding sizeEncoding, out Char[] value, Endian endian = Endian.Default)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      Int32 length;
      if (stream.TryReadSize(sizeEncoding, out length, endian))
      {
        value = encoding.GetChars(stream.ReadBytes(length));
        return true;
      }
      else
      {
        value = default(Char[]);
        return false;
      }
    }

    public static bool TryReadChars(this Stream stream, Encoding encoding, string terminator, bool omitTerm, out Char[] value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");
      if (string.IsNullOrEmpty(terminator))
        throw new ArgumentException("Value is not specified", "terminator");

      byte[] bytes;
      if (stream.TryReadBytes(encoding.GetBytes(terminator), omitTerm, out bytes))
      {
        value = encoding.GetChars(bytes);
        return true;
      }
      else
      {
        value = default(Char[]);
        return false;
      }
    }

    public static bool TryReadString(this Stream stream, Encoding encoding, int length, out String value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      byte[] bytes;
      if (stream.TryReadBytes(length, out bytes))
      {
        value = encoding.GetString(bytes);
        return true;
      }
      else
      {
        value = default(String);
        return false;
      }
    }

    public static bool TryReadString(this Stream stream, Encoding encoding, SizeEncoding sizeEncoding, out String value, Endian endian = Endian.Default)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      Int32 length;
      if (stream.TryReadSize(sizeEncoding, out length, endian))
      {
        value = encoding.GetString(stream.ReadBytes(length));
        return true;
      }
      else
      {
        value = default(String);
        return false;
      }
    }

    public static bool TryReadString(this Stream stream, Encoding encoding, string terminator, bool omitTerm, out String value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");
      if (string.IsNullOrEmpty(terminator))
        throw new ArgumentException("Value is not specified", "terminator");

      byte[] bytes;
      if (stream.TryReadBytes(encoding.GetBytes(terminator), omitTerm, out bytes))
      {
        value = encoding.GetString(bytes);
        return true;
      }
      else
      {
        value = default(String);
        return false;
      }
    }

    public static bool TryReadBytes(this Stream stream, int length, out Byte[] value)
    {
      var temp = stream.ReadBytes(length, length, StreamReadOptions.ExactSize |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
      value = temp != null ? temp : default(Byte[]);
      return temp != null;
    }

    public static bool TryReadBytes(this Stream stream, SizeEncoding sizeEncoding, out Byte[] value, Endian endian = Endian.Default)
    {
      Int32 length;
      if (stream.TryReadSize(sizeEncoding, out length, endian))
      {
        value = stream.ReadBytes(length);
        return true;
      }
      else
      {
        value = default(Byte[]);
        return false;
      }
    }

    public static bool TryReadBytes(this Stream stream, byte[] terminator, bool omitTerm, out Byte[] value)
    {
      var temp = stream.ReadBytes(initBufferDefaultSize, int.MaxValue, terminator, (omitTerm ? StreamReadOptions.OmitTerminator : 0) |
        (ThrowData != null && ThrowData(stream) ? StreamReadOptions.ThrowBuffer : 0));
      value = temp != null ? temp : default(Byte[]);
      return temp != null;
    }

    public static TryOut<SByte> TryReadSByte(this Stream stream)
    {
      SByte value;
      return TryOut.Create(stream.TryReadSByte(out value), value);
    }

    public static TryOut<Byte> TryReadUByte(this Stream stream)
    {
      Byte value;
      return TryOut.Create(stream.TryReadUByte(out value), value);
    }

    public static TryOut<Int16> TryReadInt16(this Stream stream, Endian endian = Endian.Default)
    {
      Int16 value;
      return TryOut.Create(stream.TryReadInt16(out value), value);
    }

    public static TryOut<Int32> TryReadInt32(this Stream stream, Endian endian = Endian.Default)
    {
      Int32 value;
      return TryOut.Create(stream.TryReadInt32(out value), value);
    }

    public static TryOut<Int64> TryReadInt64(this Stream stream, Endian endian = Endian.Default)
    {
      Int64 value;
      return TryOut.Create(stream.TryReadInt64(out value), value);
    }

    public static TryOut<UInt16> TryReadUInt16(this Stream stream, Endian endian = Endian.Default)
    {
      UInt16 value;
      return TryOut.Create(stream.TryReadUInt16(out value), value);
    }

    public static TryOut<UInt32> TryReadUInt32(this Stream stream, Endian endian = Endian.Default)
    {
      UInt32 value;
      return TryOut.Create(stream.TryReadUInt32(out value), value);
    }

    public static TryOut<UInt64> TryReadUInt64(this Stream stream, Endian endian = Endian.Default)
    {
      UInt64 value;
      return TryOut.Create(stream.TryReadUInt64(out value), value);
    }

    public static TryOut<Int16> TryReadInt16Vb(this Stream stream)
    {
      Int16 value;
      return TryOut.Create(stream.TryReadInt16Vb(out value), value);
    }

    public static TryOut<Int32> TryReadInt32Vb(this Stream stream)
    {
      Int32 value;
      return TryOut.Create(stream.TryReadInt32Vb(out value), value);
    }

    public static TryOut<Int64> TryReadInt64Vb(this Stream stream)
    {
      Int64 value;
      return TryOut.Create(stream.TryReadInt64Vb(out value), value);
    }

    public static TryOut<UInt16> TryReadUInt16Vb(this Stream stream)
    {
      UInt16 value;
      return TryOut.Create(stream.TryReadUInt16Vb(out value), value);
    }

    public static TryOut<UInt32> TryReadUInt32Vb(this Stream stream)
    {
      UInt32 value;
      return TryOut.Create(stream.TryReadUInt32Vb(out value), value);
    }

    public static TryOut<UInt64> TryReadUInt64Vb(this Stream stream)
    {
      UInt64 value;
      return TryOut.Create(stream.TryReadUInt64Vb(out value), value);
    }

    public static TryOut<Single> TryReadSingle(this Stream stream, Endian endian = Endian.Default)
    {
      Single value;
      return TryOut.Create(stream.TryReadSingle(out value, endian), value);
    }

    public static TryOut<Double> TryReadDouble(this Stream stream, Endian endian = Endian.Default)
    {
      Double value;
      return TryOut.Create(stream.TryReadDouble(out value, endian), value);
    }

    public static TryOut<Decimal> TryReadDecimal(this Stream stream)
    {
      Decimal value;
      return TryOut.Create(stream.TryReadDecimal(out value), value);
    }

    public static TryOut<Guid> TryReadGuid(this Stream stream)
    {
      Guid value;
      return TryOut.Create(stream.TryReadGuid(out value), value);
    }

    public static TryOut<DateTime> TryReadDateTime(this Stream stream, Endian endian = Endian.Default)
    {
      DateTime value;
      return TryOut.Create(stream.TryReadDateTime(out value, endian), value);
    }

    public static TryOut<TimeSpan> TryReadTimeSpan(this Stream stream, Endian endian = Endian.Default)
    {
      TimeSpan value;
      return TryOut.Create(stream.TryReadTimeSpan(out value, endian), value);
    }

    public static TryOut<Boolean> TryReadBoolean(this Stream stream, TypeCode typeCode, Endian endian = Endian.Default)
    {
      Boolean value;
      return TryOut.Create(stream.TryReadBoolean(typeCode, out value, endian), value);
    }

    public static TryOut<Char> TryReadChar(this Stream stream, Encoding encoding)
    {
      Char value;
      return TryOut.Create(stream.TryReadChar(encoding, out value), value);
    }

    public static TryOut<T> TryReadEnum<T>(this Stream stream, Endian endian = Endian.Default)
      where T : struct, IComparable, IFormattable, IConvertible
    {
      T value;
      return TryOut.Create(stream.TryReadEnum(out value, endian), value);
    }

    public static TryOut<Char[]> TryReadChars(this Stream stream, Encoding encoding, int count)
    {
      Char[] value;
      return TryOut.Create(stream.TryReadChars(encoding, count, out value), value);
    }

    public static TryOut<Char[]> TryReadChars(this Stream stream, Encoding encoding, SizeEncoding sizeEncoding, Endian endian = Endian.Default)
    {
      Char[] value;
      return TryOut.Create(stream.TryReadChars(encoding, sizeEncoding, out value, endian), value);
    }

    public static TryOut<Char[]> TryReadChars(this Stream stream, Encoding encoding, string terminator, bool omitTerm)
    {
      Char[] value;
      return TryOut.Create(stream.TryReadChars(encoding, terminator, omitTerm, out value), value);
    }

    public static TryOut<String> TryReadString(this Stream stream, Encoding encoding, int count)
    {
      String value;
      return TryOut.Create(stream.TryReadString(encoding, count, out value), value);
    }

    public static TryOut<String> TryReadString(this Stream stream, Encoding encoding, SizeEncoding sizeEncoding, Endian endian = Endian.Default)
    {
      String value;
      return TryOut.Create(stream.TryReadString(encoding, sizeEncoding, out value, endian), value);
    }

    public static TryOut<String> TryReadString(this Stream stream, Encoding encoding, string terminator, bool omitTerm)
    {
      String value;
      return TryOut.Create(stream.TryReadString(encoding, terminator, omitTerm, out value), value);
    }

    public static TryOut<byte[]> TryReadBytes(this Stream stream, int length)
    {
      byte[] value;
      return TryOut.Create(stream.TryReadBytes(length, out value), value);
    }

    public static TryOut<byte[]> TryReadBytes(this Stream stream, SizeEncoding sizeEncoding, Endian endian = Endian.Default)
    {
      byte[] value;
      return TryOut.Create(stream.TryReadBytes(sizeEncoding, out value, endian), value);
    }

    public static TryOut<byte[]> TryReadBytes(this Stream stream, byte[] terminator, bool omitTerm)
    {
      byte[] value;
      return TryOut.Create(stream.TryReadBytes(terminator, omitTerm, out value), value);
    }

    public static TryOut<T> TryReadObject<T>(this Stream stream, Func<Stream, T> typeReader, Func<Stream, TryOut<Boolean>> flagTrier)
      where T : class
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (typeReader == null)
        throw new ArgumentNullException("typeReader");
      if (flagTrier == null)
        throw new ArgumentNullException("flagTrier");

      TryOut<bool> flag = flagTrier(stream);
      return new TryOut<T>(flag.Success, flag.Success && flag.Value ? typeReader(stream) : default(T));
    }

    public static TryOut<T?> TryReadNullable<T>(this Stream stream, Func<Stream, T> typeReader, Func<Stream, TryOut<Boolean>> flagTrier)
      where T : struct
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (typeReader == null)
        throw new ArgumentNullException("typeReader");
      if (flagTrier == null)
        throw new ArgumentNullException("flagTrier");

      TryOut<bool> flag = flagTrier(stream);
      return new TryOut<T?>(flag.Success, flag.Success && flag.Value ? typeReader(stream) : default(T?));
    }

    #endregion
    #endregion
    #region Read collection methods

    public static IEnumerable<T> ReadCollection<T>(this Stream stream, Func<Stream, T> itemReader, int maxCount = int.MaxValue)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      return stream.ReadCollection((s, i) => itemReader(s), maxCount);
    }

    public static IEnumerable<T> ReadCollection<T>(this Stream stream, Func<Stream, int, T> itemReader, int maxCount = int.MaxValue)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");
      if (maxCount < 0)
        throw new ArgumentOutOfRangeException("maxCount");

      for (int i = 0; i < maxCount; i++)
      {
        T item;
        try
        {
          item = itemReader(stream, i);
        }
        catch (Exception ex) when (ThrowData != null && ThrowData(stream))
        {
          throw new StreamLoadException(i, ex);
        }
        yield return item;
      }
    }

    public static IEnumerable<T> ReadCollection<T>(this Stream stream, Func<Stream, TryOut<T>> itemReader, int maxCount = int.MaxValue)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      return stream.ReadCollection((s, i) => itemReader(s), maxCount);
    }

    public static IEnumerable<T> ReadCollection<T>(this Stream stream, Func<Stream, int, TryOut<T>> itemReader, int maxCount = int.MaxValue)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");
      if (maxCount < 0)
        throw new ArgumentOutOfRangeException("maxCount");

      for (int i = 0; i < maxCount; i++)
      {
        TryOut<T> item;
        try
        {
          item = itemReader(stream, i);
        }
        catch (Exception ex) when (ThrowData != null && ThrowData(stream))
        {
          throw new StreamLoadException(i, ex);
        }
        if (item.Success)
          yield return item.Value;
        else
          yield break;
      }
    }

    public static IEnumerable<Byte> ReadBytes(this Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      int read;
      while ((read = stream.ReadByte()) >= 0)
        yield return (byte)read;
    }

    public static IEnumerable<Char> ReadChars(this Stream stream, Encoding encoding)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      var decoder = encoding.GetDecoder();
      var bytes = new byte[1];
      var chars = new char[1];
      while (true)
      {
        do
          if (stream.Read(bytes, 0, bytes.Length) == 0)
            yield break;
        while (decoder.GetChars(bytes, 0, bytes.Length, chars, 0) == 0);
        yield return chars[0];
      }
    }

    public static IEnumerable<String> ReadLines(this Stream stream, Encoding encoding, Func<IList<Char>, int> terminatorMatcher, Func<IList<Char>, string> terminatorConverter, int maxCount = int.MaxValue)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");
      if (terminatorMatcher == null)
        throw new ArgumentNullException("terminatorMatcher");
      if (maxCount < 0)
        throw new ArgumentOutOfRangeException("maxCount");

      if (maxCount == 0)
        yield break;

      var decoder = encoding.GetDecoder();
      var list = new PwrList<Char>();
      var view = new PwrFrameListView<Char>(list);
      var sb = new StringBuilder();
      var bytes = new byte[1];
      var chars = new char[1];
      int matched = 0;
      while (maxCount > 0)
      {
        int read = 0;
        int accepted = 0;
        while (true)
        {
          if (matched == list.Count)
          {
            while ((read = stream.Read(bytes, 0, bytes.Length)) > 0 && decoder.GetChars(bytes, 0, bytes.Length, chars, 0) == 0) ;
            if (read == 0)
              break;
            else
              list.PushLast(chars[0]);
          }
          view.FrameSize = matched + 1;
          accepted = terminatorMatcher(view);
          if (accepted > 0)
            break;
          else if (accepted == 0)
            matched++;
          else
          {
            matched = 0;
            sb.Append(list.PopFirst());
          }
        }
        if (accepted > matched + 1)
          throw new InvalidOperationException("Accepted value is greater matched length.");
        if (accepted > 0)
        {
          if (terminatorConverter == null)
            sb.Append(list.GetRange(0, accepted).ToArray());
          else
          {
            view.FrameSize = accepted;
            string term = terminatorConverter(view);
            if (!string.IsNullOrEmpty(term))
              sb.Append(term);
          }
          yield return sb.ToString();
          sb.Clear();
          list.RemoveRange(0, accepted);
          matched = 0;
          maxCount--;
        }
        if (read == 0)
          break;
      }
      if (maxCount > 0)
      {
        if (list.Count > 0)
          sb.AppendChars(list);
        if (sb.Length > 0)
        {
          yield return sb.ToString();
          sb.Clear();
          list.Clear();
        }
      }
    }

    public static IEnumerable<String> ReadLines(this Stream stream, Encoding encoding, IList<String> terminatorsList, String terminatorStub, int maxCount = int.MaxValue)
    {
      if (terminatorsList == null)
        throw new ArgumentNullException("terminatorsList");

      var matches = new int[terminatorsList.Count];
      int maxLength = terminatorsList.Max(s => s.Length);
      return stream.ReadLines(encoding, l => StringHelper.MatchStrings(l, terminatorsList, matches, maxLength), terminatorStub != null ? l => terminatorStub : default(Func<IList<Char>, String>), maxCount);
    }

    #endregion
    #region Load methods
    #region Load array methods

    public static void LoadArray<T>(this Stream stream, T[] array, Func<Stream, T> itemReader)
		{
      stream.LoadArray(array, itemReader, 0, array != null ? array.Length : 0);
		}

		public static void LoadArray<T>(this Stream stream, T[] array, Func<Stream, int, T> itemReader)
		{
      stream.LoadArray(array, itemReader, 0, array != null ? array.Length : 0);
    }

    public static void LoadArray<T>(this Stream stream, T[] array, Func<Stream, T> itemReader, Range range)
    {
      stream.LoadArray(array, itemReader, range.Index, range.Count);
    }

    public static void LoadArray<T>(this Stream stream, T[] array, Func<Stream, int, T> itemReader, Range range)
    {
      stream.LoadArray(array, itemReader, range.Index, range.Count);
		}

		public static void LoadArray<T>(this Stream stream, T[] array, Func<Stream, T> itemReader, int index, int count)
    {
			if (itemReader == null)
				throw new ArgumentNullException("itemReader");

      stream.LoadArray(array, itemReader, index, count);
		}

		public static void LoadArray<T>(this Stream stream, T[] array, Func<Stream, int, T> itemReader, int index, int count)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      int total = 0;
      try
      {
        array.Fill(i => { var v = itemReader(stream, i); total = i + 1; return v; }, index, count);
      }
      catch (Exception ex) when (ThrowData != null && ThrowData(stream))
      {
        throw new StreamLoadException(total, ex);
      }
    }

    #endregion
    #region Load regular array methods

    public static void LoadRegularArray<T>(this Stream stream, Array array, Func<Stream, T> itemReader, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      stream.LoadRegularArray<T>(array, (s, fi, di) => itemReader(s), null, zeroBased, range, ranges);
    }

    public static void LoadRegularArray<T>(this Stream stream, Array array, Func<Stream, int, int[], T> itemReader, int[] indices, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      int total = 0;
      try
      {
        array.FillAsRegular((fi, di) => { var v = itemReader(stream, fi, di); total = fi + 1; return v; }, indices, zeroBased, range, ranges);
      }
      catch (Exception ex) when (ThrowData != null && ThrowData(stream))
      {
        throw new StreamReadException(array, total, ex);
      }
    }

    #endregion
    #region Load long regular array methods

    public static void LoadLongRegularArray<T>(this Stream stream, Array array, Func<Stream, T> itemReader, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      stream.LoadLongRegularArray<T>(array, (s, fi, di) => itemReader(s), null, zeroBased, range, ranges);
    }

    public static void LoadLongRegularArray<T>(this Stream stream, Array array, Func<Stream, long, long[], T> itemReader, long[] indices, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (itemReader == null)
        throw new ArgumentNullException("itemReader");

      long total = 0;
      try
      {
        array.FillAsLongRegular((fi, di) => { var v = itemReader(stream, fi, di); total = fi + 1L; return v; }, indices, zeroBased, range, ranges);
      }
      catch (Exception ex) when (ThrowData != null && ThrowData(stream))
      {
        throw new StreamReadLongException(array, total, ex);
      }
    }

    #endregion
    #endregion
    #region Write methods
    #region Write size methods

    public static void WriteSize(this Stream stream, int value, SizeEncoding sizeEncoding, Endian endian = Endian.Default)
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException("value ");

			switch (sizeEncoding)
			{
				case SizeEncoding.NO:
					break;
				case SizeEncoding.B1:
					stream.WriteSByte(Convert.ToSByte(value));
					break;
				case SizeEncoding.B2:
					stream.WriteInt16(Convert.ToInt16(value), endian);
					break;
				case SizeEncoding.B4:
					stream.WriteInt32(value, endian);
					break;
				case SizeEncoding.B8:
					stream.WriteInt64((Int64)value, endian);
					break;
				case SizeEncoding.VB:
					stream.WriteInt32Vb(value);
					break;
				default:
					throw new ArgumentOutOfRangeException("sizeEncoding");
			}
		}

		public static void WriteLongSize(this Stream stream, long value, SizeEncoding sizeEncoding, Endian endian = Endian.Default)
		{
			if (value < 0L)
				throw new ArgumentOutOfRangeException("value ");

			switch (sizeEncoding)
			{
				case SizeEncoding.NO:
					break;
				case SizeEncoding.B1:
					stream.WriteSByte(Convert.ToSByte(value));
					break;
				case SizeEncoding.B2:
					stream.WriteInt16(Convert.ToInt16(value), endian);
					break;
				case SizeEncoding.B4:
					stream.WriteInt32(Convert.ToInt32(value), endian);
					break;
				case SizeEncoding.B8:
					stream.WriteInt64(value, endian);
					break;
				case SizeEncoding.VB:
					stream.WriteInt64Vb(value);
					break;
				default:
					throw new ArgumentOutOfRangeException("sizeEncoding");
			}
		}

    #endregion
    #region Write base type methods

    public static void WriteSByte(this Stream stream, SByte value)
		{
			stream.WriteByte((byte)value);
		}

		public static void WriteInt16(this Stream stream, Int16 value)
		{
			stream.WriteInt16(value, Endian.Default);
		}

		public static void WriteInt32(this Stream stream, Int32 value)
		{
			stream.WriteInt32(value, Endian.Default);
		}

		public static void WriteInt64(this Stream stream, Int64 value)
		{
			stream.WriteInt64(value, Endian.Default);
		}

    public static void WriteUByte(this Stream stream, Byte value)
    {
      byte[] buffer = new byte[] { value };
      stream.WriteBytes(buffer);
    }

    public static void WriteUInt16(this Stream stream, UInt16 value)
		{
			stream.WriteUInt16(value, Endian.Default);
		}

		public static void WriteUInt32(this Stream stream, UInt32 value)
		{
			stream.WriteUInt32(value, Endian.Default);
		}

		public static void WriteUInt64(this Stream stream, UInt64 value)
		{
			stream.WriteUInt64(value, Endian.Default);
		}

		public static void WriteInt16(this Stream stream, Int16 value, Endian endian)
		{
			byte[] buffer = BitConverter.GetBytes(value);
			if ((endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian))
				Array.Reverse(buffer);
			stream.WriteBytes(buffer);
		}

		public static void WriteInt32(this Stream stream, Int32 value, Endian endian)
		{
			byte[] buffer = BitConverter.GetBytes(value);
			if ((endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian))
				Array.Reverse(buffer);
			stream.WriteBytes(buffer);
		}

		public static void WriteInt64(this Stream stream, Int64 value, Endian endian)
		{
			byte[] buffer = BitConverter.GetBytes(value);
			if ((endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian))
				Array.Reverse(buffer);
			stream.WriteBytes(buffer);
		}

		public static void WriteUInt16(this Stream stream, UInt16 value, Endian endian)
		{
			byte[] buffer = BitConverter.GetBytes(value);
			if ((endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian))
				Array.Reverse(buffer);
			stream.WriteBytes(buffer);
		}

		public static void WriteUInt32(this Stream stream, UInt32 value, Endian endian)
		{
			byte[] buffer = BitConverter.GetBytes(value);
			if ((endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian))
				Array.Reverse(buffer);
			stream.WriteBytes(buffer);
		}

		public static void WriteUInt64(this Stream stream, UInt64 value, Endian endian)
		{
			byte[] buffer = BitConverter.GetBytes(value);
			if ((endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian))
				Array.Reverse(buffer);
			stream.WriteBytes(buffer);
		}

		public static void WriteInt16Vb(this Stream stream, Int16 value)
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException("value ");

			for (; value >= 0x80; value >>= 7)
				stream.WriteByte((byte)(value | 0x80));
			stream.WriteByte((byte)value);
		}

		public static void WriteInt32Vb(this Stream stream, Int32 value)
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException("value ");

			for (; value >= 0x80; value >>= 7)
				stream.WriteByte((byte)(value | 0x80));
			stream.WriteByte((byte)value);
		}

		public static void WriteInt64Vb(this Stream stream, Int64 value)
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException("value ");

			for (; value >= 0x80; value >>= 7)
				stream.WriteByte((byte)(value | 0x80));
			stream.WriteByte((byte)value);
		}

		public static void WriteUInt16Vb(this Stream stream, UInt16 value)
		{
			for (; value >= 0x80; value >>= 7)
				stream.WriteByte((byte)(value | 0x80));
			stream.WriteByte((byte)value);
		}

		public static void WriteUInt32Vb(this Stream stream, UInt32 value)
		{
			for (; value >= 0x80; value >>= 7)
				stream.WriteByte((byte)(value | 0x80));
			stream.WriteByte((byte)value);
		}

		public static void WriteUInt64Vb(this Stream stream, UInt64 value)
		{
			for (; value >= 0x80; value >>= 7)
				stream.WriteByte((byte)(value | 0x80));
			stream.WriteByte((byte)value);
		}

		public static void WriteSingle(this Stream stream, Single value)
		{
			stream.WriteSingle(value, Endian.Default);
		}

		public static void WriteDouble(this Stream stream, Double value)
		{
			stream.WriteDouble(value, Endian.Default);
		}

    public static void WriteSingle(this Stream stream, Single value, Endian endian)
    {
      byte[] buffer = BitConverter.GetBytes(value);
      if ((endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian))
        Array.Reverse(buffer);
      stream.WriteBytes(buffer);
    }

    public static void WriteDouble(this Stream stream, Double value, Endian endian)
    {
      byte[] buffer = BitConverter.GetBytes(value);
      if ((endian == Endian.Big && BitConverter.IsLittleEndian || endian == Endian.Little && !BitConverter.IsLittleEndian))
        Array.Reverse(buffer);
      stream.WriteBytes(buffer);
    }

		public static void WriteDecimal(this Stream stream, Decimal value)
		{
			stream.WriteBytes(Decimal.GetBits(value).SelectMany(v => BitConverter.GetBytes(v)).ToArray());
		}

		public static void WriteGuid(this Stream stream, Guid value)
		{
			stream.WriteBytes(value.ToByteArray());
		}

		public static void WriteDateTime(this Stream stream, DateTime value)
		{
			stream.WriteInt64(value.Ticks, Endian.Default);
		}

		public static void WriteDateTime(this Stream stream, DateTime value, Endian endian)
		{
			stream.WriteInt64(value.Ticks, endian);
		}

		public static void WriteTimeSpan(this Stream stream, TimeSpan value)
		{
			stream.WriteInt64(value.Ticks, Endian.Default);
		}

		public static void WriteTimeSpan(this Stream stream, TimeSpan value, Endian endian)
		{
			stream.WriteInt64(value.Ticks, endian);
		}

		public static void WriteBoolean(this Stream stream, bool value, TypeCode typeCode, bool allBits)
		{
			switch (typeCode)
			{
				case TypeCode.Byte:
					stream.WriteByte(value ? allBits ? Byte.MaxValue : (Byte)1 : (Byte)0);
					break;
				case TypeCode.UInt16:
					stream.WriteUInt16(value ? allBits ? UInt16.MaxValue : (UInt16)1 : (UInt16)0);
					break;
				case TypeCode.UInt32:
					stream.WriteUInt32(value ? allBits ? UInt32.MaxValue : 1U : 0U);
					break;
				case TypeCode.UInt64:
					stream.WriteUInt64(value ? allBits ? UInt64.MaxValue : 1UL : 0UL);
					break;
				case TypeCode.SByte:
					stream.WriteSByte((SByte)(value ? allBits ? -1 : 1 : 0));
					break;
				case TypeCode.Int16:
					stream.WriteInt16((Int16)(value ? allBits ? -1 : 1 : 0));
					break;
				case TypeCode.Int32:
					stream.WriteInt32(value ? allBits ? -1 : 1 : 0);
					break;
				case TypeCode.Int64:
					stream.WriteInt64(value ? allBits ? -1L : 1L : 0L);
					break;
				default:
					throw new ArgumentOutOfRangeException("Invalid argument TypeCode value ", "typeCode");
			}
		}

		public static void WriteEnum<T>(this Stream stream, T value)
			where T : struct, IComparable, IFormattable, IConvertible
		{
			stream.WriteEnum<T>(value, Endian.Default);
		}

		public static void WriteEnum<T>(this Stream stream, T value, Endian endian)
			where T : struct, IComparable, IFormattable, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new InvalidOperationException(string.Format("Type '{0}' is not enum.", typeof(T).FullName));

			switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))))
			{
				case TypeCode.Byte:
					stream.WriteByte(value.ToByte(null));
					break;
				case TypeCode.UInt16:
					stream.WriteUInt16(value.ToUInt16(null), endian);
					break;
				case TypeCode.UInt32:
					stream.WriteUInt32(value.ToUInt32(null), endian);
					break;
				case TypeCode.UInt64:
					stream.WriteUInt64(value.ToUInt64(null), endian);
					break;
				case TypeCode.SByte:
					stream.WriteSByte(value.ToSByte(null));
					break;
				case TypeCode.Int16:
					stream.WriteInt16(value.ToInt16(null), endian);
					break;
				case TypeCode.Int32:
					stream.WriteInt32(value.ToInt32(null), endian);
					break;
				case TypeCode.Int64:
					stream.WriteInt64(value.ToInt64(null), endian);
					break;
				default:
					throw new InvalidOperationException("Unsupported enum underlying type");
			}
		}

		public static void WriteChars(this Stream stream, Char[] value, Encoding encoding)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (value == null)
				throw new ArgumentNullException("value ");
			if (encoding == null)
				throw new ArgumentNullException("encoding");

			stream.WriteBytes(encoding.GetBytes(value));
		}

		public static void WriteChars(this Stream stream, Char[] value, Encoding encoding, Action<Stream, int> sizeWriter)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (value == null)
				throw new ArgumentNullException("value ");
			if (encoding == null)
				throw new ArgumentNullException("encoding");
			if (sizeWriter == null)
				throw new ArgumentNullException("sizeWriter");

			byte[] buffer = encoding.GetBytes(value);
			sizeWriter(stream, buffer.Length);
			stream.WriteBytes(buffer);
		}

		public static void WriteChars(this Stream stream, Char[] value, Encoding encoding, SizeEncoding sizeEncoding)
		{
			stream.WriteChars(value, encoding, (s, length) => s.WriteSize(length, sizeEncoding, Endian.Default));
		}

		public static void WriteChars(this Stream stream, Char[] value, Encoding encoding, SizeEncoding sizeEncoding, Endian endian)
		{
			stream.WriteChars(value, encoding, (s, length) => s.WriteSize(length, sizeEncoding, endian));
		}

		public static void WriteChars(this Stream stream, Char[] value, int index, int count, Encoding encoding)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (value == null)
				throw new ArgumentNullException("value ");
			if (encoding == null)
				throw new ArgumentNullException("encoding");
			if (index < 0 || index > value.Length)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > value.Length - index)
				throw new ArgumentOutOfRangeException("count");

			stream.WriteBytes(encoding.GetBytes(value, index, count));
		}

		public static void WriteChars(this Stream stream, Char[] value, int index, int count, Encoding encoding, Action<Stream, int> sizeWriter)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (value == null)
				throw new ArgumentNullException("value ");
			if (encoding == null)
				throw new ArgumentNullException("encoding");
			if (sizeWriter == null)
				throw new ArgumentNullException("sizeWriter");
			if (index < 0 || index > value.Length)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > value.Length - index)
				throw new ArgumentOutOfRangeException("count");

			byte[] buffer = encoding.GetBytes(value, index, count);
			sizeWriter(stream, buffer.Length);
			stream.WriteBytes(buffer);
		}

		public static void WriteChars(this Stream stream, Char[] value, int index, int count, Encoding encoding, SizeEncoding sizeEncoding)
		{
			stream.WriteChars(value, index, count, encoding, (s, length) => s.WriteSize(length, sizeEncoding, Endian.Default));
		}

		public static void WriteChars(this Stream stream, Char[] value, int index, int count, Encoding encoding, SizeEncoding sizeEncoding, Endian endian)
		{
			stream.WriteChars(value, index, count, encoding, (s, length) => s.WriteSize(length, sizeEncoding, endian));
		}

		public static void WriteString(this Stream stream, String value, Encoding encoding)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (value == null)
				throw new ArgumentNullException("value");
			if (encoding == null)
				throw new ArgumentNullException("encoding");

			stream.WriteBytes(encoding.GetBytes(value));
		}

		public static void WriteString(this Stream stream, String value, Encoding encoding, Action<Stream, int> sizeWriter)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (value == null)
				throw new ArgumentNullException("value ");
			if (encoding == null)
				throw new ArgumentNullException("encoding");
			if (sizeWriter == null)
				throw new ArgumentNullException("sizeWriter");

			byte[] buffer = encoding.GetBytes(value);
			sizeWriter(stream, buffer.Length);
			stream.WriteBytes(buffer);
		}

		public static void WriteString(this Stream stream, String value, Encoding encoding, SizeEncoding sizeEncoding)
		{
			stream.WriteString(value, encoding, (s, length) => s.WriteSize(length, sizeEncoding, Endian.Default));
		}

		public static void WriteString(this Stream stream, String value, Encoding encoding, SizeEncoding sizeEncoding, Endian endian)
		{
			stream.WriteString(value, encoding, (s, length) => s.WriteSize(length, sizeEncoding, endian));
		}

		public static void WriteString(this Stream stream, String value, Encoding encoding, String terminator)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (value == null)
				throw new ArgumentNullException("value ");
			if (string.IsNullOrEmpty(terminator))
				throw new ArgumentException("Terminator is not specified", "terminator");
			if (encoding == null)
				throw new ArgumentNullException("encoding");

			stream.WriteBytes(encoding.GetBytes(value));
			stream.WriteBytes(encoding.GetBytes(terminator));
		}

		public static void WriteBytes(this Stream stream, byte[] value, Action<Stream, int> sizeWriter)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (value == null)
				throw new ArgumentNullException("value ");
			if (sizeWriter == null)
				throw new ArgumentNullException("sizeWriter");

			sizeWriter(stream, value.Length);
			stream.WriteBytes(value);
		}

		public static void WriteBytes(this Stream stream, byte[] value, SizeEncoding sizeEncoding)
		{
			stream.WriteBytes(value, (s, length) => s.WriteSize(length, sizeEncoding, Endian.Default));
		}

		public static void WriteBytes(this Stream stream, byte[] value, SizeEncoding sizeEncoding, Endian endian)
		{
			stream.WriteBytes(value, (s, length) => s.WriteSize(length, sizeEncoding, endian));
		}

		public static void WriteBytes(this Stream stream, byte[] value, byte[] terminator)
		{
			stream.WriteBytes(value);
			stream.WriteBytes(terminator);
		}

    public static void WriteObject<T>(this Stream stream, T value, Action<Stream, T> typeWriter, Action<Stream, Boolean> flagWriter)
      where T : class
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (typeWriter == null)
        throw new ArgumentNullException("typeWriter");
      if (flagWriter == null)
        throw new ArgumentNullException("flagWriter");

      flagWriter(stream, value != null);
      if (value != null)
        typeWriter(stream, value);
    }

    public static void WriteNullable<T>(this Stream stream, T? value, Action<Stream, T> typeWriter, Action<Stream, Boolean> flagWriter)
      where T : struct
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (typeWriter == null)
        throw new ArgumentNullException("typeWriter");
      if (flagWriter == null)
        throw new ArgumentNullException("flagWriter");

      flagWriter(stream, value.HasValue);
      if (value.HasValue)
        typeWriter(stream, value.Value);
    }

    public static void WriteVoid(this Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			byte[] buffer = new byte[0];
			stream.Write(buffer, 0, buffer.Length);
		}

    public static void WriteAt<T>(this Stream stream, long position, T value, Action<Stream, T> writer)
    {
      var temp = stream.Position;
      stream.Position = position;
      try
      {
        writer(stream, value);
      }
      finally
      {
        stream.Position = temp;
      }
    }

    #endregion
    #region Write collection methods

    public static void WriteCollection<T>(this Stream stream, IEnumerable<T> collection, Action<Stream, T> itemWriter)
    {
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      stream.WriteCollection(collection, (s, t, i) => itemWriter(s, t));
    }

    public static void WriteCollection<T>(this Stream stream, IEnumerable<T> collection, Action<Stream, T, int> itemWriter)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (collection == null)
        throw new ArgumentNullException("collection");
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      int i = 0;
      foreach (var item in collection)
      {
        try
        {
          itemWriter(stream, item, i);
        }
        catch (Exception ex) when (ThrowData != null && ThrowData(stream))
        {
          throw new StreamWriteException(i, ex);
        }
        i++;
      }
    }

    public static void WriteChars(this Stream stream, IEnumerable<Char> collection, Encoding encoding)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (collection == null)
        throw new ArgumentNullException("collection");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      var encoder = encoding.GetEncoder();
      var chars = new char[1];
      var bytes = new byte[encoding.GetMaxByteCount(1)];
      foreach (var item in collection)
      {
        chars[0] = item;
        stream.Write(bytes, 0, encoder.GetBytes(chars, 0, chars.Length, bytes, 0, false));
      }
    }

    #endregion
    #region Write array methods

    public static void WriteArray<T>(this Stream stream, T[] array, Action<Stream, T> itemWriter)
		{
      stream.WriteArray(array, itemWriter, 0, array != null ? array.Length : 0);
    }

    public static void WriteArray<T>(this Stream stream, T[] array, Action<Stream, T, int> itemWriter)
		{
      stream.WriteArray(array, itemWriter, 0, array != null ? array.Length : 0);
		}

    public static void WriteArray<T>(this Stream stream, T[] array, Action<Stream, T> itemWriter, Range range)
    {
      stream.WriteArray(array, itemWriter, range.Index, range.Count);
    }

    public static void WriteArray<T>(this Stream stream, T[] array, Action<Stream, T, int> itemWriter, Range range)
    {
      stream.WriteArray(array, itemWriter, range.Index, range.Count);
    }

    public static void WriteArray<T>(this Stream stream, T[] array, Action<Stream, T> itemWriter, int index, int count)
    {
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      stream.WriteArray(array, (s, t, i) => itemWriter(s, t), index, count);
    }

    public static void WriteArray<T>(this Stream stream, T[] array, Action<Stream, T, int> itemWriter, int index, int count)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      int total = 0;
      try
      {
        array.Apply((t, i) => { itemWriter(stream, t, i); total = i + 1; }, index, count);
      }
      catch (Exception ex) when (ThrowData != null && ThrowData(stream))
      {
        throw new StreamWriteException(total, ex);
      }
    }

    #endregion
    #region Write regular array methods

    public static void WriteRegularArray<T>(this Stream stream, Array array, Action<Stream, T> itemWriter, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      stream.WriteRegularArray<T>(array, (s, t, fi, di) => itemWriter(s, t), null, zeroBased, range, ranges);
    }

    public static void WriteRegularArray<T>(this Stream stream, Array array, Action<Stream, T, int, int[]> itemWriter, int[] indices, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      int total = 0;
      try
      {
        array.ApplyAsRegular<T>((t, fi, di) => { itemWriter(stream, t, fi, di); total = fi + 1; }, indices, zeroBased, range, ranges);
      }
      catch (Exception ex) when (ThrowData != null && ThrowData(stream))
      {
        throw new StreamWriteException(total, ex);
      }
    }

    #endregion
    #region Write long regular array methods

    public static void WriteLongRegularArray<T>(this Stream stream, Array array, Action<Stream, T> itemWriter, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      stream.WriteLongRegularArray<T>(array, (s, t, fi, di) => itemWriter(s, t), null, zeroBased, range, ranges);
    }

    public static void WriteLongRegularArray<T>(this Stream stream, Array array, Action<Stream, T, long, long[]> itemWriter, long[] indices = null, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      long total = 0L;
      try
      {
        array.ApplyAsLongRegular<T>((t, fi, di) => { itemWriter(stream, t, fi, di); total = fi + 1L; }, indices, zeroBased, range, ranges);
      }
      catch (Exception ex) when (ThrowData != null && ThrowData(stream))
      {
        throw new StreamWriteLongException(total, ex);
      }
    }

    #endregion
    #endregion
  }

  [Flags]
  public enum StreamReadOptions
	{
		NoCheckEnd = 0x01,
		NoAcceptEnd = 0x02,
		ExactSize = 0x04,
		ThrowBuffer = 0x08,
		OmitTerminator = 0x10
	}

  [Flags]
  public enum StreamWriteOptions
  {
    Expand = 0x01,
  }
}
