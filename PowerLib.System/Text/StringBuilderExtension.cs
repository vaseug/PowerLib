using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerLib.System.Text
{
  public static class PwrStringBuilderExtension
  {
    public static StringBuilder Append(this StringBuilder sb, string delimiter, IEnumerable<string> coll)
    {
      if (sb == null)
        throw new ArgumentNullException("sb");
      if (delimiter == null)
        throw new ArgumentNullException("delimiter");
      if (coll == null)
        throw new ArgumentNullException("coll");

      using (var e = coll.GetEnumerator())
      {
        if (e.MoveNext())
        {
          sb.Append(e.Current);
          while (e.MoveNext())
            sb.Append(delimiter).Append(e.Current);
        }
      }
      return sb;
    }

    public static StringBuilder AppendFormat(this StringBuilder sb, string format, string delimiter, IEnumerable<string> coll)
    {
      if (sb == null)
        throw new ArgumentNullException("sb");
      if (format == null)
        throw new ArgumentNullException("format");
      if (delimiter == null)
        throw new ArgumentNullException("delimiter");
      if (coll == null)
        throw new ArgumentNullException("coll");

      using (var e = coll.GetEnumerator())
      {
        if (e.MoveNext())
        {
          sb.AppendFormat(format, e.Current);
          while (e.MoveNext())
            sb.Append(delimiter).AppendFormat(format, e.Current);
        }
      }
      return sb;
    }

    public static StringBuilder Append(this StringBuilder sb, string delimiter, params string[] args)
    {
      if (sb == null)
        throw new ArgumentNullException("sb");
      if (delimiter == null)
        throw new ArgumentNullException("delimiter");
      if (args == null)
        throw new ArgumentNullException("args");

      if (args.Length > 0)
      {
        sb.Append(args[0]);
        for (int i = 1; i < args.Length; i++)
          sb.Append(delimiter).Append(args[i]);
      }
      return sb;
    }

    public static StringBuilder AppendFormat(this StringBuilder sb, string format, string delimiter, params string[] args)
    {
      if (sb == null)
        throw new ArgumentNullException("sb");
      if (format == null)
        throw new ArgumentNullException("format");
      if (delimiter == null)
        throw new ArgumentNullException("delimiter");
      if (args == null)
        throw new ArgumentNullException("args");

      if (args.Length > 0)
      {
        sb.AppendFormat(format, args[0]);
        for (int i = 1; i < args.Length; i++)
          sb.Append(delimiter).AppendFormat(format, args[i]);
      }
      return sb;
    }

    public static StringBuilder Append(this StringBuilder sb, string value, int repeats)
    {
      if (sb == null)
        throw new ArgumentNullException("sb");
      if (repeats < 0)
        throw new ArgumentOutOfRangeException("repeats");

      while (repeats-- > 0)
        sb.Append(value);
      return sb;
    }
  }
}
