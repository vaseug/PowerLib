using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using System.Text;
using Microsoft.SqlServer.Server;
using PowerLib.System.Collections;
using PowerLib.System.IO;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "StringCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlStringCollection : INullable, IBinarySerialize
  {
    private List<String> _list;
    private Encoding _encoding;

    #region Contructors

    public SqlStringCollection()
    {
      _list = null;
      _encoding = null;
    }

    public SqlStringCollection(IEnumerable<String> coll, Encoding encoding)
    {
      _list = coll != null ? new List<String>(coll) : null;
      _encoding = encoding;
    }

    private SqlStringCollection(List<String> list, Encoding encoding)
    {
      _list = list;
      _encoding = encoding;
    }

    #endregion
    #region Properties

    public List<String> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlStringCollection Null
    {
      get { return new SqlStringCollection(); }
    }

    public bool IsNull
    {
      get { return _list == null; }
    }

    public SqlInt32 Count
    {
      get { return _list != null ? _list.Count : SqlInt32.Null; }
    }

    public SqlInt32 Codepage
    {
      get { return _encoding != null ? _encoding.CodePage : SqlInt32.Null; }
    }

    #endregion
    #region Methods

    public static SqlStringCollection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlStringCollection(SqlFormatting.ParseCollection<String>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlFormatting.Unquote(t) : default(String)), Encoding.Unicode);
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, t => t != null ? SqlFormatting.Quote(t) : SqlString.Null.ToString());
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlString value)
    {
      _list.Add(value.IsNull ? default(String) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlString value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(String) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlString value)
    {
      _list.Remove(value.IsNull ? default(String) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      if (index.IsNull)
        return;

      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlString value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(String) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlStringCollection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlString value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(String) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlStringCollection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlString value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(String) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlStringCollection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlString value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(String) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlString GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value] != null ? _list[index.Value] : SqlString.Null;
    }

    [SqlMethod]
    public SqlStringCollection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlStringCollection(_list.GetRange(indexValue, countValue), _encoding);
    }

    [SqlMethod]
    public SqlStringArray ToArray()
    {
      return new SqlStringArray(_list, _encoding);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlStringCollection coll)
    {
      using (var ms = new MemoryStream())
      using (new StringStreamedCollection(ms, SizeEncoding.B4, SizeEncoding.B4, coll._encoding, null, coll._list, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlStringCollection(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new StringStreamedCollection(ms, null, true, false))
        return new SqlStringCollection(sa.ToList(), sa.Encoding);
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new StringStreamedCollection(rd.BaseStream, null, true, false))
      {
        _list = sa.ToList();
        _encoding = sa.Encoding;
      }
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new StringStreamedCollection(ms, SizeEncoding.B4, SizeEncoding.B4, _encoding, null, _list, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
