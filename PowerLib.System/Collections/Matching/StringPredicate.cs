using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PowerLib.System.Collections.Matching
{
	public class StringPredicate : IPredicate<string>
	{
		private string _pattern;

		#region Constructors

		public StringPredicate(string pattern)
		{
			_pattern = pattern;
		}

		#endregion
		#region Properties

		public string Pattern
		{
			get { return _pattern; }
		}

		#endregion
		#region Methods

		protected virtual bool MatchCore(string text)
		{
			return string.Equals(Pattern, text);
		}

    #endregion
    #region Methods

    public bool Match(string value)
		{
			return MatchCore(value);
		}

		#endregion
	}
}
