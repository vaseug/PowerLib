using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Numerics;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Numerics;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "ComplexArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlComplexArray : INullable, IBinarySerialize
  {
    private Complex?[] _array;

    #region Contructors

    public SqlComplexArray()
    {
      _array = null;
    }

    public SqlComplexArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new Complex?[length];
    }

    public SqlComplexArray(IEnumerable<Complex?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlComplexArray(Complex?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public Complex?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlComplexArray Null
    {
      get { return new SqlComplexArray(); }
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

    public static SqlComplexArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlComplexArray(SqlFormatting.ParseArray<Complex?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlComplex.Parse(t).Value : default(Complex?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlComplex(t.Value) : SqlComplex.Null).ToString());
    }

    [SqlMethod]
    public SqlComplex GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlComplex.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlComplex value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(Complex?);
    }

    [SqlMethod]
    public SqlComplexArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlComplexArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlComplexArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlComplex value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(Complex?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlComplexCollection ToCollection()
    {
      return new SqlComplexCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlComplexArray array)
    {
      using (var ms = new MemoryStream())
      using (new NulComplexStreamedArray(ms, SizeEncoding.B4, true, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlComplexArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulComplexStreamedArray(ms, true, false))
        return new SqlComplexArray(sa.ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulComplexStreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulComplexStreamedArray(ms, SizeEncoding.B4, true, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
