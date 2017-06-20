﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.IO;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Numerics;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "SexagesimalAngleCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlSexagesimalAngleCollection : INullable, IBinarySerialize
  {
    private List<SexagesimalAngle?> _list;

    #region Contructors

    public SqlSexagesimalAngleCollection()
    {
      _list = null;
    }

    public SqlSexagesimalAngleCollection(IEnumerable<SexagesimalAngle?> coll)
    {
      _list = coll != null ? new List<SexagesimalAngle?>(coll) : null;
    }

    private SqlSexagesimalAngleCollection(List<SexagesimalAngle?> list)
    {
      _list = list;
    }

    #endregion
    #region Properties

    public List<SexagesimalAngle?> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlSexagesimalAngleCollection Null
    {
      get { return new SqlSexagesimalAngleCollection(); }
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

    public static SqlSexagesimalAngleCollection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlSexagesimalAngleCollection(SqlFormatting.ParseCollection<SexagesimalAngle?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlSexagesimalAngle.Parse(t).Value : default(SexagesimalAngle?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, t => (t.HasValue ? new SqlSexagesimalAngle(t.Value) : SqlSexagesimalAngle.Null).ToString());
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlSexagesimalAngle value)
    {
      _list.Add(value.IsNull ? default(SexagesimalAngle?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlSexagesimalAngle value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(SexagesimalAngle?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlSexagesimalAngle value)
    {
      _list.Remove(value.IsNull ? default(SexagesimalAngle?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      if (index.IsNull)
        return;

      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlSexagesimalAngle value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(SexagesimalAngle?) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlSexagesimalAngleCollection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlSexagesimalAngle value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(SexagesimalAngle?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlSexagesimalAngleCollection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlSexagesimalAngle value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(SexagesimalAngle?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlSexagesimalAngleCollection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlSexagesimalAngle value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(SexagesimalAngle?) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlSexagesimalAngle GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value].HasValue ? _list[index.Value].Value : SqlSexagesimalAngle.Null;
    }

    [SqlMethod]
    public SqlSexagesimalAngleCollection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlSexagesimalAngleCollection(_list.GetRange(indexValue, countValue));
    }

    [SqlMethod]
    public SqlSexagesimalAngleArray ToArray()
    {
      return new SqlSexagesimalAngleArray(_list);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlSexagesimalAngleCollection coll)
    {
      using (var ms = new MemoryStream())
      using (new NulInt32StreamedCollection(ms, SizeEncoding.B4, true, coll._list.Select(t => t.HasValue ? t.Value.Units : default(Int32?)).Counted(coll._list.Count), true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlSexagesimalAngleCollection(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulInt32StreamedArray(ms, true, false))
        return new SqlSexagesimalAngleCollection(sa.Select(t => t.HasValue ? new SexagesimalAngle(t.Value) : default(SexagesimalAngle?)).ToList());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulInt32StreamedArray(rd.BaseStream, true, false))
        _list = sa.Select(t => !t.HasValue ? default(SexagesimalAngle?) : new SexagesimalAngle(t.Value)).ToList();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulInt32StreamedArray(ms, SizeEncoding.B4, true, _list.Select(t => t.HasValue ? t.Value.Units : default(Int32?)).Counted(_list.Count), true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
