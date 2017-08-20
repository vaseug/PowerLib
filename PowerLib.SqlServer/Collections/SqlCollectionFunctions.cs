using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.SqlTypes;
using PowerLib.System.Numerics;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.SqlServer.Collections
{
  public static class SqlCollectionFunctions
  {
    #region SqlBoolean collection methods

    [SqlFunction(Name = "bCollCreate", IsDeterministic = true)]
    public static SqlBytes BooleanCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing)
    {
      if (countSizing.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulBooleanStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "bCollParse", IsDeterministic = true)]
    public static SqlBytes BooleanCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing)
    {
      if (str.IsNull || countSizing.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<Boolean?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlBoolean.Parse(t).Value : default(Boolean?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulBooleanStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "bCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString BooleanCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlBoolean(t.Value) : SqlBoolean.Null).ToString());
    }

    [SqlFunction(Name = "bCollCount", IsDeterministic = true)]
    public static SqlInt32 BooleanCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "bCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 BooleanCollIndexOf(SqlBytes coll, SqlBoolean value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Boolean?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "bCollGet", IsDeterministic = true)]
    public static SqlBoolean BooleanCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlBoolean.Null;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return !v.HasValue ? SqlBoolean.Null : v.Value;
      }
    }

    [SqlFunction(Name = "bCollSet", IsDeterministic = true)]
    public static SqlBytes BooleanCollSet(SqlBytes coll, SqlInt32 index, SqlBoolean value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
        sc[index.Value] = value.IsNull ? default(Boolean?) : value.Value;
      return coll;
    }

    [SqlFunction(Name = "bCollInsert", IsDeterministic = true)]
    public static SqlBytes BooleanCollInsert(SqlBytes coll, SqlInt32 index, SqlBoolean value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, value.IsNull ? default(Boolean?) : value.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "bCollRemove", IsDeterministic = true)]
    public static SqlBytes BooleanCollRemove(SqlBytes coll, SqlBoolean value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
        sc.Remove(value.IsNull ? default(Boolean?) : value.Value);
      return coll;
    }

    [SqlFunction(Name = "bCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes BooleanCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "bCollClear", IsDeterministic = true)]
    public static SqlBytes BooleanCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "bCollGetRange", IsDeterministic = true)]
    public static SqlBytes BooleanCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulBooleanStreamedCollection(result.Stream, sc.CountSizing, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "bCollInsertRange", IsDeterministic = true)]
    public static SqlBytes BooleanCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulBooleanStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "bCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes BooleanCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlBoolean value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Boolean?) : value.Value, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "bCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes BooleanCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "bCollSetRange", IsDeterministic = true)]
    public static SqlBytes BooleanCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulBooleanStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "bCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes BooleanCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlBoolean value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Boolean?) : value.Value, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "bCollToArray", IsDeterministic = true)]
    public static SqlBytes BooleanCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulBooleanStreamedCollection(array.Stream, true, false))
      using (new NulBooleanStreamedCollection(coll.Stream, sc.CountSizing, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "bCollEnumerate", IsDeterministic = true, FillRowMethodName = "BooleanCollFillRow")]
    public static IEnumerable BooleanCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulBooleanStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void BooleanCollFillRow(object obj, out SqlInt32 Index, out SqlBoolean Value)
    {
      Tuple<int, Boolean?> tuple = (Tuple<int, Boolean?>)obj;
      Index = tuple.Item1;
      Value = !tuple.Item2.HasValue ? SqlBoolean.Null : tuple.Item2.Value;
    }

    #endregion
    #region SqlByte collection methods

    [SqlFunction(Name = "tiCollCreate", IsDeterministic = true)]
    public static SqlBytes ByteCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulByteStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "tiCollParse", IsDeterministic = true)]
    public static SqlBytes ByteCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<Byte?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlByte.Parse(t).Value : default(Byte?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulByteStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "tiCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ByteCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlByte(t.Value) : SqlByte.Null).ToString());
    }

    [SqlFunction(Name = "tiCollCount", IsDeterministic = true)]
    public static SqlInt32 ByteCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "tiCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 ByteCollIndexOf(SqlBytes coll, SqlByte value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Byte?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "tiCollGet", IsDeterministic = true)]
    public static SqlByte ByteCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlByte.Null;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? v.Value : SqlByte.Null;
      }
    }

    [SqlFunction(Name = "tiCollSet", IsDeterministic = true)]
    public static SqlBytes ByteCollSet(SqlBytes coll, SqlInt32 index, SqlByte value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(Byte?);
      return coll;
    }

    [SqlFunction(Name = "tiCollInsert", IsDeterministic = true)]
    public static SqlBytes ByteCollInsert(SqlBytes coll, SqlInt32 index, SqlByte value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(Byte?));
      }
      return coll;
    }

    [SqlFunction(Name = "tiCollRemove", IsDeterministic = true)]
    public static SqlBytes ByteCollRemove(SqlBytes coll, SqlByte value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(Byte?));
      return coll;
    }

    [SqlFunction(Name = "tiCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes ByteCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "tiCollClear", IsDeterministic = true)]
    public static SqlBytes ByteCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "tiCollGetRange", IsDeterministic = true)]
    public static SqlBytes ByteCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulByteStreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "tiCollInsertRange", IsDeterministic = true)]
    public static SqlBytes ByteCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulByteStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "tiCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes ByteCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlByte value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Byte?) : value.Value, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "tiCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes ByteCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "tiCollSetRange", IsDeterministic = true)]
    public static SqlBytes ByteCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulByteStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "tiCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes ByteCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlByte value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Byte?) : value.Value, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "tiCollToArray", IsDeterministic = true)]
    public static SqlBytes ByteCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulByteStreamedCollection(array.Stream, true, false))
      using (new NulByteStreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "tiCollEnumerate", IsDeterministic = true, FillRowMethodName = "ByteCollFillRow")]
    public static IEnumerable ByteCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulByteStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void ByteCollFillRow(object obj, out SqlInt32 Index, out SqlByte Value)
    {
      Tuple<int, Byte?> tuple = (Tuple<int, Byte?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlByte.Null;
    }

    #endregion
    #region SqlInt16 collection methods

    [SqlFunction(Name = "siCollCreate", IsDeterministic = true)]
    public static SqlBytes Int16CollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulInt16StreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "siCollParse", IsDeterministic = true)]
    public static SqlBytes Int16CollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<Int16?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt16.Parse(t).Value : default(Int16?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulInt16StreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "siCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Int16CollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlInt16(t.Value) : SqlInt16.Null).ToString());
    }

    [SqlFunction(Name = "siCollCount", IsDeterministic = true)]
    public static SqlInt32 Int16CollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "siCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 Int16CollIndexOf(SqlBytes coll, SqlInt16 value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Int16?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "siCollGet", IsDeterministic = true)]
    public static SqlInt16 Int16CollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlInt16.Null;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? v.Value : SqlInt16.Null;
      }
    }

    [SqlFunction(Name = "siCollSet", IsDeterministic = true)]
    public static SqlBytes Int16CollSet(SqlBytes coll, SqlInt32 index, SqlInt16 value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(Int16?);
      return coll;
    }

    [SqlFunction(Name = "siCollInsert", IsDeterministic = true)]
    public static SqlBytes Int16CollInsert(SqlBytes coll, SqlInt32 index, SqlInt16 value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(Int16?));
      }
      return coll;
    }

    [SqlFunction(Name = "siCollRemove", IsDeterministic = true)]
    public static SqlBytes Int16CollRemove(SqlBytes coll, SqlInt16 value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(Int16?));
      return coll;
    }

    [SqlFunction(Name = "siCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes Int16CollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "siCollClear", IsDeterministic = true)]
    public static SqlBytes Int16CollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "siCollGetRange", IsDeterministic = true)]
    public static SqlBytes Int16CollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulInt16StreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "siCollInsertRange", IsDeterministic = true)]
    public static SqlBytes Int16CollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
      using (var rc = new NulInt16StreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "siCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes Int16CollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlInt16 value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Int16?) : value.Value, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "siCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes Int16CollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "siCollSetRange", IsDeterministic = true)]
    public static SqlBytes Int16CollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
      using (var rc = new NulInt16StreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "siCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes Int16CollSetRepeat(SqlBytes coll, SqlInt32 index, SqlInt16 value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Int16?) : value.Value, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "siCollToArray", IsDeterministic = true)]
    public static SqlBytes Int16CollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulInt16StreamedCollection(array.Stream, true, false))
      using (new NulInt16StreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "siCollEnumerate", IsDeterministic = true, FillRowMethodName = "Int16CollFillRow")]
    public static IEnumerable Int16CollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulInt16StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void Int16CollFillRow(object obj, out SqlInt32 Index, out SqlInt16 Value)
    {
      Tuple<int, Int16?> tuple = (Tuple<int, Int16?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlInt16.Null;
    }

    #endregion
    #region SqlInt32 collection methods

    [SqlFunction(Name = "iCollCreate", IsDeterministic = true)]
    public static SqlBytes Int32CollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "iCollParse", IsDeterministic = true)]
    public static SqlBytes Int32CollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<Int32?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt32.Parse(t).Value : default(Int32?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulInt32StreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "iCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Int32CollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlInt32(t.Value) : SqlInt32.Null).ToString());
    }

    [SqlFunction(Name = "iCollCount", IsDeterministic = true)]
    public static SqlInt32 Int32CollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "iCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 Int32CollIndexOf(SqlBytes coll, SqlInt32 value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Int32?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "iCollGet", IsDeterministic = true)]
    public static SqlInt32 Int32CollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? v.Value : SqlInt32.Null;
      }
    }

    [SqlFunction(Name = "iCollSet", IsDeterministic = true)]
    public static SqlBytes Int32CollSet(SqlBytes coll, SqlInt32 index, SqlInt32 value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(Int32?);
      return coll;
    }

    [SqlFunction(Name = "iCollInsert", IsDeterministic = true)]
    public static SqlBytes Int32CollInsert(SqlBytes coll, SqlInt32 index, SqlInt32 value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(Int32?));
      }
      return coll;
    }

    [SqlFunction(Name = "iCollRemove", IsDeterministic = true)]
    public static SqlBytes Int32CollRemove(SqlBytes coll, SqlInt32 value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(Int32?));
      return coll;
    }

    [SqlFunction(Name = "iCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes Int32CollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "iCollClear", IsDeterministic = true)]
    public static SqlBytes Int32CollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "iCollGetRange", IsDeterministic = true)]
    public static SqlBytes Int32CollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulInt32StreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "iCollInsertRange", IsDeterministic = true)]
    public static SqlBytes Int32CollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      using (var rc = new NulInt32StreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "iCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes Int32CollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlInt32 value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "iCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes Int32CollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "iCollSetRange", IsDeterministic = true)]
    public static SqlBytes Int32CollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      using (var rc = new NulInt32StreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "iCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes Int32CollSetRepeat(SqlBytes coll, SqlInt32 index, SqlInt32 value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "iCollToArray", IsDeterministic = true)]
    public static SqlBytes Int32CollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulInt32StreamedCollection(array.Stream, true, false))
      using (new NulInt32StreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "iCollEnumerate", IsDeterministic = true, FillRowMethodName = "Int32CollFillRow")]
    public static IEnumerable Int32CollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void Int32CollFillRow(object obj, out SqlInt32 Index, out SqlInt32 Value)
    {
      Tuple<int, Int32?> tuple = (Tuple<int, Int32?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlInt32.Null;
    }

    #endregion
    #region SqlInt64 collection methods

    [SqlFunction(Name = "biCollCreate", IsDeterministic = true)]
    public static SqlBytes Int64CollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulInt64StreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "biCollParse", IsDeterministic = true)]
    public static SqlBytes Int64CollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<Int64?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt64.Parse(t).Value : default(Int64?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulInt64StreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "biCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Int64CollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlInt64(t.Value) : SqlInt64.Null).ToString());
    }

    [SqlFunction(Name = "biCollCount", IsDeterministic = true)]
    public static SqlInt32 Int64CollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "biCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 Int64CollIndexOf(SqlBytes coll, SqlInt64 value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Int64?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "biCollGet", IsDeterministic = true)]
    public static SqlInt64 Int64CollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlInt64.Null;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? v.Value : SqlInt64.Null;
      }
    }

    [SqlFunction(Name = "biCollSet", IsDeterministic = true)]
    public static SqlBytes Int64CollSet(SqlBytes coll, SqlInt32 index, SqlInt64 value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(Int64?);
      return coll;
    }

    [SqlFunction(Name = "biCollInsert", IsDeterministic = true)]
    public static SqlBytes Int64CollInsert(SqlBytes coll, SqlInt32 index, SqlInt64 value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(Int64?));
      }
      return coll;
    }

    [SqlFunction(Name = "biCollRemove", IsDeterministic = true)]
    public static SqlBytes Int64CollRemove(SqlBytes coll, SqlInt64 value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(Int64?));
      return coll;
    }

    [SqlFunction(Name = "biCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes Int64CollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "biCollClear", IsDeterministic = true)]
    public static SqlBytes Int64CollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "biCollGetRange", IsDeterministic = true)]
    public static SqlBytes Int64CollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulInt64StreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "biCollInsertRange", IsDeterministic = true)]
    public static SqlBytes Int64CollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
      using (var rc = new NulInt64StreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "biCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes Int64CollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlInt64 value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Int64?) : value.Value, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "biCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes Int64CollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "biCollSetRange", IsDeterministic = true)]
    public static SqlBytes Int64CollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
      using (var rc = new NulInt64StreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "biCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes Int64CollSetRepeat(SqlBytes coll, SqlInt32 index, SqlInt64 value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Int64?) : value.Value, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "biCollToArray", IsDeterministic = true)]
    public static SqlBytes Int64CollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulInt64StreamedCollection(array.Stream, true, false))
      using (new NulInt64StreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "biCollEnumerate", IsDeterministic = true, FillRowMethodName = "Int64CollFillRow")]
    public static IEnumerable Int64CollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulInt64StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void Int64CollFillRow(object obj, out SqlInt32 Index, out SqlInt64 Value)
    {
      Tuple<int, Int64?> tuple = (Tuple<int, Int64?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlInt64.Null;
    }

    #endregion
    #region SqlSingle collection methods

    [SqlFunction(Name = "sfCollCreate", IsDeterministic = true)]
    public static SqlBytes SingleCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulSingleStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "sfCollParse", IsDeterministic = true)]
    public static SqlBytes SingleCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<Single?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlSingle.Parse(t).Value : default(Single?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulSingleStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "sfCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString SingleCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlSingle(t.Value) : SqlSingle.Null).ToString());
    }

    [SqlFunction(Name = "sfCollCount", IsDeterministic = true)]
    public static SqlInt32 SingleCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "sfCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 SingleCollIndexOf(SqlBytes coll, SqlSingle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Single?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "sfCollGet", IsDeterministic = true)]
    public static SqlSingle SingleCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlSingle.Null;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? v.Value : SqlSingle.Null;
      }
    }

    [SqlFunction(Name = "sfCollSet", IsDeterministic = true)]
    public static SqlBytes SingleCollSet(SqlBytes coll, SqlInt32 index, SqlSingle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(Single?);
      return coll;
    }

    [SqlFunction(Name = "sfCollInsert", IsDeterministic = true)]
    public static SqlBytes SingleCollInsert(SqlBytes coll, SqlInt32 index, SqlSingle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(Single?));
      }
      return coll;
    }

    [SqlFunction(Name = "sfCollRemove", IsDeterministic = true)]
    public static SqlBytes SingleCollRemove(SqlBytes coll, SqlSingle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(Single?));
      return coll;
    }

    [SqlFunction(Name = "sfCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes SingleCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "sfCollClear", IsDeterministic = true)]
    public static SqlBytes SingleCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "sfCollGetRange", IsDeterministic = true)]
    public static SqlBytes SingleCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulSingleStreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "sfCollInsertRange", IsDeterministic = true)]
    public static SqlBytes SingleCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulSingleStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "sfCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes SingleCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlSingle value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Single?) : value.Value, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "sfCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes SingleCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "sfCollSetRange", IsDeterministic = true)]
    public static SqlBytes SingleCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulSingleStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "sfCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes SingleCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlSingle value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Single?) : value.Value, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "sfCollToArray", IsDeterministic = true)]
    public static SqlBytes SingleCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulSingleStreamedCollection(array.Stream, true, false))
      using (new NulSingleStreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "sfCollEnumerate", IsDeterministic = true, FillRowMethodName = "SingleCollFillRow")]
    public static IEnumerable SingleCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulSingleStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void SingleCollFillRow(object obj, out SqlInt32 Index, out SqlSingle Value)
    {
      Tuple<int, Single?> tuple = (Tuple<int, Single?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlSingle.Null;
    }

    #endregion
    #region SqlDouble collection methods

    [SqlFunction(Name = "dfCollCreate", IsDeterministic = true)]
    public static SqlBytes DoubleCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulDoubleStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "dfCollParse", IsDeterministic = true)]
    public static SqlBytes DoubleCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<Double?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlDouble.Parse(t).Value : default(Double?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulDoubleStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "dfCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString DoubleCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlDouble(t.Value) : SqlDouble.Null).ToString());
    }

    [SqlFunction(Name = "dfCollCount", IsDeterministic = true)]
    public static SqlInt32 DoubleCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "dfCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 DoubleCollIndexOf(SqlBytes coll, SqlDouble value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Double?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "dfCollGet", IsDeterministic = true)]
    public static SqlDouble DoubleCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlDouble.Null;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? v.Value : SqlDouble.Null;
      }
    }

    [SqlFunction(Name = "dfCollSet", IsDeterministic = true)]
    public static SqlBytes DoubleCollSet(SqlBytes coll, SqlInt32 index, SqlDouble value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(Double?);
      return coll;
    }

    [SqlFunction(Name = "dfCollInsert", IsDeterministic = true)]
    public static SqlBytes DoubleCollInsert(SqlBytes coll, SqlInt32 index, SqlDouble value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(Double?));
      }
      return coll;
    }

    [SqlFunction(Name = "dfCollRemove", IsDeterministic = true)]
    public static SqlBytes DoubleCollRemove(SqlBytes coll, SqlDouble value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(Double?));
      return coll;
    }

    [SqlFunction(Name = "dfCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes DoubleCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "dfCollClear", IsDeterministic = true)]
    public static SqlBytes DoubleCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "dfCollGetRange", IsDeterministic = true)]
    public static SqlBytes DoubleCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulDoubleStreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "dfCollInsertRange", IsDeterministic = true)]
    public static SqlBytes DoubleCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulDoubleStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "dfCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes DoubleCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlDouble value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Double?) : value.Value, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "dfCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes DoubleCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "dfCollSetRange", IsDeterministic = true)]
    public static SqlBytes DoubleCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulDoubleStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "dfCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes DoubleCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlDouble value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Double?) : value.Value, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "dfCollToArray", IsDeterministic = true)]
    public static SqlBytes DoubleCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulDoubleStreamedCollection(array.Stream, true, false))
      using (new NulDoubleStreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "dfCollEnumerate", IsDeterministic = true, FillRowMethodName = "DoubleCollFillRow")]
    public static IEnumerable DoubleCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulDoubleStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void DoubleCollFillRow(object obj, out SqlInt32 Index, out SqlDouble Value)
    {
      Tuple<int, Double?> tuple = (Tuple<int, Double?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlDouble.Null;
    }

    #endregion
    #region SqlDateTime collection methods

    [SqlFunction(Name = "dtCollCreate", IsDeterministic = true)]
    public static SqlBytes DateTimeCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulDateTimeStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "dtCollParse", IsDeterministic = true)]
    public static SqlBytes DateTimeCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<DateTime?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlDateTime.Parse(t).Value : default(DateTime?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulDateTimeStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "dtCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString DateTimeCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlDateTime(t.Value) : SqlDateTime.Null).ToString());
    }

    [SqlFunction(Name = "dtCollCount", IsDeterministic = true)]
    public static SqlInt32 DateTimeCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "dtCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 DateTimeCollIndexOf(SqlBytes coll, SqlDateTime value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(DateTime?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "dtCollGet", IsDeterministic = true)]
    public static SqlDateTime DateTimeCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlDateTime.Null;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? v.Value : SqlDateTime.Null;
      }
    }

    [SqlFunction(Name = "dtCollSet", IsDeterministic = true)]
    public static SqlBytes DateTimeCollSet(SqlBytes coll, SqlInt32 index, SqlDateTime value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(DateTime?);
      return coll;
    }

    [SqlFunction(Name = "dtCollInsert", IsDeterministic = true)]
    public static SqlBytes DateTimeCollInsert(SqlBytes coll, SqlInt32 index, SqlDateTime value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(DateTime?));
      }
      return coll;
    }

    [SqlFunction(Name = "dtCollRemove", IsDeterministic = true)]
    public static SqlBytes DateTimeCollRemove(SqlBytes coll, SqlDateTime value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(DateTime?));
      return coll;
    }

    [SqlFunction(Name = "dtCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes DateTimeCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "dtCollClear", IsDeterministic = true)]
    public static SqlBytes DateTimeCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "dtCollGetRange", IsDeterministic = true)]
    public static SqlBytes DateTimeCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulDateTimeStreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "dtCollInsertRange", IsDeterministic = true)]
    public static SqlBytes DateTimeCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulDateTimeStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "dtCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes DateTimeCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlDateTime value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(DateTime?) : value.Value, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "dtCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes DateTimeCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "dtCollSetRange", IsDeterministic = true)]
    public static SqlBytes DateTimeCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulDateTimeStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "dtCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes DateTimeCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlDateTime value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(DateTime?) : value.Value, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "dtCollToArray", IsDeterministic = true)]
    public static SqlBytes DateTimeCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulDateTimeStreamedCollection(array.Stream, true, false))
      using (new NulDateTimeStreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "dtCollEnumerate", IsDeterministic = true, FillRowMethodName = "DateTimeCollFillRow")]
    public static IEnumerable DateTimeCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulDateTimeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void DateTimeCollFillRow(object obj, out SqlInt32 Index, out SqlDateTime Value)
    {
      Tuple<int, DateTime?> tuple = (Tuple<int, DateTime?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlDateTime.Null;
    }

    #endregion
    #region SqlGuid collection methods

    [SqlFunction(Name = "uidCollCreate", IsDeterministic = true)]
    public static SqlBytes GuidCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulGuidStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "uidCollParse", IsDeterministic = true)]
    public static SqlBytes GuidCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<Guid?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlGuid.Parse(t).Value : default(Guid?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulGuidStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "uidCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GuidCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlGuid(t.Value) : SqlGuid.Null).ToString());
    }

    [SqlFunction(Name = "uidCollCount", IsDeterministic = true)]
    public static SqlInt32 GuidCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "uidCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 GuidCollIndexOf(SqlBytes coll, SqlGuid value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Guid?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "uidCollGet", IsDeterministic = true)]
    public static SqlGuid GuidCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlGuid.Null;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? v.Value : SqlGuid.Null;
      }
    }

    [SqlFunction(Name = "uidCollSet", IsDeterministic = true)]
    public static SqlBytes GuidCollSet(SqlBytes coll, SqlInt32 index, SqlGuid value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(Guid?);
      return coll;
    }

    [SqlFunction(Name = "uidCollInsert", IsDeterministic = true)]
    public static SqlBytes GuidCollInsert(SqlBytes coll, SqlInt32 index, SqlGuid value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(Guid?));
      }
      return coll;
    }

    [SqlFunction(Name = "uidCollRemove", IsDeterministic = true)]
    public static SqlBytes GuidCollRemove(SqlBytes coll, SqlGuid value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(Guid?));
      return coll;
    }

    [SqlFunction(Name = "uidCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes GuidCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "uidCollClear", IsDeterministic = true)]
    public static SqlBytes GuidCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "uidCollGetRange", IsDeterministic = true)]
    public static SqlBytes GuidCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulGuidStreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "uidCollInsertRange", IsDeterministic = true)]
    public static SqlBytes GuidCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulGuidStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "uidCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes GuidCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlGuid value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Guid?) : value.Value, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "uidCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes GuidCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "uidCollSetRange", IsDeterministic = true)]
    public static SqlBytes GuidCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulGuidStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "uidCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes GuidCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlGuid value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Guid?) : value.Value, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "uidCollToArray", IsDeterministic = true)]
    public static SqlBytes GuidCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulGuidStreamedCollection(array.Stream, true, false))
      using (new NulGuidStreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "uidCollEnumerate", IsDeterministic = true, FillRowMethodName = "GuidCollFillRow")]
    public static IEnumerable GuidCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulGuidStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void GuidCollFillRow(object obj, out SqlInt32 Index, out SqlGuid Value)
    {
      Tuple<int, Guid?> tuple = (Tuple<int, Guid?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlGuid.Null;
    }

    #endregion
    #region SqlString collection methods

    [SqlFunction(Name = "strCollCreate", IsDeterministic = true)]
    public static SqlBytes StringCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new StringStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, SqlRuntime.TextEncoding, null, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "strCollCreateByCpId", IsDeterministic = true)]
    public static SqlBytes StringCollCreateByCpId(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing,
      [DefaultValue("NULL")]SqlInt32 cpId)
    {
      if (countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new StringStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, cpId.IsNull ? SqlRuntime.TextEncoding : Encoding.GetEncoding(cpId.Value), null, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "strCollCreateByCpName", IsDeterministic = true)]
    public static SqlBytes StringCollCreateByCpName(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing,
      [SqlFacet(MaxSize = 128)][DefaultValue("NULL")]SqlString cpName)
    {
      if (countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new StringStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, cpName.IsNull ? SqlRuntime.TextEncoding : Encoding.GetEncoding(cpName.Value), null, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "strCollParse", IsDeterministic = true)]
    public static SqlBytes StringCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<String>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlFormatting.Unquote(t) : default(String));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new StringStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, SqlRuntime.TextEncoding, null, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "strCollParseByCpId", IsDeterministic = true)]
    public static SqlBytes StringCollParseByCpId([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing,
      [DefaultValue("NULL")]SqlInt32 cpId)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      Encoding encoding = cpId.IsNull ? SqlRuntime.TextEncoding : Encoding.GetEncoding(cpId.Value);
      var coll = SqlFormatting.ParseCollection<String>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlFormatting.Unquote(t) : default(String));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new StringStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, encoding, null, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "strCollParseByCpName", IsDeterministic = true)]
    public static SqlBytes StringCollParseByCpName([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing,
      [SqlFacet(MaxSize = 128)][DefaultValue("NULL")]SqlString cpName)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      Encoding encoding = cpName.IsNull ? SqlRuntime.TextEncoding : Encoding.GetEncoding(cpName.Value);
      var coll = SqlFormatting.ParseCollection<String>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlFormatting.Unquote(t) : default(String));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new StringStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, encoding, null, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "strCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString StringCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
        return SqlFormatting.Format(sc, t => t != null ? SqlFormatting.Quote(t) : SqlString.Null.ToString());
    }

    [SqlFunction(Name = "strCollCount", IsDeterministic = true)]
    public static SqlInt32 StringCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "strCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 StringCollIndexOf(SqlBytes coll, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(String) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "strCollGet", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString StringCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlString.Null;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
      {
        var v = sc[index.Value];
        return v != null ? v : SqlString.Null;
      }
    }

    [SqlFunction(Name = "strCollSet", IsDeterministic = true)]
    public static SqlBytes StringCollSet(SqlBytes coll, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(String);
      return coll;
    }

    [SqlFunction(Name = "strCollInsert", IsDeterministic = true)]
    public static SqlBytes StringCollInsert(SqlBytes coll, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(String));
      }
      return coll;
    }

    [SqlFunction(Name = "strCollRemove", IsDeterministic = true)]
    public static SqlBytes StringCollRemove(SqlBytes coll, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(String));
      return coll;
    }

    [SqlFunction(Name = "strCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes StringCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "strCollClear", IsDeterministic = true)]
    public static SqlBytes StringCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "strCollGetRange", IsDeterministic = true)]
    public static SqlBytes StringCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new StringStreamedCollection(result.Stream, sc.CountSizing, sc.ItemSizing, sc.Encoding, null, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "strCollInsertRange", IsDeterministic = true)]
    public static SqlBytes StringCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
      using (var rc = new StringStreamedCollection(range.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "strCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes StringCollInsertRepeat(SqlBytes coll, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, !value.IsNull ? value.Value : default(String), count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "strCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes StringCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "strCollSetRange", IsDeterministic = true)]
    public static SqlBytes StringCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
      using (var rc = new StringStreamedCollection(range.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "strCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes StringCollSetRepeat(SqlBytes coll, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, !value.IsNull ? value.Value : default(String), countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "strCollToArray", IsDeterministic = true)]
    public static SqlBytes StringCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new StringStreamedCollection(array.Stream, null, true, false))
      using (new StringStreamedCollection(coll.Stream, sc.CountSizing, sc.ItemSizing, sc.Encoding, null, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "strCollEnumerate", IsDeterministic = true, FillRowMethodName = "StringCollFillRow")]
    public static IEnumerable StringCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new StringStreamedCollection(coll.Stream, null, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void StringCollFillRow(object obj, out SqlInt32 Index, [SqlFacet(MaxSize = -1)] out SqlString Value)
    {
      Tuple<int, String> tuple = (Tuple<int, String>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2 != null ? tuple.Item2 : SqlString.Null;
    }

    #endregion
    #region SqlBinary collection methods

    [SqlFunction(Name = "binCollCreate", IsDeterministic = true)]
    public static SqlBytes BinaryCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new BinaryStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "binCollParse", IsDeterministic = true)]
    public static SqlBytes BinaryCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<byte[]>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? PwrBitConverter.ParseBinary(t, true) : default(byte[]));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new BinaryStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "binCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString BinaryCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => t != null ? PwrBitConverter.Format(t, true) : SqlBinary.Null.ToString());
    }

    [SqlFunction(Name = "binCollCount", IsDeterministic = true)]
    public static SqlInt32 BinaryCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "binCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 BinaryCollIndexOf(SqlBytes coll, [SqlFacet(MaxSize = -1)] SqlBinary value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Byte[]) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "binCollGet", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlBinary BinaryCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlBinary.Null;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v != null ? v : SqlBinary.Null;
      }
    }

    [SqlFunction(Name = "binCollSet", IsDeterministic = true)]
    public static SqlBytes BinaryCollSet(SqlBytes coll, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlBinary value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(Byte[]);
      return coll;
    }

    [SqlFunction(Name = "binCollInsert", IsDeterministic = true)]
    public static SqlBytes BinaryCollInsert(SqlBytes coll, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlBinary value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(Byte[]));
      }
      return coll;
    }

    [SqlFunction(Name = "binCollRemove", IsDeterministic = true)]
    public static SqlBytes BinaryCollRemove(SqlBytes coll, [SqlFacet(MaxSize = -1)] SqlBinary value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(Byte[]));
      return coll;
    }

    [SqlFunction(Name = "binCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes BinaryCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "binCollClear", IsDeterministic = true)]
    public static SqlBytes BinaryCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "binCollGetRange", IsDeterministic = true)]
    public static SqlBytes BinaryCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new BinaryStreamedCollection(result.Stream, sc.CountSizing, sc.ItemSizing, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "binCollInsertRange", IsDeterministic = true)]
    public static SqlBytes BinaryCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      using (var rc = new BinaryStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "binCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes BinaryCollInsertRepeat(SqlBytes coll, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlBinary value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, !value.IsNull ? value.Value : default(Byte[]), count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "binCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes BinaryCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "binCollSetRange", IsDeterministic = true)]
    public static SqlBytes BinaryCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      using (var rc = new BinaryStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "binCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes BinaryCollSetRepeat(SqlBytes coll, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlBinary value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, !value.IsNull ? value.Value : default(Byte[]), countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "binCollToArray", IsDeterministic = true)]
    public static SqlBytes BinaryCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new BinaryStreamedCollection(array.Stream, true, false))
      using (new BinaryStreamedCollection(coll.Stream, sc.CountSizing, sc.ItemSizing, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "binCollEnumerate", IsDeterministic = true, FillRowMethodName = "BinaryCollFillRow")]
    public static IEnumerable BinaryCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void BinaryCollFillRow(object obj, out SqlInt32 Index, [SqlFacet(MaxSize = -1)] out SqlBinary Value)
    {
      Tuple<int, Byte[]> tuple = (Tuple<int, Byte[]>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2 != null ? tuple.Item2 : SqlBinary.Null;
    }

    #endregion
    #region SqlRange collection methods

    [SqlFunction(Name = "rngCollCreate", IsDeterministic = true)]
    public static SqlBytes RangeCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulRangeStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "rngCollParse", IsDeterministic = true)]
    public static SqlBytes RangeCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<Range?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlRange.Parse(t).Value : default(Range?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulRangeStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "rngCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString RangeCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlRange(t.Value) : SqlRange.Null).ToString());
    }

    [SqlFunction(Name = "rngCollCount", IsDeterministic = true)]
    public static SqlInt32 RangeCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "rngCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 RangeCollIndexOf(SqlBytes coll, SqlRange value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Range?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "rngCollGet", IsDeterministic = true)]
    public static SqlRange RangeCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlRange.Null;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? v.Value : SqlRange.Null;
      }
    }

    [SqlFunction(Name = "rngCollSet", IsDeterministic = true)]
    public static SqlBytes RangeCollSet(SqlBytes coll, SqlInt32 index, SqlRange value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(Range?);
      return coll;
    }

    [SqlFunction(Name = "rngCollInsert", IsDeterministic = true)]
    public static SqlBytes RangeCollInsert(SqlBytes coll, SqlInt32 index, SqlRange value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(Range?));
      }
      return coll;
    }

    [SqlFunction(Name = "rngCollRemove", IsDeterministic = true)]
    public static SqlBytes RangeCollRemove(SqlBytes coll, SqlRange value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(Range?));
      return coll;
    }

    [SqlFunction(Name = "rngCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes RangeCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "rngCollClear", IsDeterministic = true)]
    public static SqlBytes RangeCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "rngCollGetRange", IsDeterministic = true)]
    public static SqlBytes RangeCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulRangeStreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "rngCollInsertRange", IsDeterministic = true)]
    public static SqlBytes RangeCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulRangeStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "rngCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes RangeCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlRange value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Range?) : value.Value, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "rngCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes RangeCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "rngCollSetRange", IsDeterministic = true)]
    public static SqlBytes RangeCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulRangeStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "rngCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes RangeCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlRange value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Range?) : value.Value, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "rngCollToArray", IsDeterministic = true)]
    public static SqlBytes RangeCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulRangeStreamedCollection(array.Stream, true, false))
      using (new NulRangeStreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "rngCollEnumerate", IsDeterministic = true, FillRowMethodName = "RangeCollFillRow")]
    public static IEnumerable RangeCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulRangeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void RangeCollFillRow(object obj, out SqlInt32 Index, out SqlRange Value)
    {
      Tuple<int, Range?> tuple = (Tuple<int, Range?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlRange.Null;
    }

    #endregion
    #region SqlLongRange collection methods

    [SqlFunction(Name = "brngCollCreate", IsDeterministic = true)]
    public static SqlBytes LongRangeCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulLongRangeStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "brngCollParse", IsDeterministic = true)]
    public static SqlBytes LongRangeCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<LongRange?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlLongRange.Parse(t).Value : default(LongRange?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulLongRangeStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "brngCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString LongRangeCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlLongRange(t.Value) : SqlLongRange.Null).ToString());
    }

    [SqlFunction(Name = "brngCollCount", IsDeterministic = true)]
    public static SqlInt32 LongRangeCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "brngCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 LongRangeCollIndexOf(SqlBytes coll, SqlLongRange value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(LongRange?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "brngCollGet", IsDeterministic = true)]
    public static SqlLongRange LongRangeCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlLongRange.Null;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? v.Value : SqlLongRange.Null;
      }
    }

    [SqlFunction(Name = "brngCollSet", IsDeterministic = true)]
    public static SqlBytes LongRangeCollSet(SqlBytes coll, SqlInt32 index, SqlLongRange value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(LongRange?);
      return coll;
    }

    [SqlFunction(Name = "brngCollInsert", IsDeterministic = true)]
    public static SqlBytes LongRangeCollInsert(SqlBytes coll, SqlInt32 index, SqlLongRange value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(LongRange?));
      }
      return coll;
    }

    [SqlFunction(Name = "brngCollRemove", IsDeterministic = true)]
    public static SqlBytes LongRangeCollRemove(SqlBytes coll, SqlLongRange value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(LongRange?));
      return coll;
    }

    [SqlFunction(Name = "brngCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes LongRangeCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "brngCollClear", IsDeterministic = true)]
    public static SqlBytes LongRangeCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "brngCollGetRange", IsDeterministic = true)]
    public static SqlBytes LongRangeCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulLongRangeStreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "brngCollInsertRange", IsDeterministic = true)]
    public static SqlBytes LongRangeCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulLongRangeStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "brngCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes LongRangeCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlLongRange value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(LongRange?) : value.Value, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "brngCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes LongRangeCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "brngCollSetRange", IsDeterministic = true)]
    public static SqlBytes LongRangeCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulLongRangeStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "brngCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes LongRangeCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlLongRange value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(LongRange?) : value.Value, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "brngCollToArray", IsDeterministic = true)]
    public static SqlBytes LongRangeCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulLongRangeStreamedCollection(array.Stream, true, false))
      using (new NulLongRangeStreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "brngCollEnumerate", IsDeterministic = true, FillRowMethodName = "LongRangeCollFillRow")]
    public static IEnumerable LongRangeCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulLongRangeStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void LongRangeCollFillRow(object obj, out SqlInt32 Index, out SqlLongRange Value)
    {
      Tuple<int, LongRange?> tuple = (Tuple<int, LongRange?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlLongRange.Null;
    }

    #endregion
    #region SqlBigInteger collection methods

    [SqlFunction(Name = "hiCollCreate", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new BinaryStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "hiCollParse", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte itemSizing)
    {
      if (str.IsNull || countSizing.IsNull || itemSizing.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<BigInteger?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlBigInteger.Parse(t).Value : default(BigInteger?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new BinaryStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, (SizeEncoding)itemSizing.Value, true, false))
        sc.AddRange(coll.Select(t => !t.HasValue ? default(byte[]) : t.Value.ToByteArray()));
      return result;
    }

    [SqlFunction(Name = "hiCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString BigIntegerCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t == null ? new SqlBigInteger(new BigInteger(t)) : SqlBigInteger.Null).ToString());
    }

    [SqlFunction(Name = "hiCollCount", IsDeterministic = true)]
    public static SqlInt32 BigIntegerCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "hiCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 BigIntegerCollIndexOf(SqlBytes coll, SqlBigInteger value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(byte[]) : value.Value.ToByteArray());
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "hiCollGet", IsDeterministic = true)]
    public static SqlBigInteger BigIntegerCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlBigInteger.Null;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v != null ? new BigInteger(v) : SqlBigInteger.Null;
      }
    }

    [SqlFunction(Name = "hiCollSet", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollSet(SqlBytes coll, SqlInt32 index, SqlBigInteger value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value.ToByteArray() : default(byte[]);
      return coll;
    }

    [SqlFunction(Name = "hiCollInsert", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollInsert(SqlBytes coll, SqlInt32 index, SqlBigInteger value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value.ToByteArray() : default(byte[]));
      }
      return coll;
    }

    [SqlFunction(Name = "hiCollRemove", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollRemove(SqlBytes coll, SqlBigInteger value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value.ToByteArray() : default(byte[]));
      return coll;
    }

    [SqlFunction(Name = "hiCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "hiCollClear", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "hiCollGetRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new BinaryStreamedCollection(result.Stream, sc.CountSizing, sc.ItemSizing, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "hiCollInsertRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      using (var rc = new BinaryStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "hiCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlBigInteger value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(byte[]) : value.Value.ToByteArray(), count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "hiCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "hiCollSetRange", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      using (var rc = new BinaryStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "hiCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlBigInteger value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(byte[]) : value.Value.ToByteArray(), countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "hiCollToArray", IsDeterministic = true)]
    public static SqlBytes BigIntegerCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new BinaryStreamedCollection(array.Stream, true, false))
      using (new BinaryStreamedCollection(coll.Stream, sc.CountSizing, sc.ItemSizing, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "hiCollEnumerate", IsDeterministic = true, FillRowMethodName = "BigIntegerCollFillRow")]
    public static IEnumerable BigIntegerCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new BinaryStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void BigIntegerCollFillRow(object obj, out SqlInt32 Index, out SqlBigInteger Value)
    {
      Tuple<int, BigInteger?> tuple = (Tuple<int, BigInteger?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlBigInteger.Null;
    }

    #endregion
    #region SqlComplex collection methods

    [SqlFunction(Name = "cxCollCreate", IsDeterministic = true)]
    public static SqlBytes ComplexCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulComplexStreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "cxCollParse", IsDeterministic = true)]
    public static SqlBytes ComplexCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.Value)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<Complex?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlComplex.Parse(t).Value : default(Complex?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulComplexStreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll);
      return result;
    }

    [SqlFunction(Name = "cxCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ComplexCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlComplex(t.Value) : SqlComplex.Null).ToString());
    }

    [SqlFunction(Name = "cxCollCount", IsDeterministic = true)]
    public static SqlInt32 ComplexCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "cxCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 ComplexCollIndexOf(SqlBytes coll, SqlComplex value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Complex?) : value.Value);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "cxCollGet", IsDeterministic = true)]
    public static SqlComplex ComplexCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlComplex.Null;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? v.Value : SqlComplex.Null;
      }
    }

    [SqlFunction(Name = "cxCollSet", IsDeterministic = true)]
    public static SqlBytes ComplexCollSet(SqlBytes coll, SqlInt32 index, SqlComplex value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value : default(Complex?);
      return coll;
    }

    [SqlFunction(Name = "cxCollInsert", IsDeterministic = true)]
    public static SqlBytes ComplexCollInsert(SqlBytes coll, SqlInt32 index, SqlComplex value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value : default(Complex?));
      }
      return coll;
    }

    [SqlFunction(Name = "cxCollRemove", IsDeterministic = true)]
    public static SqlBytes ComplexCollRemove(SqlBytes coll, SqlComplex value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value : default(Complex?));
      return coll;
    }

    [SqlFunction(Name = "cxCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes ComplexCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "cxCollClear", IsDeterministic = true)]
    public static SqlBytes ComplexCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "cxCollGetRange", IsDeterministic = true)]
    public static SqlBytes ComplexCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulComplexStreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "cxCollInsertRange", IsDeterministic = true)]
    public static SqlBytes ComplexCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulComplexStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "cxCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes ComplexCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlComplex value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Complex?) : value.Value, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "cxCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes ComplexCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "cxCollSetRange", IsDeterministic = true)]
    public static SqlBytes ComplexCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
      using (var rc = new NulComplexStreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "cxCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes ComplexCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlComplex value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Complex?) : value.Value, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "cxCollToArray", IsDeterministic = true)]
    public static SqlBytes ComplexCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulComplexStreamedCollection(array.Stream, true, false))
      using (new NulComplexStreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "cxCollEnumerate", IsDeterministic = true, FillRowMethodName = "ComplexCollFillRow")]
    public static IEnumerable ComplexCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulComplexStreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void ComplexCollFillRow(object obj, out SqlInt32 Index, out SqlComplex Value)
    {
      Tuple<int, Complex?> tuple = (Tuple<int, Complex?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? tuple.Item2.Value : SqlComplex.Null;
    }

    #endregion
    #region SqlHourAngle collection methods

    [SqlFunction(Name = "haCollCreate", IsDeterministic = true)]
    public static SqlBytes HourAngleCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "haCollParse", IsDeterministic = true)]
    public static SqlBytes HourAngleCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<HourAngle?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlHourAngle.Parse(t).Value : default(HourAngle?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulInt32StreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll.Select(t => t.HasValue ? t.Value.Units : default(Int32?)));
      return result;
    }

    [SqlFunction(Name = "haCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString HourAngleCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlHourAngle(new HourAngle(t.Value)) : SqlHourAngle.Null).ToString());
    }

    [SqlFunction(Name = "haCollCount", IsDeterministic = true)]
    public static SqlInt32 HourAngleCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "haCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 HourAngleCollIndexOf(SqlBytes coll, SqlHourAngle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Int32?) : value.Value.Units);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "haCollGet", IsDeterministic = true)]
    public static SqlHourAngle HourAngleCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlHourAngle.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? new HourAngle(v.Value) : SqlHourAngle.Null;
      }
    }

    [SqlFunction(Name = "haCollSet", IsDeterministic = true)]
    public static SqlBytes HourAngleCollSet(SqlBytes coll, SqlInt32 index, SqlHourAngle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value.Units : default(Int32?);
      return coll;
    }

    [SqlFunction(Name = "haCollInsert", IsDeterministic = true)]
    public static SqlBytes HourAngleCollInsert(SqlBytes coll, SqlInt32 index, SqlHourAngle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value.Units : default(Int32?));
      }
      return coll;
    }

    [SqlFunction(Name = "haCollRemove", IsDeterministic = true)]
    public static SqlBytes HourAngleCollRemove(SqlBytes coll, SqlHourAngle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value.Units : default(Int32?));
      return coll;
    }

    [SqlFunction(Name = "haCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes HourAngleCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "haCollClear", IsDeterministic = true)]
    public static SqlBytes HourAngleCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "haCollGetRange", IsDeterministic = true)]
    public static SqlBytes HourAngleCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulInt32StreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "haCollInsertRange", IsDeterministic = true)]
    public static SqlBytes HourAngleCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      using (var rc = new NulInt32StreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "haCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes HourAngleCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlHourAngle value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value.Units, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "haCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes HourAngleCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "haCollSetRange", IsDeterministic = true)]
    public static SqlBytes HourAngleCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      using (var rc = new NulInt32StreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "haCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes HourAngleCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlHourAngle value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value.Units, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "haCollToArray", IsDeterministic = true)]
    public static SqlBytes HourAngleCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulInt32StreamedCollection(array.Stream, true, false))
      using (new NulInt32StreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "haCollEnumerate", IsDeterministic = true, FillRowMethodName = "HourAngleCollFillRow")]
    public static IEnumerable HourAngleCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void HourAngleCollFillRow(object obj, out SqlInt32 Index, out SqlHourAngle Value)
    {
      Tuple<int, Int32?> tuple = (Tuple<int, Int32?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? new HourAngle(tuple.Item2.Value) : SqlHourAngle.Null;
    }

    #endregion
    #region SqlGradAngle collection methods

    [SqlFunction(Name = "gaCollCreate", IsDeterministic = true)]
    public static SqlBytes GradAngleCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "gaCollParse", IsDeterministic = true)]
    public static SqlBytes GradAngleCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<GradAngle?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlGradAngle.Parse(t).Value : default(GradAngle?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulInt32StreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll.Select(t => t.HasValue ? t.Value.Units : default(Int32?)));
      return result;
    }

    [SqlFunction(Name = "gaCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GradAngleCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlGradAngle(new GradAngle(t.Value)) : SqlGradAngle.Null).ToString());
    }

    [SqlFunction(Name = "gaCollCount", IsDeterministic = true)]
    public static SqlInt32 GradAngleCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "gaCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 GradAngleCollIndexOf(SqlBytes coll, SqlGradAngle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Int32?) : value.Value.Units);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "gaCollGet", IsDeterministic = true)]
    public static SqlGradAngle GradAngleCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlGradAngle.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? new GradAngle(v.Value) : SqlGradAngle.Null;
      }
    }

    [SqlFunction(Name = "gaCollSet", IsDeterministic = true)]
    public static SqlBytes GradAngleCollSet(SqlBytes coll, SqlInt32 index, SqlGradAngle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value.Units : default(Int32?);
      return coll;
    }

    [SqlFunction(Name = "gaCollInsert", IsDeterministic = true)]
    public static SqlBytes GradAngleCollInsert(SqlBytes coll, SqlInt32 index, SqlGradAngle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value.Units : default(Int32?));
      }
      return coll;
    }

    [SqlFunction(Name = "gaCollRemove", IsDeterministic = true)]
    public static SqlBytes GradAngleCollRemove(SqlBytes coll, SqlGradAngle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value.Units : default(Int32?));
      return coll;
    }

    [SqlFunction(Name = "gaCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes GradAngleCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "gaCollClear", IsDeterministic = true)]
    public static SqlBytes GradAngleCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "gaCollGetRange", IsDeterministic = true)]
    public static SqlBytes GradAngleCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulInt32StreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "gaCollInsertRange", IsDeterministic = true)]
    public static SqlBytes GradAngleCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      using (var rc = new NulInt32StreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "gaCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes GradAngleCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlGradAngle value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value.Units, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "gaCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes GradAngleCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "gaCollSetRange", IsDeterministic = true)]
    public static SqlBytes GradAngleCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      using (var rc = new NulInt32StreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "gaCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes GradAngleCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlGradAngle value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value.Units, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "gaCollToArray", IsDeterministic = true)]
    public static SqlBytes GradAngleCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulInt32StreamedCollection(array.Stream, true, false))
      using (new NulInt32StreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "gaCollEnumerate", IsDeterministic = true, FillRowMethodName = "GradAngleCollFillRow")]
    public static IEnumerable GradAngleCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void GradAngleCollFillRow(object obj, out SqlInt32 Index, out SqlGradAngle Value)
    {
      Tuple<int, Int32?> tuple = (Tuple<int, Int32?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? new GradAngle(tuple.Item2.Value) : SqlGradAngle.Null;
    }

    #endregion
    #region SqlSexagesimalAngle collection methods

    [SqlFunction(Name = "saCollCreate", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollCreate(
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedCollection(coll.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false)) ;
      return coll;
    }

    [SqlFunction(Name = "saCollParse", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      [SqlFacet(IsNullable = false)][DefaultValue(SizeEncoding.B4)]SqlByte countSizing,
      [SqlFacet(IsNullable = false)][DefaultValue(true)]SqlBoolean compact)
    {
      if (str.IsNull || countSizing.IsNull || compact.IsNull)
        return SqlBytes.Null;

      var coll = SqlFormatting.ParseCollection<SexagesimalAngle?>(str.Value, t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlSexagesimalAngle.Parse(t).Value : default(SexagesimalAngle?));
      var result = new SqlBytes(new MemoryStream());
      using (var sc = new NulInt32StreamedCollection(result.Stream, (SizeEncoding)countSizing.Value, compact.Value, true, false))
        sc.AddRange(coll.Select(t => t.HasValue ? t.Value.Units : default(Int32?)));
      return result;
    }

    [SqlFunction(Name = "saCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString SexagesimalAngleCollFormat(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlFormatting.NullText;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        return SqlFormatting.Format(sc, t => (t.HasValue ? new SqlSexagesimalAngle(new SexagesimalAngle(t.Value)) : SqlSexagesimalAngle.Null).ToString());
    }

    [SqlFunction(Name = "saCollCount", IsDeterministic = true)]
    public static SqlInt32 SexagesimalAngleCollCount(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        return sc.Count;
    }

    [SqlFunction(Name = "saCollIndexOf", IsDeterministic = true)]
    public static SqlInt32 SexagesimalAngleCollIndexOf(SqlBytes coll, SqlSexagesimalAngle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        var index = sc.IndexOf(value.IsNull ? default(Int32?) : value.Value.Units);
        return index < 0 ? SqlInt32.Null : index;
      }
    }

    [SqlFunction(Name = "saCollGet", IsDeterministic = true)]
    public static SqlSexagesimalAngle SexagesimalAngleCollGet(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return SqlSexagesimalAngle.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        var v = sc[index.Value];
        return v.HasValue ? new SexagesimalAngle(v.Value) : SqlSexagesimalAngle.Null;
      }
    }

    [SqlFunction(Name = "saCollSet", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollSet(SqlBytes coll, SqlInt32 index, SqlSexagesimalAngle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc[index.Value] = !value.IsNull ? value.Value.Units : default(Int32?);
      return coll;
    }

    [SqlFunction(Name = "saCollInsert", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollInsert(SqlBytes coll, SqlInt32 index, SqlSexagesimalAngle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.Insert(indexValue, !value.IsNull ? value.Value.Units : default(Int32?));
      }
      return coll;
    }

    [SqlFunction(Name = "saCollRemove", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollRemove(SqlBytes coll, SqlSexagesimalAngle value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc.Remove(!value.IsNull ? value.Value.Units : default(Int32?));
      return coll;
    }

    [SqlFunction(Name = "saCollRemoveAt", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollRemoveAt(SqlBytes coll, SqlInt32 index)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || index.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc.RemoveAt(index.Value);
      return coll;
    }

    [SqlFunction(Name = "saCollClear", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollClear(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
        sc.Clear();
      return coll;
    }

    [SqlFunction(Name = "saCollGetRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollGetRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        var result = new SqlBytes(new MemoryStream());
        using (var rc = new NulInt32StreamedCollection(result.Stream, sc.CountSizing, sc.Compact, true, false))
          rc.AddRange(sc.EnumerateRange(indexValue, countValue));
        return result;
      }
    }

    [SqlFunction(Name = "saCollInsertRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollInsertRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      using (var rc = new NulInt32StreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "saCollInsertRepeat", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollInsertRepeat(SqlBytes coll, SqlInt32 index, SqlSexagesimalAngle value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || count.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.InsertRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value.Units, count.Value);
      }
      return coll;
    }

    [SqlFunction(Name = "saCollRemoveRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollRemoveRange(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.RemoveRange(indexValue, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "saCollSetRange", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollSetRange(SqlBytes coll, SqlInt32 index, SqlBytes range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (coll.IsNull || range.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      using (var rc = new NulInt32StreamedCollection(range.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count : index.Value;
        sc.SetRange(indexValue, rc);
      }
      return coll;
    }

    [SqlFunction(Name = "saCollSetRepeat", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollSetRepeat(SqlBytes coll, SqlInt32 index, SqlSexagesimalAngle value, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return coll;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        sc.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value.Units, countValue);
      }
      return coll;
    }

    [SqlFunction(Name = "saCollToArray", IsDeterministic = true)]
    public static SqlBytes SexagesimalAngleCollToArray(SqlBytes coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlBytes.Null;

      var array = new SqlBytes(new MemoryStream());
      using (var sc = new NulInt32StreamedCollection(array.Stream, true, false))
      using (new NulInt32StreamedCollection(coll.Stream, sc.CountSizing, sc.Compact, sc, true, false)) ;
      return array;
    }

    [SqlFunction(Name = "saCollEnumerate", IsDeterministic = true, FillRowMethodName = "SexagesimalAngleCollFillRow")]
    public static IEnumerable SexagesimalAngleCollEnumerate(SqlBytes coll, SqlInt32 index, SqlInt32 count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;

      using (var sc = new NulInt32StreamedCollection(coll.Stream, true, false))
      {
        int indexValue = index.IsNull ? sc.Count - (count.IsNull ? sc.Count : count.Value) : index.Value;
        int countValue = count.IsNull ? sc.Count - (index.IsNull ? 0 : index.Value) : count.Value;
        foreach (var item in sc.EnumerateRange(indexValue, countValue))
          yield return Tuple.Create(indexValue++, item);
      }
    }

    private static void SexagesimalAngleCollFillRow(object obj, out SqlInt32 Index, out SqlSexagesimalAngle Value)
    {
      Tuple<int, Int32?> tuple = (Tuple<int, Int32?>)obj;
      Index = tuple.Item1;
      Value = tuple.Item2.HasValue ? new SexagesimalAngle(tuple.Item2.Value) : SqlSexagesimalAngle.Null;
    }

    #endregion
  }
}
