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
  [SqlUserDefinedType(Format.UserDefined, Name = "UidCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlGuidCollection : INullable, IBinarySerialize
  {
    private List<Guid?> _list;

    #region Contructors

    public SqlGuidCollection()
    {
      _list = null;
    }

    public SqlGuidCollection(IEnumerable<Guid?> coll)
    {
      _list = coll != null ? new List<Guid?>(coll) : null;
    }

    private SqlGuidCollection(List<Guid?> list)
    {
      _list = list;
    }

    #endregion
    #region Properties

    public List<Guid?> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlGuidCollection Null
    {
      get { return new SqlGuidCollection(); }
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

    public static SqlGuidCollection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlGuidCollection(SqlFormatting.ParseCollection<Guid?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlGuid.Parse(t).Value : default(Guid?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, (v, i) => (v.HasValue ? new SqlGuid(v.Value) : SqlGuid.Null).ToString());
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlGuid value)
    {
      _list.Add(value.IsNull ? default(Guid?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlGuid value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(Guid?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlGuid value)
    {
      _list.Remove(value.IsNull ? default(Guid?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      if (index.IsNull)
        return;

      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlGuid value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(Guid?) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlGuidCollection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlGuid value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(Guid?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlGuidCollection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlGuid value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(Guid?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlGuidCollection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlGuid value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(Guid?) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlGuid GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value].HasValue ? _list[index.Value].Value : SqlGuid.Null;
    }

    [SqlMethod]
    public SqlGuidCollection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlGuidCollection(_list.GetRange(indexValue, countValue));
    }

    [SqlMethod]
    public SqlGuidArray ToArray()
    {
      return new SqlGuidArray(_list);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlGuidCollection coll)
    {
      using (var ms = new MemoryStream())
      using (new NulGuidStreamedCollection(ms, SizeEncoding.B4, true, coll._list, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlGuidCollection(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sc = new NulGuidStreamedCollection(ms, true, false))
        return new SqlGuidCollection(sc.ToList());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sc = new NulGuidStreamedCollection(rd.BaseStream, true, false))
        _list = sc.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sc = new NulGuidStreamedCollection(ms, SizeEncoding.B4, true, _list, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
