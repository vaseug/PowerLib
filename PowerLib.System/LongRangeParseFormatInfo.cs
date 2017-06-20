using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using PowerLib.System.Text;

namespace PowerLib.System
{
  public class LongRangeParseFormatInfo : IFormatter<LongRange>, IParser<LongRange>, INumberParser<LongRange>
  {
    private const string DecPattern = "[-+]?[0-9]+";
    private const string HexPattern = "[0-9A-Fa-f]+";
    private const string FormatString = @"{2}{{0{0}}}{1}{{1{0}}}{3}"; // parameters: 0 - number format, 1 - delimiter, 2 - opening, 3 - closing
    private const string PatternString =
      @"(?:" +
        @"(?'open'{2}[{4}]*)" +
          @"(?'index'{0})[{4}]*{1}[{4}]*(?'count'{0})" +
        @"(?'close-open'[{4}]*{3})(?(open)(?!))" +
      @")"; // parameters: 0 - item pattern, 1 - delimiter, 2 - opening, 3 - closing, 4 - spaces

    private string _delimiter;
    private string _opening;
    private string _closing;
    private char[] _spaces;

    private static Lazy<LongRangeParseFormatInfo> @default = new Lazy<LongRangeParseFormatInfo>(() => new LongRangeParseFormatInfo(",", "(", ")", new[] { ' ', '\t' }));

    public static LongRangeParseFormatInfo Default
    {
      get { return @default.Value; }
    }

    public LongRangeParseFormatInfo(string delimiter, string opening, string closing, char[] spaces)
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

    public string Format(LongRange value, string format, IFormatProvider formatProvider)
    {
      return string.Format(formatProvider, GetFormat(format, formatProvider), value.Index, value.Count);
    }

    public LongRange Parse(string s, IFormatProvider formatProvider)
    {
      return Parse(s, DefaultNumberStyles, formatProvider);
    }

    public LongRange Parse(string s, NumberStyles styles, IFormatProvider formatProvider)
    {
      LongRange result;
      if (!TryParse(s, styles, formatProvider, out result))
        throw new FormatException();
      return result;
    }

    public bool TryParse(string s, IFormatProvider formatProvider, out LongRange result)
    {
      return TryParse(s, DefaultNumberStyles, formatProvider, out result);
    }

    public bool TryParse(string s, NumberStyles styles, IFormatProvider formatProvider, out LongRange result)
    {
      if (!string.IsNullOrWhiteSpace(s))
      {
        var match = Regex.Match(s, GetPattern(styles, formatProvider), RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
          long index = match.Groups["index"].Success ? long.Parse(match.Groups["index"].Value, styles, formatProvider) : 0;
          long count = match.Groups["count"].Success ? long.Parse(match.Groups["count"].Value, styles, formatProvider) : 0;
          result = new LongRange(index, count);
          return true;
        }
      }
      result = default(LongRange);
      return false;
    }
  }
}
