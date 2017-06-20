using System;
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
  [SqlUserDefinedType(Format.UserDefined, Name = "BigIntCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlInt64Collection : INullable, IBinarySerialize
  {
    private List<Int64?> _list;

    #region Contructors

    public SqlInt64Collection()
    {
      _list = null;
    }

    public SqlInt64Collection(IEnumerable<Int64?> coll)
    {
      _list = coll != null ? new List<Int64?>(coll) : null;
    }

    private SqlInt64Collection(List<Int64?> list)
    {
      _list = list;
    }

    #endregion
    #region Properties

    public List<Int64?> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlInt64Collection Null
    {
      get { return new SqlInt64Collection(); }
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

    public static SqlInt64Collection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlInt64Collection(SqlFormatting.ParseCollection<Int64?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlInt64.Parse(t).Value : default(Int64?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, t => (t.HasValue ? new SqlInt64(t.Value) : SqlInt64.Null).ToString());
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlInt64 value)
    {
      _list.Add(value.IsNull ? default(Int64?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlInt64 value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(Int64?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlInt64 value)
    {
      _list.Remove(value.IsNull ? default(Int64?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      if (index.IsNull)
        return;

      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlInt64 value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(Int64?) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlInt64Collection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlInt64 value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(Int64?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlInt64Collection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlInt64 value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(Int64?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlInt64Collection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlInt64 value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(Int64?) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlInt64 GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value].HasValue ? _list[index.Value].Value : SqlInt64.Null;
    }

    [SqlMethod]
    public SqlInt64Collection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlInt64Collection(_list.GetRange(indexValue, countValue));
    }

    [SqlMethod]
    public SqlInt64Array ToArray()
    {
      return new SqlInt64Array(_list);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlInt64Collection coll)
    {
      using (var ms = new MemoryStream())
      using (new NulInt64StreamedCollection(ms, SizeEncoding.B4, true, coll._list, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlInt64Collection(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sc = new NulInt64StreamedCollection(ms, true, false))
        return new SqlInt64Collection(sc.ToList());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sc = new NulInt64StreamedCollection(rd.BaseStream, true, false))
        _list = sc.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sc = new NulInt64StreamedCollection(ms, SizeEncoding.B4, true, _list, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
