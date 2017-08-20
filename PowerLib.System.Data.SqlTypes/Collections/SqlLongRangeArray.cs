using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "BigRangeArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlLongRangeArray : INullable, IBinarySerialize
  {
    private LongRange?[] _array;

    #region Contructors

    public SqlLongRangeArray()
    {
      _array = null;
    }

    public SqlLongRangeArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new LongRange?[length];
    }

    public SqlLongRangeArray(IEnumerable<LongRange?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlLongRangeArray(LongRange?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public LongRange?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlLongRangeArray Null
    {
      get { return new SqlLongRangeArray(); }
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

    public static SqlLongRangeArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlLongRangeArray(SqlFormatting.ParseArray<LongRange?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlLongRange.Parse(t).Value : default(LongRange?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlLongRange(t.Value) : SqlLongRange.Null).ToString());
    }

    [SqlMethod]
    public SqlLongRange GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlLongRange.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlLongRange value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(LongRange?);
    }

    [SqlMethod]
    public SqlLongRangeArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlLongRangeArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlLongRangeArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlLongRange value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(LongRange?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlLongRangeCollection ToCollection()
    {
      return new SqlLongRangeCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlLongRangeArray array)
    {
      using (var ms = new MemoryStream())
      using (new NulLongRangeStreamedArray(ms, SizeEncoding.B4, true, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlLongRangeArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulLongRangeStreamedArray(ms, true, false))
        return new SqlLongRangeArray(sa.ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulLongRangeStreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulLongRangeStreamedArray(ms, SizeEncoding.B4, true, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
