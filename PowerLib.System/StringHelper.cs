using System;
using System.Collections.Generic;
using PowerLib.System.Collections;

namespace PowerLib.System
{
  internal class StringHelper
  {
    internal static int MatchStrings(IList<Char> chars, IList<String> strings, IList<int> matches, int maxLength)
    {
      bool matched = false;
      for (int i = 0; i < matches.Count; i++)
        if (strings[i].Length >= chars.Count && matches[i] == chars.Count - 1 && strings[i][chars.Count - 1] == chars[chars.Count - 1])
        {
          matches[i]++;
          if (!matched)
            matched = true;
        }
      if (matched)
      {
        if (chars.Count < maxLength)
          return 0;
        matches.Fill(0);
        return chars.Count;
      }
      else
      {
        if (chars.Count == 1)
          return -1;
        int found = -1;
        for (int i = 0; i < matches.Count; i++)
          if (matches[i] == strings[i].Length && (found < 0 || matches[i] > matches[found]))
            found = i;
        int accepted = found >= 0 ? matches[found] : -1;
        matches.Fill(0);
        return accepted;
      }
    }
  }
}
