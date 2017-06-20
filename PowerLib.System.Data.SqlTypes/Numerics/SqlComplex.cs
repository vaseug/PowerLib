using System;
using System.IO;
using System.Globalization;
using System.Numerics;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.IO;
using PowerLib.System.Numerics;

namespace PowerLib.System.Data.SqlTypes.Numerics
{
  [SqlUserDefinedType(Format.UserDefined, Name = "Complex", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = 17)]
  public struct SqlComplex : INullable, IBinarySerialize
  {
    private static readonly SqlComplex @null = new SqlComplex();

    private Complex? _value;

    #region Constructor

    public SqlComplex(Complex number)
    {
      _value = number;
    }

    #endregion
    #region Properties

    public bool IsNull
    {
      get { return !_value.HasValue; }
    }

    public static SqlComplex Null
    {
      get { return @null; }
    }

    public SqlDouble Real
    {
      get
      {
        return _value.HasValue ? _value.Value.Real : SqlDouble.Null;
      }
    }

    public SqlDouble Imaginary
    {
      get
      {
        return _value.HasValue ? _value.Value.Imaginary : SqlDouble.Null;
      }
    }

    public SqlDouble Magnitude
    {
      get
      {
        return _value.HasValue ? _value.Value.Magnitude : SqlDouble.Null;
      }
    }

    public SqlDouble Phase
    {
      get
      {
        return _value.HasValue ? _value.Value.Phase : SqlDouble.Null;
      }
    }

    public Complex Value
    {
      get { return _value.Value; }
    }

    #endregion
    #region Methods

    public static SqlComplex Parse(SqlString s)
    {
      return s.IsNull ? @null : new SqlComplex(ComplexNumber.Parse(s.Value, CultureInfo.InvariantCulture));
    }

    public override String ToString()
    {
      return !_value.HasValue ? SqlFormatting.NullText : _value.Value.ToString();
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Conjugate()
    {
      return _value.HasValue ? Complex.Conjugate(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Reciprocal()
    {
      return _value.HasValue ? Complex.Reciprocal(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Negate()
    {
      return _value.HasValue ? Complex.Negate(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Add(SqlComplex addend)
    {
      return _value.HasValue && addend._value.HasValue ? Complex.Add(_value.Value, addend._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Subtract(SqlComplex subtrahend)
    {
      return _value.HasValue && subtrahend._value.HasValue ? Complex.Subtract(_value.Value, subtrahend._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Multiply(SqlComplex multiplier)
    {
      return _value.HasValue && multiplier._value.HasValue ? Complex.Multiply(_value.Value, multiplier._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Divide(SqlComplex divisor)
    {
      return _value.HasValue && divisor._value.HasValue ? Complex.Divide(_value.Value, divisor._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlDouble Abs()
    {
      return _value.HasValue ? Complex.Abs(_value.Value) : SqlDouble.Null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Acos()
    {
      return _value.HasValue ? Complex.Acos(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Asin()
    {
      return _value.HasValue ? Complex.Asin(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Atan()
    {
      return _value.HasValue ? Complex.Atan(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Cos()
    {
      return _value.HasValue ? Complex.Cos(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Cosh()
    {
      return _value.HasValue ? Complex.Cosh(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Sin()
    {
      return _value.HasValue ? Complex.Sin(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Sinh()
    {
      return _value.HasValue ? Complex.Sinh(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Tan()
    {
      return _value.HasValue ? new SqlComplex(Complex.Tan(_value.Value)) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Tanh()
    {
      return _value.HasValue ? Complex.Tanh(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Exp()
    {
      return _value.HasValue ? Complex.Exp(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Log()
    {
      return _value.HasValue ? Complex.Log(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Log(SqlDouble baseValue)
    {
      return _value.HasValue && !baseValue.IsNull ? Complex.Log(_value.Value, baseValue.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Log10()
    {
      return _value.HasValue ? Complex.Log10(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Pow(SqlDouble power)
    {
      return _value.HasValue && !power.IsNull ? Complex.Pow(_value.Value, power.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Pow(SqlComplex power)
    {
      return _value.HasValue && power._value.HasValue ? Complex.Pow(_value.Value, power._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlComplex Sqrt()
    {
      return _value.HasValue ? Complex.Sqrt(_value.Value) : @null;
    }

    #endregion
    #region Operators

    public static implicit operator SqlComplex(Complex number)
    {
      return new SqlComplex(number);
    }

    public static explicit operator Complex(SqlComplex number)
    {
      return number._value.Value;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _value = rd.ReadNullable(vr => new Complex(rd.ReadDouble(), rd.ReadDouble()));
    }

    public void Write(BinaryWriter wr)
    {
      wr.Write(_value, (vw, v) => { vw.Write(v.Real); vw.Write(v.Imaginary); });
    }

    #endregion
  }
}
