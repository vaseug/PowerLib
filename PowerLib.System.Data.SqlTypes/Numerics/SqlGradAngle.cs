using System;
using System.IO;
using System.Globalization;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.Numerics;

namespace PowerLib.System.Data.SqlTypes.Numerics
{
  [SqlUserDefinedType(Format.UserDefined, Name = "GradAngle", IsByteOrdered = false, IsFixedLength = true, MaxByteSize = 5)]
  public struct SqlGradAngle : INullable, IBinarySerialize
  {
    private static readonly SqlGradAngle @null = new SqlGradAngle();

    private GradAngle? _value;

    #region Constructors

    public SqlGradAngle(GradAngle angle)
    {
      _value = angle;
    }

    #endregion
    #region Properties

    public static SqlGradAngle Null
    {
      get { return @null; }
    }

    public bool IsNull
    {
      get { return !_value.HasValue; }
    }

    public GradAngle Value
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
        return _value.HasValue ? _value.Value.Grads : SqlInt32.Null;
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

    public static SqlGradAngle Parse(SqlString s)
    {
      return s.IsNull ? Null : new SqlGradAngle(GradAngle.Parse(s.Value, CultureInfo.InvariantCulture));
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

    public SqlDouble ToGrads()
    {
      return _value.HasValue ? _value.Value.ToGrads() : SqlDouble.Null;
    }

    public SqlGradAngle Add(SqlGradAngle op)
    {
      return _value.HasValue && op._value.HasValue ? new SqlGradAngle(this._value.Value.Add(op._value.Value)) : SqlGradAngle.Null;
    }

    public SqlGradAngle Sub(SqlGradAngle op)
    {
      return _value.HasValue && op._value.HasValue ? new SqlGradAngle(this._value.Value.Subtract(op._value.Value)) : SqlGradAngle.Null;
    }

    public SqlGradAngle Mul(SqlInt32 op)
    {
      return _value.HasValue && !op.IsNull ? new SqlGradAngle(this._value.Value.Multiply(op.Value)) : SqlGradAngle.Null;
    }

    public SqlGradAngle Div(SqlInt32 op)
    {
      return _value.HasValue && !op.IsNull ? new SqlGradAngle(this._value.Value.Divide(op.Value)) : SqlGradAngle.Null;
    }

    #endregion
    #region Operators

    public static implicit operator SqlGradAngle(GradAngle angle)
    {
      return new SqlGradAngle(angle);
    }

    public static explicit operator GradAngle(SqlGradAngle angle)
    {
      return angle._value.Value;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _value = !rd.ReadBoolean() ? default(GradAngle?) : new GradAngle(rd.ReadInt32());
    }

    public void Write(BinaryWriter wr)
    {
      wr.Write(_value.HasValue);
      wr.Write(_value.HasValue ? _value.Value.Units : default(int));
    }

    #endregion
  }
}
