using System;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.Numerics;
using PowerLib.System.Linq;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "SexagesimalAngleRegularArray", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlSexagesimalAngleRegularArray : INullable, IBinarySerialize
  {
    private Array _array;
    private RegularArrayInfo _arrayInfo;

    #region Constructors

    public SqlSexagesimalAngleRegularArray()
    {
      _array = null;
      _arrayInfo = null;
    }

    public SqlSexagesimalAngleRegularArray(int[] lengths)
    {
      if (lengths != null && lengths.Any(l => l < 0))
        throw new ArgumentException("One or many items have invalid values.", "lengths");

      _array = PwrArray.CreateAsRegular<SexagesimalAngle?>(lengths);
      _arrayInfo = new RegularArrayInfo(lengths);
    }

    private SqlSexagesimalAngleRegularArray(Array array)
    {
      _array = array;
      _arrayInfo = _array != null ? new RegularArrayInfo(array.GetRegularArrayDimensions()) : null;
    }

    #endregion
    #region Properties

    public Array Array
    {
      get { return _array; }
      set
      {
        if (_array != null && _array.GetType().GetElementType() != typeof(SexagesimalAngle))
          throw new ArgumentException("Invalid array element type.");

        _array = value;
        _arrayInfo = value != null ? new RegularArrayInfo(value.GetRegularArrayDimensions()) : null;
      }
    }

    public RegularArrayInfo ArrayInfo
    {
      get { return _arrayInfo; }
    }

    public static SqlSexagesimalAngleRegularArray Null
    {
      get { return new SqlSexagesimalAngleRegularArray(); }
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

    public static SqlSexagesimalAngleRegularArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlSexagesimalAngleRegularArray(SqlFormatting.ParseRegular<SexagesimalAngle?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlSexagesimalAngle.Parse(t).Value : default(SexagesimalAngle?)));
    }

    public override String ToString()
    {
      return SqlFormatting.FormatRegular<SexagesimalAngle?>(_array, t => (t.HasValue ? new SqlSexagesimalAngle(t.Value) : SqlSexagesimalAngle.Null).ToString());
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
    public SqlSexagesimalAngle GetDimItem(SqlInt32Array indices)
    {
      return !indices.IsNull ? _arrayInfo.GetValue<SexagesimalAngle?>(indices.Array.Enumerate(0, indices.Length.Value).Select(t => t.Value).ToArray()).With(v => v.HasValue ? new SqlSexagesimalAngle(v.Value) : SqlSexagesimalAngle.Null) : SqlSexagesimalAngle.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetDimItem(SqlSexagesimalAngle value, SqlInt32Array indices)
    {
      if (!indices.IsNull)
        _arrayInfo.SetValue<SexagesimalAngle?>(_array, !value.IsNull ? value.Value : default(SexagesimalAngle?), indices.Array.Enumerate(0, indices.Length.Value).Select(t => t.Value).ToArray());
    }

    [SqlMethod]
    public SqlSexagesimalAngle GetFlatItem(SqlInt32 index)
    {
      return !index.IsNull ? new ArrayIndex(_arrayInfo) { FlatIndex = index.Value }.GetValue<SexagesimalAngle?>(_array).With(v => v.HasValue ? new SqlSexagesimalAngle(v.Value) : SqlSexagesimalAngle.Null) : SqlSexagesimalAngle.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetFlatItem(SqlSexagesimalAngle value, SqlInt32 index)
    {
      if (!index.IsNull)
        new ArrayIndex(_arrayInfo) { FlatIndex = index.Value }.SetValue(_array, !value.IsNull ? value.Value : default(SexagesimalAngle?));
    }

    [SqlMethod]
    public SqlSexagesimalAngleArray GetFlatRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlSexagesimalAngleArray(_array.EnumerateAsRegular<SexagesimalAngle?>(false, new Range(indexValue, countValue)));
    }

    [SqlMethod(IsMutator = true)]
    public void SetFlatRange(SqlSexagesimalAngleArray range, SqlInt32 index)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - _array.Length : index.Value;
      _array.FillAsRegular((fi, di) => range.Array[fi - indexValue], null, false, new Range(indexValue, range.Length.Value));
    }

    [SqlMethod(IsMutator = true)]
    public void FillFlatRange(SqlSexagesimalAngle value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.FillAsRegular(!value.IsNull ? value.Value : default(SexagesimalAngle?), false, new Range(indexValue, countValue));
    }

    [SqlMethod]
    public SqlSexagesimalAngleRegularArray GetDimRange(SqlRangeArray ranges)
    {
      return new SqlSexagesimalAngleRegularArray(_array.RangeAsRegular(null, null, false, null,
        Enumerable.Range(0, _array.Rank).Select(d => !ranges.IsNull && ranges.Array[d].HasValue ? ranges.Array[d].Value : new Range(0, _array.GetLength(d))).ToArray()));
    }

    [SqlMethod(IsMutator = true)]
    public void SetDimRange(SqlSexagesimalAngleRegularArray range, SqlInt32Array indices)
    {
      if (range.IsNull)
        return;

      _array.FillAsRegular<SexagesimalAngle?>((fi, di) => (SexagesimalAngle?)range.Array.GetValue(di), new int[_array.Rank], true, null,
        Enumerable.Range(0, Array.Rank).Select(d => new Range(indices.Array[d].HasValue ? indices.Array[d].Value : _array.GetLength(d) - range.Array.GetLength(d), range.Array.GetLength(d))).ToArray());
    }

    [SqlMethod(IsMutator = true)]
    public void FillDimRange(SqlSexagesimalAngle value, SqlRangeArray ranges)
    {
      _array.FillAsRegular<SexagesimalAngle?>(!value.IsNull ? value.Value : default(SexagesimalAngle?), false, null,
        Enumerable.Range(0, _array.Rank).Select(d => !ranges.IsNull && ranges.Array[d].HasValue ? ranges.Array[d].Value : new Range(0, _array.GetLength(d))).ToArray());
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlSexagesimalAngleRegularArray array)
    {
      using (var ms = new MemoryStream())
      using (new NulInt32StreamedRegularArray(ms, SizeEncoding.B4, true, array._array.GetRegularArrayLengths(),
        array._array.SelectAsRegular<SexagesimalAngle?, Int32?>(t => t.HasValue ? t.Value.Units : default(Int32?)).Counted(array._array.Length), true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlSexagesimalAngleRegularArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulInt32StreamedRegularArray(ms, true, false))
        return new SqlSexagesimalAngleRegularArray(new RegularArrayInfo(sa.Lengths.ToArray()).CreateArray(sa));
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulInt32StreamedRegularArray(rd.BaseStream, true, false))
        _array = new RegularArrayInfo(sa.Lengths.ToArray()).CreateArray(sa);
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulInt32StreamedRegularArray(ms, SizeEncoding.B4, true, _array.GetRegularArrayLengths(),
        _array.SelectAsRegular<SexagesimalAngle?, Int32?>(t => t.HasValue ? t.Value.Units : default(Int32?)).Counted(_array.Length), true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
