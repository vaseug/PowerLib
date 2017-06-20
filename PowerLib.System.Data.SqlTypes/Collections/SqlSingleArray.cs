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
  [SqlUserDefinedType(Format.UserDefined, Name = "SingleFloatArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlSingleArray : INullable, IBinarySerialize
  {
    private Single?[] _array;

    #region Contructors

    public SqlSingleArray()
    {
      _array = null;
    }

    public SqlSingleArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new Single?[length];
    }

    public SqlSingleArray(IEnumerable<Single?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlSingleArray(Single?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public Single?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlSingleArray Null
    {
      get { return new SqlSingleArray(); }
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

    public static SqlSingleArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlSingleArray(SqlFormatting.ParseArray<Single?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlSingle.Parse(t).Value : default(Single?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlSingle(t.Value) : SqlSingle.Null).ToString());
    }

    [SqlMethod]
    public SqlSingle GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlSingle.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlSingle value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(Single?);
    }

    [SqlMethod]
    public SqlSingleArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlSingleArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlSingleArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlSingle value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(Single?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlSingleCollection ToCollection()
    {
      return new SqlSingleCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlSingleArray array)
    {
      using (var ms = new MemoryStream())
      using (new NulSingleStreamedArray(ms, SizeEncoding.B4, true, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlSingleArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulSingleStreamedArray(ms, true, false))
        return new SqlSingleArray(sa.ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulSingleStreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulSingleStreamedArray(ms, SizeEncoding.B4, true, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
