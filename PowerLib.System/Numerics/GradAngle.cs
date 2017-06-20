using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PowerLib.System.Numerics
{
  /// <summary>
  /// Represents measure of angle in gradians (grads). Grad is unit of plane angle, equivalents 1/400 of turn.
  /// </summary>
  [TypeConverter(typeof(GradAngleConverter))]
  public struct GradAngle : IAngle<double>
  {
    public const int MaxTotalGrads = 400;
    public const int MinutesPerGrad = 100;
    public const int SecondsPerMinute = 100;
    public const int HundredthsPerSecond = 100;
    public const int SecondsPerGrad = MinutesPerGrad * SecondsPerMinute;
    public const int HundredthsPerMinute = SecondsPerMinute * HundredthsPerSecond;
    public const int HundredthsPerGrad = MinutesPerGrad * SecondsPerMinute * HundredthsPerSecond;
    public const int MaxTotalUnits = MaxTotalGrads * MinutesPerGrad * SecondsPerMinute * HundredthsPerSecond;
    private const string PatternFormat = @"^(?'sign'[-+]?)(?:[ ]*(?'grad'[0-9]|[1-9][0-9]|[1-3][0-9]{{2}})[ᵍg])(?:[ ]*(?'minute'[0-9]|[1-9][0-9])[ᶜ'](?:[ ]*(?'second'[0-9]|[1-9][0-9])(?:[{0}](?'hundredth'[0-9]{{2}}))?(?:[""]|[ᶜ]{{2}}))?)?$";
    private const string NumericPattern = @"^(?'start'(?'angle'[-+]?[0-9]+(?:[{0}][0-9]+)?(?:[eE][-+]?[0-9]+)?))(?:(?'radian-start'(?=[r]))|(?'grad-start'(?=[ᵍg]?)))[ᵍgr]?(?(start)(?!))$";

    private int _units;
    public static readonly GradAngle MaxValue= new GradAngle(MaxTotalUnits - 1);
    public static readonly GradAngle MinValue= new GradAngle(-MaxTotalUnits + 1);

    #region Constructors

    public GradAngle(int units)
    {
      if (units < -MaxTotalUnits || units > MaxTotalUnits)
        throw new ArgumentOutOfRangeException("units");
      _units = units;
    }

    public GradAngle(int grad, int minute, int second)
      : this(grad, minute, second, 0)
    {
    }

    public GradAngle(int grad, int minute, int second, bool negative)
      : this(grad, minute, second, 0, negative)
    {
    }

    public GradAngle(int grad, int minute, int second, int hundredth)
      : this(grad, minute, second, hundredth, false)
    {
    }

    public GradAngle(int grad, int minute, int second, int hundredth, bool negative)
    {
      if (grad < 0 || grad >= MaxTotalGrads)
        throw new ArgumentOutOfRangeException("grad");
      if (minute < 0 || minute >= MinutesPerGrad)
        throw new ArgumentOutOfRangeException("minute");
      if (second < 0 || second >= SecondsPerMinute)
        throw new ArgumentOutOfRangeException("second");
      if (hundredth < 0 || hundredth >= HundredthsPerSecond)
        throw new ArgumentOutOfRangeException("hundredth");
      int value = grad * HundredthsPerGrad + minute * HundredthsPerMinute + second * HundredthsPerSecond + hundredth;
      if (negative)
        value = -value ;
      if (value <= -MaxTotalUnits || value >= MaxTotalUnits)
        throw new ArgumentOutOfRangeException(null, "Inconsistenarguments item");
      _units = value ;
    }

    public GradAngle(double grads)
    {
      _units = (int)global::System.Math.Round(Angle.NormGrad(grads) * HundredthsPerGrad);
    }

    #endregion
    #region Properties

    public int Units
    {
      get
      {
        return _units;
      }
    }

    public bool Negative
    {
      get
      {
        return _units < 0;
      }
    }

    public int Grads
    {
      get
      {
        return global::System.Math.Abs(_units / HundredthsPerGrad);
      }
    }

    public int Minutes
    {
      get
      {
        return global::System.Math.Abs(_units % HundredthsPerGrad / HundredthsPerMinute);
      }
    }

    public int Seconds
    {
      get
      {
        return global::System.Math.Abs(_units % HundredthsPerGrad % HundredthsPerMinute / HundredthsPerSecond);
      }
    }

    public int Hundredths
    {
      get
      {
        return global::System.Math.Abs(_units % HundredthsPerSecond);
      }
    }

    #endregion
    #region Operators
    #region Comparison

    public static bool operator==(GradAngle op1, GradAngle op2)
    {
      return op1._units == op2._units;
    }

    public static bool operator!=(GradAngle op1, GradAngle op2)
    {
      return op1._units != op2._units;
    }

    public static bool operator>(GradAngle op1, GradAngle op2)
    {
      return op1._units > op2._units;
    }

    public static bool operator>=(GradAngle op1, GradAngle op2)
    {
      return op1._units >= op2._units;
    }

    public static bool operator<(GradAngle op1, GradAngle op2)
    {
      return op1._units < op2._units;
    }

    public static bool operator<=(GradAngle op1, GradAngle op2)
    {
      return op1._units <= op2._units;
    }

    #endregion
    #region Mathematic

    public static GradAngle operator+(GradAngle x, GradAngle y)
    {
      return x.Add(y);
    }

    public static GradAngle operator-(GradAngle x, GradAngle y)
    {
      return x.Subtract(y);
    }

    public static GradAngle operator*(GradAngle x, int y)
    {
      return x.Multiply(y);
    }

    public static GradAngle operator/(GradAngle x, int y)
    {
      return x.Divide(y);
    }

    #endregion
    #endregion
    #region Methods

    public override bool Equals(object obj)
    {
      if (obj == null || obj.GetType() != typeof(GradAngle))
        return false;
      return this == (GradAngle)obj;
    }

    public override int GetHashCode()
    {
      return _units.GetHashCode();
    }

    public override string ToString()
    {
      return ToString(null);
    }

    public string ToString(IFormatProvider provider)
    {
      CultureInfo culture = provider as CultureInfo;
      if (culture == null)
        culture = CultureInfo.CurrentCulture;
      return string.Format(@"{0}{1}ᵍ{2:0#}ᶜ{3:0#}{4}ᶜᶜ", Negative ? "-" : "", Grads, Minutes, Seconds, Hundredths != 0 ? string.Format("{0}{1:0#}", culture.NumberFormat.NumberDecimalSeparator, Hundredths) : "");
    }

    public double ToRadian()
    {
      return Angle.FromGrad(ToGrads());
    }

    public double ToDegree()
    {
      return Angle.ToDegree(Angle.FromGrad(ToGrads()));
    }

    public double ToGrads()
    {
      return (double)Grads + (double)Minutes / (double)MinutesPerGrad + (double)Seconds / (double)SecondsPerGrad + (double)Hundredths / (double)HundredthsPerGrad;
    }

    public GradAngle Add(GradAngle op)
    {
      return new GradAngle((int)(((long)this._units + (long)op._units) % (long)MaxTotalUnits));
    }

    public GradAngle Subtract(GradAngle op)
    {
      return new GradAngle((int)(((long)this._units - (long)op._units) % (long)MaxTotalUnits));
    }

    public GradAngle Multiply(int op)
    {
      return new GradAngle((int)(((long)this._units * (long)op) % (long)MaxTotalUnits));
    }

    public GradAngle Divide(int op)
    {
      return new GradAngle((int)(((long)this._units / (long)op) % (long)MaxTotalUnits));
    }

    #endregion
    #region Static methods

    public static GradAngle FromRadian(double radian)
    {
      return new GradAngle(Angle.ToGrad(radian));
    }

    public static GradAngle Parse(string s)
    {
      GradAngle result;
      if (!TryParse(s, out result))
        throw new ArgumentException("Invalid input string");
      return result;
    }

    public static GradAngle Parse(string s, IFormatProvider provider)
    {
      GradAngle result;
      if (!TryParse(s, provider, out result))
        throw new ArgumentException("Invalid input string");
      return result;
    }

    public static bool TryParse(string s, IFormatProvider provider, out GradAngle result)
    {
      if (provider == null)
        provider = CultureInfo.CurrentCulture;
      NumberFormatInfo nfi = (NumberFormatInfo)provider.GetFormat(typeof(NumberFormatInfo));
      string pattern = string.Format(PatternFormat, nfi.NumberDecimalSeparator);
      Match match = Regex.Match(s, pattern);
      if (match.Success)
      {
        int grad = string.IsNullOrEmpty(match.Groups["grad"].Value) ? 0 : int.Parse(match.Groups["grad"].Value);
        int minute = string.IsNullOrEmpty(match.Groups["minute"].Value) ? 0 : int.Parse(match.Groups["minute"].Value);
        int second = string.IsNullOrEmpty(match.Groups["second"].Value) ? 0 : int.Parse(match.Groups["second"].Value);
        int hundredth = string.IsNullOrEmpty(match.Groups["hundredth"].Value) ? 0 : int.Parse(match.Groups["hundredth"].Value);
        bool negative = match.Groups["sign"].Value== "-";
        result = new GradAngle(grad, minute, second, hundredth, negative);
        return true;
      }
      else
      {
        pattern = string.Format(NumericPattern, nfi.NumberDecimalSeparator);
        match = Regex.Match(s, pattern);
        if (match.Success)
        {
          double angle;
          if (match.Groups["degree"].Success && double.TryParse(match.Groups["angle"].Value, NumberStyles.Float, provider, out angle))
          {
            result = new GradAngle(angle);
            return true;
          }
          else if (match.Groups["radian"].Success && double.TryParse(match.Groups["angle"].Value, NumberStyles.Float, provider, out angle))
          {
            result = GradAngle.FromRadian(angle);
            return true;
          }
        }
      }
      result = new GradAngle();
      return false;
    }

    public static bool TryParse(string s, out GradAngle result)
    {
      return TryParse(s, null, out result);
    }

    #endregion
  }
}
