using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PowerLib.System.Collections.Matching
{
	public class StringPredicateEx : StringPredicate
	{
		private CompareOptions _options;
		private CultureInfo _ci;

		#region Constructors

		public StringPredicateEx(string pattern)
			: this(pattern, CompareOptions.None, CultureInfo.CurrentCulture)
		{
		}

		public StringPredicateEx(string pattern, bool ignoreCase)
			: this(pattern, CompareOptions.IgnoreCase, CultureInfo.CurrentCulture)
		{
		}

		public StringPredicateEx(string pattern, bool ignoreCase, CultureInfo ci)
			: this(pattern, CompareOptions.IgnoreCase, ci)
		{
		}

		//public StringPredicateEx(string pattern, StringComparisocomparisonType)
		//	: this(pattern, )
		//{
		//}

		public StringPredicateEx(string pattern, CompareOptions options, CultureInfo ci)
			: base(pattern)
		{
			if (ci == null)
				throw new ArgumentNullException("ci");

			_options = options;
			_ci = ci;
		}

		#endregion
		#region Properties

		public CultureInfo CultureInfo
		{
			get { return _ci; }
		}

		public CompareOptions Options
		{
			get { return _options; }
		}

		#endregion
		#region Methods

		protected override bool MatchCore(string text)
		{
			return string.Compare(Pattern, text, CultureInfo, Options) != 0;
		}

		#endregion
	}
}
