using System;
using System.Collections.Generic;

namespace PowerLib.System
{
  public static class RingBuffer
  {
    public static void Copy<T>(T[] sBuffer, int sPosition, T[] dBuffer, int dPosition, int count)
    {
      if (sBuffer == dBuffer && (sPosition == dPosition || count == 0))
        return;
      bool reverse = sBuffer == dBuffer && (sPosition > dPosition && count > sBuffer.Length - sPosition && count - (sBuffer.Length - sPosition) > dPosition || sPosition < dPosition && count > dPosition - sPosition);

      if (count <= sBuffer.Length - sPosition)
      {
        if (count <= dBuffer.Length - dPosition)
        {
          Array.Copy(sBuffer, sPosition, dBuffer, dPosition, count);
        }
        else
        {
          int part = dBuffer.Length - dPosition;
          Array.Copy(sBuffer, sPosition, dBuffer, dPosition, part);
          Array.Copy(sBuffer, sPosition + part, dBuffer, 0, count - part);
        }
      }
      else
      {
        if (count <= dBuffer.Length - dPosition)
        {
          int part = sBuffer.Length - sPosition;
          Array.Copy(sBuffer, sPosition, dBuffer, dPosition, part);
          Array.Copy(sBuffer, 0, dBuffer, dPosition + part, count - part);
        }
        else
        {
          int sPart = sBuffer.Length - sPosition;
          int dPart = dBuffer.Length - dPosition;
          if (sPart < dPart)
          {
            if (reverse)
            {
              Array.Copy(sBuffer, dPart, dBuffer, 0, count - dPart);
              Array.Copy(sBuffer, 0, dBuffer, dPosition + sPart, dPart - sPart);
              Array.Copy(sBuffer, sPosition, dBuffer, dPosition, sPart);
            }
            else
            {
              Array.Copy(sBuffer, sPosition, dBuffer, dPosition, sPart);
              Array.Copy(sBuffer, 0, dBuffer, dPosition + sPart, dPart - sPart);
              Array.Copy(sBuffer, dPart, dBuffer, 0, count - dPart);
            }
          }
          else
          {
            if (reverse)
            {
              Array.Copy(sBuffer, 0, dBuffer, sPart, count - sPart);
              Array.Copy(sBuffer, sPosition + dPart, dBuffer, 0, sPart - dPart);
              Array.Copy(sBuffer, sPosition, dBuffer, dPosition, dPart);
            }
            else
            {
              Array.Copy(sBuffer, sPosition, dBuffer, dPosition, dPart);
              Array.Copy(sBuffer, sPosition + dPart, dBuffer, 0, sPart - dPart);
              Array.Copy(sBuffer, 0, dBuffer, sPart, count - sPart);
            }
          }
        }
      }
    }

    internal static void Copy<T>(T[] sBuffer, int sOffset, int sIndex, T[] dBuffer, int dOffset, int dIndex, int length, bool reverse)
    {
      if (sIndex >= sBuffer.Length - sOffset)
      {
        if (dIndex >= dBuffer.Length - dOffset)
        {
          Array.Copy(sBuffer, sIndex - (sBuffer.Length - sOffset), dBuffer, dIndex - (dBuffer.Length - dOffset), length);
        }
        else if (dIndex + length <= dBuffer.Length - dOffset)
        {
          Array.Copy(sBuffer, sIndex - (sBuffer.Length - sOffset), dBuffer, dOffset + dIndex, length);
        }
        else
        {
          int part = dBuffer.Length - (dOffset + dIndex);
          if (reverse)
          {
            Array.Copy(sBuffer, sIndex - (sBuffer.Length - sOffset) + part, dBuffer, 0, length - part);
            Array.Copy(sBuffer, sIndex - (sBuffer.Length - sOffset), dBuffer, dOffset + dIndex, part);
          }
          else
          {
            Array.Copy(sBuffer, sIndex - (sBuffer.Length - sOffset), dBuffer, dOffset + dIndex, part);
            Array.Copy(sBuffer, sIndex - (sBuffer.Length - sOffset) + part, dBuffer, 0, length - part);
          }
        }
      }
      else if (sIndex + length <= sBuffer.Length - sOffset)
      {
        if (dIndex >= dBuffer.Length - dOffset)
        {
          Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dIndex - (dBuffer.Length - dOffset), length);
        }
        else if (dIndex + length <= dBuffer.Length - dOffset)
        {
          Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dOffset + dIndex, length);
        }
        else
        {
          int part = dBuffer.Length - (dOffset + dIndex);
          if (reverse)
          {
            Array.Copy(sBuffer, sOffset + sIndex + part, dBuffer, 0, length - part);
            Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dOffset + dIndex, part);
          }
          else
          {
            Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dOffset + dIndex, part);
            Array.Copy(sBuffer, sOffset + sIndex + part, dBuffer, 0, length - part);
          }
        }
      }
      else
      {
        int srcPart = sBuffer.Length - (sOffset + sIndex);
        if (dIndex >= dBuffer.Length - dOffset)
        {
          if (reverse)
          {
            Array.Copy(sBuffer, 0, dBuffer, dIndex - (dBuffer.Length - dOffset) + srcPart, length - srcPart);
            Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dIndex - (dBuffer.Length - dOffset), srcPart);
          }
          else
          {
            Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dIndex - (dBuffer.Length - dOffset), srcPart);
            Array.Copy(sBuffer, 0, dBuffer, dIndex - (dBuffer.Length - dOffset) + srcPart, length - srcPart);
          }
        }
        else if (dIndex + length <= dBuffer.Length - dOffset)
        {
          if (reverse)
          {
            Array.Copy(sBuffer, 0, dBuffer, dOffset + dIndex + srcPart, length - srcPart);
            Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dOffset + dIndex, srcPart);
          }
          else
          {
            Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dOffset + dIndex, srcPart);
            Array.Copy(sBuffer, 0, dBuffer, dOffset + dIndex + srcPart, length - srcPart);
          }
        }
        else
        {
          int dstPart = dBuffer.Length - (dOffset + dIndex);
          if (srcPart < dstPart)
          {
            if (reverse)
            {
              Array.Copy(sBuffer, dstPart - srcPart, dBuffer, 0, length - dstPart);
              Array.Copy(sBuffer, 0, dBuffer, dOffset + dIndex + srcPart, dstPart - srcPart);
              Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dOffset + dIndex, srcPart);
            }
            else
            {
              Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dOffset + dIndex, srcPart);
              Array.Copy(sBuffer, 0, dBuffer, dOffset + dIndex + srcPart, dstPart - srcPart);
              Array.Copy(sBuffer, dstPart - srcPart, dBuffer, 0, length - dstPart);
            }
          }
          else if (srcPart > dstPart)
          {
            if (reverse)
            {
              Array.Copy(sBuffer, 0, dBuffer, srcPart - dstPart, length - srcPart);
              Array.Copy(sBuffer, sOffset + sIndex + dstPart, dBuffer, 0, srcPart - dstPart);
              Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dOffset + dIndex, dstPart);
            }
            else
            {
              Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dOffset + dIndex, dstPart);
              Array.Copy(sBuffer, sOffset + sIndex + dstPart, dBuffer, 0, srcPart - dstPart);
              Array.Copy(sBuffer, 0, dBuffer, srcPart - dstPart, length - srcPart);
            }
          }
          else
          {
            if (reverse)
            {
              Array.Copy(sBuffer, 0, dBuffer, 0, length - srcPart);
              Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dOffset + dIndex, srcPart);
            }
            else
            {
              Array.Copy(sBuffer, sOffset + sIndex, dBuffer, dOffset + dIndex, srcPart);
              Array.Copy(sBuffer, 0, dBuffer, 0, length - srcPart);
            }
          }
        }
      }
    }

    public static void Copy<T>(T[] buffer, int offset, int sIndex, int dIndex, int length)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset < 0 || offset >= buffer.Length)
        throw new ArgumentOutOfRangeException("offset");
      if (sIndex < 0 || sIndex > buffer.Length)
        throw new ArgumentOutOfRangeException("sIndex");
      if (dIndex < 0 || dIndex > buffer.Length)
        throw new ArgumentOutOfRangeException("dIndex");
      if (length < 0 || length > buffer.Length - sIndex || length > buffer.Length - dIndex)
        throw new ArgumentOutOfRangeException("length");
      if (sIndex == dIndex || length == 0)
        return;

      Copy<T>(buffer, offset, sIndex, buffer, offset, dIndex, length, false);
    }

    public static void Copy<T>(T[] sBuffer, int sOffset, int sIndex, T[] dBuffer, int dOffset, int dIndex, int length)
    {
      if (sBuffer == null)
        throw new ArgumentNullException("sBuffer");
      if (dBuffer == null)
        throw new ArgumentNullException("dBuffer");
      if (sOffset < 0 || sOffset >= sBuffer.Length)
        throw new ArgumentOutOfRangeException("sOffset");
      if (dOffset < 0 || dOffset >= dBuffer.Length)
        throw new ArgumentOutOfRangeException("dOffset");
      if (sIndex < 0 || sIndex > sBuffer.Length)
        throw new ArgumentOutOfRangeException("sIndex");
      if (dIndex < 0 || dIndex > dBuffer.Length)
        throw new ArgumentOutOfRangeException("dIndex");
      if (length < 0 || length > sBuffer.Length - sIndex || length > dBuffer.Length - dIndex)
        throw new ArgumentOutOfRangeException("length");

      if (sBuffer == dBuffer && sOffset == dOffset)
        return;

      Copy<T>(sBuffer, sOffset, 0, dBuffer, dOffset, 0, length, sBuffer == dBuffer && sOffset < dOffset);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sBuffer"></param>
    /// <param name="sStart"></param>
    /// <param name="sGap"></param>
    /// <param name="sSkew"></param>
    /// <param name="dBuffer"></param>
    /// <param name="dStart"></param>
    /// <param name="dGap"></param>
    /// <param name="dSkew"></param>
    /// <param name="range"></param>
    /// <param name="delta"></param>
    /// <param name="count"></param>
    /// <param name="reverse"></param>
    public static void Copy<T>(T[] sBuffer, int sStart, int sGap, int sSkew, T[] dBuffer, int dStart, int dGap, int dSkew, int range, int delta, int count, bool reverse)
    {
      if (sBuffer == null)
        throw new ArgumentNullException("sArray");
      if (dBuffer == null)
        throw new ArgumentNullException("dArray");
      if (sStart < 0 || sStart > sBuffer.Length)
        throw new ArgumentOutOfRangeException("sStart");
      if (dStart < 0 || dStart > dBuffer.Length)
        throw new ArgumentOutOfRangeException("dStart");
      if (range < 0)
        throw new ArgumentOutOfRangeException("range");
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      if (sGap < 0)
        throw new ArgumentOutOfRangeException("sGap");
      if (dGap < 0)
        throw new ArgumentOutOfRangeException("dGap");
      if (range + delta * (count - 1) < 0)
        throw new ArgumentOutOfRangeException("delta");
      if (sGap + sSkew * (count - 1) < 0)
        throw new ArgumentOutOfRangeException("sSkew");
      if (dGap + dSkew * (count - 1) < 0)
        throw new ArgumentOutOfRangeException("dSkew");
      if (range * count + delta * (((count - 1) * count) >> 1) + (count < 2 ? 0 : (sGap * (count - 1) + sSkew * (((count - 2) * count - 1) >> 1))) > sBuffer.Length)
        throw new ArgumentOutOfRangeException(null, "Combination of source parameters.");
      if (range * count + delta * (((count - 1) * count) >> 1) + (count < 2 ? 0 : (dGap * (count - 1) + dSkew * (((count - 2) * count - 1) >> 1))) > dBuffer.Length)
        throw new ArgumentOutOfRangeException(null, "Combination of target parameters.");
      if (count == 0 || range == 0 && delta == 0)
        return;
      if (sBuffer == dBuffer && sStart < dStart)
      {
        for (int i = 0, length = range + delta * (count - 1), sSpan = sGap + sSkew * (count - 1), dSpan = dGap + dSkew * (count - 1),
          sIndex = (range + sGap) * (count - 1) + (delta + sSkew) * (((count - 1) * count) >> 1) - sSpan,
          dIndex = (range + dGap) * (count - 1) + (delta + dSkew) * (((count - 1) * count) >> 1) - dSpan;
          i < count;
          i++, length -= delta, sSpan -= sSkew, dSpan -= dSkew, sIndex -= sSpan + length, dIndex -= dSpan + length)
        {
          if (sIndex >= sBuffer.Length - sStart)
          {
            if (dIndex >= dBuffer.Length - dStart)
            {
              Array.Copy(sBuffer, sIndex - (sBuffer.Length - sStart), dBuffer, dIndex - (dBuffer.Length - dStart), length);
            }
            else if (dIndex + length <= dBuffer.Length - dStart)
            {
              Array.Copy(sBuffer, sIndex - (sBuffer.Length - sStart), dBuffer, dStart + dIndex, length);
            }
            else
            {
              int part = dBuffer.Length - (dStart + dIndex);
              Array.Copy(sBuffer, sIndex - (sBuffer.Length - sStart) + part, dBuffer, 0, length - part);
              Array.Copy(sBuffer, sIndex - (sBuffer.Length - sStart), dBuffer, dStart + dIndex, part);
            }
          }
          else if (sIndex + length <= sBuffer.Length - sStart)
          {
            if (dIndex >= dBuffer.Length - dStart)
            {
              Array.Copy(sBuffer, sStart + sIndex, dBuffer, dIndex - (dBuffer.Length - dStart), length);
            }
            else if (dIndex + length <= dBuffer.Length - dStart)
            {
              Array.Copy(sBuffer, sStart + sIndex, dBuffer, dStart + dIndex, length);
            }
            else
            {
              int part = dBuffer.Length - (dStart + dIndex);
              Array.Copy(sBuffer, sStart + sIndex + part, dBuffer, 0, length - part);
              Array.Copy(sBuffer, sStart + sIndex, dBuffer, dStart + dIndex, part);
            }
          }
          else
          {
            int srcPart = sBuffer.Length - (sStart + sIndex);
            if (dIndex >= dBuffer.Length - dStart)
            {
              Array.Copy(sBuffer, 0, dBuffer, dIndex - (dBuffer.Length - dStart) + srcPart, length - srcPart);
              Array.Copy(sBuffer, sStart + sIndex, dBuffer, dIndex - (dBuffer.Length - dStart), srcPart);
            }
            else if (dIndex + length <= dBuffer.Length - dStart)
            {
              Array.Copy(sBuffer, 0, dBuffer, dStart + dIndex + srcPart, length - srcPart);
              Array.Copy(sBuffer, sStart + sIndex, dBuffer, dStart + dIndex, srcPart);
            }
            else
            {
              int dstPart = dBuffer.Length - (dStart + dIndex);
              if (srcPart < dstPart)
              {
                Array.Copy(sBuffer, dstPart - srcPart, dBuffer, 0, length - dstPart);
                Array.Copy(sBuffer, 0, dBuffer, dStart + dIndex + srcPart, dstPart - srcPart);
                Array.Copy(sBuffer, sStart + sIndex, dBuffer, dStart + dIndex, srcPart);
              }
              else if (srcPart > dstPart)
              {
                Array.Copy(sBuffer, 0, dBuffer, srcPart - dstPart, length - srcPart);
                Array.Copy(sBuffer, sStart + sIndex + dstPart, dBuffer, 0, srcPart - dstPart);
                Array.Copy(sBuffer, sStart + sIndex, dBuffer, dStart + dIndex, dstPart);
              }
              else
              {
                Array.Copy(sBuffer, 0, dBuffer, 0, length - srcPart);
                Array.Copy(sBuffer, sStart + sIndex, dBuffer, dStart + dIndex, srcPart);
              }
            }
          }
        }
      }
      else
      {
        for (int i = 0, length = range, sIndex = 0, dIndex = 0, sSpan = sGap, dSpan = dGap;
          i < count;
          i++, sIndex += sSpan + length, dIndex += dSpan + length, length += delta, sSpan += sSkew, dSpan += dSkew)
        {
          if (sIndex >= sBuffer.Length - sStart)
          {
            if (dIndex >= dBuffer.Length - dStart)
            {
              Array.Copy(sBuffer, sIndex - (sBuffer.Length - sStart), dBuffer, dIndex - (dBuffer.Length - dStart), length);
            }
            else if (dIndex + length <= dBuffer.Length - dStart)
            {
              Array.Copy(sBuffer, sIndex - (sBuffer.Length - sStart), dBuffer, dStart + dIndex, length);
            }
            else
            {
              int part = dBuffer.Length - (dStart + dIndex);
              Array.Copy(sBuffer, sIndex - (sBuffer.Length - sStart), dBuffer, dStart + dIndex, part);
              Array.Copy(sBuffer, sIndex - (sBuffer.Length - sStart) + part, dBuffer, 0, length - part);
            }
          }
          else if (sIndex + length <= sBuffer.Length - sStart)
          {
            if (dIndex >= dBuffer.Length - dStart)
            {
              Array.Copy(sBuffer, sStart + sIndex, dBuffer, dIndex - (dBuffer.Length - dStart), length);
            }
            else if (dIndex + length <= dBuffer.Length - dStart)
            {
              Array.Copy(sBuffer, sStart + sIndex, dBuffer, dStart + dIndex, length);
            }
            else
            {
              int part = dBuffer.Length - (dStart + dIndex);
              Array.Copy(sBuffer, sStart + sIndex, dBuffer, dStart + dIndex, part);
              Array.Copy(sBuffer, sStart + sIndex + part, dBuffer, 0, length - part);
            }
          }
          else
          {
            int srcPart = sBuffer.Length - (sStart + sIndex);
            if (dIndex >= dBuffer.Length - dStart)
            {
              Array.Copy(sBuffer, sStart + sIndex, dBuffer, dIndex - (dBuffer.Length - dStart), srcPart);
              Array.Copy(sBuffer, 0, dBuffer, dIndex - (dBuffer.Length - dStart) + srcPart, length - srcPart);
            }
            else if (dIndex + length <= dBuffer.Length - dStart)
            {
              Array.Copy(sBuffer, sStart + sIndex, dBuffer, dStart + dIndex, srcPart);
              Array.Copy(sBuffer, 0, dBuffer, dStart + dIndex + srcPart, length - srcPart);
            }
            else
            {
              int dstPart = dBuffer.Length - (dStart + dIndex);
              if (srcPart < dstPart)
              {
                Array.Copy(sBuffer, sStart + sIndex, dBuffer, dStart + dIndex, srcPart);
                Array.Copy(sBuffer, 0, dBuffer, dStart + dIndex + srcPart, dstPart - srcPart);
                Array.Copy(sBuffer, dstPart - srcPart, dBuffer, 0, length - dstPart);
              }
              else if (srcPart > dstPart)
              {
                Array.Copy(sBuffer, sStart + sIndex, dBuffer, dStart + dIndex, dstPart);
                Array.Copy(sBuffer, sStart + sIndex + dstPart, dBuffer, 0, srcPart - dstPart);
                Array.Copy(sBuffer, 0, dBuffer, srcPart - dstPart, length - srcPart);
              }
              else
              {
                Array.Copy(sBuffer, sStart + sIndex, dBuffer, dStart + dIndex, srcPart);
                Array.Copy(sBuffer, 0, dBuffer, 0, length - srcPart);
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sBuffer">Sourcarray</param>
    /// <param name="sStart"></param>
    /// <param name="sJump"></param>
    /// <param name="sGap"></param>
    /// <param name="sSkew"></param>
    /// <param name="dBuffer">Destinatioarray</param>
    /// <param name="dStart"></param>
    /// <param name="dJump"></param>
    /// <param name="dGap"></param>
    /// <param name="dSkew"></param>
    /// <param name="range"></param>
    /// <param name="delta"></param>
    /// <param name="count"></param>
    /// <param name="swap"></param>
    public static void Copy<T>(T[] sBuffer, int sStart, int sJump, int sGap, int sSkew, T[] dBuffer, int dStart, int dJump, int dGap, int dSkew, int range, int delta, int count, bool swap)
    {
      if (sBuffer == null)
        throw new ArgumentNullException("sArray");
      if (dBuffer == null)
        throw new ArgumentNullException("dArray");
      if (sStart < 0 || sStart >= sBuffer.Length)
        throw new ArgumentOutOfRangeException("sStart");
      if (dStart < 0 || dStart >= dBuffer.Length)
        throw new ArgumentOutOfRangeException("dStart");
      if (range < 0)
        throw new ArgumentOutOfRangeException("range");
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      if (sGap < 0)
        throw new ArgumentOutOfRangeException("sGap");
      if (dGap < 0)
        throw new ArgumentOutOfRangeException("dGap");
      if (range + delta * (count - 1) < 0)
        throw new ArgumentOutOfRangeException("delta");
      if (sGap + sSkew * (count - 1) < 0)
        throw new ArgumentOutOfRangeException("sSkew");
      if (dGap + dSkew * (count - 1) < 0)
        throw new ArgumentOutOfRangeException("dSkew");
      if (range * count + delta * (((count - 1) * count) >> 1) + (count < 2 ? 0 : (sGap * (count - 1) + sSkew * (((count - 2) * count - 1) >> 1))) > sBuffer.Length)
        throw new ArgumentOutOfRangeException(null, "Combination of source parameters.");
      if (range * count + delta * (((count - 1) * count) >> 1) + (count < 2 ? 0 : (dGap * (count - 1) + dSkew * (((count - 2) * count - 1) >> 1))) > dBuffer.Length)
        throw new ArgumentOutOfRangeException(null, "Combination of target parameters.");

      T temp = default(T);
      for (int i = 0, length = range, sRangePos = 0, dRangePos = 0, sSpan = sGap, dSpan = dGap; i < count; i++, sRangePos += length + sSpan, dRangePos += length + dSpan, length += delta, sSpan += sSkew, dSpan += dSkew)
        for (int j = 0, sItemPos = sRangePos, dItemPos = dRangePos; j < length; j++, sItemPos += sJump + 1, dItemPos += dJump + 1)
        {
          int sIndex = sItemPos >= sBuffer.Length - sStart ? sItemPos - (sBuffer.Length - sStart) : sStart + sItemPos;
          int dIndex = dItemPos >= dBuffer.Length - dStart ? dItemPos - (dBuffer.Length - dStart) : dStart + dItemPos;
          if (swap)
            temp = dBuffer[dIndex];
          dBuffer[dIndex] = sBuffer[sIndex];
          if (swap)
            sBuffer[sIndex] = temp;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="start"></param>
    /// <param name="jump"></param>
    /// <param name="gap"></param>
    /// <param name="skew"></param>
    /// <param name="range"></param>
    /// <param name="delta"></param>
    /// <param name="count"></param>
    /// <param name="value "></param>
    public static void Fill<T>(this T[] array, int start, int jump, int gap, int skew, int range, int delta, int count, T value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      for (int i = 0, length = range, rangePos = 0, span = gap; i < count; i++, rangePos += length + span, length += delta, span += skew)
        for (int j = 0, itemPos = rangePos; j < length; j++, itemPos += jump + 1)
        {
          int index = itemPos >= array.Length - start ? itemPos - (array.Length - start) : start + itemPos;
          array[index] = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="start"></param>
    /// <param name="gap"></param>
    /// <param name="skew"></param>
    /// <param name="range"></param>
    /// <param name="delta"></param>
    /// <param name="count"></param>
    public static void Clear<T>(this T[] array, int start, int gap, int skew, int range, int delta, int count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      for (int i = 0, index = 0, length = range, spa = gap; i < count; i++, index += spa + length, spa += skew, length += delta)
      {
        if (index >= array.Length - start)
          Array.Clear(array, index - (array.Length - start), range);
        else if (index + range <= array.Length - start)
          Array.Clear(array, start + index, range);
        else
        {
          int part = array.Length - (start + index);
          Array.Clear(array, start + index, part);
          Array.Clear(array, 0, range - part);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="start"></param>
    /// <param name="jump"></param>
    /// <param name="gap"></param>
    /// <param name="skew"></param>
    /// <param name="range"></param>
    /// <param name="delta"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static IEnumerable<T> Enumerate<T>(this T[] array, int start, int jump, int gap, int skew, int range, int delta, int count)
    {
      for (int i = 0, length = range, rangePos = 0, span = gap; i < count; i++, rangePos += length + span, length += delta, span += skew)
        for (int j = 0, itemPos = rangePos; j < length; j++, itemPos += jump + 1)
        {
          int index = itemPos >= array.Length - start ? itemPos - (array.Length - start) : start + itemPos;
          yield return array[index];
        }
    }

    public static int SizeOf(int gap, int skew, int range, int delta, int count)
    {
      if (range < 0)
        throw new ArgumentOutOfRangeException("range");
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      if (gap < 0)
        throw new ArgumentOutOfRangeException("gap");
      if (range + delta * (count - 1) < 0)
        throw new ArgumentOutOfRangeException("delta");
      if (gap + skew * (count - 1) < 0)
        throw new ArgumentOutOfRangeException("skew");

      return range * count + delta * (((count - 1) * count) >> 1) + (count < 2 ? 0 : (gap * (count - 1) + skew * (((count - 2) * count - 1) >> 1)));
    }

    public static void Move<T>(T[] buffer, int offset, int length, int sIndex, int dIndex, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (length < 0 || length > buffer.Length)
        throw new ArgumentOutOfRangeException("length");
      if (sIndex < 0 || sIndex >= length)
        throw new ArgumentOutOfRangeException("sIndex");



      if (dIndex < 0 || dIndex >= length)
        throw new ArgumentOutOfRangeException("dIndex");


      if (count == 0 || sIndex == dIndex)
        return;
      T[] temp = new T[count];

      //Array.Copy();


      //_array.Copy(_start + srcIndex, 0, 0, buffer, 0, 0, 0, count, 0, 1, false);
      //if (srcIndex < dstIndex)
      //  _array.Copy(_start + srcIndex + count, 0, 0, _array, _start + srcIndex, 0, 0, dstIndex - srcIndex, 0, 1, false);
      //else if (srcIndex > dstIndex)
      //  _array.Copy(_start + dstIndex, 0, 0, _array, _start + dstIndex + count, 0, 0, srcIndex - dstIndex, 0, 1, true);
      //buffer.Copy(0, 0, 0, _array, _start + dstIndex, 0, 0, count, 0, 1, false);


      return;
    }

    public static T[] Insert<T>(T[] array, int arrayIndex, int arrayCount, int index, T[] value, int valueIndex, int valueCount)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (value == null)
        throw new ArgumentNullException("value ");
      if (index < 0 || index > array.Length)
        throw new ArgumentOutOfRangeException("index");
      if (valueCount > int.MaxValue - arrayCount)
        throw new ArgumentException("Too big resularray.");

      T[] result = new T[array.Length + value.Length];
      if (index > 0)
        Array.Copy(array, 0, result, 0, index);
      Array.Copy(value, 0, result, index, value.Length);
      if (index < array.Length)
        Array.Copy(array, index, result, index + value.Length, array.Length - index);
      return result;
    }
  }

  public struct RingBuffer<T>
  {
    private T[] _buffer;
    private int _offset;
    private int _length;

    public RingBuffer(T[] buffer, int offset, int length)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (offset < 0 || offset >= buffer.Length)
        throw new ArgumentOutOfRangeException("offset");
      if (length < 0 || length > buffer.Length)
        throw new ArgumentOutOfRangeException("length");

      _buffer = buffer;
      _offset = offset;
      _length = length;
    }

    public RingBuffer(T[] buffer)
      : this(buffer, 0, buffer != null ? buffer.Length : 0)
    {
    }

    public T[] Buffer { get { return _buffer; } }

    public int Offset { get { return _offset; } }

    public int Length { get { return _length; } }

    public int Free { get { return Size - Length; } }

    public int Size { get { return Buffer != null ? Buffer.Length : 0; } }
  }
}
