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
  [SqlUserDefinedType(Format.UserDefined, Name = "BitCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlBooleanCollection : INullable, IBinarySerialize
  {
    private List<Boolean?> _list;

    #region Contructors

    public SqlBooleanCollection()
    {
      _list = null;
    }

    public SqlBooleanCollection(IEnumerable<Boolean?> coll)
    {
      _list = coll != null ? new List<Boolean?>(coll) : null;
    }

    private SqlBooleanCollection(List<Boolean?> list)
    {
      _list = list;
    }

    #endregion
    #region Properties

    public List<Boolean?> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlBooleanCollection Null
    {
      get { return new SqlBooleanCollection(); }
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

    public static SqlBooleanCollection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlBooleanCollection(SqlFormatting.ParseCollection<Boolean?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlBoolean.Parse(t).Value : default(Boolean?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, t => (t.HasValue ? new SqlBoolean(t.Value) : SqlBoolean.Null).ToString());
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlBoolean value)
    {
      _list.Add(value.IsNull ? default(Boolean?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlBoolean value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(Boolean?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlBoolean value)
    {
      _list.Remove(value.IsNull ? default(Boolean?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      if (index.IsNull)
        return;

      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlBoolean value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(Boolean?) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlBooleanCollection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlBoolean value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(Boolean?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlBooleanCollection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlBoolean value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(Boolean?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlBooleanCollection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlBoolean value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(Boolean?) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlBoolean GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value].HasValue ? _list[index.Value].Value : SqlBoolean.Null;
    }

    [SqlMethod]
    public SqlBooleanCollection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlBooleanCollection(_list.GetRange(indexValue, countValue));
    }

    [SqlMethod]
    public SqlBooleanArray ToArray()
    {
      return new SqlBooleanArray(_list);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlBooleanCollection coll)
    {
      using (var ms = new MemoryStream())
      using (var sc = new NulBooleanStreamedCollection(ms, SizeEncoding.B4, coll._list, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlBooleanCollection(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sc = new NulBooleanStreamedCollection(ms, true, false))
        return new SqlBooleanCollection(sc.ToList());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sc = new NulBooleanStreamedCollection(rd.BaseStream, true, false))
        _list = sc.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sc = new NulBooleanStreamedCollection(ms, SizeEncoding.B4, _list, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
