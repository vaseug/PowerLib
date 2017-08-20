using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerLib.System
{
  public static class RangeExtension
  {
    public static Range Find(this Range range, Func<int, int, int> match)
    {
      if (match == null)
        throw new ArgumentNullException("match");

      int index = range.Index;
      int count = range.Count;
      int matched = 0;
      int accepted = 0;
      while (count > 0 && matched < count && accepted <= 0)
      {
        accepted = match(index, matched);
        if (accepted == 0)
          matched++;
        else if (accepted < 0)
        {
          matched = 0;
          count--;
          index++;
        }
      }
      if (accepted > matched + 1)
        throw new InvalidOperationException("Accepted value is greater matched length.");
      return new Range(index, accepted > 0 ? accepted : -matched - 1);
    }

    public static Range FindLast(this Range range, Func<int, int, int> match)
    {
      if (match == null)
        throw new ArgumentNullException("match");

      int index = range.Index;
      int count = range.Count;
      int matched = 0;
      int accepted = 0;
      while (count > 0 && matched < count && accepted <= 0)
      {
        accepted = match(index + count - 1 - matched, matched);
        if (accepted == 0)
          matched++;
        else if (accepted < 0)
        {
          matched = 0;
          count--;
        }
      }
      if (accepted > matched + 1)
        throw new InvalidOperationException("Accepted value is greater matched length.");
      return new Range(index + count - (accepted > 0 ? accepted : matched), accepted > 0 ? matched : -matched - 1);
    }

    public static IEnumerable<int> Enumerate(this Range range)
    {
      return Enumerable.Range(range.Index, range.Count);
    }
  }
}
