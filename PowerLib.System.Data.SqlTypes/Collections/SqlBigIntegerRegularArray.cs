using System;
using System.Linq;
using System.IO;
using System.Numerics;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.Collections;
using PowerLib.System.Linq;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "HugeIntRegularArray", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlBigIntegerRegularArray : INullable, IBinarySerialize
  {
    private Array _array;
    private RegularArrayInfo _arrayInfo;

    #region Constructors

    public SqlBigIntegerRegularArray()
    {
      _array = null;
      _arrayInfo = null;
    }

    public SqlBigIntegerRegularArray(int[] lengths)
    {
      if (lengths != null && lengths.Any(l => l < 0))
        throw new ArgumentException("One or many items have invalid values.", "lengths");

      _array = PwrArray.CreateAsRegular<BigInteger?>(lengths);
      _arrayInfo = new RegularArrayInfo(lengths);
    }

    private SqlBigIntegerRegularArray(Array array)
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
        if (_array != null && _array.GetType().GetElementType() != typeof(BigInteger))
          throw new ArgumentException("Invalid array element type.");

        _array = value;
        _arrayInfo = value != null ? new RegularArrayInfo(value.GetRegularArrayDimensions()) : null;
      }
    }

    public RegularArrayInfo ArrayInfo
    {
      get { return _arrayInfo; }
    }

    public static SqlBigIntegerRegularArray Null
    {
      get { return new SqlBigIntegerRegularArray(); }
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

    public static SqlBigIntegerRegularArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlBigIntegerRegularArray(SqlFormatting.ParseRegular<BigInteger?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlBigInteger.Parse(t).Value : default(BigInteger?)));
    }

    public override String ToString()
    {
      return SqlFormatting.FormatRegular<BigInteger?>(_array, t => (t.HasValue ? new SqlBigInteger(t.Value) : SqlBigInteger.Null).ToString());
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
    public SqlBigInteger GetDimItem(SqlInt32Array indices)
    {
      return !indices.IsNull ? _arrayInfo.GetValue<BigInteger?>(indices.Array.Enumerate(0, indices.Length.Value).Select(t => t.Value).ToArray()).With(v => v.HasValue ? new SqlBigInteger(v.Value) : SqlBigInteger.Null) : SqlBigInteger.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetDimItem(SqlBigInteger value, SqlInt32Array indices)
    {
      if (!indices.IsNull)
        _arrayInfo.SetValue<BigInteger?>(_array, !value.IsNull ? value.Value : default(BigInteger?), indices.Array.Enumerate(0, indices.Length.Value).Select(t => t.Value).ToArray());
    }

    [SqlMethod]
    public SqlBigInteger GetFlatItem(SqlInt32 index)
    {
      return !index.IsNull ? new ArrayIndex(_arrayInfo) { FlatIndex = index.Value }.GetValue<BigInteger?>(_array).With(v => v.HasValue ? new SqlBigInteger(v.Value) : SqlBigInteger.Null) : SqlBigInteger.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetFlatItem(SqlBigInteger value, SqlInt32 index)
    {
      if (!index.IsNull)
        new ArrayIndex(_arrayInfo) { FlatIndex = index.Value }.SetValue(_array, !value.IsNull ? value.Value : default(BigInteger?));
    }

    [SqlMethod]
    public SqlBigIntegerArray GetFlatRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlBigIntegerArray(_array.EnumerateAsRegular<BigInteger?>(false, new Range(indexValue, countValue)));
    }

    [SqlMethod(IsMutator = true)]
    public void SetFlatRange(SqlBigIntegerArray range, SqlInt32 index)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - _array.Length : index.Value;
      _array.FillAsRegular((fi, di) => range.Array[fi - indexValue], null, false, new Range(indexValue, range.Length.Value));
    }

    [SqlMethod(IsMutator = true)]
    public void FillFlatRange(SqlBigInteger value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.FillAsRegular(!value.IsNull ? value.Value : default(BigInteger?), false, new Range(indexValue, countValue));
    }

    [SqlMethod]
    public SqlBigIntegerRegularArray GetDimRange(SqlRangeArray ranges)
    {
      return new SqlBigIntegerRegularArray(_array.RangeAsRegular(null, null, false, null,
        Enumerable.Range(0, _array.Rank).Select(d => !ranges.IsNull && ranges.Array[d].HasValue ? ranges.Array[d].Value : new Range(0, _array.GetLength(d))).ToArray()));
    }

    [SqlMethod(IsMutator = true)]
    public void SetDimRange(SqlBigIntegerRegularArray range, SqlInt32Array indices)
    {
      if (range.IsNull)
        return;

      _array.FillAsRegular<BigInteger?>((fi, di) => (BigInteger?)range.Array.GetValue(di), new int[_array.Rank], true, null,
        Enumerable.Range(0, Array.Rank).Select(d => new Range(indices.Array[d].HasValue ? indices.Array[d].Value : _array.GetLength(d) - range.Array.GetLength(d), range.Array.GetLength(d))).ToArray());
    }

    [SqlMethod(IsMutator = true)]
    public void FillDimRange(SqlBigInteger value, SqlRangeArray ranges)
    {
      _array.FillAsRegular<BigInteger?>(!value.IsNull ? value.Value : default(BigInteger?), false, null,
        Enumerable.Range(0, _array.Rank).Select(d => !ranges.IsNull && ranges.Array[d].HasValue ? ranges.Array[d].Value : new Range(0, _array.GetLength(d))).ToArray());
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlBigIntegerRegularArray array)
    {
      using (var ms = new MemoryStream())
      using (new BinaryStreamedRegularArray(ms, SizeEncoding.B4, SizeEncoding.B4, array._array.GetRegularArrayLengths(),
        array._array.SelectAsRegular<BigInteger?, byte[]>(t => t.HasValue ? t.Value.ToByteArray() : default(byte[])).Counted(array._array.Length), true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlBigIntegerRegularArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new BinaryStreamedRegularArray(ms, true, false))
        return new SqlBigIntegerRegularArray(new RegularArrayInfo(sa.Lengths.ToArray()).CreateArray(sa));
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new BinaryStreamedRegularArray(rd.BaseStream, true, false))
        _array = new RegularArrayInfo(sa.Lengths.ToArray()).CreateArray(sa);
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new BinaryStreamedRegularArray(ms, SizeEncoding.B4, SizeEncoding.B4, _array.GetRegularArrayLengths(),
        _array.SelectAsRegular<BigInteger?, byte[]>(t => t.HasValue ? t.Value.ToByteArray() : default(byte[])).Counted(_array.Length), true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
