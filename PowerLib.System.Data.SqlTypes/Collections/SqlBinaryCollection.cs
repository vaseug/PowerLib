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
  [SqlUserDefinedType(Format.UserDefined, Name = "BinaryCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlBinaryCollection : INullable, IBinarySerialize
  {
    private List<Byte[]> _list;

    #region Contructors

    public SqlBinaryCollection()
    {
      _list = null;
    }

    public SqlBinaryCollection(IEnumerable<Byte[]> coll)
    {
      _list = coll != null ? new List<Byte[]>(coll) : null;
    }

    private SqlBinaryCollection(List<Byte[]> list)
    {
      _list = list;
    }

    #endregion
    #region Properties

    public List<Byte[]> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlBinaryCollection Null
    {
      get { return new SqlBinaryCollection(); }
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

    public static SqlBinaryCollection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlBinaryCollection(SqlFormatting.ParseCollection<Byte[]>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? PwrBitConverter.ParseBinary(t, true) : null));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, t => (t != null ? PwrBitConverter.Format(t, true) : SqlBinary.Null.ToString()));
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlBinary value)
    {
      _list.Add(value.IsNull ? default(Byte[]) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlBinary value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(Byte[]) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlBinary value)
    {
      _list.Remove(value.IsNull ? default(Byte[]) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlBinary value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(Byte[]) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlBinaryCollection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlBinary value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(Byte[]) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlBinaryCollection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlBinary value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(Byte[]) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlBinaryCollection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlBinary value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(Byte[]) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlBinary GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value] != null ? _list[index.Value] : SqlBinary.Null;
    }

    [SqlMethod]
    public SqlBinaryCollection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlBinaryCollection(_list.GetRange(indexValue, countValue));
    }

    [SqlMethod]
    public SqlBinaryArray ToArray()
    {
      return new SqlBinaryArray(_list);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlBinaryCollection coll)
    {
      using (var ms = new MemoryStream())
      using (new BinaryStreamedCollection(ms, SizeEncoding.B4, SizeEncoding.B4, coll._list, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlBinaryCollection(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new BinaryStreamedCollection(ms, true, false))
        return new SqlBinaryCollection(sa.ToList());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new BinaryStreamedCollection(rd.BaseStream, true, false))
        _list = sa.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new BinaryStreamedCollection(ms, SizeEncoding.B4, SizeEncoding.B4, _list, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
