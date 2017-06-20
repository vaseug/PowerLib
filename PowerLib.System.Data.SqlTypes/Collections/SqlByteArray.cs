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
  [SqlUserDefinedType(Format.UserDefined, Name = "TinyIntArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlByteArray : INullable, IBinarySerialize
  {
    private Byte?[] _array;

    #region Contructors

    public SqlByteArray()
    {
      _array = null;
    }

    public SqlByteArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new Byte?[length];
    }

    public SqlByteArray(IEnumerable<Byte?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlByteArray(Byte?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public Byte?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlByteArray Null
    {
      get { return new SqlByteArray(); }
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

    public static SqlByteArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlByteArray(SqlFormatting.ParseArray<Byte?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlByte.Parse(t).Value : default(Byte?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlByte(t.Value) : SqlByte.Null).ToString());
    }

    [SqlMethod]
    public SqlByte GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlByte.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlByte value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(Byte?);
    }

    [SqlMethod]
    public SqlByteArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlByteArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlByteArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlByte value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(Byte?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlByteCollection ToCollection()
    {
      return new SqlByteCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlByteArray array)
    {
      using (var ms = new MemoryStream())
      using (new NulByteStreamedArray(ms, SizeEncoding.B4, true, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlByteArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulByteStreamedArray(ms, true, false))
        return new SqlByteArray(sa.ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulByteStreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulByteStreamedArray(ms, SizeEncoding.B4, true, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
