using System;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.Linq;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "BinaryRegularArray", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlBinaryRegularArray : INullable, IBinarySerialize
  {
    private Array _array;
    private RegularArrayInfo _arrayInfo;

    #region Constructors

    public SqlBinaryRegularArray()
    {
      _array = null;
      _arrayInfo = null;
    }

    public SqlBinaryRegularArray(int[] lengths)
    {
      if (lengths != null && lengths.Any(l => l < 0))
        throw new ArgumentException("One or many items have invalid values.", "lengths");

      _array = PwrArray.CreateAsRegular<Byte[]>(lengths);
      _arrayInfo = new RegularArrayInfo(lengths);
    }

    private SqlBinaryRegularArray(Array array)
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
        if (_array != null && _array.GetType().GetElementType() != typeof(Byte[]))
          throw new ArgumentException("Invalid array element type.");

        _array = value;
        _arrayInfo = value != null ? new RegularArrayInfo(value.GetRegularArrayDimensions()) : null;
      }
    }

    public RegularArrayInfo ArrayInfo
    {
      get { return _arrayInfo; }
    }

    public static SqlBinaryRegularArray Null
    {
      get { return new SqlBinaryRegularArray(); }
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

    public static SqlBinaryRegularArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlBinaryRegularArray(SqlFormatting.ParseRegular<Byte[]>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? PwrBitConverter.ParseBinary(t, true) : null));
    }

    public override String ToString()
    {
      return SqlFormatting.FormatRegular<Byte[]>(_array, t => (t != null ? PwrBitConverter.Format(t, true) : SqlBinary.Null.ToString()));
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
    public SqlBinary GetDimItem(SqlInt32Array indices)
    {
      return !indices.IsNull ? _arrayInfo.GetValue<Byte[]>(indices.Array.Enumerate(0, indices.Length.Value).Select(t => t.Value).ToArray()).With(v => v != null ? new SqlBinary(v) : SqlBinary.Null) : SqlBinary.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetDimItem(SqlBinary value, SqlInt32Array indices)
    {
      if (!indices.IsNull)
        _arrayInfo.SetValue<Byte[]>(_array, !value.IsNull ? value.Value : null, indices.Array.Enumerate(0, indices.Length.Value).Select(t => t.Value).ToArray());
    }

    [SqlMethod]
    public SqlBinary GetFlatItem(SqlInt32 index)
    {
      return !index.IsNull ? new ArrayIndex(_arrayInfo) { FlatIndex = index.Value }.GetValue<Byte[]>(_array).With(v => v != null ? new SqlBinary(v) : SqlBinary.Null) : SqlBinary.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetFlatItem(SqlBinary value, SqlInt32 index)
    {
      if (!index.IsNull)
        new ArrayIndex(_arrayInfo) { FlatIndex = index.Value }.SetValue(_array, !value.IsNull ? value.Value : null);
    }

    [SqlMethod]
    public SqlBinaryArray GetFlatRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlBinaryArray(_array.EnumerateAsRegular<Byte[]>(false, new Range(indexValue, countValue)));
    }

    [SqlMethod(IsMutator = true)]
    public void SetFlatRange(SqlBinaryArray range, SqlInt32 index)
    {
      if (range.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - _array.Length : index.Value;
      _array.FillAsRegular((fi, di) => range.Array[fi - indexValue], null, false, new Range(indexValue, range.Length.Value));
    }

    [SqlMethod(IsMutator = true)]
    public void FillFlatRange(SqlBinary value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.FillAsRegular(!value.IsNull ? value.Value : null, false, new Range(indexValue, countValue));
    }

    [SqlMethod]
    public SqlBinaryRegularArray GetDimRange(SqlRangeArray ranges)
    {
      return new SqlBinaryRegularArray(_array.RangeAsRegular(null, null, false, null,
        Enumerable.Range(0, _array.Rank).Select(d => !ranges.IsNull && ranges.Array[d].HasValue ? ranges.Array[d].Value : new Range(0, _array.GetLength(d))).ToArray()));
    }

    [SqlMethod(IsMutator = true)]
    public void SetDimRange(SqlBinaryRegularArray range, SqlInt32Array indices)
    {
      if (range.IsNull)
        return;

      _array.FillAsRegular<Byte[]>((fi, di) => (Byte[])range.Array.GetValue(di), new int[_array.Rank], true, null,
        Enumerable.Range(0, Array.Rank).Select(d => new Range(indices.Array[d].HasValue ? indices.Array[d].Value : _array.GetLength(d) - range.Array.GetLength(d), range.Array.GetLength(d))).ToArray());
    }

    [SqlMethod(IsMutator = true)]
    public void FillDimRange(SqlBinary value, SqlRangeArray ranges)
    {
      _array.FillAsRegular<Byte[]>(!value.IsNull ? value.Value : null, false, null,
        Enumerable.Range(0, _array.Rank).Select(d => !ranges.IsNull && ranges.Array[d].HasValue ? ranges.Array[d].Value : new Range(0, _array.GetLength(d))).ToArray());
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlBinaryRegularArray array)
    {
      using (var ms = new MemoryStream())
      using (new BinaryStreamedRegularArray(ms, SizeEncoding.B4, SizeEncoding.B4, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlBinaryRegularArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new BinaryStreamedRegularArray(ms, true, false))
        return new SqlBinaryRegularArray(sa.ToRegularArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new BinaryStreamedRegularArray(rd.BaseStream, true, false))
        _array = sa.ToRegularArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new BinaryStreamedRegularArray(ms, SizeEncoding.B4, SizeEncoding.B4, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
