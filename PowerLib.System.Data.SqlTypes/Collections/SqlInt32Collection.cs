using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.IO;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "IntCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlInt32Collection : INullable, IBinarySerialize
  {
    private List<Int32?> _list;

    #region Contructors

    public SqlInt32Collection()
    {
      _list = null;
    }

    public SqlInt32Collection(IEnumerable<Int32?> coll)
    {
      _list = coll != null ? new List<Int32?>(coll) : null;
    }

    private SqlInt32Collection(List<Int32?> list)
    {
      _list = list;
    }

    #endregion
    #region Properties

    public List<Int32?> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlInt32Collection Null
    {
      get { return new SqlInt32Collection(); }
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

    public static SqlInt32Collection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlInt32Collection(SqlFormatting.ParseCollection<Int32?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt32.Parse(t).Value : default(Int32?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, t => (t.HasValue ? new SqlInt32(t.Value) : SqlInt32.Null).ToString());
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlInt32 value)
    {
      _list.Add(value.IsNull ? default(Int32?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlInt32 value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(Int32?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlInt32 value)
    {
      _list.Remove(value.IsNull ? default(Int32?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      if (index.IsNull)
        return;

      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlInt32 value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(Int32?) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlInt32Collection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlInt32 value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(Int32?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlInt32Collection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlInt32 value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlInt32Collection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlInt32 value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(Int32?) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlInt32 GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value].HasValue ? _list[index.Value].Value : SqlInt32.Null;
    }

    [SqlMethod]
    public SqlInt32Collection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlInt32Collection(_list.GetRange(indexValue, countValue));
    }

    [SqlMethod]
    public SqlInt32Array ToArray()
    {
      return new SqlInt32Array(_list);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlInt32Collection coll)
    {
      using (var ms = new MemoryStream())
      using (new NulInt32StreamedCollection(ms, SizeEncoding.B4, true, coll._list, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlInt32Collection(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sc = new NulInt32StreamedCollection(ms, true, false))
        return new SqlInt32Collection(sc.ToList());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sc = new NulInt32StreamedCollection(rd.BaseStream, true, false))
        _list = sc.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sc = new NulInt32StreamedCollection(ms, SizeEncoding.B4, true, _list, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
