using System;
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
  [SqlUserDefinedType(Format.UserDefined, Name = "GradAngleArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlGradAngleArray : INullable, IBinarySerialize
  {
    private GradAngle?[] _array;

    #region Contructors

    public SqlGradAngleArray()
    {
      _array = null;
    }

    public SqlGradAngleArray(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new GradAngle?[length];
    }

    public SqlGradAngleArray(IEnumerable<GradAngle?> coll)
    {
      _array = coll != null ? coll.ToArray() : null;
    }

    private SqlGradAngleArray(GradAngle?[] array)
    {
      _array = array;
    }

    #endregion
    #region Properties

    public GradAngle?[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlGradAngleArray Null
    {
      get { return new SqlGradAngleArray(); }
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

    public static SqlGradAngleArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlGradAngleArray(SqlFormatting.ParseArray<GradAngle?>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlGradAngle.Parse(t).Value : default(GradAngle?)));
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => (t.HasValue ? new SqlGradAngle(t.Value) : SqlGradAngle.Null).ToString());
    }

    [SqlMethod]
    public SqlGradAngle GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value].HasValue ? _array[index.Value].Value : SqlGradAngle.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem(SqlGradAngle value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : default(GradAngle?);
    }

    [SqlMethod]
    public SqlGradAngleArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlGradAngleArray(_array.Range(indexValue, countValue));
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlGradAngleArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange(SqlGradAngle value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : default(GradAngle?), indexValue, countValue);
    }

    [SqlMethod]
    public SqlGradAngleCollection ToCollection()
    {
      return new SqlGradAngleCollection(_array);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlGradAngleArray array)
    {
      using (var ms = new MemoryStream())
      using (new NulInt32StreamedArray(ms, SizeEncoding.B4, true, array._array.Select(t => t.HasValue ? t.Value.Units : default(Int32?)).Counted(array._array.Length), true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlGradAngleArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new NulInt32StreamedArray(ms, true, false))
        return new SqlGradAngleArray(sa.Select(t => t.HasValue ? new GradAngle(t.Value) : default(GradAngle?)).ToArray());
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulInt32StreamedArray(rd.BaseStream, true, false))
        _array = sa.Select(t => !t.HasValue ? default(GradAngle?) : new GradAngle(t.Value)).ToArray();
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
