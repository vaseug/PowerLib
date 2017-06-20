using System;
using System.IO;
using System.Numerics;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.IO;

namespace PowerLib.System.Data.SqlTypes.Numerics
{
  [SqlUserDefinedType(Format.UserDefined, Name = "HugeInt", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = -1)]
  public struct SqlBigInteger : INullable, IBinarySerialize
  {
    private static readonly SqlBigInteger @null = new SqlBigInteger();
    private static readonly SqlBigInteger zero = new SqlBigInteger(BigInteger.Zero);
    private static readonly SqlBigInteger one = new SqlBigInteger(BigInteger.One);
    private static readonly SqlBigInteger minusOne = new SqlBigInteger(BigInteger.MinusOne);

    private BigInteger? _value;

    #region Constructor

    public SqlBigInteger(BigInteger number)
    {
      _value = number;
    }

    #endregion
    #region Properties

    public bool IsNull
    {
      get { return !_value.HasValue; }
    }

    public static SqlBigInteger Null
    {
      get { return @null; }
    }

    public static SqlBigInteger Zero
    {
      get { return zero; }
    }

    public static SqlBigInteger One
    {
      get { return one; }
    }

    public static SqlBigInteger MinusOne
    {
      get { return minusOne; }
    }

    public SqlInt32 Sign
    {
      get
      {
        return _value.HasValue ? _value.Value.Sign : SqlInt32.Null;
      }
    }

    public SqlBoolean IsEven
    {
      get
      {
        return _value.HasValue ? _value.Value.IsEven : SqlBoolean.Null;
      }
    }

    public SqlBoolean IsOne
    {
      get
      {
        return _value.HasValue ? _value.Value.IsOne : SqlBoolean.Null;
      }
    }

    public SqlBoolean IsPowerOfTwo
    {
      get
      {
        return _value.HasValue ? _value.Value.IsPowerOfTwo : SqlBoolean.Null;
      }
    }

    public SqlBoolean IsZero
    {
      get
      {
        return _value.HasValue ? _value.Value.IsZero : SqlBoolean.Null;
      }
    }

    public BigInteger Value
    {
      get
      {
        return _value.Value;
      }
    }

    #endregion
    #region Methods

    public static SqlBigInteger Parse(SqlString s)
    {
      return s.IsNull ? @null : new SqlBigInteger(BigInteger.Parse(s.Value));
    }

    public override String ToString()
    {
      return !_value.HasValue ? SqlFormatting.NullText : _value.Value.ToString();
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlBigInteger Abs()
    {
      return _value.HasValue ? BigInteger.Abs( _value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlBigInteger Negate()
    {
      return _value.HasValue ? BigInteger.Negate(_value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlDouble Log()
    {
      return _value.HasValue ? BigInteger.Log(_value.Value) : SqlDouble.Null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlDouble Log(SqlDouble baseValue)
    {
      return _value.HasValue && !baseValue.IsNull ? BigInteger.Log(_value.Value, baseValue.Value) : SqlDouble.Null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlDouble Log10()
    {
      return _value.HasValue ? BigInteger.Log10(_value.Value) : SqlDouble.Null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlBigInteger Pow(SqlInt32 exponent)
    {
      return _value.HasValue && !exponent.IsNull ? BigInteger.Pow(_value.Value, exponent.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlBigInteger ModPow(SqlBigInteger exponent, SqlBigInteger modulus)
    {
      return _value.HasValue && exponent._value.HasValue && modulus._value.HasValue ? BigInteger.ModPow(_value.Value, exponent._value.Value, modulus._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlBigInteger Add(SqlBigInteger addend)
    {
      return _value.HasValue && addend._value.HasValue ? BigInteger.Add(_value.Value, addend._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlBigInteger Subtract(SqlBigInteger subtrahend)
    {
      return _value.HasValue && subtrahend._value.HasValue ? BigInteger.Subtract(_value.Value, subtrahend._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlBigInteger Multiply(SqlBigInteger multiplier)
    {
      return _value.HasValue && multiplier._value.HasValue ? BigInteger.Multiply(_value.Value, multiplier._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlBigInteger Divide(SqlBigInteger divisor)
    {
      return _value.HasValue && divisor._value.HasValue ? BigInteger.Divide(_value.Value, divisor._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlBigInteger Remainder(SqlBigInteger divisor)
    {
      return _value.HasValue && divisor._value.HasValue ? BigInteger.Remainder(_value.Value, divisor._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlBigInteger GreatestCommonDivisor(SqlBigInteger other)
    {
      return _value.HasValue && other._value.HasValue ? BigInteger.GreatestCommonDivisor(_value.Value, other._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlBigInteger Max(SqlBigInteger other)
    {
      return _value.HasValue && other._value.HasValue ? BigInteger.Max(_value.Value, other._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlBigInteger Min(SqlBigInteger other)
    {
      return _value.HasValue && other._value.HasValue ? BigInteger.Min(_value.Value, other._value.Value) : @null;
    }

    [SqlMethod(IsDeterministic = true)]
    public SqlInt32 Compare(SqlBigInteger other)
    {
      return _value.HasValue && other._value.HasValue ? BigInteger.Compare(_value.Value, other._value.Value) : SqlInt32.Null;
    }

    #endregion
    #region Operators

    public static implicit operator SqlBigInteger(BigInteger number)
    {
      return new SqlBigInteger(number);
    }

    public static explicit operator BigInteger(SqlBigInteger number)
    {
      return number._value.Value;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _value = rd.ReadNullable(vr => new BigInteger(vr.ReadBytes(SizeEncoding.VB)));
    }

    public void Write(BinaryWriter wr)
    {
      wr.Write(_value, (vw, v) => vw.Write(v.ToByteArray(), SizeEncoding.VB));
    }

    #endregion
  }
}
