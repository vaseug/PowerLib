using System;
using System.Globalization;

namespace PowerLib.System
{
	public struct LongRange : IFormattable
	{
		public static readonly LongRange[] Empty = new LongRange[0];
		private long _index;
		private long _count;

		#region Constructors

		public LongRange(long index, long count)
		{
			_index = index;
			_count = count;
		}

		#endregion
		#region Properties

		public long Index
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

		public long Count
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

    public static LongRange Parse(string s)
    {
      return Parse(s, NumberStyles.Integer, null);
    }

    public static LongRange Parse(string s, NumberStyles styles)
    {
      return Parse(s, styles, null);
    }

    public static LongRange Parse(string s, IFormatProvider formatProvider)
    {
      return Parse(s, NumberStyles.Integer, formatProvider);
    }

    public static LongRange Parse(string s, NumberStyles styles, IFormatProvider formatProvider)
    {
      LongRange result;
      if (!TryParse(s, styles, formatProvider, out result))
        throw new FormatException("Invalid input string");
      return result;
    }

    public static bool TryParse(string s, out LongRange result)
    {
      return TryParse(s, NumberStyles.Integer, null, out result);
    }

    public static bool TryParse(string s, NumberStyles styles, out LongRange result)
    {
      return TryParse(s, styles, null, out result);
    }

    public static bool TryParse(string s, IFormatProvider provider, out LongRange result)
    {
      return TryParse(s, NumberStyles.Integer, provider, out result);
    }

    public static bool TryParse(string s, NumberStyles styles, IFormatProvider provider, out LongRange result)
    {
      return LongRangeParseFormatInfo.Default.TryParse(s, styles, provider, out result);
    }

    public override string ToString()
    {
      return LongRangeParseFormatInfo.Default.Format(this, null, null);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
      return LongRangeParseFormatInfo.Default.Format(this, format, formatProvider);
    }

    #endregion
  }
}
