using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerLib.System.Collections;


namespace PowerLib.System
{
	/// <summary>
	/// 
	/// </summary>
	public static class StringExtension
	{
    public static string Quote(this string str, char quote)
    {
      if (str == null)
        return null;
      StringBuilder sb = new StringBuilder();
      sb.Append(quote);
      for (int i = 0; i < str.Length; i++)
      {
        if (str[i] == quote)
          sb.Append(quote);
        sb.Append(str[i]);
      }
      sb.Append(quote);
      return sb.ToString();
    }

    public static string Quote(this string str, char quote, char escape)
    {
      if (str == null)
        return null;
      StringBuilder sb = new StringBuilder();
      sb.Append(quote);
      for (int i = 0; i < str.Length; i++)
      {
        if (str[i] == quote)
          sb.Append(escape);
        sb.Append(str[i]);
      }
      sb.Append(quote);
      return sb.ToString();
    }

    public static string Unquote(this string str, char quote)
    {
      if (string.IsNullOrEmpty(str))
        return str;
      if (str[0] != quote || str[str.Length - 1] != quote)
        return str;
      bool quoted = false;
      StringBuilder sb = new StringBuilder();
      for (int i = 1; i < str.Length - 1; i++)
        if (!quoted && str[i] == quote)
          quoted = true;
        else if (quoted && str[i] != quote)
          throw new ArgumentException("Quoted string invalid format");
        else
        {
          sb.Append(str[i]);
          if (quoted)
            quoted = false;
        }
      if (quoted)
        throw new ArgumentException("Quoted string invalid format");
      return sb.ToString();
    }

    public static string Unquote(this string str, char quote, char escape)
    {
      if (string.IsNullOrEmpty(str))
        return str;
      if (str[0] != quote || str[str.Length - 1] != quote)
        return str;
      bool escaped = false;
      StringBuilder sb = new StringBuilder();
      for (int i = 1; i < str.Length - 1; i++)
        if (!escaped && str[i] == escape)
          escaped = true;
        else if (!escaped && str[i] == quote)
          throw new ArgumentException("Quoted string invalid format");
        else
        {
          if (escaped && str[i] != quote)
            sb.Append(escape);
          sb.Append(str[i]);
          if (escaped)
            escaped = false;
        }
      if (escaped)
        throw new ArgumentException("Quoted string invalid format");
      return sb.ToString();
   }

    public static string Escape(this string str, char escape, params char[] symbols)
    {
      return str.Escape(escape, true, symbols);
    }

    public static string Escape(this string str, char escape, bool oneself, params char[] symbols)
    {
      if (str == null)
        return null;
      var escaping = new HashSet<char>(symbols);
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < str.Length; i++)
      {
        if (str[i] == escape && oneself || escaping.Contains(str[i]))
          sb.Append(escape);
        sb.Append(str[i]);
      }
      return sb.ToString();
    }

    public static string Unescape(this string str, char escape, params char[] symbols)
    {
      if (str == null)
        return null;
      var escaping = new HashSet<char>(symbols);
      StringBuilder sb = new StringBuilder();
      bool escaped = false;
      for (int i = 0; i < str.Length; i++)
        if (!escaped && str[i] == escape)
          escaped = true;
        else
        {
          if (escaped && escaping.Count > 0 && !symbols.Contains(str[i]))
            sb.Append(escape);
          sb.Append(str[i]);
          if (escaped)
            escaped = false;
        }
      if (escaped)
        throw new ArgumentException("Hanging escapsymbol");
      return sb.ToString();
    }

		public static string CutLeft(this string str, int totalWidth)
		{
			if (str == null)
				throw new NullReferenceException();
			if (totalWidth < 0)
				throw new ArgumentOutOfRangeException("totalWidth");

			return str.Length <= totalWidth ? str : str.Remove(0, str.Length - totalWidth);
		}

		public static string CutRight(this string str, int totalWidth)
		{
			if (str == null)
				throw new NullReferenceException();
			if (totalWidth < 0)
				throw new ArgumentOutOfRangeException("totalWidth");

			return str.Length <= totalWidth ? str : str.Remove(totalWidth, str.Length - totalWidth);
		}

		public static string[] Split(this string str, char[] separators, char[] trims, StringEscape[] escapes, StringQuotation[] quotations)
		{
			return Split(str, separators, trims, escapes, quotations, int.MaxValue , StringSplitOptionsEx.None);
		}

		public static string[] Split(this string str, char[] separators, char[] trims, StringEscape[] escapes, StringQuotation[] quotations, int count)
		{
			return Split(str, separators, trims, escapes, quotations, count, StringSplitOptionsEx.None);
		}

		public static string[] Split(this string str, char[] separators, char[] trims, StringEscape[] escapes, StringQuotation[] quotations, StringSplitOptionsEx options)
		{
			return Split(str, separators, trims, escapes, quotations, int.MaxValue , options);
		}

		public static string[] Split(this string str, char[] separators, char[] trims, StringEscape[] escapes, StringQuotation[] quotations, int count, StringSplitOptionsEx options)
		{
			if (str == null)
				throw new NullReferenceException();
			if (count < 0)
				throw new ArgumentOutOfRangeException("count");
			if (trims != null && separators != null && separators.Any(c => trims.Any(t => t == c)))
				throw new ArgumentException("Trim symbol is contained in separators");
			if (escapes != null && separators != null && separators.Any(c => escapes.Any(e => e.Escape == c)))
				throw new ArgumentException("Escape symbol is contained in separators");
      //
			if (count-- == 0)
				return new string[0];
			List<KeyValuePair<int, int>> openQuotes = new List<KeyValuePair<int, int>>();
			List<string> itemsList = new List<string>();
			StringBuilder sb = new StringBuilder();
			int escapeIndex = -1;
			int quoteIndex = -1;
			int orphanCount = 0;
			int trimCount = 0;
			for (int i = 0; i < str.Length; i++)
			{
				//	Escape
				if (escapeIndex >= 0)
				{
          if (!escapes[escapeIndex].Oneself || escapes[escapeIndex].Escape == str[i])
          {
            escapeIndex = -1;
            sb.Append(str[i]);
            continue;
          }
          else
          {
            escapeIndex = -1;
            i--;
          }
				}
				else if (escapes != null && escapes.Length > 0)
				{
          escapeIndex = Array.FindIndex(escapes, t => t.Escape == str[i]);
          if (escapeIndex >= 0)
          {
            //  Self quoted
            if (!escapes[escapeIndex].SelfQuoted || i < str.Length - 1 && openQuotes.FindLastIndex(p => quotations[p.Key].Open == str[i]) >= 0)
              continue;
            else
              escapeIndex = -1;
          }
				}
				//	Quotclose
				if (openQuotes.Count > 0)
				{
          int openIndex = openQuotes.FindLastIndex(p => quotations[p.Key].Close == str[i] || !quotations[p.Key].Orphan);
          if (openIndex >= 0 && quotations[openQuotes[openIndex].Key].Close == str[i])
					{
            //  Trim quotation
            if (quotations[openQuotes[openIndex].Key].Trim && trims != null)
            {
              int removeCount= 0;
              for (int j = openQuotes[openIndex].Value + 1; j < sb.Length && trims.Contains(sb[j]); j++)
                removeCount++;
              if (removeCount > 0)
                sb.Remove(openQuotes[openIndex].Value + 1, removeCount);
              removeCount = 0;
              for (int j = sb.Length - 1; j > openQuotes[openIndex].Value && trims.Contains(sb[j]); j--)
                removeCount++;
              if (removeCount > 0)
                sb.Remove(sb.Length - removeCount, removeCount);
            }
            //
            if (quotations[openQuotes[openIndex].Key].Unquote)
						  sb.Remove(openQuotes[openIndex].Value, 1);
					  else
						  sb.Append(str[i]);
            //
            for (int j = openIndex; j < openQuotes.Count; j++)
              if (quotations[openQuotes[j].Key].Orphan)
                orphanCount--;
					  openQuotes.RemoveRange(openIndex, openQuotes.Count - openIndex);
					  continue;
					}
				}
				//	Quotopen
        quoteIndex = quotations != null ? Array.FindIndex(quotations, t => t.Open == str[i]) : -1;
				if (quoteIndex >= 0)
				{
					openQuotes.Add(new KeyValuePair<int, int>(quoteIndex, sb.Length));
          sb.Append(str[i]);
          if (quotations[quoteIndex].Orphan)
						orphanCount++;
					continue;
				}
				//	Separate
				if (count > 0 && separators.Contains(str[i]) && openQuotes.Count == orphanCount)
				{
					//	Trim
          if (trims != null && sb.Length > 0)
          {
            if ((options & StringSplitOptionsEx.NoTrimLeft) == 0)
            {
              for (trimCount = 0; trimCount < sb.Length && trims.Contains(sb[trimCount]); trimCount++) ;
              sb.Remove(0, trimCount);
            }
            if ((options & StringSplitOptionsEx.NoTrimRight) == 0)
            {
              for (trimCount = 0; trimCount < sb.Length && trims.Contains(sb[sb.Length - 1 - trimCount]); trimCount++) ;
              sb.Remove(sb.Length - trimCount, trimCount);
            }
          }
          //
					if (sb.Length != 0 || (options & StringSplitOptionsEx.RemoveEmptyEntries) == 0)
						itemsList.Add(sb.ToString());
					sb.Length = 0;
					openQuotes.Clear();
					orphanCount = 0;
					count--;
					continue;
				}
				//
				sb.Append(str[i]);
			}
			if (escapeIndex >= 0)
				throw new InvalidOperationException("Iwas orphaescapsymbol");
			//	Trim
      if (trims != null && sb.Length > 0 && (options & StringSplitOptionsEx.NoTrimLeft) == 0)
			{
				for (trimCount = 0; trimCount < sb.Length && trims.Contains(sb[trimCount]); trimCount++) ;
				sb.Remove(0, trimCount);
			}
      if (trims != null && sb.Length > 0 && (options & StringSplitOptionsEx.NoTrimRight) == 0)
			{
				for (trimCount = 0; trimCount < sb.Length && trims.Contains(sb[sb.Length - 1 - trimCount]); trimCount++) ;
				sb.Remove(sb.Length - trimCount, trimCount);
			}
      //
			if (sb.Length != 0 || (options & StringSplitOptionsEx.RemoveEmptyEntries) == 0)
				itemsList.Add(sb.ToString());
			sb.Length = 0;
			return itemsList.ToArray();
		}

    public static string[] Split(this string str, char[] separators, char[] trims, char[] controlSeparators, char[] controlEscapes, string controls, int count, StringSplitOptionsEx options)
    {
      StringEscape[] controlEscapeArray = controlEscapes != null ? new StringEscape[controlEscapes.Length] : null;
      if (controlEscapeArray != null)
        for (int i = 0; i < controlEscapes.Length; i++)
          controlEscapeArray[i] = new StringEscape() { Escape = controlEscapes[i], Oneself = false, SelfQuoted = false };
      //
      PwrList<StringEscape> escapeItems = new PwrList<StringEscape>();
      PwrList<StringQuotation> quotationItems = new PwrList<StringQuotation>();
      if (!string.IsNullOrEmpty(controls))
      {
        string[] controlItems = controls.Split(controlSeparators, new[] { ' ' }, controlEscapeArray, null, int.MaxValue , StringSplitOptionsEx.RemoveEmptyEntries);
        for (int i = 0; i < controlItems.Length; i++)
        {
          int p = 0;
          switch (controlItems[i][p++])
          {
            case 'E':
            case 'e':
              if (p == controlItems[i].Length)
                throw new ArgumentException("Invaliformat", "controls");
              StringEscape escape = new StringEscape() { Escape = controlItems[i][p++] };
              while (p + 1 < controlItems[i].Length)
              {
                switch (controlItems[i][p++])
                {
                  case 'O':
                  case 'o':
                    switch (controlItems[i][p++])
                    {
                      case '+':
                        escape.Oneself = true;
                        break;
                      case '-':
                        escape.Oneself = false;
                        break;
                      default:
                        throw new ArgumentException("Invaliformat", "controls");
                    }
                    break;
                  case 'Q':
                  case 'q':
                    switch (controlItems[i][p++])
                    {
                      case '+':
                        escape.SelfQuoted = true;
                        break;
                      case '-':
                        escape.SelfQuoted = false;
                        break;
                      default:
                        throw new ArgumentException("Invaliformat", "controls");
                    }
                    break;
                  default:
                    throw new ArgumentException("Invaliformat", "controls");
                }
              }
              if (p != controlItems[i].Length)
                throw new ArgumentException("Invaliformat", "controls");
              escapeItems.Add(escape);
              break;
            case 'Q':
            case 'q':
              if (p + 2 > controlItems[i].Length)
                throw new ArgumentException("Invaliformat", "controls");
              StringQuotation quotation = new StringQuotation() { Open = controlItems[i][p++], Close = controlItems[i][p++] };
              while (p + 1 < controlItems[i].Length)
              {
                switch (controlItems[i][p++])
                {
                  case 'U':
                  case 'u':
                    switch (controlItems[i][p++])
                    {
                      case '+':
                        quotation.Unquote = true;
                        break;
                      case '-':
                        quotation.Unquote = false;
                        break;
                      default:
                        throw new ArgumentException("Invaliformat", "controls");
                    }
                    break;
                  case 'O':
                  case 'o':
                    switch (controlItems[i][p++])
                    {
                      case '+':
                        quotation.Orphan = true;
                        break;
                      case '-':
                        quotation.Orphan = false;
                        break;
                      default:
                        throw new ArgumentException("Invaliformat", "controls");
                    }
                    break;
                  case 'T':
                  case 't':
                    switch (controlItems[i][p++])
                    {
                      case '+':
                        quotation.Trim = true;
                        break;
                      case '-':
                        quotation.Trim = false;
                        break;
                      default:
                        throw new ArgumentException("Invaliformat", "controls");
                    }
                    break;
                  default:
                    throw new ArgumentException("Invaliformat", "controls");
                }
              }
              if (p != controlItems[i].Length)
                throw new ArgumentException("Invaliformat", "controls");
              quotationItems.Add(quotation);
              break;
          }
        }
      }
      //
      return Split(str, separators, trims, escapeItems.ToArray(), quotationItems.ToArray(), count, options);
    }

		public static IEnumerable<KeyValuePair<string, string>> SplitToKeyValue(this string str, char[] itemDelimiters, char[] keyDelimiters)
		{
			return SplitToKeyValue(str, itemDelimiters, keyDelimiters, int.MaxValue , StringSplitOptions.None);
		}

		public static IEnumerable<KeyValuePair<string, string>> SplitToKeyValue(this string str, char[] itemDelimiters, char[] keyDelimiters, int count)
		{
			return SplitToKeyValue(str, itemDelimiters, keyDelimiters, count, StringSplitOptions.None);
		}

		public static IEnumerable<KeyValuePair<string, string>> SplitToKeyValue(this string str, char[] itemDelimiters, char[] keyDelimiters, StringSplitOptions options)
		{
			return SplitToKeyValue(str, itemDelimiters, keyDelimiters, int.MaxValue , options);
		}

		public static IEnumerable<KeyValuePair<string, string>> SplitToKeyValue(this string str, char[] itemDelimiters, char[] keyDelimiters, int count, StringSplitOptions options)
		{
			return str.Split(itemDelimiters, count, options).Select(item =>
				{
					string[] items = item.Split(keyDelimiters, 2, options & ~StringSplitOptions.RemoveEmptyEntries);
					return new KeyValuePair<string, string>(items[0], items.Length > 1 && (items[1] != string.Empty || (options & StringSplitOptions.RemoveEmptyEntries) == 0) ? items[1] : null);
				});
		}

		public static IEnumerable<KeyValuePair<string, string>> SplitToKeyValue(this string str, char[] itemDelimiters, char[] keyDelimiters,
			char[] trims, StringEscape[] escapes, StringQuotation[] quotations)
		{
			return SplitToKeyValue(str, itemDelimiters, keyDelimiters, trims, escapes, quotations, int.MaxValue , StringSplitOptionsEx.None);
		}

		public static IEnumerable<KeyValuePair<string, string>> SplitToKeyValue(this string str, char[] itemDelimiters, char[] keyDelimiters,
			char[] trims, StringEscape[] escapes, StringQuotation[] quotations, int count)
		{
			return SplitToKeyValue(str, itemDelimiters, keyDelimiters, trims, escapes, quotations, count, StringSplitOptionsEx.None);
		}

		public static IEnumerable<KeyValuePair<string, string>> SplitToKeyValue(this string str, char[] itemDelimiters, char[] keyDelimiters,
			char[] trims, StringEscape[] escapes, StringQuotation[] quotations, StringSplitOptionsEx options)
		{
			return SplitToKeyValue(str, itemDelimiters, keyDelimiters, trims, escapes, quotations, int.MaxValue , options);
		}

		public static IEnumerable<KeyValuePair<string, string>> SplitToKeyValue(this string str, char[] itemDelimiters, char[] keyDelimiters,
			char[] trims, StringEscape[] escapes, StringQuotation[] quotations, int count, StringSplitOptionsEx options)
		{
			return str.Split(itemDelimiters, trims, escapes, quotations, count, options).Select(item =>
				{
					string[] items = item.Split(keyDelimiters, trims, escapes, quotations, 2, options & ~StringSplitOptionsEx.RemoveEmptyEntries).ToArray();
					return new KeyValuePair<string, string>(items[0], items.Length > 1 && (items[1] != string.Empty || (options & StringSplitOptionsEx.RemoveEmptyEntries) == 0) ? items[1] : null);
				});
		}

		public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> SplitToKeyGroup(this string str, char[] itemDelimiters, char[] keyDelimiters, char[] elementDelimiters)
		{
			return SplitToKeyGroup(str, itemDelimiters, keyDelimiters, elementDelimiters, int.MaxValue , int.MaxValue , StringSplitOptions.None);
		}

		public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> SplitToKeyGroup(this string str, char[] itemDelimiters, char[] keyDelimiters, char[] elementDelimiters,
			int itemCount, int elementCount)
		{
			return SplitToKeyGroup(str, itemDelimiters, keyDelimiters, elementDelimiters, itemCount, elementCount, StringSplitOptions.None);
		}

		public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> SplitToKeyGroup(this string str, char[] itemDelimiters, char[] keyDelimiters, char[] elementDelimiters,
			StringSplitOptions options)
		{
			return SplitToKeyGroup(str, itemDelimiters, keyDelimiters, elementDelimiters, int.MaxValue , int.MaxValue , options);
		}

		public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> SplitToKeyGroup(this string str, char[] itemDelimiters, char[] keyDelimiters, char[] elementDelimiters,
			int itemCount, int elementCount, StringSplitOptions options)
		{
			return str.Split(itemDelimiters, itemCount, options).Select<string, KeyValuePair<string, IEnumerable<string>>>(item =>
				{
					string[] items = item.Split(keyDelimiters, 2, options & ~StringSplitOptions.RemoveEmptyEntries);
					return new KeyValuePair<string, IEnumerable<string>>(items[0], items.Length > 1 && (items[1] != string.Empty || (options & StringSplitOptions.RemoveEmptyEntries) == 0)
						? items[1].Split(elementDelimiters, elementCount, options) : Enumerable.Empty<string>());
				});
		}

		public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> SplitToKeyGroup(this string str, char[] itemDelimiters, char[] keyDelimiters, char[] elementDelimiters,
			char[] trims, StringEscape[] escapes, StringQuotation[] quotations)
		{
			return SplitToKeyGroup(str, itemDelimiters, keyDelimiters, elementDelimiters, trims, escapes, quotations, int.MaxValue , int.MaxValue , StringSplitOptionsEx.None);
		}

		public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> SplitToKeyGroup(this string str, char[] itemDelimiters, char[] keyDelimiters, char[] elementDelimiters,
			char[] trims, StringEscape[] escapes, StringQuotation[] quotations, int itemCount, int elementCount)
		{
			return SplitToKeyGroup(str, itemDelimiters, keyDelimiters, elementDelimiters, trims, escapes, quotations, itemCount, elementCount, StringSplitOptionsEx.None);
		}

		public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> SplitToKeyGroup(this string str, char[] itemDelimiters, char[] keyDelimiters, char[] elementDelimiters,
			char[] trims, StringEscape[] escapes, StringQuotation[] quotations, StringSplitOptionsEx options)
		{
			return SplitToKeyGroup(str, itemDelimiters, keyDelimiters, elementDelimiters, trims, escapes, quotations, int.MaxValue , int.MaxValue , options);
		}

		public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> SplitToKeyGroup(this string str, char[] itemDelimiters, char[] keyDelimiters, char[] elementDelimiters,
			char[] trims, StringEscape[] escapes, StringQuotation[] quotations, int itemCount, int elementCount, StringSplitOptionsEx options)
		{
			return str.Split(itemDelimiters, trims, escapes, quotations, itemCount, options).Select<string, KeyValuePair<string, IEnumerable<string>>>(item =>
			{
				string[] items = item.Split(keyDelimiters, trims, escapes, quotations, 2, options & ~StringSplitOptionsEx.RemoveEmptyEntries);
				return new KeyValuePair<string, IEnumerable<string>>(items[0], items.Length > 1 && (items[1] != string.Empty || (options & StringSplitOptionsEx.RemoveEmptyEntries) == 0)
					? items[1].Split(elementDelimiters, trims, escapes, quotations, elementCount, options) : Enumerable.Empty<string>());
			});
		}

		public static ILookup<string, string> SplitToLookup(this string str, char[] itemDelimiters, char[] keyDelimiters)
		{
			return SplitToLookup(str, itemDelimiters, keyDelimiters, int.MaxValue , StringSplitOptions.None);
		}

		public static ILookup<string, string> SplitToLookup(this string str, char[] itemDelimiters, char[] keyDelimiters, int count)
		{
			return SplitToLookup(str, itemDelimiters, keyDelimiters, count, StringSplitOptions.None);
		}

		public static ILookup<string, string> SplitToLookup(this string str, char[] itemDelimiters, char[] keyDelimiters, StringSplitOptions options)
		{
			return SplitToLookup(str, itemDelimiters, keyDelimiters, int.MaxValue , options);
		}

		public static ILookup<string, string> SplitToLookup(this string str, char[] itemDelimiters, char[] keyDelimiters, int count, StringSplitOptions options)
		{
			return str.Split(itemDelimiters, count, options).ToLookup(item =>
			{
				string[] items = item.Split(keyDelimiters, 2, options & ~StringSplitOptions.RemoveEmptyEntries);
				return items[0];
			}, item =>
			{
				string[] items = item.Split(keyDelimiters, 2, options & ~StringSplitOptions.RemoveEmptyEntries);
				return items.Length > 1 && (items[1] != string.Empty || (options & StringSplitOptions.RemoveEmptyEntries) == 0) ? items[1] : null;
			});
		}

		public static ILookup<string, string> SplitToLookup(this string str, char[] itemDelimiters, char[] keyDelimiters, char[] trims, StringEscape[] escapes, StringQuotation[] quotations)
		{
			return SplitToLookup(str, itemDelimiters, keyDelimiters, trims, escapes, quotations, int.MaxValue , StringSplitOptionsEx.None);
		}

		public static ILookup<string, string> SplitToLookup(this string str, char[] itemDelimiters, char[] keyDelimiters, char[] trims, StringEscape[] escapes, StringQuotation[] quotations, int count)
		{
			return SplitToLookup(str, itemDelimiters, keyDelimiters, trims, escapes, quotations, count, StringSplitOptionsEx.None);
		}

		public static ILookup<string, string> SplitToLookup(this string str, char[] itemDelimiters, char[] keyDelimiters, char[] trims, StringEscape[] escapes, StringQuotation[] quotations, StringSplitOptionsEx options)
		{
			return SplitToLookup(str, itemDelimiters, keyDelimiters, trims, escapes, quotations, int.MaxValue , options);
		}

		public static ILookup<string, string> SplitToLookup(this string str, char[] itemDelimiters, char[] keyDelimiters, char[] trims, StringEscape[] escapes, StringQuotation[] quotations, int count, StringSplitOptionsEx options)
		{
			return str.Split(itemDelimiters, trims, escapes, quotations, count, options).ToLookup(item =>
			{
				string[] items = item.Split(keyDelimiters, trims, escapes, quotations, 2, options & ~StringSplitOptionsEx.RemoveEmptyEntries);
				return items[0];
			}, item =>
			{
				string[] items = item.Split(keyDelimiters, trims, escapes, quotations, 2, options & ~StringSplitOptionsEx.RemoveEmptyEntries);
				return items.Length > 1 && (items[1] != string.Empty || (options & StringSplitOptionsEx.RemoveEmptyEntries) == 0) ? items[1] : null;
			});
		}

		public static IDictionary<string, string> SplitToDictionary(this string str, char[] itemDelimiters, char[] keyValueDelimiters)
		{
			return SplitToDictionary(str, itemDelimiters, keyValueDelimiters, int.MaxValue , StringSplitOptions.None);
		}

		public static IDictionary<string, string> SplitToDictionary(this string str, char[] itemDelimiters, char[] keyValueDelimiters, int count)
		{
			return SplitToDictionary(str, itemDelimiters, keyValueDelimiters, count, StringSplitOptions.None);
		}

		public static IDictionary<string, string> SplitToDictionary(this string str, char[] itemDelimiters, char[] keyValueDelimiters, StringSplitOptions options)
		{
			return SplitToDictionary(str, itemDelimiters, keyValueDelimiters, int.MaxValue , options);
		}

		public static IDictionary<string, string> SplitToDictionary(this string str, char[] itemDelimiters, char[] keyValueDelimiters, int count, StringSplitOptions options)
		{
			return str.Split(itemDelimiters, count, options).ToDictionary(item =>
				{
					string[] items = item.Split(keyValueDelimiters, 2, options & ~StringSplitOptions.RemoveEmptyEntries);
					return items[0];
				}, item =>
				{
					string[] items = item.Split(keyValueDelimiters, 2, options & ~StringSplitOptions.RemoveEmptyEntries);
					return items.Length > 1 && (items[1] != string.Empty || (options & StringSplitOptions.RemoveEmptyEntries) == 0) ? items[1] : null;
				});
		}

		public static IDictionary<string, string> SplitToDictionary(this string str, char[] itemDelimiters, char[] keyValueDelimiters, char[] trims, StringEscape[] escapes, StringQuotation[] quotations)
		{
			return SplitToDictionary(str, itemDelimiters, keyValueDelimiters, trims, escapes, quotations, int.MaxValue , StringSplitOptionsEx.None);
		}

		public static IDictionary<string, string> SplitToDictionary(this string str, char[] itemDelimiters, char[] keyValueDelimiters, char[] trims, StringEscape[] escapes, StringQuotation[] quotations, int count)
		{
			return SplitToDictionary(str, itemDelimiters, keyValueDelimiters, trims, escapes, quotations, count, StringSplitOptionsEx.None);
		}

		public static IDictionary<string, string> SplitToDictionary(this string str, char[] itemDelimiters, char[] keyValueDelimiters, char[] trims, StringEscape[] escapes, StringQuotation[] quotations, StringSplitOptionsEx options)
		{
			return SplitToDictionary(str, itemDelimiters, keyValueDelimiters, trims, escapes, quotations, int.MaxValue , options);
		}

		public static IDictionary<string, string> SplitToDictionary(this string str, char[] itemDelimiters, char[] keyValueDelimiters, char[] trims, StringEscape[] escapes, StringQuotation[] quotations, int count, StringSplitOptionsEx options)
		{
			return str.Split(itemDelimiters, trims, escapes, quotations, count, options).ToDictionary(item =>
			{
				string[] items = item.Split(keyValueDelimiters, trims, escapes, quotations, 2, options & ~StringSplitOptionsEx.RemoveEmptyEntries);
				return items[0];
			}, item =>
			{
				string[] items = item.Split(keyValueDelimiters, trims, escapes, quotations, 2, options & ~StringSplitOptionsEx.RemoveEmptyEntries);
				return items.Length > 1 && (items[1] != string.Empty || (options & StringSplitOptionsEx.RemoveEmptyEntries) == 0) ? items[1] : null;
			});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="exceptOf"></param>
		/// <returns></returns>
		public static int IndexOfExcept(this string str, char[] exceptOf)
		{
			return IndexOfExcept(str, exceptOf, 0, str != null ? str.Length : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="exceptOf"></param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public static int IndexOfExcept(this string str, char[] exceptOf, int startIndex)
		{
			return IndexOfExcept(str, exceptOf, startIndex, str != null ? str.Length - startIndex : 0);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="str"></param>
		/// <param name="exceptOf"></param>
		/// <param name="startIndex"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static int IndexOfExcept(this string str, char[] exceptOf, int startIndex, int count)
		{
			if (str == null)
				throw new NullReferenceException();
			if (exceptOf == null)
				throw new ArgumentNullException("exceptOf");
			if (startIndex < 0 || startIndex > str.Length)
				throw new ArgumentOutOfRangeException("startIndex");
			if (count < 0 || count > str.Length - startIndex)
				throw new ArgumentOutOfRangeException("count");

			while (count-- > 0)
				if (!exceptOf.Contains(str[startIndex]))
					return startIndex;
				else
					startIndex++;
			return -1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="exceptOf"></param>
		/// <returns></returns>
		public static int LastIndexOfExcept(this string str, char[] exceptOf)
		{
			return LastIndexOfExcept(str, exceptOf, 0, str != null ? str.Length : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="exceptOf"></param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public static int LastIndexOfExcept(this string str, char[] exceptOf, int startIndex)
		{
			return LastIndexOfExcept(str, exceptOf, startIndex, str != null ? str.Length - startIndex : 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="exceptOf"></param>
		/// <param name="startIndex"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static int LastIndexOfExcept(this string str, char[] exceptOf, int startIndex, int count)
		{
			if (str == null)
				throw new NullReferenceException();
			if (exceptOf == null)
				throw new ArgumentNullException("exceptOf");
			if (startIndex < -1 || startIndex >= str.Length + (str.Length == 0 ? 1 : 0))
				throw new ArgumentOutOfRangeException("startIndex");
			if (count < 0 || count > startIndex + 1)
				throw new ArgumentOutOfRangeException("count");

			while (count-- > 0)
				if (!exceptOf.Contains(str[startIndex]))
					return startIndex;
				else
					startIndex--;
			return -1;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum StringSplitOptionsEx
	{
		None = 0,
		RemoveEmptyEntries = 1,
		NoTrimLeft = 2,
		NoTrimRight = 4
	}

	/// <summary>
	/// 
	/// </summary>
	public class StringEscape
	{
		/// <summary>
		/// Escape symbol
		/// </summary>
		public char Escape { get; set; }

		/// <summary>
		/// Determines that symbol escapes onself only
		/// </summary>
		public bool Oneself { get; set; }

    /// <summary>
    /// Determines that symbol escapes self quoted
    /// </summary>
    public bool SelfQuoted { get; set; }
	}

	/// <summary>
	///
	/// </summary>
	public class StringQuotation
	{
		/// <summary>
		/// Opequoting symbol
		/// </summary>
		public char Open { get; set; }

		/// <summary>
		/// Closquoting symbol
		/// </summary>
		public char Close { get; set; }

		/// <summary>
		/// Unquote quoted substring
		/// </summary>
		public bool Unquote { get; set; }

		/// <summary>
		/// Determines that separator symbol has higher priority and would be separate always
		/// </summary>
		public bool Orphan { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Trim { get; set; }
	}
}
