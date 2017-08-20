using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PowerLib.System.Linq;

namespace PowerLib.System.IO
{
	public static class BinaryWriterExtension
	{
    public static bool ThrowDataInfo = false;

    #region Write size

    public static void WriteSize(this BinaryWriter writer, int value, SizeEncoding sizeEncoding, Endian? endian = null)
		{
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (value < 0)
        throw new ArgumentOutOfRangeException("value");

      if (endian.HasValue)
        writer.BaseStream.WriteSize(value, sizeEncoding, endian.Value);
      else
        switch (sizeEncoding)
        {
          case SizeEncoding.NO:
            break;
          case SizeEncoding.B1:
            writer.Write(Convert.ToSByte(value));
            break;
          case SizeEncoding.B2:
            writer.Write(Convert.ToInt16(value));
            break;
          case SizeEncoding.B4:
            writer.Write(value);
            break;
          case SizeEncoding.B8:
            writer.Write((Int64)value);
            break;
          case SizeEncoding.VB:
            writer.WriteMb(value);
            break;
          default:
            throw new ArgumentOutOfRangeException("sizeEncoding");
        }
		}

		public static void WriteLongSize(this BinaryWriter writer, long value, SizeEncoding sizeEncoding, Endian? endian = null)
		{
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (value < 0L)
        throw new ArgumentOutOfRangeException("value");

      if (endian.HasValue)
        writer.BaseStream.WriteLongSize(value, sizeEncoding, endian.Value);
      else
        switch (sizeEncoding)
        {
          case SizeEncoding.NO:
            break;
          case SizeEncoding.B1:
            writer.Write(Convert.ToSByte(value));
            break;
          case SizeEncoding.B2:
            writer.Write(Convert.ToInt16(value));
            break;
          case SizeEncoding.B4:
            writer.Write(Convert.ToInt32(value));
            break;
          case SizeEncoding.B8:
            writer.Write(value);
            break;
          case SizeEncoding.VB:
            writer.WriteMb(value);
            break;
          default:
            throw new ArgumentOutOfRangeException("sizeEncoding");
        }
		}

    #endregion
    #region Write boolean

    public static void Write(this BinaryWriter writer, bool value, TypeCode typeCode)
		{
			writer.Write(value, typeCode, false);
		}

		public static void Write(this BinaryWriter writer, bool value, TypeCode typeCode, bool allBits)
		{
			writer.BaseStream.WriteBoolean(value, typeCode, allBits);
		}

		#endregion
		#region Write integer

		public static void Write(this BinaryWriter writer, Int16 value, Endian endian)
		{
			if (writer == null)
				throw new NullReferenceException();

			writer.BaseStream.WriteInt16(value, endian);
		}

		public static void Write(this BinaryWriter writer, Int32 value, Endian endian)
		{
			if (writer == null)
				throw new NullReferenceException();

			writer.BaseStream.WriteInt32(value, endian);
		}

		public static void Write(this BinaryWriter writer, Int64 value, Endian endian)
		{
			if (writer == null)
				throw new NullReferenceException();

			writer.BaseStream.WriteInt64(value, endian);
		}

		public static void Write(this BinaryWriter writer, UInt16 value, Endian endian)
		{
			if (writer == null)
				throw new NullReferenceException();

			writer.BaseStream.WriteUInt16(value, endian);
		}

		public static void Write(this BinaryWriter writer, UInt32 value, Endian endian)
		{
			if (writer == null)
				throw new NullReferenceException();

			writer.BaseStream.WriteUInt32(value, endian);
		}

		public static void Write(this BinaryWriter writer, UInt64 value, Endian endian)
		{
			if (writer == null)
				throw new NullReferenceException();

			writer.BaseStream.WriteUInt64(value, endian);
		}

		public static void WriteMb(this BinaryWriter writer, Int16 value)
		{
			if (writer == null)
				throw new NullReferenceException();

			writer.BaseStream.WriteInt16Vb(value);
		}

		public static void WriteMb(this BinaryWriter writer, Int32 value)
		{
			if (writer == null)
				throw new NullReferenceException();

			writer.BaseStream.WriteInt32Vb(value);
		}

		public static void WriteMb(this BinaryWriter writer, Int64 value)
		{
			if (writer == null)
				throw new NullReferenceException();

			writer.BaseStream.WriteInt64Vb(value);
		}

		public static void WriteMb(this BinaryWriter writer, UInt16 value)
		{
			if (writer == null)
				throw new NullReferenceException();

			writer.BaseStream.WriteUInt16Vb(value);
		}

		public static void WriteMb(this BinaryWriter writer, UInt32 value)
		{
			if (writer == null)
				throw new NullReferenceException();

			writer.BaseStream.WriteUInt32Vb(value);
		}

		public static void WriteMb(this BinaryWriter writer, UInt64 value)
		{
			if (writer == null)
				throw new NullReferenceException();

			writer.BaseStream.WriteUInt64Vb(value);
		}

		#endregion
    #region Write real

    public static void Write(this BinaryWriter writer, Single value, Endian endian)
    {
      if (writer == null)
        throw new NullReferenceException();

      writer.BaseStream.WriteSingle(value, endian);
    }

    public static void Write(this BinaryWriter writer, Double value, Endian endian)
    {
      if (writer == null)
        throw new NullReferenceException();

      writer.BaseStream.WriteDouble(value, endian);
    }

    #endregion
    #region Write string

    public static void Write(this BinaryWriter writer, char[] value, Encoding encoding)
		{
			if (writer == null)
				throw new NullReferenceException();
			if (value == null)
				throw new ArgumentNullException("value ");
			if (encoding == null)
				throw new ArgumentNullException("encoding");

			writer.BaseStream.WriteChars(value, encoding);
		}

		public static void Write(this BinaryWriter writer, char[] value, Encoding encoding, SizeEncoding sizeEncoding)
		{
			if (writer == null)
				throw new NullReferenceException();
			if (value == null)
				throw new ArgumentNullException("value ");
			if (encoding == null)
				throw new ArgumentNullException("encoding");

			writer.BaseStream.WriteChars(value, encoding, sizeEncoding);
		}

		public static void Write(this BinaryWriter writer, char[] value, int index, int count, Encoding encoding)
		{
			if (writer == null)
				throw new NullReferenceException();
			if (value == null)
				throw new ArgumentNullException("value ");
			if (encoding == null)
				throw new ArgumentNullException("encoding");

			writer.BaseStream.WriteChars(value, index, count, encoding);
		}

		public static void Write(this BinaryWriter writer, char[] value, int index, int count, Encoding encoding, SizeEncoding sizeEncoding)
		{
			if (writer == null)
				throw new NullReferenceException();
			if (value == null)
				throw new ArgumentNullException("value ");
			if (encoding == null)
				throw new ArgumentNullException("encoding");

			writer.BaseStream.WriteChars(value, index, count, encoding, sizeEncoding);
		}

		public static void Write(this BinaryWriter writer, string value, Encoding encoding)
		{
			if (writer == null)
				throw new NullReferenceException();
			if (value == null)
				throw new ArgumentNullException("value ");
			if (encoding == null)
				throw new ArgumentNullException("encoding");

			writer.BaseStream.WriteString(value, encoding);
		}

		public static void Write(this BinaryWriter writer, string value, Encoding encoding, SizeEncoding sizeEncoding)
		{
			if (writer == null)
				throw new NullReferenceException();
			if (value == null)
				throw new ArgumentNullException("value ");
			if (encoding == null)
				throw new ArgumentNullException("encoding");

			writer.BaseStream.WriteString(value, encoding, sizeEncoding);
		}

		public static void Write(this BinaryWriter writer, string value, Encoding encoding, string terminator)
		{
			if (writer == null)
				throw new NullReferenceException();
			if (value == null)
				throw new ArgumentNullException("value ");
			if (encoding == null)
				throw new ArgumentNullException("encoding");
			if (string.IsNullOrEmpty(terminator))
				throw new ArgumentException("Value is not specified", "terminator");

			writer.BaseStream.WriteString(value, encoding, terminator);
		}

		#endregion
		#region Write enum

		public static void WriteEnum<T>(this BinaryWriter writer, T value)
			where T : struct, IComparable, IFormattable, IConvertible
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
      if (!typeof(T).IsEnum)
        throw new InvalidOperationException(string.Format("Type '{0}' is not enum.", typeof(T).FullName));

      switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))))
      {
        case TypeCode.Byte:
          writer.Write(value.ToByte(null));
          break;
        case TypeCode.UInt16:
          writer.Write(value.ToUInt16(null));
          break;
        case TypeCode.UInt32:
          writer.Write(value.ToUInt32(null));
          break;
        case TypeCode.UInt64:
          writer.Write(value.ToUInt64(null));
          break;
        case TypeCode.SByte:
          writer.Write(value.ToSByte(null));
          break;
        case TypeCode.Int16:
          writer.Write(value.ToInt16(null));
          break;
        case TypeCode.Int32:
          writer.Write(value.ToInt32(null));
          break;
        case TypeCode.Int64:
          writer.Write(value.ToInt64(null));
          break;
        default:
          throw new InvalidOperationException("Unsupported enum underlying type");
      }
    }

    public static void WriteEnum<T>(this BinaryWriter writer, T value, Endian endian)
			where T : struct, IComparable, IFormattable, IConvertible
		{
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (!typeof(T).IsEnum)
        throw new InvalidOperationException(string.Format("Type '{0}' is not enum.", typeof(T).FullName));

      switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))))
      {
        case TypeCode.Byte:
          writer.Write(value.ToByte(null));
          break;
        case TypeCode.UInt16:
          writer.Write(value.ToUInt16(null), endian);
          break;
        case TypeCode.UInt32:
          writer.Write(value.ToUInt32(null), endian);
          break;
        case TypeCode.UInt64:
          writer.Write(value.ToUInt64(null), endian);
          break;
        case TypeCode.SByte:
          writer.Write(value.ToSByte(null), endian);
          break;
        case TypeCode.Int16:
          writer.Write(value.ToInt16(null), endian);
          break;
        case TypeCode.Int32:
          writer.Write(value.ToInt32(null), endian);
          break;
        case TypeCode.Int64:
          writer.Write(value.ToInt64(null), endian);
          break;
        default:
          throw new InvalidOperationException("Unsupported enum underlying type");
      }
    }

    #endregion
    #region Write others methods

    public static void Write(this BinaryWriter writer, Guid value)
    {
      writer.Write(value.ToByteArray());
    }

    public static void Write(this BinaryWriter writer, DateTime value)
    {
      writer.Write(value.Ticks);
    }

    public static void Write(this BinaryWriter writer, DateTime value, Endian endian)
    {
      writer.Write(value.Ticks, endian);
    }

    public static void Write(this BinaryWriter writer, TimeSpan value)
    {
      writer.Write(value.Ticks);
    }

    public static void Write(this BinaryWriter writer, TimeSpan value, Endian endian)
    {
      writer.Write(value.Ticks, endian);
    }

    #endregion
    #region Write structured methods

    public static void Write<T>(this BinaryWriter writer, T? value, Action<BinaryWriter, T> typeWriter)
      where T : struct
    {
      if (writer == null)
        throw new NullReferenceException("writer");
      if (typeWriter == null)
        throw new ArgumentNullException("typeWriter");

      writer.Write(value.HasValue);
      if (value.HasValue)
        typeWriter(writer, value.Value);
    }

    public static void Write<T>(this BinaryWriter writer, T value, Action<BinaryWriter, T> typeWriter)
      where T : class
    {
      if (writer == null)
        throw new NullReferenceException("writer");
      if (typeWriter == null)
        throw new ArgumentNullException("typeWriter");

      writer.Write(value != null);
      if (value != null)
        typeWriter(writer, value);
    }

    public static void Write<T>(this BinaryWriter writer, T? value, Action<BinaryWriter, T> typeWriter, Action<BinaryWriter, Boolean> flagWriter)
			where T : struct
		{
			if (writer == null)
				throw new NullReferenceException("writer");
			if (typeWriter == null)
				throw new ArgumentNullException("typeWriter");
			if (flagWriter == null)
				throw new ArgumentNullException("flagWriter");

			flagWriter(writer, value.HasValue);
			if (value.HasValue)
				typeWriter(writer, value.Value);
		}

		public static void Write<T>(this BinaryWriter writer, T value, Action<BinaryWriter, T> typeWriter, Action<BinaryWriter, Boolean> flagWriter)
			where T : class
		{
			if (writer == null)
				throw new NullReferenceException("writer");
			if (typeWriter == null)
				throw new ArgumentNullException("typeWriter");
			if (flagWriter == null)
				throw new ArgumentNullException("flagWriter");

			flagWriter(writer, value != null);
			if (value != null)
				typeWriter(writer, value);
		}

		#endregion
    #region Write bytes methods

    public static void Write(this BinaryWriter writer, byte[] value, SizeEncoding sizeEncoding)
		{
			if (writer == null)
				throw new NullReferenceException();
			if (value == null)
				throw new ArgumentNullException("value ");

			writer.WriteSize(value.Length, sizeEncoding);
			writer.Write(value, 0, value.Length);
		}

		public static void Write(this BinaryWriter writer, byte[] value, int index, int count, SizeEncoding sizeEncoding)
		{
			if (writer == null)
				throw new NullReferenceException();
			if (value == null)
				throw new ArgumentNullException("value ");
			if (index < 0 || index > value.Length)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > value.Length - index)
				throw new ArgumentOutOfRangeException("count");

			writer.WriteSize(count, sizeEncoding);
			writer.Write(value, index, count);
		}

    #endregion
    #region Write collection methods

    public static void WriteCollection<T>(this BinaryWriter writer, IEnumerable<T> collection, Action<BinaryWriter, T> itemWriter)
    {
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      writer.WriteCollection(collection, (wr, t, i) => itemWriter(wr, t));
    }

    public static void WriteCollection<T>(this BinaryWriter writer, IEnumerable<T> collection, Action<BinaryWriter, T, int> itemWriter)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      int total = 0;
      try
      {
        collection.ForEach((v, i) => { itemWriter(writer, v, i); total = i + 1; });
      }
      catch (Exception ex) when (ThrowDataInfo)
      {
        throw new StreamWriteException(total, ex);
      }
    }

    #endregion
    #region Write array methods

    public static void WriteArray<T>(this BinaryWriter writer, T[] array, Action<BinaryWriter, T> itemWriter)
		{
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      writer.WriteArray(array, (wr, t, i) => itemWriter(wr, t));
		}

		public static void WriteArray<T>(this BinaryWriter writer, T[] array, Action<BinaryWriter, T, int> itemWriter)
		{
      if (array == null)
        throw new ArgumentNullException("array");

      writer.WriteArray(array, itemWriter, 0, array.Length);
		}

    public static void WriteArray<T>(this BinaryWriter writer, T[] array, Action<BinaryWriter, T> itemWriter, Range range)
    {
      writer.WriteArray(array, itemWriter, range.Index, range.Count);
    }

    public static void WriteArray<T>(this BinaryWriter writer, T[] array, Action<BinaryWriter, T, int> itemWriter, Range range)
    {
      writer.WriteArray(array, itemWriter, range.Index, range.Count);
    }

    public static void WriteArray<T>(this BinaryWriter writer, T[] array, Action<BinaryWriter, T> itemWriter, int index, int count)
    {
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      writer.WriteArray(array, (wr, t, i) => itemWriter(wr, t), index, count);
		}

		public static void WriteArray<T>(this BinaryWriter writer, T[] array, Action<BinaryWriter, T, int> itemWriter, int index, int count)
    {
			if (writer == null)
				throw new ArgumentNullException("writer");
      if (itemWriter == null)
				throw new ArgumentNullException("itemWriter");

      int total = 0;
      try
      {
        array.Apply((t, i) => { itemWriter(writer, array[i], i); total = i + 1; }, index, count);
      }
      catch (Exception ex) when (ThrowDataInfo)
      {
        throw new StreamWriteException(total, ex);
      }
    }

    #endregion
    #region Write regular array methods

    public static void WriteRegularArray<T>(this BinaryWriter writer, Array array, Action<BinaryWriter, T> itemWriter, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      writer.WriteRegularArray<T>(array, (wr, t, fi, di) => itemWriter(wr, t), null, zeroBased, range, ranges);
    }

    public static void WriteRegularArray<T>(this BinaryWriter writer, Array array, Action<BinaryWriter, T, int, int[]> itemWriter, int[] indices = null, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      int total = 0;
      try
      {
        array.ApplyAsRegular<T>((t, fi, di) => { itemWriter(writer, t, fi, di); total = fi + 1; }, indices, zeroBased, range, ranges);
      }
      catch (Exception ex) when (ThrowDataInfo)
      {
        throw new StreamWriteException(total, ex);
      }
    }

    #endregion
    #region Write long regular array methods

    public static void WriteLongRegularArray<T>(this BinaryWriter writer, Array array, Action<BinaryWriter, T> itemWriter, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      writer.WriteLongRegularArray<T>(array, (wr, t, fi, di) => itemWriter(wr, t), null, zeroBased, range, ranges);
    }

    public static void WriteLongRegularArray<T>(this BinaryWriter writer, Array array, Action<BinaryWriter, T, long, long[]> itemWriter, long[] indices = null, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (itemWriter == null)
        throw new ArgumentNullException("itemWriter");

      long total = 0;
      try
      {
        array.ApplyAsLongRegular<T>((t, fi, di) => { itemWriter(writer, t, fi, di); total = fi + 1; }, indices, zeroBased, range, ranges);
      }
      catch (Exception ex) when (ThrowDataInfo)
      {
        throw new StreamWriteLongException(total, ex);
      }
    }

    #endregion
  }
}
