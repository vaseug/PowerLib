using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PowerLib.System.Numerics
{
	/// <summary>
	/// Represents measure of angle in sexagesimal numeral system (bas60).
	/// </summary>
	[TypeConverter(typeof(SexagesimalAngleConverter))]
	public struct SexagesimalAngle : IAngle<double>
	{
		public const int MaxTotalDegrees = 360;
		public const int MinutesPerDegree = 60;
		public const int SecondsPerMinute = 60;
		public const int HundredthsPerSecond = 100;
		public const int SecondsPerDegree = MinutesPerDegree * SecondsPerMinute;
		public const int HundredthsPerMinute = SecondsPerMinute * HundredthsPerSecond;
		public const int HundredthsPerDegree = MinutesPerDegree * SecondsPerMinute * HundredthsPerSecond;
		public const int MaxTotalUnits = MaxTotalDegrees * MinutesPerDegree * SecondsPerMinute * HundredthsPerSecond;
		private const string PatternFormat = @"^(?'sign'[-+]?)(?:(?'degree'[0-9]|[1-9][0-9]|[1-2][0-9]{{2}}|[3][0-5][0-9])[°d])(?:(?'minute'[0-9]|[1-5][0-9])['](?:(?'second'[0-9]|[1-5][0-9])(?:[{0}](?'hundredth'[0-9]{{2}}))?[""])?)?$";
    private const string NumericPattern = @"^(?'start'(?'angle'[-+]?[0-9]+(?:[{0}][0-9]+)?(?:[eE][-+]?[0-9]+)?))(?:(?'radian-start'(?=[r]))|(?'degree-start'(?=[°d]?)))[°dr]?(?(start)(?!))$";

    private int _units;
		public static readonly SexagesimalAngle MaxValue= new SexagesimalAngle(MaxTotalUnits - 1);
		public static readonly SexagesimalAngle MinValue= new SexagesimalAngle(-MaxTotalUnits + 1);

		#region Constructors

		public SexagesimalAngle(int units)
		{
			if (units < -MaxTotalUnits || units > MaxTotalUnits)
				throw new ArgumentOutOfRangeException("units");
			_units = units;
		}

		public SexagesimalAngle(int degrees, int minutes, int seconds)
			: this(degrees, minutes, seconds, 0)
		{
		}

		public SexagesimalAngle(int degrees, int minutes, int seconds, bool negative)
			: this(degrees, minutes, seconds, 0, negative)
		{
		}

		public SexagesimalAngle(int degrees, int minutes, int seconds, int hundredths)
			: this(degrees, minutes, seconds, hundredths, false)
		{
		}

		public SexagesimalAngle(int degrees, int minutes, int seconds, int hundredths, bool negative)
		{
			if (degrees < 0 || degrees >= MaxTotalDegrees)
				throw new ArgumentOutOfRangeException("degrees");
			if (minutes < 0 || minutes >= MinutesPerDegree)
				throw new ArgumentOutOfRangeException("minutes");
			if (seconds < 0 || seconds >= SecondsPerMinute)
				throw new ArgumentOutOfRangeException("seconds");
			if (hundredths < 0 || hundredths >= HundredthsPerSecond)
				throw new ArgumentOutOfRangeException("hundredths");
			int value = degrees * HundredthsPerDegree + minutes * HundredthsPerMinute + seconds * HundredthsPerSecond + hundredths;
			if (negative)
				value = -value ;
			if (value <= -MaxTotalUnits || value >= MaxTotalUnits)
				throw new ArgumentOutOfRangeException(null, "Inconsistenarguments item");
			_units = value ;
		}

		public SexagesimalAngle(double degrees)
		{
      _units = (int)global::System.Math.Round(Angle.NormDegree(degrees) * HundredthsPerDegree);
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

		public int Degrees
		{
			get
			{
				return global::System.Math.Abs(_units / HundredthsPerDegree);
			}
		}

		public int Minutes
		{
			get
			{
				return global::System.Math.Abs(_units % HundredthsPerDegree / HundredthsPerMinute);
			}
		}

		public int Seconds
		{
			get
			{
				return global::System.Math.Abs(_units % HundredthsPerDegree % HundredthsPerMinute / HundredthsPerSecond);
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

		public static bool operator==(SexagesimalAngle op1, SexagesimalAngle op2)
		{
			return op1._units == op2._units;
		}

		public static bool operator!=(SexagesimalAngle op1, SexagesimalAngle op2)
		{
			return op1._units != op2._units;
		}

		public static bool operator>(SexagesimalAngle op1, SexagesimalAngle op2)
		{
			return op1._units > op2._units;
		}

		public static bool operator>=(SexagesimalAngle op1, SexagesimalAngle op2)
		{
			return op1._units >= op2._units;
		}

		public static bool operator<(SexagesimalAngle op1, SexagesimalAngle op2)
		{
			return op1._units < op2._units;
		}

		public static bool operator<=(SexagesimalAngle op1, SexagesimalAngle op2)
		{
			return op1._units <= op2._units;
		}

		#endregion
		#region Mathematic

		public static SexagesimalAngle operator+(SexagesimalAngle x, SexagesimalAngle y)
		{
      return x.Add(y);
		}

		public static SexagesimalAngle operator-(SexagesimalAngle x, SexagesimalAngle y)
		{
      return x.Subtract(y);
		}

    public static SexagesimalAngle operator*(SexagesimalAngle x, int y)
    {
      return x.Multiply(y);
    }

    public static SexagesimalAngle operator/(SexagesimalAngle x, int y)
    {
      return x.Divide(y);
    }

    #endregion
    #endregion
    #region Methods

    public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != typeof(SexagesimalAngle))
				return false;
			return this == (SexagesimalAngle)obj;
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
			return string.Format(@"{0}{1}°{2:0#}'{3:0#}{4}""", Negative ? "-" : "", Degrees, Minutes, Seconds, Hundredths != 0 ? string.Format("{0}{1:0#}", culture.NumberFormat.NumberDecimalSeparator, Hundredths) : "");
		}

		public double ToRadian()
		{
      return Angle.FromDegree(ToDegree());
		}

		public double ToDegree()
		{
			return (double)Degrees + (double)Minutes / (double)MinutesPerDegree + (double)Seconds / (double)SecondsPerDegree + (double)Hundredths / (double)HundredthsPerDegree;
		}

    public SexagesimalAngle Add(SexagesimalAngle op)
    {
      return new SexagesimalAngle((int)(((long)this._units + (long)op._units) % (long)MaxTotalUnits));
    }

    public SexagesimalAngle Subtract(SexagesimalAngle op)
    {
      return new SexagesimalAngle((int)(((long)this._units - (long)op._units) % (long)MaxTotalUnits));
    }

    public SexagesimalAngle Multiply(int op)
    {
      return new SexagesimalAngle((int)(((long)this._units * (long)op) % (long)MaxTotalUnits));
    }

    public SexagesimalAngle Divide(int op)
    {
      return new SexagesimalAngle((int)(((long)this._units / (long)op) % (long)MaxTotalUnits));
    }

    #endregion
    #region Static methods

    public static SexagesimalAngle FromRadian(double radian)
		{
      return new SexagesimalAngle(Angle.ToDegree(radian));
		}

		public static SexagesimalAngle Parse(string s)
		{
			SexagesimalAngle result;
			if (!TryParse(s, out result))
				throw new ArgumentException("Invalid input string");
			return result;
		}

		public static SexagesimalAngle Parse(string s, IFormatProvider provider)
		{
			SexagesimalAngle result;
			if (!TryParse(s, provider, out result))
				throw new ArgumentException("Invalid input string");
			return result;
		}

		public static bool TryParse(string s, IFormatProvider provider, out SexagesimalAngle result)
		{
      if (provider == null)
        provider = CultureInfo.CurrentCulture;
      NumberFormatInfo nfi = (NumberFormatInfo)provider.GetFormat(typeof(NumberFormatInfo));
      string pattern = string.Format(PatternFormat, nfi.NumberDecimalSeparator);
			Match match = Regex.Match(s, pattern);
      if (match.Success)
      {
        int degree = string.IsNullOrEmpty(match.Groups["degree"].Value) ? 0 : int.Parse(match.Groups["degree"].Value);
        int minute = string.IsNullOrEmpty(match.Groups["minute"].Value) ? 0 : int.Parse(match.Groups["minute"].Value);
        int second = string.IsNullOrEmpty(match.Groups["second"].Value) ? 0 : int.Parse(match.Groups["second"].Value);
        int hundredth = string.IsNullOrEmpty(match.Groups["hundredth"].Value) ? 0 : int.Parse(match.Groups["hundredth"].Value);
        bool negative = match.Groups["sign"].Value== "-";
        result = new SexagesimalAngle(degree, minute, second, hundredth, negative);
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
            result = new SexagesimalAngle(angle);
            return true;
          }
          else if (match.Groups["radian"].Success && double.TryParse(match.Groups["angle"].Value, NumberStyles.Float, provider, out angle))
          {
            result = SexagesimalAngle.FromRadian(angle);
            return true;
          }
        }
      }
      result = new SexagesimalAngle();
      return false;
		}

		public static bool TryParse(string s, out SexagesimalAngle result)
		{
			return TryParse(s, null, out result);
		}

		#endregion
	}
}
