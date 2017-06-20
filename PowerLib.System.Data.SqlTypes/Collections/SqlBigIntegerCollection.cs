using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Numerics;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.IO;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "HugeIntCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlBigIntegerCollection : INullable, IBinarySerialize
  {
    private List<BigInteger?> _list;

    #region Contructors

    public SqlBigIntegerCollection()
    {
      _list = null;
    }

    public SqlBigIntegerCollection(IEnumerable<BigInteger?> coll)
    {
      _list = coll != null ? new List<BigInteger?>(coll) : null;
    }

    private SqlBigIntegerCollection(List<BigInteger?> list)
    {
      _list = list;
    }

    #endregion
    #region Properties

    public List<BigInteger?> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlBigIntegerCollection Null
    {
      get { return new SqlBigIntegerCollection(); }
    }

    public bool IsNull
    {
      get { return _list == null; }
    }

    public SqlInt32 Count
    {
      get { return _list != null ? _list.Count : SqlInt32.Null; }
    }

    #endregion
    #region Methods

    public static SqlBigIntegerCollection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlBigIntegerCollection(SqlFormatting.ParseCollection<BigInteger?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlBigInteger.Parse(t).Value : default(BigInteger?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, t => (t.HasValue ? new SqlBigInteger(t.Value) : SqlBigInteger.Null).ToString());
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlBigInteger value)
    {
      _list.Add(value.IsNull ? default(BigInteger?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlBigInteger value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(BigInteger?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlBigInteger value)
    {
      _list.Remove(value.IsNull ? default(BigInteger?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      if (index.IsNull)
        return;

      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlBigInteger value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(BigInteger?) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlBigIntegerCollection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlBigInteger value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(BigInteger?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlBigIntegerCollection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlBigInteger value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(BigInteger?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlBigIntegerCollection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlBigInteger value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(BigInteger?) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlBigInteger GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value].HasValue ? _list[index.Value].Value : SqlBigInteger.Null;
    }

    [SqlMethod]
    public SqlBigIntegerCollection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlBigIntegerCollection(_list.GetRange(indexValue, countValue));
    }

    [SqlMethod]
    public SqlBigIntegerArray ToArray()
    {
      return new SqlBigIntegerArray(_list);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlBigIntegerCollection coll)
    {
      using (var ms = new MemoryStream())
      using (new BinaryStreamedCollection(ms, SizeEncoding.B4, SizeEncoding.VB, coll._list.Select(t => t.HasValue ? t.Value.ToByteArray() : default(byte[])).Counted(coll._list.Count), true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlBigIntegerCollection(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sc = new BinaryStreamedCollection(ms, true, false))
        return new SqlBigIntegerCollection(sc.Select(t => t != null ? new BigInteger(t) : default(BigInteger?)).ToList());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new BinaryStreamedCollection(rd.BaseStream, true, false))
        _list = sa.Select(t => t == null ? default(BigInteger?) : new BigInteger(t)).ToList();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sc = new BinaryStreamedCollection(ms, SizeEncoding.B4, SizeEncoding.VB, _list.Select(t => t.HasValue ? t.Value.ToByteArray() : default(byte[])).Counted(_list.Count), true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
