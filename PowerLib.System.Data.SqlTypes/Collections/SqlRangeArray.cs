using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "RangeArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlRangeArray : INullable, IBinarySerialize
  {
    private Range?[] _array;

    #region Contructors

    public SqlRangeArray()
    {
      _array = null;
    }

    public SqlRangeArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new Range?[length];
    }

    public SqlRangeArray(IEnumerable<Range?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlRangeArray(Range?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public Range?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlRangeArray Null
    {
      get { return new SqlRangeArray(); }
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

    public static SqlRangeArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlRangeArray(SqlFormatting.ParseArray<Range?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlRange.Parse(t).Value : default(Range?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlRange(t.Value) : SqlRange.Null).ToString());
    }

    [SqlMethod]
    public SqlRange GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlRange.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlRange value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(Range?);
    }

    [SqlMethod]
    public SqlRangeArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlRangeArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlRangeArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlRange value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(Range?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlRangeCollection ToCollection()
    {
      return new SqlRangeCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlRangeArray array)
    {
      using (var ms = new MemoryStream())
      using (new NulRangeStreamedArray(ms, SizeEncoding.B4, true, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlRangeArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulRangeStreamedArray(ms, true, false))
        return new SqlRangeArray(sa.ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulRangeStreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulRangeStreamedArray(ms, SizeEncoding.B4, true, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
