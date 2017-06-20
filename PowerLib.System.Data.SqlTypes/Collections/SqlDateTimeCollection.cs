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
  [SqlUserDefinedType(Format.UserDefined, Name = "DateTimeCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlDateTimeCollection : INullable, IBinarySerialize
  {
    private List<DateTime?> _list;

    #region Contructors

    public SqlDateTimeCollection()
    {
      _list = null;
    }

    public SqlDateTimeCollection(IEnumerable<DateTime?> coll)
    {
      _list = coll != null ? new List<DateTime?>(coll) : null;
    }

    private SqlDateTimeCollection(List<DateTime?> list)
    {
      _list = list;
    }

    #endregion
    #region Properties

    public List<DateTime?> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlDateTimeCollection Null
    {
      get { return new SqlDateTimeCollection(); }
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

    public static SqlDateTimeCollection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlDateTimeCollection(SqlFormatting.ParseCollection<DateTime?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlDateTime.Parse(t).Value : default(DateTime?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, t => (t.HasValue ? new SqlDateTime(t.Value) : SqlDateTime.Null).ToString());
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlDateTime value)
    {
      _list.Add(value.IsNull ? default(DateTime?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlDateTime value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(DateTime?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlDateTime value)
    {
      _list.Remove(value.IsNull ? default(DateTime?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      if (index.IsNull)
        return;

      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlDateTime value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(DateTime?) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlDateTimeCollection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlDateTime value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(DateTime?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlDateTimeCollection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlDateTime value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(DateTime?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlDateTimeCollection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlDateTime value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(DateTime?) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlDateTime GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value].HasValue ? _list[index.Value].Value : SqlDateTime.Null;
    }

    [SqlMethod]
    public SqlDateTimeCollection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlDateTimeCollection(_list.GetRange(indexValue, countValue));
    }

    [SqlMethod]
    public SqlDateTimeArray ToArray()
    {
      return new SqlDateTimeArray(_list);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlDateTimeCollection coll)
    {
      using (var ms = new MemoryStream())
      using (new NulDateTimeStreamedCollection(ms, SizeEncoding.B4, true, coll._list, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlDateTimeCollection(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sc = new NulDateTimeStreamedCollection(ms, true, false))
        return new SqlDateTimeCollection(sc.ToList());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulDateTimeStreamedCollection(rd.BaseStream, true, false))
        _list = sa.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sc = new NulDateTimeStreamedCollection(ms, SizeEncoding.B4, true, _list, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
