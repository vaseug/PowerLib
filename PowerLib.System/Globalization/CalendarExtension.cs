using System;
using System.Globalization;


namespace PowerLib.System.Globalization
{
	/// <summary>
	/// Calendar extension methods
	/// </summary>
	public static class CalendarExtension
	{
		public const int FractionTicks = 10000;
		public const int MillisecondsPerSecond = 1000;
		public const int SecondsPerMinute = 60;
		public const int MinutesPerHour = 60;
		public const int HoursPerDay = 24;
		public const int DaysInWeek = 7;

		/// <summary>
		/// Get day ordinal number in week
		/// </summary>
		/// <param name="currDayOfWeek">Current day of week</param>
		/// <param name="firstDayOfWeek">First day of week</param>
		/// <returns>Day ordinal number</returns>
		public static int GetOrdinalByDayOfWeek(DayOfWeek currDayOfWeek, DayOfWeek firstDayOfWeek)
		{
			return (int)currDayOfWeek - (int)firstDayOfWeek + ((currDayOfWeek >= firstDayOfWeek) ? 1 : 8);
		}

		/// <summary>
		/// Get day of week
		/// </summary>
		/// <param name="ordinal">Day ordinal number in week</param>
		/// <param name="firstDayOfWeek">First day of week</param>
		/// <returns>Day of week</returns>
		public static DayOfWeek GetDayOfWeekByOrdinal(int ordinal, DayOfWeek firstDayOfWeek)
		{
			if (ordinal < 1 || ordinal > 7)
				throw new ArgumentOutOfRangeException("ordinal");
			return (DayOfWeek)Enum.ToObject(typeof(DayOfWeek), ((int)firstDayOfWeek + ordinal - 1) % 7);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="millenia"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddMillenia(this Calendar calendar, DateTime dtm, int millenia, bool eraBound)
		{
			DateTime dtmResult = calendar.AddYears(dtm, millenia * 1000);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="centuries"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddCenturies(this Calendar calendar, DateTime dtm, int centuries, bool eraBound)
		{
			DateTime dtmResult = calendar.AddYears(dtm, centuries * 100);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;	
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="years"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddYears(this Calendar calendar, DateTime dtm, int years, bool eraBound)
		{
			DateTime dtmResult = calendar.AddYears(dtm, years);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="halves"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddHalves(this Calendar calendar, DateTime dtm, int halves, bool eraBound)
		{
			int monthsInYear = calendar.GetMonthsInYear(calendar.GetYear(dtm), calendar.GetEra(dtm));
			if (monthsInYear % 2 != 0)
				throw new InvalidOperationException();
			DateTime dtmResult = calendar.AddYears(dtm, halves / 2);
			dtmResult = calendar.AddMonths(dtmResult, monthsInYear / 2 * halves % 2);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="quarters"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddQuarters(this Calendar calendar, DateTime dtm, int quarters, bool eraBound)
		{
			int monthsInYear = calendar.GetMonthsInYear(calendar.GetYear(dtm), calendar.GetEra(dtm));
			if (monthsInYear % 4 != 0)
				throw new InvalidOperationException();
			DateTime dtmResult = calendar.AddYears(dtm, quarters / 4);
			dtmResult = calendar.AddMonths(dtm, monthsInYear / 4 * quarters % 4);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="months"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddMonths(this Calendar calendar, DateTime dtm, int months, bool eraBound)
		{
			DateTime dtmResult = calendar.AddMonths(dtm, months);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="weeks"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddWeeks(this Calendar calendar, DateTime dtm, int weeks, bool eraBound)
		{
			DateTime dtmResult = calendar.AddWeeks(dtm, weeks);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="weeks"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddWeeks(this Calendar calendar, DateTime dtm, int weeks, DayOfWeek firstDayOfWeek, bool eraBound)
		{
			DateTime dtmResult;
			if (weeks > 0)
			{
				dtmResult = calendar.AddDays(dtm, 8 - GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(dtm), firstDayOfWeek));
				weeks -= 1;
			}
			else if (weeks < 0)
			{
				dtmResult = calendar.AddDays(dtm, -6 - GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(dtm), firstDayOfWeek));
				weeks += 1;
			}
			else
				dtmResult = dtm;
			if (weeks != 0)
				dtmResult = calendar.AddWeeks(dtmResult, weeks);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="days"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddDays(this Calendar calendar, DateTime dtm, int days, bool eraBound)
		{
			DateTime dtmResult = calendar.AddDays(dtm, days);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="hours"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddHours(this Calendar calendar, DateTime dtm, int hours, bool eraBound)
		{
			DateTime dtmResult = calendar.AddHours(dtm, hours);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="minutes"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddMinutes(this Calendar calendar, DateTime dtm, int minutes, bool eraBound)
		{
			DateTime dtmResult = calendar.AddMinutes(dtm, minutes);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="seconds"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddSeconds(this Calendar calendar, DateTime dtm, int seconds, bool eraBound)
		{
			DateTime dtmResult = calendar.AddSeconds(dtm, seconds);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="milliseconds"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddMilliseconds(this Calendar calendar, DateTime dtm, int milliseconds, bool eraBound)
		{
			DateTime dtmResult = calendar.AddMilliseconds(dtm, milliseconds);
			if (eraBound && calendar.GetEra(dtm) != calendar.GetEra(dtmResult))
				throw new ArgumentException("The resulting DateTime is outside the supported range of era");
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="dtmPrecision"></param>
		/// <param name="units"></param>
		/// <param name="eraBound"></param>
		/// <returns></returns>
		public static DateTime AddUnits(this Calendar calendar, DateTime dtm, DateTimePrecision dtmPrecision, int units, bool eraBound)
		{
			switch (dtmPrecision)
			{
				case DateTimePrecision.Millenium:
					return AddMillenia(calendar, dtm, units, eraBound);
				case DateTimePrecision.Century:
					return AddCenturies(calendar, dtm, units, eraBound);
				case DateTimePrecision.Year:
					return AddYears(calendar, dtm, units, eraBound);
				case DateTimePrecision.Half:
					return AddHalves(calendar, dtm, units, eraBound);
				case DateTimePrecision.Quarter:
					return AddQuarters(calendar, dtm, units, eraBound);
				case DateTimePrecision.Month:
					return AddMonths(calendar, dtm, units, eraBound);
				case DateTimePrecision.Week:
					return AddWeeks(calendar, dtm, units, eraBound);
				case DateTimePrecision.Day:
					return AddDays(calendar, dtm, units, eraBound);
				case DateTimePrecision.Hour:
					return AddHours(calendar, dtm, units, eraBound);
				case DateTimePrecision.Minute:
					return AddMinutes(calendar, dtm, units, eraBound);
				case DateTimePrecision.Second:
					return AddSeconds(calendar, dtm, units, eraBound);
				case DateTimePrecision.Millisecond:
					return AddMilliseconds(calendar, dtm, units, eraBound);
				default:
					throw new ArgumentException("Invalid datetime precision", "dtmPrecision");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <returns></returns>
		public static int GetMonthsInYear(this Calendar calendar, int year)
		{
			return calendar.GetMonthsInYear(year);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static int GetMonthsInYear(this Calendar calendar, int year, int era)
		{
			return calendar.GetMonthsInYear(year, era);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <returns></returns>
		public static int GetWeeksInYear(this Calendar calendar, int year, DayOfWeek firstDayOfWeek)
		{
			int daysInYear = calendar.GetDaysInYear(year);
			int partialDays = (8 - GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(calendar.ToDateTime(year, 1, 1, 0, 0, 0, 0)), firstDayOfWeek)) % 7;
			return ((partialDays > 0) ? 1 : 0) + (daysInYear - partialDays) / 7 + (((daysInYear - partialDays) % 7 > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static int GetWeeksInYear(this Calendar calendar, int year, DayOfWeek firstDayOfWeek, int era)
		{
			int daysInYear = calendar.GetDaysInYear(year, era);
			int partialDays = (8 - GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(calendar.ToDateTime(year, 1, 1, 0, 0, 0, 0, era)), firstDayOfWeek)) % 7;
			return ((partialDays > 0) ? 1 : 0) + (daysInYear - partialDays) / 7 + (((daysInYear - partialDays) % 7 > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <returns></returns>
		public static int GetWeeksInYear(this Calendar calendar, int year)
		{
			int daysInYear = calendar.GetDaysInYear(year);
			return daysInYear / 7 + ((daysInYear % 7 > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static int GetWeeksInYear(this Calendar calendar, int year, int era)
		{
			int daysInYear = calendar.GetDaysInYear(year, era);
			return daysInYear / 7 + ((daysInYear % 7 > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <returns></returns>
		public static int GetWeeksInMonth(this Calendar calendar, int year, int month, DayOfWeek firstDayOfWeek)
		{
			int daysInMonth = calendar.GetDaysInMonth(year, month);
			int partialDays = (8 - GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(calendar.ToDateTime(year, month, 1, 0, 0, 0, 0)), firstDayOfWeek)) % 7;
			return ((partialDays > 0) ? 1 : 0) + (daysInMonth - partialDays) / 7 + (((daysInMonth - partialDays) % 7 > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static int GetWeeksInMonth(this Calendar calendar, int year, int month, DayOfWeek firstDayOfWeek, int era)
		{
			int daysInMonth = calendar.GetDaysInMonth(year, month, era);
			int partialDays = (8 - GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(calendar.ToDateTime(year, month, 1, 0, 0, 0, 0, era)), firstDayOfWeek)) % 7;
			return ((partialDays > 0) ? 1 : 0) + (daysInMonth - partialDays) / 7 + (((daysInMonth - partialDays) % 7 > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <returns></returns>
		public static int GetWeeksInMonth(this Calendar calendar, int year, int month)
		{
			int daysInMonth = calendar.GetDaysInMonth(year, month);
			return daysInMonth / 7 + ((daysInMonth % 7 > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static int GetWeeksInMonth(this Calendar calendar, int year, int month, int era)
		{
			int daysInMonth = calendar.GetDaysInMonth(year, month, era);
			return daysInMonth / 7 + ((daysInMonth % 7 > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <returns></returns>
		public static int GetDaysInYear(this Calendar calendar, int year)
		{
			return calendar.GetDaysInYear(year);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static int GetDaysInYear(this Calendar calendar, int year, int era)
		{
			return calendar.GetDaysInYear(year, era);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <returns></returns>
		public static int GetDaysInMonth(this Calendar calendar, int year, int month)
		{
			return calendar.GetDaysInMonth(year, month);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static int GetDaysInMonth(this Calendar calendar, int year, int month, int era)
		{
			return calendar.GetDaysInMonth(year, month, era);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetEra(this Calendar calendar, DateTime dtm)
		{
			return calendar.GetEra(dtm);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetMillenium(this Calendar calendar, DateTime dtm)
		{
			return (calendar.GetYear(dtm) - 1) / 1000 + 1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetCentury(this Calendar calendar, DateTime dtm)
		{
			return (calendar.GetYear(dtm) - 1) / 100 + 1;
		}

		public static int GetYear(this Calendar calendar, DateTime dtm)
		{
			return calendar.GetYear(dtm);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetMonth(this Calendar calendar, DateTime dtm)
		{
			return calendar.GetMonth(dtm);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetQuarter(this Calendar calendar, DateTime dtm)
		{
			int monthsInYear = calendar.GetMonthsInYear(calendar.GetMonth(dtm));
			if (monthsInYear % 4 != 0)
				throw new NotSupportedException();
			int monthOfYear = calendar.GetMonth(dtm);
			return (monthOfYear / (monthsInYear / 4)) + ((monthOfYear % (monthsInYear / 4) > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetHalf(this Calendar calendar, DateTime dtm)
		{
			int monthsInYear = calendar.GetMonthsInYear(calendar.GetMonth(dtm));
			if (monthsInYear % 2 != 0)
				throw new NotSupportedException();
			int monthOfYear = calendar.GetMonth(dtm);
			return (monthOfYear / (monthsInYear / 2)) + ((monthOfYear % (monthsInYear / 2) > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="minDaysInFirstWeek"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <returns></returns>
		public static int GetWeekOfYear(this Calendar calendar, DateTime dtm, int minDaysInFirstWeek, DayOfWeek firstDayOfWeek)
		{
			if (minDaysInFirstWeek < 1 || minDaysInFirstWeek > 7)
				throw new ArgumentOutOfRangeException("MinDaysInWeek");
			int dayOfYear = calendar.GetDayOfYear(dtm);
			int daysInYear = calendar.GetDaysInYear(calendar.GetYear(dtm));
			int dayOrdinal = GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(dtm), firstDayOfWeek);
			if (dayOrdinal - dayOfYear > 7 - minDaysInFirstWeek)
				return GetWeekOfYear(calendar, calendar.AddDays(dtm, -dayOfYear), minDaysInFirstWeek, firstDayOfWeek);
			else if (daysInYear - dayOfYear + dayOrdinal <= 7 - minDaysInFirstWeek)
				return GetWeekOfYear(calendar, calendar.AddDays(dtm, daysInYear - dayOfYear + 1), minDaysInFirstWeek, firstDayOfWeek);
			else
			{
				int partialDays = (8 - GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(calendar.AddDays(dtm, -dayOfYear + 1)), firstDayOfWeek)) % 7;
				return ((partialDays >= minDaysInFirstWeek) ? 1 : 0) + (dayOfYear - partialDays) / 7 + (((dayOfYear - partialDays) % 7 > 0) ? 1 : 0);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetWeekOfYear(this Calendar calendar, DateTime dtm)
		{
			int dayOfYear = calendar.GetDayOfYear(dtm);
			return dayOfYear / 7 + ((dayOfYear % 7 > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <returns></returns>
		public static int GetWeekOfMonth(this Calendar calendar, DateTime dtm, DayOfWeek firstDayOfWeek)
		{
			int dayOfMonth = calendar.GetDayOfMonth(dtm);
			int partialDays = (8 - GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(calendar.AddDays(dtm, -calendar.GetDayOfMonth(dtm) + 1)), firstDayOfWeek)) % 7;
			return ((partialDays > 0) ? 1 : 0) + (dayOfMonth - partialDays) / 7 + (((dayOfMonth - partialDays) % 7 > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetWeekOfMonth(this Calendar calendar, DateTime dtm)
		{
			int dayOfMonth = calendar.GetDayOfMonth(dtm);
			return dayOfMonth / 7 + ((dayOfMonth % 7 > 0) ? 1 : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetDayOfYear(this Calendar calendar, DateTime dtm)
		{
			return calendar.GetDayOfYear(dtm);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetDayOfMonth(this Calendar calendar, DateTime dtm)
		{
			return calendar.GetDayOfMonth(dtm);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static DayOfWeek GetDayOfWeek(this Calendar calendar, DateTime dtm)
		{
			return calendar.GetDayOfWeek(dtm);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetHour(this Calendar calendar, DateTime dtm)
		{
			return calendar.GetHour(dtm);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetMinute(this Calendar calendar, DateTime dtm)
		{
			return calendar.GetMinute(dtm);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static int GetSecond(this Calendar calendar, DateTime dtm)
		{
			return calendar.GetSecond(dtm);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <returns></returns>
		public static double GetMilliseconds(this Calendar calendar, DateTime dtm)
		{
			return calendar.GetMilliseconds(dtm);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="day"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this Calendar calendar, int year, int month, int day)
		{
			return calendar.ToDateTime(year, month, day, 0, 0, 0, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="day"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this Calendar calendar, int year, int month, int day, int era)
		{
			return calendar.ToDateTime(year, month, day, 0, 0, 0, 0, era);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="week"></param>
		/// <param name="day"></param>
		/// <param name="minDaysInFirstWeek"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this Calendar calendar, int year, int week, DayOfWeek day, int minDaysInFirstWeek, DayOfWeek firstDayOfWeek)
		{
			if (week < 1)
				throw new ArgumentOutOfRangeException("week");
			DateTime first = calendar.ToDateTime(year, 1, 1, 0, 0, 0, 0);
			int firstOrdinal = GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(first), firstDayOfWeek);
			int dayOrdinal = GetOrdinalByDayOfWeek(day, firstDayOfWeek);
			int daysOffset = dayOrdinal - firstOrdinal;
			if ((week -= (8 - firstOrdinal >= minDaysInFirstWeek) ? 1 : 0) > 0)
				daysOffset += 7 + (week - 1) * 7;
			if (daysOffset >= calendar.GetDaysInYear(year))
				throw new ArgumentException("Exceeds days in year");
			return calendar.AddDays(first, daysOffset);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="week"></param>
		/// <param name="day"></param>
		/// <param name="minDaysInFirstWeek"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this Calendar calendar, int year, int week, DayOfWeek day, int minDaysInFirstWeek, DayOfWeek firstDayOfWeek, int era)
		{
			if (week < 1)
				throw new ArgumentOutOfRangeException("week");
			DateTime first = calendar.ToDateTime(year, 1, 1, 0, 0, 0, 0, era);
			int firstOrdinal = GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(first), firstDayOfWeek);
			int dayOrdinal = GetOrdinalByDayOfWeek(day, firstDayOfWeek);
			int daysOffset = dayOrdinal - firstOrdinal;
			if ((week -= (8 - firstOrdinal >= minDaysInFirstWeek) ? 1 : 0) > 0)
				daysOffset += 7 + (week - 1) * 7;
			if (daysOffset >= calendar.GetDaysInYear(year, era))
				throw new ArgumentException("Exceeds days in year");
			return calendar.AddDays(first, daysOffset);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="week"></param>
		/// <param name="day"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this Calendar calendar, int year, int week, DayOfWeek day)
		{
			DateTime first = ToDateTime(calendar, year, 1, 1);
			int dayOrdinal = GetOrdinalByDayOfWeek(day, calendar.GetDayOfWeek(first));
			int daysOffset = (week - 1) * 7 + dayOrdinal - 1;
			if (daysOffset >= calendar.GetDaysInYear(year))
				throw new ArgumentException("Exceeds days in year");
			return calendar.AddDays(first, daysOffset);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="week"></param>
		/// <param name="day"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this Calendar calendar, int year, int week, DayOfWeek day, int era)
		{
			DateTime first = ToDateTime(calendar, year, 1, 1, era);
			int dayOrdinal = GetOrdinalByDayOfWeek(day, calendar.GetDayOfWeek(first));
			int daysOffset = (week - 1) * 7 + dayOrdinal - 1;
			if (daysOffset >= calendar.GetDaysInYear(year, era))
				throw new ArgumentException("Exceeds days in year");
			return calendar.AddDays(first, daysOffset);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="week"></param>
		/// <param name="day"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this Calendar calendar, int year, int month, int week, DayOfWeek day, DayOfWeek firstDayOfWeek)
		{
			if (week < 1)
				throw new ArgumentOutOfRangeException("week");
			DateTime first = calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);
			int firstOrdinal = GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(first), firstDayOfWeek);
			int dayOrdinal = GetOrdinalByDayOfWeek(day, firstDayOfWeek);
			int daysOffset = dayOrdinal - firstOrdinal;
			if (--week > 0)
				daysOffset += 7 + (week - 1) * 7;
			if (daysOffset >= calendar.GetDaysInMonth(year, month))
				throw new ArgumentException("Exceeds days in month");
			return calendar.AddDays(first, daysOffset);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="week"></param>
		/// <param name="day"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this Calendar calendar, int year, int month, int week, DayOfWeek day, DayOfWeek firstDayOfWeek, int era)
		{
			if (week < 1)
				throw new ArgumentOutOfRangeException("week");
			DateTime first = calendar.ToDateTime(year, month, 1, 0, 0, 0, 0, era);
			int firstOrdinal = GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(first), firstDayOfWeek);
			int dayOrdinal = GetOrdinalByDayOfWeek(day, firstDayOfWeek);
			int daysOffset = dayOrdinal - firstOrdinal;
			if (--week > 0)
				daysOffset += 7 + (week - 1) * 7;
			if (daysOffset >= calendar.GetDaysInMonth(year, month, era))
				throw new ArgumentException("Exceeds days in month");
			return calendar.AddDays(first, daysOffset);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="week"></param>
		/// <param name="day"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this Calendar calendar, int year, int month, int week, DayOfWeek day)
		{
			DateTime first = ToDateTime(calendar, year, month, 1);
			int dayOrdinal = GetOrdinalByDayOfWeek(day, calendar.GetDayOfWeek(first));
			int daysOffset = (week - 1) * 7 + dayOrdinal - 1;
			if (daysOffset >= calendar.GetDaysInMonth(year, month))
				throw new ArgumentException("Exceeds days in month");
			return calendar.AddDays(first, daysOffset);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="week"></param>
		/// <param name="day"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this Calendar calendar, int year, int month, int week, DayOfWeek day, int era)
		{
			DateTime first = ToDateTime(calendar, year, month, 1, era);
			int dayOrdinal = GetOrdinalByDayOfWeek(day, calendar.GetDayOfWeek(first));
			int daysOffset = (week - 1) * 7 + dayOrdinal - 1;
			if (daysOffset >= calendar.GetDaysInMonth(year, month, era))
				throw new ArgumentException("Exceeds days in month");
			return calendar.AddDays(first, daysOffset);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="dtmPrecision"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <returns></returns>
		public static DateTime GetMinDateTime(this Calendar calendar, DateTime dtm, DateTimePrecision dtmPrecision, DayOfWeek firstDayOfWeek)
		{
			DateTime dtmResult;
			if (dtmPrecision == DateTimePrecision.Era)
			{
				dtmResult = GetEraMinDateTime(calendar, calendar.GetEra(dtm));
			}
			else
			{
				dtmResult = dtm;
				if (dtmPrecision != DateTimePrecision.Tick)
				{
					dtmResult -= TimeSpan.FromTicks(dtmResult.Ticks % FractionTicks);
					if (dtmPrecision != DateTimePrecision.Millisecond)
					{
						dtmResult = AddMilliseconds(calendar, dtmResult, -(int)calendar.GetMilliseconds(dtmResult), true);
						if (dtmPrecision != DateTimePrecision.Second)
						{
							dtmResult = AddSeconds(calendar, dtmResult, -calendar.GetSecond(dtmResult), true);
							if (dtmPrecision != DateTimePrecision.Minute)
							{
								dtmResult = AddMinutes(calendar, dtmResult, -calendar.GetMinute(dtmResult), true);
								if (dtmPrecision != DateTimePrecision.Hour)
								{
									dtmResult = AddHours(calendar, dtmResult, -calendar.GetHour(dtmResult), true);
									if (dtmPrecision != DateTimePrecision.Day)
									{
										try
										{
											if (dtmPrecision == DateTimePrecision.Week)
											{
												dtmResult = AddDays(calendar, dtmResult, -(GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(dtmResult), firstDayOfWeek) - 1), true);
											}
											else
											{
												if (dtmPrecision == DateTimePrecision.Quarter)
												{
													int monthsInYear = calendar.GetMonthsInYear(calendar.GetYear(dtmResult), calendar.GetEra(dtmResult));
													if (monthsInYear % 4 != 0)
														throw new InvalidOperationException("Incompatible datetime precision");
													dtmResult = AddMonths(calendar, dtmResult, -(calendar.GetMonth(dtmResult) - 1) % (monthsInYear / 4), true);
												}
												else if (dtmPrecision == DateTimePrecision.Half)
												{
													int monthsInYear = calendar.GetMonthsInYear(calendar.GetYear(dtmResult), calendar.GetEra(dtmResult));
													if (monthsInYear % 2 != 0)
														throw new InvalidOperationException("Incompatible datetime precision");
													dtmResult = AddMonths(calendar, dtmResult, -(calendar.GetMonth(dtmResult) - 1) % (monthsInYear / 2), true);
												}
												else if (dtmPrecision != DateTimePrecision.Month)
												{
													if (dtmPrecision == DateTimePrecision.Millenium)
														dtmResult = AddMillenia(calendar, dtmResult, -(calendar.GetYear(dtmResult) - 1 % 1000), true);
													else if (dtmPrecision == DateTimePrecision.Century)
														dtmResult = AddYears(calendar, dtmResult, -(calendar.GetYear(dtmResult) - 1 % 100), true);

													dtmResult = AddMonths(calendar, dtmResult, -(calendar.GetMonth(dtmResult) - 1), true);
												}
												dtmResult = AddDays(calendar, dtmResult, -(calendar.GetDayOfMonth(dtmResult) - 1), true);
											}
										}
										catch (ArgumentException ex)
										{
											if (ex.ParamName != null)
												throw ex;
											dtmResult = GetEraMinDateTime(calendar, calendar.GetEra(dtm));
										}
									}
								}
							}
						}
					}
				}
			}
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="dtm"></param>
		/// <param name="dtmPrecision"></param>
		/// <param name="firstDayOfWeek"></param>
		/// <returns></returns>
		public static DateTime GetMaxDateTime(this Calendar calendar, DateTime dtm, DateTimePrecision dtmPrecision, DayOfWeek firstDayOfWeek)
		{
			DateTime dtmResult;
			if (dtmPrecision == DateTimePrecision.Era)
			{
				dtmResult = GetEraMaxDateTime(calendar, calendar.GetEra(dtm));
			}
			else
			{
				dtmResult = dtm;
				if (dtmPrecision != DateTimePrecision.Tick)
				{
					dtmResult += TimeSpan.FromTicks(FractionTicks - 1 - dtmResult.Ticks % FractionTicks);
					if (dtmPrecision != DateTimePrecision.Millisecond)
					{
						dtmResult = AddMilliseconds(calendar, dtmResult, MillisecondsPerSecond - 1 - (int)calendar.GetMilliseconds(dtmResult), true);
						if (dtmPrecision != DateTimePrecision.Second)
						{
							dtmResult = AddSeconds(calendar, dtmResult, SecondsPerMinute - 1 - calendar.GetSecond(dtmResult), true);
							if (dtmPrecision != DateTimePrecision.Minute)
							{
								dtmResult = AddMinutes(calendar, dtmResult, MinutesPerHour - 1 - calendar.GetMinute(dtmResult), true);
								if (dtmPrecision != DateTimePrecision.Hour)
								{
									dtmResult = AddHours(calendar, dtmResult, HoursPerDay - 1 - calendar.GetHour(dtmResult), true);
									if (dtmPrecision != DateTimePrecision.Day)
									{
										try
										{
											if (dtmPrecision == DateTimePrecision.Week)
											{
												dtmResult = AddDays(calendar, dtmResult, DaysInWeek - GetOrdinalByDayOfWeek(calendar.GetDayOfWeek(dtmResult), firstDayOfWeek), true);
											}
											else
											{
												if (dtmPrecision == DateTimePrecision.Quarter)
												{
													int monthsInYear = calendar.GetMonthsInYear(calendar.GetYear(dtmResult), calendar.GetEra(dtmResult));
													if (monthsInYear % 4 != 0)
														throw new InvalidOperationException("Incompatible datetime precision");
													dtmResult = AddMonths(calendar, dtmResult, monthsInYear / 4 - calendar.GetMonth(dtmResult) % (monthsInYear / 4), true);
												}
												else if (dtmPrecision == DateTimePrecision.Half)
												{
													int monthsInYear = calendar.GetMonthsInYear(calendar.GetYear(dtmResult), calendar.GetEra(dtmResult));
													if (monthsInYear % 2 != 0)
														throw new InvalidOperationException("Incompatible datetime precision");
													dtmResult = AddMonths(calendar, dtmResult, monthsInYear / 2 - calendar.GetMonth(dtmResult) % (monthsInYear / 2), true);
												}
												else if (dtmPrecision != DateTimePrecision.Month)
												{
													if (dtmPrecision == DateTimePrecision.Millenium)
														dtmResult = AddMillenia(calendar, dtmResult, 1000 -calendar.GetYear(dtmResult) % 100 , true);
													else if (dtmPrecision == DateTimePrecision.Century)
														dtmResult = AddYears(calendar, dtmResult, 100 - calendar.GetYear(dtmResult) % 100, true);
													dtmResult = AddMonths(calendar, dtmResult, calendar.GetMonthsInYear(calendar.GetYear(dtmResult), calendar.GetEra(dtmResult)) - calendar.GetMonth(dtmResult), true);
												}
												dtmResult = AddDays(calendar, dtmResult, calendar.GetDaysInMonth(calendar.GetYear(dtmResult), calendar.GetMonth(dtmResult), calendar.GetEra(dtmResult)) - calendar.GetDayOfMonth(dtmResult), true);
											}
										}
										catch (ArgumentException ex)
										{
											if (ex.ParamName != null)
												throw ex;
											dtmResult = GetEraMaxDateTime(calendar, calendar.GetEra(dtm));
										}
									}
								}
							}
						}
					}
				}
			}
			return dtmResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static DateTime GetEraMinDateTime(this Calendar calendar, int era)
		{
			if (era <= 0 || era > calendar.Eras.Length)
				throw new ArgumentOutOfRangeException("era");
			else if (era == 1)
				return calendar.MinSupportedDateTime;
			else
			{
				DateTime dtm = calendar.ToDateTime(1, 1, 1, 0, 0, 0, 0, era);
				int pastDays = calendar.GetDaysInYear(1, era) - 1;
				DateTime dtmUp = calendar.AddDays(dtm, pastDays);
				while (calendar.GetEra(dtm) != era)
				{
					for (DateTime dtmMid = calendar.AddDays(dtmUp, -(pastDays / 2 + pastDays % 2));
						calendar.GetEra(dtmMid) >= era && pastDays != 0;
						dtmMid = calendar.AddDays(dtmUp, -(pastDays / 2 + pastDays % 2)))
					{
						dtmUp = dtmMid;
						pastDays = (dtmUp - dtm).Days;
					}
					for (DateTime dtmMid = calendar.AddDays(dtm, (pastDays / 2 + pastDays % 2));
						calendar.GetEra(dtmMid) < era || pastDays == 1;
						dtmMid = calendar.AddDays(dtm, (pastDays / 2 + pastDays % 2)))
					{
						dtm = dtmMid;
						pastDays = (dtmUp - dtm).Days;
					}
				}
				return dtm;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="calendar"></param>
		/// <param name="era"></param>
		/// <returns></returns>
		public static DateTime GetEraMaxDateTime(this Calendar calendar, int era)
		{
			if (era <= 0 || era > calendar.Eras.Length)
				throw new ArgumentOutOfRangeException("era");
			else if (era == calendar.Eras.Length)
				return calendar.MaxSupportedDateTime;
			else
				return GetEraMinDateTime(calendar, era + 1) - TimeSpan.FromTicks(1);
		}
	}
}
