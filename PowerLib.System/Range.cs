using System;
using System.Globalization;

namespace PowerLib.System
{
	public struct Range : IFormattable
	{
    public static readonly Range[] Empty = new Range[0];
    private int _index;
		private int _count;

		#region Constructors

		public Range(int index, int count)
		{
			_index = index;
			_count = count;
		}

		#endregion
		#region Properties

		public int Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = value ;
			}
		}

		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value ;
			}
		}

    #endregion
    #region Methods

    public static Range Parse(string s)
    {
      return Parse(s, NumberStyles.Integer, null);
    }

    public static Range Parse(string s, NumberStyles styles)
    {
      return Parse(s, styles, null);
    }

    public static Range Parse(string s, IFormatProvider formatProvider)
    {
      return Parse(s, NumberStyles.Integer, formatProvider);
    }

    public static Range Parse(string s, NumberStyles styles, IFormatProvider formatProvider)
    {
      Range result;
      if (!TryParse(s, styles, formatProvider, out result))
        throw new FormatException("Invalid input string");
      return result;
    }

    public static bool TryParse(string s, out Range result)
    {
      return TryParse(s, NumberStyles.Integer, null, out result);
    }

    public static bool TryParse(string s, NumberStyles styles, out Range result)
    {
      return TryParse(s, styles, null, out result);
    }

    public static bool TryParse(string s, IFormatProvider provider, out Range result)
    {
      return TryParse(s, NumberStyles.Integer, provider, out result);
    }

    public static bool TryParse(string s, NumberStyles styles, IFormatProvider provider, out Range result)
    {
      return RangeParseFormatInfo.Default.TryParse(s, styles, provider, out result);
    }

    public override string ToString()
    {
      return RangeParseFormatInfo.Default.Format(this, null, null);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
      return RangeParseFormatInfo.Default.Format(this, format, formatProvider);
    }

    #endregion
  }
}
