using System;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;

namespace PowerLib.System.Data.SqlTypes
{
  [SqlUserDefinedType(Format.UserDefined, Name = "BigRange", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = 17)]
  public struct SqlLongRange : INullable, IBinarySerialize
  {
    private static readonly SqlLongRange @null = new SqlLongRange();

    private LongRange? _value;

    #region Constructor

    public SqlLongRange(LongRange range)
    {
      _value = range;
    }

    #endregion
    #region Properties

    public static SqlLongRange Null
    {
      get { return @null; }
    }

    public bool IsNull
    {
      get { return !_value.HasValue; }
    }

    public SqlInt64 Index
    {
      get
      {
        return _value.HasValue ? _value.Value.Index : SqlInt64.Null;
      }
    }

    public SqlInt64 Count
    {
      get
      {
        return _value.HasValue ? _value.Value.Count : SqlInt64.Null;
      }
    }

    public LongRange Value
    {
      get { return _value ?? default(LongRange); }
    }

    #endregion
    #region Methods

    public static SqlLongRange Parse(SqlString s)
    {
      return s.IsNull ? Null : new SqlLongRange(LongRange.Parse(s.Value));
    }

    public override String ToString()
    {
      return !_value.HasValue ? "Null" : _value.Value.ToString();
    }

    #endregion
    #region Operators

    public static implicit operator SqlLongRange(LongRange range)
    {
      return new SqlLongRange(range);
    }

    public static explicit operator LongRange(SqlLongRange range)
    {
      return range._value.Value;
    }


    public static implicit operator byte[] (SqlLongRange range)
    {
      using (MemoryStream ms = new MemoryStream())
      using (BinaryWriter wr = new BinaryWriter(ms))
      {
        range.Write(wr);
        return ms.ToArray();
      }
    }

    public static explicit operator SqlLongRange(byte[] buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      using (MemoryStream ms = new MemoryStream(buffer))
      using (BinaryReader rd = new BinaryReader(ms))
      {
        SqlLongRange range = new SqlLongRange();
        range.Read(rd);
        return range;
      }
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _value = !rd.ReadBoolean() ? default(LongRange?) : new LongRange(rd.ReadInt64(), rd.ReadInt64());
    }

    public void Write(BinaryWriter wr)
    {
      wr.Write(_value.HasValue);
      if (_value.HasValue)
      {
        wr.Write(_value.Value.Index);
        wr.Write(_value.Value.Count);
      }
    }

    #endregion
  }
}
