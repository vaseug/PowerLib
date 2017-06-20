using System;
using System.IO;
using System.Globalization;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.Numerics;

namespace PowerLib.System.Data.SqlTypes.Numerics
{
  [SqlUserDefinedType(Format.UserDefined, Name = "SexagesimalAngle", IsByteOrdered = false, IsFixedLength = true, MaxByteSize = 5)]
  public struct SqlSexagesimalAngle : INullable, IBinarySerialize
  {
    private static readonly SqlSexagesimalAngle @null = new SqlSexagesimalAngle();

    private SexagesimalAngle? _value;

    #region Constructor

    public SqlSexagesimalAngle(SexagesimalAngle angle)
    {
      _value = angle;
    }

    #endregion
    #region Properties

    public static SqlSexagesimalAngle Null
    {
      get { return @null; }
    }

    public bool IsNull
    {
      get { return !_value.HasValue; }
    }

    public SexagesimalAngle Value
    {
      get { return _value.Value; }
    }

    public SqlInt32 Units
    {
      get
      {
        return _value.HasValue ? SqlInt32.Null : new SqlInt32(_value.Value.Units);
      }
    }

    public SqlBoolean Negative
    {
      get
      {
        return _value.HasValue ? _value.Value.Negative : SqlBoolean.Null;
      }
    }

    public SqlInt32 Degrees
    {
      get
      {
        return _value.HasValue ? _value.Value.Degrees : SqlInt32.Null;
      }
    }

    public SqlInt32 Minutes
    {
      get
      {
        return _value.HasValue ? _value.Value.Minutes : SqlInt32.Null;
      }
    }

    public SqlInt32 Seconds
    {
      get
      {
        return _value.HasValue ? _value.Value.Seconds : SqlInt32.Null;
      }
    }

    public SqlInt32 Hundredths
    {
      get
      {
        return _value.HasValue ? _value.Value.Hundredths : SqlInt32.Null;
      }
    }

    #endregion
    #region Methods

    public static SqlSexagesimalAngle Parse(SqlString s)
    {
      return s.IsNull ? Null : new SqlSexagesimalAngle(SexagesimalAngle.Parse(s.Value, CultureInfo.InvariantCulture));
    }

    public override String ToString()
    {
      return !_value.HasValue ? SqlFormatting.NullText : _value.Value.ToString();
    }

    public SqlDouble ToRadian()
    {
      return _value.HasValue ? _value.Value.ToRadian() : SqlDouble.Null;
    }

    public SqlDouble ToDegree()
    {
      return _value.HasValue ? _value.Value.ToDegree() : SqlDouble.Null;
    }

    public SqlSexagesimalAngle Add(SqlSexagesimalAngle op)
    {
      return _value.HasValue && op._value.HasValue ? new SqlSexagesimalAngle(this._value.Value.Add(op._value.Value)) : SqlSexagesimalAngle.Null;
    }

    public SqlSexagesimalAngle Sub(SqlSexagesimalAngle op)
    {
      return _value.HasValue && op._value.HasValue ? new SqlSexagesimalAngle(this._value.Value.Subtract(op._value.Value)) : SqlSexagesimalAngle.Null;
    }

    public SqlSexagesimalAngle Mul(SqlInt32 op)
    {
      return _value.HasValue && !op.IsNull ? new SqlSexagesimalAngle(this._value.Value.Multiply(op.Value)) : SqlSexagesimalAngle.Null;
    }

    public SqlSexagesimalAngle Div(SqlInt32 op)
    {
      return _value.HasValue && !op.IsNull ? new SqlSexagesimalAngle(this._value.Value.Divide(op.Value)) : SqlSexagesimalAngle.Null;
    }

    #endregion
    #region Operators

    public static implicit operator SqlSexagesimalAngle(SexagesimalAngle angle)
    {
      return new SqlSexagesimalAngle(angle);
    }

    public static explicit operator SexagesimalAngle(SqlSexagesimalAngle angle)
    {
      return angle._value.Value;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _value = !rd.ReadBoolean() ? default(SexagesimalAngle?) : new SexagesimalAngle(rd.ReadInt32());
    }

    public void Write(BinaryWriter wr)
    {
      wr.Write(_value.HasValue);
      wr.Write(_value.HasValue ? _value.Value.Units : default(int));
    }

    #endregion
  }
}
