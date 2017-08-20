using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.Collections;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "BigRangeCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlLongRangeCollection : INullable, IBinarySerialize
  {
    private readonly SqlLongRangeCollection @null = new SqlLongRangeCollection();

    private List<LongRange?> _list;

    #region Contructors

    public SqlLongRangeCollection()
    {
      _list = null;
    }

    public SqlLongRangeCollection(IEnumerable<LongRange?> coll)
    {
      _list = coll != null ? new List<LongRange?>(coll) : null;
    }

    private SqlLongRangeCollection(List<LongRange?> list)
    {
      _list = list;
    }

    #endregion
    #region Properties

    public List<LongRange?> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlLongRangeCollection Null
    {
      get { return new SqlLongRangeCollection(); }
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

    public static SqlLongRangeCollection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlLongRangeCollection(SqlFormatting.ParseCollection<LongRange?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlLongRange.Parse(t).Value : default(LongRange?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, t => (t.HasValue ? new SqlLongRange(t.Value) : SqlLongRange.Null).ToString());
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlLongRange value)
    {
      _list.Add(value.IsNull ? default(LongRange?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlLongRange value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(LongRange?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlLongRange value)
    {
      _list.Remove(value.IsNull ? default(LongRange?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      if (index.IsNull)
        return;

      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlLongRange value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(LongRange?) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlLongRangeCollection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlLongRange value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(LongRange?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlLongRangeCollection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlLongRange value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(LongRange?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlLongRangeCollection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlLongRange value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(LongRange?) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlLongRange GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value].HasValue ? _list[index.Value].Value : SqlLongRange.Null;
    }

    [SqlMethod]
    public SqlLongRangeCollection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlLongRangeCollection(_list.GetRange(indexValue, countValue));
    }

    [SqlMethod]
    public SqlLongRangeArray ToArray()
    {
      return new SqlLongRangeArray(_list);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlLongRangeCollection coll)
    {
      using (var ms = new MemoryStream())
      using (new NulLongRangeStreamedCollection(ms, SizeEncoding.B4, true, coll._list, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlLongRangeCollection(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sc = new NulLongRangeStreamedCollection(ms, true, false))
        return new SqlLongRangeCollection(sc.ToList());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sc = new NulLongRangeStreamedCollection(rd.BaseStream, true, false))
        _list = sc.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sc = new NulLongRangeStreamedCollection(ms, SizeEncoding.B4, true, _list, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
