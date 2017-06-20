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
using PowerLib.System .Data.SqlTypes.Numerics;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "HourAngleRegularArray", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlHourAngleRegularArray : INullable, IBinarySerialize
  {
    private Array _array;
    private RegularArrayInfo _arrayInfo;

    #region Constructors

    public SqlHourAngleRegularArray()
    {
      _array = null;
      _arrayInfo = null;
    }

    public SqlHourAngleRegularArray(int[] lengths)
    {
      if (lengths != null && lengths.Any(l => l < 0))
        throw new ArgumentException("One or many items have invalid values.", "lengths");

      _array = PwrArray.CreateAsRegular<HourAngle?>(lengths);
      _arrayInfo = new RegularArrayInfo(lengths);
    }

    private SqlHourAngleRegularArray(Array array)
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
        if (_array != null && _array.GetType().GetElementType() != typeof(HourAngle))
          throw new ArgumentException("Invalid array element type.");

        _array = value;
        _arrayInfo = value != null ? new RegularArrayInfo(value.GetRegularArrayDimensions()) : null;
      }
    }

    public RegularArrayInfo ArrayInfo
    {
      get { return _arrayInfo; }
    }

    public static SqlHourAngleRegularArray Null
    {
      get { return new SqlHourAngleRegularArray(); }
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

    public static SqlHourAngleRegularArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlHourAngleRegularArray(SqlFormatting.ParseRegular<HourAngle?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlHourAngle.Parse(t).Value : default(HourAngle?)));
    }

    public override String ToString()
    {
      return SqlFormatting.FormatRegular<HourAngle?>(_array, t => (t.HasValue ? new SqlHourAngle(t.Value) : SqlHourAngle.Null).ToString());
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
    public SqlHourAngle GetDimItem(SqlInt32Array indices)
    {
      return !indices.IsNull ? _arrayInfo.GetValue<HourAngle?>(indices.Array.Enumerate(0, indices.Length.Value).Select(t => t.Value).ToArray()).With(v => v.HasValue ? new SqlHourAngle(v.Value) : SqlHourAngle.Null) : SqlHourAngle.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetDimItem(SqlHourAngle value, SqlInt32Array indices)
    {
      if (!indices.IsNull)
        _arrayInfo.SetValue<HourAngle?>(_array, !value.IsNull ? value.Value : default(HourAngle?), indices.Array.Enumerate(0, indices.Length.Value).Select(t => t.Value).ToArray());
    }

    [SqlMethod]
    public SqlHourAngle GetFlatItem(SqlInt32 index)
    {
      return !index.IsNull ? new ArrayIndex(_arrayInfo) { FlatIndex = index.Value }.GetValue<HourAngle?>(_array).With(v => v.HasValue ? new SqlHourAngle(v.Value) : SqlHourAngle.Null) : SqlHourAngle.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetFlatItem(SqlHourAngle value, SqlInt32 index)
    {
      if (!index.IsNull)
        new ArrayIndex(_arrayInfo) { FlatIndex = index.Value }.SetValue(_array, !value.IsNull ? value.Value : default(HourAngle?));
    }

    [SqlMethod]
    public SqlHourAngleArray GetFlatRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlHourAngleArray(_array.EnumerateAsRegular<HourAngle?>(false, new Range(indexValue, countValue)));
    }

    [SqlMethod(IsMutator = true)]
    public void SetFlatRange(SqlHourAngleArray range, SqlInt32 index)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - _array.Length : index.Value;
      _array.FillAsRegular((fi, di) => range.Array[fi - indexValue], null, false, new Range(indexValue, range.Length.Value));
    }

    [SqlMethod(IsMutator = true)]
    public void FillFlatRange(SqlHourAngle value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.FillAsRegular(!value.IsNull ? value.Value : default(HourAngle?), false, new Range(indexValue, countValue));
    }

    [SqlMethod]
    public SqlHourAngleRegularArray GetDimRange(SqlRangeArray ranges)
    {
      return new SqlHourAngleRegularArray(_array.RangeAsRegular(null, null, false, null,
        Enumerable.Range(0, _array.Rank).Select(d => !ranges.IsNull && ranges.Array[d].HasValue ? ranges.Array[d].Value : new Range(0, _array.GetLength(d))).ToArray()));
    }

    [SqlMethod(IsMutator = true)]
    public void SetDimRange(SqlHourAngleRegularArray range, SqlInt32Array indices)
    {
      if (range.IsNull)
        return;

      _array.FillAsRegular<HourAngle?>((fi, di) => (HourAngle?)range.Array.GetValue(di), new int[_array.Rank], true, null,
        Enumerable.Range(0, Array.Rank).Select(d => new Range(indices.Array[d].HasValue ? indices.Array[d].Value : _array.GetLength(d) - range.Array.GetLength(d), range.Array.GetLength(d))).ToArray());
    }

    [SqlMethod(IsMutator = true)]
    public void FillDimRange(SqlHourAngle value, SqlRangeArray ranges)
    {
      _array.FillAsRegular<HourAngle?>(!value.IsNull ? value.Value : default(HourAngle?), false, null,
        Enumerable.Range(0, _array.Rank).Select(d => !ranges.IsNull && ranges.Array[d].HasValue ? ranges.Array[d].Value : new Range(0, _array.GetLength(d))).ToArray());
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlHourAngleRegularArray array)
    {
      using (var ms = new MemoryStream())
      using (new NulInt32StreamedRegularArray(ms, SizeEncoding.B4, true, array._array.GetRegularArrayLengths(),
        array._array.SelectAsRegular<HourAngle?, Int32?>(t => t.HasValue ? t.Value.Units : default(Int32?)).Counted(array._array.Length), true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlHourAngleRegularArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulInt32StreamedRegularArray(ms, true, false))
        return new SqlHourAngleRegularArray(new RegularArrayInfo(sa.Lengths.ToArray()).CreateArray(sa));
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
        _array.SelectAsRegular<HourAngle?, Int32?>(t => t.HasValue ? t.Value.Units : default(Int32?)).Counted(_array.Length), true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
