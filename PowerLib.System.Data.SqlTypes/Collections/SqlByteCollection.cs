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
  [SqlUserDefinedType(Format.UserDefined, Name = "TinyIntCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlByteCollection : INullable, IBinarySerialize
  {
    private List<Byte?> _list;

    #region Contructors

    public SqlByteCollection()
    {
      _list = null;
    }

    public SqlByteCollection(IEnumerable<Byte?> coll)
    {
      _list = coll != null ? new List<Byte?>(coll) : null;
    }

    private SqlByteCollection(List<Byte?> list)
    {
      _list = list;
    }

    #endregion
    #region Properties

    public List<Byte?> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlByteCollection Null
    {
      get { return new SqlByteCollection(); }
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

    public static SqlByteCollection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlByteCollection(SqlFormatting.ParseCollection<Byte?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlByte.Parse(t).Value : default(Byte?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, t => (t.HasValue ? new SqlByte(t.Value) : SqlByte.Null).ToString());
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlByte value)
    {
      _list.Add(value.IsNull ? default(Byte?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlByte value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(Byte?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlByte value)
    {
      _list.Remove(value.IsNull ? default(Byte?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      if (index.IsNull)
        return;

      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlByte value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(Byte?) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlByteCollection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlByte value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(Byte?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlByteCollection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlByte value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(Byte?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlByteCollection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlByte value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(Byte?) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlByte GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value].HasValue ? _list[index.Value].Value : SqlByte.Null;
    }

    [SqlMethod]
    public SqlByteCollection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlByteCollection(_list.GetRange(indexValue, countValue));
    }

    [SqlMethod]
    public SqlByteArray ToArray()
    {
      return new SqlByteArray(_list);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlByteCollection coll)
    {
      using (var ms = new MemoryStream())
      using (new NulByteStreamedCollection(ms, SizeEncoding.B4, true, coll._list, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlByteCollection(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sc = new NulByteStreamedCollection(ms, true, false))
        return new SqlByteCollection(sc.ToList());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sc = new NulByteStreamedCollection(rd.BaseStream, true, false))
        _list = sc.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sc = new NulByteStreamedCollection(ms, SizeEncoding.B4, true, _list, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
