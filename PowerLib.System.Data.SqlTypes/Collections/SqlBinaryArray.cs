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
  [SqlUserDefinedType(Format.UserDefined, Name = "BinaryArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlBinaryArray : INullable, IBinarySerialize
  {
    private Byte[][] _array;

    #region Contructors

    public SqlBinaryArray()
    {
      _array = null;
    }

    public SqlBinaryArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new byte[length][];
    }

    public SqlBinaryArray(IEnumerable<byte[]> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlBinaryArray(byte[][] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public byte[][] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlBinaryArray Null
    {
      get { return new SqlBinaryArray(); }
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

    public static SqlBinaryArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlBinaryArray(SqlFormatting.ParseArray<Byte[]>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? PwrBitConverter.ParseBinary(t, true) : null));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t != null ? PwrBitConverter.Format(t, true) : SqlBinary.Null.ToString()));
    }

    [SqlMethod]
    public SqlBinary GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value] != null ? _array[index.Value] : SqlBinary.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlBinary value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : null;
    }

    [SqlMethod]
    public SqlBinaryArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlBinaryArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlBinaryArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlBinary value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : null, indexValue, countValue);
    }

    [SqlMethod]
    public SqlBinaryCollection ToCollection()
    {
      return new SqlBinaryCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlBinaryArray array)
    {
      using (var ms = new MemoryStream())
      using (new BinaryStreamedArray(ms, SizeEncoding.B4, SizeEncoding.B4, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlBinaryArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new BinaryStreamedArray(ms, true, false))
        return new SqlBinaryArray(sa.ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new BinaryStreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new BinaryStreamedArray(ms, SizeEncoding.B4, SizeEncoding.B4, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
