﻿using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.Linq;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "StringRegularArray", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlStringRegularArray : INullable, IBinarySerialize
  {
    private Encoding _encoding;
    private Array _array;
    private RegularArrayInfo _arrayInfo;

    #region Constructors

    public SqlStringRegularArray()
    {
      _array = null;
      _arrayInfo = null;
      _encoding = null;
    }

    public SqlStringRegularArray(int[] lengths, Encoding encoding)
    {
      if (lengths != null && lengths.Any(t => t < 0))
        throw new ArgumentException("One or many items have invalid values.", "lengths");

      _array = PwrArray.CreateAsRegular<String>(lengths);
      _arrayInfo = new RegularArrayInfo(lengths);
      _encoding = encoding;
    }

    private SqlStringRegularArray(Array array, Encoding encoding)
    {
      _array = array;
      _arrayInfo = _array != null ? new RegularArrayInfo(array.GetRegularArrayDimensions()) : null;
      _encoding = encoding;
    }

    #endregion
    #region Properties

    public Array Array
    {
      get { return _array; }
      set
      {
        if (_array != null && _array.GetType().GetElementType() != typeof(String))
          throw new ArgumentException("Invalid array element type.");

        _array = value;
        _arrayInfo = value != null ? new RegularArrayInfo(value.GetRegularArrayDimensions()) : null;
      }
    }

    public RegularArrayInfo ArrayInfo
    {
      get { return _arrayInfo; }
    }

    public static SqlStringRegularArray Null
    {
      get { return Null; }
    }

    public bool IsNull
    {
      get { return _array == null; }
    }

    public SqlInt32 Rank
    {
      get { return _array != null ? _array.Rank : SqlInt32.Null; }
    }

    public SqlInt32 Length
    {
      get { return _array != null ? _array.Length : SqlInt32.Null; }
    }

    #endregion
    #region Methods

    public static SqlStringRegularArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlStringRegularArray(SqlFormatting.ParseRegular<String>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlFormatting.Unquote(t) : default(String)), Encoding.Unicode);
    }

    public override String ToString()
    {
      return SqlFormatting.FormatRegular<String>(_array, t => t != null ? SqlFormatting.Quote(t) : SqlString.Null.ToString());
    }

    [SqlMethod]
    public SqlInt32Array GetLengths()
    {
      return new SqlInt32Array(_array.GetRegularArrayLengths());
    }

    [SqlMethod]
    public SqlInt32 GetLength(SqlInt32 dim)
    {
      return !dim.IsNull ? new SqlInt32(_array.GetLength(dim.Value)) : SqlInt32.Null;
    }

    [SqlMethod]
    public SqlInt32Array GetDimIndices(SqlInt32 index)
    {
      if (index.IsNull)
        return SqlInt32Array.Null;

      int[] indices = new int[_array.Rank];
      _arrayInfo.CalcDimIndices(index.Value, indices);
      return new SqlInt32Array(indices);
    }

    [SqlMethod]
    public SqlInt32 GetFlatIndex(SqlInt32Array indices)
    {
      return !indices.IsNull ? new SqlInt32(_arrayInfo.CalcFlatIndex(indices.Array.Enumerate(0, indices.Length.Value).Select(t => t.Value).ToArray())) : SqlInt32.Null;
    }

    [SqlMethod]
    [return: SqlFacet(MaxSize = -1)]
    public SqlString GetDimItem(SqlInt32Array indices)
    {
      return !indices.IsNull ? _arrayInfo.GetValue<String>(indices.Array.Enumerate(0, indices.Length.Value).Select(t => t.Value).ToArray()).With(v => v != null ? new SqlString(v) : SqlString.Null) : SqlString.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetDimItem(SqlString value, SqlInt32Array indices)
    {
      if (!indices.IsNull)
        _arrayInfo.SetValue<String>(_array, !value.IsNull ? value.Value : null, indices.Array.Enumerate(0, indices.Length.Value).Select(t => t.Value).ToArray());
    }

    [SqlMethod]
    [return: SqlFacet(MaxSize = -1)]
    public SqlString GetFlatItem(SqlInt32 index)
    {
      return !index.IsNull ? new ArrayIndex(_arrayInfo) { FlatIndex = index.Value }.GetValue<String>(_array).With(v => v != null ? new SqlString(v) : SqlString.Null) : SqlString.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetFlatItem([SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 index)
    {
      if (!index.IsNull)
        new ArrayIndex(_arrayInfo) { FlatIndex = index.Value }.SetValue(_array, !value.IsNull ? value.Value : null);
    }

    [SqlMethod]
    public SqlStringArray GetFlatRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlStringArray(_array.EnumerateAsRegular<String>(false, new Range(indexValue, countValue)), _encoding);
    }

    [SqlMethod(IsMutator = true)]
    public void SetFlatRange(SqlStringArray range, SqlInt32 index)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - _array.Length : index.Value;
      _array.FillAsRegular((fi, di) => range.Array[fi - indexValue], null, false, new Range(indexValue, range.Length.Value));
    }

    [SqlMethod(IsMutator = true)]
    public void FillFlatRange([SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.FillAsRegular(!value.IsNull ? value.Value : null, false, new Range(indexValue, countValue));
    }

    [SqlMethod]
    public SqlStringRegularArray GetDimRange(SqlRangeArray ranges)
    {
      return new SqlStringRegularArray(_array.RangeAsRegular(null, null, false, null,
        Enumerable.Range(0, _array.Rank).Select(d => !ranges.IsNull && ranges.Array[d].HasValue ? ranges.Array[d].Value : new Range(0, _array.GetLength(d))).ToArray()), _encoding);
    }

    [SqlMethod(IsMutator = true)]
    public void SetDimRange(SqlStringRegularArray range, SqlInt32Array indices)
    {
      if (range.IsNull)
        return;

      _array.FillAsRegular<String>((fi, di) => (String)range.Array.GetValue(di), new int[_array.Rank], true, null,
        Enumerable.Range(0, Array.Rank).Select(d => new Range(indices.Array[d].HasValue ? indices.Array[d].Value : _array.GetLength(d) - range.Array.GetLength(d), range.Array.GetLength(d))).ToArray());
    }

    [SqlMethod(IsMutator = true)]
    public void FillDimRange([SqlFacet(MaxSize = -1)] SqlString value, SqlRangeArray ranges)
    {
      _array.FillAsRegular<String>(!value.IsNull ? value.Value : null, false, null,
        Enumerable.Range(0, _array.Rank).Select(d => !ranges.IsNull && ranges.Array[d].HasValue ? ranges.Array[d].Value : new Range(0, _array.GetLength(d))).ToArray());
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlStringRegularArray array)
    {
      using (var ms = new MemoryStream())
      using (new StringStreamedRegularArray(ms, SizeEncoding.B4, SizeEncoding.B4, array._encoding, null, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlStringRegularArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new StringStreamedRegularArray(ms, null, true, false))
        return new SqlStringRegularArray(sa.ToRegularArray(), sa.Encoding);
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new StringStreamedRegularArray(rd.BaseStream, null, true, false))
        _array = sa.ToRegularArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new StringStreamedRegularArray(ms, SizeEncoding.B4, SizeEncoding.B4, _encoding, null, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
