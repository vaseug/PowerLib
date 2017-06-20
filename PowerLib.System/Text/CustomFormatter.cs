using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace PowerLib.System.Text
{
  public sealed class CustomFormatter : IFormatProvider, ICustomFormatter
  {
    private IDictionary<Type, Func<string, object, IFormatProvider, string>> _formatters;
    private IFormatProvider _formatProvider;

    #region Constructors

    public CustomFormatter(IDictionary<Type, Func<string, object, IFormatProvider, string>> formatters)
      : this(CultureInfo.CurrentCulture, formatters)
    {
    }

    public CustomFormatter(IFormatProvider formatProvider, IDictionary<Type, Func<string, object, IFormatProvider, string>> formatters)
    {
      if (formatters == null)
        throw new ArgumentNullException("cusromFormatters");
      if (formatProvider == null)
        throw new ArgumentNullException("formatProvider");

      _formatters = formatters;
      _formatProvider = formatProvider;
    }

    #endregion
    #region IFormatProvider implementation

    object IFormatProvider.GetFormat(Type formatType)
    {
      return formatType == typeof(ICustomFormatter) ? this : _formatProvider.GetFormat(formatType);
    }

    #endregion
    #region ICustomFormatter implementation

    string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
    {
      Func<string, object, IFormatProvider, string> formatter = null;
      if (arg != null)
        for (Type type = arg.GetType(); type != null; type = type.BaseType)
          if (_formatters.TryGetValue(type, out formatter))
            break;
      if (formatter != null)
        return formatter(format, arg, _formatProvider);

      ICustomFormatter customFormatter = _formatProvider != null ? (ICustomFormatter)_formatProvider.GetFormat(typeof(ICustomFormatter)) : null;
      if (customFormatter != null)
        return customFormatter.Format(format, arg, _formatProvider);

      IFormattable formattable = arg as IFormattable;
      if (formattable != null)
        return formattable.ToString(format, _formatProvider);

      return arg != null ? arg.ToString() : string.Empty;
    }

    #endregion
  }
}
