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
  [SqlUserDefinedType(Format.UserDefined, Name = "DoubleFloatArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlDoubleArray : INullable, IBinarySerialize
  {
    private Double?[] _array;

    #region Contructors

    public SqlDoubleArray()
    {
      _array = null;
    }

    public SqlDoubleArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new Double?[length];
    }

    public SqlDoubleArray(IEnumerable<Double?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlDoubleArray(Double?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public Double?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlDoubleArray Null
    {
      get { return new SqlDoubleArray(); }
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

    public static SqlDoubleArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlDoubleArray(SqlFormatting.ParseArray<Double?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlDouble.Parse(t).Value : default(Double?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlDouble(t.Value) : SqlDouble.Null).ToString());
    }

    [SqlMethod]
    public SqlDouble GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlDouble.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlDouble value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(Double?);
    }

    [SqlMethod]
    public SqlDoubleArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlDoubleArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlDoubleArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlDouble value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(Double?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlDoubleCollection ToCollection()
    {
      return new SqlDoubleCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlDoubleArray array)
    {
      using (MemoryStream ms = new MemoryStream())
      using (BinaryWriter wr = new BinaryWriter(ms))
      {
        array.Write(wr);
        return ms.ToArray();
      }
    }

    public static explicit operator SqlDoubleArray(byte[] buffer)
    {
      SqlDoubleArray array = new SqlDoubleArray();
      if (buffer != null)
        using (MemoryStream ms = new MemoryStream(buffer))
        using (BinaryReader rd = new BinaryReader(ms))
          array.Read(rd);
      return array;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulDoubleStreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulDoubleStreamedArray(ms, SizeEncoding.B4, true, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
