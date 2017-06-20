using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using PowerLib.System.Text;

namespace PowerLib.System
{
  public class ArrayParseFormatInfo<T> : IFormatter<T[]>, IParser<T[]>
  {
    private const string arrayItemFormat = @"(?:(?![^{4}](?:{0}|{1}|{2})).)*"; // parameters: 0 - delimiter, 1 - open brackets, 2 - close brackets, 3 - spaces, 4 - escapes

    private const string arrayDelimiterFormat = @"[{3}]*(?<![{4}]){0}[{3}]*"; // parameters: 0 - delimiter, 1 - open brackets, 2 - close brackets, 3 - spaces, 4 - escapes

    private const string arrayPatternFormat = 
      @"^(?:" +
        @"(?'Open'{2}[{4}]*)" +
          @"(?'Items'(?:(?:(?<![^{5}]{2})[{4}]*(?<![{5}]){1}[{4}]*)?{0})*)" +
        @"(?'Close-Open'[{4}]*{3})(?(Open)(?!))" +
      @")$"; // parameters: 0 - item pattern, 1 - delimiter, 2 - opening, 3 - closing, 4 - spaces, 5 - escapes

    private Func<T, int, string, string> _itemFormatter;
    private Func<string, int, T> _itemParser;
    private string _itemPattern;
    private string _delimiter;
    private string _opening;
    private string _closing;
    private char[] _spaces;
    private char[] _escapes;

    public ArrayParseFormatInfo(Func<T, int, string, string> itemFormatter, Func<string, int, T> itemParser, string itemPattern, string delimiter, string opening, string closing, char[] spaces, char[] escapes)
    {
      _itemFormatter = itemFormatter;
      _itemParser = itemParser;
      _itemPattern = itemPattern;
      _delimiter = delimiter;
      _opening = opening;
      _closing = closing;
      _spaces = (char[])spaces.Clone();
      _escapes = (char[])escapes.Clone();
    }

    public string Format(T[] value, string format, IFormatProvider formatProvider)
    {
      if (value == null)
        return null;
      StringBuilder sb = new StringBuilder();
      if (_opening != null)
        sb.Append(_opening);
      for (int i = 0; i < value.Length; i++)
      {
        if (i > 0 && _delimiter != null)
          sb.Append(_delimiter);
        sb.Append(_itemFormatter(value[i], i, format));
      }
      if (_closing != null)
        sb.Append(_closing);
      return sb.ToString();
    }

    public T[] Parse(string s, IFormatProvider formatProvider)
    {
      T[] result;
      if (!TryParse(s, formatProvider, out result))
        throw new FormatException();
      return result;
    }

    public bool TryParse(string s, IFormatProvider formatProvider, out T[] result)
    {
      if (s != null)
      {
        string fmt;
        string pattern = string.Format(arrayPatternFormat, !string.IsNullOrEmpty(_itemPattern) ? _itemPattern : fmt = string.Format(arrayItemFormat,
          Regex.Escape(_delimiter), Regex.Escape(_opening), Regex.Escape(_closing), _spaces != null ? Regex.Escape(new string(_spaces)) : string.Empty, _escapes != null ? Regex.Escape(new string(_escapes)) : string.Empty),
          Regex.Escape(_delimiter), Regex.Escape(_opening), Regex.Escape(_closing), _spaces != null ? Regex.Escape(new string(_spaces)) : string.Empty, _escapes != null ? Regex.Escape(new string(_escapes)) : string.Empty);
        Match match = Regex.Match(s, pattern, RegexOptions.Multiline);
        if (match.Success)
        {
          string splitter = string.Format(arrayDelimiterFormat,
            Regex.Escape(_delimiter), Regex.Escape(_opening), Regex.Escape(_closing), _spaces != null ? Regex.Escape(new string(_spaces)) : string.Empty, _escapes != null ? Regex.Escape(new string(_escapes)) : string.Empty);
          Regex split = new Regex(splitter, RegexOptions.Multiline);
          result = split.Split(match.Groups["Items"].Captures[0].Value).Select((t, i) => _itemParser(t, i)).ToArray();
          return true;
        }
      }
      result = null;
      return false;
    }
  }
}
