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
  [SqlUserDefinedType(Format.UserDefined, Name = "SmallIntArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlInt16Array : INullable, IBinarySerialize
  {
    private Int16?[] _array;

    #region Contructors

    public SqlInt16Array()
    {
      _array = null;
    }

    public SqlInt16Array(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new Int16?[length];
    }

    public SqlInt16Array(IEnumerable<Int16?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlInt16Array(Int16?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public Int16?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlInt16Array Null
    {
      get { return new SqlInt16Array(); }
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

    public static SqlInt16Array Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlInt16Array(SqlFormatting.ParseArray<Int16?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt16.Parse(t).Value : default(Int16?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlInt16(t.Value) : SqlInt16.Null).ToString());
    }

    [SqlMethod]
    public SqlInt16 GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? new SqlInt16(_array[index.Value].Value) : SqlInt16.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt16 value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(Int16?);
    }

    [SqlMethod]
    public SqlInt16Array GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlInt16Array(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt16Array array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlInt16 value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(Int16?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlInt16Collection ToCollection()
    {
      return new SqlInt16Collection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlInt16Array array)
    {
      using (var ms = new MemoryStream())
      using (new NulInt16StreamedArray(ms, SizeEncoding.B4, true, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlInt16Array(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulInt16StreamedArray(ms, true, false))
        return new SqlInt16Array(sa.ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulInt16StreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulInt16StreamedArray(ms, SizeEncoding.B4, true, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
