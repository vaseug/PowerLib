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
  [SqlUserDefinedType(Format.UserDefined, Name = "DoubleFloatCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlDoubleCollection : INullable, IBinarySerialize
  {
    private List<Double?> _list;

    #region Contructors

    public SqlDoubleCollection()
    {
      _list = null;
    }

    public SqlDoubleCollection(IEnumerable<Double?> coll)
    {
      _list = coll != null ? new List<Double?>(coll) : null;
    }

    private SqlDoubleCollection(List<Double?> list)
    {
      _list = list;
    }

    #endregion
    #region Properties

    public List<Double?> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlDoubleCollection Null
    {
      get { return new SqlDoubleCollection(); }
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

    public static SqlDoubleCollection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlDoubleCollection(SqlFormatting.ParseCollection<Double?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlDouble.Parse(t).Value : default(Double?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, t => (t.HasValue ? new SqlDouble(t.Value) : SqlDouble.Null).ToString());
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlDouble value)
    {
      _list.Add(value.IsNull ? default(Double?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlDouble value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(Double?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlDouble value)
    {
      _list.Remove(value.IsNull ? default(Double?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      if (index.IsNull)
        return;

      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlDouble value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(Double?) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlDoubleCollection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlDouble value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(Double?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlDoubleCollection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlDouble value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(Double?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlDoubleCollection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlDouble value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(Double?) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlDouble GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value].HasValue ? _list[index.Value].Value : SqlDouble.Null;
    }

    [SqlMethod]
    public SqlDoubleCollection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlDoubleCollection(_list.GetRange(indexValue, countValue));
    }

    [SqlMethod]
    public SqlDoubleArray ToArray()
    {
      return new SqlDoubleArray(_list);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlDoubleCollection coll)
    {
      using (var ms = new MemoryStream())
      using (new NulDoubleStreamedCollection(ms, SizeEncoding.B4, true, coll._list, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlDoubleCollection(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sc = new NulDoubleStreamedCollection(ms, true, false))
        return new SqlDoubleCollection(sc.ToList());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sc = new NulDoubleStreamedCollection(rd.BaseStream, true, false))
        _list = sc.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sc = new NulDoubleStreamedCollection(ms, SizeEncoding.B4, true, _list, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
