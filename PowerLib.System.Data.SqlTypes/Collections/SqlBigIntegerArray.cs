using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Numerics;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "HugeIntArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlBigIntegerArray : INullable, IBinarySerialize
  {
    private BigInteger?[] _array;

    #region Contructors

    public SqlBigIntegerArray()
    {
      _array = null;
    }

    public SqlBigIntegerArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new BigInteger?[length];
    }

    public SqlBigIntegerArray(IEnumerable<BigInteger?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlBigIntegerArray(BigInteger?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public BigInteger?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlBigIntegerArray Null
    {
      get { return new SqlBigIntegerArray(); }
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

    public static SqlBigIntegerArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlBigIntegerArray(SqlFormatting.ParseArray<BigInteger?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlBigInteger.Parse(t).Value : default(BigInteger?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlBigInteger(t.Value) : SqlBigInteger.Null).ToString());
    }

    [SqlMethod]
    public SqlBigInteger GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlBigInteger.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlBigInteger value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(BigInteger?);
    }

    [SqlMethod]
    public SqlBigIntegerArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlBigIntegerArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlBigIntegerArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlBigInteger value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(BigInteger?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlBigIntegerCollection ToCollection()
    {
      return new SqlBigIntegerCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlBigIntegerArray array)
    {
      using (var ms = new MemoryStream())
      using (new BinaryStreamedArray(ms, SizeEncoding.B4, SizeEncoding.B4, array._array.Select(t => t.HasValue ? t.Value.ToByteArray() : default(byte[])).Counted(array._array.Length), true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlBigIntegerArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new BinaryStreamedArray(ms, true, false))
        return new SqlBigIntegerArray(sa.Select(t => t != null ? new BigInteger(t) : default(BigInteger?)).ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new BinaryStreamedArray(rd.BaseStream, true, false))
        _array = sa.Select(t => t != null ? new BigInteger(t) : default(BigInteger?)).ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new BinaryStreamedArray(ms, SizeEncoding.B4, SizeEncoding.B4, _array.Select(t => t.HasValue ? t.Value.ToByteArray() : default(byte[])).Counted(_array.Length), true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
