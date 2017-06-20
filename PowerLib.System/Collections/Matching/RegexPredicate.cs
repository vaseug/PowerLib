using System;
using System.Text.RegularExpressions;


namespace PowerLib.System.Collections.Matching
{
	/// <summary>
	/// Match strings by regulaexpression.
	/// </summary>
	public sealed class RegexPredicate : StringPredicate
	{
		private Regex _re;

		#region Constructors

		/// <summary>
		/// Initializinstancby <paramref name="pattern"/>.
		/// </summary>
		/// <param name="pattern">Regulaexpressiopattern</param>
		public RegexPredicate(string pattern)
			: base(pattern)
		{
			if (pattern == null)
				throw new ArgumentNullException("pattern");
			//
			_re = new Regex(pattern);
		}

		/// <summary>
		/// Initializinstancby <paramref name="pattern"/> an<paramref name="options"/>.
		/// </summary>
		/// <param name="pattern">Regulaexpressiopattern.</param>
		/// <param name="options">Regulaexpressiooptions.</param>
		public RegexPredicate(string pattern, RegexOptions options)
			: base(pattern)
		{
			if (pattern == null)
				throw new ArgumentNullException("pattern");
			//
			_re = new Regex(pattern, options);
		}

		/// <summary>
		/// Initializinstancby <paramref name="regex"/>.
		/// </summary>
		/// <param name="regex">Regex object.</param>
		public RegexPredicate(Regex regex)
			: base(null)
		{
			if (regex == null)
				throw new ArgumentNullException("regex");
			//
			_re = regex;
		}

		#endregion
		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public RegexOptions Options
		{
			get { return _re.Options; }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Match string tregulaexpressiopattern
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		protected override bool MatchCore(string text)
		{
			if (text == null)
				throw new ArgumentNullException();

			return _re.IsMatch(text);
		}

		#endregion
	}
}
