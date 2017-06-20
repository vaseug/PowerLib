using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "BigIntArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlInt64Array : INullable, IBinarySerialize
  {
    private Int64?[] _array;

    #region Contructors

    public SqlInt64Array()
    {
      _array = null;
    }

    public SqlInt64Array(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new Int64?[length];
    }

    public SqlInt64Array(IEnumerable<Int64?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlInt64Array(Int64?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public Int64?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlInt64Array Null
    {
      get { return new SqlInt64Array(); }
    }

    public bool IsNull
    {
      get { return _array == null; }
    }

    public SqlInt32 Length
    {
      get { return _array != null ? _array.Length : SqlInt32.Null; }
    }

    #endregion
    #region Methods

    public static SqlInt64Array Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlInt64Array(SqlFormatting.ParseArray<Int64?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt64.Parse(t).Value : default(Int64?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlInt64(t.Value) : SqlInt64.Null).ToString());
    }

    [SqlMethod]
    public SqlInt64 GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlInt64.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt64 value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(Int64?);
    }

    [SqlMethod]
    public SqlInt64Array GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlInt64Array(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt64Array array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlInt64 value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(Int64?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlInt64Collection ToCollection()
    {
      return new SqlInt64Collection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlInt64Array array)
    {
      using (var ms = new MemoryStream())
      using (new NulInt64StreamedArray(ms, SizeEncoding.B4, true, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlInt64Array(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulInt64StreamedArray(ms, true, false))
        return new SqlInt64Array(sa.ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulInt64StreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulInt64StreamedArray(ms, SizeEncoding.B4, true, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
