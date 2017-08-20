using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.Linq;
using PowerLib.System.IO;
using PowerLib.System.Data.SqlTypes.IO;

namespace PowerLib.SqlServer.Binary
{
	public static class SqlBinaryFunctions
	{
		private const int SizeOfGuid = 16;

    #region Scalar-valued function methods
    #region Manipulation functions

    [SqlFunction(Name = "binInsert", IsDeterministic = true)]
    public static SqlBytes Insert(SqlBytes input, SqlInt64 index, SqlBytes value)
    {
      if (input.IsNull || value.IsNull)
        return SqlBytes.Null;
      if (!index.IsNull && index.Value > input.Length)
        throw new ArgumentOutOfRangeException("index");

      long indexValue = index.IsNull ? input.Length : index.Value;
      SqlBytes result = CreateResult(input.Length + value.Length);
      if (indexValue > 0)
        input.Read(0L, result.Buffer, 0, (int)indexValue);
      value.Read(0L, result.Buffer, (int)indexValue, (int)value.Length);
      if (indexValue < input.Length)
        input.Read(indexValue, result.Buffer, (int)(indexValue + value.Length), (int)(input.Length - indexValue));
      return result;
    }

    [SqlFunction(Name = "binRemove", IsDeterministic = true)]
    public static SqlBytes Remove(SqlBytes input, SqlInt64 index, SqlInt64 count)
    {
      if (input.IsNull)
        return SqlBytes.Null;
      if (!index.IsNull && index.Value > input.Length)
        throw new ArgumentOutOfRangeException("index");
      if (!count.IsNull && count.Value > input.Length - (index.IsNull ? 0 : index.Value))
        throw new ArgumentOutOfRangeException("count");

      long indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : input.Length - count.Value;
      long countValue = !count.IsNull ? count.Value : index.IsNull ? input.Length : input.Length - index.Value;
      SqlBytes result = CreateResult(input.Length - countValue);
      if (indexValue > 0)
        input.Read(0L, result.Buffer, 0, (int)indexValue);
      if (indexValue < input.Length - countValue)
        input.Read(indexValue + countValue, result.Buffer, (int)indexValue, (int)(input.Length - indexValue - countValue));
      return result;
    }

    [SqlFunction(Name = "binReplicate", IsDeterministic = true)]
    public static SqlBytes Replicate(SqlBytes input, SqlInt64 count)
    {
      if (input.IsNull || count.IsNull)
        return SqlBytes.Null;

      SqlBytes result = CreateResult(input.Length * count.Value);
      for (int offset = 0, length = (int)input.Length, c = (int)count.Value; c > 0; offset += length, c--)
        input.Read(0L, result.Buffer, offset, length);
      return result;
    }

    [SqlFunction(Name = "binRange", IsDeterministic = true)]
    public static SqlBytes Range(SqlBytes input, SqlInt64 index, SqlInt64 count)
    {
      if (input.IsNull)
        return SqlBytes.Null;
      if (!index.IsNull && index.Value > input.Length)
        throw new ArgumentOutOfRangeException("index");
      if (!count.IsNull && count.Value > input.Length - (index.IsNull ? 0 : index.Value))
        throw new ArgumentOutOfRangeException("count");

      long indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : input.Length - count.Value;
      long countValue = !count.IsNull ? count.Value : index.IsNull ? input.Length : input.Length - index.Value;
      SqlBytes result = CreateResult(countValue);
      input.Read(indexValue, result.Buffer, 0, (int)countValue);
      return result;
    }

    [SqlFunction(Name = "binReverse", IsDeterministic = true)]
    public static SqlBytes Reverse(SqlBytes input)
    {
      return ReverseRange(input, SqlInt64.Null, SqlInt64.Null);
    }

    [SqlFunction(Name = "binReverseRange", IsDeterministic = true)]
    public static SqlBytes ReverseRange(SqlBytes input, SqlInt64 index, SqlInt64 count)
    {
      if (input.IsNull)
        return SqlBytes.Null;
      if (!index.IsNull && index.Value > input.Length)
        throw new ArgumentOutOfRangeException("index");
      if (!count.IsNull && count.Value > input.Length - (index.IsNull ? 0 : index.Value))
        throw new ArgumentOutOfRangeException("count");

      long indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : input.Length - count.Value;
      long countValue = !count.IsNull ? count.Value : index.IsNull ? input.Length : input.Length - index.Value;
      SqlBytes result = CreateResult(countValue);
      input.Read(indexValue, result.Buffer, 0, (int)countValue);
      Array.Reverse(result.Buffer, 0, (int)countValue);
      return result;
    }

    [SqlFunction(Name = "binReplace", IsDeterministic = true)]
    public static SqlBytes Replace(SqlBytes input, SqlBytes value, SqlBytes replacement)
    {
      return ReplaceRange(input, value, replacement, SqlInt64.Null, SqlInt64.Null);
    }

    [SqlFunction(Name = "binReplaceRange", IsDeterministic = true)]
    public static SqlBytes ReplaceRange(SqlBytes input, SqlBytes value, SqlBytes replacement, SqlInt64 index, SqlInt64 count)
    {
      if (input.IsNull || value.IsNull || replacement.IsNull)
        return SqlBytes.Null;
      if (!index.IsNull && (index.Value < 0 || index > input.Length))
        throw new ArgumentOutOfRangeException("index");
      if (!count.IsNull && (count.Value < 0 || count > input.Length - (index.IsNull ? 0 : index.Value)))
        throw new ArgumentOutOfRangeException("count");

      long indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : input.Length - count.Value;
      long countValue = !count.IsNull ? count.Value : index.IsNull ? input.Length : input.Length - index.Value;
      List<byte> result = new List<byte>(Enumerate(input, 0L, indexValue));
      long position = indexValue;
      long found = -1L;
      if (input.Storage == StorageState.Stream)
        input.Stream.Position = position;
      while (position < indexValue + countValue)
      {
        found =
          input.Storage == StorageState.Buffer ? input.Buffer.SequenceFind((int)position, (int)(indexValue + countValue - position), (int)value.Length, false, (byte v, int i) => v == value[i]) :
          input.Storage == StorageState.Stream ? input.Stream.Find(indexValue + countValue - position, value.Length, (byte v, long i, long j) => value.Buffer[i] == (j >= 0 ? value.Buffer[j] : v)) :
          -1L;
        if (found < 0)
          break;
        else if (input.Storage == StorageState.Stream)
          found += position;
        if (found > position)
          result.AddRange(Enumerate(input, position, found - position));
        result.AddRange(Enumerate(replacement, 0, replacement.Length));
        position = found + value.Length;
        if (input.Storage == StorageState.Stream)
          input.Stream.Position = position;
      }
      if (position < input.Length)
        result.AddRange(Enumerate(input, position, input.Length - position));
      return new SqlBytes(result.ToArray());
    }

    #endregion
    #region Convert functions

    [SqlFunction(Name = "binToString", IsDeterministic = true)]
    public static SqlChars ToString(SqlBytes input, SqlInt64 index, SqlInt64 count)
    {
      if (input.IsNull)
        return SqlChars.Null;
      if (!index.IsNull && (index.Value < 0 || index > input.Length))
        throw new ArgumentOutOfRangeException("index");
      if (!count.IsNull && (count.Value < 0 || count > input.Length - (index.IsNull ? 0 : index.Value)))
        throw new ArgumentOutOfRangeException("count");

      long indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : input.Length - count.Value;
      long countValue = !count.IsNull ? count.Value : index.IsNull ? input.Length : input.Length - index.Value;
      return new SqlChars(SqlRuntime.TextEncoding.GetChars(input.Buffer, (int)indexValue, (int)countValue));
    }

    [SqlFunction(Name = "binToStringByCpId", IsDeterministic = true)]
    public static SqlChars ToStringByCpId(SqlBytes input, SqlInt64 index, SqlInt64 count, SqlInt32 cpId)
    {
      if (input.IsNull)
        return SqlChars.Null;
      if (!index.IsNull && (index.Value < 0 || index > input.Length))
        throw new ArgumentOutOfRangeException("index");
      if (!count.IsNull && (count.Value < 0 || count > input.Length - (index.IsNull ? 0 : index.Value)))
        throw new ArgumentOutOfRangeException("count");

      long indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : input.Length - count.Value;
      long countValue = !count.IsNull ? count.Value : index.IsNull ? input.Length : input.Length - index.Value;
      Encoding encoding = cpId.IsNull ? SqlRuntime.TextEncoding : Encoding.GetEncoding(cpId.Value);
      return new SqlChars(encoding.GetChars(input.Buffer, (int)indexValue, (int)countValue));
    }

    [SqlFunction(Name = "binToStringByCpName", IsDeterministic = true)]
    public static SqlChars ToStringByCpName(SqlBytes input, SqlInt64 index, SqlInt64 count, [SqlFacet(MaxSize = 128)] SqlString cpName)
    {
      if (input.IsNull)
        return SqlChars.Null;
      if (!index.IsNull && (index.Value < 0 || index > input.Length))
        throw new ArgumentOutOfRangeException("index");
      if (!count.IsNull && (count.Value < 0 || count > input.Length - (index.IsNull ? 0 : index.Value)))
        throw new ArgumentOutOfRangeException("count");

      long indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : input.Length - count.Value;
      long countValue = !count.IsNull ? count.Value : index.IsNull ? input.Length : input.Length - index.Value;
      Encoding encoding = cpName.IsNull ? SqlRuntime.TextEncoding : Encoding.GetEncoding(cpName.Value);
      return new SqlChars(encoding.GetChars(input.Buffer, (int)indexValue, (int)countValue));
    }

    [SqlFunction(Name = "binToBase64String", IsDeterministic = true)]
    public static SqlChars ToBase64String(SqlBytes input, SqlInt64 index, SqlInt64 count)
    {
      if (input.IsNull)
        return SqlChars.Null;
      if (!index.IsNull && (index.Value < 0 || index > input.Length))
        throw new ArgumentOutOfRangeException("index");
      if (!count.IsNull && (count.Value < 0 || count > input.Length - (index.IsNull ? 0 : index.Value)))
        throw new ArgumentOutOfRangeException("count");

      long indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : input.Length - count.Value;
      long countValue = !count.IsNull ? count.Value : index.IsNull ? input.Length : input.Length - index.Value;
      char[] buffer = new char[(input.Length / 3 + (input.Length % 3 != 0 ? 1 : 0)) * 4];
      Convert.ToBase64CharArray(input.Buffer, (int)indexValue, (int)countValue, buffer, 0, Base64FormattingOptions.None);
      return new SqlChars(buffer);
    }

    [SqlFunction(Name = "binToTinyInt", IsDeterministic = true)]
    public static SqlByte ToByte(SqlBytes input, SqlInt64 index)
    {
      if (input.IsNull || index.IsNull)
        return SqlByte.Null;
      if (input.Length < sizeof(Byte))
        throw new ArgumentException("Binary has too small size.", "input");
      if (index.Value < 0 || index.Value > input.Length - sizeof(Byte))
        throw new ArgumentOutOfRangeException("index");

      return new SqlByte(input[index.Value]);
    }

    [SqlFunction(Name = "binToSmallInt", IsDeterministic = true)]
    public static SqlInt16 ToInt16(SqlBytes input, SqlInt64 index)
    {
      if (input.IsNull || index.IsNull)
        return SqlInt16.Null;
      if (input.Length < sizeof(short))
        throw new ArgumentException("Binary has too small size.", "input");
      if (index.Value < 0 || index.Value > input.Length - sizeof(short))
        throw new ArgumentOutOfRangeException("index");

      byte[] buffer = new byte[sizeof(short)];
      input.Read(index.Value, buffer, 0, sizeof(short));
      return new SqlInt16(BitConverter.ToInt16(buffer, 0));
    }

    [SqlFunction(Name = "binToInt", IsDeterministic = true)]
    public static SqlInt32 ToInt32(SqlBytes input, SqlInt64 index)
    {
      if (input.IsNull || index.IsNull)
        return SqlInt32.Null;
      if (input.Length < sizeof(int))
        throw new ArgumentException("Binary has too small size.", "input");
      if (index.Value < 0 || index.Value > input.Length - sizeof(int))
        throw new ArgumentOutOfRangeException("index");

      byte[] buffer = new byte[sizeof(int)];
      input.Read(index.Value, buffer, 0, sizeof(int));
      return new SqlInt32(BitConverter.ToInt32(buffer, 0));
    }

    [SqlFunction(Name = "binToBigInt", IsDeterministic = true)]
    public static SqlInt64 ToInt64(SqlBytes input, SqlInt64 index)
    {
      if (input.IsNull || index.IsNull)
        return SqlInt64.Null;
      if (input.Length < sizeof(long))
        throw new ArgumentException("Binary has too small size.", "input");
      if (index.Value < 0 || index.Value > input.Length - sizeof(long))
        throw new ArgumentOutOfRangeException("index");

      byte[] buffer = new byte[sizeof(long)];
      input.Read(index.Value, buffer, 0, sizeof(long));
      return new SqlInt64(BitConverter.ToInt64(buffer, 0));
    }

    [SqlFunction(Name = "binToSingleFloat", IsDeterministic = true)]
    public static SqlSingle ToSingle(SqlBytes input, SqlInt64 index)
    {
      if (input.IsNull || index.IsNull)
        return SqlSingle.Null;
      if (input.Length < sizeof(float))
        throw new ArgumentException("Binary has too small size.", "input");
      if (index.Value < 0 || index.Value > input.Length - sizeof(float))
        throw new ArgumentOutOfRangeException("index");

      byte[] buffer = new byte[sizeof(float)];
      input.Read(index.Value, buffer, 0, sizeof(float));
      return new SqlSingle(BitConverter.ToSingle(buffer, 0));
    }

    [SqlFunction(Name = "binToDoubleFloat", IsDeterministic = true)]
    public static SqlDouble ToDouble(SqlBytes input, SqlInt64 index)
    {
      if (input.IsNull || index.IsNull)
        return SqlDouble.Null;
      if (input.Length < sizeof(double))
        throw new ArgumentException("Binary has too small size.", "input");
      if (index.Value < 0 || index.Value > input.Length - sizeof(double))
        throw new ArgumentOutOfRangeException("index");

      byte[] buffer = new byte[sizeof(double)];
      input.Read(index.Value, buffer, 0, sizeof(double));
      return new SqlDouble(BitConverter.ToDouble(buffer, 0));
    }

    [SqlFunction(Name = "binToDateTime", IsDeterministic = true)]
    public static SqlDateTime ToDateTime(SqlBytes input, SqlInt64 index)
    {
      if (input.IsNull || index.IsNull)
        return SqlDateTime.Null;
      if (input.Length < sizeof(long))
        throw new ArgumentException("Binary has too small size.", "input");
      if (index.Value < 0 || index.Value > input.Length - sizeof(long))
        throw new ArgumentOutOfRangeException("index");

      byte[] buffer = new byte[sizeof(long)];
      input.Read(index.Value, buffer, 0, sizeof(long));
      return new SqlDateTime(new DateTime(BitConverter.ToInt64(buffer, 0)));
    }

    [SqlFunction(Name = "binToUid", IsDeterministic = true)]
    public static SqlGuid ToGuid(SqlBytes input, SqlInt64 index)
    {
      if (input.IsNull || index.IsNull)
        return SqlGuid.Null;
      if (input.Length < SizeOfGuid)
        throw new ArgumentException("Binary has too small size.", "input");
      if (index.Value < 0 || index.Value > input.Length - SizeOfGuid)
        throw new ArgumentOutOfRangeException("index");

      byte[] buffer = new byte[SizeOfGuid];
      input.Read(index.Value, buffer, 0, SizeOfGuid);
      return new SqlGuid(buffer);
    }

    [SqlFunction(Name = "binFromString", IsDeterministic = true)]
    public static SqlBytes FromString([SqlFacet(MaxSize = -1)] SqlChars input)
    {
      return !input.IsNull ? new SqlBytes(SqlRuntime.TextEncoding.GetBytes(input.Buffer, 0, (int)input.Length)) : SqlBytes.Null;
    }

    [SqlFunction(Name = "binFromStringByCpId", IsDeterministic = true)]
    public static SqlBytes FromStringByCpId([SqlFacet(MaxSize = -1)] SqlChars input, SqlInt32 cpId)
    {
      return !input.IsNull ? new SqlBytes((!cpId.IsNull ? Encoding.GetEncoding(cpId.Value) : SqlRuntime.TextEncoding).GetBytes(input.Buffer, 0, (int)input.Length)) : SqlBytes.Null;
    }

    [SqlFunction(Name = "binFromStringByCpName", IsDeterministic = true)]
    public static SqlBytes FromStringByCpName([SqlFacet(MaxSize = -1)] SqlChars input, [SqlFacet(MaxSize = 128)] SqlString cpName)
    {
      return !input.IsNull ? new SqlBytes((!cpName.IsNull ? Encoding.GetEncoding(cpName.Value) : SqlRuntime.TextEncoding).GetBytes(input.Buffer, 0, (int)input.Length)) : SqlBytes.Null;
    }

    [SqlFunction(Name = "binFromBase64String", IsDeterministic = true)]
    public static SqlBytes FromBase64String([SqlFacet(MaxSize = -1)] SqlChars input)
    {
      return !input.IsNull ? new SqlBytes(Convert.FromBase64CharArray(input.Buffer, 0, (int)input.Length)) : SqlBytes.Null;
    }

    [SqlFunction(Name = "binFromTinyInt", IsDeterministic = true)]
    public static SqlBytes FromByte(SqlByte input)
    {
      return !input.IsNull ? new SqlBytes(new byte[] { input.Value }) : SqlBytes.Null;
    }

    [SqlFunction(Name = "binFromSmallInt", IsDeterministic = true)]
    public static SqlBytes FromInt16(SqlInt16 input)
    {
      return !input.IsNull ? new SqlBytes(BitConverter.GetBytes(input.Value)) : SqlBytes.Null;
    }

    [SqlFunction(Name = "binFromInt", IsDeterministic = true)]
    public static SqlBytes FromInt32(SqlInt32 input)
    {
      return !input.IsNull ? new SqlBytes(BitConverter.GetBytes(input.Value)) : SqlBytes.Null;
    }

    [SqlFunction(Name = "binFromBigInt", IsDeterministic = true)]
    public static SqlBytes FromInt64(SqlInt64 input)
    {
      return !input.IsNull ? new SqlBytes(BitConverter.GetBytes(input.Value)) : SqlBytes.Null;
    }

    [SqlFunction(Name = "binFromSingleFloat", IsDeterministic = true)]
    public static SqlBytes FromSingle(SqlSingle input)
    {
      return !input.IsNull ? new SqlBytes(BitConverter.GetBytes(input.Value)) : SqlBytes.Null;
    }

    [SqlFunction(Name = "binFromDoubleFloat", IsDeterministic = true)]
    public static SqlBytes FromDouble(SqlDouble input)
    {
      return !input.IsNull ? new SqlBytes(BitConverter.GetBytes(input.Value)) : SqlBytes.Null;
    }

    [SqlFunction(Name = "binFromDateTime", IsDeterministic = true)]
    public static SqlBytes FromDateTime(SqlDateTime input)
    {
      return !input.IsNull ? new SqlBytes(BitConverter.GetBytes(input.Value.Ticks)) : SqlBytes.Null;
    }

    [SqlFunction(Name = "binFromUid", IsDeterministic = true)]
    public static SqlBytes FromGuid(SqlGuid input)
    {
      return !input.IsNull ? new SqlBytes(input.Value.ToByteArray()) : SqlBytes.Null;
    }

    #endregion
    #region Retrieve functions

    [SqlFunction(Name = "binLength", IsDeterministic = true)]
    public static SqlInt64 Length(SqlBytes input)
    {
      return input.IsNull ? SqlInt64.Null : new SqlInt64(input.Length);
    }

    [SqlFunction(Name = "binIndexOf", IsDeterministic = true)]
    public static SqlInt64 IndexOf(SqlBytes input, SqlBytes value)
    {
      return IndexOfRange(input, value, SqlInt64.Null, SqlInt64.Null);
    }

    [SqlFunction(Name = "binIndexOfRange", IsDeterministic = true)]
    public static SqlInt64 IndexOfRange(SqlBytes input, SqlBytes value, SqlInt64 index, SqlInt64 count)
    {
      if (input.IsNull || value.IsNull)
        return SqlInt64.Null;
      if (!index.IsNull && (index.Value < 0 || index > input.Length))
        throw new ArgumentOutOfRangeException("index");
      if (!count.IsNull && (count.Value < 0 || count > input.Length - (index.IsNull ? 0 : index.Value)))
        throw new ArgumentOutOfRangeException("count");
      if (input.Length == 0L || value.Length > input.Length)
        return new SqlInt64(-1L);

      long indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : input.Length - count.Value;
      long countValue = !count.IsNull ? count.Value : index.IsNull ? input.Length : input.Length - index.Value;
      switch (input.Storage)
      {
        case StorageState.Buffer:
          return new SqlInt64(input.Buffer.SequenceFind((int)indexValue, (int)countValue, (int)value.Length, false, (byte v, int i) => v == value.Buffer[i]));
        case StorageState.Stream:
          Stream stream = input.Stream;
          stream.Position = indexValue;
          long position = input.Stream.Find(countValue, (int)value.Length, (byte v, int i, int j) => value.Buffer[i] == (j >= 0 ? value.Buffer[j] : v));
          if (position >= 0)
            position += indexValue;
          return new SqlInt64(position);
        default:
          throw new NotSupportedException("Unsupported input storage type.");
      }
    }

    [SqlFunction(Name = "binLastIndexOf", IsDeterministic = true)]
    public static SqlInt64 LastIndexOf(SqlBytes input, SqlBytes value, SqlInt64 index, SqlInt64 count)
    {
      return LastIndexOfRange(input, value, index, count);
    }

    [SqlFunction(Name = "binLastIndexOfRange", IsDeterministic = true)]
    public static SqlInt64 LastIndexOfRange(SqlBytes input, SqlBytes value, SqlInt64 index, SqlInt64 count)
    {
      if (input.IsNull || value.IsNull)
        return SqlInt64.Null;
      if (!index.IsNull && (index.Value < 0L || index > input.Length))
        throw new ArgumentOutOfRangeException("index");
      if (!count.IsNull && (count.Value < 0L || count > input.Length - (index.IsNull ? 0L : index.Value)))
        throw new ArgumentOutOfRangeException("count");
      if (input.Length == 0L || value.Length > input.Length)
        return new SqlInt64(-1L);

      long indexValue = index.IsNull ? (count.IsNull ? input.Length : count.Value) - 1L : index.Value;
      long countValue = count.IsNull ? (index.IsNull ? input.Length : index.Value + 1L) : count.Value;
      switch (input.Storage)
      {
        case StorageState.Buffer:
          return new SqlInt64(input.Buffer.SequenceFindLast((int)indexValue, (int)countValue, (int)value.Length, false, (byte v, int i) => v == value.Buffer[i]));
        case StorageState.Stream:
          Stream stream = input.Stream;
          stream.Position = indexValue;
          long position = StreamExtension.FindLast(input.Stream, countValue, (int)value.Length, (byte v, int i, int j) => value.Buffer[(int)value.Length - 1 - i] == (j >= 0 ? value.Buffer[(int)value.Length - 1 - j] : v));
          if (position >= 0L)
            position = indexValue - position;
          return new SqlInt64(position);
        default:
          throw new NotSupportedException("Unsupported input storage type.");
      }
    }

    #endregion
    #region Comparison functions

    [SqlFunction(Name = "binCompare", IsDeterministic = true)]
    public static SqlInt32 Compare(SqlBytes xData, SqlBytes yData)
    {
      if (xData.IsNull || yData.IsNull)
        return SqlInt32.Null;

      return xData.Buffer.SequenceCompare(0, yData.Buffer, 0, int.MaxValue);
    }

    [SqlFunction(Name = "binCompareRange", IsDeterministic = true)]
    public static SqlInt32 CompareRange(SqlBytes xData, SqlInt64 xIndex, SqlBytes yData, SqlInt64 yIndex, SqlInt64 count)
    {
      if (xData.IsNull || yData.IsNull)
        return SqlInt32.Null;

      long xIndexValue = !xIndex.IsNull ? xIndex.Value : count.IsNull ? 0L : (xData.Length - Comparable.Min(count.Value, xData.Length));
      long yIndexValue = !yIndex.IsNull ? yIndex.Value : count.IsNull ? 0L : (yData.Length - Comparable.Min(count.Value, yData.Length));
      long countValue = !count.IsNull ? count.Value : int.MaxValue;
      return xData.Buffer.SequenceCompare((int)xIndexValue, yData.Buffer, (int)yIndexValue, (int)countValue);
    }

    [SqlFunction(Name = "binEqual", IsDeterministic = true)]
    public static SqlBoolean Equal(SqlBytes xData, SqlBytes yData)
    {
      if (xData.IsNull || yData.IsNull)
        return SqlBoolean.Null;

      return xData.Buffer.SequenceEqual(0, yData.Buffer, 0, int.MaxValue);
    }

    [SqlFunction(Name = "binEqualRange", IsDeterministic = true)]
    public static SqlBoolean EqualRange(SqlBytes xData, SqlInt64 xIndex, SqlBytes yData, SqlInt64 yIndex, SqlInt64 count)
    {
      if (xData.IsNull || yData.IsNull)
        return SqlBoolean.Null;

      long xIndexValue = !xIndex.IsNull ? xIndex.Value : count.IsNull ? 0 : xData.Length - Comparable.Min(count.Value, xData.Length);
      long yIndexValue = !yIndex.IsNull ? yIndex.Value : count.IsNull ? 0 : yData.Length - Comparable.Min(count.Value, yData.Length);
      long countValue = !count.IsNull ? count.Value : int.MaxValue;
      return xData.Buffer.SequenceEqual((int)xIndexValue, yData.Buffer, (int)yIndexValue, (int)countValue);
    }

    #endregion
    #endregion
    #region Table-valued function methods

    [SqlFunction(Name = "binSplitToString", IsDeterministic = true, FillRowMethodName = "SplitToStringFillRow")]
		public static IEnumerable SplitToString(SqlBytes input)
		{
			if (input.IsNull)
				yield break;

      using (BinaryReader rd = new BinaryReader(input.Stream))
      {
        Encoding encoding;
        switch (rd.ReadByte())
        {
          case 0:
            yield break;
          case 1:
            encoding = SqlRuntime.TextEncoding;
            break;
          case 2:
            encoding = Encoding.GetEncoding(rd.ReadUInt16());
            break;
          default:
            throw new InvalidDataException();
        }
        for (int count = rd.ReadInt32(); count > 0; count--)
          switch (rd.ReadByte())
          {
            case 0:
              yield return null;
              break;
            case 1:
              yield return rd.ReadString(encoding, SizeEncoding.VB);
              break;
            case 2:
              yield return rd.ReadString(Encoding.GetEncoding(rd.ReadUInt16()), SizeEncoding.VB);
              break;
            default:
              throw new InvalidDataException();
          }
      }
		}

    [SqlFunction(Name = "binSplitToBinary", IsDeterministic = true, FillRowMethodName = "SplitToBinaryFillRow")]
    public static IEnumerable SplitToBinary(SqlBytes input)
    {
      if (input.IsNull)
        yield break;

      using (BinaryReader rd = new BinaryReader(input.Stream))
        if (!rd.ReadBoolean())
          yield break;
        else
          foreach (var item in rd.ReadCollection(tr => tr.ReadObject(vr => vr.ReadBytes(tr.ReadInt32())), rd.ReadInt32()))
            yield return item;
    }

		[SqlFunction(Name = "binSplitToBit", IsDeterministic = true, FillRowMethodName = "SplitToBitFillRow")]
		public static IEnumerable SplitToBit(SqlBytes input)
		{
			if (input.IsNull)
				yield break;

      using (BinaryReader rd = new BinaryReader(input.Stream))
        if (!rd.ReadBoolean())
          yield break;
        else
          foreach (var item in rd.ReadCollection(tr =>
            {
              switch (tr.ReadByte())
              {
                case 0:
                  return default(Boolean?);
                case 1:
                  return false;
                case 2:
                  return true;
                default:
                  throw new InvalidDataException();
              }
            }, rd.ReadInt32()))
            yield return item;
		}

		[SqlFunction(Name = "binSplitToTinyInt", IsDeterministic = true, FillRowMethodName = "SplitToByteFillRow")]
		public static IEnumerable SplitToByte(SqlBytes input)
		{
			if (input.IsNull)
				yield break;

      using (BinaryReader rd = new BinaryReader(input.Stream))
        if (!rd.ReadBoolean())
          yield break;
        else
          foreach (var item in rd.ReadCollection(tr => tr.ReadNullable(vr => vr.ReadByte()), rd.ReadInt32()))
            yield return item;
		}

		[SqlFunction(Name = "binSplitToSmallInt", IsDeterministic = true, FillRowMethodName = "SplitToInt16FillRow")]
		public static IEnumerable SplitToInt16(SqlBytes input)
		{
      if (input.IsNull)
        yield break;

      using (BinaryReader rd = new BinaryReader(input.Stream))
        if (!rd.ReadBoolean())
          yield break;
        else
          foreach (var item in rd.ReadCollection(tr => tr.ReadNullable(vr => vr.ReadInt16()), rd.ReadInt32()))
            yield return item;
    }

    [SqlFunction(Name = "binSplitToInt", IsDeterministic = true, FillRowMethodName = "SplitToInt32FillRow")]
		public static IEnumerable SplitToInt32(SqlBytes input)
		{
      if (input.IsNull)
        yield break;

      using (BinaryReader rd = new BinaryReader(input.Stream))
        if (!rd.ReadBoolean())
          yield break;
        else
          foreach (var item in rd.ReadCollection(tr => tr.ReadNullable(vr => vr.ReadInt32()), rd.ReadInt32()))
            yield return item;
    }

    [SqlFunction(Name = "binSplitToBigInt", IsDeterministic = true, FillRowMethodName = "SplitToInt64FillRow")]
		public static IEnumerable SplitToInt64(SqlBytes input)
		{
      if (input.IsNull)
        yield break;

      using (BinaryReader rd = new BinaryReader(input.Stream))
        if (!rd.ReadBoolean())
          yield break;
        else
          foreach (var item in rd.ReadCollection(tr => tr.ReadNullable(vr => vr.ReadInt64()), rd.ReadInt32()))
            yield return item;
    }

    [SqlFunction(Name = "binSplitToSingle", IsDeterministic = true, FillRowMethodName = "SplitToSingleFillRow")]
		public static IEnumerable SplitToSingle(SqlBytes input)
		{
      if (input.IsNull)
        yield break;

      using (BinaryReader rd = new BinaryReader(input.Stream))
        if (!rd.ReadBoolean())
          yield break;
        else
          foreach (var item in rd.ReadCollection(tr => tr.ReadNullable(vr => vr.ReadSingle()), rd.ReadInt32()))
            yield return item;
    }

    [SqlFunction(Name = "binSplitToDouble", IsDeterministic = true, FillRowMethodName = "SplitToDoubleFillRow")]
		public static IEnumerable SplitToDouble(SqlBytes input)
		{
      if (input.IsNull)
        yield break;

      using (BinaryReader rd = new BinaryReader(input.Stream))
        if (!rd.ReadBoolean())
          yield break;
        else
          foreach (var item in rd.ReadCollection(tr => tr.ReadNullable(vr => vr.ReadDouble()), rd.ReadInt32()))
            yield return item;
    }

    [SqlFunction(Name = "binSplitToDateTime", IsDeterministic = true, FillRowMethodName = "SplitToDateTimeFillRow")]
		public static IEnumerable SplitToDateTime(SqlBytes input)
		{
      if (input.IsNull)
        yield break;

      using (BinaryReader rd = new BinaryReader(input.Stream))
        if (!rd.ReadBoolean())
          yield break;
        else
          foreach (var item in rd.ReadCollection(tr => tr.ReadNullable(vr => vr.ReadDateTime()), rd.ReadInt32()))
            yield return item;
    }

    [SqlFunction(Name = "binSplitToUid", IsDeterministic = true, FillRowMethodName = "SplitToGuidFillRow")]
		public static IEnumerable SplitToGuid(SqlBytes input)
		{
      if (input.IsNull)
        yield break;

      using (BinaryReader rd = new BinaryReader(input.Stream))
        if (!rd.ReadBoolean())
          yield break;
        else
          foreach (var item in rd.ReadCollection(tr => tr.ReadNullable(vr => vr.ReadGuid()), rd.ReadInt32()))
            yield return item;
    }

    #endregion
    #region FillRow methods

    private static void SplitToBinaryFillRow(object obj, out SqlBytes Value)
    {
      Value = obj == null ? SqlBytes.Null : new SqlBytes((byte[])obj);
    }

		private static void SplitToStringFillRow(object obj,  out SqlChars Value)
		{
			Value = obj == null ? SqlChars.Null : new SqlChars(((String)obj).ToCharArray());
		}

		private static void SplitToBitFillRow(object obj, out SqlBoolean Value)
		{
			Value = default(Boolean?).Equals(obj) ? SqlBoolean.Null : (Boolean)obj;
		}

		private static void SplitToByteFillRow(object obj, out SqlByte Value)
		{
			Value = default(Byte?).Equals(obj) ? SqlByte.Null : (Byte)obj;
		}

		private static void SplitToInt16FillRow(object obj, out SqlInt16 Value)
		{
			Value = default(Int16?).Equals(obj) ? SqlInt16.Null : (Int16)obj;
		}

		private static void SplitToInt32FillRow(object obj, out SqlInt32 Value)
		{
			Value = default(Int32?).Equals(obj) ? SqlInt32.Null : (Int32)obj;
		}

		private static void SplitToInt64FillRow(object obj, out SqlInt64 Value)
		{
			Value = default(Int64?).Equals(obj) ? SqlInt64.Null : (Int64)obj;
		}

		private static void SplitToSingleFillRow(object obj, out SqlSingle Value)
		{
			Value = default(Single?).Equals(obj) ? SqlSingle.Null : (Single)obj;
		}

		private static void SplitToDoubleFillRow(object obj, out SqlDouble Value)
		{
			Value = default(Double?).Equals(obj) ? SqlDouble.Null : (Double)obj;
		}

		private static void SplitToDateTimeFillRow(object obj, out SqlDateTime Value)
		{
			Value = default(DateTime?).Equals(obj) ? SqlDateTime.Null : (DateTime)obj;
		}

		private static void SplitToGuidFillRow(object obj, out SqlGuid Value)
		{
			Value = default(Guid?).Equals(obj) ? SqlGuid.Null : (Guid)obj;
		}

    #endregion
    #region Internal methods

    private static SqlBytes CreateResult(long size)
    {
      if (size > SqlRuntime.MaxBufferSize)
        throw new InvalidOperationException("Exceeded maximum allowable size.");

      return new SqlBytes(new byte[size]);
    }

    private static IEnumerable<byte> Enumerate(SqlBytes input, long offset, long count)
    {
      switch (input.Storage)
      {
        case StorageState.Buffer:
          return input.Buffer.Enumerate((int)offset, (int)count);
        case StorageState.Stream:
          return input.Stream.Locate(offset).ReadBytes().Take(count);
        default:
          throw new NotSupportedException("Unsupported input storage type.");
      }
    }

    private static void Copy(SqlBytes source, long srcOffset, SqlBytes destination, long dstOffset, long length)
    {
      if (length == 0)
        return;
      byte[] buffer = new byte[Comparable.Min(SqlRuntime.IoBufferSize, length)];
      long read = 0;
      while (length > 0 && (read = source.Read(srcOffset, buffer, 0, (int)Comparable.Min(length, buffer.Length))) > 0L)
      {
        destination.Write(dstOffset, buffer, 0, (int)read);
        length -= read;
        srcOffset += read;
        dstOffset += read;
      }
    }

    #endregion
  }
}
