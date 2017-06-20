using System;


namespace PowerLib.System
{
	/// <summary>
	/// Represents inforamtion about regular long array dimension (length, lower and upper bound).
	/// </summary>
	public struct ArrayLongDimension
	{
		private long _length;
		private long _lowerBound;

		#region Constructors

		public ArrayLongDimension(long length)
			: this(length, 0L)
		{
		}

		public ArrayLongDimension(long length, long lowerBound)
		{
			if (length < 0L)
				throw new ArgumentOutOfRangeException("length");
			if (lowerBound < 0L)
				throw new ArgumentOutOfRangeException("lowerBound");
			if (lowerBound > int.MaxValue - length)
				throw new ArgumentException("Inconsistent array dimensiolength anlowerBound");
			//
			_length = length;
			_lowerBound = lowerBound;
		}

		#endregion
		#region Properties

		public long Length
		{
			get
			{
				return _length;
			}
		}

		public long LowerBound
		{
			get
			{
				return _lowerBound;
			}
		}

		public long UpperBound
		{
			get
			{
				return Length == 0 ? LowerBound : LowerBound + Length - 1;
			}
		}

		#endregion
	}
}
