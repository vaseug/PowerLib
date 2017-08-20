using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Numerics;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.Linq;
using PowerLib.System.Numerics;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.SqlTypes;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.SqlServer.Collections
{
  public static class SqlArrayFunctions
  {
    #region SqlBoolean array methods

    [SqlFunction(Name = "bArrayCreate", IsDeterministic = true)]
    public static SqlBytes BooleanArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing)
    {
      if (length.IsNull || countSizing.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulBooleanStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, default(Boolean?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "bArrayParse", IsDeterministic = true)]
    public static SqlBytes BooleanArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing)
    {
      if (str.IsNull || countSizing.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Boolean?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlBoolean.Parse(t).Value : default(Boolean?));
      using (new NulBooleanStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "bArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString BooleanArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulBooleanStreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlBoolean(t.Value) : SqlBoolean.Null).ToString());
    }

    [SqlFunction(Name = "bArrayLength", IsDeterministic = true)]
    public static SqlInt32 BooleanArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulBooleanStreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "bArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 BooleanArrayIndexOf(SqlBytes array, SqlBoolean value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulBooleanStreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Boolean?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "bArrayGet", IsDeterministic = true)]
    public static SqlBoolean BooleanArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlBoolean.Null;

      using (var sa = new NulBooleanStreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlBoolean.Null : v.Value;
      }
    }

    [SqlFunction(Name = "bArraySet", IsDeterministic = true)]
    public static SqlBytes BooleanArraySet(SqlBytes array, SqlInt32 index, SqlBoolean value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulBooleanStreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Boolean?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "bArrayGetRange", IsDeterministic = true)]
    public static SqlBytes BooleanArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulBooleanStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulBooleanStreamedArray(result.Stream, sa.CountSizing, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "bArraySetRange", IsDeterministic = true)]
    public static SqlBytes BooleanArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulBooleanStreamedArray(array.Stream, true, false))
      using (var ra = new NulBooleanStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "bArrayFillRange", IsDeterministic = true)]
    public static SqlBytes BooleanArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlBoolean value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulBooleanStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Boolean?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "bArrayToCollection", IsDeterministic = true)]
    public static SqlBytes BooleanArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulBooleanStreamedArray(array.Stream, true, false))
      using (new NulBooleanStreamedCollection(coll.Stream, sa.CountSizing, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "bArrayEnumerate", IsDeterministic = true, FillRowMethodName = "BooleanArrayFillRow")]
    public static IEnumerable BooleanArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulBooleanStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void BooleanArrayFillRow(object obj, out SqlInt32 Index, out SqlBoolean Value)
    {
      Tuple<int, Boolean?> tuple = (Tuple<int, Boolean?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlBoolean.Null;
    }

    #endregion
    #region SqlByte array methods

    [SqlFunction(Name = "tiArrayCreate", IsDeterministic = true)]
    public static SqlBytes ByteArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulByteStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(Byte?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "tiArrayParse", IsDeterministic = true)]
    public static SqlBytes ByteArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Byte?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlByte.Parse(t).Value : default(Byte?));
      using (new NulByteStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "tiArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ByteArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulByteStreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlByte(t.Value) : SqlByte.Null).ToString());
    }

    [SqlFunction(Name = "tiArrayLength", IsDeterministic = true)]
    public static SqlInt32 ByteArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulByteStreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "tiArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 ByteArrayIndexOf(SqlBytes array, SqlByte value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulByteStreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Byte?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "tiArrayGet", IsDeterministic = true)]
    public static SqlByte ByteArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlByte.Null;

      using (var sa = new NulByteStreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlByte.Null : v.Value;
      }
    }

    [SqlFunction(Name = "tiArraySet", IsDeterministic = true)]
    public static SqlBytes ByteArraySet(SqlBytes array, SqlInt32 index, SqlByte value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulByteStreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Byte?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "tiArrayGetRange", IsDeterministic = true)]
    public static SqlBytes ByteArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulByteStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulByteStreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "tiArraySetRange", IsDeterministic = true)]
    public static SqlBytes ByteArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulByteStreamedArray(array.Stream, true, false))
      using (var ra = new NulByteStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "tiArrayFillRange", IsDeterministic = true)]
    public static SqlBytes ByteArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlByte value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulByteStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Byte?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "tiArrayToCollection", IsDeterministic = true)]
    public static SqlBytes ByteArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulByteStreamedArray(array.Stream, true, false))
      using (new NulByteStreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "tiArrayEnumerate", IsDeterministic = true, FillRowMethodName = "ByteArrayFillRow")]
    public static IEnumerable ByteArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulByteStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void ByteArrayFillRow(object obj, out SqlInt32 Index, out SqlByte Value)
    {
      Tuple<int, Byte?> tuple = (Tuple<int, Byte?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlByte.Null;
    }

    #endregion
    #region SqlInt16 array methods

    [SqlFunction(Name = "siArrayCreate", IsDeterministic = true)]
    public static SqlBytes Int16ArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulInt16StreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(Int16?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "siArrayParse", IsDeterministic = true)]
    public static SqlBytes Int16ArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Int16?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt16.Parse(t).Value : default(Int16?));
      using (new NulInt16StreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "siArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Int16ArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulInt16StreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlInt16(t.Value) : SqlInt16.Null).ToString());
    }

    [SqlFunction(Name = "siArrayLength", IsDeterministic = true)]
    public static SqlInt32 Int16ArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt16StreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "siArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 Int16ArrayIndexOf(SqlBytes array, SqlInt16 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt16StreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Int16?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "siArrayGet", IsDeterministic = true)]
    public static SqlInt16 Int16ArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt16.Null;

      using (var sa = new NulInt16StreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlInt16.Null : v.Value;
      }
    }

    [SqlFunction(Name = "siArraySet", IsDeterministic = true)]
    public static SqlBytes Int16ArraySet(SqlBytes array, SqlInt32 index, SqlInt16 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulInt16StreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Int16?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "siArrayGetRange", IsDeterministic = true)]
    public static SqlBytes Int16ArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulInt16StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulInt16StreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "siArraySetRange", IsDeterministic = true)]
    public static SqlBytes Int16ArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulInt16StreamedArray(array.Stream, true, false))
      using (var ra = new NulInt16StreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "siArrayFillRange", IsDeterministic = true)]
    public static SqlBytes Int16ArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlInt16 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulInt16StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Int16?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "siArrayToCollection", IsDeterministic = true)]
    public static SqlBytes Int16ArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulInt16StreamedArray(array.Stream, true, false))
      using (new NulInt16StreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "siArrayEnumerate", IsDeterministic = true, FillRowMethodName = "Int16ArrayFillRow")]
    public static IEnumerable Int16ArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulInt16StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void Int16ArrayFillRow(object obj, out SqlInt32 Index, out SqlInt16 Value)
    {
      Tuple<int, Int16?> tuple = (Tuple<int, Int16?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlInt16.Null;
    }

    #endregion
    #region SqlInt32 array methods

    [SqlFunction(Name = "iArrayCreate", IsDeterministic = true)]
    public static SqlBytes Int32ArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(Int32?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "iArrayParse", IsDeterministic = true)]
    public static SqlBytes Int32ArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Int32?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt32.Parse(t).Value : default(Int32?));
      using (new NulInt32StreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "iArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Int32ArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlInt32(t.Value) : SqlInt32.Null).ToString());
    }

    [SqlFunction(Name = "iArrayLength", IsDeterministic = true)]
    public static SqlInt32 Int32ArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "iArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 Int32ArrayIndexOf(SqlBytes array, SqlInt32 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Int32?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "iArrayGet", IsDeterministic = true)]
    public static SqlInt32 Int32ArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlInt32.Null : v.Value;
      }
    }

    [SqlFunction(Name = "iArraySet", IsDeterministic = true)]
    public static SqlBytes Int32ArraySet(SqlBytes array, SqlInt32 index, SqlInt32 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Int32?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "iArrayGetRange", IsDeterministic = true)]
    public static SqlBytes Int32ArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulInt32StreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "iArraySetRange", IsDeterministic = true)]
    public static SqlBytes Int32ArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      using (var ra = new NulInt32StreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "iArrayFillRange", IsDeterministic = true)]
    public static SqlBytes Int32ArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlInt32 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "iArrayToCollection", IsDeterministic = true)]
    public static SqlBytes Int32ArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      using (new NulInt32StreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "iArrayEnumerate", IsDeterministic = true, FillRowMethodName = "Int32ArrayFillRow")]
    public static IEnumerable Int32ArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void Int32ArrayFillRow(object obj, out SqlInt32 Index, out SqlInt32 Value)
    {
      Tuple<int, Int32?> tuple = (Tuple<int, Int32?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlInt32.Null;
    }

    #endregion
    #region SqlInt64 array methods

    [SqlFunction(Name = "biArrayCreate", IsDeterministic = true)]
    public static SqlBytes Int64ArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulInt64StreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(Int64?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "biArrayParse", IsDeterministic = true)]
    public static SqlBytes Int64ArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Int64?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt64.Parse(t).Value : default(Int64?));
      using (new NulInt64StreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "biArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Int64ArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulInt64StreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlInt64(t.Value) : SqlInt64.Null).ToString());
    }

    [SqlFunction(Name = "biArrayLength", IsDeterministic = true)]
    public static SqlInt32 Int64ArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt64StreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "biArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 Int64ArrayIndexOf(SqlBytes array, SqlInt64 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt64StreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Int64?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "biArrayGet", IsDeterministic = true)]
    public static SqlInt64 Int64ArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt64.Null;

      using (var sa = new NulInt64StreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlInt64.Null : v.Value;
      }
    }

    [SqlFunction(Name = "biArraySet", IsDeterministic = true)]
    public static SqlBytes Int64ArraySet(SqlBytes array, SqlInt32 index, SqlInt64 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulInt64StreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Int64?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "biArrayGetRange", IsDeterministic = true)]
    public static SqlBytes Int64ArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulInt64StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulInt64StreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "biArraySetRange", IsDeterministic = true)]
    public static SqlBytes Int64ArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulInt64StreamedArray(array.Stream, true, false))
      using (var ra = new NulInt64StreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "biArrayFillRange", IsDeterministic = true)]
    public static SqlBytes Int64ArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlInt64 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulInt64StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Int64?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "biArrayToCollection", IsDeterministic = true)]
    public static SqlBytes Int64ArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulInt64StreamedArray(array.Stream, true, false))
      using (new NulInt64StreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "biArrayEnumerate", IsDeterministic = true, FillRowMethodName = "Int64ArrayFillRow")]
    public static IEnumerable Int64ArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulInt64StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void Int64ArrayFillRow(object obj, out SqlInt32 Index, out SqlInt64 Value)
    {
      Tuple<int, Int64?> tuple = (Tuple<int, Int64?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlInt64.Null;
    }

    #endregion
    #region SqlSingle array methods

    [SqlFunction(Name = "sfArrayCreate", IsDeterministic = true)]
    public static SqlBytes SingleArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulSingleStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(Single?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "sfArrayParse", IsDeterministic = true)]
    public static SqlBytes SingleArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Single?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlSingle.Parse(t).Value : default(Single?));
      using (new NulSingleStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "sfArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString SingleArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulSingleStreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlSingle(t.Value) : SqlSingle.Null).ToString());
    }

    [SqlFunction(Name = "sfArrayLength", IsDeterministic = true)]
    public static SqlInt32 SingleArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulSingleStreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "sfArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 SingleArrayIndexOf(SqlBytes array, SqlSingle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulSingleStreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Single?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "sfArrayGet", IsDeterministic = true)]
    public static SqlSingle SingleArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlSingle.Null;

      using (var sa = new NulSingleStreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlSingle.Null : v.Value;
      }
    }

    [SqlFunction(Name = "sfArraySet", IsDeterministic = true)]
    public static SqlBytes SingleArraySet(SqlBytes array, SqlInt32 index, SqlSingle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulSingleStreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Single?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "sfArrayGetRange", IsDeterministic = true)]
    public static SqlBytes SingleArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulSingleStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulSingleStreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "sfArraySetRange", IsDeterministic = true)]
    public static SqlBytes SingleArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulSingleStreamedArray(array.Stream, true, false))
      using (var ra = new NulSingleStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "sfArrayFillRange", IsDeterministic = true)]
    public static SqlBytes SingleArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlSingle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulSingleStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Single?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "sfArrayToCollection", IsDeterministic = true)]
    public static SqlBytes SingleArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulSingleStreamedArray(array.Stream, true, false))
      using (new NulSingleStreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "sfArrayEnumerate", IsDeterministic = true, FillRowMethodName = "SingleArrayFillRow")]
    public static IEnumerable SingleArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulSingleStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void SingleArrayFillRow(object obj, out SqlInt32 Index, out SqlSingle Value)
    {
      Tuple<int, Single?> tuple = (Tuple<int, Single?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlSingle.Null;
    }

    #endregion
    #region SqlDouble array methods

    [SqlFunction(Name = "dfArrayCreate", IsDeterministic = true)]
    public static SqlBytes DoubleArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulDoubleStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(Double?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "dfArrayParse", IsDeterministic = true)]
    public static SqlBytes DoubleArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Double?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlDouble.Parse(t).Value : default(Double?));
      using (new NulDoubleStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "dfArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString DoubleArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulDoubleStreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlDouble(t.Value) : SqlDouble.Null).ToString());
    }

    [SqlFunction(Name = "dfArrayLength", IsDeterministic = true)]
    public static SqlInt32 DoubleArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulDoubleStreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "dfArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 DoubleArrayIndexOf(SqlBytes array, SqlDouble value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulDoubleStreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Double?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "dfArrayGet", IsDeterministic = true)]
    public static SqlDouble DoubleArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlDouble.Null;

      using (var sa = new NulDoubleStreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlDouble.Null : v.Value;
      }
    }

    [SqlFunction(Name = "dfArraySet", IsDeterministic = true)]
    public static SqlBytes DoubleArraySet(SqlBytes array, SqlInt32 index, SqlDouble value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulDoubleStreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Double?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "dfArrayGetRange", IsDeterministic = true)]
    public static SqlBytes DoubleArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulDoubleStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulDoubleStreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "dfArraySetRange", IsDeterministic = true)]
    public static SqlBytes DoubleArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulDoubleStreamedArray(array.Stream, true, false))
      using (var ra = new NulDoubleStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "dfArrayFillRange", IsDeterministic = true)]
    public static SqlBytes DoubleArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlDouble value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulDoubleStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Double?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "dfArrayToCollection", IsDeterministic = true)]
    public static SqlBytes DoubleArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulDoubleStreamedArray(array.Stream, true, false))
      using (new NulDoubleStreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "dfArrayEnumerate", IsDeterministic = true, FillRowMethodName = "DoubleArrayFillRow")]
    public static IEnumerable DoubleArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulDoubleStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void DoubleArrayFillRow(object obj, out SqlInt32 Index, out SqlDouble Value)
    {
      Tuple<int, Double?> tuple = (Tuple<int, Double?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlDouble.Null;
    }

    #endregion
    #region SqlDateTime array methods

    [SqlFunction(Name = "dtArrayCreate", IsDeterministic = true)]
    public static SqlBytes DateTimeArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulDateTimeStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(DateTime?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "dtArrayParse", IsDeterministic = true)]
    public static SqlBytes DateTimeArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<DateTime?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlDateTime.Parse(t).Value : default(DateTime?));
      using (new NulDateTimeStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "dtArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString DateTimeArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulDateTimeStreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlDateTime(t.Value) : SqlDateTime.Null).ToString());
    }

    [SqlFunction(Name = "dtArrayLength", IsDeterministic = true)]
    public static SqlInt32 DateTimeArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulDateTimeStreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "dtArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 DateTimeArrayIndexOf(SqlBytes array, SqlDateTime value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulDateTimeStreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(DateTime?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "dtArrayGet", IsDeterministic = true)]
    public static SqlDateTime DateTimeArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlDateTime.Null;

      using (var sa = new NulDateTimeStreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlDateTime.Null : v.Value;
      }
    }

    [SqlFunction(Name = "dtArraySet", IsDeterministic = true)]
    public static SqlBytes DateTimeArraySet(SqlBytes array, SqlInt32 index, SqlDateTime value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulDateTimeStreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(DateTime?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "dtArrayGetRange", IsDeterministic = true)]
    public static SqlBytes DateTimeArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulDateTimeStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulDateTimeStreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "dtArraySetRange", IsDeterministic = true)]
    public static SqlBytes DateTimeArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulDateTimeStreamedArray(array.Stream, true, false))
      using (var ra = new NulDateTimeStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "dtArrayFillRange", IsDeterministic = true)]
    public static SqlBytes DateTimeArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlDateTime value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulDateTimeStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(DateTime?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "dtArrayToCollection", IsDeterministic = true)]
    public static SqlBytes DateTimeArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulDateTimeStreamedArray(array.Stream, true, false))
      using (new NulDateTimeStreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "dtArrayEnumerate", IsDeterministic = true, FillRowMethodName = "DateTimeArrayFillRow")]
    public static IEnumerable DateTimeArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulDateTimeStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void DateTimeArrayFillRow(object obj, out SqlInt32 Index, out SqlDateTime Value)
    {
      Tuple<int, DateTime?> tuple = (Tuple<int, DateTime?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlDateTime.Null;
    }

    #endregion
    #region SqlGuid array methods

    [SqlFunction(Name = "uidArrayCreate", IsDeterministic = true)]
    public static SqlBytes GuidArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulGuidStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(Guid?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "uidArrayParse", IsDeterministic = true)]
    public static SqlBytes GuidArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Guid?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlGuid.Parse(t).Value : default(Guid?));
      using (new NulGuidStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "uidArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GuidArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulGuidStreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlGuid(t.Value) : SqlGuid.Null).ToString());
    }

    [SqlFunction(Name = "uidArrayLength", IsDeterministic = true)]
    public static SqlInt32 GuidArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulGuidStreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "uidArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 GuidArrayIndexOf(SqlBytes array, SqlGuid value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulGuidStreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Guid?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "uidArrayGet", IsDeterministic = true)]
    public static SqlGuid GuidArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlGuid.Null;

      using (var sa = new NulGuidStreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlGuid.Null : v.Value;
      }
    }

    [SqlFunction(Name = "uidArraySet", IsDeterministic = true)]
    public static SqlBytes GuidArraySet(SqlBytes array, SqlInt32 index, SqlGuid value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulGuidStreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Guid?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "uidArrayGetRange", IsDeterministic = true)]
    public static SqlBytes GuidArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulGuidStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulGuidStreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "uidArraySetRange", IsDeterministic = true)]
    public static SqlBytes GuidArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulGuidStreamedArray(array.Stream, true, false))
      using (var ra = new NulGuidStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "uidArrayFillRange", IsDeterministic = true)]
    public static SqlBytes GuidArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlGuid value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulGuidStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Guid?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "uidArrayToCollection", IsDeterministic = true)]
    public static SqlBytes GuidArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulGuidStreamedArray(array.Stream, true, false))
      using (new NulGuidStreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "uidArrayEnumerate", IsDeterministic = true, FillRowMethodName = "GuidArrayFillRow")]
    public static IEnumerable GuidArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulGuidStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void GuidArrayFillRow(object obj, out SqlInt32 Index, out SqlGuid Value)
    {
      Tuple<int, Guid?> tuple = (Tuple<int, Guid?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlGuid.Null;
    }

    #endregion
    #region SqlString array methods

    [SqlFunction(Name = "strArrayCreate", IsDeterministic = true)]
    public static SqlBytes StringArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (length.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new StringStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, SqlRuntime.TextEncoding, null, default(String).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "strArrayCreateByCpId", IsDeterministic = true)]
    public static SqlBytes StringArrayCreateByCpId([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing,
      [DefaultValue("NULL")]SqlInt32 cpId)
    {
      if (length.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      Encoding encoding = cpId.IsNull ? SqlRuntime.TextEncoding : Encoding.GetEncoding(cpId.Value);
      var array = new SqlBytes(new MemoryStream());
      using (new StringStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, encoding, null, default(String).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "strArrayCreateByCpName", IsDeterministic = true)]
    public static SqlBytes StringArrayCreateByCpName([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing, [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing,
      [SqlFacet(MaxSize = 128)][DefaultValue("NULL")]SqlString cpName)
    {
      if (length.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      Encoding encoding = cpName.IsNull ? SqlRuntime.TextEncoding : Encoding.GetEncoding(cpName.Value);
      var array = new SqlBytes(new MemoryStream());
      using (new StringStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, encoding, null, default(String).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "strArrayParse", IsDeterministic = true)]
    public static SqlBytes StringArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var array = SqlFormatting.ParseArray<String>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlFormatting.Unquote(t) : default(String));
      var result = new SqlBytes(new MemoryStream());
      using (new StringStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, SqlRuntime.TextEncoding, null, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "strArrayParseByCpId", IsDeterministic = true)]
    public static SqlBytes StringArrayParseByCpId([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing,
      [DefaultValue("NULL")]SqlInt32 cpId)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      Encoding encoding = cpId.IsNull ? SqlRuntime.TextEncoding : Encoding.GetEncoding(cpId.Value);
      var array = SqlFormatting.ParseArray<String>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlFormatting.Unquote(t) : default(String));
      var result = new SqlBytes(new MemoryStream());
      using (new StringStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, encoding, null, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "strArrayParseByCpName", IsDeterministic = true)]
    public static SqlBytes StringArrayParseByCpName([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing,
      [SqlFacet(MaxSize = 128)][DefaultValue("NULL")]SqlString cpName)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      Encoding encoding = cpName.IsNull ? SqlRuntime.TextEncoding : Encoding.GetEncoding(cpName.Value);
      var array = SqlFormatting.ParseArray<String>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlFormatting.Unquote(t) : default(String));
      var result = new SqlBytes(new MemoryStream());
      using (new StringStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, encoding, null, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "strArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString StringArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new StringStreamedArray(array.Stream, null, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => t != null ? SqlFormatting.Quote(t) : SqlString.Null.ToString());
    }

    [SqlFunction(Name = "strArrayLength", IsDeterministic = true)]
    public static SqlInt32 StringArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new StringStreamedArray(array.Stream, null, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "strArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 StringArrayIndexOf(SqlBytes array, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new StringStreamedArray(array.Stream, null, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(String) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "strArrayGet", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString StringArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlString.Null;

      using (var sa = new StringStreamedArray(array.Stream, null, true, false))
      {
        var v = sa[index.Value];
        return v == null ? SqlString.Null : v;
      }
    }

    [SqlFunction(Name = "strArraySet", IsDeterministic = true)]
    public static SqlBytes StringArraySet(SqlBytes array, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new StringStreamedArray(array.Stream, null, true, false))
        sa[index.Value] = value.IsNull ? null : value.Value;
      return array;
    }

    [SqlFunction(Name = "strArrayGetRange", IsDeterministic = true)]
    public static SqlBytes StringArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new StringStreamedArray(array.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new StringStreamedArray(result.Stream, sa.CountSizing, sa.ItemSizing, sa.Encoding, null, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "strArraySetRange", IsDeterministic = true)]
    public static SqlBytes StringArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new StringStreamedArray(array.Stream, null, true, false))
      using (var ra = new StringStreamedArray(range.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "strArrayFillRange", IsDeterministic = true)]
    public static SqlBytes StringArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new StringStreamedArray(array.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? null : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "strArrayToCollection", IsDeterministic = true)]
    public static SqlBytes StringArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new StringStreamedArray(array.Stream, null, true, false))
      using (new StringStreamedCollection(coll.Stream, sa.CountSizing, sa.ItemSizing, sa.Encoding, null, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "strArrayEnumerate", IsDeterministic = true, FillRowMethodName = "StringArrayFillRow")]
    public static IEnumerable StringArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new StringStreamedArray(array.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void StringArrayFillRow(object obj, out SqlInt32 Index, [SqlFacet(MaxSize = -1)] out SqlString Value)
    {
      Tuple<int, String> tuple = (Tuple<int, String>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2 != null ? tuple.Item2 : SqlString.Null;
    }

    #endregion
    #region SqlBinary array methods

    [SqlFunction(Name = "binArrayCreate", IsDeterministic = true)]
    public static SqlBytes BinaryArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new BinaryStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, default(byte[]).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "binArrayParse", IsDeterministic = true)]
    public static SqlBytes BinaryArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<byte[]>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? PwrBitConverter.ParseBinary(t, true) : default(byte[]));
      using (new BinaryStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "binArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString BinaryArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => t != null ? PwrBitConverter.Format(t, true) : SqlBinary.Null.ToString());
    }

    [SqlFunction(Name = "binArrayLength", IsDeterministic = true)]
    public static SqlInt32 BinaryArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "binArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 BinaryArrayIndexOf(SqlBytes array, [SqlFacet(MaxSize = -1)] SqlBinary value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(byte[]) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "binArrayGet", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlBinary BinaryArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlBinary.Null;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return v == null ? SqlBinary.Null : v;
      }
    }

    [SqlFunction(Name = "binArraySet", IsDeterministic = true)]
    public static SqlBytes BinaryArraySet(SqlBytes array, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlBinary value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? null : value.Value;
      return array;
    }

    [SqlFunction(Name = "binArrayGetRange", IsDeterministic = true)]
    public static SqlBytes BinaryArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new BinaryStreamedArray(result.Stream, sa.CountSizing, sa.ItemSizing, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "binArraySetRange", IsDeterministic = true)]
    public static SqlBytes BinaryArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      using (var ra = new BinaryStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "binArrayFillRange", IsDeterministic = true)]
    public static SqlBytes BinaryArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, [SqlFacet(MaxSize = -1)] SqlBinary value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? null : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "binArrayToCollection", IsDeterministic = true)]
    public static SqlBytes BinaryArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      using (new BinaryStreamedCollection(coll.Stream, sa.CountSizing, sa.ItemSizing, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "binArrayEnumerate", IsDeterministic = true, FillRowMethodName = "BinaryArrayFillRow")]
    public static IEnumerable BinaryArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void BinaryArrayFillRow(object obj, out SqlInt32 Index, [SqlFacet(MaxSize = -1)] out SqlBinary Value)
    {
      Tuple<int, Byte[]> tuple = (Tuple<int, Byte[]>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2 != null ? tuple.Item2 : SqlBinary.Null;
    }

    #endregion
    #region SqlRange array methods

    [SqlFunction(Name = "rngArrayCreate", IsDeterministic = true)]
    public static SqlBytes RangeArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulRangeStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(Range?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "rngArrayParse", IsDeterministic = true)]
    public static SqlBytes RangeArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Range?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlRange.Parse(t).Value : default(Range?));
      using (new NulRangeStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "rngArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString RangeArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulRangeStreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlRange(t.Value) : SqlRange.Null).ToString());
    }

    [SqlFunction(Name = "rngArrayLength", IsDeterministic = true)]
    public static SqlInt32 RangeArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulRangeStreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "rngArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 RangeArrayIndexOf(SqlBytes array, SqlRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulRangeStreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Range?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "rngArrayGet", IsDeterministic = true)]
    public static SqlRange RangeArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlRange.Null;

      using (var sa = new NulRangeStreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlRange.Null : v.Value;
      }
    }

    [SqlFunction(Name = "rngArraySet", IsDeterministic = true)]
    public static SqlBytes RangeArraySet(SqlBytes array, SqlInt32 index, SqlRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulRangeStreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Range?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "rngArrayGetRange", IsDeterministic = true)]
    public static SqlBytes RangeArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulRangeStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulRangeStreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "rngArraySetRange", IsDeterministic = true)]
    public static SqlBytes RangeArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulRangeStreamedArray(array.Stream, true, false))
      using (var ra = new NulRangeStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "rngArrayFillRange", IsDeterministic = true)]
    public static SqlBytes RangeArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulRangeStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Range?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "rngArrayToCollection", IsDeterministic = true)]
    public static SqlBytes RangeArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulRangeStreamedArray(array.Stream, true, false))
      using (new NulRangeStreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "rngArrayEnumerate", IsDeterministic = true, FillRowMethodName = "RangeArrayFillRow")]
    public static IEnumerable RangeArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulRangeStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void RangeArrayFillRow(object obj, out SqlInt32 Index, out SqlRange Value)
    {
      Tuple<int, Range?> tuple = (Tuple<int, Range?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlRange.Null;
    }

    #endregion
    #region SqlLongRange array methods

    [SqlFunction(Name = "brngArrayCreate", IsDeterministic = true)]
    public static SqlBytes LongRangeArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulLongRangeStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(LongRange?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "brngArrayParse", IsDeterministic = true)]
    public static SqlBytes LongRangeArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<LongRange?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlLongRange.Parse(t).Value : default(LongRange?));
      using (new NulLongRangeStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "brngArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString LongRangeArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulLongRangeStreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlLongRange(t.Value) : SqlLongRange.Null).ToString());
    }

    [SqlFunction(Name = "brngArrayLength", IsDeterministic = true)]
    public static SqlInt32 LongRangeArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulLongRangeStreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "brngArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 LongRangeArrayIndexOf(SqlBytes array, SqlLongRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulLongRangeStreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(LongRange?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "brngArrayGet", IsDeterministic = true)]
    public static SqlLongRange LongRangeArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlLongRange.Null;

      using (var sa = new NulLongRangeStreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlLongRange.Null : v.Value;
      }
    }

    [SqlFunction(Name = "brngArraySet", IsDeterministic = true)]
    public static SqlBytes LongRangeArraySet(SqlBytes array, SqlInt32 index, SqlLongRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulLongRangeStreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(LongRange?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "brngArrayGetRange", IsDeterministic = true)]
    public static SqlBytes LongRangeArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulLongRangeStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulLongRangeStreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "brngArraySetRange", IsDeterministic = true)]
    public static SqlBytes LongRangeArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulLongRangeStreamedArray(array.Stream, true, false))
      using (var ra = new NulLongRangeStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "brngArrayFillRange", IsDeterministic = true)]
    public static SqlBytes LongRangeArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlLongRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulLongRangeStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(LongRange?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "brngArrayToCollection", IsDeterministic = true)]
    public static SqlBytes LongRangeArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulLongRangeStreamedArray(array.Stream, true, false))
      using (new NulLongRangeStreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "brngArrayEnumerate", IsDeterministic = true, FillRowMethodName = "LongRangeArrayFillRow")]
    public static IEnumerable LongRangeArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulLongRangeStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void LongRangeArrayFillRow(object obj, out SqlInt32 Index, out SqlLongRange Value)
    {
      Tuple<int, LongRange?> tuple = (Tuple<int, LongRange?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlLongRange.Null;
    }

    #endregion
    #region SqlBigInteger array methods

    [SqlFunction(Name = "hiArrayCreate", IsDeterministic = true)]
    public static SqlBytes BigIntegerArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new BinaryStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, default(byte[]).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "hiArrayParse", IsDeterministic = true)]
    public static SqlBytes BigIntegerArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<byte[]>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlBigInteger.Parse(t).Value.ToByteArray() : default(byte[]));
      using (new BinaryStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "hiArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString BigIntegerArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t != null ? new SqlBigInteger(new BigInteger(t)) : SqlBigInteger.Null).ToString());
    }

    [SqlFunction(Name = "hiArrayLength", IsDeterministic = true)]
    public static SqlInt32 BigIntegerArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "hiArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 BigIntegerArrayIndexOf(SqlBytes array, SqlBigInteger value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(byte[]) : value.Value.ToByteArray());
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "hiArrayGet", IsDeterministic = true)]
    public static SqlBigInteger BigIntegerArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlBigInteger.Null;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return v == null ? SqlBigInteger.Null : new BigInteger(v);
      }
    }

    [SqlFunction(Name = "hiArraySet", IsDeterministic = true)]
    public static SqlBytes BigIntegerArraySet(SqlBytes array, SqlInt32 index, SqlBigInteger value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? null : value.Value.ToByteArray();
      return array;
    }

    [SqlFunction(Name = "hiArrayGetRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new BinaryStreamedArray(result.Stream, sa.CountSizing, sa.ItemSizing, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "hiArraySetRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      using (var ra = new BinaryStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "hiArrayFillRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlBigInteger value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? null : value.Value.ToByteArray(), countValue);
      }
      return array;
    }

    [SqlFunction(Name = "hiArrayToCollection", IsDeterministic = true)]
    public static SqlBytes BigIntegerArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      using (new BinaryStreamedCollection(coll.Stream, sa.CountSizing, sa.ItemSizing, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "hiArrayEnumerate", IsDeterministic = true, FillRowMethodName = "BigIntegerArrayFillRow")]
    public static IEnumerable BigIntegerArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new BinaryStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue).Select(t => t == null ? default(BigInteger?) : new BigInteger(t)))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void BigIntegerArrayFillRow(object obj, out SqlInt32 Index, out SqlBigInteger Value)
    {
      Tuple<int, BigInteger?> tuple = (Tuple<int, BigInteger?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlBigInteger.Null;
    }

    #endregion
    #region SqlComplex array methods

    [SqlFunction(Name = "cxArrayCreate", IsDeterministic = true)]
    public static SqlBytes ComplexArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulComplexStreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(Complex?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "cxArrayParse", IsDeterministic = true)]
    public static SqlBytes ComplexArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Complex?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlComplex.Parse(t).Value : default(Complex?));
      using (new NulComplexStreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "cxArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ComplexArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulComplexStreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlComplex(t.Value) : SqlComplex.Null).ToString());
    }

    [SqlFunction(Name = "cxArrayLength", IsDeterministic = true)]
    public static SqlInt32 ComplexArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulComplexStreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "cxArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 ComplexArrayIndexOf(SqlBytes array, SqlComplex value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulComplexStreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Complex?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "cxArrayGet", IsDeterministic = true)]
    public static SqlComplex ComplexArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlComplex.Null;

      using (var sa = new NulComplexStreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlComplex.Null : v.Value;
      }
    }

    [SqlFunction(Name = "cxArraySet", IsDeterministic = true)]
    public static SqlBytes ComplexArraySet(SqlBytes array, SqlInt32 index, SqlComplex value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulComplexStreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Complex?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "cxArrayGetRange", IsDeterministic = true)]
    public static SqlBytes ComplexArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulComplexStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulComplexStreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "cxArraySetRange", IsDeterministic = true)]
    public static SqlBytes ComplexArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulComplexStreamedArray(array.Stream, true, false))
      using (var ra = new NulComplexStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "cxArrayFillRange", IsDeterministic = true)]
    public static SqlBytes ComplexArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlComplex value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulComplexStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Complex?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "cxArrayToCollection", IsDeterministic = true)]
    public static SqlBytes ComplexArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulComplexStreamedArray(array.Stream, true, false))
      using (new NulComplexStreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "cxArrayEnumerate", IsDeterministic = true, FillRowMethodName = "ComplexArrayFillRow")]
    public static IEnumerable ComplexArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulComplexStreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void ComplexArrayFillRow(object obj, out SqlInt32 Index, out SqlComplex Value)
    {
      Tuple<int, Complex?> tuple = (Tuple<int, Complex?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlComplex.Null;
    }

    #endregion
    #region SqlHourAngle array methods

    [SqlFunction(Name = "haArrayCreate", IsDeterministic = true)]
    public static SqlBytes HourAngleArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(Int32?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "haArrayParse", IsDeterministic = true)]
    public static SqlBytes HourAngleArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Int32?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlHourAngle.Parse(t).Value.Units : default(Int32?));
      using (new NulInt32StreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "haArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString HourAngleArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlHourAngle(new HourAngle(t.Value)) : SqlHourAngle.Null).ToString());
    }

    [SqlFunction(Name = "haArrayLength", IsDeterministic = true)]
    public static SqlInt32 HourAngleArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "haArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 HourAngleArrayIndexOf(SqlBytes array, SqlHourAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Int32?) : value.Value.Units);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "haArrayGet", IsDeterministic = true)]
    public static SqlHourAngle HourAngleArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlHourAngle.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlHourAngle.Null : new HourAngle(v.Value);
      }
    }

    [SqlFunction(Name = "haArraySet", IsDeterministic = true)]
    public static SqlBytes HourAngleArraySet(SqlBytes array, SqlInt32 index, SqlHourAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Int32?) : value.Value.Units;
      return array;
    }

    [SqlFunction(Name = "haArrayGetRange", IsDeterministic = true)]
    public static SqlBytes HourAngleArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulInt32StreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "haArraySetRange", IsDeterministic = true)]
    public static SqlBytes HourAngleArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      using (var ra = new NulInt32StreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "haArrayFillRange", IsDeterministic = true)]
    public static SqlBytes HourAngleArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlHourAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value.Units, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "haArrayToCollection", IsDeterministic = true)]
    public static SqlBytes HourAngleArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      using (new NulInt32StreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "haArrayEnumerate", IsDeterministic = true, FillRowMethodName = "HourAngleArrayFillRow")]
    public static IEnumerable HourAngleArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void HourAngleArrayFillRow(object obj, out SqlInt32 Index, out SqlHourAngle Value)
    {
      Tuple<int, Int32?> tuple = (Tuple<int, Int32?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? new HourAngle(tuple.Item2.Value) : SqlHourAngle.Null;
    }

    #endregion
    #region SqlGradAngle array methods

    [SqlFunction(Name = "gaArrayCreate", IsDeterministic = true)]
    public static SqlBytes GradAngleArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(Int32?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "gaArrayParse", IsDeterministic = true)]
    public static SqlBytes GradAngleArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.Value)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Int32?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlGradAngle.Parse(t).Value.Units : default(Int32?));
      using (new NulInt32StreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "gaArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GradAngleArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlGradAngle(new GradAngle(t.Value)) : SqlGradAngle.Null).ToString());
    }

    [SqlFunction(Name = "gaArrayLength", IsDeterministic = true)]
    public static SqlInt32 GradAngleArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "gaArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 GradArrayIndexOf(SqlBytes array, SqlGradAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Int32?) : value.Value.Units);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "gaArrayGet", IsDeterministic = true)]
    public static SqlGradAngle GradAngleArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlGradAngle.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlGradAngle.Null : new GradAngle(v.Value);
      }
    }

    [SqlFunction(Name = "gaArraySet", IsDeterministic = true)]
    public static SqlBytes GradAngleArraySet(SqlBytes array, SqlInt32 index, SqlGradAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Int32?) : value.Value.Units;
      return array;
    }

    [SqlFunction(Name = "gaArrayGetRange", IsDeterministic = true)]
    public static SqlBytes GradAngleArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulInt32StreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "gaArraySetRange", IsDeterministic = true)]
    public static SqlBytes GradAngleArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      using (var ra = new NulInt32StreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "gaArrayFillRange", IsDeterministic = true)]
    public static SqlBytes GradAngleArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlGradAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value.Units, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "gaArrayToCollection", IsDeterministic = true)]
    public static SqlBytes GradAngleArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      using (new NulInt32StreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "gaArrayEnumerate", IsDeterministic = true, FillRowMethodName = "GradAngleArrayFillRow")]
    public static IEnumerable GradAngleArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void GradAngleArrayFillRow(object obj, out SqlInt32 Index, out SqlGradAngle Value)
    {
      Tuple<int, Int32?> tuple = (Tuple<int, Int32?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? new GradAngle(tuple.Item2.Value) : SqlGradAngle.Null;
    }

    #endregion
    #region SqlSexagesimalAngle array methods

    [SqlFunction(Name = "saArrayCreate", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArrayCreate([SqlFacet(IsNullable = false)]SqlInt32 length,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, default(Int32?).Repeat(length.Value), true, false)) ;
      return array;
    }

    [SqlFunction(Name = "saArrayParse", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArrayParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseArray<Int32?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlSexagesimalAngle.Parse(t).Value.Units : default(Int32?));
      using (new NulInt32StreamedArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array, true, false)) ;
      return result;
    }

    [SqlFunction(Name = "saArrayFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString SexagesimalAngleArrayFormat(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
        return SqlFormatting.Format(sa.ToArray(), t => (t.HasValue ? new SqlSexagesimalAngle(new SexagesimalAngle(t.Value)) : SqlSexagesimalAngle.Null).ToString());
    }

    [SqlFunction(Name = "saArrayLength", IsDeterministic = true)]
    public static SqlInt32 SexagesimalAngleArrayLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
        return sa.Count;
    }

    [SqlFunction(Name = "saArrayIndexOf", IsDeterministic = true)]
    public static SqlInt32 SexagesimalArrayIndexOf(SqlBytes array, SqlSexagesimalAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        var index = sa.IndexOf(value.IsNull ? default(Int32?) : value.Value.Units);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "saArrayGet", IsDeterministic = true)]
    public static SqlSexagesimalAngle SexagesimalAngleArrayGet(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlSexagesimalAngle.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        var v = sa[index.Value];
        return !v.HasValue ? SqlSexagesimalAngle.Null : new SexagesimalAngle(v.Value);
      }
    }

    [SqlFunction(Name = "saArraySet", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArraySet(SqlBytes array, SqlInt32 index, SqlSexagesimalAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
        sa[index.Value] = value.IsNull ? default(Int32?) : value.Value.Units;
      return array;
    }

    [SqlFunction(Name = "saArrayGetRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArrayGetRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulInt32StreamedArray(result.Stream, sa.CountSizing, sa.Compact, sa.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "saArraySetRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArraySetRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      using (var ra = new NulInt32StreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count : index.Value;
        sa.SetRange(indexValue, ra);
      }
      return array;
    }

    [SqlFunction(Name = "saArrayFillRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArrayFillRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlSexagesimalAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sa.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value.Units, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "saArrayToCollection", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArrayToCollection(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      using (new NulInt32StreamedCollection(coll.Stream, sa.CountSizing, sa.Compact, sa, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "saArrayEnumerate", IsDeterministic = true, FillRowMethodName = "SexagesimalAngleArrayFillRow")]
    public static IEnumerable SexagesimalAngleArrayEnumerate(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var sa = new NulInt32StreamedArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? sa.Count - (count.IsNull ? sa.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sa.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sa.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void SexagesimalAngleArrayFillRow(object obj, out SqlInt32 Index, out SqlSexagesimalAngle Value)
    {
      Tuple<int, Int32?> tuple = (Tuple<int, Int32?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? new SexagesimalAngle(tuple.Item2.Value) : SqlSexagesimalAngle.Null;
    }

    #endregion
    #region SqlBoolean regular array methods

    [SqlFunction(Name = "bArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes BooleanArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing)
    {
      if (lengths.IsNull || countSizing.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulBooleanStreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "bArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes BooleanArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing)
    {
      if (str.IsNull || countSizing.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Boolean?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlBoolean.Parse(t).Value : default(Boolean?));
      using (var a = new NulBooleanStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Boolean?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "bArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString BooleanArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Boolean?>(a.ToRegularArray(), t => (t.HasValue ? new SqlBoolean(t.Value) : SqlBoolean.Null).ToString());
    }

    [SqlFunction(Name = "bArrayRank", IsDeterministic = true)]
    public static SqlInt32 BooleanArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "bArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 BooleanArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "bArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes BooleanArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (Int32?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "bArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 BooleanArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "bArrayGetFlat", IsDeterministic = true)]
    public static SqlBoolean BooleanArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlBoolean.Null;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlBoolean.Null : v.Value;
      }
    }

    [SqlFunction(Name = "bArraySetFlat", IsDeterministic = true)]
    public static SqlBytes BooleanArraySetFlat(SqlBytes array, SqlInt32 index, SqlBoolean value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Boolean?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "bArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes BooleanArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var r = new NulBooleanStreamedArray(result.Stream, a.CountSizing, a.EnumerateRange(indexValue, countValue).ToArray(), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "bArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes BooleanArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
      using (var r = new NulBooleanStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "bArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes BooleanArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlBoolean value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Boolean?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "bArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "BooleanArrayFillRowRegular")]
    public static IEnumerable BooleanArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "bArrayGetDim", IsDeterministic = true)]
    public static SqlBoolean BooleanArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlBoolean.Null;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlBoolean.Null : v.Value;
      }
    }

    [SqlFunction(Name = "bArraySetDim", IsDeterministic = true)]
    public static SqlBytes BooleanArraySetDim(SqlBytes array, SqlBytes indices, SqlBoolean value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Boolean?) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "bArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes BooleanArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulBooleanStreamedRegularArray(result.Stream, a.CountSizing, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "bArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes BooleanArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulBooleanStreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "bArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes BooleanArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlBoolean value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Boolean?) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "bArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "BooleanArrayFillRowRegular")]
    public static IEnumerable BooleanArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulBooleanStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void BooleanArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlBoolean Value)
    {
      Tuple<int, int[], Boolean?> tuple = (Tuple<int, int[], Boolean?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlBoolean.Null;
    }

    #endregion
    #region SqlByte regular array methods

    [SqlFunction(Name = "tiArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes ByteArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulByteStreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "tiArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes ByteArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Byte?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlByte.Parse(t).Value : default(Byte?));
      using (var a = new NulByteStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Byte?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "tiArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ByteArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Byte?>(a.ToRegularArray(), t => (t.HasValue ? new SqlByte(t.Value) : SqlByte.Null).ToString());
    }

    [SqlFunction(Name = "tiArrayRank", IsDeterministic = true)]
    public static SqlInt32 ByteArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "tiArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 ByteArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "tiArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes ByteArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "tiArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 ByteArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "tiArrayGetFlat", IsDeterministic = true)]
    public static SqlByte ByteArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlByte.Null;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlByte.Null : v.Value;
      }
    }

    [SqlFunction(Name = "tiArraySetFlat", IsDeterministic = true)]
    public static SqlBytes ByteArraySetFlat(SqlBytes array, SqlInt32 index, SqlByte value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Byte?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "tiArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes ByteArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var r = new NulByteStreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "tiArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes ByteArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
      using (var r = new NulByteStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "tiArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes ByteArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlByte value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Byte?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "tiArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "ByteArrayFillRowRegular")]
    public static IEnumerable ByteArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "tiArrayGetDim", IsDeterministic = true)]
    public static SqlByte ByteArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlByte.Null;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlByte.Null : v.Value;
      }
    }

    [SqlFunction(Name = "tiArraySetDim", IsDeterministic = true)]
    public static SqlBytes ByteArraySetDim(SqlBytes array, SqlBytes indices, SqlByte value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Byte?) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "tiArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes ByteArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulByteStreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "tiArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes ByteArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulByteStreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "tiArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes ByteArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlByte value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Byte?) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "tiArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "ByteArrayFillRowRegular")]
    public static IEnumerable ByteArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulByteStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void ByteArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlByte Value)
    {
      Tuple<int, int[], Byte?> tuple = (Tuple<int, int[], Byte?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlByte.Null;
    }

    #endregion
    #region SqlInt16 regular array methods

    [SqlFunction(Name = "siArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes Int16ArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulInt16StreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "siArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes Int16ArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Int16?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt16.Parse(t).Value : default(Int16?));
      using (var a = new NulInt16StreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Int16?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "siArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Int16ArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Int16?>(a.ToRegularArray(), t => (t.HasValue ? new SqlInt16(t.Value) : SqlInt16.Null).ToString());
    }

    [SqlFunction(Name = "siArrayRank", IsDeterministic = true)]
    public static SqlInt32 Int16ArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "siArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 Int16ArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "siArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes Int16ArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "siArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 Int16ArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "siArrayGetFlat", IsDeterministic = true)]
    public static SqlInt16 Int16ArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt16.Null;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlInt16.Null : v.Value;
      }
    }

    [SqlFunction(Name = "siArraySetFlat", IsDeterministic = true)]
    public static SqlBytes Int16ArraySetFlat(SqlBytes array, SqlInt32 index, SqlInt16 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Int16?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "siArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes Int16ArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulInt16StreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "siArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes Int16ArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
      using (var r = new NulInt16StreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "siArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes Int16ArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlInt16 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Int16?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "siArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "Int16ArrayFillRowRegular")]
    public static IEnumerable Int16ArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "siArrayGetDim", IsDeterministic = true)]
    public static SqlInt16 Int16ArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlInt16.Null;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlInt16.Null : v.Value;
      }
    }

    [SqlFunction(Name = "siArraySetDim", IsDeterministic = true)]
    public static SqlBytes Int16ArraySetDim(SqlBytes array, SqlBytes indices, SqlInt16 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Int16?) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "siArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes Int16ArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulInt16StreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "siArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes Int16ArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulInt16StreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "siArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes Int16ArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlInt16 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Int16?) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "siArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "Int16ArrayFillRowRegular")]
    public static IEnumerable Int16ArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulInt16StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void Int16ArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlInt16 Value)
    {
      Tuple<int, int[], Int16?> tuple = (Tuple<int, int[], Int16?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlInt16.Null;
    }

    #endregion
    #region SqlInt32 regular array methods

    [SqlFunction(Name = "iArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes Int32ArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulInt32StreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "iArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes Int32ArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Int32?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt32.Parse(t).Value : default(Int32?));
      using (var a = new NulInt32StreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Int32?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "iArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Int32ArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Int32?>(a.ToRegularArray(), t => (t.HasValue ? new SqlInt32(t.Value) : SqlInt32.Null).ToString());
    }

    [SqlFunction(Name = "iArrayRank", IsDeterministic = true)]
    public static SqlInt32 Int32ArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "iArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 Int32ArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "iArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes Int32ArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "iArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 Int32ArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "iArrayGetFlat", IsDeterministic = true)]
    public static SqlInt32 Int32ArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlInt32.Null : v.Value;
      }
    }

    [SqlFunction(Name = "iArraySetFlat", IsDeterministic = true)]
    public static SqlBytes Int32ArraySetFlat(SqlBytes array, SqlInt32 index, SqlInt32 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Int32?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "iArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes Int32ArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var r = new NulInt32StreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "iArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes Int32ArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (var r = new NulInt32StreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "iArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes Int32ArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlInt32 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "iArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "Int32ArrayFillRowRegular")]
    public static IEnumerable Int32ArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "iArrayGetDim", IsDeterministic = true)]
    public static SqlInt32 Int32ArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlInt32.Null : v.Value;
      }
    }

    [SqlFunction(Name = "iArraySetDim", IsDeterministic = true)]
    public static SqlBytes Int32ArraySetDim(SqlBytes array, SqlBytes indices, SqlInt32 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Int32?) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "iArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes Int32ArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulInt32StreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "iArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes Int32ArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulInt32StreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "iArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes Int32ArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlInt32 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Int32?) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "iArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "Int32ArrayFillRowRegular")]
    public static IEnumerable Int32ArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void Int32ArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlInt32 Value)
    {
      Tuple<int, int[], Int32?> tuple = (Tuple<int, int[], Int32?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlInt32.Null;
    }

    #endregion
    #region SqlInt64 regular array methods

    [SqlFunction(Name = "biArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes Int64ArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulInt64StreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "biArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes Int64ArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Int64?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt64.Parse(t).Value : default(Int64?));
      using (var a = new NulInt64StreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Int64?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "biArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Int64ArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Int64?>(a.ToRegularArray(), t => (t.HasValue ? new SqlInt64(t.Value) : SqlInt64.Null).ToString());
    }

    [SqlFunction(Name = "biArrayRank", IsDeterministic = true)]
    public static SqlInt32 Int64ArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "biArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 Int64ArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "biArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes Int64ArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "biArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 Int64ArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "biArrayGetFlat", IsDeterministic = true)]
    public static SqlInt64 Int64ArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt64.Null;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlInt64.Null : v.Value;
      }
    }

    [SqlFunction(Name = "biArraySetFlat", IsDeterministic = true)]
    public static SqlBytes Int64ArraySetFlat(SqlBytes array, SqlInt32 index, SqlInt64 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Int64?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "biArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes Int64ArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulInt64StreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "biArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes Int64ArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
      using (var r = new NulInt64StreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "biArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes Int64ArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlInt64 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Int64?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "biArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "Int64ArrayFillRowRegular")]
    public static IEnumerable Int64ArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "biArrayGetDim", IsDeterministic = true)]
    public static SqlInt64 Int64ArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlInt64.Null;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlInt64.Null : v.Value;
      }
    }

    [SqlFunction(Name = "biArraySetDim", IsDeterministic = true)]
    public static SqlBytes Int64ArraySetDim(SqlBytes array, SqlBytes indices, SqlInt64 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Int64?) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "biArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes Int64ArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulInt64StreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "biArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes Int64ArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulInt64StreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "biArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes Int64ArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlInt64 value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Int64?) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "biArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "Int64ArrayFillRowRegular")]
    public static IEnumerable Int64ArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulInt64StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void Int64ArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlInt64 Value)
    {
      Tuple<int, int[], Int64?> tuple = (Tuple<int, int[], Int64?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlInt64.Null;
    }

    #endregion
    #region SqlSingle regular array methods

    [SqlFunction(Name = "sfArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes SingleArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulSingleStreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "sfArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes SingleArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Single?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlSingle.Parse(t).Value : default(Single?));
      using (var a = new NulSingleStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Single?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "sfArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString SingleArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Single?>(a.ToRegularArray(), t => (t.HasValue ? new SqlSingle(t.Value) : SqlSingle.Null).ToString());
    }

    [SqlFunction(Name = "sfArrayRank", IsDeterministic = true)]
    public static SqlInt32 SingleArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "sfArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 SingleArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "sfArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes SingleArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "sfArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 SingleArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "sfArrayGetFlat", IsDeterministic = true)]
    public static SqlSingle SingleArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlSingle.Null;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlSingle.Null : v.Value;
      }
    }

    [SqlFunction(Name = "sfArraySetFlat", IsDeterministic = true)]
    public static SqlBytes SingleArraySetFlat(SqlBytes array, SqlInt32 index, SqlSingle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Single?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "sfArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes SingleArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulSingleStreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false))
        return result;
      }
    }

    [SqlFunction(Name = "sfArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes SingleArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
      using (var r = new NulSingleStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "sfArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes SingleArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlSingle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Single?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "sfArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "SingleArrayFillRowRegular")]
    public static IEnumerable SingleArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "sfArrayGetDim", IsDeterministic = true)]
    public static SqlSingle SingleArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlSingle.Null;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlSingle.Null : v.Value;
      }
    }

    [SqlFunction(Name = "sfArraySetDim", IsDeterministic = true)]
    public static SqlBytes SingleArraySetDim(SqlBytes array, SqlBytes indices, SqlSingle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Single?) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "sfArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes SingleArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulSingleStreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "sfArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes SingleArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulSingleStreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "sfArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes SingleArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlSingle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Single?) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "sfArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "SingleArrayFillRowRegular")]
    public static IEnumerable SingleArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulSingleStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void SingleArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlSingle Value)
    {
      Tuple<int, int[], Single?> tuple = (Tuple<int, int[], Single?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlSingle.Null;
    }

    #endregion
    #region SqlDouble regular array methods

    [SqlFunction(Name = "dfArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes DoubleArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulDoubleStreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "dfArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes DoubleArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Double?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlDouble.Parse(t).Value : default(Double?));
      using (var a = new NulDoubleStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Double?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "dfArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString DoubleArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Double?>(a.ToRegularArray(), t => (t.HasValue ? new SqlDouble(t.Value) : SqlDouble.Null).ToString());
    }

    [SqlFunction(Name = "dfArrayRank", IsDeterministic = true)]
    public static SqlInt32 DoubleArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "dfArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 DoubleArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "dfArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes DoubleArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "dfArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 DoubleArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "dfArrayGetFlat", IsDeterministic = true)]
    public static SqlDouble DoubleArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlDouble.Null;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlDouble.Null : v.Value;
      }
    }

    [SqlFunction(Name = "dfArraySetFlat", IsDeterministic = true)]
    public static SqlBytes DoubleArraySetFlat(SqlBytes array, SqlInt32 index, SqlDouble value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Double?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "dfArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes DoubleArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulDoubleStreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false))
        return result;
      }
    }

    [SqlFunction(Name = "dfArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes DoubleArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
      using (var r = new NulDoubleStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "dfArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes DoubleArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlDouble value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Double?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "dfArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "DoubleArrayFillRowRegular")]
    public static IEnumerable DoubleArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "dfArrayGetDim", IsDeterministic = true)]
    public static SqlDouble DoubleArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlDouble.Null;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlDouble.Null : v.Value;
      }
    }

    [SqlFunction(Name = "dfArraySetDim", IsDeterministic = true)]
    public static SqlBytes DoubleArraySetDim(SqlBytes array, SqlBytes indices, SqlDouble value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Double?) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "dfArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes DoubleArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulDoubleStreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "dfArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes DoubleArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulDoubleStreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "dfArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes DoubleArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlDouble value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Double?) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "dfArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "DoubleArrayFillRowRegular")]
    public static IEnumerable DoubleArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulDoubleStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void DoubleArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlDouble Value)
    {
      Tuple<int, int[], Double?> tuple = (Tuple<int, int[], Double?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlDouble.Null;
    }

    #endregion
    #region SqlDateTime regular array methods

    [SqlFunction(Name = "dtArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes DateTimeArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "dtArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes DateTimeArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<DateTime?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlDateTime.Parse(t).Value : default(DateTime?));
      using (var a = new NulDateTimeStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<DateTime?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "dtArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString DateTimeArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<DateTime?>(a.ToRegularArray(), t => (t.HasValue ? new SqlDateTime(t.Value) : SqlDateTime.Null).ToString());
    }

    [SqlFunction(Name = "dtArrayRank", IsDeterministic = true)]
    public static SqlInt32 DateTimeArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "dtArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 DateTimeArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "dtArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes DateTimeArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "dtArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 DateTimeArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "dtArrayGetFlat", IsDeterministic = true)]
    public static SqlDateTime DateTimeArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlDateTime.Null;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlDateTime.Null : v.Value;
      }
    }

    [SqlFunction(Name = "dtArraySetFlat", IsDeterministic = true)]
    public static SqlBytes DateTimeArraySetFlat(SqlBytes array, SqlInt32 index, SqlDateTime value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(DateTime?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "dtArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes DateTimeArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var r = new NulDateTimeStreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "dtArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes DateTimeArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
      using (var r = new NulDateTimeStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "dtArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes DateTimeArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlDateTime value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(DateTime?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "dtArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "DateTimeArrayFillRowRegular")]
    public static IEnumerable DateTimeArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "dtArrayGetDim", IsDeterministic = true)]
    public static SqlDateTime DateTimeArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlDateTime.Null;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlDateTime.Null : v.Value;
      }
    }

    [SqlFunction(Name = "dtArraySetDim", IsDeterministic = true)]
    public static SqlBytes DateTimeArraySetDim(SqlBytes array, SqlBytes indices, SqlDateTime value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(DateTime?) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "dtArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes DateTimeArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulDateTimeStreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "dtArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes DateTimeArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulDateTimeStreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "dtArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes DateTimeArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlDateTime value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(DateTime?) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "dtArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "DateTimeArrayFillRowRegular")]
    public static IEnumerable DateTimeArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulDateTimeStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void DateTimeArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlDateTime Value)
    {
      Tuple<int, int[], DateTime?> tuple = (Tuple<int, int[], DateTime?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlDateTime.Null;
    }

    #endregion
    #region SqlGuid regular array methods

    [SqlFunction(Name = "uidArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes GuidArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulGuidStreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "uidArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes GuidArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Guid?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlGuid.Parse(t).Value : default(Guid?));
      using (var a = new NulGuidStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Guid?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "uidArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GuidArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Guid?>(a.ToRegularArray(), t => (t.HasValue ? new SqlGuid(t.Value) : SqlGuid.Null).ToString());
    }

    [SqlFunction(Name = "uidArrayRank", IsDeterministic = true)]
    public static SqlInt32 GuidArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "uidArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 GuidArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "uidArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes GuidArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "uidArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 GuidArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "uidArrayGetFlat", IsDeterministic = true)]
    public static SqlGuid GuidArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlGuid.Null;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlGuid.Null : v.Value;
      }
    }

    [SqlFunction(Name = "uidArraySetFlat", IsDeterministic = true)]
    public static SqlBytes GuidArraySetFlat(SqlBytes array, SqlInt32 index, SqlGuid value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Guid?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "uidArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes GuidArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulGuidStreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "uidArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes GuidArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
      using (var r = new NulGuidStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "uidArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes GuidArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlGuid value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Guid?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "uidArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "GuidArrayFillRowRegular")]
    public static IEnumerable GuidArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "uidArrayGetDim", IsDeterministic = true)]
    public static SqlGuid GuidArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlGuid.Null;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlGuid.Null : v.Value;
      }
    }

    [SqlFunction(Name = "uidArraySetDim", IsDeterministic = true)]
    public static SqlBytes GuidArraySetDim(SqlBytes array, SqlBytes indices, SqlGuid value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Guid?) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "uidArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes GuidArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulGuidStreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "uidArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes GuidArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulGuidStreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "uidArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes GuidArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlGuid value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Guid?) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "uidArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "GuidArrayFillRowRegular")]
    public static IEnumerable GuidArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulGuidStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void GuidArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlGuid Value)
    {
      Tuple<int, int[], Guid?> tuple = (Tuple<int, int[], Guid?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlGuid.Null;
    }

    #endregion
    #region SqlString regular array methods

    [SqlFunction(Name = "strArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes StringArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (lengths.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new StringStreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, SqlRuntime.TextEncoding, null, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "strArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes StringArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<String>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlFormatting.Unquote(t) : default(String));
      using (var a = new StringStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, SqlRuntime.TextEncoding, null, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<String>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "strArrayParseRegularByCpId", IsDeterministic = true)]
    public static SqlBytes StringArrayParseRegularByCpId([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing,
      [DefaultValue("NULL")]SqlInt32 cpId)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      Encoding encoding = cpId.IsNull ? SqlRuntime.TextEncoding : Encoding.GetEncoding(cpId.Value);
      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<String>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlFormatting.Unquote(t) : default(String));
      using (var a = new StringStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, encoding, null, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<String>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "strArrayParseRegularByCpName", IsDeterministic = true)]
    public static SqlBytes StringArrayParseRegularByCpName([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing,
      [SqlFacet(MaxSize = 128)][DefaultValue("NULL")]SqlString cpName)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      Encoding encoding = cpName.IsNull ? SqlRuntime.TextEncoding : Encoding.GetEncoding(cpName.Value);
      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<String>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlFormatting.Unquote(t) : default(String));
      using (var a = new StringStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, encoding, null, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<String>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "strArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString StringArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
        return SqlFormatting.FormatRegular<String>(a.ToRegularArray(), t => t != null ? SqlFormatting.Quote(t) : SqlString.Null.ToString());
    }

    [SqlFunction(Name = "strArrayRank", IsDeterministic = true)]
    public static SqlInt32 StringArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "strArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 StringArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "strArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes StringArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "strArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 StringArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "strArrayGetFlat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString StringArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlString.Null;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
      {
        var v = a[index.Value];
        return v ?? SqlString.Null;
      }
    }

    [SqlFunction(Name = "strArraySetFlat", IsDeterministic = true)]
    public static SqlBytes StringArraySetFlat(SqlBytes array, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
        a[index.Value] = value.IsNull ? default(String) : value.Value;
      return array;
    }

    [SqlFunction(Name = "strArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes StringArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new StringStreamedArray(result.Stream, a.CountSizing, a.ItemSizing, a.Encoding, null, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "strArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes StringArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
      using (var r = new StringStreamedArray(range.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "strArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes StringArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(String) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "strArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "StringArrayFillRowRegular")]
    public static IEnumerable StringArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "strArrayGetDim", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString StringArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlString.Null;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return v ?? SqlString.Null;
      }
    }

    [SqlFunction(Name = "strArraySetDim", IsDeterministic = true)]
    public static SqlBytes StringArraySetDim(SqlBytes array, SqlBytes indices, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(String) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "strArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes StringArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new StringStreamedRegularArray(result.Stream, a.CountSizing, a.ItemSizing, a.Encoding, null, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "strArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes StringArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, true))
      using (var r = new StringStreamedRegularArray(range.Stream, null, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "strArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes StringArrayFillDimRange(SqlBytes array, SqlBytes ranges, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(String) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "strArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "StringArrayFillRowRegular")]
    public static IEnumerable StringArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new StringStreamedRegularArray(array.Stream, null, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void StringArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, [SqlFacet(MaxSize = -1)] out SqlString Value)
    {
      Tuple<int, int[], String> tuple = (Tuple<int, int[], String>)obj;
      FlatIndex = tuple.Item1;
      var indices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(indices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      DimIndices = indices;
      Value = tuple.Item3 ?? SqlString.Null;
    }

    #endregion
    #region SqlBinary regular array methods

    [SqlFunction(Name = "binArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes BinaryArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (lengths.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new BinaryStreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "binArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes BinaryArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<byte[]>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? PwrBitConverter.ParseBinary(t, true) : default(byte[]));
      using (var a = new BinaryStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<byte[]>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "binArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString BinaryArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<byte[]>(a.ToRegularArray(), t => t != null ? PwrBitConverter.Format(t, true) : SqlBinary.Null.ToString());
    }

    [SqlFunction(Name = "binArrayRank", IsDeterministic = true)]
    public static SqlInt32 BinaryArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "binArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 BinaryArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "binArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes BinaryArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "binArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 BinaryArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "binArrayGetFlat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlBinary BinaryArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlBinary.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return v ?? SqlBinary.Null;
      }
    }

    [SqlFunction(Name = "binArraySetFlat", IsDeterministic = true)]
    public static SqlBytes BinaryArraySetFlat(SqlBytes array, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlBinary value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(byte[]) : value.Value;
      return array;
    }

    [SqlFunction(Name = "binArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes BinaryArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new BinaryStreamedArray(result.Stream, a.CountSizing, a.ItemSizing, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "binArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes BinaryArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      using (var r = new BinaryStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "binArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes BinaryArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, [SqlFacet(MaxSize = -1)] SqlBinary value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(byte[]) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "binArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "BinaryArrayFillRowRegular")]
    public static IEnumerable BinaryArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "binArrayGetDim", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlBinary BinaryArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlBinary.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return v ?? SqlBinary.Null;
      }
    }

    [SqlFunction(Name = "binArraySetDim", IsDeterministic = true)]
    public static SqlBytes BinaryArraySetDim(SqlBytes array, SqlBytes indices, [SqlFacet(MaxSize = -1)] SqlBinary value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(byte[]) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "binArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes BinaryArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new BinaryStreamedRegularArray(result.Stream, a.CountSizing, a.ItemSizing, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "binArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes BinaryArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, true))
      using (var r = new BinaryStreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "binArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes BinaryArrayFillDimRange(SqlBytes array, SqlBytes ranges, [SqlFacet(MaxSize = -1)] SqlBinary value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(byte[]) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "binArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "BinaryArrayFillRowRegular")]
    public static IEnumerable BinaryArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void BinaryArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, [SqlFacet(MaxSize = -1)] out SqlBinary Value)
    {
      Tuple<int, int[], byte[]> tuple = (Tuple<int, int[], byte[]>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3 ?? SqlBinary.Null;
    }

    #endregion
    #region SqlRange regular array methods

    [SqlFunction(Name = "rngArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes RangeArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulRangeStreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.IsNull, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "rngArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes RangeArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Range?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlRange.Parse(t).Value : default(Range?));
      using (var a = new NulRangeStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.IsNull, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Range?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "rngArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString RangeArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Range?>(a.ToRegularArray(), t => (t.HasValue ? new SqlRange(t.Value) : SqlRange.Null).ToString());
    }

    [SqlFunction(Name = "rngArrayRank", IsDeterministic = true)]
    public static SqlInt32 RangeArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "rngArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 RangeArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "rngArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes RangeArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "rngArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 RangeArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "rngArrayGetFlat", IsDeterministic = true)]
    public static SqlRange RangeArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlRange.Null;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlRange.Null : v.Value;
      }
    }

    [SqlFunction(Name = "rngArraySetFlat", IsDeterministic = true)]
    public static SqlBytes RangeArraySetFlat(SqlBytes array, SqlInt32 index, SqlRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Range?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "rngArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes RangeArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulRangeStreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "rngArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes RangeArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
      using (var r = new NulRangeStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "rngArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes RangeArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Range?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "rngArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "RangeArrayFillRowRegular")]
    public static IEnumerable RangeArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "rngArrayGetDim", IsDeterministic = true)]
    public static SqlRange RangeArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlRange.Null;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlRange.Null : v.Value;
      }
    }

    [SqlFunction(Name = "rngArraySetDim", IsDeterministic = true)]
    public static SqlBytes RangeArraySetDim(SqlBytes array, SqlBytes indices, SqlRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Range?) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "rngArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes RangeArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulRangeStreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "rngArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes RangeArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "rngArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes RangeArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Range?) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "rngArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "RangeArrayFillRowRegular")]
    public static IEnumerable RangeArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulRangeStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void RangeArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlRange Value)
    {
      Tuple<int, int[], Range?> tuple = (Tuple<int, int[], Range?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlRange.Null;
    }

    #endregion
    #region SqlLongRange regular array methods

    [SqlFunction(Name = "brngArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes LongRangeArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.IsNull, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "brngArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes LongRangeArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<LongRange?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlLongRange.Parse(t).Value : default(LongRange?));
      using (var a = new NulLongRangeStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.IsNull, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<LongRange?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "brngArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString LongRangeArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<LongRange?>(a.ToRegularArray(), t => (t.HasValue ? new SqlLongRange(t.Value) : SqlLongRange.Null).ToString());
    }

    [SqlFunction(Name = "brngArrayRank", IsDeterministic = true)]
    public static SqlInt32 LongRangeArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "brngArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 LongRangeArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "brngArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes LongRangeArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "brngArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 LongRangeArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "brngArrayGetFlat", IsDeterministic = true)]
    public static SqlLongRange LongRangeArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlLongRange.Null;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlLongRange.Null : v.Value;
      }
    }

    [SqlFunction(Name = "brngArraySetFlat", IsDeterministic = true)]
    public static SqlBytes LongRangeArraySetFlat(SqlBytes array, SqlInt32 index, SqlLongRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(LongRange?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "brngArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes LongRangeArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulLongRangeStreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "brngArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes LongRangeArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
      using (var r = new NulLongRangeStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "brngArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes LongRangeArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlLongRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(LongRange?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "brngArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "LongRangeArrayFillRowRegular")]
    public static IEnumerable LongRangeArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "brngArrayGetDim", IsDeterministic = true)]
    public static SqlLongRange LongRangeArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlLongRange.Null;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlLongRange.Null : v.Value;
      }
    }

    [SqlFunction(Name = "brngArraySetDim", IsDeterministic = true)]
    public static SqlBytes LongRangeArraySetDim(SqlBytes array, SqlBytes indices, SqlLongRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(LongRange?) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "brngArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes LongRangeArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulLongRangeStreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "brngArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes LongRangeArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulLongRangeStreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "brngArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes LongRangeArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlLongRange value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(LongRange?) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "brngArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "LongRangeArrayFillRowRegular")]
    public static IEnumerable LongRangeArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulLongRangeStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void LongRangeArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlLongRange Value)
    {
      Tuple<int, int[], LongRange?> tuple = (Tuple<int, int[], LongRange?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlLongRange.Null;
    }

    #endregion
    #region SqlBigInteger regular array methods

    [SqlFunction(Name = "hiArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes BigIntegerArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (lengths.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new BinaryStreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "hiArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes BigIntegerArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<byte[]>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlBigInteger.Parse(t).Value.ToByteArray() : default(byte[]));
      using (var a = new BinaryStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<byte[]>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "hiArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString BigIntegerArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<byte[]>(a.ToRegularArray(), t => (t != null ? new SqlBigInteger(new BigInteger(t)) : SqlBigInteger.Null).ToString());
    }

    [SqlFunction(Name = "hiArrayRank", IsDeterministic = true)]
    public static SqlInt32 BigIntegerArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "hiArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 BigIntegerArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "hiArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes BigIntegerArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "hiArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 BigIntegerArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "hiArrayGetFlat", IsDeterministic = true)]
    public static SqlBigInteger BigIntegerArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlBigInteger.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return v == null ? SqlBigInteger.Null : new BigInteger(v);
      }
    }

    [SqlFunction(Name = "hiArraySetFlat", IsDeterministic = true)]
    public static SqlBytes BigIntegerArraySetFlat(SqlBytes array, SqlInt32 index, SqlBigInteger value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(byte[]) : value.Value.ToByteArray();
      return array;
    }

    [SqlFunction(Name = "hiArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var r = new BinaryStreamedArray(result.Stream, a.CountSizing, a.ItemSizing, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false))
        return result;
      }
    }

    [SqlFunction(Name = "hiArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      using (var r = new BinaryStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "hiArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlBigInteger value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(byte[]) : value.Value.ToByteArray(), countValue);
      }
      return array;
    }

    [SqlFunction(Name = "hiArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "BigIntegerArrayFillRowRegular")]
    public static IEnumerable BigIntegerArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "hiArrayGetDim", IsDeterministic = true)]
    public static SqlBigInteger BigIntegerArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlBigInteger.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return v == null ? SqlBigInteger.Null : new BigInteger(v);
      }
    }

    [SqlFunction(Name = "hiArraySetDim", IsDeterministic = true)]
    public static SqlBytes BigIntegerArraySetDim(SqlBytes array, SqlBytes indices, SqlBigInteger value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(byte[]) : value.Value.ToByteArray(), d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "hiArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new BinaryStreamedRegularArray(result.Stream, a.CountSizing, a.ItemSizing, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "hiArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, true))
      using (var r = new BinaryStreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "hiArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlBigInteger value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(byte[]) : value.Value.ToByteArray(); });
      }
      return array;
    }

    [SqlFunction(Name = "hiArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "BigIntegerArrayFillRowRegular")]
    public static IEnumerable BigIntegerArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new BinaryStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void BigIntegerArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlBigInteger Value)
    {
      Tuple<int, int[], BigInteger?> tuple = (Tuple<int, int[], BigInteger?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlBigInteger.Null;
    }

    #endregion
    #region SqlComplex regular array methods

    [SqlFunction(Name = "cxArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes ComplexArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulComplexStreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "cxArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes ComplexArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Complex?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlComplex.Parse(t).Value : default(Complex?));
      using (var a = new NulComplexStreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Complex?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "cxArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ComplexArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Complex?>(a.ToRegularArray(), t => (t.HasValue ? new SqlComplex(t.Value) : SqlComplex.Null).ToString());
    }

    [SqlFunction(Name = "cxArrayRank", IsDeterministic = true)]
    public static SqlInt32 ComplexArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "cxArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 ComplexArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "cxArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes ComplexArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "cxArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 ComplexArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "cxArrayGetFlat", IsDeterministic = true)]
    public static SqlComplex ComplexArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlComplex.Null;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlComplex.Null : v.Value;
      }
    }

    [SqlFunction(Name = "cxArraySetFlat", IsDeterministic = true)]
    public static SqlBytes ComplexArraySetFlat(SqlBytes array, SqlInt32 index, SqlComplex value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Complex?) : value.Value;
      return array;
    }

    [SqlFunction(Name = "cxArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes ComplexArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulComplexStreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "cxArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes ComplexArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
      using (var r = new NulComplexStreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "cxArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes ComplexArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlComplex value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Complex?) : value.Value, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "cxArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "ComplexArrayFillRowRegular")]
    public static IEnumerable ComplexArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "cxArrayGetDim", IsDeterministic = true)]
    public static SqlComplex ComplexArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlComplex.Null;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlComplex.Null : v.Value;
      }
    }

    [SqlFunction(Name = "cxArraySetDim", IsDeterministic = true)]
    public static SqlBytes ComplexArraySetDim(SqlBytes array, SqlBytes indices, SqlComplex value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Complex?) : value.Value, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "cxArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes ComplexArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulComplexStreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "cxArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes ComplexArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulComplexStreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "cxArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes ComplexArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlComplex value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Complex?) : value.Value; });
      }
      return array;
    }

    [SqlFunction(Name = "cxArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "ComplexArrayFillRowRegular")]
    public static IEnumerable ComplexArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulComplexStreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void ComplexArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlComplex Value)
    {
      Tuple<int, int[], Complex?> tuple = (Tuple<int, int[], Complex?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? tuple.Item3.Value : SqlComplex.Null;
    }

    #endregion
    #region SqlHourAngle regular array methods

    [SqlFunction(Name = "haArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes HourAngleArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulInt32StreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "haArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes HourAngleArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Int32?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlHourAngle.Parse(t).Value.Units : default(Int32?));
      using (var a = new NulInt32StreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Int32?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "haArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString HourAngleArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Int32?>(a.ToRegularArray(), t => (t.HasValue ? new SqlHourAngle(new HourAngle(t.Value)) : SqlHourAngle.Null).ToString());
    }

    [SqlFunction(Name = "haArrayRank", IsDeterministic = true)]
    public static SqlInt32 HourAngleArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "haArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 HourAngleArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "haArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes HourAngleArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "haArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 HourAngleArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "haArrayGetFlat", IsDeterministic = true)]
    public static SqlHourAngle HourAngleArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlHourAngle.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlHourAngle.Null : new HourAngle(v.Value);
      }
    }

    [SqlFunction(Name = "haArraySetFlat", IsDeterministic = true)]
    public static SqlBytes HourAngleArraySetFlat(SqlBytes array, SqlInt32 index, SqlHourAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Int32?) : value.Value.Units;
      return array;
    }

    [SqlFunction(Name = "haArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes HourAngleArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulInt32StreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "haArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes HourAngleArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (var r = new NulInt32StreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "haArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes HourAngleArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlHourAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value.Units, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "haArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "HourAngleArrayFillRowRegular")]
    public static IEnumerable HourAngleArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "haArrayGetDim", IsDeterministic = true)]
    public static SqlHourAngle HourAngleArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlHourAngle.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlHourAngle.Null : new HourAngle(v.Value);
      }
    }

    [SqlFunction(Name = "haArraySetDim", IsDeterministic = true)]
    public static SqlBytes HourAngleArraySetDim(SqlBytes array, SqlBytes indices, SqlHourAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Int32?) : value.Value.Units, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "haArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes HourAngleArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulInt32StreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "haArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes HourAngleArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulInt32StreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "haArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes HourAngleArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlHourAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Int32?) : value.Value.Units; });
      }
      return array;
    }

    [SqlFunction(Name = "haArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "HourAngleArrayFillRowRegular")]
    public static IEnumerable HourAngleArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void HourAngleArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlHourAngle Value)
    {
      Tuple<int, int[], Int32?> tuple = (Tuple<int, int[], Int32?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? new HourAngle(tuple.Item3.Value) : SqlHourAngle.Null;
    }

    #endregion
    #region SqlGradAngle regular array methods

    [SqlFunction(Name = "gaArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes GradAngleArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulInt32StreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "gaArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes GradAngleArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Int32?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlGradAngle.Parse(t).Value.Units : default(Int32?));
      using (var a = new NulInt32StreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Int32?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "gaArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GradAngleArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Int32?>(a.ToRegularArray(), t => (t.HasValue ? new SqlGradAngle(new GradAngle(t.Value)) : SqlGradAngle.Null).ToString());
    }

    [SqlFunction(Name = "gaArrayRank", IsDeterministic = true)]
    public static SqlInt32 GradAngleArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "gaArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 GradAngleArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "gaArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes GradAngleArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "gaArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 GradAngleArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "gaArrayGetFlat", IsDeterministic = true)]
    public static SqlGradAngle GradAngleArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlGradAngle.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlGradAngle.Null : new GradAngle(v.Value);
      }
    }

    [SqlFunction(Name = "gaArraySetFlat", IsDeterministic = true)]
    public static SqlBytes GradAngleArraySetFlat(SqlBytes array, SqlInt32 index, SqlGradAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Int32?) : value.Value.Units;
      return array;
    }

    [SqlFunction(Name = "gaArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes GradAngleArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulInt32StreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false))
        return result;
      }
    }

    [SqlFunction(Name = "gaArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes GradAngleArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (var r = new NulInt32StreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "gaArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes GradAngleArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlGradAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value.Units, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "gaArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "GradAngleArrayFillRowRegular")]
    public static IEnumerable GradAngleArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "gaArrayGetDim", IsDeterministic = true)]
    public static SqlGradAngle GradAngleArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlGradAngle.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlGradAngle.Null : new GradAngle(v.Value);
      }
    }

    [SqlFunction(Name = "gaArraySetDim", IsDeterministic = true)]
    public static SqlBytes GradAngleArraySetDim(SqlBytes array, SqlBytes indices, SqlGradAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Int32?) : value.Value.Units, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "gaArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes GradAngleArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulInt32StreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "gaArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes GradAngleArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulInt32StreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "gaArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes GradAngleArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlGradAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Int32?) : value.Value.Units; });
      }
      return array;
    }

    [SqlFunction(Name = "gaArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "GradAngleArrayFillRowRegular")]
    public static IEnumerable GradAngleArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void GradAngleArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlGradAngle Value)
    {
      Tuple<int, int[], Int32?> tuple = (Tuple<int, int[], Int32?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? new GradAngle(tuple.Item3.Value) : SqlGradAngle.Null;
    }

    #endregion
    #region SqlSexagesimalAngle regular array methods

    [SqlFunction(Name = "saArrayCreateRegular", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArrayCreateRegular([SqlFacet(IsNullable = false)]SqlBytes lengths,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (lengths.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var l = new NulInt32StreamedArray(lengths.Stream, true, false))
      using (var a = new NulInt32StreamedRegularArray(array.Stream, (SizeEncoding)countSizing.Value, compact.Value, l.Select(t => t.Value).ToArray(), true, false))
        return array;
    }

    [SqlFunction(Name = "saArrayParseRegular", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArrayParseRegular([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var result = new SqlBytes(new MemoryStream());
      var array = SqlFormatting.ParseRegular<Int32?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlSexagesimalAngle.Parse(t).Value.Units : default(Int32?));
      using (var a = new NulInt32StreamedRegularArray(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, array.GetRegularArrayLengths(), true, false))
        a.SetRange(0, array.EnumerateAsRegular<Int32?>().Counted(array.Length));
      return result;
    }

    [SqlFunction(Name = "saArrayFormatRegular", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString SexagesimalAngleArrayFormatRegular(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlFormatting.NullText;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return SqlFormatting.FormatRegular<Int32?>(a.ToRegularArray(), t => (t.HasValue ? new SqlSexagesimalAngle(new SexagesimalAngle(t.Value)) : SqlSexagesimalAngle.Null).ToString());
    }

    [SqlFunction(Name = "saArrayRank", IsDeterministic = true)]
    public static SqlInt32 SexagesimalAngleArrayRank(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return a.Lengths.Count;
    }

    [SqlFunction(Name = "saArrayFlatLength", IsDeterministic = true)]
    public static SqlInt32 SexagesimalAngleArrayFlatLength(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return a.Count;
    }

    [SqlFunction(Name = "saArrayDimLengths", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArrayDimLengths(SqlBytes array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      var lengths = new SqlBytes(new MemoryStream());
      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (new NulInt32StreamedArray(lengths.Stream, SizeEncoding.B1, true, a.Lengths.Select(t => (int?)t).Counted(a.Lengths.Count), true, false)) ;
      return lengths;
    }

    [SqlFunction(Name = "saArrayDimLength", IsDeterministic = true)]
    public static SqlInt32 SexagesimalAngleArrayDimLength(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        return a.Lengths[index.Value];
    }

    [SqlFunction(Name = "saArrayGetFlat", IsDeterministic = true)]
    public static SqlSexagesimalAngle SexagesimalAngleArrayGetFlat(SqlBytes array, SqlInt32 index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return SqlSexagesimalAngle.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        var v = a[index.Value];
        return !v.HasValue ? SqlSexagesimalAngle.Null : new SexagesimalAngle(v.Value);
      }
    }

    [SqlFunction(Name = "saArraySetFlat", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArraySetFlat(SqlBytes array, SqlInt32 index, SqlSexagesimalAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull || index.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
        a[index.Value] = value.IsNull ? default(Int32?) : value.Value.Units;
      return array;
    }

    [SqlFunction(Name = "saArrayGetFlatRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArrayGetFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return SqlBytes.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (new NulInt32StreamedArray(result.Stream, a.CountSizing, true, a.EnumerateRange(indexValue, countValue).Counted(countValue), true, false)) ;
        return result;
      }
    }

    [SqlFunction(Name = "saArraySetFlatRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArraySetFlatRange(SqlBytes array, SqlInt32 index, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || range.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (var r = new NulInt32StreamedArray(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count : index.Value;
        a.SetRange(indexValue, r);
      }
      return array;
    }

    [SqlFunction(Name = "saArrayFillFlatRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArrayFillFlatRange(SqlBytes array, SqlInt32 index, SqlInt32 count, SqlSexagesimalAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        a.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value.Units, countValue);
      }
      return array;
    }

    [SqlFunction(Name = "saArrayEnumerateFlat", IsDeterministic = true, FillRowMethodName = "SexagesimalAngleArrayFillRowRegular")]
    public static IEnumerable SexagesimalAngleArrayEnumerateFlat(SqlBytes array, SqlInt32 index, SqlInt32 count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.IsNull)
        yield break;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      {
        int indexValue = index.IsNull ? a.Count - (count.IsNull ? a.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? a.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in a.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue, a.GetDimIndices(indexValue++), item);
      }
    }

    [SqlFunction(Name = "saArrayGetDim", IsDeterministic = true)]
    public static SqlSexagesimalAngle SexagesimalAngleArrayGetDim(SqlBytes array, SqlBytes indices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return SqlSexagesimalAngle.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
      {
        var v = a.GetValue(d.Select(t => t.Value).ToArray());
        return !v.HasValue ? SqlSexagesimalAngle.Null : new SexagesimalAngle(v.Value);
      }
    }

    [SqlFunction(Name = "saArraySetDim", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArraySetDim(SqlBytes array, SqlBytes indices, SqlSexagesimalAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");

      if (array.IsNull || indices.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, false))
      using (var d = new NulInt32StreamedArray(indices.Stream, true, false))
        a.SetValue(value.IsNull ? default(Int32?) : value.Value.Units, d.Select(t => t.Value).ToArray());
      return array;
    }

    [SqlFunction(Name = "saArrayGetDimRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArrayGetDimRange(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return SqlBytes.Null;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (var d = new NulInt32StreamedRegularArray(result.Stream, a.CountSizing, true, ra.Select(t => t.Count).ToArray(), true, true))
          a.EnumerateRangeIndex(ra)
            .ForEach((t, i) => { d[i] = a[t]; });
        return result;
      }
    }

    [SqlFunction(Name = "saArraySetDimRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArraySetDimRange(SqlBytes array, SqlBytes indices, SqlBytes range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (indices == null)
        throw new ArgumentNullException("indices");
      if (range == null)
        throw new ArgumentNullException("range");

      if (array.IsNull || indices.IsNull || range.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulInt32StreamedRegularArray(range.Stream, true, true))
      using (var n = new NulInt32StreamedArray(indices.Stream, true, true))
        a.EnumerateRangeIndex(r.Lengths.Select((l, i) => new Range(n[i].Value, l)).ToArray())
          .ForEach((t, i) => { a[t] = r[i]; });
      return array;
    }

    [SqlFunction(Name = "saArrayFillDimRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleArrayFillDimRange(SqlBytes array, SqlBytes ranges, SqlSexagesimalAngle value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        return array;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        a.EnumerateRangeIndex(ra)
          .ForEach(t => { a[t] = value.IsNull ? default(Int32?) : value.Value.Units; });
      }
      return array;
    }

    [SqlFunction(Name = "saArrayEnumerateDim", IsDeterministic = true, FillRowMethodName = "SexagesimalAngleArrayFillRowRegular")]
    public static IEnumerable SexagesimalAngleArrayEnumerateDim(SqlBytes array, SqlBytes ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");

      if (array.IsNull || ranges.IsNull)
        yield break;

      using (var a = new NulInt32StreamedRegularArray(array.Stream, true, true))
      using (var r = new NulRangeStreamedArray(ranges.Stream, true, true))
      {
        Range[] ra = r.Select(t => t.Value).ToArray();
        foreach (var fi in a.EnumerateRangeIndex(ra))
          yield return Tuple.Create(fi, a.GetDimIndices(fi), a[fi]);
      }
    }

    private static void SexagesimalAngleArrayFillRowRegular(object obj, out SqlInt32 FlatIndex, out SqlBytes DimIndices, out SqlSexagesimalAngle Value)
    {
      Tuple<int, int[], Int32?> tuple = (Tuple<int, int[], Int32?>)obj;
      FlatIndex = tuple.Item1;
      DimIndices = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(DimIndices.Stream, SizeEncoding.B1, true, tuple.Item2.Select(t => (int?)t).Counted(tuple.Item2.Length), true, false)) ;
      Value = tuple.Item3.HasValue ? new SexagesimalAngle(tuple.Item3.Value) : SqlSexagesimalAngle.Null;
    }

    #endregion
  }
}
