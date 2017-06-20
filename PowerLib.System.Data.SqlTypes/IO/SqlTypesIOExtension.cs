﻿using System;
using System.IO;
using System.Text;
using System.Data.SqlTypes;
using PowerLib.System.IO;

namespace PowerLib.System.Data.SqlTypes.IO
{
  public static class SqlTypesIOExtension
  {
    private const int sizeofGuid = 16;

    private const int PresetIoBufferSize = 32768;

    private static int ioBufferSize = PresetIoBufferSize;

    public static int IoBufferSize
    {
      get { return ioBufferSize; }
      set { ioBufferSize = value; }
    }

    #region Read sql data types from stream

    public static SqlBoolean ReadSqlBoolean(this Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      switch (stream.ReadByte())
      {
        case 0:
          return SqlBoolean.Null;
        case 1:
          return SqlBoolean.False;
        case 2:
          return SqlBoolean.True;
        default:
          throw new InvalidDataException("Invalid SqlBoolean value format.");
      }
    }

    public static SqlByte ReadSqlByte(this Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      return stream.ReadBoolean(TypeCode.Byte) ? stream.ReadUByte() : SqlByte.Null;
    }

    public static SqlInt16 ReadSqlInt16(this Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      return stream.ReadBoolean(TypeCode.Byte) ? stream.ReadInt16() : SqlInt16.Null;
    }

    public static SqlInt32 ReadSqlInt32(this Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      return stream.ReadBoolean(TypeCode.Byte) ? stream.ReadInt32() : SqlInt32.Null;
    }

    public static SqlInt64 ReadSqlInt64(this Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      return stream.ReadBoolean(TypeCode.Byte) ? stream.ReadInt64() : SqlInt64.Null;
    }

    public static SqlSingle ReadSqlSingle(this Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      return stream.ReadBoolean(TypeCode.Byte) ? stream.ReadSingle() : SqlSingle.Null;
    }

    public static SqlDouble ReadSqlDouble(this Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      return stream.ReadBoolean(TypeCode.Byte) ? stream.ReadDouble() : SqlDouble.Null;
    }

    public static SqlDateTime ReadSqlDateTime(this Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      return stream.ReadBoolean(TypeCode.Byte) ? new DateTime(stream.ReadInt64()) : SqlDateTime.Null;
    }

    public static SqlGuid ReadSqlGuid(this Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      return stream.ReadBoolean(TypeCode.Byte) ? new Guid(stream.ReadBytes(sizeofGuid)) : SqlGuid.Null;
    }

    public static SqlBinary ReadSqlBinary(this Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      return stream.ReadBoolean(TypeCode.Byte) ? stream.ReadBytes(stream.ReadInt32()) : SqlBinary.Null;
    }

    public static SqlBytes ReadSqlBytes(this Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      return stream.ReadBoolean(TypeCode.Byte) ? new SqlBytes(stream.ReadBytes(stream.ReadInt32())) : SqlBytes.Null;
    }

    public static SqlString ReadSqlString(this Stream stream, Encoding encoding)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      return stream.ReadBoolean(TypeCode.Byte) ? encoding.GetString(stream.ReadBytes(stream.ReadInt32Vb())) : SqlString.Null;
    }

    public static SqlChars ReadSqlChars(this Stream stream, Encoding encoding)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      return stream.ReadBoolean(TypeCode.Byte) ? new SqlChars(encoding.GetString(stream.ReadBytes(stream.ReadInt32Vb()))) : SqlChars.Null;
    }

    #endregion
    #region Read sql data types from binary reader

    public static SqlBoolean ReadSqlBoolean(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      switch (reader.ReadByte())
      {
        case 0:
          return SqlBoolean.Null;
        case 1:
          return SqlBoolean.False;
        case 2:
          return SqlBoolean.True;
        default:
          throw new InvalidDataException("Invalid SqlBoolean value format.");
      }
    }

    public static SqlByte ReadSqlByte(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.ReadBoolean() ? reader.ReadByte() : SqlByte.Null;
    }

    public static SqlInt16 ReadSqlInt16(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.ReadBoolean() ? reader.ReadInt16() : SqlInt16.Null;
    }

    public static SqlInt32 ReadSqlInt32(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.ReadBoolean() ? reader.ReadInt32() : SqlInt32.Null;
    }

    public static SqlInt64 ReadSqlInt64(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.ReadBoolean() ? reader.ReadInt64() : SqlInt64.Null;
    }

    public static SqlSingle ReadSqlSingle(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.ReadBoolean() ? reader.ReadSingle() : SqlSingle.Null;
    }

    public static SqlDouble ReadSqlDouble(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.ReadBoolean() ? reader.ReadDouble() : SqlDouble.Null;
    }

    public static SqlDateTime ReadSqlDateTime(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.ReadBoolean() ? new DateTime(reader.ReadInt64()) : SqlDateTime.Null;
    }

    public static SqlGuid ReadSqlGuid(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.ReadBoolean() ? new Guid(reader.ReadBytes(sizeofGuid)) : SqlGuid.Null;
    }

    public static SqlBinary ReadSqlBinary(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.ReadBoolean() ? reader.ReadBytes(reader.ReadInt32()) : SqlBinary.Null;
    }

    public static SqlBytes ReadSqlBytes(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.ReadBoolean() ? new SqlBytes(reader.ReadBytes(reader.ReadInt32())) : SqlBytes.Null;
    }

    public static SqlString ReadSqlString(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.ReadBoolean() ? reader.ReadString() : SqlString.Null;
    }

    public static SqlChars ReadSqlChars(this BinaryReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      return reader.ReadBoolean() ? new SqlChars(reader.ReadString()) : SqlChars.Null;
    }

    public static SqlString ReadSqlString(this BinaryReader reader, Encoding encoding)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      return reader.ReadBoolean() ? encoding.GetString(reader.ReadBytes(reader.ReadInt32Mb())) : SqlString.Null;
    }

    public static SqlChars ReadSqlChars(this BinaryReader reader, Encoding encoding)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      return reader.ReadBoolean() ? new SqlChars(encoding.GetString(reader.ReadBytes(reader.ReadInt32Mb()))) : SqlChars.Null;
    }

    #endregion
    #region Write sql data types to stream

    public static void Write(this Stream stream, SqlBoolean value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Write((Byte)(value.IsNull ? 0 : !value.Value ? 1 : 2));
    }

    public static void Write(this Stream stream, SqlByte value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Write(!value.IsNull);
      if (!value.IsNull)
        stream.Write(value.Value);
    }

    public static void Write(this Stream stream, SqlInt16 value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Write(!value.IsNull);
      if (!value.IsNull)
        stream.Write(value.Value);
    }

    public static void Write(this Stream stream, SqlInt32 value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Write(!value.IsNull);
      if (!value.IsNull)
        stream.Write(value.Value);
    }

    public static void Write(this Stream stream, SqlInt64 value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Write(!value.IsNull);
      if (!value.IsNull)
        stream.Write(value.Value);
    }

    public static void Write(this Stream stream, SqlSingle value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Write(!value.IsNull);
      if (!value.IsNull)
        stream.Write(value.Value);
    }

    public static void Write(this Stream stream, SqlDouble value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Write(!value.IsNull);
      if (!value.IsNull)
        stream.Write(value.Value);
    }

    public static void Write(this Stream stream, SqlDateTime value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Write(!value.IsNull);
      if (!value.IsNull)
        stream.Write(value.Value.Ticks);
    }

    public static void Write(this Stream stream, SqlGuid value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Write(!value.IsNull);
      if (!value.IsNull)
        stream.Write(value.Value.ToByteArray());
    }

    public static void Write(this Stream stream, SqlBinary value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Write(!value.IsNull);
      if (!value.IsNull)
      {
        byte[] data = value.Value;
        stream.Write(data.Length);
        stream.Write(data);
      }
    }

    public static void Write(this Stream stream, SqlBytes value)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      stream.Write(!value.IsNull);
      if (!value.IsNull)
      {
        stream.Write((int)value.Length);
        switch (value.Storage)
        {
          case StorageState.Buffer:
            stream.Write(value.Buffer, 0, (int)value.Length);
            break;
          case StorageState.Stream:
            value.Stream.Copy(stream, int.MaxValue, IoBufferSize);
            break;
          default:
            throw new InvalidOperationException("Unsupported SqlBytes storage.");
        }
      }
    }

    public static void Write(this Stream stream, SqlString value, Encoding encoding)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      stream.Write(!value.IsNull);
      if (!value.IsNull)
      {
        byte[] data = encoding.GetBytes(value.Value);
        stream.WriteInt32Vb(data.Length);
        stream.Write(data);
      }
    }

    public static void Write(this Stream stream, SqlChars value, Encoding encoding)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      stream.Write(!value.IsNull);
      if (!value.IsNull)
      {
        byte[] data = encoding.GetBytes(value.Buffer, 0, (int)value.Length);
        stream.WriteInt32Vb(data.Length);
        stream.Write(data);
      }
    }

    #endregion
    #region Write sql data types to binary writer

    public static void Write(this BinaryWriter writer, SqlBoolean value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write((Byte)(value.IsNull ? 0 : !value.Value ? 1 : 2));
    }

    public static void Write(this BinaryWriter writer, SqlByte value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, SqlInt16 value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, SqlInt32 value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, SqlInt64 value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, SqlSingle value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, SqlDouble value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, SqlDateTime value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
        writer.Write(value.Value.Ticks);
    }

    public static void Write(this BinaryWriter writer, SqlGuid value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
        writer.Write(value.Value.ToByteArray());
    }

    public static void Write(this BinaryWriter writer, SqlBinary value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
      {
        byte[] data = value.Value;
        writer.Write(data.Length);
        writer.Write(data);
      }
    }

    public static void Write(this BinaryWriter writer, SqlBytes value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
      {
        writer.Write((int)value.Length);
        switch (value.Storage)
        {
          case StorageState.Buffer:
            writer.Write(value.Buffer, 0, (int)value.Length);
            break;
          case StorageState.Stream:
            value.Stream.Copy(writer.BaseStream, int.MaxValue, IoBufferSize);
            break;
          default:
            throw new InvalidOperationException("Unsupported SqlBytes storage.");
        }
      }
    }

    public static void Write(this BinaryWriter writer, SqlString value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, SqlChars value)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
        writer.Write(value.Value);
    }

    public static void Write(this BinaryWriter writer, SqlString value, Encoding encoding)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
      {
        byte[] data = encoding.GetBytes(value.Value);
        writer.WriteMb(data.Length);
        writer.Write(data);
      }
    }

    public static void Write(this BinaryWriter writer, SqlChars value, Encoding encoding)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      writer.Write(!value.IsNull);
      if (!value.IsNull)
      {
        byte[] data = encoding.GetBytes(value.Buffer, 0, (int)value.Length);
        writer.WriteMb(data.Length);
        writer.Write(data);
      }
    }

    #endregion
  }
}
