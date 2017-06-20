using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Numerics;

namespace PowerLib.System.Numerics
{
  public static class Numeric
  {
    #region Patterns

    private static readonly string UnsignedPattern = @"[0-9]+";

    private static readonly string SignedPattern = string.Format(@"[-+]?{0}", UnsignedPattern);

    private static readonly string DecimalPattern = string.Format(@"{0}(?:[{{0}}]{1})?", SignedPattern, UnsignedPattern); // parameters: 0 - decimal separator

    private static readonly string ExponentialPattern = string.Format(@"{0}(?:[Ee]{1})?", DecimalPattern, SignedPattern);  // parameters: 0 - decimal separator

    private static readonly string ComplexPattern =
      @"(?:" +
        @"(?:" +
          string.Format(@"(?'real'{0})(?:[ ]+(?'image'{0})(?'suffix'[idghrp]))?", ExponentialPattern) +
        @")|(?:" +
          @"(?'open'{{2}}[ ]*)" +
            string.Format(@"(?'real'{0})[ ]*{{1}}[ ]*(?'image'{0})", ExponentialPattern) +
          @"(?'close-open'[ ]*{{3}})(?(open)(?!))" +
        @")" +
      @")"; // parameters: 0 - decimal separator, 1 - parts separator, 2 - open bracket, 3 - close bracket

    private static readonly string RangePattern =
      @"(?:" +
        @"(?'open'{{1}}[ ]*)" +
          string.Format(@"(?'index'{0})[ ]*{{0}}[ ]*(?'count'{0})", SignedPattern) +
        @"(?'close-open'[ ]*{{2}})(?(open)(?!))" +
      @")"; // parameters: 0 - parts separator, 1 - open bracket, 2 - close bracket

    #endregion
    #region Internal methods

    private static NumberFormatInfo GetNumberFormatInfo(IFormatProvider formatProvider)
    {
      NumberFormatInfo numberFormatInfo = formatProvider as NumberFormatInfo;
      if (numberFormatInfo == null)
        numberFormatInfo = formatProvider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
      if (numberFormatInfo == null)
      {
        CultureInfo culture = formatProvider as CultureInfo;
        if (culture == null)
          numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
      }
      return numberFormatInfo;
    }

    public static string GetDecimalPattern(IFormatProvider formatProvider)
    {
      return string.Format(DecimalPattern, GetNumberFormatInfo(formatProvider).NumberDecimalSeparator);
    }

    public static string GetExponentialPattern(IFormatProvider formatProvider)
    {
      return string.Format(ExponentialPattern, GetNumberFormatInfo(formatProvider).NumberDecimalSeparator);
    }

    public static string GetComplexPattern(IFormatProvider formatProvider, string separator, string open, string close)
    {
      return string.Format(ComplexPattern, GetNumberFormatInfo(formatProvider).NumberDecimalSeparator, separator, open, close);
    }

    public static string GetRangePattern(IFormatProvider formatProvider, string separator, string open, string close)
    {
      return string.Format(RangePattern, separator, open, close);
    }

    #endregion
    #region Complex methods

    public static Complex ParseComplex(string s)
    {
      Complex result;
      if (!TryParseCompex(s, out result))
        throw new ArgumentException("Invalid input string");
      return result;
    }

    public static Complex ParseComplex(string s, IFormatProvider provider)
    {
      Complex result;
      if (!TryParseComplex(s, provider, out result))
        throw new ArgumentException("Invalid input string");
      return result;
    }

    public static bool TryParseCompex(string s, out Complex result)
    {
      return TryParseComplex(s, null, out result);
    }

    public static bool TryParseComplex(string s, IFormatProvider provider, out Complex result)
    {
      if (!string.IsNullOrWhiteSpace(s))
      {
        string pattern = string.Format(ComplexPattern, GetNumberFormatInfo(provider).NumberDecimalSeparator);
        var match = Regex.Match(s, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
          double real = match.Groups["real"].Success ? double.Parse(match.Groups["real"].Value, provider) : 0d;
          double image = match.Groups["image"].Success ? double.Parse(match.Groups["image"].Value, provider) : 0d;
          switch (match.Groups["suffix"].Value)
          {
            case "d":
              result = Complex.FromPolarCoordinates(real, Angle.FromDegree(image));
              break;
            case "g":
              result = Complex.FromPolarCoordinates(real, Angle.FromGrad(image));
              break;
            case "h":
              result = Complex.FromPolarCoordinates(real, Angle.FromHour(image));
              break;
            case "r":
            case "p":
              result = Complex.FromPolarCoordinates(real, image);
              break;
            default:
              result = new Complex(real, image);
              break;
          }
          return true;
        }
      }
      result = default(Complex);
      return false;
    }

    #endregion
    #region Range methods

    public static bool TryParseRange(string s, IFormatProvider provider, out Range result)
    {
      if (!string.IsNullOrWhiteSpace(s))
      {
        string pattern = string.Format(RangePattern, ",", "(", ")");
        var match = Regex.Match(s, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
          int index = match.Groups["index"].Success ? int.Parse(match.Groups["index"].Value, provider) : 0;
          int count = match.Groups["count"].Success ? int.Parse(match.Groups["count"].Value, provider) : 0;
          result = new Range(index, count);
          return true;
        }
      }
      result = default(Range);
      return false;
    }

    #endregion
  }
}
