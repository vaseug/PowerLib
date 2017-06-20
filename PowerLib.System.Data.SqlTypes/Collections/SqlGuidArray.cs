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
  [SqlUserDefinedType(Format.UserDefined, Name = "UidArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlGuidArray : INullable, IBinarySerialize
  {
    private Guid?[] _array;

    #region Contructors

    public SqlGuidArray()
    {
      _array = null;
    }

    public SqlGuidArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new Guid?[length];
    }

    public SqlGuidArray(IEnumerable<Guid?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlGuidArray(Guid?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public Guid?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlGuidArray Null
    {
      get { return new SqlGuidArray(); }
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

    public static SqlGuidArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlGuidArray(SqlFormatting.ParseArray<Guid?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlGuid.Parse(t).Value : default(Guid?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, (v, i) => (v.HasValue ? new SqlGuid(v.Value) : SqlGuid.Null).ToString());
    }

    [SqlMethod]
    public SqlGuid GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlGuid.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlGuid value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(Guid?);
    }

    [SqlMethod]
    public SqlGuidArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlGuidArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlGuidArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlGuid value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(Guid?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlGuidCollection ToCollection()
    {
      return new SqlGuidCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlGuidArray array)
    {
      using (var ms = new MemoryStream())
      using (new NulGuidStreamedArray(ms, SizeEncoding.B4, true, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlGuidArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulGuidStreamedArray(ms, true, false))
        return new SqlGuidArray(sa.ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulGuidStreamedArray(rd.BaseStream, true, false))
        _array = sa.ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulGuidStreamedArray(ms, SizeEncoding.B4, true, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
