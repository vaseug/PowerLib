using System;
using System.Numerics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using PowerLib.System.Numerics;

namespace PowerLib.System.Numerics
{
	public static class Numeral
	{
    #region Abs operations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte Abs(this sbyte value)
		{
			return (sbyte)(value < 0 ? -value : value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short Abs(this short value)
		{
			return (short)(value < 0 ? -value : value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Abs(this int value)
		{
			return value < 0 ? -value : value ;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Abs(this long value)
		{
			return value < 0 ? -value : value ;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Abs(this float value)
		{
			return value < 0 ? -value : value ;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Abs(this double value)
		{
			return value < 0 ? -value : value ;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Abs(this decimal value)
		{
			return value < 0 ? -value : value ;
		}

		#endregion
		#region Sign operations

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this sbyte value)
		{
			return value < 0 ? -1 : value > 0 ? 1 : 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this short value)
		{
			return value < 0 ? -1 : value > 0 ? 1 : 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this int value)
		{
			return value < 0 ? -1 : value > 0 ? 1 : 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this long value)
		{
			return value < 0 ? -1 : value > 0 ? 1 : 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this float value)
		{
			return value < 0 ? -1 : value > 0 ? 1 : 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this double value)
		{
			return value < 0 ? -1 : value > 0 ? 1 : 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(this decimal value)
		{
			return value < 0 ? -1 : value > 0 ? 1 : 0;
		}

    #endregion
  }
}
