using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using PowerLib.System.Linq;

namespace PowerLib.System
{
  public static class PwrBitConverter
  {
    private const string HexPrefix = "0x";

    #region Size methods

    public static int ToSize(Byte[] bytes, int index, SizeEncoding sizeEncoding)
    {
      switch (sizeEncoding)
      {
        case SizeEncoding.B1:
          return (int)bytes[index];
        case SizeEncoding.B2:
          return (int)Convert.ToInt32(BitConverter.ToUInt16(bytes, index));
        case SizeEncoding.B4:
          return BitConverter.ToInt32(bytes, index);
        case SizeEncoding.B8:
          return Convert.ToInt32(BitConverter.ToInt64(bytes, index));
        case SizeEncoding.VB:
          return VbToInt32(bytes, index);
        default:
          throw new ArgumentOutOfRangeException("sizeEncoding");
      }
    }

    public static long ToLongSize(Byte[] bytes, int index, SizeEncoding sizeEncoding)
    {
      switch (sizeEncoding)
      {
        case SizeEncoding.B1:
          return (long)bytes[index];
        case SizeEncoding.B2:
          return (long)BitConverter.ToUInt16(bytes, index);
        case SizeEncoding.B4:
          return (long)BitConverter.ToUInt32(bytes, index);
        case SizeEncoding.B8:
          return BitConverter.ToInt64(bytes, index);
        case SizeEncoding.VB:
          return VbToInt64(bytes, index);
        default:
          throw new ArgumentOutOfRangeException("sizeEncoding");
      }
    }

    public static Byte[] GetSizeEncodingBytes(int size, SizeEncoding sizeEncoding)
    {
      if (size < 0)
        throw new ArgumentOutOfRangeException("size");

      switch (sizeEncoding)
      {
        case SizeEncoding.NO:
          return null;
        case SizeEncoding.B1:
          return new[] { Convert.ToByte(size) };
        case SizeEncoding.B2:
          return BitConverter.GetBytes(Convert.ToUInt16(size));
        case SizeEncoding.B4:
          return BitConverter.GetBytes(size);
        case SizeEncoding.B8:
          return BitConverter.GetBytes(Convert.ToInt64(size));
        case SizeEncoding.VB:
          return GetVarBytes(size);
        default:
          throw new ArgumentOutOfRangeException("sizeEncoding");
      }
    }

    public static Byte[] GetSizeEncodingBytes(long size, SizeEncoding sizeEncoding)
    {
      if (size < 0)
        throw new ArgumentOutOfRangeException("size");

      switch (sizeEncoding)
      {
        case SizeEncoding.NO:
          return null;
        case SizeEncoding.B1:
          return new[] { Convert.ToByte(size) };
        case SizeEncoding.B2:
          return BitConverter.GetBytes(Convert.ToUInt16(size));
        case SizeEncoding.B4:
          return BitConverter.GetBytes(Convert.ToUInt32(size));
        case SizeEncoding.B8:
          return BitConverter.GetBytes(size);
        case SizeEncoding.VB:
          return GetVarBytes(size);
        default:
          throw new ArgumentOutOfRangeException("sizeEncoding");
      }
    }

    public static int GetSizeEncodingSize(int size, SizeEncoding sizeEncoding)
    {
      switch (sizeEncoding)
      {
        case SizeEncoding.B1:
          return sizeof(byte);
        case SizeEncoding.B2:
          return sizeof(ushort);
        case SizeEncoding.B4:
          return sizeof(uint);
        case SizeEncoding.B8:
          return sizeof(ulong);
        case SizeEncoding.VB:
          return GetVbSize(size);
        default:
          throw new InvalidOperationException();
      }
    }

    public static int GetSizeEncodingSize(long size, SizeEncoding sizeEncoding)
    {
      switch (sizeEncoding)
      {
        case SizeEncoding.B1:
          return sizeof(byte);
        case SizeEncoding.B2:
          return sizeof(ushort);
        case SizeEncoding.B4:
          return sizeof(uint);
        case SizeEncoding.B8:
          return sizeof(ulong);
        case SizeEncoding.VB:
          return GetVbSize(size);
        default:
          throw new InvalidOperationException();
      }
    }

    public static int GetVbSize(Int16 value)
    {
      int count = 1;
      for (Int16 v = value; v >= 0x80; v >>= 7, count++) ;
      return count;
    }

    public static int GetVbSize(Int32 value)
    {
      int count = 1;
      for (Int32 v = value; v >= 0x80; v >>= 7, count++) ;
      return count;
    }

    public static int GetVbSize(Int64 value)
    {
      int count = 1;
      for (Int64 v = value; v >= 0x80; v >>= 7, count++) ;
      return count;
    }

    public static int GetVbSize(UInt16 value)
    {
      int count = 1;
      for (UInt16 v = value; v >= 0x80; v >>= 7, count++) ;
      return count;
    }

    public static int GetVbSize(UInt32 value)
    {
      int count = 1;
      for (UInt32 v = value; v >= 0x80; v >>= 7, count++) ;
      return count;
    }

    public static int GetVbSize(UInt64 value)
    {
      int count = 1;
      for (UInt64 v = value; v >= 0x80; v >>= 7, count++) ;
      return count;
    }

    public static Byte[] GetVarBytes(Int16 value)
    {
      int count = 1;
      for (Int16 v = value; v >= 0x80; v >>= 7, count++) ;
      byte[] buffer = new byte[count];
      for (int i = 0; value >= 0x80; value >>= 7, i++)
        buffer[i] = (byte)(value | 0x80);
      buffer[count - 1] = (byte)value;
      return buffer;
    }

    public static Byte[] GetVarBytes(Int32 value)
    {
      int count = 1;
      for (Int32 v = value; v >= 0x80; v >>= 7, count++);
      byte[] buffer = new byte[count];
      for (int i = 0; value >= 0x80; value >>= 7, i++)
        buffer[i] = (byte)(value | 0x80);
      buffer[count - 1] = (byte)value;
      return buffer;
    }

    public static Byte[] GetVarBytes(Int64 value)
    {
      int count = 1;
      for (Int64 v = value; v >= 0x80; v >>= 7, count++) ;
      byte[] buffer = new byte[count];
      for (int i = 0; value >= 0x80; value >>= 7, i++)
        buffer[i] = (byte)(value | 0x80);
      buffer[count - 1] = (byte)value;
      return buffer;
    }

    public static Byte[] GetVarBytes(UInt16 value)
    {
      int count = 1;
      for (UInt16 v = value; v >= 0x80; v >>= 7, count++) ;
      byte[] buffer = new byte[count];
      for (int i = 0; value >= 0x80; value >>= 7, i++)
        buffer[i] = (byte)(value | 0x80);
      buffer[count - 1] = (byte)value;
      return buffer;
    }

    public static Byte[] GetVarBytes(UInt32 value)
    {
      int count = 1;
      for (UInt32 v = value; v >= 0x80; v >>= 7, count++) ;
      byte[] buffer = new byte[count];
      for (int i = 0; value >= 0x80; value >>= 7, i++)
        buffer[i] = (byte)(value | 0x80);
      buffer[count - 1] = (byte)value;
      return buffer;
    }

    public static Byte[] GetVarBytes(UInt64 value)
    {
      int count = 1;
      for (UInt64 v = value; v >= 0x80; v >>= 7, count++) ;
      byte[] buffer = new byte[count];
      for (int i = 0; value >= 0x80; value >>= 7, i++)
        buffer[i] = (byte)(value | 0x80);
      buffer[count - 1] = (byte)value;
      return buffer;
    }

    public static Int16 VbToInt16(Byte[] bytes, int index)
    {
      if (bytes == null)
        throw new ArgumentNullException("bytes");
      if (index < 0 || index >= bytes.Length)
        throw new ArgumentOutOfRangeException("index");

      Int16 value = 0;
      for (int i = 0, s = 0, r = sizeof(Int16) * 8; r > 0; s += 7)
      {
        if (i >= bytes.Length - index)
          throw new InvalidOperationException("Bad multi-byte integer format");

        byte read = bytes[index + i];
        if (read < 0x80)
          r = 0;
        else if (r > 7)
          r -= 7;
        else
          throw new InvalidOperationException("Bad multi-byte integer format");
        value |= (Int16)((read & ~0x80U) << s);
      }
      return value;
    }

    public static Int32 VbToInt32(Byte[] bytes, int index)
    {
      if (bytes == null)
        throw new ArgumentNullException("bytes");
      if (index < 0 || index >= bytes.Length)
        throw new ArgumentOutOfRangeException("index");

      Int32 value = 0;
      for (int i = 0, s = 0, r = sizeof(Int32) * 8 - 1; r > 0; s += 7)
      {
        if (i >= bytes.Length - index)
          throw new InvalidOperationException("Bad multi-byte integer format");

        byte read = bytes[index + i];
        if (read < 0x80)
          r = 0;
        else if (r > 7)
          r -= 7;
        else
          throw new InvalidOperationException("Bad multi-byte integer format");
        value |= (read & ~0x80) << s;
      }
      return value;
    }

    public static Int64 VbToInt64(Byte[] bytes, int index)
    {
      if (bytes == null)
        throw new ArgumentNullException("bytes");
      if (index < 0 || index >= bytes.Length)
        throw new ArgumentOutOfRangeException("index");

      Int64 value = 0L;
      for (int i = 0, s = 0, r = sizeof(Int64) * 8; r > 0; s += 7)
      {
        if (i >= bytes.Length - index)
          throw new InvalidOperationException("Bad multi-byte integer format");

        byte read = bytes[index + i];
        if (read < 0x80)
          r = 0;
        else if (r > 7)
          r -= 7;
        else
          throw new InvalidOperationException("Bad multi-byte integer format");
        value |= (read & ~0x80U) << s;
      }
      return value;
    }

    public static UInt16 VbToUInt16(Byte[] bytes, int index)
    {
      if (bytes == null)
        throw new ArgumentNullException("bytes");
      if (index < 0 || index >= bytes.Length)
        throw new ArgumentOutOfRangeException("index");

      UInt16 value = 0;
      for (int i = 0, s = 0, r = sizeof(UInt16) * 8; r > 0; s += 7)
      {
        if (i >= bytes.Length - index)
          throw new InvalidOperationException("Bad multi-byte integer format");

        byte read = bytes[index + i];
        if (read < 0x80)
          r = 0;
        else if (r > 7)
          r -= 7;
        else
          throw new InvalidOperationException("Bad multi-byte integer format");
        value |= (UInt16)((read & ~0x80U) << s);
      }
      return value;
    }

    public static UInt32 VbToUInt32(Byte[] bytes, int index)
    {
      if (bytes == null)
        throw new ArgumentNullException("bytes");
      if (index < 0 || index >= bytes.Length)
        throw new ArgumentOutOfRangeException("index");

      UInt32 value = 0U;
      for (int i = 0, s = 0, r = sizeof(UInt32) * 8; r > 0; s += 7)
      {
        if (i >= bytes.Length - index)
          throw new InvalidOperationException("Bad multi-byte integer format");

        byte read = bytes[index + i];
        if (read < 0x80)
          r = 0;
        else if (r > 7)
          r -= 7;
        else
          throw new InvalidOperationException("Bad multi-byte integer format");
        value |= (read & ~0x80U) << s;
      }
      return value;
    }

    public static UInt64 VbToUInt64(Byte[] bytes, int index)
    {
      if (bytes == null)
        throw new ArgumentNullException("bytes");
      if (index < 0 || index >= bytes.Length)
        throw new ArgumentOutOfRangeException("index");

      UInt64 value = 0UL;
      for (int i = 0, s = 0, r = sizeof(UInt64) * 8; r > 0; s += 7)
      {
        if (i >= bytes.Length - index)
          throw new InvalidOperationException("Bad multi-byte integer format");

        byte read = bytes[index + i];
        if (read < 0x80)
          r = 0;
        else if (r > 7)
          r -= 7;
        else
          throw new InvalidOperationException("Bad multi-byte integer format");
        value |= (read & ~0x80U) << s;
      }
      return value;
    }

    #endregion
    #region Masked methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt32 GetMaskUInt32(int index, int count)
    {
      return count == 0 ? 0U : ((count < sizeof(UInt32) * 8 ? (1U << count) : 0U) - 1U) << index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64 GetMaskUInt64(int index, int count)
    {
      return count == 0 ? 0UL : ((count < sizeof(UInt64) * 8 ? (1UL << count) : 0UL) - 1UL) << index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetMaskInt32(int index, int count)
    {
      return count == 0 ? 0 : ((count < sizeof(Int32) * 8 ? (1 << count) : 0) - 1) << index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 GetMaskInt64(int index, int count)
    {
      return count == 0 ? 0L : ((count < sizeof(Int64) * 8 ? (1L << count) : 0L) - 1L) << index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt32 GetMasked(UInt32 value, int index, int count)
    {
      return count == 0 ? 0U : (value & (((count < sizeof(UInt32) * 8 ? (1U << count) : 0U) - 1U) << index)) >> index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64 GetMasked(UInt64 value, int index, int count)
    {
      return count == 0 ? 0UL : (value & (((count < sizeof(UInt64) * 8 ? (1UL << count) : 0UL) - 1UL) << index)) >> index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetMasked(Int32 value, int index, int count)
    {
      return count == 0 ? 0 : ((value & (((count < sizeof(Int32) * 8 ? (1 << count) : 0) - 1) << index)) >> index) & ((count < sizeof(Int32) * 8 ? (1 << count) : 0) - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 GetMasked(Int64 value, int index, int count)
    {
      return count == 0 ? 0L : ((value & (((count < sizeof(Int64) * 8 ? (1L << count) : 0L) - 1L) << index)) >> index) & ((count < sizeof(Int32) * 8 ? (1L << count) : 0L) - 1L);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt32 SetMasked(UInt32 value, int index, int count, UInt32 bits)
    {
      return count == 0 ? value : (value & ~(((count < sizeof(UInt32) * 8 ? (1U << count) : 0U) - 1U) << index)) | ((bits & ((count < sizeof(UInt32) * 8 ? (1U << count) : 0U) - 1U)) << index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64 SetMasked(UInt64 value, int index, int count, UInt64 bits)
    {
      return count == 0 ? value : (value & ~(((count < sizeof(UInt64) * 8 ? (1UL << count) : 0UL) - 1UL) << index)) | ((bits & ((count < sizeof(UInt64) * 8 ? (1UL << count) : 0UL) - 1UL)) << index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 SetMasked(Int32 value, int index, int count, Int32 bits)
    {
      return count == 0 ? value : (value & ~(((count < sizeof(Int32) * 8 ? (1 << count) : 0) - 1) << index)) | ((bits & ((count < sizeof(Int32) * 8 ? (1 << count) : 0) - 1)) << index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 SetMasked(Int64 value, int index, int count, Int64 bits)
    {
      return count == 0 ? value : (value & ~(((count < sizeof(Int64) * 8 ? (1L << count) : 0L) - 1L) << index)) | ((bits & ((count < sizeof(Int64) * 8 ? (1L << count) : 0L) - 1L)) << index);
    }

    #endregion
    #region Bit methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean GetBit(UInt32 value, int index)
    {
      return (value & (1U << index)) != 0U;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean GetBit(UInt64 value, int index)
    {
      return (value & (1UL << index)) != 0UL;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean GetBit(Int32 value, int index)
    {
      return (value & (1 << index)) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean GetBit(Int64 value, int index)
    {
      return (value & (1L << index)) != 0L;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt32 SetBit(UInt32 value, int index, Boolean bit)
    {
      return bit ? value | (1U << index) : value & ~(1U << index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64 SetBit(UInt64 value, int index, Boolean bit)
    {
      return bit ? value | (1UL << index) : value & ~(1UL << index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 SetBit(Int32 value, int index, Boolean bit)
    {
      return bit ? value | (1 << index) : value & ~(1 << index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 SetBit(Int64 value, int index, Boolean bit)
    {
      return bit ? value | (1L << index) : value & ~(1L << index);
    }

    #endregion
    #region Formatting methods

    public static byte[] ParseBinary(string str, bool needPrefix = false)
    {
      if (str == null)
        throw new ArgumentNullException("str");

      bool hasPrefix = str.StartsWith(HexPrefix, StringComparison.InvariantCultureIgnoreCase);
      if (needPrefix && !hasPrefix)
        throw new FormatException("Hexadecimal prefix is missed.");
      List<byte> list = new List<byte>();
      int offset = hasPrefix ? HexPrefix.Length : 0;
      if (offset < str.Length)
      {
        int length = 2 - (str.Length - offset) % 2;
        list.Add(byte.Parse(str.Substring(offset, length), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));
        for (offset += length; offset < str.Length; offset += 2)
          list.Add(byte.Parse(str.Substring(offset, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));
      }
      return list.ToArray();
    }

    public static string Format(byte[] binary, bool prefix = false, bool capital = false)
    {
      if (binary == null)
        throw new ArgumentNullException("binary"); ;

      StringBuilder sb = new StringBuilder();
      if (prefix)
        sb.Append((capital ? HexPrefix.ToUpper() : HexPrefix.ToLower()));
      for (int i = 0; i < binary.Length; i++)
        sb.AppendFormat(CultureInfo.InvariantCulture, capital ? "{0:X2}" : "{0:x2}", binary[i]);
      return sb.ToString();
    }

    #endregion
    #region Compare

    public static bool Equals(byte[] x, byte[] y)
    {
      return x == null && y == null || x != null && y != null && x.SequenceEqual(y);
    }

    public static int Compare(byte[] x, byte[] y)
    {
      return x != null ? y != null ? x.SequenceCompare(y) : 1 : y != null ? -1 : 0;
    }

    #endregion
  }
}
