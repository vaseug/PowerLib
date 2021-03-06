﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Numerics;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "HourAngleArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlHourAngleArray : INullable, IBinarySerialize
  {
    private HourAngle?[] _array;

    #region Contructors

    public SqlHourAngleArray()
    {
      _array = null;
    }

    public SqlHourAngleArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new HourAngle?[length];
    }

    public SqlHourAngleArray(IEnumerable<HourAngle?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlHourAngleArray(HourAngle?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public HourAngle?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlHourAngleArray Null
    {
      get { return new SqlHourAngleArray(); }
    }

    public bool IsNull
    {
      get { return _array == null; }
    }

    public SqlInt32 Length
    {
      get { return _array != null ? _array.Length : SqlInt32.Null; }
    }

    #endregion
    #region Methods

    public static SqlHourAngleArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlHourAngleArray(SqlFormatting.ParseArray<HourAngle?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlHourAngle.Parse(t).Value : default(HourAngle?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlHourAngle(t.Value) : SqlHourAngle.Null).ToString());
    }

    [SqlMethod]
    public SqlHourAngle GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlHourAngle.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlHourAngle value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(HourAngle?);
    }

    [SqlMethod]
    public SqlHourAngleArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlHourAngleArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlHourAngleArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlHourAngle value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(HourAngle?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlHourAngleCollection ToCollection()
    {
      return new SqlHourAngleCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlHourAngleArray array)
    {
      using (var ms = new MemoryStream())
      using (new NulInt32StreamedArray(ms, SizeEncoding.B4, true, array._array.Select(t => t.HasValue ? t.Value.Units : default(Int32?)).Counted(array._array.Length), true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlHourAngleArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulInt32StreamedArray(ms, true, false))
        return new SqlHourAngleArray(sa.Select(t => t.HasValue ? new HourAngle(t.Value) : default(HourAngle?)).ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulInt32StreamedArray(rd.BaseStream, true, false))
        _array = sa.Select(t => !t.HasValue ? default(HourAngle?) : new HourAngle(t.Value)).ToArray();
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new NulInt32StreamedArray(ms, SizeEncoding.B4, true, _array.Select(t => t.HasValue ? t.Value.Units : default(Int32?)).Counted(_array.Length), true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
