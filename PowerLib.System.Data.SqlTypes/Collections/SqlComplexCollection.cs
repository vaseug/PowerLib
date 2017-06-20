using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Numerics;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.IO;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "ComplexCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlComplexCollection : INullable, IBinarySerialize
  {
    private List<Complex?> _list;

    #region Contructors

    public SqlComplexCollection()
    {
      _list = null;
    }

    public SqlComplexCollection(IEnumerable<Complex?> coll)
    {
      _list = coll != null ? new List<Complex?>(coll) : null;
    }

    private SqlComplexCollection(List<Complex?> list)
    {
      _list = list;
    }

    #endregion
    #region Properties

    public List<Complex?> List
    {
      get { return _list; }
      set { _list = value; }
    }

    public static SqlComplexCollection Null
    {
      get { return new SqlComplexCollection(); }
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

    public static SqlComplexCollection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlComplexCollection(SqlFormatting.ParseCollection<Complex?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlComplex.Parse(t).Value : default(Complex?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_list, t => (t.HasValue ? new SqlComplex(t.Value) : SqlComplex.Null).ToString());
    }

    [SqlMethod(IsMutator = true)]
    public void Clear()
    {
      _list.Clear();
    }

    [SqlMethod(IsMutator = true)]
    public void AddItem(SqlComplex value)
    {
      _list.Add(value.IsNull ? default(Complex?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertItem(SqlInt32 index, SqlComplex value)
    {
      _list.Insert(index.IsNull ? _list.Count : index.Value, value.IsNull ? default(Complex?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveItem(SqlComplex value)
    {
      _list.Remove(value.IsNull ? default(Complex?) : value.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveAt(SqlInt32 index)
    {
      if (index.IsNull)
        return;

      _list.RemoveAt(index.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlInt32 index, SqlComplex value)
    {
      if (index.IsNull)
        return;

      _list[index.Value] = value.IsNull ? default(Complex?) : value.Value;
    }

    [SqlMethod(IsMutator = true)]
    public void AddRange(SqlComplexCollection coll)
    {
      if (coll.IsNull)
        return;

      _list.AddRange(coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void AddRepeat(SqlComplex value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      _list.AddRepeat(value.IsNull ? default(Complex?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRange(SqlInt32 index, SqlComplexCollection coll)
    {
      if (coll.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRange(indexValue, coll._list);
    }

    [SqlMethod(IsMutator = true)]
    public void InsertRepeat(SqlInt32 index, SqlComplex value, SqlInt32 count)
    {
      if (count.IsNull)
        return;

      int indexValue = !index.IsNull ? index.Value : _list.Count;
      _list.InsertRepeat(indexValue, value.IsNull ? default(Complex?) : value.Value, count.Value);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlInt32 index, SqlComplexCollection range)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _list.Count - Comparable.Min(_list.Count, range._list.Count) : index.Value;
      _list.SetRange(indexValue, range.List);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRepeat(SqlInt32 index, SqlComplex value, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.SetRepeat(indexValue, value.IsNull ? default(Complex?) : value.Value, countValue);
    }

    [SqlMethod(IsMutator = true)]
    public void RemoveRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      _list.RemoveRange(indexValue, countValue);
    }

    [SqlMethod]
    public SqlComplex GetItem(SqlInt32 index)
    {
      return !index.IsNull && _list[index.Value].HasValue ? _list[index.Value].Value : SqlComplex.Null;
    }

    [SqlMethod]
    public SqlComplexCollection GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _list.Count - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _list.Count - index.Value;
      return new SqlComplexCollection(_list.GetRange(indexValue, countValue));
    }

    [SqlMethod]
    public SqlComplexArray ToArray()
    {
      return new SqlComplexArray(_list);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlComplexCollection coll)
    {
      using (MemoryStream ms = new MemoryStream())
      using (BinaryWriter wr = new BinaryWriter(ms))
      {
        coll.Write(wr);
        return ms.ToArray();
      }
    }

    public static explicit operator SqlComplexCollection(byte[] buffer)
    {
      SqlComplexCollection coll = new SqlComplexCollection();
      if (buffer != null)
        using (MemoryStream ms = new MemoryStream(buffer))
        using (BinaryReader rd = new BinaryReader(ms))
          coll.Read(rd);
      return coll;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sc = new NulComplexStreamedCollection(rd.BaseStream, true, false))
        _list = sc.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sc = new NulComplexStreamedCollection(ms, SizeEncoding.B4, true, _list, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
