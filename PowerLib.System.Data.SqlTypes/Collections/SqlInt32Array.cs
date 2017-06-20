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
  [SqlUserDefinedType(Format.UserDefined, Name = "IntArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlInt32Array : INullable, IBinarySerialize
  {
    private Int32?[] _array;

    #region Contructors

    public SqlInt32Array()
    {
      _array = null;
    }

    public SqlInt32Array(Int32 length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new Int32?[length];
    }

    public SqlInt32Array(IEnumerable<Int32> coll)
    {
      _array = coll != null ? coll.Select(t => new Nullable<Int32>(t)).ToArray() : null;
    }

    public SqlInt32Array(IEnumerable<Int32?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlInt32Array(Int32?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public Int32?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlInt32Array Null
    {
      get { return new SqlInt32Array(); }
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

    public static SqlInt32Array Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlInt32Array(SqlFormatting.ParseArray<Int32?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt32.Parse(t).Value : default(Int32?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlInt32(t.Value) : SqlInt32.Null).ToString());
    }

    [SqlMethod]
    public SqlInt32 GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlInt32.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(Int32?);
    }

    [SqlMethod]
    public SqlInt32Array GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlInt32Array(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32Array array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[- indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlInt32 value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(Int32?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlInt32Collection ToCollection()
    {
      return new SqlInt32Collection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[](SqlInt32Array array)
    {
      using (var ms = new MemoryStream())
      using (new NulInt32StreamedArray(ms, SizeEncoding.B4, true, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlInt32Array(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulInt32StreamedArray(ms, true, false))
        return new SqlInt32Array(sa.ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulInt32StreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulInt32StreamedArray(ms, SizeEncoding.B4, true, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
