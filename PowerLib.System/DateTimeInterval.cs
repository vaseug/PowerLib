using System;
using System.Text;
using System.Globalization;
using PowerLib.System.ComponentModel;


namespace PowerLib.System
{
	public struct DateTimeInterval
	{
		private DateTime _dateTime;
		private TimeSpan _timeSpan;

    public DateTimeInterval(DateTime dateTime, TimeSpan timeSpan)
    {
      if (timeSpan < DateTime.MinValue - dateTime || timeSpan > DateTime.MaxValue - dateTime)
        throw new ArgumentException("DateTime interval is out of range");

      _dateTime = dateTime;
      _timeSpan = timeSpan;
    }

    public DateTimeInterval(DateTime first, DateTime second)
    {
      _dateTime = first <= second ? first : second;
      _timeSpan = first <= second ? second - first : first - second;
    }

    public DateTime DateTimeStart
		{
			get
			{
				return _timeSpan >= TimeSpan.Zero ? _dateTime : _dateTime + _timeSpan;
			}
		}

		public DateTime DateTimeEnd
		{
			get
			{
				return _timeSpan >= TimeSpan.Zero ? _dateTime + _timeSpan : _dateTime;
			}
		}

    public DateTime DateTime
    {
      get
      {
        return _dateTime;
      }
    }

		public TimeSpan TimeSpan
		{
			get
			{
				return _timeSpan;
			}
		}

		public DateTimeInterval Shift(TimeSpan shift)
		{
			return new DateTimeInterval(DateTime + TimeSpan, TimeSpan);
		}

    public DateTimeInterval Shift(TimeSpan shiftStart, TimeSpan shiftEnd)
    {
      if (shiftStart < DateTime.MinValue - DateTimeStart || shiftStart > DateTime.MaxValue - DateTimeStart ||
        shiftEnd < DateTime.MinValue - DateTimeEnd || shiftEnd > DateTime.MaxValue - DateTimeEnd)
        throw new ArgumentException("DateTime interval is out of range");

      return new DateTimeInterval(DateTimeStart + shiftStart, DateTimeEnd + shiftEnd);
    }

    public DateTimeIntervalMatchResult Match(DateTimeInterval dtm)
		{
			return Match(this, dtm);
		}

		public static DateTimeIntervalMatchResult Match(DateTimeInterval dtmFirst, DateTimeInterval dtmSecond)
		{
			if (dtmFirst.DateTimeEnd < dtmSecond.DateTimeStart)
				return DateTimeIntervalMatchResult.Before;
			else if (dtmFirst.DateTimeStart > dtmSecond.DateTimeEnd)
				return DateTimeIntervalMatchResult.After;
			else if (dtmFirst.DateTimeStart == dtmSecond.DateTimeStart && dtmFirst.DateTimeEnd == dtmSecond.DateTimeEnd)
				return DateTimeIntervalMatchResult.Equal;
			else if (dtmFirst.DateTimeStart >= dtmSecond.DateTimeStart && dtmFirst.DateTimeEnd <= dtmSecond.DateTimeEnd)
				return DateTimeIntervalMatchResult.Belong;
			else if (dtmFirst.DateTimeStart < dtmSecond.DateTimeStart && dtmFirst.DateTimeEnd > dtmSecond.DateTimeEnd)
				return DateTimeIntervalMatchResult.Enclose;
			else if (dtmFirst.DateTimeStart < dtmSecond.DateTimeStart)
				return DateTimeIntervalMatchResult.OverlapBefore;
			else if (dtmFirst.DateTimeEnd > dtmSecond.DateTimeEnd)
				return DateTimeIntervalMatchResult.OverlapAfter;
			else
				throw new InvalidOperationException();
		}
	}

  public enum DateTimeIntervalMatchResult
  {
    [DisplayStringResource(typeof(DateTimeIntervalMatchResult), "PowerLib.System.DateTimeInterval", "DateTimeMatchResult_Before")]
    Before,
    [DisplayStringResource(typeof(DateTimeIntervalMatchResult), "PowerLib.System.DateTimeInterval", "DateTimeMatchResult_After")]
    After,
    [DisplayStringResource(typeof(DateTimeIntervalMatchResult), "PowerLib.System.DateTimeInterval", "DateTimeMatchResult_OverlapBefore")]
    OverlapBefore,
    [DisplayStringResource(typeof(DateTimeIntervalMatchResult), "PowerLib.System.DateTimeInterval", "DateTimeMatchResult_OverlapAfter")]
    OverlapAfter,
    [DisplayStringResource(typeof(DateTimeIntervalMatchResult), "PowerLib.System.DateTimeInterval", "DateTimeMatchResult_Equal")]
    Equal,
    [DisplayStringResource(typeof(DateTimeIntervalMatchResult), "PowerLib.System.DateTimeInterval", "DateTimeMatchResult_Belong")]
    Belong,
    [DisplayStringResource(typeof(DateTimeIntervalMatchResult), "PowerLib.System.DateTimeInterval", "DateTimeMatchResult_Enclose")]
    Enclose,
  }
}
