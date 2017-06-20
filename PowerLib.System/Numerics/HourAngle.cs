using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PowerLib.System.Numerics
{
  /// <summary>
  /// Represents measure of angle in hours. Hour is unit of plane angle, equivalents 1/24 of turn.
  /// </summary>
	[TypeConverter(typeof(HourAngleConverter))]
	public struct HourAngle : IAngle<double>
	{
		public const int MaxTotalHours = 24;
		public const int MinutesPerHour = 60;
		public const int SecondsPerMinute = 60;
		public const int HundredthsPerSecond = 100;
		public const int SecondsPerHour = MinutesPerHour * SecondsPerMinute;
		public const int HundredthsPerMinute = SecondsPerMinute * HundredthsPerSecond;
		public const int HundredthsPerHour = MinutesPerHour * SecondsPerMinute * HundredthsPerSecond;
		public const int MaxTotalUnits = MaxTotalHours * MinutesPerHour * SecondsPerMinute * HundredthsPerSecond;
		private const string PatternFormat = @"^(?'sign'[-+]?)(?:(?'hour'[0-9]|[1][0-9]|[2][0-3])[ʰh])(?:(?'minute'[0-9]|[1-5][0-9])['](?:(?'second'[0-9]|[1-5][0-9])(?:[{0}](?'hundredth'[0-9]{{2}}))?[""])?)?$";
    private const string NumericPattern = @"^(?'start'(?'angle'[-+]?[0-9]+(?:[{0}][0-9]+)?(?:[eE][-+]?[0-9]+)?))(?:(?'radian-start'(?=[r]))|(?'hour-start'(?=[ʰh]?)))[ʰhr]?(?(start)(?!))$";

    private int _units;
		public static readonly HourAngle MaxValue = new HourAngle(MaxTotalUnits - 1);
		public static readonly HourAngle MinValue = new HourAngle(-MaxTotalUnits + 1);
 
		#region Constructors

		public HourAngle(int units)
		{
			if (units < -MaxTotalUnits || units > MaxTotalUnits)
				throw new ArgumentOutOfRangeException("units");
			_units = units;
		}

		public HourAngle(int hour, int minute, int second)
			: this(hour, minute, second, 0, false)
		{
		}

		public HourAngle(int hour, int minute, int second, bool negative)
			: this(hour, minute, second, 0, negative)
		{
		}

		public HourAngle(int hour, int minute, int second, int hundredth)
			: this(hour, minute, second, hundredth, false)
		{
		}

		public HourAngle(int hour, int minute, int second, int hundredth, bool negative)
		{
			if (hour < 0 || hour >= MaxTotalHours)
				throw new ArgumentOutOfRangeException("hour");
			if (minute < 0 || minute >= MinutesPerHour)
				throw new ArgumentOutOfRangeException("minute");
			if (second < 0 || second >= SecondsPerMinute)
				throw new ArgumentOutOfRangeException("second");
			if (hundredth < 0 || hundredth >= HundredthsPerSecond)
				throw new ArgumentOutOfRangeException("hundredth");
			int value = hour * HundredthsPerHour + minute * HundredthsPerMinute + second * HundredthsPerSecond + hundredth;
			if (negative)
				value = -value ;
			if (value <= -MaxTotalUnits || value >= MaxTotalUnits)
				throw new ArgumentOutOfRangeException(null, "Inconsistenarguments item");
			_units = value ;
		}

		public HourAngle(double hours)
		{
      _units = (int)global::System.Math.Round(Angle.NormHour(hours) * HundredthsPerHour);
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

		public int Hours
		{
			get
			{
				return global::System.Math.Abs(_units / HundredthsPerHour);
			}
		}

		public int Minutes
		{
			get
			{
				return global::System.Math.Abs(_units % HundredthsPerHour / HundredthsPerMinute);
			}
		}

		public int Seconds
		{
			get
			{
				return global::System.Math.Abs(_units % HundredthsPerHour % HundredthsPerMinute / HundredthsPerSecond);
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

		public static bool operator==(HourAngle op1, HourAngle op2)
		{
			return op1._units == op2._units;
		}

		public static bool operator!=(HourAngle op1, HourAngle op2)
		{
			return op1._units != op2._units;
		}

		public static bool operator>(HourAngle op1, HourAngle op2)
		{
			return op1._units > op2._units;
		}

		public static bool operator>=(HourAngle op1, HourAngle op2)
		{
			return op1._units >= op2._units;
		}

		public static bool operator<(HourAngle op1, HourAngle op2)
		{
			return op1._units < op2._units;
		}

		public static bool operator<=(HourAngle op1, HourAngle op2)
		{
			return op1._units <= op2._units;
		}

    #endregion
    #region Mathematic

    public static HourAngle operator+(HourAngle x, HourAngle y)
    {
      return x.Add(y);
    }

    public static HourAngle operator-(HourAngle x, HourAngle y)
    {
      return x.Subtract(y);
    }

    public static HourAngle operator*(HourAngle x, int y)
    {
      return x.Multiply(y);
    }

    public static HourAngle operator/(HourAngle x, int y)
    {
      return x.Divide(y);
    }

    #endregion
    #endregion
    #region Methods

    public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != typeof(HourAngle))
				return false;
			return this == (HourAngle)obj;
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
			return string.Format(@"{0}{1}ʰ{2:0#}'{3:0#}{4}""", Negative ? "-" : "", Hours, Minutes, Seconds, Hundredths != 0 ? string.Format("{0}{1:0#}", culture.NumberFormat.NumberDecimalSeparator, Hundredths) : "");
		}

		public double ToRadian()
		{
			return Angle.FromHour(ToDegree());
		}

		public double ToDegree()
		{
			return (double)Hours + (double)Minutes / (double)MinutesPerHour + (double)Seconds / (double)SecondsPerHour + (double)Hundredths / (double)HundredthsPerHour;
		}

    public HourAngle Add(HourAngle op)
    {
      return new HourAngle((int)(((long)this._units + (long)op._units) % (long)MaxTotalUnits));
    }

    public HourAngle Subtract(HourAngle op)
    {
      return new HourAngle((int)(((long)this._units - (long)op._units) % (long)MaxTotalUnits));
    }

    public HourAngle Multiply(int op)
    {
      return new HourAngle((int)(((long)this._units * (long)op) % (long)MaxTotalUnits));
    }

    public HourAngle Divide(int op)
    {
      return new HourAngle((int)(((long)this._units / (long)op) % (long)MaxTotalUnits));
    }

    #endregion
    #region Static methods

    public static HourAngle FromRadian(double radian)
		{
      return new HourAngle(Angle.ToHour(radian));
		}

		public static HourAngle Parse(string s)
		{
			HourAngle result;
			if (!TryParse(s, out result))
				throw new ArgumentException("Invalid input string");
			return result;
		}

		public static HourAngle Parse(string s, IFormatProvider provider)
		{
			HourAngle result;
			if (!TryParse(s, provider, out result))
				throw new ArgumentException("Invalid input string");
			return result;
		}

		public static bool TryParse(string s, IFormatProvider provider, out HourAngle result)
		{
      if (provider == null)
        provider = CultureInfo.CurrentCulture;
      NumberFormatInfo nfi = (NumberFormatInfo)provider.GetFormat(typeof(NumberFormatInfo));
      string pattern = string.Format(PatternFormat, nfi.NumberDecimalSeparator);
			Match match = Regex.Match(s, pattern);
			if (match.Success)
			{
				int degree = string.IsNullOrEmpty(match.Groups["hour"].Value) ? 0 : int.Parse(match.Groups["hour"].Value);
				int minute = string.IsNullOrEmpty(match.Groups["minute"].Value) ? 0 : int.Parse(match.Groups["minute"].Value);
				int second = string.IsNullOrEmpty(match.Groups["second"].Value) ? 0 : int.Parse(match.Groups["second"].Value);
				int hundredth = string.IsNullOrEmpty(match.Groups["hundredth"].Value) ? 0 : int.Parse(match.Groups["hundredth"].Value);
				bool isNegative = match.Groups["sign"].Value== "-";
				result = (isNegative) ? new HourAngle(-degree, -minute, -second, -hundredth) : new HourAngle(degree, minute, second, hundredth);
        return true;
			}
      else
      {
        pattern = string.Format(NumericPattern, nfi.NumberDecimalSeparator);
        match = Regex.Match(s, pattern);
        if (match.Success)
        {
          double angle;
          if (match.Groups["hour"].Success && double.TryParse(match.Groups["angle"].Value, NumberStyles.Float, provider, out angle))
          {
            result = new HourAngle(angle);
            return true;
          }
          else if (match.Groups["radian"].Success && double.TryParse(match.Groups["angle"].Value, NumberStyles.Float, provider, out angle))
          {
            result = HourAngle.FromRadian(angle);
            return true;
          }
        }
      }
      result = new HourAngle();
      return false;
    }

    public static bool TryParse(string s, out HourAngle result)
		{
			return TryParse(s, null, out result);
		}

		#endregion
	}
}
