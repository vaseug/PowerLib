using System;
using System.Numerics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using PowerLib.System.Numerics;

namespace PowerLib.System.Numerics
{
	public static class ComplexNumber
	{
    private const string NumberPattern = "[-+]?[0-9]+(?:[{0}][0-9]+)?(?:[e][-+]?[0-9]+)?";

    //  Pattern formatting parameters: 0 - item pattern, 1 - delimiters, 2 - spaces, 3 - escapes, 4 - open brackets, 5 - close brackets
    private const string PatternFormat =
@"^[ ]*(?:" +
  @"(?:(?'real'[-+]?[0-9]+(?:[{0}][0-9]+)?(?:[e][-+]?[0-9]+)?)?(?:[ ]*(?'image'(?:(?<=[0-9])[-+]|(?<![0-9])[-+]?)[0-9]+(?:[{0}][0-9]+)?(?:[e][-+]?[0-9]+)?)(?'suffix'[idghrp]))?)" +
  @"|" +
  @"(?:(?'open'\([ ]*)?" +
    @"(?:(?'real'[-+]?[0-9]+(?:[{0}][0-9]+)?(?:[e][-+]?[0-9]+)?)[ ]*[,][ ]*(?'image'[-+]?[0-9]+(?:[{0}][0-9]+)?(?:[e][-+]?[0-9]+)?))" +
  @"(?'close-open'[ ]*\))?(?(open)(?!)))" +
@")[ ]*$";

    public static Complex Parse(string s)
    {
      Complex result;
      if (!TryParse(s, out result))
        throw new ArgumentException("Invalid input string");
      return result;
    }

    public static Complex Parse(string s, IFormatProvider provider)
    {
      Complex result;
      if (!TryParse(s, provider, out result))
        throw new ArgumentException("Invalid input string");
      return result;
    }

    public static bool TryParse(string s, out Complex result)
    {
      return TryParse(s, null, out result);
    }

    public static bool TryParse(string s, IFormatProvider provider, out Complex result)
    {
      if (!string.IsNullOrWhiteSpace(s))
      {
        NumberFormatInfo numberFormatInfo = provider as NumberFormatInfo;
        if (numberFormatInfo == null)
          numberFormatInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
        if (numberFormatInfo == null)
        {
          CultureInfo culture = provider as CultureInfo;
          if (culture == null)
            numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
        }
        string pattern = string.Format(PatternFormat, numberFormatInfo.NumberDecimalSeparator);
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
  }
}
