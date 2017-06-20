using System;
using System.IO;
using System.Globalization;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.Numerics;

namespace PowerLib.System.Data.SqlTypes.Numerics
{
  [SqlUserDefinedType(Format.UserDefined, Name = "HourAngle", IsByteOrdered = false, IsFixedLength = true, MaxByteSize = 5)]
  public struct SqlHourAngle : INullable, IBinarySerialize
  {
    private static readonly SqlHourAngle @null = new SqlHourAngle();

    private HourAngle? _value;

    #region Constructors

    public SqlHourAngle(HourAngle angle)
    {
      _value = angle;
    }

    #endregion
    #region Properties

    public static SqlHourAngle Null
    {
      get { return @null; }
    }

    public bool IsNull
    {
      get { return !_value.HasValue; }
    }

    public HourAngle Value
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

    public SqlInt32 Hours
    {
      get
      {
        return _value.HasValue ? _value.Value.Hours : SqlInt32.Null;
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

    public static SqlHourAngle Parse(SqlString s)
    {
      return s.IsNull ? Null : new SqlHourAngle(HourAngle.Parse(s.Value, CultureInfo.InvariantCulture));
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

    public SqlHourAngle Add(SqlHourAngle op)
    {
      return _value.HasValue && op._value.HasValue ? new SqlHourAngle(this._value.Value.Add(op._value.Value)) : SqlHourAngle.Null;
    }

    public SqlHourAngle Sub(SqlHourAngle op)
    {
      return _value.HasValue && op._value.HasValue ? new SqlHourAngle(this._value.Value.Subtract(op._value.Value)) : SqlHourAngle.Null;
    }

    public SqlHourAngle Mul(SqlInt32 op)
    {
      return _value.HasValue && !op.IsNull ? new SqlHourAngle(this._value.Value.Multiply(op.Value)) : SqlHourAngle.Null;
    }

    public SqlHourAngle Div(SqlInt32 op)
    {
      return _value.HasValue && !op.IsNull ? new SqlHourAngle(this._value.Value.Divide(op.Value)) : SqlHourAngle.Null;
    }

    #endregion
    #region Operators

    public static implicit operator SqlHourAngle(HourAngle angle)
    {
      return new SqlHourAngle(angle);
    }

    public static explicit operator HourAngle(SqlHourAngle angle)
    {
      return angle._value.Value;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _value = !rd.ReadBoolean() ? default(HourAngle?) : new HourAngle(rd.ReadInt32());
    }

    public void Write(BinaryWriter wr)
    {
      wr.Write(_value.HasValue);
      wr.Write(_value.HasValue ? _value.Value.Units : default(int));
    }

    #endregion
  }
}
