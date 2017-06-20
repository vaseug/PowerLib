using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace PowerLib.SqlServer.Text
{
	public static class SqlRegexFunctions
	{
    #region Regex functions

    [SqlFunction(Name = "regexIsMatch", IsDeterministic = true)]
		public static SqlBoolean IsMatch([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString pattern, SqlInt32 options)
		{
			if (input.IsNull || pattern.IsNull)
				return SqlBoolean.Null;

			return Regex.IsMatch(input.Value, pattern.Value, options.IsNull ? RegexOptions.None : (RegexOptions)options.Value);
		}

		[SqlFunction(Name = "regexMatches", IsDeterministic = true, FillRowMethodName = "MatchesFillRow")]
		public static IEnumerable Matches([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString pattern, SqlInt32 options)
		{
			if (input.IsNull || pattern.IsNull)
				yield break;

			Regex regex = new Regex(pattern.Value, options.IsNull ? RegexOptions.None : (RegexOptions)options.Value);
			MatchCollection matches = regex.Matches(input.Value);
			for (int i = 0; i < matches.Count; i++)
			{
				Match match = matches[i];
				for (int j = 0; j < match.Groups.Count; j++)
				{
					Group group = match.Groups[j];
					if (!group.Success)
						yield return new MatchRow(i, j, -1, group.Success, regex.GroupNameFromNumber(j), group.Index, group.Length, group.Value);
					else
						for (int k = 0; k < group.Captures.Count; k++)
						{
							Capture capture = group.Captures[k];
							yield return new MatchRow(i, j, k, group.Success, regex.GroupNameFromNumber(j), capture.Index, capture.Length, capture.Value);
						}
				}
			}
		}

		[SqlFunction(Name = "regexSplit", IsDeterministic = true, FillRowMethodName = "SplitFillRow")]
		public static IEnumerable Split([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString pattern, SqlInt32 options)
		{
			if (input.IsNull || pattern.IsNull)
				yield break;

			string[] array = Regex.Split(input.Value, pattern.Value, options.IsNull ? RegexOptions.None : (RegexOptions)options.Value);
			for (int i = 0; i < array.Length; i++)
				yield return array[i];
		}

		[SqlFunction(Name = "regexReplace", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
		public static SqlString Replace([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString pattern, [SqlFacet(MaxSize = -1)] SqlString replacement, SqlInt32 options)
		{
			if (input.IsNull || pattern.IsNull || replacement.IsNull)
				return SqlString.Null;

			return Regex.Replace(input.Value, pattern.Value, replacement.Value, options.IsNull ? RegexOptions.None : (RegexOptions)options.Value);
		}

		[SqlFunction(Name = "regexEscape", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Escape([SqlFacet(MaxSize = -1)] SqlString input)
		{
      if (input.IsNull)
        return SqlString.Null;

      return Regex.Escape(input.Value);
		}

		[SqlFunction(Name = "regexUnescape", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Unescape([SqlFacet(MaxSize = -1)] SqlString input)
		{
      if (input.IsNull)
        return SqlString.Null;
      
      return Regex.Unescape(input.Value);
		}

		#endregion
		#region FillRow functions

		private static void SplitFillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlString Value)
		{
			Value = (string)obj;
		}

		private static void MatchesFillRow(object obj, out SqlInt32 MatchNumber, out SqlInt32 GroupNumber, out SqlInt32 CaptureNumber,
			out SqlBoolean GroupSuccess, [SqlFacet(MaxSize = 255)] out SqlString GroupName, out SqlInt32 Index, out SqlInt32 Length, [SqlFacet(MaxSize = -1)] out SqlString Value)
		{
			MatchRow matchRow = (MatchRow)obj;
			MatchNumber = new SqlInt32(matchRow.MatchNumber);
			GroupNumber = new SqlInt32(matchRow.GroupNumber);
			CaptureNumber = new SqlInt32(matchRow.CaptureNumber);
			GroupSuccess = new SqlBoolean(matchRow.GroupSuccess);
			GroupName = new SqlString(matchRow.GroupName);
			Index = matchRow.GroupSuccess ? new SqlInt32(matchRow.Index) : SqlInt32.Null;
			Length = matchRow.GroupSuccess ? new SqlInt32(matchRow.Length) : SqlInt32.Null;
			Value = matchRow.GroupSuccess ? new SqlString(matchRow.Value) : SqlString.Null;
		}

		#endregion
		#region Row types

		private class MatchRow
		{
			private int _matchNumber;
			private int _groupNumber;
			private int _captureNumber;
			private bool _groupSuccess;
			private string _groupName;
			private int _index;
			private int _length;
			private string _value;

			#region Constructors

			public MatchRow(int matchNumber, int groupNumber, int captureNumber, bool groupSuccess, string groupName, int index, int length, string value)
			{
				_matchNumber = matchNumber;
				_groupNumber = groupNumber;
				_captureNumber = captureNumber;
				_groupSuccess = groupSuccess;
				_groupName = groupName;
				_index = index;
				_length = length;
				_value = value;
			}

			#endregion
			#region Properties

			public int MatchNumber
			{
				get
				{
					return _matchNumber;
				}
			}

			public int GroupNumber
			{
				get
				{
					return _groupNumber;
				}
			}

			public int CaptureNumber
			{
				get
				{
					return _captureNumber;
				}
			}

			public bool GroupSuccess
			{
				get
				{
					return _groupSuccess;
				}
			}

			public string GroupName
			{
				get
				{
					return _groupName;
				}
			}

			public int Index
			{
				get
				{
					return _index;
				}
			}

			public int Length
			{
				get
				{
					return _length;
				}
			}

			public string Value
			{
				get
				{
					return _value;
				}
			}

			#endregion
		}

		#endregion
	}
}
