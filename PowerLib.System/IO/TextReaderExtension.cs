using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PowerLib.System.Collections;
using PowerLib.System.Text;

namespace PowerLib.System.IO
{
	public static class TextReaderExtension
	{
    public static IEnumerable<String> ReadLines(this TextReader reader, Func<IList<Char>, int> terminatorMatcher, Func<IList<Char>, string> terminatorConverter, int maxCount)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (terminatorMatcher == null)
        throw new ArgumentNullException("matchTerm");
      if (maxCount < 0)
        throw new ArgumentOutOfRangeException("maxCount");

      if (maxCount == 0)
        yield break;

      int matched = 0;
      var list = new PwrList<Char>();
      var view = new PwrFrameListView<Char>(list);
      var sb = new StringBuilder();
      while (maxCount > 0)
      {
        int read = 0;
        int accepted = 0;
        while (true)
        {
          if (matched == list.Count)
            if ((read = reader.Read()) < 0)
              break;
            else
              list.PushLast((char)read);
          view.FrameSize = matched + 1;
          accepted = terminatorMatcher(view);
          if (accepted > 0)
            break;
          else if (accepted == 0)
            matched++;
          else
          {
            matched = 0;
            sb.Append(list.PopFirst());
          }
        }
        if (accepted > matched + 1)
          throw new InvalidOperationException("Accepted value is greater matched length.");
        if (accepted > 0)
        {
          if (terminatorConverter == null)
            sb.Append(list.GetRange(0, accepted).ToArray());
          else
          {
            view.FrameSize = accepted;
            string term = terminatorConverter(view);
            if (!string.IsNullOrEmpty(term))
              sb.Append(term);
          }
          yield return sb.ToString();
          sb.Clear();
          list.RemoveRange(0, accepted);
          matched = 0;
          maxCount--;
        }
        if (read < 0)
          break;
      }
      if (maxCount > 0)
      {
        if (list.Count > 0)
          sb.AppendChars(list);
        if (sb.Length > 0)
        {
          yield return sb.ToString();
          sb.Clear();
          list.Clear();
        }
      }
    }

    public static IEnumerable<String> ReadLines(this TextReader reader, IList<String> terminatorsList, String terminatorStub, int maxCount)
    {
      if (terminatorsList == null)
        throw new ArgumentNullException("termsList");

      var matches = new int[terminatorsList.Count];
      int maxLength = terminatorsList.Max(s => s.Length);
      return reader.ReadLines(l => StringHelper.MatchStrings(l, terminatorsList, matches, maxLength), terminatorStub != null ? l => terminatorStub : default(Func<IList<Char>, String>), maxCount);
    }
  }
}
