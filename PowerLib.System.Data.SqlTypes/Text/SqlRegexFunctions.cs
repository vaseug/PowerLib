using System;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace PowerLib.System.Data.SqlTypes.Text
{
	public static class SqlRegexFunctions
	{
    private static RegexOptions defaultRegexOptions = RegexOptions.None;

    #region Configuration

    public static RegexOptions DefaultRegexOptions
    {
      get { return defaultRegexOptions; }
    }

    [SqlProcedure(Name = "setRegexOptions")]
    public static void SetRegexOptions(SqlInt32 regexOptions)
    {
      if (regexOptions.IsNull)
        return;

      defaultRegexOptions = (RegexOptions)regexOptions.Value;
    }

    [SqlProcedure(Name = "getRegexOptions")]
    public static void GetRegexOptions(out SqlInt32 regexOptions)
    {
      regexOptions = (int)defaultRegexOptions;
    }

    #endregion
    #region Regex functions

    [SqlFunction(Name = "reCreate", IsDeterministic = true)]
    public static SqlRegex Create([SqlFacet(MaxSize = -1)] SqlString pattern, SqlInt32 options)
    {
      return pattern.IsNull ? SqlRegex.Null : new SqlRegex(pattern.Value, options.IsNull ? defaultRegexOptions : (RegexOptions)options.Value);
    }

    [SqlFunction(Name = "reMatches", IsDeterministic = true, FillRowMethodName = "MatchesFillRow")]
    public static IEnumerable Matches(SqlRegex regex, [SqlFacet(MaxSize = -1)] SqlString input)
    {
      if (regex == null)
        throw new ArgumentNullException("regex");

      if (regex.IsNull || input.IsNull)
        yield break;

      MatchCollection matches = regex.Regex.Matches(input.Value);
      for (int i = 0; i < matches.Count; i++)
      {
        Match match = matches[i];
        for (int j = 0; j < match.Groups.Count; j++)
        {
          Group group = match.Groups[j];
          if (!group.Success)
            yield return new MatchRow(i, j, -1, group.Success, regex.Regex.GroupNameFromNumber(j), group.Index, group.Length, group.Value);
          else
            for (int k = 0; k < group.Captures.Count; k++)
            {
              Capture capture = group.Captures[k];
              yield return new MatchRow(i, j, k, group.Success, regex.Regex.GroupNameFromNumber(j), capture.Index, capture.Length, capture.Value);
            }
        }
      }
    }

    [SqlFunction(Name = "reMatchesAt", IsDeterministic = true, FillRowMethodName = "MatchesFillRow")]
    public static IEnumerable MatchesAt(SqlRegex regex, [SqlFacet(MaxSize = -1)] SqlString input, [DefaultValue("NULL")] SqlInt32 startat)
    {
      if (regex == null)
        throw new ArgumentNullException("regex");

      if (regex.IsNull || input.IsNull)
        yield break;

      MatchCollection matches = regex.Regex.Matches(input.Value, !startat.IsNull ? startat.Value : 0);
      for (int i = 0; i < matches.Count; i++)
      {
        Match match = matches[i];
        for (int j = 0; j < match.Groups.Count; j++)
        {
          Group group = match.Groups[j];
          if (!group.Success)
            yield return new MatchRow(i, j, -1, group.Success, regex.Regex.GroupNameFromNumber(j), group.Index, group.Length, group.Value);
          else
            for (int k = 0; k < group.Captures.Count; k++)
            {
              Capture capture = group.Captures[k];
              yield return new MatchRow(i, j, k, group.Success, regex.Regex.GroupNameFromNumber(j), capture.Index, capture.Length, capture.Value);
            }
        }
      }
    }

    [SqlFunction(Name = "reSplit", IsDeterministic = true, FillRowMethodName = "SplitFillRow")]
    public static IEnumerable Split(SqlRegex regex, [SqlFacet(MaxSize = -1)] SqlString input)
    {
      if (regex == null)
        throw new ArgumentNullException("regex");

      if (regex.IsNull || input.IsNull)
        yield break;

      var results = regex.Regex.Split(input.Value);
      for (int i = 0; i < results.Length; i++)
        yield return results[i];
    }

    [SqlFunction(Name = "reSplitLim", IsDeterministic = true, FillRowMethodName = "SplitFillRow")]
    public static IEnumerable SplitLim(SqlRegex regex, [SqlFacet(MaxSize = -1)] SqlString input, [DefaultValue("NULL")] SqlInt32 count)
    {
      if (regex == null)
        throw new ArgumentNullException("regex");

      if (regex.IsNull || input.IsNull)
        yield break;

      var results = regex.Regex.Split(input.Value, !count.IsNull ? count.Value : int.MaxValue);
      for (int i = 0; i < results.Length; i++)
        yield return results[i];
    }

    [SqlFunction(Name = "reSplitLimAt", IsDeterministic = true, FillRowMethodName = "SplitFillRow")]
    public static IEnumerable SplitLimAt(SqlRegex regex, [SqlFacet(MaxSize = -1)] SqlString input, [DefaultValue("NULL")] SqlInt32 count, [DefaultValue("NULL")] SqlInt32 startat)
    {
      if (regex == null)
        throw new ArgumentNullException("regex");

      if (regex.IsNull || input.IsNull)
        yield break;

      var results = regex.Regex.Split(input.Value, !count.IsNull ? count.Value : int.MaxValue, !startat.IsNull ? startat.Value : 0);
      for (int i = 0; i < results.Length; i++)
        yield return results[i];
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
