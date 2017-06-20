using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using PowerLib.System.Text;

namespace PowerLib.System
{
  public class RangeParseFormatInfo : IFormatter<Range>, IParser<Range>, INumberParser<Range>
  {
    private const string DecPattern = "[-+]?[0-9]+";
    private const string HexPattern = "[0-9A-Fa-f]+";
    private const string FormatString = @"{2}{{0{0}}}{1}{{1{0}}}{3}"; // parameters: 0 - number format, 1 - delimiter, 2 - opening, 3 - closing
    private const string PatternString =
      @"^(?:" +
        @"(?'open'{2}[{4}]*)" +
          @"(?'index'{0})[{4}]*{1}[{4}]*(?'count'{0})" +
        @"(?'close-open'[{4}]*{3})(?(open)(?!))" +
      @")$"; // parameters: 0 - item pattern, 1 - delimiter, 2 - opening, 3 - closing, 4 - spaces

    private string _delimiter;
    private string _opening;
    private string _closing;
    private char[] _spaces;

    private static Lazy<RangeParseFormatInfo> @default = new Lazy<RangeParseFormatInfo>(() => new RangeParseFormatInfo(",", "(", ")", new[] { ' ', '\t' }));

    public static RangeParseFormatInfo Default
    {
      get { return @default.Value; }
    }

    public RangeParseFormatInfo(string delimiter, string opening, string closing, char[] spaces)
    {
      _delimiter = delimiter;
      _opening = opening;
      _closing = closing;
      _spaces = (char[])spaces.Clone();
    }

    protected virtual NumberStyles DefaultNumberStyles
    {
      get { return NumberStyles.Integer; }
    }

    protected virtual string GetFormat(string format, IFormatProvider formatProvider)
    {
      return string.Format(formatProvider, FormatString,
        string.IsNullOrEmpty(format) ? string.Empty : ":" + format.Replace(@"{", @"{{").Replace(@"}", @"}}"),
        _delimiter.ToString().Replace(@"{", @"{{").Replace(@"}", @"}}"),
        _opening.ToString().Replace(@"{", @"{{").Replace(@"}", @"}}"),
        _closing.ToString().Replace(@"{", @"{{").Replace(@"}", @"}}"));
    }

    protected virtual string GetPattern(NumberStyles styles, IFormatProvider formatProvider)
    {
      return string.Format(formatProvider, PatternString, (styles & NumberStyles.AllowHexSpecifier) != 0 ? HexPattern : DecPattern,
        Regex.Escape(_delimiter), Regex.Escape(_opening), Regex.Escape(_closing), Regex.Escape(new string(_spaces)));
    }

    public string Format(Range value, string format, IFormatProvider formatProvider)
    {
      return string.Format(formatProvider, GetFormat(format, formatProvider), value.Index, value.Count);
    }

    public Range Parse(string s, IFormatProvider formatProvider)
    {
      return Parse(s, DefaultNumberStyles, formatProvider);
    }

    public Range Parse(string s, NumberStyles styles, IFormatProvider formatProvider)
    {
      Range result;
      if (!TryParse(s, styles, formatProvider, out result))
        throw new FormatException();
      return result;
    }

    public bool TryParse(string s, IFormatProvider formatProvider, out Range result)
    {
      return TryParse(s, DefaultNumberStyles, formatProvider, out result);
    }

    public bool TryParse(string s, NumberStyles styles, IFormatProvider formatProvider, out Range result)
    {
      if (!string.IsNullOrWhiteSpace(s))
      {
        string x;
        var match = Regex.Match(s, x = GetPattern(styles, formatProvider), RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
          int index = match.Groups["index"].Success ? int.Parse(match.Groups["index"].Value, styles, formatProvider) : 0;
          int count = match.Groups["count"].Success ? int.Parse(match.Groups["count"].Value, styles, formatProvider) : 0;
          result = new Range(index, count);
          return true;
        }
      }
      result = default(Range);
      return false;
    }
  }
}
