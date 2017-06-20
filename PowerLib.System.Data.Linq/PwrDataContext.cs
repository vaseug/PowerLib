using System;
using System.Linq;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Reflection;
using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace PowerLib.System.Data.Linq
{
	public class PwrDataContext : DataContext
	{
    private const string Schema = "pwrlib.";

		#region Constructors

		public PwrDataContext(IDbConnection connection)
			: base(connection)
		{
		}

		public PwrDataContext(string fileOrServerOrConnection)
			: base(fileOrServerOrConnection)
		{
		}

		public PwrDataContext(IDbConnection connection, MappingSource mapping)
			: base(connection, mapping)
		{
		}

		public PwrDataContext(string fileOrServerOrConnection, MappingSource mapping)
			: base(fileOrServerOrConnection, mapping)
		{
		}

    #endregion
    #region Function methods
    #region Regular expression functions

    /// <summary>
    /// Indicates whether the specified regular expression finds a match in the specified input string, using the specified matching options.
    /// </summary>
    /// <param name="input">The string to search for a match.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
    /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
    [Function(Name = Schema + "regexIsMatch", IsComposable = true)]
		public bool? regexIsMatch(string input, string pattern, RegexOptions options)
		{
      return (bool)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, pattern, options).ReturnValue;
    }

    /// <summary>
    /// In a specified input string, replaces all strings that match a specified regular expression with a specified replacement string. Specified options modify the matching operation.
    /// </summary>
    /// <param name="input">The string to search for a match.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="replacement">The replacement string.</param>
    /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
    /// <returns>A new string that is identical to the input string, except that the replacement string takes the place of each matched string. If pattern is not matched in the current instance, the method returns the current instance unchanged.</returns>
		[Function(Name = Schema + "regexReplace", IsComposable = true)]
		public string regexReplace(string input, string pattern, string replacement, RegexOptions options)
		{
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, pattern, replacement).ReturnValue;
    }

    /// <summary>
    /// Escapes a minimal set of characters (\, *, +, ?, |, {, [, (,), ^, $,., #, and white space) by replacing them with their escape codes. This instructs the regular expression engine to interpret these characters literally rather than as metacharacters.
    /// </summary>
    /// <param name="input">The input string that contains the text to convert.</param>
    /// <returns>A string of characters with metacharacters converted to their escaped form.</returns>
		[Function(Name = Schema + "regexEscape", IsComposable = true)]
		public string regexEscape(string input)
		{
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    /// <summary>
    /// Converts any escaped characters in the input string.
    /// </summary>
    /// <param name="input">The input string containing the text to convert.</param>
    /// <returns>A string of characters with any escaped characters converted to their unescaped form.</returns>
		[Function(Name = Schema + "regexUnescape", IsComposable = true)]
		public string regexUnescape(string input)
		{
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    /// <summary>
    /// Splits an input string into an array of substrings at the positions defined by a specified regular expression pattern. Specified options modify the matching operation.
    /// </summary>
    /// <param name="input">The string to split.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
    /// <returns></returns>
		[Function(Name = Schema + "regexSplit", IsComposable = true)]
		public IQueryable<StringRow> regexSplit(string input, string pattern, RegexOptions options)
		{
			return CreateMethodCallQuery<StringRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, pattern, options);
		}

    /// <summary>
    /// Searches the specified input string for all occurrences of a specified regular expression, using the specified matching options.
    /// </summary>
    /// <param name="input">The string to search for a match.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specify options for matching.</param>
    /// <returns>A collection of the RegexMatchRow objects found by the search. If no matches are found, the method returns an empty collection object.</returns>
		[Function(Name = Schema + "regexMatches", IsComposable = true)]
		public IQueryable<RegexMatchRow> regexMatches(string input, string pattern, RegexOptions options)
		{
			return CreateMethodCallQuery<RegexMatchRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, pattern, options);
    }

    #endregion
    #region String functions
    #region String manipulation functions

    [Function(Name = Schema + "strInsert", IsComposable = true)]
    public string strInsert(string input, int? index, string value)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "strRemove", IsComposable = true)]
    public string strRemove(string input, int? index, int? count)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "strReplace", IsComposable = true)]
    public string strReplace(string input, string pattern, string replacemant)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, pattern, replacemant).ReturnValue;
    }

    [Function(Name = Schema + "strSubstring", IsComposable = true)]
    public string strSubstring(string input, int? index, int? count)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "strReverse", IsComposable = true)]
    public string strReverse(string input)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "strReplicate", IsComposable = true)]
    public string strReplicate(string input, int count)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, count).ReturnValue;
    }

    [Function(Name = Schema + "strPadLeft", IsComposable = true)]
    public string strPadLeft(string input, string padding, int width)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, padding, width).ReturnValue;
    }

    [Function(Name = Schema + "strPadRight", IsComposable = true)]
    public string strPadRight(string input, string padding, int width)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, padding, width).ReturnValue;
    }

    [Function(Name = Schema + "strCutLeft", IsComposable = true)]
    public string strCutLeft(string input, int width)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, width).ReturnValue;
    }

    [Function(Name = Schema + "strCutRight", IsComposable = true)]
    public string strCutRight(string input, int width)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, width).ReturnValue;
    }

    [Function(Name = Schema + "strTrimLeft", IsComposable = true)]
    public string strTrimLeft(string input, string trimming)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, trimming).ReturnValue;
    }

    [Function(Name = Schema + "strTrimRight", IsComposable = true)]
    public string strTrimRight(string input, string trimming)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, trimming).ReturnValue;
    }

    [Function(Name = Schema + "strTrim", IsComposable = true)]
    public string strTrim(string input, string trimming)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, trimming).ReturnValue;
    }

    [Function(Name = Schema + "strQuote", IsComposable = true)]
    public String strQuote(string input, [Parameter(DbType = "nchar(1)")] string quote, [Parameter(DbType = "nchar(1)")] string escape)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, quote, escape).ReturnValue;
    }

    [Function(Name = Schema + "strUnquote", IsComposable = true)]
    public String strUnquote(string input, [Parameter(DbType = "nchar(1)")] string quote, [Parameter(DbType = "nchar(1)")] string escape)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, quote, escape).ReturnValue;
    }

    [Function(Name = Schema + "strEscape", IsComposable = true)]
    public String strEscape(string input, [Parameter(DbType = "nchar(1)")] string escape, string symbols)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, escape, symbols).ReturnValue;
    }

    [Function(Name = Schema + "strUnescape", IsComposable = true)]
    public String strUnescape(string input, [Parameter(DbType = "nchar(1)")] string escape, string symbols)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, escape, symbols).ReturnValue;
    }

    #endregion
    #region String convert functions

    [Function(Name = Schema + "strToLower", IsComposable = true)]
    public string strToLower(string input)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "strToLowerByLcId", IsComposable = true)]
    public string strToLower(string input, int? lcId)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strToLowerByLcName", IsComposable = true)]
    public string strToLower(string input, string lcName)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, lcName).ReturnValue;
    }

    [Function(Name = Schema + "strToUpper", IsComposable = true)]
    public string strToUpper(string input)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "strToUpperByLcId", IsComposable = true)]
    public string strToUpper(string input, int? lcId)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strToUpperByLcName", IsComposable = true)]
    public string strToUpper(string input, string lcName)
    {
      return (string)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, lcName).ReturnValue;
    }

    #endregion
    #region String retrieve functions

    [Function(Name = Schema + "strLength", IsComposable = true)]
    public int? strLength(string input)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "strIndexOf", IsComposable = true)]
    public int? strIndexOf(string input, string value, CompareOptions compareOptions)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions).ReturnValue;
    }

    [Function(Name = Schema + "strIndexOfRange", IsComposable = true)]
    public int? strIndexOf(string input, string value, int? index, int? count, CompareOptions compareOptions)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, index, count, compareOptions).ReturnValue;
    }

    [Function(Name = Schema + "strIndexOfByLcId", IsComposable = true)]
    public int? strIndexOf(string input, string value, CompareOptions compareOptions, int? lcId)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strIndexOfRangeByLcId", IsComposable = true)]
    public int? strIndexOf(string input, string value, int? index, int? count, CompareOptions compareOptions, int? lcId)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, index, count, compareOptions, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strIndexOfByLcName", IsComposable = true)]
    public int? strIndexOf(string input, string value, CompareOptions compareOptions, string lcName)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions, lcName).ReturnValue;
    }

    [Function(Name = Schema + "strIndexOfRangeByLcName", IsComposable = true)]
    public int? strIndexOf(string input, string value, int? index, int? count, CompareOptions compareOptions, string lcName)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, index, count, compareOptions, lcName).ReturnValue;
    }

    [Function(Name = Schema + "strLastIndexOf", IsComposable = true)]
    public int? strLastIndexOf(string input, string value, CompareOptions compareOptions)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions).ReturnValue;
    }

    [Function(Name = Schema + "strLastIndexOfRange", IsComposable = true)]
    public int? strLastIndexOf(string input, string value, int? index, int? count, CompareOptions compareOptions)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, index, count, compareOptions).ReturnValue;
    }

    [Function(Name = Schema + "strLastIndexOfByLcId", IsComposable = true)]
    public int? strLastIndexOf(string input, string value, CompareOptions compareOptions, int? lcId)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strLastIndexOfRangeByLcId", IsComposable = true)]
    public int? strLastIndexOf(string input, string value, int? index, int? count, CompareOptions compareOptions, int? lcId)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, index, count, compareOptions, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strLastIndexOfByLcName", IsComposable = true)]
    public int? strLastIndexOf(string input, string value, CompareOptions compareOptions, string lcName)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions, lcName).ReturnValue;
    }

    [Function(Name = Schema + "strLastIndexOfRangeByLcName", IsComposable = true)]
    public int? strLastIndexOf(string input, string value, int? index, int? count, CompareOptions compareOptions, string lcName)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, index, count, compareOptions, lcName).ReturnValue;
    }

    [Function(Name = Schema + "strContains", IsComposable = true)]
    public bool? strContains(string input, string value, CompareOptions compareOptions)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions).ReturnValue;
    }

    [Function(Name = Schema + "strContainsByLcId", IsComposable = true)]
    public bool? strContains(string input, string value, CompareOptions compareOptions, int? lcId)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strContainsByLcName", IsComposable = true)]
    public bool? strContains(string input, string value, CompareOptions compareOptions, string lcName)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions, lcName).ReturnValue;
    }

    [Function(Name = Schema + "strStartsWith", IsComposable = true)]
    public bool? strStartsWith(string input, string value, CompareOptions compareOptions)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions).ReturnValue;
    }

    [Function(Name = Schema + "strStartsWithByLcId", IsComposable = true)]
    public bool? strStartsWith(string input, string value, CompareOptions compareOptions, int? lcId)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strStartsWithByLcName", IsComposable = true)]
    public bool? strStartsWith(string input, string value, CompareOptions compareOptions, string lcName)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions, lcName).ReturnValue;
    }

    [Function(Name = Schema + "strEndsWith", IsComposable = true)]
    public bool? strEndsWith(string input, string value, CompareOptions compareOptions)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions).ReturnValue;
    }

    [Function(Name = Schema + "strEndsWithByLcId", IsComposable = true)]
    public bool? strEndsWith(string input, string value, CompareOptions compareOptions, int? lcId)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strEndsWithByLcName", IsComposable = true)]
    public bool? strEndsWith(string input, string value, CompareOptions compareOptions, string lcName)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, compareOptions, lcName).ReturnValue;
    }

    [Function(Name = Schema + "strIndexOfAny", IsComposable = true)]
    public int? strIndexOfAny(string input, string anyOf)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, anyOf).ReturnValue;
    }

    [Function(Name = Schema + "strIndexOfAnyRange", IsComposable = true)]
    public int? strIndexOfAny(string input, string anyOf, int? index, int? count)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, anyOf, index, count).ReturnValue;
    }

    [Function(Name = Schema + "strLastIndexOfAny", IsComposable = true)]
    public int? strLastIndexOfAny(string input, string anyOf)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, anyOf).ReturnValue;
    }

    [Function(Name = Schema + "strLastIndexOfAnyRange", IsComposable = true)]
    public int? strLastIndexOfAny(string input, string anyOf, int? index, int? count)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, anyOf, index, count).ReturnValue;
    }

    [Function(Name = Schema + "strIndexOfExcept", IsComposable = true)]
    public int? strIndexOfExcept(string input, string exceptOf)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, exceptOf).ReturnValue;
    }

    [Function(Name = Schema + "strIndexOfExceptRange", IsComposable = true)]
    public int? strIndexOfExcept(string input, string exceptOf, int? index, int? count)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, exceptOf, index, count).ReturnValue;
    }

    [Function(Name = Schema + "strLastIndexOfExcept", IsComposable = true)]
    public int? strLastIndexOfExcept(string input, string exceptOf)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, exceptOf).ReturnValue;
    }

    [Function(Name = Schema + "strLastIndexOfExceptRange", IsComposable = true)]
    public int? strLastIndexOfExcept(string input, string exceptOf, int? index, int? count)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, exceptOf, index, count).ReturnValue;
    }

    #endregion
    #region String comparison functions

    [Function(Name = Schema + "strCompare", IsComposable = true)]
    public int? strCompare(string value1, string value2, CompareOptions compareOptions)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), value1, value2, compareOptions).ReturnValue;
    }

    [Function(Name = Schema + "strCompareRange", IsComposable = true)]
    public int? strCompare(string value1, int? index1, int? count1, string value2, int? index2, int? count2, CompareOptions compareOptions)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions).ReturnValue;
    }

    [Function(Name = Schema + "strCompareByLcId", IsComposable = true)]
    public int? strCompare(string value1, string value2, CompareOptions compareOptions, int? lcId)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), value1, value2, compareOptions, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strCompareRangeByLcId", IsComposable = true)]
    public int? strCompare(string value1, int? index1, int? count1, string value2, int? index2, int? count2, CompareOptions compareOptions, int? lcId)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strCompareByLcName", IsComposable = true)]
    public int? strCompare(string value1, string value2, CompareOptions compareOptions, string lcName)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), value1, value2, compareOptions, lcName).ReturnValue;
    }

    [Function(Name = Schema + "strCompareRangeByLcName", IsComposable = true)]
    public int? strCompare(string value1, int? index1, int? count1, string value2, int? index2, int? count2, CompareOptions compareOptions, string lcName)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions, lcName).ReturnValue;
    }

    [Function(Name = Schema + "strEqual", IsComposable = true)]
    public bool? strEqual(string value1, string value2, CompareOptions compareOptions)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), value1, value2, compareOptions).ReturnValue;
    }

    [Function(Name = Schema + "strEqualRange", IsComposable = true)]
    public bool? strEqual(string value1, int? index1, int? count1, string value2, int? index2, int? count2, CompareOptions compareOptions)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions).ReturnValue;
    }

    [Function(Name = Schema + "strEqualByLcId", IsComposable = true)]
    public bool? strEqual(string value1, string value2, CompareOptions compareOptions, int? lcId)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), value1, value2, compareOptions, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strEqualRangeByLcId", IsComposable = true)]
    public bool? strEqual(string value1, int? index1, int? count1, string value2, int? index2, int? count2, CompareOptions compareOptions, int? lcId)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions, lcId).ReturnValue;
    }

    [Function(Name = Schema + "strEqualByLcName", IsComposable = true)]
    public bool? strEqual(string value1, string value2, CompareOptions compareOptions, string lcName)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), value1, value2, compareOptions, lcName).ReturnValue;
    }

    [Function(Name = Schema + "strEqualRangeByLcName", IsComposable = true)]
    public bool? strEqual(string value1, int? index1, int? count1, string value2, int? index2, int? count2, CompareOptions compareOptions, string lcName)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions, lcName).ReturnValue;
    }

    #endregion
    #region String split functions

    [Function(Name = Schema + "strSplitToChars", IsComposable = true)]
    public IQueryable<StringRow> strSplitToChars(string input, int? index, int? count)
    {
      return CreateMethodCallQuery<StringRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [Function(Name = Schema + "strSplitByChars", IsComposable = true)]
    public IQueryable<StringRow> strSplitByChars(string input, string delimitChars, int? count, StringSplitOptions options)
    {
      return CreateMethodCallQuery<StringRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, delimitChars, count, options);
    }

    [Function(Name = Schema + "strSplitByWords", IsComposable = true)]
    public IQueryable<StringRow> strSplitByWords(string input, string separators, string delimitChars, int? count, StringSplitOptions options)
    {
      return CreateMethodCallQuery<StringRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, separators, delimitChars, count, options);
    }

    [Function(Name = Schema + "strSplit", IsComposable = true)]
    public IQueryable<StringRow> strSplit(string input, string separator, int? count, StringSplitOptions options)
    {
      return CreateMethodCallQuery<StringRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, separator, count, options);
    }

    [Function(Name = Schema + "strSplitSmart", IsComposable = true)]
    public IQueryable<StringRow> strSplit(string input, string separators, string trims, string controlSeparators, string controlEscapes, string controls, int? count, StringSplitOptionsEx options)
    {
      return CreateMethodCallQuery<StringRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, separators, trims, controlSeparators, controlEscapes, controls, count, options);
    }

    #endregion
    #region String aggregate functions

    [Function(Name = Schema + "strConcat", IsComposable = true)]
    public string strConcat(string value)
    {
      throw new NotSupportedException();
    }

    [Function(Name = Schema + "strJoin", IsComposable = true)]
    public string strJoin(string value, string delimiter)
    {
      throw new NotSupportedException();
    }

    #endregion
    #endregion
    #region Binary functions
    #region Binary manipulation functions

    [Function(Name = Schema + "binInsert", IsComposable = true)]
    public Byte[] binInsert(Byte[] input, Int64? index, Byte[] value)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "binRemove", IsComposable = true)]
    public Byte[] binRemove(Byte[] input, Int64? index, Int64? count)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "binReplicate", IsComposable = true)]
    public Byte[] binReplicate(Byte[] input, Int64 count)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, count).ReturnValue;
    }

    [Function(Name = Schema + "binRange", IsComposable = true)]
    public Byte[] binRange(Byte[] input, Int64? index, Int64? count)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "binReverse", IsComposable = true)]
    public Byte[] binReverse(Byte[] input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binReverseRange", IsComposable = true)]
    public Byte[] binReverse(Byte[] input, Int64? index, Int64? count)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "binReplace", IsComposable = true)]
    public Byte[] binReplace(Byte[] input, Byte[] value, Byte[] replacement)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, replacement).ReturnValue;
    }

    [Function(Name = Schema + "binReplaceRange", IsComposable = true)]
    public Byte[] binReplace(Byte[] input, Byte[] value, Byte[] replacement, Int64? index, Int64? count)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, replacement, index, count).ReturnValue;
    }

    #endregion
    #region Binary convert functions

    [Function(Name = Schema + "binToString", IsComposable = true)]
    public String binToString(Byte[] input, Int64? index, Int64? count)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "binToStringByCpId", IsComposable = true)]
    public String binToString(Byte[] input, Int64? index, Int64? count, int? cpId)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count, cpId).ReturnValue;
    }

    [Function(Name = Schema + "binToStringByCpName", IsComposable = true)]
    public String binToString(Byte[] input, Int64? index, Int64? count, string cpName)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count, cpName).ReturnValue;
    }

    [Function(Name = Schema + "binToBase64String", IsComposable = true)]
    public String binToBase64String(Byte[] input, Int64? index, Int64? count)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "binToTinyInt", IsComposable = true)]
    public Byte? binToByte(Byte[] input, Int64? index)
    {
      return (Byte?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "binToSmallInt", IsComposable = true)]
    public Int16? binToInt16(Byte[] input, Int64? index)
    {
      return (Int16?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "binToInt", IsComposable = true)]
    public Int32? binToInt32(Byte[] input, Int64? index)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "binToBigInt", IsComposable = true)]
    public Int64? binToInt64(Byte[] input, Int64? index)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "binToSingle", IsComposable = true)]
    public Single? binToSingle(Byte[] input, Int64? index)
    {
      return (Single?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "binToDouble", IsComposable = true)]
    public Double? binToDouble(Byte[] input, Int64? index)
    {
      return (Double?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "binToDateTime", IsComposable = true)]
    public DateTime? binToDateTime(Byte[] input, Int64? index)
    {
      return (DateTime?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "binToUid", IsComposable = true)]
    public Guid? binToGuid(Byte[] input, Int64? index)
    {
      return (Guid?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "binFromString", IsComposable = true)]
    public Byte[] binFromString(String input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binFromStringByCpId", IsComposable = true)]
    public Byte[] binFromString(String input, int? cpId)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, cpId).ReturnValue;
    }

    [Function(Name = Schema + "binFromStringByCpName", IsComposable = true)]
    public Byte[] binFromString(String input, string cpName)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, cpName).ReturnValue;
    }

    [Function(Name = Schema + "binFromBase64String", IsComposable = true)]
    public Byte[] binFromBase64String(String input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binFromTinyInt", IsComposable = true)]
    public Byte[] binFromByte(Byte? input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binFromSmallInt", IsComposable = true)]
    public Byte[] binFromInt16(Int16? input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binFromInt", IsComposable = true)]
    public Byte[] binFromInt32(Int32? input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binFromBigInt", IsComposable = true)]
    public Byte[] binFromInt64(Int64? input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binFromSingleFloat", IsComposable = true)]
    public Byte[] binFromSingle(Single? input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binFromDoubleFloat", IsComposable = true)]
    public Byte[] binFromDouble(Double? input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binFromDateTime", IsComposable = true)]
    public Byte[] binFromDateTime(DateTime? input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binFromUid", IsComposable = true)]
    public Byte[] binFromGuid(Guid? input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    #endregion
    #region Binary retrieve functions

    [Function(Name = Schema + "binLength", IsComposable = true)]
    public Int64? binLength(Byte[] input)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binIndexOf", IsComposable = true)]
    public Int64? binIndexOf(Byte[] input, Byte[] value)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "binIndexOfRange", IsComposable = true)]
    public Int64? binIndexOf(Byte[] input, Byte[] value, Int64? index, Int64? count)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, index, count).ReturnValue;
    }

    [Function(Name = Schema + "binLastIndexOf", IsComposable = true)]
    public Int64? binLastIndexOf(Byte[] input, Byte[] value)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "binLastIndexOfRange", IsComposable = true)]
    public Int64? binLastIndexOf(Byte[] input, Byte[] value, Int64? index, Int64? count)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value, index, count).ReturnValue;
    }

    #endregion
    #region Binary comparison functions

    [Function(Name = Schema + "binCompare", IsComposable = true)]
    public Int64? binCompare(Byte[] xValue, Byte[] yValue)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), xValue, yValue).ReturnValue;
    }

    [Function(Name = Schema + "binCompareRange", IsComposable = true)]
    public Int64? binCompare(Byte[] xValue, Int64? xIndex, Byte[] yValue, Int64? yIndex, Int64? count)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), xValue, xIndex, yValue, yIndex, count).ReturnValue;
    }

    [Function(Name = Schema + "binEqual", IsComposable = true)]
    public bool? binEqual(Byte[] xValue, Byte[] yValue)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), xValue, yValue).ReturnValue;
    }

    [Function(Name = Schema + "binEqualRange", IsComposable = true)]
    public bool? binEqual(Byte[] xValue, Int64? xIndex, Byte[] yValue, Int64? yIndex, Int64? count)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), xValue, xIndex, yValue, yIndex, count).ReturnValue;
    }

    #endregion
    #region Binary split functions

    [Function(Name = Schema + "binSplitToBit", IsComposable = true)]
    public IQueryable<BooleanRow> binSplitToBoolean(Byte[] input)
    {
      return CreateMethodCallQuery<BooleanRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input);
    }

    [Function(Name = Schema + "binSplitToBinary", IsComposable = true)]
    public IQueryable<BinaryRow> binSplitToBinary(Byte[] input)
    {
      return CreateMethodCallQuery<BinaryRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input);
    }

    [Function(Name = Schema + "binSplitToString", IsComposable = true)]
    public IQueryable<StringRow> binSplitToString(Byte[] input)
    {
      return CreateMethodCallQuery<StringRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input);
    }

    [Function(Name = Schema + "binSplitToTinyInt", IsComposable = true)]
    public IQueryable<ByteRow> binSplitToByte(Byte[] input)
    {
      return CreateMethodCallQuery<ByteRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input);
    }

    [Function(Name = Schema + "binSplitToSmallInt", IsComposable = true)]
    public IQueryable<Int16Row> binSplitToInt16(Byte[] input)
    {
      return CreateMethodCallQuery<Int16Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input);
    }

    [Function(Name = Schema + "binSplitToInt", IsComposable = true)]
    public IQueryable<Int32Row> binSplitToInt32(Byte[] input)
    {
      return CreateMethodCallQuery<Int32Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input);
    }

    [Function(Name = Schema + "binSplitToBigInt", IsComposable = true)]
    public IQueryable<Int64Row> binSplitToInt64(Byte[] input)
    {
      return CreateMethodCallQuery<Int64Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input);
    }

    [Function(Name = Schema + "binSplitToSingle", IsComposable = true)]
    public IQueryable<SingleRow> binSplitToSingle(Byte[] input)
    {
      return CreateMethodCallQuery<SingleRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input);
    }

    [Function(Name = Schema + "binSplitToDouble", IsComposable = true)]
    public IQueryable<DoubleRow> binSplitToDouble(Byte[] input)
    {
      return CreateMethodCallQuery<DoubleRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input);
    }

    [Function(Name = Schema + "binSplitToDateTime", IsComposable = true)]
    public IQueryable<DateTimeRow> binSplitToDateTime(Byte[] input)
    {
      return CreateMethodCallQuery<DateTimeRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input);
    }

    [Function(Name = Schema + "binSplitToUid", IsComposable = true)]
    public IQueryable<GuidRow> binSplitToGuid(Byte[] input)
    {
      return CreateMethodCallQuery<GuidRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input);
    }

    #endregion
    #region Binary aggregate functions

    [Function(Name = Schema + "binConcat", IsComposable = true)]
    public Byte[] binConcat(Byte[] value)
    {
      throw new NotSupportedException();
    }

    #endregion
    #endregion
    #region Xml functions
    #region Scalar functions

    /// <summary>
    /// Evaluates the xpath expression and returns Xml element.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Xml element.</returns>
    [Function(Name = Schema + "xmlEvaluate", IsComposable = true)]
    public XElement xmlEvaluate(XElement input, String xpath, String nsmap)
    {
      return (XElement)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap).ReturnValue;
    }

    /// <summary>
    /// Evaluates the xpath expression and returns String value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as String.</returns>
    [Function(Name = Schema + "xmlEvaluateAsString", IsComposable = true)]
    public String xmlEvaluateAsString(XElement input, String xpath, String nsmap)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap).ReturnValue;
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Boolean value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Boolean.</returns>
    [Function(Name = Schema + "xmlEvaluateAsBit", IsComposable = true)]
    public Boolean? xmlEvaluateAsBoolean(XElement input, String xpath, String nsmap)
    {
      return (Boolean?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap).ReturnValue;
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Byte value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Byte.</returns>
    [Function(Name = Schema + "xmlEvaluateAsTinyInt", IsComposable = true)]
    public Byte? xmlEvaluateAsByte(XElement input, String xpath, String nsmap)
    {
      return (Byte?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap).ReturnValue;
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Int16 value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Int16.</returns>
    [Function(Name = Schema + "xmlEvaluateAsSmallInt", IsComposable = true)]
    public Int16? xmlEvaluateAsInt16(XElement input, String xpath, String nsmap)
    {
      return (Int16?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap).ReturnValue;
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Int32 value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="path">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsMap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Int32.</returns>
    [Function(Name = Schema + "xmlEvaluateAsInt", IsComposable = true)]
    public Int32? xmlEvaluateAsInt32(XElement input, String path, String nsMap)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, path, nsMap).ReturnValue;
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Int64 value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Int64.</returns>
    [Function(Name = Schema + "xmlEvaluateAsBigInt", IsComposable = true)]
    public Int64? xmlEvaluateAsInt64(XElement input, String xpath, String nsmap)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap).ReturnValue;
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Single value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Single.</returns>
    [Function(Name = Schema + "xmlEvaluateAsSingle", IsComposable = true)]
    public Single? xmlEvaluateAsSingle(XElement input, String xpath, String nsmap)
    {
      return (Single?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap).ReturnValue;
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Double value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Double.</returns>
    [Function(Name = Schema + "xmlEvaluateAsDouble", IsComposable = true)]
    public Double? xmlEvaluateAsDouble(XElement input, String xpath, String nsmap)
    {
      return (Double?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap).ReturnValue;
    }

    /// <summary>
    /// Evaluates the xpath expression and returns DateTime value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as DateTime.</returns>
    [Function(Name = Schema + "xmlEvaluateAsDateTime", IsComposable = true)]
    public DateTime? xmlEvaluateAsDateTime(XElement input, String xpath, String nsmap)
    {
      return (DateTime?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap).ReturnValue;
    }

    /// <summary>
    /// Transforms XML data using an XSLT style sheet.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="stylesheet">Xslt style sheet.</param>
    /// <returns>Output xml document.</returns>
    [Function(Name = Schema + "xmlTransform", IsComposable = true)]
    public XElement xmlTransform(XElement input, XElement stylesheet)
    {
      return (XElement)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, stylesheet).ReturnValue;
    }

    /// <summary>
    /// Convert XML data to JSON.
    /// </summary>
    /// <param name="input">Input XML data.</param>
    /// <returns>Output JSON data.</returns>
    [Function(Name = Schema + "xmlToJson", IsComposable = true)]
    public String xmlToJson(XElement input)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    /// <summary>
    /// Convert JSON data to XML.
    /// </summary>
    /// <param name="input">Input JSON data.</param>
    /// <returns>Output XML data.</returns>
    [Function(Name = Schema + "xmlFromJson", IsComposable = true)]
    public XElement xmlFromJson(String input)
    {
      return (XElement)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    #endregion
    #region Table valued functions

    /// <summary>
    /// Selects xml elements, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of xml elements.</returns>
    [Function(Name = Schema + "xmlSelect", IsComposable = true)]
    public IQueryable<XmlRow> xmlSelect(XElement input, String xpath, String nsmap)
    {
      return CreateMethodCallQuery<XmlRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects String values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of String.</returns>
    [Function(Name = Schema + "xmlSelectAsString", IsComposable = true)]
    public IQueryable<StringRow> xmlSelectAsString(XElement input, String xpath, String nsmap)
    {
      return CreateMethodCallQuery<StringRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Boolean values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Boolean.</returns>
    [Function(Name = Schema + "xmlSelectAsBit", IsComposable = true)]
    public IQueryable<BooleanRow> xmlSelectAsBoolean(XElement input, String xpath, String nsmap)
    {
      return CreateMethodCallQuery<BooleanRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Byte values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Byte.</returns>
    [Function(Name = Schema + "xmlSelectAsTinyInt", IsComposable = true)]
    public IQueryable<ByteRow> xmlSelectAsByte(XElement input, String xpath, String nsmap)
    {
      return CreateMethodCallQuery<ByteRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Int16 values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="path">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsMap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Int16.</returns>
    [Function(Name = Schema + "xmlSelectAsSmallInt", IsComposable = true)]
    public IQueryable<Int16Row> xmlSelectAsInt16(XElement input, String path, String nsMap)
    {
      return CreateMethodCallQuery<Int16Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, path, nsMap);
    }

    /// <summary>
    /// Selects Int32 values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Int32.</returns>
    [Function(Name = Schema + "xmlSelectAsInt", IsComposable = true)]
    public IQueryable<Int32Row> xmlSelectAsInt32(XElement input, String xpath, String nsmap)
    {
      return CreateMethodCallQuery<Int32Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Int64 values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Int64.</returns>
    [Function(Name = Schema + "xmlSelectAsBigInt", IsComposable = true)]
    public IQueryable<Int64Row> xmlSelectAsInt64(XElement input, String xpath, String nsmap)
    {
      return CreateMethodCallQuery<Int64Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Single values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Single.</returns>
    [Function(Name = Schema + "xmlSelectAsSingle", IsComposable = true)]
    public IQueryable<SingleRow> xmlSelectAsSingle(XElement input, String xpath, String nsmap)
    {
      return CreateMethodCallQuery<SingleRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Double values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Double.</returns>
    [Function(Name = Schema + "xmlSelectAsDouble", IsComposable = true)]
    public IQueryable<DoubleRow> xmlSelectAsDouble(XElement input, String xpath, String nsmap)
    {
      return CreateMethodCallQuery<DoubleRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects DateTime values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of DateTime.</returns>
    [Function(Name = Schema + "xmlSelectAsDateTime", IsComposable = true)]
    public IQueryable<DateTimeRow> xmlSelectAsDateTime(XElement input, String xpath, String nsmap)
    {
      return CreateMethodCallQuery<DateTimeRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, xpath, nsmap);
    }

    #endregion
    #endregion
    #region Compression functions

    /// <summary>
    /// Compress input data by Deflate algorithm.
    /// </summary>
    /// <param name="input">Input data to compress.</param>
    /// <param name="compressionLevel">One of the enumeration values that indicates whether to emphasize speed or compression effectiveness when creating the entry.</param>
    /// <returns>Compressed data.</returns>
    [Function(Name = Schema + "comprDeflateCompress", IsComposable = true)]
    public Byte[] comprDeflateCompress(Byte[] input, CompressionLevel compressionLevel)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, compressionLevel).ReturnValue;
    }

    /// <summary>
    /// Decompress input data compressed by Deflate algorithm.
    /// </summary>
    /// <param name="input">Compressed data to decompress.</param>
    /// <returns>Decompressed data.</returns>
    [Function(Name = Schema + "comprDeflateDecompress", IsComposable = true)]
    public Byte[] comprDeflateDecompress(Byte[] input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    /// <summary>
    /// Compress input data by GZip algorithm.
    /// </summary>
    /// <param name="input">Input data to compress.</param>
    /// <param name="compressionLevel">One of the enumeration values that indicates whether to emphasize speed or compression effectiveness when creating the entry.</param>
    /// <returns>Compressed data.</returns>
    [Function(Name = Schema + "comprGZipCompress", IsComposable = true)]
    public Byte[] comprGZipCompress(Byte[] input, CompressionLevel compressionLevel)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, compressionLevel).ReturnValue;
    }

    /// <summary>
    /// Decompress input data compressed by GZip algorithm.
    /// </summary>
    /// <param name="input">Compressed data to decompress.</param>
    /// <returns>Decompressed data.</returns>
    [Function(Name = Schema + "comprGZipDecompress", IsComposable = true)]
    public Byte[] comprGZipDecompress(Byte[] input)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    /// <summary>
    /// Gets the collection of entry names that are currently in the zip archive.
    /// </summary>
    /// <param name="input">Zip archive data.</param>
    /// <returns>collection of entry names that are currently in the zip archive.</returns>
    [Function(Name = Schema + "zipArchiveGetEntries", IsComposable = true)]
    public IQueryable<ZipArchiveEntryRow> zipArchiveGetEntries(Byte[] input)
    {
      return CreateMethodCallQuery<ZipArchiveEntryRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Gets the entry data that are currently in the zip archive.
    /// </summary>
    /// <param name="input">Zip archive data.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that specifies the name of the entry to be created.</param>
    /// <returns>Zip archive entry data.</returns>
    [Function(Name = Schema + "zipArchiveGetEntry", IsComposable = true)]
    public Byte[] zipArchiveGetEntry(Byte[] input, string entryName)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, entryName).ReturnValue;
    }

    /// <summary>
    /// Add entry data with the specified path and entry name in the zip archive.
    /// </summary>
    /// <param name="input">Zip archive data.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that specifies the name of the entry to be created.</param>
    /// <param name="entryData">Entry data.</param>
    /// <param name="compressionLevel">One of the enumeration values that indicates whether to emphasize speed or compression effectiveness when creating the entry.</param>
    /// <returns>Zip archive data after add entry operation.</returns>
    [Function(Name = Schema + "zipArchiveAddEntry", IsComposable = true)]
    public Byte[] zipArchiveAddEntry(Byte[] input, string entryName, Byte[] entryData, CompressionLevel compressionLevel)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, entryName, entryData, compressionLevel).ReturnValue;
    }

    /// <summary>
    /// Deletes the entry from the zip archive.
    /// </summary>
    /// <param name="input">Zip archive data.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that specifies the name of the entry in archive.</param>
    /// <returns>Zip archive data after delete entry operation.</returns>
    [Function(Name = Schema + "zipArchiveDeleteEntry", IsComposable = true)]
    public Byte[] zipArchiveDeleteEntry(Byte[] input, string entryName)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, entryName).ReturnValue;
    }

    #endregion
    #region Cryptography functions

    /// <summary>
    /// Generate cryptographically strong sequence of random values.
    /// </summary>
    /// <param name="count">Generated sequence length.</param>
    /// <returns>Bytes array with a cryptographically strong sequence of random values.</returns>
    [Function(Name = Schema + "cryptRandom", IsComposable = true)]
    public Byte[] cryptRandom(int count)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), count).ReturnValue;
    }

    /// <summary>
    /// Generate cryptographically strong sequence of random nonzero values.
    /// </summary>
    /// <param name="count">Generated sequence length.</param>
    /// <returns>Bytes array with a cryptographically strong sequence of random nonzero values.</returns>
    [Function(Name = Schema + "cryptNonZeroRandom", IsComposable = true)]
    public Byte[] cryptNonZeroRandom(int count)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), count).ReturnValue;
    }

    /// <summary>
    /// Computes the hash value for the specified data.
    /// </summary>
    /// <param name="input">The input data to compute the hash code for.</param>
    /// <param name="algorithmName">The hash algorithm implementation to use.</param>
    /// <returns>The computed hash value.</returns>
    [Function(Name = Schema + "cryptComputeHash", IsComposable = true)]
    public Byte[] cryptComputeHash(Byte[] input, string algorithmName)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, algorithmName).ReturnValue;
    }

    /// <summary>
    /// Verifies the hash value for the specified data.
    /// </summary>
    /// <param name="input">The input data to compute the hash code for.</param>
    /// <param name="hash">The hash value to verify.</param>
    /// <param name="algorithmName">The hash algorithm implementation to use.</param>
    /// <returns>Hash verification result.</returns>
    [Function(Name = Schema + "cryptVerifyHash", IsComposable = true)]
    public bool? cryptVerifyHash(Byte[] input, Byte[] hash, string algorithmName)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, hash, algorithmName).ReturnValue;
    }

    /// <summary>
    /// Computes the hash value for the specified key and data.
    /// </summary>
    /// <param name="input">The input data to compute the hash code for.</param>
    /// <param name="key">The key to use in the hash algorithm.</param>
    /// <param name="algorithmName">The keyed hash algorithm implementation to use.</param>
    /// <returns>The computed hash value.</returns>
    [Function(Name = Schema + "cryptComputeKeyedHash", IsComposable = true)]
    public Byte[] cryptComputeKeyedHash(Byte[] input, Byte[] key, string algorithmName)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, key, algorithmName).ReturnValue;
    }

    /// <summary>
    /// Verifies the hash value for the specified key and data.
    /// </summary>
    /// <param name="input">The input data to compute the hash code for.</param>
    /// <param name="key">The key to use in the hash algorithm.</param>
    /// <param name="hash">The hash value to verify.</param>
    /// <param name="algorithmName">The keyed hash algorithm implementation to use.</param>
    /// <returns>Hash verification result.</returns>
    [Function(Name = Schema + "cryptVerifyKeyedHash", IsComposable = true)]
    public bool? cryptVerifyKeyedHash(Byte[] input, Byte[] key, Byte[] hash, string algorithmName)
    {
      return (bool?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, key, hash, algorithmName).ReturnValue;
    }

    /// <summary>
    /// Encrypt data by symmetric cryptographyc algorithm.
    /// </summary>
    /// <param name="input">Input data to encrypt.</param>
    /// <param name="key">Secret key for the symmetric algorithm.</param>
    /// <param name="iv">Initialization vector (IV) for the symmetric algorithm.</param>
    /// <param name="algorithmName">The name of the specific implementation of the symmetric cryptographic algorithm to use.</param>
    /// <param name="mode">Mode for operation of the symmetric algorithm.</param>
    /// <param name="padding">Padding mode used in the symmetric algorithm.</param>
    /// <returns>Encrypted data.</returns>
    [Function(Name = Schema + "cryptEncryptSymmetric", IsComposable = true)]
    public Byte[] cryptEncryptSymmetric(Byte[] input, Byte[] key, Byte[] iv, string algorithmName, CipherMode mode, PaddingMode padding)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, key, iv, algorithmName, mode, padding).ReturnValue;
    }

    /// <summary>
    /// Decrypt data by symmetric cryptographyc algorithm.
    /// </summary>
    /// <param name="input">Encrypted data to decrypt.</param>
    /// <param name="key">Secret key for the symmetric algorithm.</param>
    /// <param name="iv">Initialization vector (IV) for the symmetric algorithm.</param>
    /// <param name="algorithmName">The name of the specific implementation of the symmetric cryptographic algorithm to use.</param>
    /// <param name="mode">Mode for operation of the symmetric algorithm.</param>
    /// <param name="padding">Padding mode used in the symmetric algorithm.</param>
    /// <returns>Decrypted data.</returns>
    [Function(Name = Schema + "cryptDecryptSymmetric", IsComposable = true)]
    public Byte[] cryptDecryptSymmetric(Byte[] input, Byte[] key, Byte[] iv, string algorithmName, CipherMode mode, PaddingMode padding)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, key, iv, algorithmName, mode, padding).ReturnValue;
    }

    #endregion
    #region Collect functions

    [Function(Name = Schema + "bCollect", IsComposable = true)]
    public Byte[] boolCollect(Boolean? value)
    {
      throw new NotSupportedException();
    }

    [Function(Name = Schema + "tiCollect", IsComposable = true)]
    public Byte[] byteCollect(Byte? value)
    {
      throw new NotSupportedException();
    }

    [Function(Name = Schema + "siCollect", IsComposable = true)]
    public Byte[] int16Collect(Int16? value)
    {
      throw new NotSupportedException();
    }

    [Function(Name = Schema + "iCollect", IsComposable = true)]
    public Byte[] int32Collect(Int32? value)
    {
      throw new NotSupportedException();
    }

    [Function(Name = Schema + "biCollect", IsComposable = true)]
    public Byte[] int64Collect(Int64? value)
    {
      throw new NotSupportedException();
    }

    [Function(Name = Schema + "sfCollect", IsComposable = true)]
    public Byte[] sglCollect(Single? value)
    {
      throw new NotSupportedException();
    }

    [Function(Name = Schema + "dfCollect", IsComposable = true)]
    public Byte[] dblCollect(Double? value)
    {
      throw new NotSupportedException();
    }

    [Function(Name = Schema + "dtCollect", IsComposable = true)]
    public Byte[] dtmCollect(DateTime? value)
    {
      throw new NotSupportedException();
    }

    [Function(Name = Schema + "uidCollect", IsComposable = true)]
    public Byte[] guidCollect(Guid? value)
    {
      throw new NotSupportedException();
    }

    [Function(Name = Schema + "binCollect", IsComposable = true)]
    public Byte[] binCollect(Byte[] value)
    {
      throw new NotSupportedException();
    }

    [Function(Name = Schema + "strCollect", IsComposable = true)]
    public Byte[] strCollect(String value)
    {
      throw new NotSupportedException();
    }

    #endregion
    #region Collection functions
    #region Boolean collection

    [Function(Name = Schema + "bCollCreate", IsComposable = true)]
    public byte[] boolCollCreate(SizeEncoding countSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing).ReturnValue;
    }

    [Function(Name = Schema + "bCollParse", IsComposable = true)]
    public byte[] boolCollParse(String input, SizeEncoding countSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing).ReturnValue;
    }

    [Function(Name = Schema + "bCollFormat", IsComposable = true)]
    public byte[] boolCollFormat(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "bCollCount", IsComposable = true)]
    public int? boolCollCount(byte[] input)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "bCollIndexOf", IsComposable = true)]
    public int? boolCollIndexOf(byte[] input, Boolean? value)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "bCollGet", IsComposable = true)]
    public Boolean? boolCollGet(byte[] input, int index)
    {
      return (Boolean?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "bCollSet", IsComposable = true)]
    public byte[] boolCollSet(byte[] input, int index, Boolean? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "bCollInsert", IsComposable = true)]
    public byte[] boolCollInsert(byte[] input, int? index, Boolean? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "bCollRemove", IsComposable = true)]
    public byte[] boolCollRemove(byte[] input, Boolean? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "bCollRemoveAt", IsComposable = true)]
    public byte[] boolCollRemoveAt(byte[] input, int index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "bCollClear", IsComposable = true)]
    public byte[] boolCollClear(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "bCollGetRange", IsComposable = true)]
    public byte[] boolCollGetRange(byte[] input, int? index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "bCollSetRange", IsComposable = true)]
    public byte[] boolCollSetRange(byte[] input, int index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "bCollSetRepeat", IsComposable = true)]
    public byte[] boolCollSetRepeat(byte[] input, int index, Boolean? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "bCollInsertRange", IsComposable = true)]
    public byte[] boolCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "bCollInsertRepeat", IsComposable = true)]
    public byte[] boolCollInsertRepeat(byte[] input, int? index, Boolean? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "bCollRemoveRange", IsComposable = true)]
    public byte[] boolCollRemoveRange(byte[] input, int index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "bCollToArray", IsComposable = true)]
    public byte[] boolCollToArray(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "bCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedBooleanRow> boolCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateMethodCallQuery<IndexedBooleanRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Byte collection

    [Function(Name = Schema + "tiCollCreate", IsComposable = true)]
    public byte[] byteCollCreate(SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "tiCollParse", IsComposable = true)]
    public byte[] byteCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "tiCollFormat", IsComposable = true)]
    public String byteCollFormat(byte[] input)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "tiCollCount", IsComposable = true)]
    public int? byteCollCount(byte[] input)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "tiCollIndexOf", IsComposable = true)]
    public int? byteCollIndexOf(byte[] input, Byte? value)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "tiCollGet", IsComposable = true)]
    public Byte? byteCollGet(byte[] input, int index)
    {
      return (Byte?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "tiCollSet", IsComposable = true)]
    public byte[] byteCollSet(byte[] input, int index, Byte? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "tiCollInsert", IsComposable = true)]
    public byte[] byteCollInsert(byte[] input, int? index, Byte? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "tiCollRemove", IsComposable = true)]
    public byte[] byteCollRemove(byte[] input, Byte? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "tiCollRemoveAt", IsComposable = true)]
    public byte[] byteCollRemoveAt(byte[] input, int index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "tiCollClear", IsComposable = true)]
    public byte[] byteCollClear(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "tiCollGetRange", IsComposable = true)]
    public byte[] byteCollGetRange(byte[] input, int? index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "tiCollSetRange", IsComposable = true)]
    public byte[] byteCollSetRange(byte[] input, int index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "tiCollSetRepeat", IsComposable = true)]
    public byte[] byteCollSetRepeat(byte[] input, int index, Byte? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "tiCollInsertRange", IsComposable = true)]
    public byte[] byteCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "tiCollInsertRepeat", IsComposable = true)]
    public byte[] byteCollInsertRepeat(byte[] input, int? index, Byte? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "tiCollRemoveRange", IsComposable = true)]
    public byte[] byteCollRemoveRange(byte[] input, int index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "tiCollToArray", IsComposable = true)]
    public byte[] byteCollToArray(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "tiCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedByteRow> byteCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateMethodCallQuery<IndexedByteRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Int16 collection

    [Function(Name = Schema + "siCollCreate", IsComposable = true)]
    public byte[] int16CollCreate(SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "siCollParse", IsComposable = true)]
    public byte[] int16CollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "siCollFormat", IsComposable = true)]
    public String int16CollFormat(byte[] input)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "siCollCount", IsComposable = true)]
    public int? int16CollCount(byte[] input)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "siCollIndexOf", IsComposable = true)]
    public int? int16CollIndexOf(byte[] input, Int16? value)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "siCollGet", IsComposable = true)]
    public Int16? int16CollGet(byte[] input, int index)
    {
      return (Int16?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "siCollSet", IsComposable = true)]
    public byte[] int16CollSet(byte[] input, int index, Int16? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "siCollInsert", IsComposable = true)]
    public byte[] int16CollInsert(byte[] input, int? index, Int16? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "siCollRemove", IsComposable = true)]
    public byte[] int16CollRemove(byte[] input, Int16? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "siCollRemoveAt", IsComposable = true)]
    public byte[] int16CollRemoveAt(byte[] input, int index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "siCollClear", IsComposable = true)]
    public byte[] int16CollClear(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "siCollGetRange", IsComposable = true)]
    public byte[] int16CollGetRange(byte[] input, int? index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "siCollSetRange", IsComposable = true)]
    public byte[] int16CollSetRange(byte[] input, int index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "siCollSetRepeat", IsComposable = true)]
    public byte[] int16CollSetRepeat(byte[] input, int index, Int16? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "siCollInsertRange", IsComposable = true)]
    public byte[] int16CollInsertRange(byte[] input, int? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "siCollInsertRepeat", IsComposable = true)]
    public byte[] int16CollInsertRepeat(byte[] input, int? index, Int16? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "siCollRemoveRange", IsComposable = true)]
    public byte[] int16CollRemoveRange(byte[] input, int index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "siCollToArray", IsComposable = true)]
    public byte[] int16CollToArray(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "siCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedInt16Row> int16CollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateMethodCallQuery<IndexedInt16Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Int32 collection

    [Function(Name = Schema + "iCollCreate", IsComposable = true)]
    public byte[] int32CollCreate(SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "iCollParse", IsComposable = true)]
    public byte[] int32CollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "iCollFormat", IsComposable = true)]
    public String int32CollFormat(byte[] input)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "iCollCount", IsComposable = true)]
    public int? int32CollCount(byte[] input)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "iCollIndexOf", IsComposable = true)]
    public int? int32CollIndexOf(byte[] input, Int32? value)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "iCollGet", IsComposable = true)]
    public Int32? int32CollGet(byte[] input, int index)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "iCollSet", IsComposable = true)]
    public byte[] int32CollSet(byte[] input, int index, Int32? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "iCollInsert", IsComposable = true)]
    public byte[] int32CollInsert(byte[] input, int? index, Int32? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "iCollRemove", IsComposable = true)]
    public byte[] int32CollRemove(byte[] input, Int32? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "iCollRemoveAt", IsComposable = true)]
    public byte[] int32CollRemoveAt(byte[] input, int index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "iCollClear", IsComposable = true)]
    public byte[] int32CollClear(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "iCollGetRange", IsComposable = true)]
    public byte[] int32CollGetRange(byte[] input, int? index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "iCollSetRange", IsComposable = true)]
    public byte[] int32CollSetRange(byte[] input, int index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "iCollSetRepeat", IsComposable = true)]
    public byte[] int32CollSetRepeat(byte[] input, int index, Int32? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "iCollInsertRange", IsComposable = true)]
    public byte[] int32CollInsertRange(byte[] input, int? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "iCollInsertRepeat", IsComposable = true)]
    public byte[] int32CollInsertRepeat(byte[] input, int? index, Int32? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "iCollRemoveRange", IsComposable = true)]
    public byte[] int32CollRemoveRange(byte[] input, int index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "iCollToArray", IsComposable = true)]
    public byte[] int32CollToArray(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "iCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedInt32Row> int32CollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateMethodCallQuery<IndexedInt32Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Int64 collection

    [Function(Name = Schema + "biCollCreate", IsComposable = true)]
    public byte[] int64CollCreate(SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "biCollParse", IsComposable = true)]
    public byte[] int64CollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "biCollFormat", IsComposable = true)]
    public String int64CollFormat(byte[] input)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "biCollCount", IsComposable = true)]
    public int? int64CollCount(byte[] input)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "biCollIndexOf", IsComposable = true)]
    public int? int64CollIndexOf(byte[] input, Int64? value)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "biCollGet", IsComposable = true)]
    public Int64? int64CollGet(byte[] input, int index)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "biCollSet", IsComposable = true)]
    public byte[] int64CollSet(byte[] input, int index, Int64? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "biCollInsert", IsComposable = true)]
    public byte[] int64CollInsert(byte[] input, int? index, Int64? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "biCollRemove", IsComposable = true)]
    public byte[] int64CollRemove(byte[] input, Int64? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "biCollRemoveAt", IsComposable = true)]
    public byte[] int64CollRemoveAt(byte[] input, int index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "biCollClear", IsComposable = true)]
    public byte[] int64CollClear(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "biCollGetRange", IsComposable = true)]
    public byte[] int64CollGetRange(byte[] input, int? index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "biCollSetRange", IsComposable = true)]
    public byte[] int64CollSetRange(byte[] input, int index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "biCollSetRepeat", IsComposable = true)]
    public byte[] int64CollSetRepeat(byte[] input, int index, Int64? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "biCollInsertRange", IsComposable = true)]
    public byte[] int64CollInsertRange(byte[] input, int? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "biCollInsertRepeat", IsComposable = true)]
    public byte[] int64CollInsertRepeat(byte[] input, int? index, Int64? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "biCollRemoveRange", IsComposable = true)]
    public byte[] int64CollRemoveRange(byte[] input, int index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "biCollToArray", IsComposable = true)]
    public byte[] int64CollToArray(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "biCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedInt64Row> int64CollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateMethodCallQuery<IndexedInt64Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Single collection

    [Function(Name = Schema + "sfCollCreate", IsComposable = true)]
    public byte[] sglCollCreate(SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "sfCollParse", IsComposable = true)]
    public byte[] sglCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "sfCollFormat", IsComposable = true)]
    public String sglCollFormat(byte[] input)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "sfCollCount", IsComposable = true)]
    public int? sglCollCount(byte[] input)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "sfCollIndexOf", IsComposable = true)]
    public int? sglCollIndexOf(byte[] input, Single? value)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "sfCollGet", IsComposable = true)]
    public Single? sglCollGet(byte[] input, int index)
    {
      return (Single?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "sfCollSet", IsComposable = true)]
    public byte[] sglCollSet(byte[] input, int index, Single? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "sfCollInsert", IsComposable = true)]
    public byte[] sglCollInsert(byte[] input, int? index, Single? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "sfCollRemove", IsComposable = true)]
    public byte[] sglCollRemove(byte[] input, Single? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "sfCollRemoveAt", IsComposable = true)]
    public byte[] sglCollRemoveAt(byte[] input, int index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "sfCollClear", IsComposable = true)]
    public byte[] sglCollClear(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "sfCollGetRange", IsComposable = true)]
    public byte[] sglCollGetRange(byte[] input, int? index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "sfCollSetRange", IsComposable = true)]
    public byte[] sglCollSetRange(byte[] input, int index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "sfCollSetRepeat", IsComposable = true)]
    public byte[] sglCollSetRepeat(byte[] input, int index, Single? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "sfCollInsertRange", IsComposable = true)]
    public byte[] sglCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "sfCollInsertRepeat", IsComposable = true)]
    public byte[] sglCollInsertRepeat(byte[] input, int? index, Single? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "sfCollRemoveRange", IsComposable = true)]
    public byte[] sglCollRemoveRange(byte[] input, int index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "sfCollToArray", IsComposable = true)]
    public byte[] sglCollToArray(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "sfCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedSingleRow> sglCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateMethodCallQuery<IndexedSingleRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Double collection

    [Function(Name = Schema + "dfCollCreate", IsComposable = true)]
    public byte[] dblCollCreate(SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "dfCollParse", IsComposable = true)]
    public byte[] dblCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "dfCollFormat", IsComposable = true)]
    public String dblCollFormat(byte[] input)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "dfCollCount", IsComposable = true)]
    public int? dblCollCount(byte[] input)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "dfCollIndexOf", IsComposable = true)]
    public int? dblCollIndexOf(byte[] input, Double? value)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "dfCollGet", IsComposable = true)]
    public Double? dblCollGet(byte[] input, int index)
    {
      return (Double?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "dfCollSet", IsComposable = true)]
    public byte[] dblCollSet(byte[] input, int index, Double? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "dfCollInsert", IsComposable = true)]
    public byte[] dblCollInsert(byte[] input, int? index, Double? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "dfCollRemove", IsComposable = true)]
    public byte[] dblCollRemove(byte[] input, Double? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "dfCollRemoveAt", IsComposable = true)]
    public byte[] dblCollRemoveAt(byte[] input, int index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "dfCollClear", IsComposable = true)]
    public byte[] dblCollClear(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "dfCollGetRange", IsComposable = true)]
    public byte[] dblCollGetRange(byte[] input, int? index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "dfCollSetRange", IsComposable = true)]
    public byte[] dblCollSetRange(byte[] input, int index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "dfCollSetRepeat", IsComposable = true)]
    public byte[] dblCollSetRepeat(byte[] input, int index, Double? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "dfCollInsertRange", IsComposable = true)]
    public byte[] dblCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "dfCollInsertRepeat", IsComposable = true)]
    public byte[] dblCollInsertRepeat(byte[] input, int? index, Double? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "dfCollRemoveRange", IsComposable = true)]
    public byte[] dblCollRemoveRange(byte[] input, int index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "dfCollToArray", IsComposable = true)]
    public byte[] dblCollToArray(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "dfCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedDoubleRow> dblCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateMethodCallQuery<IndexedDoubleRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region DateTime collection

    [Function(Name = Schema + "dtCollCreate", IsComposable = true)]
    public byte[] dtmCollCreate(SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "dtCollParse", IsComposable = true)]
    public byte[] dtmCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "dtCollFormat", IsComposable = true)]
    public String dtmCollFormat(byte[] input)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "dtCollCount", IsComposable = true)]
    public int? dtmCollCount(byte[] input)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "dtCollIndexOf", IsComposable = true)]
    public int? dtmCollIndexOf(byte[] input, DateTime? value)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "dtCollGet", IsComposable = true)]
    public DateTime? dtmCollGet(byte[] input, int index)
    {
      return (DateTime?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "dtCollSet", IsComposable = true)]
    public byte[] dtmCollSet(byte[] input, int index, DateTime? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "dtCollInsert", IsComposable = true)]
    public byte[] dtmCollInsert(byte[] input, int? index, DateTime? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "dtCollRemove", IsComposable = true)]
    public byte[] dtmCollRemove(byte[] input, DateTime? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "dtCollRemoveAt", IsComposable = true)]
    public byte[] dtmCollRemoveAt(byte[] input, int index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "dtCollClear", IsComposable = true)]
    public byte[] dtmCollClear(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "dtCollGetRange", IsComposable = true)]
    public byte[] dtmCollGetRange(byte[] input, int? index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "dtCollSetRange", IsComposable = true)]
    public byte[] dtmCollSetRange(byte[] input, int index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "dtCollSetRepeat", IsComposable = true)]
    public byte[] dtmCollSetRepeat(byte[] input, int index, DateTime? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "dtCollInsertRange", IsComposable = true)]
    public byte[] dtmCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "dtCollInsertRepeat", IsComposable = true)]
    public byte[] dtmCollInsertRepeat(byte[] input, int? index, DateTime? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "dtCollRemoveRange", IsComposable = true)]
    public byte[] dtmCollRemoveRange(byte[] input, int index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "dtCollToArray", IsComposable = true)]
    public byte[] dtmCollToArray(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "dtCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedDateTimeRow> dtmCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateMethodCallQuery<IndexedDateTimeRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Guid collection

    [Function(Name = Schema + "uidCollCreate", IsComposable = true)]
    public byte[] guidCollCreate(SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "uidCollParse", IsComposable = true)]
    public byte[] guidCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "uidCollFormat", IsComposable = true)]
    public String guidCollFormat(byte[] input)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "uidCollCount", IsComposable = true)]
    public int? guidCollCount(byte[] input)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "uidCollIndexOf", IsComposable = true)]
    public int? guidCollIndexOf(byte[] input, Guid? value)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "uidCollGet", IsComposable = true)]
    public Guid? guidCollGet(byte[] input, int index)
    {
      return (Guid?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "uidCollSet", IsComposable = true)]
    public byte[] guidCollSet(byte[] input, int index, Guid? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "uidCollInsert", IsComposable = true)]
    public byte[] guidCollInsert(byte[] input, int? index, Guid? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "uidCollRemove", IsComposable = true)]
    public byte[] guidCollRemove(byte[] input, Guid? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "uidCollRemoveAt", IsComposable = true)]
    public byte[] guidCollRemoveAt(byte[] input, int index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "uidCollClear", IsComposable = true)]
    public byte[] guidCollClear(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "uidCollGetRange", IsComposable = true)]
    public byte[] guidCollGetRange(byte[] input, int? index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "uidCollSetRange", IsComposable = true)]
    public byte[] guidCollSetRange(byte[] input, int index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "uidCollSetRepeat", IsComposable = true)]
    public byte[] guidCollSetRepeat(byte[] input, int index, Guid? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "uidCollInsertRange", IsComposable = true)]
    public byte[] guidCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "uidCollInsertRepeat", IsComposable = true)]
    public byte[] guidCollInsertRepeat(byte[] input, int? index, Guid? value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "uidCollRemoveRange", IsComposable = true)]
    public byte[] guidCollRemoveRange(byte[] input, int index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "uidCollToArray", IsComposable = true)]
    public byte[] guidCollToArray(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "uidCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedGuidRow> guidCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateMethodCallQuery<IndexedGuidRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region String collection

    [Function(Name = Schema + "strCollCreate", IsComposable = true)]
    public byte[] strCollCreate(SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing, itemSizing).ReturnValue;
    }

    [Function(Name = Schema + "strCollCreateByCpId", IsComposable = true)]
    public byte[] strCollCreate(SizeEncoding countSizing, SizeEncoding itemSizing, int? cpId)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing, itemSizing, cpId).ReturnValue;
    }

    [Function(Name = Schema + "strCollCreateByCpName", IsComposable = true)]
    public byte[] strCollCreate(SizeEncoding countSizing, SizeEncoding itemSizing, string cpName)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing, itemSizing, cpName).ReturnValue;
    }

    [Function(Name = Schema + "strCollParse", IsComposable = true)]
    public byte[] strCollParse(String input, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing, itemSizing).ReturnValue;
    }

    [Function(Name = Schema + "strCollParseByCpId", IsComposable = true)]
    public byte[] strCollParse(String input, SizeEncoding countSizing, SizeEncoding itemSizing, int? cpId)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing, itemSizing, cpId).ReturnValue;
    }

    [Function(Name = Schema + "strCollParseByCpName", IsComposable = true)]
    public byte[] strCollParse(String input, SizeEncoding countSizing, SizeEncoding itemSizing, string cpName)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing, itemSizing, cpName).ReturnValue;
    }

    [Function(Name = Schema + "strCollFormat", IsComposable = true)]
    public String strCollFormat(byte[] input)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "strCollCount", IsComposable = true)]
    public int? strCollCount(byte[] input)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "strCollIndexOf", IsComposable = true)]
    public int? strCollIndexOf(byte[] input, String value)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "strCollGet", IsComposable = true)]
    public String strCollGet(byte[] input, int index)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "strCollSet", IsComposable = true)]
    public byte[] strCollSet(byte[] input, int index, String value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "strCollInsert", IsComposable = true)]
    public byte[] strCollInsert(byte[] input, int? index, String value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "strCollRemove", IsComposable = true)]
    public byte[] strCollRemove(byte[] input, String value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "strCollRemoveAt", IsComposable = true)]
    public byte[] strCollRemoveAt(byte[] input, int index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "strCollClear", IsComposable = true)]
    public byte[] strCollClear(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "strCollGetRange", IsComposable = true)]
    public byte[] strCollGetRange(byte[] input, int? index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "strCollSetRange", IsComposable = true)]
    public byte[] strCollSetRange(byte[] input, int index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "strCollSetRepeat", IsComposable = true)]
    public byte[] strCollSetRepeat(byte[] input, int index, String value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "strCollInsertRange", IsComposable = true)]
    public byte[] strCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "strCollInsertRepeat", IsComposable = true)]
    public byte[] strCollInsertRepeat(byte[] input, int? index, String value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "strCollRemoveRange", IsComposable = true)]
    public byte[] strCollRemoveRange(byte[] input, int index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "strCollToArray", IsComposable = true)]
    public byte[] strCollToArray(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "strCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedStringRow> strCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateMethodCallQuery<IndexedStringRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Binary collection

    [Function(Name = Schema + "binCollCreate", IsComposable = true)]
    public byte[] binCollCreate(SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), countSizing, itemSizing).ReturnValue;
    }

    [Function(Name = Schema + "binCollParse", IsComposable = true)]
    public byte[] binCollParse(String input, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, countSizing, itemSizing).ReturnValue;
    }

    [Function(Name = Schema + "binCollFormat", IsComposable = true)]
    public String binCollFormat(byte[] input)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binCollCount", IsComposable = true)]
    public int? binCollCount(byte[] input)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binCollIndexOf", IsComposable = true)]
    public int? binCollIndexOf(byte[] input, byte[] value)
    {
      return (int?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "binCollGet", IsComposable = true)]
    public byte[] binCollGet(byte[] input, int index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "binCollSet", IsComposable = true)]
    public byte[] binCollSet(byte[] input, int index, byte[] value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "binCollInsert", IsComposable = true)]
    public byte[] binCollInsert(byte[] input, int? index, byte[] value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value).ReturnValue;
    }

    [Function(Name = Schema + "binCollRemove", IsComposable = true)]
    public byte[] binCollRemove(byte[] input, byte[] value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, value).ReturnValue;
    }

    [Function(Name = Schema + "binCollRemoveAt", IsComposable = true)]
    public byte[] binCollRemoveAt(byte[] input, int index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index).ReturnValue;
    }

    [Function(Name = Schema + "binCollClear", IsComposable = true)]
    public byte[] binCollClear(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binCollGetRange", IsComposable = true)]
    public byte[] binCollGetRange(byte[] input, int? index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "binCollSetRange", IsComposable = true)]
    public byte[] binCollSetRange(byte[] input, int index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "binCollSetRepeat", IsComposable = true)]
    public byte[] binCollSetRepeat(byte[] input, int index, byte[] value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "binCollInsertRange", IsComposable = true)]
    public byte[] binCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, range).ReturnValue;
    }

    [Function(Name = Schema + "binCollInsertRepeat", IsComposable = true)]
    public byte[] binCollInsertRepeat(byte[] input, int? index, byte[] value, int count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, value, count).ReturnValue;
    }

    [Function(Name = Schema + "binCollRemoveRange", IsComposable = true)]
    public byte[] binCollRemoveRange(byte[] input, int index, int? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count).ReturnValue;
    }

    [Function(Name = Schema + "binCollToArray", IsComposable = true)]
    public byte[] binCollToArray(byte[] input)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input).ReturnValue;
    }

    [Function(Name = Schema + "binCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedBinaryRow> binCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateMethodCallQuery<IndexedBinaryRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #endregion
    #region Array functions
    #region Boolean array methods

    [Function(Name = Schema + "bArrayCreate", IsComposable = true)]
    public byte[] boolArrayCreate(Int32 length, SizeEncoding countSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing).ReturnValue;
    }

    [Function(Name = Schema + "bArrayParse", IsComposable = true)]
    public byte[] boolArrayParse(String str, SizeEncoding countSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing).ReturnValue;
    }

    [Function(Name = Schema + "bArrayFormat", IsComposable = true)]
    public String boolArrayFormat(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "bArrayLength", IsComposable = true)]
    public Int32? boolArrayLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "bArrayIndexOf", IsComposable = true)]
    public Int32? boolArrayIndexOf(byte[] array, Boolean? value)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, value).ReturnValue;
    }

    [Function(Name = Schema + "bArrayGet", IsComposable = true)]
    public Boolean? boolArrayGet(byte[] array, Int32 index)
    {
      return (Boolean?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "bArraySet", IsComposable = true)]
    public byte[] boolArraySet(byte[] array, Int32 index, Boolean? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "bArrayGetRange", IsComposable = true)]
    public byte[] boolArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "bArraySetRange", IsComposable = true)]
    public byte[] boolArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, range).ReturnValue;
    }

    [Function(Name = Schema + "bArrayFillRange", IsComposable = true)]
    public byte[] boolArrayFillRange(byte[] array, Int32? index, Int32? count, Boolean? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "bArrayToCollection", IsComposable = true)]
    public byte[] boolArrayToCollection(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "bArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedBooleanRow> boolArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<IndexedBooleanRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Byte array methods

    [Function(Name = Schema + "tiArrayCreate", IsComposable = true)]
    public byte[] byteArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayParse", IsComposable = true)]
    public byte[] byteArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayFormat", IsComposable = true)]
    public String byteArrayFormat(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayLength", IsComposable = true)]
    public Int32? byteArrayLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayIndexOf", IsComposable = true)]
    public Int32? byteArrayIndexOf(byte[] array, Byte? value)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, value).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayGet", IsComposable = true)]
    public Byte? byteArrayGet(byte[] array, Int32 index)
    {
      return (Byte?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "tiArraySet", IsComposable = true)]
    public byte[] byteArraySet(byte[] array, Int32 index, Byte? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayGetRange", IsComposable = true)]
    public byte[] byteArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "tiArraySetRange", IsComposable = true)]
    public byte[] byteArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, range).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayFillRange", IsComposable = true)]
    public byte[] byteArrayFillRange(byte[] array, Int32? index, Int32? count, Byte? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayToCollection", IsComposable = true)]
    public byte[] byteArrayToCollection(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedByteRow> byteArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<IndexedByteRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Int16 array methods

    [Function(Name = Schema + "siArrayCreate", IsComposable = true)]
    public byte[] int16ArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "siArrayParse", IsComposable = true)]
    public byte[] int16ArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "siArrayFormat", IsComposable = true)]
    public String int16ArrayFormat(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "siArrayLength", IsComposable = true)]
    public Int32? int16ArrayLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "siArrayIndexOf", IsComposable = true)]
    public Int32? int16ArrayIndexOf(byte[] array, Int16? value)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, value).ReturnValue;
    }

    [Function(Name = Schema + "siArrayGet", IsComposable = true)]
    public Int16? int16ArrayGet(byte[] array, Int32 index)
    {
      return (Int16?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "siArraySet", IsComposable = true)]
    public byte[] int16ArraySet(byte[] array, Int32 index, Int16? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "siArrayGetRange", IsComposable = true)]
    public byte[] int16ArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "siArraySetRange", IsComposable = true)]
    public byte[] int16ArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, range).ReturnValue;
    }

    [Function(Name = Schema + "siArrayFillRange", IsComposable = true)]
    public byte[] int16ArrayFillRange(byte[] array, Int32? index, Int32? count, Int16? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "siArrayToCollection", IsComposable = true)]
    public byte[] int16ArrayToCollection(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "siArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedInt16Row> int16ArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<IndexedInt16Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Int32 array methods

    [Function(Name = Schema + "iArrayCreate", IsComposable = true)]
    public byte[] int32ArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "iArrayParse", IsComposable = true)]
    public byte[] int32ArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "iArrayFormat", IsComposable = true)]
    public String int32ArrayFormat(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "iArrayLength", IsComposable = true)]
    public Int32? int32ArrayLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "iArrayIndexOf", IsComposable = true)]
    public Int32? int32ArrayIndexOf(byte[] array, Int32? value)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, value).ReturnValue;
    }

    [Function(Name = Schema + "iArrayGet", IsComposable = true)]
    public Int32? int32ArrayGet(byte[] array, Int32 index)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "iArraySet", IsComposable = true)]
    public byte[] int32ArraySet(byte[] array, Int32 index, Int32? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "iArrayGetRange", IsComposable = true)]
    public byte[] int32ArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "iArraySetRange", IsComposable = true)]
    public byte[] int32ArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, range).ReturnValue;
    }

    [Function(Name = Schema + "iArrayFillRange", IsComposable = true)]
    public byte[] int32ArrayFillRange(byte[] array, Int32? index, Int32? count, Int32? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "iArrayToCollection", IsComposable = true)]
    public byte[] int32ArrayToCollection(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "iArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedInt32Row> int32ArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<IndexedInt32Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Int64 array methods

    [Function(Name = Schema + "biArrayCreate", IsComposable = true)]
    public byte[] int64ArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "biArrayParse", IsComposable = true)]
    public byte[] int64ArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "biArrayFormat", IsComposable = true)]
    public String int64ArrayFormat(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "biArrayLength", IsComposable = true)]
    public Int32? int64ArrayLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "biArrayIndexOf", IsComposable = true)]
    public Int32? int64ArrayIndexOf(byte[] array, Int64? value)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, value).ReturnValue;
    }

    [Function(Name = Schema + "biArrayGet", IsComposable = true)]
    public Int64? int64ArrayGet(byte[] array, Int32 index)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "biArraySet", IsComposable = true)]
    public byte[] int64ArraySet(byte[] array, Int32 index, Int64? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "biArrayGetRange", IsComposable = true)]
    public byte[] int64ArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "biArraySetRange", IsComposable = true)]
    public byte[] int64ArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, range).ReturnValue;
    }

    [Function(Name = Schema + "biArrayFillRange", IsComposable = true)]
    public byte[] int64ArrayFillRange(byte[] array, Int32? index, Int32? count, Int64? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "biArrayToCollection", IsComposable = true)]
    public byte[] int64ArrayToCollection(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "biArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedInt64Row> int64ArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<IndexedInt64Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Single array methods

    [Function(Name = Schema + "sfArrayCreate", IsComposable = true)]
    public byte[] sglArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayParse", IsComposable = true)]
    public byte[] sglArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayFormat", IsComposable = true)]
    public String sglArrayFormat(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayLength", IsComposable = true)]
    public Int32? sglArrayLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayIndexOf", IsComposable = true)]
    public Int32? sglArrayIndexOf(byte[] array, Single? value)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, value).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayGet", IsComposable = true)]
    public Single? sglArrayGet(byte[] array, Int32 index)
    {
      return (Single?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "sfArraySet", IsComposable = true)]
    public byte[] sglArraySet(byte[] array, Int32 index, Single? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayGetRange", IsComposable = true)]
    public byte[] sglArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "sfArraySetRange", IsComposable = true)]
    public byte[] sglArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, range).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayFillRange", IsComposable = true)]
    public byte[] sglArrayFillRange(byte[] array, Int32? index, Int32? count, Single? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayToCollection", IsComposable = true)]
    public byte[] sglArrayToCollection(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedSingleRow> sglArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<IndexedSingleRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Double array methods

    [Function(Name = Schema + "dfArrayCreate", IsComposable = true)]
    public byte[] dblArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayParse", IsComposable = true)]
    public byte[] dblArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayFormat", IsComposable = true)]
    public String dblArrayFormat(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayLength", IsComposable = true)]
    public Int32? dblArrayLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayIndexOf", IsComposable = true)]
    public Int32? dblArrayIndexOf(byte[] array, Double? value)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, value).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayGet", IsComposable = true)]
    public Double? dblArrayGet(byte[] array, Int32 index)
    {
      return (Double?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "dfArraySet", IsComposable = true)]
    public byte[] dblArraySet(byte[] array, Int32 index, Double? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayGetRange", IsComposable = true)]
    public byte[] dblArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "dfArraySetRange", IsComposable = true)]
    public byte[] dblArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, range).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayFillRange", IsComposable = true)]
    public byte[] dblArrayFillRange(byte[] array, Int32? index, Int32? count, Double? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayToCollection", IsComposable = true)]
    public byte[] dblArrayToCollection(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedDoubleRow> dblArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<IndexedDoubleRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region DateTime array methods

    [Function(Name = Schema + "dtArrayCreate", IsComposable = true)]
    public byte[] dtmArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayParse", IsComposable = true)]
    public byte[] dtmArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayFormat", IsComposable = true)]
    public String dtmArrayFormat(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayLength", IsComposable = true)]
    public Int32? dtmArrayLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayIndexOf", IsComposable = true)]
    public Int32? dtmArrayIndexOf(byte[] array, DateTime? value)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, value).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayGet", IsComposable = true)]
    public DateTime? dtmArrayGet(byte[] array, Int32 index)
    {
      return (DateTime?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "dtArraySet", IsComposable = true)]
    public byte[] dtmArraySet(byte[] array, Int32 index, DateTime? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayGetRange", IsComposable = true)]
    public byte[] dtmArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "dtArraySetRange", IsComposable = true)]
    public byte[] dtmArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, range).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayFillRange", IsComposable = true)]
    public byte[] dtmArrayFillRange(byte[] array, Int32? index, Int32? count, DateTime? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayToCollection", IsComposable = true)]
    public byte[] dtmArrayToCollection(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedDateTimeRow> dtmArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<IndexedDateTimeRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Guid array methods

    [Function(Name = Schema + "uidArrayCreate", IsComposable = true)]
    public byte[] guidArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayParse", IsComposable = true)]
    public byte[] guidArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayFormat", IsComposable = true)]
    public String guidArrayFormat(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayLength", IsComposable = true)]
    public Int32? guidArrayLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayIndexOf", IsComposable = true)]
    public Int32? guidArrayIndexOf(byte[] array, Guid? value)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, value).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayGet", IsComposable = true)]
    public Guid? guidArrayGet(byte[] array, Int32 index)
    {
      return (Guid?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "uidArraySet", IsComposable = true)]
    public byte[] guidArraySet(byte[] array, Int32 index, Guid? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayGetRange", IsComposable = true)]
    public byte[] guidArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "uidArraySetRange", IsComposable = true)]
    public byte[] guidArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, range).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayFillRange", IsComposable = true)]
    public byte[] guidArrayFillRange(byte[] array, Int32? index, Int32? count, Guid? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayToCollection", IsComposable = true)]
    public byte[] guidArrayToCollection(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedGuidRow> guidArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<IndexedGuidRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region String array methods

    [Function(Name = Schema + "strArrayCreate", IsComposable = true)]
    public byte[] strArrayCreate(Int32 length, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing, itemSizing).ReturnValue;
    }

    [Function(Name = Schema + "strArrayCreateByCpId", IsComposable = true)]
    public byte[] strArrayCreate(Int32 length, SizeEncoding countSizing, SizeEncoding itemSizing, Int32? cpId)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing, itemSizing, cpId).ReturnValue;
    }

    [Function(Name = Schema + "strArrayCreateByCpName", IsComposable = true)]
    public byte[] strArrayCreate(Int32 length, SizeEncoding countSizing, SizeEncoding itemSizing, String cpName)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing, itemSizing, cpName).ReturnValue;
    }

    [Function(Name = Schema + "strArrayParse", IsComposable = true)]
    public byte[] strArrayParse(String str, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing).ReturnValue;
    }

    [Function(Name = Schema + "strArrayParseByCpId", IsComposable = true)]
    public byte[] strArrayParse(String str, SizeEncoding countSizing, SizeEncoding itemSizing, Int32? cpId)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing, cpId).ReturnValue;
    }

    [Function(Name = Schema + "strArrayParseByCpName", IsComposable = true)]
    public byte[] strArrayParse(String str, SizeEncoding countSizing, SizeEncoding itemSizing, String cpName)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing, cpName).ReturnValue;
    }

    [Function(Name = Schema + "strArrayFormat", IsComposable = true)]
    public String strArrayFormat(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "strArrayLength", IsComposable = true)]
    public Int32? strArrayLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "strArrayIndexOf", IsComposable = true)]
    public Int32? strArrayIndexOf(byte[] array, String value)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, value).ReturnValue;
    }

    [Function(Name = Schema + "strArrayGet", IsComposable = true)]
    public String strArrayGet(byte[] array, Int32 index)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "strArraySet", IsComposable = true)]
    public byte[] strArraySet(byte[] array, Int32 index, String value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "strArrayGetRange", IsComposable = true)]
    public byte[] strArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "strArraySetRange", IsComposable = true)]
    public byte[] strArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, range).ReturnValue;
    }

    [Function(Name = Schema + "strArrayFillRange", IsComposable = true)]
    public byte[] strArrayFillRange(byte[] array, Int32? index, Int32? count, String value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "strArrayToCollection", IsComposable = true)]
    public byte[] strArrayToCollection(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "strArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedStringRow> strArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<IndexedStringRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Binary array methods

    [Function(Name = Schema + "binArrayCreate", IsComposable = true)]
    public byte[] binArrayCreate(Int32 length, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), length, countSizing, itemSizing).ReturnValue;
    }

    [Function(Name = Schema + "binArrayParse", IsComposable = true)]
    public byte[] binArrayParse(String str, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing).ReturnValue;
    }

    [Function(Name = Schema + "binArrayFormat", IsComposable = true)]
    public String binArrayFormat(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "binArrayLength", IsComposable = true)]
    public Int32? binArrayLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "binArrayIndexOf", IsComposable = true)]
    public Int32? binArrayIndexOf(byte[] array, Byte[] value)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, value).ReturnValue;
    }

    [Function(Name = Schema + "binArrayGet", IsComposable = true)]
    public Byte[] binArrayGet(byte[] array, Int32 index)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "binArraySet", IsComposable = true)]
    public byte[] binArraySet(byte[] array, Int32 index, Byte[] value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "binArrayGetRange", IsComposable = true)]
    public byte[] binArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "binArraySetRange", IsComposable = true)]
    public byte[] binArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, range).ReturnValue;
    }

    [Function(Name = Schema + "binArrayFillRange", IsComposable = true)]
    public byte[] binArrayFillRange(byte[] array, Int32? index, Int32? count, Byte[] value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "binArrayToCollection", IsComposable = true)]
    public byte[] binArrayToCollection(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "binArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedBinaryRow> binArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<IndexedBinaryRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #endregion
    #region Regular array functions
    #region SqlBoolean regular array methods

    [Function(Name = Schema + "bArrayCreateRegular", IsComposable = true)]
    public byte[] boolArrayCreateRegular(byte[] lengths, SizeEncoding countSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing).ReturnValue;
    }

    [Function(Name = Schema + "bArrayParseRegular", IsComposable = true)]
    public byte[] boolArrayParseRegular(String str, SizeEncoding countSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing).ReturnValue;
    }

    [Function(Name = Schema + "bArrayFormatRegular", IsComposable = true)]
    public String boolArrayFormatRegular(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "bArrayRank", IsComposable = true)]
    public Int32? boolArrayRank(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "bArrayFlatLength", IsComposable = true)]
    public Int32? boolArrayFlatLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "bArrayDimLengths", IsComposable = true)]
    public byte[] boolArrayDimLengths(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "bArrayDimLength", IsComposable = true)]
    public byte[] boolArrayDimLength(byte[] array, Int32? index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "bArrayGetFlat", IsComposable = true)]
    public Boolean? boolArrayGetFlat(byte[] array, Int32? index)
    {
      return (Boolean?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "bArraySetFlat", IsComposable = true)]
    public byte[] boolArraySetFlat(byte[] array, Int32? index, Boolean? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "bArrayGetFlatRange", IsComposable = true)]
    public byte[] boolArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "bArraySetFlatRange", IsComposable = true)]
    public byte[] boolArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, range).ReturnValue;
    }

    [Function(Name = Schema + "bArrayFillFlatRange", IsComposable = true)]
    public byte[] boolArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Boolean? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "bArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedBooleanRow> boolArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<RegularIndexedBooleanRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [Function(Name = Schema + "bArrayGetDim", IsComposable = true)]
    public Boolean? boolArrayGetDim(byte[] array, byte[] indices)
    {
      return (Boolean?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices).ReturnValue;
    }

    [Function(Name = Schema + "bArraySetDim", IsComposable = true)]
    public byte[] boolArraySetDim(byte[] array, byte[] indices, Boolean? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, value).ReturnValue;
    }

    [Function(Name = Schema + "bArrayGetDimRange", IsComposable = true)]
    public byte[] boolArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges).ReturnValue;
    }

    [Function(Name = Schema + "bArraySetDimRange", IsComposable = true)]
    public byte[] boolArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, range).ReturnValue;
    }

    [Function(Name = Schema + "bArrayFillDimRange", IsComposable = true)]
    public byte[] boolArrayFillDimRange(byte[] array, byte[] ranges, Boolean? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges, value).ReturnValue;
    }

    [Function(Name = Schema + "bArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedBooleanRow> boolArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateMethodCallQuery<RegularIndexedBooleanRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlByte regular array methods

    [Function(Name = Schema + "tiArrayCreateRegular", IsComposable = true)]
    public byte[] byteArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayParseRegular", IsComposable = true)]
    public byte[] byteArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayFormatRegular", IsComposable = true)]
    public String byteArrayFormatRegular(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayRank", IsComposable = true)]
    public Int32? byteArrayRank(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayFlatLength", IsComposable = true)]
    public Int32? byteArrayFlatLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayDimLengths", IsComposable = true)]
    public byte[] byteArrayDimLengths(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayDimLength", IsComposable = true)]
    public byte[] byteArrayDimLength(byte[] array, Int32? index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayGetFlat", IsComposable = true)]
    public Byte? byteArrayGetFlat(byte[] array, Int32? index)
    {
      return (Byte?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "tiArraySetFlat", IsComposable = true)]
    public byte[] byteArraySetFlat(byte[] array, Int32? index, Byte? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayGetFlatRange", IsComposable = true)]
    public byte[] byteArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "tiArraySetFlatRange", IsComposable = true)]
    public byte[] byteArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, range).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayFillFlatRange", IsComposable = true)]
    public byte[] byteArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Byte? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedByteRow> byteArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<RegularIndexedByteRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [Function(Name = Schema + "tiArrayGetDim", IsComposable = true)]
    public Byte? byteArrayGetDim(byte[] array, byte[] indices)
    {
      return (Byte?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices).ReturnValue;
    }

    [Function(Name = Schema + "tiArraySetDim", IsComposable = true)]
    public byte[] byteArraySetDim(byte[] array, byte[] indices, Byte? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, value).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayGetDimRange", IsComposable = true)]
    public byte[] byteArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges).ReturnValue;
    }

    [Function(Name = Schema + "tiArraySetDimRange", IsComposable = true)]
    public byte[] byteArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, range).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayFillDimRange", IsComposable = true)]
    public byte[] byteArrayFillDimRange(byte[] array, byte[] ranges, Byte? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges, value).ReturnValue;
    }

    [Function(Name = Schema + "tiArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedByteRow> byteArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateMethodCallQuery<RegularIndexedByteRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlInt16 regular array methods

    [Function(Name = Schema + "siArrayCreateRegular", IsComposable = true)]
    public byte[] int16ArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "siArrayParseRegular", IsComposable = true)]
    public byte[] int16ArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "siArrayFormatRegular", IsComposable = true)]
    public String int16ArrayFormatRegular(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "siArrayRank", IsComposable = true)]
    public Int32? int16ArrayRank(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "siArrayFlatLength", IsComposable = true)]
    public Int32? int16ArrayFlatLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "siArrayDimLengths", IsComposable = true)]
    public byte[] int16ArrayDimLengths(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "siArrayDimLength", IsComposable = true)]
    public byte[] int16ArrayDimLength(byte[] array, Int32? index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "siArrayGetFlat", IsComposable = true)]
    public Int16? int16ArrayGetFlat(byte[] array, Int32? index)
    {
      return (Int16?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "siArraySetFlat", IsComposable = true)]
    public byte[] int16ArraySetFlat(byte[] array, Int32? index, Int16? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "siArrayGetFlatRange", IsComposable = true)]
    public byte[] int16ArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "siArraySetFlatRange", IsComposable = true)]
    public byte[] int16ArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, range).ReturnValue;
    }

    [Function(Name = Schema + "siArrayFillFlatRange", IsComposable = true)]
    public byte[] int16ArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Int16? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "siArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedInt16Row> int16ArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<RegularIndexedInt16Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [Function(Name = Schema + "siArrayGetDim", IsComposable = true)]
    public Int16? int16ArrayGetDim(byte[] array, byte[] indices)
    {
      return (Int16?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices).ReturnValue;
    }

    [Function(Name = Schema + "siArraySetDim", IsComposable = true)]
    public byte[] int16ArraySetDim(byte[] array, byte[] indices, Int16? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, value).ReturnValue;
    }

    [Function(Name = Schema + "siArrayGetDimRange", IsComposable = true)]
    public byte[] int16ArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges).ReturnValue;
    }

    [Function(Name = Schema + "siArraySetDimRange", IsComposable = true)]
    public byte[] int16ArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, range).ReturnValue;
    }

    [Function(Name = Schema + "siArrayFillDimRange", IsComposable = true)]
    public byte[] int16ArrayFillDimRange(byte[] array, byte[] ranges, Int16? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges, value).ReturnValue;
    }

    [Function(Name = Schema + "siArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedInt16Row> int16ArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateMethodCallQuery<RegularIndexedInt16Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlInt32 regular array methods

    [Function(Name = Schema + "iArrayCreateRegular", IsComposable = true)]
    public byte[] int32ArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "iArrayParseRegular", IsComposable = true)]
    public byte[] int32ArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "iArrayFormatRegular", IsComposable = true)]
    public String int32ArrayFormatRegular(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "iArrayRank", IsComposable = true)]
    public Int32? int32ArrayRank(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "iArrayFlatLength", IsComposable = true)]
    public Int32? int32ArrayFlatLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "iArrayDimLengths", IsComposable = true)]
    public byte[] int32ArrayDimLengths(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "iArrayDimLength", IsComposable = true)]
    public byte[] int32ArrayDimLength(byte[] array, Int32? index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "iArrayGetFlat", IsComposable = true)]
    public Int32? int32ArrayGetFlat(byte[] array, Int32? index)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "iArraySetFlat", IsComposable = true)]
    public byte[] int32ArraySetFlat(byte[] array, Int32? index, Int32? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "iArrayGetFlatRange", IsComposable = true)]
    public byte[] int32ArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "iArraySetFlatRange", IsComposable = true)]
    public byte[] int32ArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, range).ReturnValue;
    }

    [Function(Name = Schema + "iArrayFillFlatRange", IsComposable = true)]
    public byte[] int32ArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Int32? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "iArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedInt32Row> int32ArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<RegularIndexedInt32Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [Function(Name = Schema + "iArrayGetDim", IsComposable = true)]
    public Int32? int32ArrayGetDim(byte[] array, byte[] indices)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices).ReturnValue;
    }

    [Function(Name = Schema + "iArraySetDim", IsComposable = true)]
    public byte[] int32ArraySetDim(byte[] array, byte[] indices, Int32? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, value).ReturnValue;
    }

    [Function(Name = Schema + "iArrayGetDimRange", IsComposable = true)]
    public byte[] int32ArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges).ReturnValue;
    }

    [Function(Name = Schema + "iArraySetDimRange", IsComposable = true)]
    public byte[] int32ArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, range).ReturnValue;
    }

    [Function(Name = Schema + "iArrayFillDimRange", IsComposable = true)]
    public byte[] int32ArrayFillDimRange(byte[] array, byte[] ranges, Int32? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges, value).ReturnValue;
    }

    [Function(Name = Schema + "iArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedInt32Row> int32ArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateMethodCallQuery<RegularIndexedInt32Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlInt64 regular array methods

    [Function(Name = Schema + "biArrayCreateRegular", IsComposable = true)]
    public byte[] int64ArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "biArrayParseRegular", IsComposable = true)]
    public byte[] int64ArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "biArrayFormatRegular", IsComposable = true)]
    public String int64ArrayFormatRegular(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "biArrayRank", IsComposable = true)]
    public Int32? int64ArrayRank(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "biArrayFlatLength", IsComposable = true)]
    public Int32? int64ArrayFlatLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "biArrayDimLengths", IsComposable = true)]
    public byte[] int64ArrayDimLengths(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "biArrayDimLength", IsComposable = true)]
    public byte[] int64ArrayDimLength(byte[] array, Int32? index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "biArrayGetFlat", IsComposable = true)]
    public Int64? int64ArrayGetFlat(byte[] array, Int32? index)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "biArraySetFlat", IsComposable = true)]
    public byte[] int64ArraySetFlat(byte[] array, Int32? index, Int64? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "biArrayGetFlatRange", IsComposable = true)]
    public byte[] int64ArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "biArraySetFlatRange", IsComposable = true)]
    public byte[] int64ArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, range).ReturnValue;
    }

    [Function(Name = Schema + "biArrayFillFlatRange", IsComposable = true)]
    public byte[] int64ArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Int64? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "biArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedInt64Row> int64ArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<RegularIndexedInt64Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [Function(Name = Schema + "biArrayGetDim", IsComposable = true)]
    public Int64? int64ArrayGetDim(byte[] array, byte[] indices)
    {
      return (Int64?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices).ReturnValue;
    }

    [Function(Name = Schema + "biArraySetDim", IsComposable = true)]
    public byte[] int64ArraySetDim(byte[] array, byte[] indices, Int64? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, value).ReturnValue;
    }

    [Function(Name = Schema + "biArrayGetDimRange", IsComposable = true)]
    public byte[] int64ArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges).ReturnValue;
    }

    [Function(Name = Schema + "biArraySetDimRange", IsComposable = true)]
    public byte[] int64ArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, range).ReturnValue;
    }

    [Function(Name = Schema + "biArrayFillDimRange", IsComposable = true)]
    public byte[] int64ArrayFillDimRange(byte[] array, byte[] ranges, Int64? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges, value).ReturnValue;
    }

    [Function(Name = Schema + "biArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedInt64Row> int64ArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateMethodCallQuery<RegularIndexedInt64Row>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlSingle regular array methods

    [Function(Name = Schema + "sfArrayCreateRegular", IsComposable = true)]
    public byte[] sglArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayParseRegular", IsComposable = true)]
    public byte[] sglArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayFormatRegular", IsComposable = true)]
    public String sglArrayFormatRegular(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayRank", IsComposable = true)]
    public Int32? sglArrayRank(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayFlatLength", IsComposable = true)]
    public Int32? sglArrayFlatLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayDimLengths", IsComposable = true)]
    public byte[] sglArrayDimLengths(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayDimLength", IsComposable = true)]
    public byte[] sglArrayDimLength(byte[] array, Int32? index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayGetFlat", IsComposable = true)]
    public Single? sglArrayGetFlat(byte[] array, Int32? index)
    {
      return (Single?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "sfArraySetFlat", IsComposable = true)]
    public byte[] sglArraySetFlat(byte[] array, Int32? index, Single? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayGetFlatRange", IsComposable = true)]
    public byte[] sglArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "sfArraySetFlatRange", IsComposable = true)]
    public byte[] sglArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, range).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayFillFlatRange", IsComposable = true)]
    public byte[] sglArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Single? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedSingleRow> sglArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<RegularIndexedSingleRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [Function(Name = Schema + "sfArrayGetDim", IsComposable = true)]
    public Single? sglArrayGetDim(byte[] array, byte[] indices)
    {
      return (Single?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices).ReturnValue;
    }

    [Function(Name = Schema + "sfArraySetDim", IsComposable = true)]
    public byte[] sglArraySetDim(byte[] array, byte[] indices, Single? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, value).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayGetDimRange", IsComposable = true)]
    public byte[] sglArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges).ReturnValue;
    }

    [Function(Name = Schema + "sfArraySetDimRange", IsComposable = true)]
    public byte[] sglArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, range).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayFillDimRange", IsComposable = true)]
    public byte[] sglArrayFillDimRange(byte[] array, byte[] ranges, Single? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges, value).ReturnValue;
    }

    [Function(Name = Schema + "sfArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedSingleRow> sglArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateMethodCallQuery<RegularIndexedSingleRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlDouble regular array methods

    [Function(Name = Schema + "dfArrayCreateRegular", IsComposable = true)]
    public byte[] dblArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayParseRegular", IsComposable = true)]
    public byte[] dblArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayFormatRegular", IsComposable = true)]
    public String dblArrayFormatRegular(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayRank", IsComposable = true)]
    public Int32? dblArrayRank(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayFlatLength", IsComposable = true)]
    public Int32? dblArrayFlatLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayDimLengths", IsComposable = true)]
    public byte[] dblArrayDimLengths(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayDimLength", IsComposable = true)]
    public byte[] dblArrayDimLength(byte[] array, Int32? index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayGetFlat", IsComposable = true)]
    public Double? dblArrayGetFlat(byte[] array, Int32? index)
    {
      return (Double?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "dfArraySetFlat", IsComposable = true)]
    public byte[] dblArraySetFlat(byte[] array, Int32? index, Double? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayGetFlatRange", IsComposable = true)]
    public byte[] dblArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "dfArraySetFlatRange", IsComposable = true)]
    public byte[] dblArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, range).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayFillFlatRange", IsComposable = true)]
    public byte[] dblArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Double? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedDoubleRow> dblArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<RegularIndexedDoubleRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [Function(Name = Schema + "dfArrayGetDim", IsComposable = true)]
    public Double? dblArrayGetDim(byte[] array, byte[] indices)
    {
      return (Double?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices).ReturnValue;
    }

    [Function(Name = Schema + "dfArraySetDim", IsComposable = true)]
    public byte[] dblArraySetDim(byte[] array, byte[] indices, Double? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, value).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayGetDimRange", IsComposable = true)]
    public byte[] dblArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges).ReturnValue;
    }

    [Function(Name = Schema + "dfArraySetDimRange", IsComposable = true)]
    public byte[] dblArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, range).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayFillDimRange", IsComposable = true)]
    public byte[] dblArrayFillDimRange(byte[] array, byte[] ranges, Double? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges, value).ReturnValue;
    }

    [Function(Name = Schema + "dfArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedDoubleRow> dblArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateMethodCallQuery<RegularIndexedDoubleRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlDateTime regular array methods

    [Function(Name = Schema + "dtArrayCreateRegular", IsComposable = true)]
    public byte[] dtmArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayParseRegular", IsComposable = true)]
    public byte[] dtmArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayFormatRegular", IsComposable = true)]
    public String dtmArrayFormatRegular(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayRank", IsComposable = true)]
    public Int32? dtmArrayRank(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayFlatLength", IsComposable = true)]
    public Int32? dtmArrayFlatLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayDimLengths", IsComposable = true)]
    public byte[] dtmArrayDimLengths(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayDimLength", IsComposable = true)]
    public byte[] dtmArrayDimLength(byte[] array, Int32? index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayGetFlat", IsComposable = true)]
    public DateTime? dtmArrayGetFlat(byte[] array, Int32? index)
    {
      return (DateTime?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "dtArraySetFlat", IsComposable = true)]
    public byte[] dtmArraySetFlat(byte[] array, Int32? index, DateTime? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayGetFlatRange", IsComposable = true)]
    public byte[] dtmArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "dtArraySetFlatRange", IsComposable = true)]
    public byte[] dtmArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, range).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayFillFlatRange", IsComposable = true)]
    public byte[] dtmArrayFillFlatRange(byte[] array, Int32? index, Int32? count, DateTime? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedDateTimeRow> dtmArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<RegularIndexedDateTimeRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [Function(Name = Schema + "dtArrayGetDim", IsComposable = true)]
    public DateTime? dtmArrayGetDim(byte[] array, byte[] indices)
    {
      return (DateTime?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices).ReturnValue;
    }

    [Function(Name = Schema + "dtArraySetDim", IsComposable = true)]
    public byte[] dtmArraySetDim(byte[] array, byte[] indices, DateTime? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, value).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayGetDimRange", IsComposable = true)]
    public byte[] dtmArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges).ReturnValue;
    }

    [Function(Name = Schema + "dtArraySetDimRange", IsComposable = true)]
    public byte[] dtmArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, range).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayFillDimRange", IsComposable = true)]
    public byte[] dtmArrayFillDimRange(byte[] array, byte[] ranges, DateTime? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges, value).ReturnValue;
    }

    [Function(Name = Schema + "dtArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedDateTimeRow> dtmArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateMethodCallQuery<RegularIndexedDateTimeRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlGuid regular array methods

    [Function(Name = Schema + "uidArrayCreateRegular", IsComposable = true)]
    public byte[] guidArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayParseRegular", IsComposable = true)]
    public byte[] guidArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, compact).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayFormatRegular", IsComposable = true)]
    public String guidArrayFormatRegular(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayRank", IsComposable = true)]
    public Int32? guidArrayRank(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayFlatLength", IsComposable = true)]
    public Int32? guidArrayFlatLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayDimLengths", IsComposable = true)]
    public byte[] guidArrayDimLengths(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayDimLength", IsComposable = true)]
    public byte[] guidArrayDimLength(byte[] array, Int32? index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayGetFlat", IsComposable = true)]
    public Guid? guidArrayGetFlat(byte[] array, Int32? index)
    {
      return (Guid?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "uidArraySetFlat", IsComposable = true)]
    public byte[] guidArraySetFlat(byte[] array, Int32? index, Guid? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayGetFlatRange", IsComposable = true)]
    public byte[] guidArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "uidArraySetFlatRange", IsComposable = true)]
    public byte[] guidArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, range).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayFillFlatRange", IsComposable = true)]
    public byte[] guidArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Guid? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedGuidRow> guidArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<RegularIndexedGuidRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [Function(Name = Schema + "uidArrayGetDim", IsComposable = true)]
    public Guid? guidArrayGetDim(byte[] array, byte[] indices)
    {
      return (Guid?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices).ReturnValue;
    }

    [Function(Name = Schema + "uidArraySetDim", IsComposable = true)]
    public byte[] guidArraySetDim(byte[] array, byte[] indices, Guid? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, value).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayGetDimRange", IsComposable = true)]
    public byte[] guidArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges).ReturnValue;
    }

    [Function(Name = Schema + "uidArraySetDimRange", IsComposable = true)]
    public byte[] guidArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, range).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayFillDimRange", IsComposable = true)]
    public byte[] guidArrayFillDimRange(byte[] array, byte[] ranges, Guid? value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges, value).ReturnValue;
    }

    [Function(Name = Schema + "uidArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedGuidRow> guidArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateMethodCallQuery<RegularIndexedGuidRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlString regular array methods

    [Function(Name = Schema + "strArrayCreateRegular", IsComposable = true)]
    public byte[] strArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing, itemSizing).ReturnValue;
    }

    [Function(Name = Schema + "strArrayCreateRegularByCpId", IsComposable = true)]
    public byte[] strArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, SizeEncoding itemSizing, Int32? cpId)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing, itemSizing, cpId).ReturnValue;
    }

    [Function(Name = Schema + "strArrayCreateRegularByCpName", IsComposable = true)]
    public byte[] strArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, SizeEncoding itemSizing, String cpName)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing, itemSizing, cpName).ReturnValue;
    }

    [Function(Name = Schema + "strArrayParseRegular", IsComposable = true)]
    public byte[] strArrayParseRegular(String str, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing).ReturnValue;
    }

    [Function(Name = Schema + "strArrayParseRegularByCpId", IsComposable = true)]
    public byte[] strArrayParseRegular(String str, SizeEncoding countSizing, SizeEncoding itemSizing, Int32? cpId)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing, cpId).ReturnValue;
    }

    [Function(Name = Schema + "strArrayParseRegularByCpName", IsComposable = true)]
    public byte[] strArrayParseRegular(String str, SizeEncoding countSizing, SizeEncoding itemSizing, String cpName)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing, cpName).ReturnValue;
    }

    [Function(Name = Schema + "strArrayFormatRegular", IsComposable = true)]
    public String strArrayFormatRegular(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "strArrayRank", IsComposable = true)]
    public Int32? strArrayRank(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "strArrayFlatLength", IsComposable = true)]
    public Int32? strArrayFlatLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "strArrayDimLengths", IsComposable = true)]
    public byte[] strArrayDimLengths(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "strArrayDimLength", IsComposable = true)]
    public byte[] strArrayDimLength(byte[] array, Int32? index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "strArrayGetFlat", IsComposable = true)]
    public String strArrayGetFlat(byte[] array, Int32? index)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "strArraySetFlat", IsComposable = true)]
    public byte[] strArraySetFlat(byte[] array, Int32? index, String value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "strArrayGetFlatRange", IsComposable = true)]
    public byte[] strArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "strArraySetFlatRange", IsComposable = true)]
    public byte[] strArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, range).ReturnValue;
    }

    [Function(Name = Schema + "strArrayFillFlatRange", IsComposable = true)]
    public byte[] strArrayFillFlatRange(byte[] array, Int32? index, Int32? count, String value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "strArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedStringRow> strArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<RegularIndexedStringRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [Function(Name = Schema + "strArrayGetDim", IsComposable = true)]
    public String strArrayGetDim(byte[] array, byte[] indices)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices).ReturnValue;
    }

    [Function(Name = Schema + "strArraySetDim", IsComposable = true)]
    public byte[] strArraySetDim(byte[] array, byte[] indices, String value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, value).ReturnValue;
    }

    [Function(Name = Schema + "strArrayGetDimRange", IsComposable = true)]
    public byte[] strArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges).ReturnValue;
    }

    [Function(Name = Schema + "strArraySetDimRange", IsComposable = true)]
    public byte[] strArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, range).ReturnValue;
    }

    [Function(Name = Schema + "strArrayFillDimRange", IsComposable = true)]
    public byte[] strArrayFillDimRange(byte[] array, byte[] ranges, String value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges, value).ReturnValue;
    }

    [Function(Name = Schema + "strArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedStringRow> strArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateMethodCallQuery<RegularIndexedStringRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlBinary regular array methods

    [Function(Name = Schema + "binArrayCreateRegular", IsComposable = true)]
    public byte[] binArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), lengths, countSizing, itemSizing).ReturnValue;
    }

    [Function(Name = Schema + "binArrayParseRegular", IsComposable = true)]
    public byte[] binArrayParseRegular(String str, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing).ReturnValue;
    }

    [Function(Name = Schema + "binArrayFormatRegular", IsComposable = true)]
    public String binArrayFormatRegular(byte[] array)
    {
      return (String)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "binArrayRank", IsComposable = true)]
    public Int32? binArrayRank(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "binArrayFlatLength", IsComposable = true)]
    public Int32? binArrayFlatLength(byte[] array)
    {
      return (Int32?)ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "binArrayDimLengths", IsComposable = true)]
    public byte[] binArrayDimLengths(byte[] array)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array).ReturnValue;
    }

    [Function(Name = Schema + "binArrayDimLength", IsComposable = true)]
    public byte[] binArrayDimLength(byte[] array, Int32? index)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "binArrayGetFlat", IsComposable = true)]
    public Byte[] binArrayGetFlat(byte[] array, Int32? index)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index).ReturnValue;
    }

    [Function(Name = Schema + "binArraySetFlat", IsComposable = true)]
    public byte[] binArraySetFlat(byte[] array, Int32? index, Byte[] value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, value).ReturnValue;
    }

    [Function(Name = Schema + "binArrayGetFlatRange", IsComposable = true)]
    public byte[] binArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count).ReturnValue;
    }

    [Function(Name = Schema + "binArraySetFlatRange", IsComposable = true)]
    public byte[] binArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, range).ReturnValue;
    }

    [Function(Name = Schema + "binArrayFillFlatRange", IsComposable = true)]
    public byte[] binArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Byte[] value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count, value).ReturnValue;
    }

    [Function(Name = Schema + "binArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedBinaryRow> binArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateMethodCallQuery<RegularIndexedBinaryRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [Function(Name = Schema + "binArrayGetDim", IsComposable = true)]
    public Byte[] binArrayGetDim(byte[] array, byte[] indices)
    {
      return (Byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices).ReturnValue;
    }

    [Function(Name = Schema + "binArraySetDim", IsComposable = true)]
    public byte[] binArraySetDim(byte[] array, byte[] indices, Byte[] value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, value).ReturnValue;
    }

    [Function(Name = Schema + "binArrayGetDimRange", IsComposable = true)]
    public byte[] binArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges).ReturnValue;
    }

    [Function(Name = Schema + "binArraySetDimRange", IsComposable = true)]
    public byte[] binArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, indices, range).ReturnValue;
    }

    [Function(Name = Schema + "binArrayFillDimRange", IsComposable = true)]
    public byte[] binArrayFillDimRange(byte[] array, byte[] ranges, Byte[] value)
    {
      return (byte[])ExecuteMethodCall(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges, value).ReturnValue;
    }

    [Function(Name = Schema + "binArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedBinaryRow> binArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateMethodCallQuery<RegularIndexedBinaryRow>(this, (MethodInfo)MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #endregion
    #endregion
  }
}
