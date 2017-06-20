using System;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;

namespace PowerLib.System.Data.SqlTypes
{
  [SqlUserDefinedType(Format.UserDefined, Name = "Range", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = 9)]
  public struct SqlRange : INullable, IBinarySerialize
  {
    private static readonly SqlRange @null = new SqlRange();

    private Range? _value;

    #region Constructor

    public SqlRange(Range range)
    {
      _value = range;
    }

    #endregion
    #region Properties

    public static SqlRange Null
    {
      get { return @null; }
    }

    public bool IsNull
    {
      get { return !_value.HasValue; }
    }

    public SqlInt32 Index
    {
      get
      {
        return _value.HasValue ? _value.Value.Index : SqlInt32.Null;
      }
    }

    public SqlInt32 Count
    {
      get
      {
        return _value.HasValue ? _value.Value.Count : SqlInt32.Null;
      }
    }

    public Range Value
    {
      get { return _value ?? default(Range); }
    }

    #endregion
    #region Methods

    public static SqlRange Parse(SqlString s)
    {
      return s.IsNull ? Null : new SqlRange(Range.Parse(s.Value));
    }

    public override String ToString()
    {
      return !_value.HasValue ? "Null" : _value.Value.ToString();
    }

    #endregion
    #region Operators

    public static implicit operator SqlRange(Range range)
    {
      return new SqlRange(range);
    }

    public static explicit operator Range(SqlRange range)
    {
      return range._value.Value;
    }


    public static implicit operator byte[] (SqlRange range)
    {
      using (MemoryStream ms = new MemoryStream())
      using (BinaryWriter wr = new BinaryWriter(ms))
      {
        range.Write(wr);
        return ms.ToArray();
      }
    }

    public static explicit operator SqlRange(byte[] buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      using (MemoryStream ms = new MemoryStream(buffer))
      using (BinaryReader rd = new BinaryReader(ms))
      {
        SqlRange range = new SqlRange();
        range.Read(rd);
        return range;
      }
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _value = !rd.ReadBoolean() ? default(Range?) : new Range(rd.ReadInt32(), rd.ReadInt32());
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
