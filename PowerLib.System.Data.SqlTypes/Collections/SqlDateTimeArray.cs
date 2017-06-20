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
  [SqlUserDefinedType(Format.UserDefined, Name = "DateTimeArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlDateTimeArray : INullable, IBinarySerialize
  {
    private DateTime?[] _array;

    #region Contructors

    public SqlDateTimeArray()
    {
      _array = null;
    }

    public SqlDateTimeArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new DateTime?[length];
    }

    public SqlDateTimeArray(IEnumerable<DateTime?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlDateTimeArray(DateTime?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public DateTime?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlDateTimeArray Null
    {
      get { return new SqlDateTimeArray(); }
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

    public static SqlDateTimeArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlDateTimeArray(SqlFormatting.ParseArray<DateTime?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlDateTime.Parse(t).Value : default(DateTime?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlDateTime(t.Value) : SqlDateTime.Null).ToString());
    }

    [SqlMethod]
    public SqlDateTime GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlDateTime.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlDateTime value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(DateTime?);
    }

    [SqlMethod]
    public SqlDateTimeArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlDateTimeArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlDateTimeArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlDateTime value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(DateTime?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlDateTimeCollection ToCollection()
    {
      return new SqlDateTimeCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlDateTimeArray array)
    {
      using (var ms = new MemoryStream())
      using (new NulDateTimeStreamedArray(ms, SizeEncoding.B4, true, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlDateTimeArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulDateTimeStreamedArray(ms, true, false))
        return new SqlDateTimeArray(sa.ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulDateTimeStreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulDateTimeStreamedArray(ms, SizeEncoding.B4, true, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
