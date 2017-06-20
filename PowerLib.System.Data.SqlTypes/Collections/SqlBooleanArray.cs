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
  [SqlUserDefinedType(Format.UserDefined, Name = "BitArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlBooleanArray : INullable, IBinarySerialize
  {
    private Boolean?[] _array;

    #region Contructors

    public SqlBooleanArray()
    {
      _array = null;
    }

    public SqlBooleanArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new Boolean?[length];
    }

    public SqlBooleanArray(IEnumerable<Boolean?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlBooleanArray(Boolean?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public Boolean?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlBooleanArray Null
    {
      get { return new SqlBooleanArray(); }
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

    public static SqlBooleanArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlBooleanArray(SqlFormatting.ParseArray<Boolean?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlBoolean.Parse(t).Value : default(Boolean?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlBoolean(t.Value) : SqlBoolean.Null).ToString());
    }

    [SqlMethod]
    public SqlBoolean GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlBoolean.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlBoolean value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(Boolean?);
    }

    [SqlMethod]
    public SqlBooleanArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlBooleanArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlBooleanArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(t => array._array[- indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlBoolean value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(Boolean?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlBooleanCollection ToCollection()
    {
      return new SqlBooleanCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlBooleanArray array)
    {
      using (var ms = new MemoryStream())
      using (new NulBooleanStreamedArray(ms, SizeEncoding.B4, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlBooleanArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulBooleanStreamedArray(ms, true, false))
        return new SqlBooleanArray(sa.ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulBooleanStreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulBooleanStreamedArray(ms, SizeEncoding.B4, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
