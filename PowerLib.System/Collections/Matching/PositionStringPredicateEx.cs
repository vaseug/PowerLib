using System.Globalization;

namespace PowerLib.System.Collections.Matching
{
	public class PositionStringPredicateEx : StringPredicateEx
	{
		private int _index = 0;

		#region Constructors

		public PositionStringPredicateEx(int index, string pattern)
			: base(pattern)
		{
			_index = index;
		}

		public PositionStringPredicateEx(int index, string pattern, CompareOptions options, CultureInfo ci)
			: base(pattern, options, ci)
		{
			_index = index;
		}

		#endregion
		#region Properties

		public int Index
		{
			get { return _index; }
		}

		#endregion
		#region Methods

		protected override bool MatchCore(string text)
		{
			return (Index < 0 && text.Length + Index >= 0 || Index >= 0 && Index <= text.Length) &&
				string.Compare(text, (Index < 0 ? text.Length : 0) + Index, Pattern, 0, Pattern.Length, CultureInfo, Options) == 0;
		}

		#endregion
	}

}
