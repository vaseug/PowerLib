using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Data.Entity;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using PowerLib.System.ComponentModel.DataAnnotations;
using PowerLib.System.IO;

namespace PowerLib.System.Data.Entity
{
  public static class PwrDbFunctions
  {
    private const string ContextNamespace = "PwrDbContext";
    private const string StoreNamespace = "PwrDbStore";

    #region Registration methods

    public static void InitializeModel(DbModelBuilder modelBuilder)
    {
      modelBuilder.ComplexType<RegexMatchRow>();
      modelBuilder.ComplexType<ZipArchiveEntryRow>();
      modelBuilder.ComplexType<BinaryRow>();
      modelBuilder.ComplexType<StringRow>();
      modelBuilder.ComplexType<BooleanRow>();
      modelBuilder.ComplexType<ByteRow>();
      modelBuilder.ComplexType<Int16Row>();
      modelBuilder.ComplexType<Int32Row>();
      modelBuilder.ComplexType<Int64Row>();
      modelBuilder.ComplexType<SingleRow>();
      modelBuilder.ComplexType<DoubleRow>();
      modelBuilder.ComplexType<DecimalRow>();
      modelBuilder.ComplexType<DateTimeRow>();
      modelBuilder.ComplexType<GuidRow>();
      modelBuilder.ComplexType<IndexedBinaryRow>();
      modelBuilder.ComplexType<IndexedStringRow>();
      modelBuilder.ComplexType<IndexedBooleanRow>();
      modelBuilder.ComplexType<IndexedByteRow>();
      modelBuilder.ComplexType<IndexedInt16Row>();
      modelBuilder.ComplexType<IndexedInt32Row>();
      modelBuilder.ComplexType<IndexedInt64Row>();
      modelBuilder.ComplexType<IndexedSingleRow>();
      modelBuilder.ComplexType<IndexedDoubleRow>();
      modelBuilder.ComplexType<IndexedDecimalRow>();
      modelBuilder.ComplexType<IndexedDateTimeRow>();
      modelBuilder.ComplexType<IndexedGuidRow>();
      modelBuilder.ComplexType<RegularIndexedBinaryRow>();
      modelBuilder.ComplexType<RegularIndexedStringRow>();
      modelBuilder.ComplexType<RegularIndexedBooleanRow>();
      modelBuilder.ComplexType<RegularIndexedByteRow>();
      modelBuilder.ComplexType<RegularIndexedInt16Row>();
      modelBuilder.ComplexType<RegularIndexedInt32Row>();
      modelBuilder.ComplexType<RegularIndexedInt64Row>();
      modelBuilder.ComplexType<RegularIndexedSingleRow>();
      modelBuilder.ComplexType<RegularIndexedDoubleRow>();
      modelBuilder.ComplexType<RegularIndexedDecimalRow>();
      modelBuilder.ComplexType<RegularIndexedDateTimeRow>();
      modelBuilder.ComplexType<RegularIndexedGuidRow>();
      modelBuilder.Conventions.Add(new FunctionsConvention(typeof(PwrDbFunctions), "pwrlib", "value", StoreNamespace));
    }

    #endregion
    #region Internal methods

    private static T ExecuteUnsupported<T>(MethodBase method, params object[] args)
    {
      DbFunctionAttribute attrFunction = method.GetCustomAttribute<DbFunctionAttribute>();
      if (attrFunction == null)
        throw new InvalidOperationException("Method must be marked by'DbFunction' attribute.");
      throw new NotSupportedException(string.Format("Function {0} execution is not supported in runtime.", attrFunction.FunctionName));
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
    [DbFunctionEx(StoreNamespace, "regexIsMatch", IsComposable = true)]
    public static Boolean? regexIsMatch(this String input, String pattern, RegexOptions options)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, pattern, options);
    }

    /// <summary>
    /// In a specified input string, replaces all strings that match a specified regular expression with a specified replacement string. Specified options modify the matching operation.
    /// </summary>
    /// <param name="input">The string to search for a match.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="replacement">The replacement string.</param>
    /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
    /// <returns>A new string that is identical to the input string, except that the replacement string takes the place of each matched string. If pattern is not matched in the current instance, the method returns the current instance unchanged.</returns>
    [DbFunctionEx(StoreNamespace, "regexReplace", IsComposable = true)]
    public static String regexReplace(this String input, String pattern, String replacement, RegexOptions options)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, pattern, replacement, options);
    }

    /// <summary>
    /// Escapes a minimal set of characters (\, *, +, ?, |, {, [, (,), ^, $,., #, and white space) by replacing them with their escape codes. This instructs the regular expression engine to interpret these characters literally rather than as metacharacters.
    /// </summary>
    /// <param name="input">The input string that contains the text to convert.</param>
    /// <returns>A string of characters with metacharacters converted to their escaped form.</returns>
    [DbFunctionEx(StoreNamespace, "regexEscape", IsComposable = true)]
    public static String regexEscape(this String input)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Converts any escaped characters in the input string.
    /// </summary>
    /// <param name="input">The input string containing the text to convert.</param>
    /// <returns>A string of characters with any escaped characters converted to their unescaped form.</returns>
    [DbFunctionEx(StoreNamespace, "regexUnescape", IsComposable = true)]
    public static String regexUnescape(this String input)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Splits an input string into an array of substrings at the positions defined by a specified regular expression pattern. Specified options modify the matching operation.
    /// </summary>
    /// <param name="input">The string to split.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
    /// <returns></returns>
    [DbFunctionEx(ContextNamespace, "regexSplit", IsComposable = true)]
    public static IQueryable<StringRow> regexSplit(this String input, String pattern, RegexOptions options)
    {
      return ExecuteUnsupported<IQueryable<StringRow>>(MethodBase.GetCurrentMethod(), input, pattern, options);
    }

    /// <summary>
    /// Searches the specified input string for all occurrences of a specified regular expression, using the specified matching options.
    /// </summary>
    /// <param name="input">The string to search for a match.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specify options for matching.</param>
    /// <returns>A collection of the RegexMatchRow objects found by the search. If no matches are found, the method returns an empty collection object.</returns>
    [DbFunctionEx(ContextNamespace, "regexMatches", IsComposable = true)]
    public static IQueryable<RegexMatchRow> regexMatches(this String input, String pattern, RegexOptions options)
    {
      return ExecuteUnsupported<IQueryable<RegexMatchRow>>(MethodBase.GetCurrentMethod(), input, pattern, options);
    }

    #endregion
    #region String functions
    #region String manipulation functions

    [DbFunctionEx(StoreNamespace, "strInsert", IsComposable = true)]
    public static String strInsert(this String input, Int32? index, String value)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "strRemove", IsComposable = true)]
    public static String strRemove(this String input, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strReplace", IsComposable = true)]
    public static String strReplace(this String input, String pattern, String replacement)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, pattern, replacement);
    }

    [DbFunctionEx(StoreNamespace, "strSubstring", IsComposable = true)]
    public static String strSubstring(this String input, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strReverse", IsComposable = true)]
    public static String strReverse(this String input)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strReplicate", IsComposable = true)]
    public static String strReplicate(this String input, Int32 count)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, count);
    }

    [DbFunctionEx(StoreNamespace, "strPadLeft", IsComposable = true)]
    public static String strPadLeft(this String input, String padding, Int32 width)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, padding, width);
    }

    [DbFunctionEx(StoreNamespace, "strPadRight", IsComposable = true)]
    public static String strPadRight(this String input, String padding, Int32 width)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, padding, width);
    }

    [DbFunctionEx(StoreNamespace, "strCutLeft", IsComposable = true)]
    public static String strCutLeft(this String input, Int32 width)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, width);
    }

    [DbFunctionEx(StoreNamespace, "strCutRight", IsComposable = true)]
    public static String strCutRight(this String input, Int32 width)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, width);
    }

    [DbFunctionEx(StoreNamespace, "strTrim", IsComposable = true)]
    public static String strTrim(this String input, String trimming)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, trimming);
    }

    [DbFunctionEx(StoreNamespace, "strTrimLeft", IsComposable = true)]
    public static String strTrimLeft(this String input, String trimming)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, trimming);
    }

    [DbFunctionEx(StoreNamespace, "strTrimRight", IsComposable = true)]
    public static String strTrimRight(this String input, String trimming)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, trimming);
    }

    [DbFunctionEx(StoreNamespace, "strQuote", IsComposable = true)]
    public static String strQuote(this String input, [FunctionParameter(TypeName = "nchar"), MaxLength(1), MinLength(1)] String quote, [FunctionParameter(TypeName = "nchar"), MaxLength(1), MinLength(1)] String escape)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, quote, escape);
    }

    [DbFunctionEx(StoreNamespace, "strUnquote", IsComposable = true)]
    public static String strUnquote(this String input, [FunctionParameter(TypeName = "nchar"), MaxLength(1), MinLength(1)] String quote, [FunctionParameter(TypeName = "nchar"), MaxLength(1), MinLength(1)] String escape)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, quote, escape);
    }

    [DbFunctionEx(StoreNamespace, "strEscape", IsComposable = true)]
    public static String strEscape(this String input, [FunctionParameter(TypeName = "nchar"), MaxLength(1), MinLength(1)] String escape, String symbols)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, escape, symbols);
    }

    [DbFunctionEx(StoreNamespace, "strUnescape", IsComposable = true)]
    public static String strUnescape(this String input, [FunctionParameter(TypeName = "nchar"), MaxLength(1), MinLength(1)] String escape, String symbols)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, escape, symbols);
    }

    #endregion
    #region String convert functions

    [DbFunctionEx(StoreNamespace, "strToLower", IsComposable = true)]
    public static String strToLower(this String input)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strToLowerByLcId", IsComposable = true)]
    public static String strToLower(this String input, Int32? lcId)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strToLowerByLcName", IsComposable = true)]
    public static String strToLower(this String input, String lcName)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strToUpper", IsComposable = true)]
    public static String strToUpper(this String input)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strToUpperByLcId", IsComposable = true)]
    public static String strToUpper(this String input, Int32? lcId)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strToUpperByLcName", IsComposable = true)]
    public static String strToUpper(this String input, String lcName)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, lcName);
    }

    #endregion
    #region String retrieve functions

    [DbFunctionEx(StoreNamespace, "strLength", IsComposable = true)]
    public static Int32? strLength(this String input)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOf", IsComposable = true)]
    public static Int32? strIndexOf(this String input, String value, CompareOptions compareOptions)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, value, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfRange", IsComposable = true)]
    public static Int32? strIndexOf(this String input, String value, Int32? index, Int32? count, CompareOptions compareOptions)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, value, index, count, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfByLcId", IsComposable = true)]
    public static Int32? strIndexOf(this String input, String value, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfRangeByLcId", IsComposable = true)]
    public static Int32? strIndexOf(this String input, String value, Int32? index, Int32? count, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, value, index, count, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfByLcName", IsComposable = true)]
    public static Int32? strIndexOf(this String input, String value, CompareOptions compareOptions, String lcName)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfRangeByLcName", IsComposable = true)]
    public static Int32? strIndexOf(this String input, String value, Int32? index, Int32? count, CompareOptions compareOptions, String lcName)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, value, index, count, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOf", IsComposable = true)]
    public static Int32? strLastIndexOf(this String input, String value, CompareOptions compareOptions)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, value, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfRange", IsComposable = true)]
    public static Int32? strLastIndexOf(this String input, String value, Int32? index, Int32? count, CompareOptions compareOptions)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, value, index, count, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfByLcId", IsComposable = true)]
    public static Int32? strLastIndexOf(this String input, String value, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfRangeByLcId", IsComposable = true)]
    public static Int32? strLastIndexOf(this String input, String value, Int32? index, Int32? count, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, value, index, count, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfByLcName", IsComposable = true)]
    public static Int32? strLastIndexOf(this String input, String value, CompareOptions compareOptions, String lcName)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfRangeByLcName", IsComposable = true)]
    public static Int32? strLastIndexOf(this String input, String value, Int32? index, Int32? count, CompareOptions compareOptions, String lcName)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, value, index, count, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strContains", IsComposable = true)]
    public static Boolean? strContains(this String input, String value, CompareOptions compareOptions)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strContainsByLcId", IsComposable = true)]
    public static Boolean? strContains(this String input, String value, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strContainsByLcName", IsComposable = true)]
    public static Boolean? strContains(this String input, String value, CompareOptions compareOptions, String lcName)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strStartsWith", IsComposable = true)]
    public static Boolean? strStartsWith(this String input, String value, CompareOptions compareOptions)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strStartsWithByLcId", IsComposable = true)]
    public static Boolean? strStartsWith(this String input, String value, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strStartsWithByLcName", IsComposable = true)]
    public static Boolean? strStartsWith(this String input, String value, CompareOptions compareOptions, String lcName)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strEndsWith", IsComposable = true)]
    public static Boolean? strEndsWith(this String input, String value, CompareOptions compareOptions)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strEndsWithByLcId", IsComposable = true)]
    public static Boolean? strEndsWith(this String input, String value, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strEndsWithByLcName", IsComposable = true)]
    public static Boolean? strEndsWith(this String input, String value, CompareOptions compareOptions, String lcName)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfAny", IsComposable = true)]
    public static Int32? strIndexOfAny(this String input, String anyOf)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, anyOf);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfAnyRange", IsComposable = true)]
    public static Int32? strIndexOfAny(this String input, String anyOf, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, anyOf, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfAny", IsComposable = true)]
    public static Int32? strLastIndexOfAny(this String input, String anyOf)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, anyOf);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfAnyRange", IsComposable = true)]
    public static Int32? strLastIndexOfAny(this String input, String anyOf, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, anyOf, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfExcept", IsComposable = true)]
    public static Int32? strIndexOfExcept(this String input, String exceptOf)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, exceptOf);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfExceptRange", IsComposable = true)]
    public static Int32? strIndexOfExcept(this String input, String exceptOf, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, exceptOf, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfExcept", IsComposable = true)]
    public static Int32? strLastIndexOfExcept(this String input, String exceptOf)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, exceptOf);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfExceptRange", IsComposable = true)]
    public static Int32? strLastIndexOfExcept(this String input, String exceptOf, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, exceptOf, index, count);
    }

    #endregion
    #region String comparison functons

    [DbFunctionEx(StoreNamespace, "strCompare", IsComposable = true)]
    public static Int32? strCompare(String value1, String value2, CompareOptions compareOptions)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), value1, value2, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strCompareRange", IsComposable = true)]
    public static Int32? strCompare(String value1, Int32? index1, Int32? count1, String value2, Int32? index2, Int32? count2, CompareOptions compareOptions)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strCompareByLcId", IsComposable = true)]
    public static Int32? strCompare(String value1, String value2, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), value1, value2, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strCompareRangeByLcId", IsComposable = true)]
    public static Int32? strCompare(String value1, Int32? index1, Int32? count1, String value2, Int32? index2, Int32? count2, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strCompareByLcName", IsComposable = true)]
    public static Int32? strCompare(String value1, String value2, CompareOptions compareOptions, String lcName)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), value1, value2, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strCompareRangeByLcName", IsComposable = true)]
    public static Int32? strCompare(String value1, Int32? index1, Int32? count1, String value2, Int32? index2, Int32? count2, CompareOptions compareOptions, String lcName)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strEqual", IsComposable = true)]
    public static Boolean? strEqual(String value1, String value2, CompareOptions compareOptions)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), value1, value2, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strEqualRange", IsComposable = true)]
    public static Boolean? strEqual(String value1, Int32? index1, Int32? count1, String value2, Int32? index2, Int32? count2, CompareOptions compareOptions)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strEqualByLcId", IsComposable = true)]
    public static Boolean? strEqual(String value1, String value2, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), value1, value2, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strEqualRangeByLcId", IsComposable = true)]
    public static Boolean? strEqual(String value1, Int32? index1, Int32? count1, String value2, Int32? index2, Int32? count2, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strEqualByLcName", IsComposable = true)]
    public static Boolean? strEqual(String value1, String value2, CompareOptions compareOptions, String lcName)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), value1, value2, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strEqualRangeByLcName", IsComposable = true)]
    public static Boolean? strEqual(String value1, Int32? index1, Int32? count1, String value2, Int32? index2, Int32? count2, CompareOptions compareOptions, String lcName)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions, lcName);
    }

    #endregion
    #region String split functions

    [DbFunctionEx(ContextNamespace, "strSplitToChars", IsComposable = true)]
    public static IQueryable<StringRow> strSplitToChars(this String input, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<StringRow>>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(ContextNamespace, "strSplitByChars", IsComposable = true)]
    public static IQueryable<StringRow> strSplitByChars(this String input, String delimitChars, Int32? count, StringSplitOptions options)
    {
      return ExecuteUnsupported<IQueryable<StringRow>>(MethodBase.GetCurrentMethod(), input, delimitChars, count, options);
    }

    [DbFunctionEx(ContextNamespace, "strSplitByWords", IsComposable = true)]
    public static IQueryable<StringRow> strSplitByWords(this String input, String separators, String delimitChars, Int32? count, StringSplitOptions options)
    {
      return ExecuteUnsupported<IQueryable<StringRow>>(MethodBase.GetCurrentMethod(), input, separators, delimitChars, count, options);
    }

    [DbFunctionEx(ContextNamespace, "strSplit", IsComposable = true)]
    public static IQueryable<StringRow> strSplit(this String input, String separator, Int32? count, StringSplitOptions options)
    {
      return ExecuteUnsupported<IQueryable<StringRow>>(MethodBase.GetCurrentMethod(), input, separator, count, options);
    }

    [DbFunctionEx(ContextNamespace, "strSplitSmart", IsComposable = true)]
    public static IQueryable<StringRow> strSplit(this String input, String separators, String trims, String controlSeparators, String controlEscapes, String controls, Int32? count, StringSplitOptionsEx options)
    {
      return ExecuteUnsupported<IQueryable<StringRow>>(MethodBase.GetCurrentMethod(), input, separators, trims, controlSeparators, controlEscapes, controls, count, options);
    }

    #endregion
    #region String aggregate functions

    [DbFunctionEx(StoreNamespace, "strConcat", IsComposable = true, IsAggregate = true)]
    public static String strConcat(String value)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "strJoin", IsComposable = true, IsAggregate = true)]
    public static String strJoin(String value, String delimiter)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), value, delimiter);
    }

    #endregion
    #endregion
    #region Binary functions
    #region Binary manipulation functions

    [DbFunctionEx(StoreNamespace, "binInsert", IsComposable = true)]
    public static Byte[] binInsert(this Byte[] input, Int64? index, Byte[] value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "binRemove", IsComposable = true)]
    public static Byte[] binRemove(this Byte[] input, Int64? index, Int64? count)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binReplicate", IsComposable = true)]
    public static Byte[] binReplicate(this Byte[] input, Int64 count)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, count);
    }

    [DbFunctionEx(StoreNamespace, "binRange", IsComposable = true)]
    public static Byte[] binRange(this Byte[] input, Int64? index, Int64? count)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binReverse", IsComposable = true)]
    public static Byte[] binReverse(this Byte[] input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binReverseRange", IsComposable = true)]
    public static Byte[] binReverse(this Byte[] input, Int64? index, Int64? count)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binReplace", IsComposable = true)]
    public static Byte[] binReplace(this Byte[] input, Byte[] value, Byte[] replacement)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, value, replacement);
    }

    [DbFunctionEx(StoreNamespace, "binReplaceRange", IsComposable = true)]
    public static Byte[] binReplace(this Byte[] input, Byte[] value, Byte[] replacement, Int64? index, Int64? count)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, value, replacement, index, count);
    }

    #endregion
    #region Binary convert functions

    [DbFunctionEx(StoreNamespace, "binToString", IsComposable = true)]
    public static String binToString(this Byte[] input, Int64? index, Int64? count)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binToStringByCpId", IsComposable = true)]
    public static String binToString(this Byte[] input, Int64? index, Int64? count, Int32? cpId)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, index, count, cpId);
    }

    [DbFunctionEx(StoreNamespace, "binToStringByCpName", IsComposable = true)]
    public static String binToString(this Byte[] input, Int64? index, Int64? count, String cpName)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, index, count, cpName);
    }

    [DbFunctionEx(StoreNamespace, "binToSmallInt", IsComposable = true)]
    public static Int16? binToInt16(this Byte[] input, Int64? index)
    {
      return ExecuteUnsupported<Int16?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binToInt", IsComposable = true)]
    public static Int32? binToInt32(this Byte[] input, Int64? index)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binToBigInt", IsComposable = true)]
    public static Int64? binToInt64(this Byte[] input, Int64? index)
    {
      return ExecuteUnsupported<Int64?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binToSingle", IsComposable = true)]
    public static Single? binToSingle(this Byte[] input, Int64? index)
    {
      return ExecuteUnsupported<Single?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binToDouble", IsComposable = true)]
    public static Double? binToDouble(this Byte[] input, Int64? index)
    {
      return ExecuteUnsupported<Double?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binToDateTime", IsComposable = true)]
    public static DateTime? binToDateTime(Byte[] input, Int64? index)
    {
      return ExecuteUnsupported<DateTime?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binToUid", IsComposable = true)]
    public static Guid? binToGuid(Byte[] input, Int64? index)
    {
      return ExecuteUnsupported<Guid?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binFromString", IsComposable = true)]
    public static Byte[] binFromString(this String input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromStringByCpId", IsComposable = true)]
    public static Byte[] binFromString(this String input, Int32? cpId)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromStringByCpName", IsComposable = true)]
    public static Byte[] binFromString(this String input, String cpName)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromBase64String", IsComposable = true)]
    public static Byte[] binFromBase64String(this String input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromTinyInt", IsComposable = true)]
    public static Byte[] binFromByte(this Byte? input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromSmallInt", IsComposable = true)]
    public static Byte[] binFromInt16(this Int16? input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromInt", IsComposable = true)]
    public static Byte[] binFromInt32(Int32? input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromBigInt", IsComposable = true)]
    public static Byte[] binFromInt64(Int64? input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromSingleFloat", IsComposable = true)]
    public static Byte[] binFromSingle(this Single? input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromDoubleFloat", IsComposable = true)]
    public static Byte[] binFromDouble(this Double? input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromDateTime", IsComposable = true)]
    public static Byte[] binFromDateTime(this DateTime? input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromUid", IsComposable = true)]
    public static Byte[] binFromGuid(this Guid? input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    #endregion
    #region Binary retrieve functions

    [DbFunctionEx(StoreNamespace, "binLength", IsComposable = true)]
    public static Int64? binLength(Byte[] input)
    {
      return ExecuteUnsupported<Int64?>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binIndexOf", IsComposable = true)]
    public static Int64? binIndexOf(Byte[] input, Byte[] value)
    {
      return ExecuteUnsupported<Int64?>(MethodBase.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "binIndexOfRange", IsComposable = true)]
    public static Int64? binIndexOf(Byte[] input, Byte[] value, Int64? index, Int64? count)
    {
      return ExecuteUnsupported<Int64?>(MethodBase.GetCurrentMethod(), input, value, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binLastIndexOf", IsComposable = true)]
    public static Int64? binLastIndexOf(Byte[] input, Byte[] value)
    {
      return ExecuteUnsupported<Int64?>(MethodBase.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "binLastIndexOfRange", IsComposable = true)]
    public static Int64? binLastIndexOf(Byte[] input, Byte[] value, Int64? index, Int64? count)
    {
      return ExecuteUnsupported<Int64?>(MethodBase.GetCurrentMethod(), input, value, index, count);
    }

    #endregion
    #region Binary comparison functions

    [DbFunctionEx(StoreNamespace, "binCompare", IsComposable = true)]
    public static Int64? binCompare(Byte[] xValue, Byte[] yValue)
    {
      return ExecuteUnsupported<Int64?>(MethodBase.GetCurrentMethod(), xValue, yValue);
    }

    [DbFunctionEx(StoreNamespace, "binCompareRange", IsComposable = true)]
    public static Int64? binCompare(Byte[] xValue, Int64? xIndex, Byte[] yValue, Int64? yIndex, Int64? count)
    {
      return ExecuteUnsupported<Int64?>(MethodBase.GetCurrentMethod(), xValue, xIndex, yValue, yIndex, count);
    }

    [DbFunctionEx(StoreNamespace, "binEqual", IsComposable = true)]
    public static Boolean? binEqual(Byte[] xValue, Byte[] yValue)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), xValue, yValue);
    }

    [DbFunctionEx(StoreNamespace, "binEqualRange", IsComposable = true)]
    public static Boolean? binEqual(Byte[] xValue, Int64? xIndex, Byte[] yValue, Int64? yIndex, Int64? count)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), xValue, xIndex, yValue, yIndex, count);
    }

    #endregion
    #region Binary split functions

    [DbFunctionEx(ContextNamespace, "binSplitToBit", IsComposable = true)]
    public static IQueryable<BooleanRow> binSplitToBoolean(this Byte[] input)
    {
      return ExecuteUnsupported<IQueryable<BooleanRow>>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToBinary", IsComposable = true)]
    public static IQueryable<BinaryRow> binSplitToBinary(this Byte[] input)
    {
      return ExecuteUnsupported<IQueryable<BinaryRow>>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToString", IsComposable = true)]
    public static IQueryable<StringRow> binSplitToString(this Byte[] input)
    {
      return ExecuteUnsupported<IQueryable<StringRow>>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToTinyint", IsComposable = true)]
    public static IQueryable<ByteRow> binSplitToByte(this Byte[] input)
    {
      return ExecuteUnsupported<IQueryable<ByteRow>>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToSmallInt", IsComposable = true)]
    public static IQueryable<Int16Row> binSplitToInt16(this Byte[] input)
    {
      return ExecuteUnsupported<IQueryable<Int16Row>>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToInt", IsComposable = true)]
    public static IQueryable<Int32Row> binSplitToInt32(this Byte[] input)
    {
      return ExecuteUnsupported<IQueryable<Int32Row>>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToBigInt", IsComposable = true)]
    public static IQueryable<Int64Row> binSplitToInt64(this Byte[] input)
    {
      return ExecuteUnsupported<IQueryable<Int64Row>>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToSingle", IsComposable = true)]
    public static IQueryable<SingleRow> binSplitToSingle(this Byte[] input)
    {
      return ExecuteUnsupported<IQueryable<SingleRow>>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToDouble", IsComposable = true)]
    public static IQueryable<DoubleRow> binSplitToDouble(this Byte[] input)
    {
      return ExecuteUnsupported<IQueryable<DoubleRow>>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToDateTime", IsComposable = true)]
    public static IQueryable<DateTimeRow> binSplitToDateTime(this Byte[] input)
    {
      return ExecuteUnsupported<IQueryable<DateTimeRow>>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToUid", IsComposable = true)]
    public static IQueryable<GuidRow> binSplitToGuid(this Byte[] input)
    {
      return ExecuteUnsupported<IQueryable<GuidRow>>(MethodBase.GetCurrentMethod(), input);
    }

    #endregion
    #region Binary aggregate functions

    [DbFunctionEx(StoreNamespace, "binConcat", IsComposable = true, IsAggregate = true)]
    public static Byte[] binConcat(Byte[] value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
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
    [DbFunctionEx(StoreNamespace, "xmlEvaluate", IsComposable = true)]
    public static String xmlEvaluate(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns String value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as String.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsString", IsComposable = true)]
    public static String xmlEvaluateAsString(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Boolean value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Boolean.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsBit", IsComposable = true)]
    public static Boolean? xmlEvaluateAsBoolean(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Byte value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Byte.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsTinyInt", IsComposable = true)]
    public static Byte? xmlEvaluateAsByte(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<Byte?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Int16 value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Int16.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsSmallInt", IsComposable = true)]
    public static Int16? xmlEvaluateAsInt16(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<Int16?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Int32 value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="path">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsMap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Int32.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsInt", IsComposable = true)]
    public static Int32? xmlEvaluateAsInt32(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<Int32?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Int64 value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Int64.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsBigInt", IsComposable = true)]
    public static Int64? xmlEvaluateAsInt64(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<Int64?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Single value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Single.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsSingle", IsComposable = true)]
    public static Single? xmlEvaluateAsSingle(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<Single?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Double value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Double.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsDouble", IsComposable = true)]
    public static Double? xmlEvaluateAsDouble(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<Double?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns DateTime value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as DateTime.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsDateTime", IsComposable = true)]
    public static DateTime? xmlEvaluateAsDateTime(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<DateTime?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Transforms XML data using an XSLT style sheet.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="stylesheet">Xslt style sheet.</param>
    /// <returns>Output xml document.</returns>
    [DbFunctionEx(StoreNamespace, "xmlTransform", IsComposable = true)]
    public static String xmlTransform(this String input, String stylesheet)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input, stylesheet);
    }

    /// <summary>
    /// Convert XML data to JSON.
    /// </summary>
    /// <param name="input">Input XML data.</param>
    /// <returns>Output JSON data.</returns>
    [DbFunctionEx(StoreNamespace, "xmlToJson", IsComposable = true)]
    public static String xmlToJson(String input)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Convert JSON data to XML.
    /// </summary>
    /// <param name="input">Input JSON data.</param>
    /// <returns>Output XML data.</returns>
    [DbFunctionEx(StoreNamespace, "xmlFromJson", IsComposable = true)]
    public static String xmlFromJson(String input)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), input);
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
    [DbFunctionEx(ContextNamespace, "xmlSelect", IsComposable = true)]
    public static IQueryable<StringRow> xmlSelect(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<IQueryable<StringRow>>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects String values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of String.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsString", IsComposable = true)]
    public static IQueryable<StringRow> xmlSelectAsString(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<IQueryable<StringRow>>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Boolean values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Boolean.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsBit", IsComposable = true)]
    public static IQueryable<BooleanRow> xmlSelectAsBoolean(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<IQueryable<BooleanRow>>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Byte values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Byte.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsTinyInt", IsComposable = true)]
    public static IQueryable<ByteRow> xmlSelectAsByte(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<IQueryable<ByteRow>>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Int16 values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="path">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsMap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Int16.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsSmallInt", IsComposable = true)]
    public static IQueryable<Int16Row> xmlSelectAsInt16(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<IQueryable<Int16Row>>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Int32 values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Int32.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsInt", IsComposable = true)]
    public static IQueryable<Int32Row> xmlSelectAsInt32(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<IQueryable<Int32Row>>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Int64 values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Int64.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsBigInt", IsComposable = true)]
    public static IQueryable<Int64Row> xmlSelectAsInt64(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<IQueryable<Int64Row>>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Single values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Single.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsSingle", IsComposable = true)]
    public static IQueryable<SingleRow> xmlSelectAsSingle(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<IQueryable<SingleRow>>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Double values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Double.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsDouble", IsComposable = true)]
    public static IQueryable<DoubleRow> xmlSelectAsDouble(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<IQueryable<DoubleRow>>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects DateTime values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of DateTime.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsDateTime", IsComposable = true)]
    public static IQueryable<DateTimeRow> xmlSelectAsDateTime(this String input, String xpath, String nsmap)
    {
      return ExecuteUnsupported<IQueryable<DateTimeRow>>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
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
    [DbFunctionEx(StoreNamespace, "comprDeflateCompress", IsComposable = true)]
    public static Byte[] comprDeflateCompress(this Byte[] input, CompressionLevel compressionLevel)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, compressionLevel);
    }

    /// <summary>
    /// Decompress input data compressed by Deflate algorithm.
    /// </summary>
    /// <param name="input">Compressed data to decompress.</param>
    /// <returns>Decompressed data.</returns>
    [DbFunctionEx(StoreNamespace, "comprDeflateDecompress", IsComposable = true)]
    public static Byte[] comprDeflateDecompress(this Byte[] input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Compress input data by GZip algorithm.
    /// </summary>
    /// <param name="input">Input data to compress.</param>
    /// <param name="compressionLevel">One of the enumeration values that indicates whether to emphasize speed or compression effectiveness when creating the entry.</param>
    /// <returns>Compressed data.</returns>
    [DbFunctionEx(StoreNamespace, "comprGZipCompress", IsComposable = true)]
    public static Byte[] comprGZipCompress(this Byte[] input, CompressionLevel compressionLevel)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, compressionLevel);
    }

    /// <summary>
    /// Decompress input data compressed by GZip algorithm.
    /// </summary>
    /// <param name="input">Compressed data to decompress.</param>
    /// <returns>Decompressed data.</returns>
    [DbFunctionEx(StoreNamespace, "comprGZipDecompress", IsComposable = true)]
    public static Byte[] comprGZipDecompress(this Byte[] input)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Gets the collection of entry names that are currently in the zip archive.
    /// </summary>
    /// <param name="input">Zip archive data.</param>
    /// <returns>collection of entry names that are currently in the zip archive.</returns>
    [DbFunctionEx(ContextNamespace, "zipArchiveGetEntries", IsComposable = true)]
    public static IQueryable<ZipArchiveEntryRow> zipArchiveGetEntries(this Byte[] input)
    {
      return ExecuteUnsupported<IQueryable<ZipArchiveEntryRow>>(MethodBase.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Gets the entry data that are currently in the zip archive.
    /// </summary>
    /// <param name="input">Zip archive data.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that specifies the name of the entry to be created.</param>
    /// <returns>Zip archive entry data.</returns>
    [DbFunctionEx(StoreNamespace, "zipArchiveGetEntry", IsComposable = true)]
    public static Byte[] zipArchiveGetEntry(this Byte[] input, String entryName)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, entryName);
    }

    /// <summary>
    /// Add entry data with the specified path and entry name in the zip archive.
    /// </summary>
    /// <param name="input">Zip archive data.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that specifies the name of the entry to be created.</param>
    /// <param name="entryData">Entry data.</param>
    /// <param name="compressionLevel">One of the enumeration values that indicates whether to emphasize speed or compression effectiveness when creating the entry.</param>
    /// <returns>Zip archive data after add entry operation.</returns>
    [DbFunctionEx(StoreNamespace, "zipArchiveAddEntry", IsComposable = true)]
    public static Byte[] zipArchiveAddEntry(this Byte[] input, String entryName, Byte[] entryData, CompressionLevel compressionLevel)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, entryName, entryData, compressionLevel);
    }

    /// <summary>
    /// Deletes the entry from the zip archive.
    /// </summary>
    /// <param name="input">Zip archive data.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that specifies the name of the entry in archive.</param>
    /// <returns>Zip archive data after delete entry operation.</returns>
    [DbFunctionEx(StoreNamespace, "zipArchiveDeleteEntry", IsComposable = true)]
    public static Byte[] zipArchiveDeleteEntry(this Byte[] input, String entryName)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, entryName);
    }

    #endregion
    #region Cryptography functions

    /// <summary>
    /// Generate cryptographically strong sequence of random values.
    /// </summary>
    /// <param name="count">Generated sequence length.</param>
    /// <returns>Bytes array with a cryptographically strong sequence of random values.</returns>
    [DbFunctionEx(StoreNamespace, "cryptRandom", IsComposable = true)]
    public static Byte[] cryptRandom(this int count)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), count);
    }

    /// <summary>
    /// Generate cryptographically strong sequence of random nonzero values.
    /// </summary>
    /// <param name="count">Generated sequence length.</param>
    /// <returns>Bytes array with a cryptographically strong sequence of random nonzero values.</returns>
    [DbFunctionEx(StoreNamespace, "cryptNonZeroRandom", IsComposable = true)]
    public static Byte[] cryptNonZeroRandom(this int count)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), count);
    }

    /// <summary>
    /// Computes the hash value for the specified data.
    /// </summary>
    /// <param name="input">The input data to compute the hash code for.</param>
    /// <param name="algorithmName">The hash algorithm implementation to use.</param>
    /// <returns>The computed hash value.</returns>
    [DbFunctionEx(StoreNamespace, "cryptComputeHash", IsComposable = true)]
    public static Byte[] cryptComputeHash(this Byte[] input, String algorithmName)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, algorithmName);
    }

    /// <summary>
    /// Verifies the hash value for the specified data.
    /// </summary>
    /// <param name="input">The input data to compute the hash code for.</param>
    /// <param name="hash">The hash value to verify.</param>
    /// <param name="algorithmName">The hash algorithm implementation to use.</param>
    /// <returns>Hash verification result.</returns>
    [DbFunctionEx(StoreNamespace, "cryptVerifyHash", IsComposable = true)]
    public static Boolean? cryptVerifyHash(this Byte[] input, Byte[] hash, String algorithmName)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, hash, algorithmName);
    }

    /// <summary>
    /// Computes the hash value for the specified key and data.
    /// </summary>
    /// <param name="input">The input data to compute the hash code for.</param>
    /// <param name="key">The key to use in the hash algorithm.</param>
    /// <param name="algorithmName">The keyed hash algorithm implementation to use.</param>
    /// <returns>The computed hash value.</returns>
    [DbFunctionEx(StoreNamespace, "cryptComputeKeyedHash", IsComposable = true)]
    public static Byte[] cryptComputeKeyedHash(this Byte[] input, Byte[] key, String algorithmName)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, key, algorithmName);
    }

    /// <summary>
    /// Verifies the hash value for the specified key and data.
    /// </summary>
    /// <param name="input">The input data to compute the hash code for.</param>
    /// <param name="key">The key to use in the hash algorithm.</param>
    /// <param name="hash">The hash value to verify.</param>
    /// <param name="algorithmName">The keyed hash algorithm implementation to use.</param>
    /// <returns>Hash verification result.</returns>
    [DbFunctionEx(StoreNamespace, "cryptVerifyKeyedHash", IsComposable = true)]
    public static Boolean? cryptVerifyKeyedHash(this Byte[] input, Byte[] key, Byte[] hash, String algorithmName)
    {
      return ExecuteUnsupported<Boolean?>(MethodBase.GetCurrentMethod(), input, key, hash, algorithmName);
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
    [DbFunctionEx(StoreNamespace, "cryptEncryptSymmetric", IsComposable = true)]
    public static Byte[] cryptEncryptSymmetric(this Byte[] input, Byte[] key, Byte[] iv, String algorithmName, CipherMode mode, PaddingMode padding)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, key, iv, algorithmName, mode, padding);
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
    [DbFunctionEx(StoreNamespace, "cryptDecryptSymmetric", IsComposable = true)]
    public static Byte[] cryptDecryptSymmetric(this Byte[] input, Byte[] key, Byte[] iv, String algorithmName, CipherMode mode, PaddingMode padding)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), input, key, iv, algorithmName, mode, padding);
    }

    #endregion
    #region Collect functions

    [DbFunctionEx(StoreNamespace, "bCollect", IsComposable = true, IsAggregate = true)]
    public static Byte[] boolCollect(Boolean? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "tiCollect", IsComposable = true, IsAggregate = true)]
    public static Byte[] byteCollect(Byte? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "siCollect", IsComposable = true, IsAggregate = true)]
    public static Byte[] int16Collect(Int16? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "iCollect", IsComposable = true, IsAggregate = true)]
    public static Byte[] int32Collect(Int32? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "biCollect", IsComposable = true, IsAggregate = true)]
    public static Byte[] int64Collect(Int64? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "sfCollect", IsComposable = true, IsAggregate = true)]
    public static Byte[] sglCollect(Single? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "dfCollect", IsComposable = true, IsAggregate = true)]
    public static Byte[] dblCollect(Double? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "dtCollect", IsComposable = true, IsAggregate = true)]
    public static Byte[] dtmCollect(DateTime? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "uidCollect", IsComposable = true, IsAggregate = true)]
    public static Byte[] guidCollect(Guid? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "binCollect", IsComposable = true, IsAggregate = true)]
    public static Byte[] binCollect(Byte[] value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "strCollect", IsComposable = true, IsAggregate = true)]
    public static Byte[] strCollect(String value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    #endregion
    #region Collection functions
    #region Boolean collection

    [DbFunctionEx(StoreNamespace, "bCollCreate", IsComposable = true)]
    public static byte[] boolCollCreate(SizeEncoding countSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing);
    }

    [DbFunctionEx(StoreNamespace, "bCollParse", IsComposable = true)]
    public static byte[] boolCollParse(String input, SizeEncoding countSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing);
    }

    [DbFunctionEx(StoreNamespace, "bCollFormat", IsComposable = true)]
    public static byte[] boolCollFormat(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "bCollCount", IsComposable = true)]
    public static int? boolCollCount(byte[] input)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "bCollIndexOf", IsComposable = true)]
    public static int? boolCollIndexOf(byte[] input, Boolean? value)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "bCollGet", IsComposable = true)]
    public static Boolean? boolCollGet(byte[] input, int index)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "bCollSet", IsComposable = true)]
    public static byte[] boolCollSet(byte[] input, int index, Boolean? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "bCollInsert", IsComposable = true)]
    public static byte[] boolCollInsert(byte[] input, int? index, Boolean? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "bCollRemove", IsComposable = true)]
    public static byte[] boolCollRemove(byte[] input, Boolean? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "bCollRemoveAt", IsComposable = true)]
    public static byte[] boolCollRemoveAt(byte[] input, int index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "bCollClear", IsComposable = true)]
    public static byte[] boolCollClear(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "bCollGetRange", IsComposable = true)]
    public static byte[] boolCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "bCollSetRange", IsComposable = true)]
    public static byte[] boolCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "bCollSetRepeat", IsComposable = true)]
    public static byte[] boolCollSetRepeat(byte[] input, int index, Boolean? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "bCollInsertRange", IsComposable = true)]
    public static byte[] boolCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "bCollInsertRepeat", IsComposable = true)]
    public static byte[] boolCollInsertRepeat(byte[] input, int? index, Boolean? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "bCollRemoveRange", IsComposable = true)]
    public static byte[] boolCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "bCollToArray", IsComposable = true)]
    public static byte[] boolCollToArray(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "bCollEnumerate", IsComposable = true)]
    public static IQueryable<IndexedBooleanRow> boolCollEnumerate(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedBooleanRow>>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Byte collection

    [DbFunctionEx(StoreNamespace, "tiCollCreate", IsComposable = true)]
    public static byte[] byteCollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "tiCollParse", IsComposable = true)]
    public static byte[] byteCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "tiCollFormat", IsComposable = true)]
    public static String byteCollFormat(byte[] input)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "tiCollCount", IsComposable = true)]
    public static int? byteCollCount(byte[] input)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "tiCollIndexOf", IsComposable = true)]
    public static int? byteCollIndexOf(byte[] input, Byte? value)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "tiCollGet", IsComposable = true)]
    public static Byte? byteCollGet(byte[] input, int index)
    {
      return ExecuteUnsupported<Byte?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "tiCollSet", IsComposable = true)]
    public static byte[] byteCollSet(byte[] input, int index, Byte? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "tiCollInsert", IsComposable = true)]
    public static byte[] byteCollInsert(byte[] input, int? index, Byte? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "tiCollRemove", IsComposable = true)]
    public static byte[] byteCollRemove(byte[] input, Byte? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "tiCollRemoveAt", IsComposable = true)]
    public static byte[] byteCollRemoveAt(byte[] input, int index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "tiCollClear", IsComposable = true)]
    public static byte[] byteCollClear(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "tiCollGetRange", IsComposable = true)]
    public static byte[] byteCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "tiCollSetRange", IsComposable = true)]
    public static byte[] byteCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "tiCollSetRepeat", IsComposable = true)]
    public static byte[] byteCollSetRepeat(byte[] input, int index, Byte? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "tiCollInsertRange", IsComposable = true)]
    public static byte[] byteCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "tiCollInsertRepeat", IsComposable = true)]
    public static byte[] byteCollInsertRepeat(byte[] input, int? index, Byte? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "tiCollRemoveRange", IsComposable = true)]
    public static byte[] byteCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "tiCollToArray", IsComposable = true)]
    public static byte[] byteCollToArray(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "tiCollEnumerate", IsComposable = true)]
    public static IQueryable<IndexedByteRow> byteCollEnumerate(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedByteRow>>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Int16 collection

    [DbFunctionEx(StoreNamespace, "siCollCreate", IsComposable = true)]
    public static byte[] int16CollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "siCollParse", IsComposable = true)]
    public static byte[] int16CollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "siCollFormat", IsComposable = true)]
    public static String int16CollFormat(byte[] input)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "siCollCount", IsComposable = true)]
    public static int? int16CollCount(byte[] input)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "siCollIndexOf", IsComposable = true)]
    public static int? int16CollIndexOf(byte[] input, Int16? value)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "siCollGet", IsComposable = true)]
    public static Int16? int16CollGet(byte[] input, int index)
    {
      return ExecuteUnsupported<Int16?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "siCollSet", IsComposable = true)]
    public static byte[] int16CollSet(byte[] input, int index, Int16? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "siCollInsert", IsComposable = true)]
    public static byte[] int16CollInsert(byte[] input, int? index, Int16? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "siCollRemove", IsComposable = true)]
    public static byte[] int16CollRemove(byte[] input, Int16? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "siCollRemoveAt", IsComposable = true)]
    public static byte[] int16CollRemoveAt(byte[] input, int index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "siCollClear", IsComposable = true)]
    public static byte[] int16CollClear(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "siCollGetRange", IsComposable = true)]
    public static byte[] int16CollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "siCollSetRange", IsComposable = true)]
    public static byte[] int16CollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "siCollSetRepeat", IsComposable = true)]
    public static byte[] int16CollSetRepeat(byte[] input, int index, Int16? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "siCollInsertRange", IsComposable = true)]
    public static byte[] int16CollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "siCollInsertRepeat", IsComposable = true)]
    public static byte[] int16CollInsertRepeat(byte[] input, int? index, Int16? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "siCollRemoveRange", IsComposable = true)]
    public static byte[] int16CollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "siCollToArray", IsComposable = true)]
    public static byte[] int16CollToArray(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "siCollEnumerate", IsComposable = true)]
    public static IQueryable<IndexedInt16Row> int16CollEnumerate(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedInt16Row>>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Int32 collection

    [DbFunctionEx(StoreNamespace, "iCollCreate", IsComposable = true)]
    public static byte[] int32CollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "iCollParse", IsComposable = true)]
    public static byte[] int32CollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "iCollFormat", IsComposable = true)]
    public static String int32CollFormat(byte[] input)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "iCollCount", IsComposable = true)]
    public static int? int32CollCount(byte[] input)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "iCollIndexOf", IsComposable = true)]
    public static int? int32CollIndexOf(byte[] input, Int32? value)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "iCollGet", IsComposable = true)]
    public static Int32? int32CollGet(byte[] input, int index)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "iCollSet", IsComposable = true)]
    public static byte[] int32CollSet(byte[] input, int index, Int32? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "iCollInsert", IsComposable = true)]
    public static byte[] int32CollInsert(byte[] input, int? index, Int32? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "iCollRemove", IsComposable = true)]
    public static byte[] int32CollRemove(byte[] input, Int32? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "iCollRemoveAt", IsComposable = true)]
    public static byte[] int32CollRemoveAt(byte[] input, int index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "iCollClear", IsComposable = true)]
    public static byte[] int32CollClear(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "iCollGetRange", IsComposable = true)]
    public static byte[] int32CollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "iCollSetRange", IsComposable = true)]
    public static byte[] int32CollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "iCollSetRepeat", IsComposable = true)]
    public static byte[] int32CollSetRepeat(byte[] input, int index, Int32? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "iCollInsertRange", IsComposable = true)]
    public static byte[] int32CollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "iCollInsertRepeat", IsComposable = true)]
    public static byte[] int32CollInsertRepeat(byte[] input, int? index, Int32? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "iCollRemoveRange", IsComposable = true)]
    public static byte[] int32CollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "iCollToArray", IsComposable = true)]
    public static byte[] int32CollToArray(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "iCollEnumerate", IsComposable = true)]
    public static IQueryable<IndexedInt32Row> int32CollEnumerate(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedInt32Row>>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Int64 collection

    [DbFunctionEx(StoreNamespace, "biCollCreate", IsComposable = true)]
    public static byte[] int64CollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "biCollParse", IsComposable = true)]
    public static byte[] int64CollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "biCollFormat", IsComposable = true)]
    public static String int64CollFormat(byte[] input)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "biCollCount", IsComposable = true)]
    public static int? int64CollCount(byte[] input)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "biCollIndexOf", IsComposable = true)]
    public static int? int64CollIndexOf(byte[] input, Int64? value)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "biCollGet", IsComposable = true)]
    public static Int64? int64CollGet(byte[] input, int index)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "biCollSet", IsComposable = true)]
    public static byte[] int64CollSet(byte[] input, int index, Int64? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "biCollInsert", IsComposable = true)]
    public static byte[] int64CollInsert(byte[] input, int? index, Int64? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "biCollRemove", IsComposable = true)]
    public static byte[] int64CollRemove(byte[] input, Int64? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "biCollRemoveAt", IsComposable = true)]
    public static byte[] int64CollRemoveAt(byte[] input, int index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "biCollClear", IsComposable = true)]
    public static byte[] int64CollClear(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "biCollGetRange", IsComposable = true)]
    public static byte[] int64CollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "biCollSetRange", IsComposable = true)]
    public static byte[] int64CollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "biCollSetRepeat", IsComposable = true)]
    public static byte[] int64CollSetRepeat(byte[] input, int index, Int64? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "biCollInsertRange", IsComposable = true)]
    public static byte[] int64CollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "biCollInsertRepeat", IsComposable = true)]
    public static byte[] int64CollInsertRepeat(byte[] input, int? index, Int64? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "biCollRemoveRange", IsComposable = true)]
    public static byte[] int64CollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "biCollToArray", IsComposable = true)]
    public static byte[] int64CollToArray(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "biCollEnumerate", IsComposable = true)]
    public static IQueryable<IndexedInt64Row> int64CollEnumerate(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedInt64Row>>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Single collection

    [DbFunctionEx(StoreNamespace, "sfCollCreate", IsComposable = true)]
    public static byte[] sglCollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "sfCollParse", IsComposable = true)]
    public static byte[] sglCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "sfCollFormat", IsComposable = true)]
    public static String sglCollFormat(byte[] input)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "sfCollCount", IsComposable = true)]
    public static int? sglCollCount(byte[] input)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "sfCollIndexOf", IsComposable = true)]
    public static int? sglCollIndexOf(byte[] input, Single? value)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "sfCollGet", IsComposable = true)]
    public static Single? sglCollGet(byte[] input, int index)
    {
      return ExecuteUnsupported<Single?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "sfCollSet", IsComposable = true)]
    public static byte[] sglCollSet(byte[] input, int index, Single? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "sfCollInsert", IsComposable = true)]
    public static byte[] sglCollInsert(byte[] input, int? index, Single? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "sfCollRemove", IsComposable = true)]
    public static byte[] sglCollRemove(byte[] input, Single? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "sfCollRemoveAt", IsComposable = true)]
    public static byte[] sglCollRemoveAt(byte[] input, int index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "sfCollClear", IsComposable = true)]
    public static byte[] sglCollClear(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "sfCollGetRange", IsComposable = true)]
    public static byte[] sglCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "sfCollSetRange", IsComposable = true)]
    public static byte[] sglCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "sfCollSetRepeat", IsComposable = true)]
    public static byte[] sglCollSetRepeat(byte[] input, int index, Single? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "sfCollInsertRange", IsComposable = true)]
    public static byte[] sglCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "sfCollInsertRepeat", IsComposable = true)]
    public static byte[] sglCollInsertRepeat(byte[] input, int? index, Single? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "sfCollRemoveRange", IsComposable = true)]
    public static byte[] sglCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "sfCollToArray", IsComposable = true)]
    public static byte[] sglCollToArray(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "sfCollEnumerate", IsComposable = true)]
    public static IQueryable<IndexedSingleRow> sglCollEnumerate(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedSingleRow>>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Double collection

    [DbFunctionEx(StoreNamespace, "dfCollCreate", IsComposable = true)]
    public static byte[] dblCollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dfCollParse", IsComposable = true)]
    public static byte[] dblCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dfCollFormat", IsComposable = true)]
    public static String dblCollFormat(byte[] input)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "dfCollCount", IsComposable = true)]
    public static int? dblCollCount(byte[] input)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "dfCollIndexOf", IsComposable = true)]
    public static int? dblCollIndexOf(byte[] input, Double? value)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "dfCollGet", IsComposable = true)]
    public static Double? dblCollGet(byte[] input, int index)
    {
      return ExecuteUnsupported<Double?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "dfCollSet", IsComposable = true)]
    public static byte[] dblCollSet(byte[] input, int index, Double? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dfCollInsert", IsComposable = true)]
    public static byte[] dblCollInsert(byte[] input, int? index, Double? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dfCollRemove", IsComposable = true)]
    public static byte[] dblCollRemove(byte[] input, Double? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "dfCollRemoveAt", IsComposable = true)]
    public static byte[] dblCollRemoveAt(byte[] input, int index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "dfCollClear", IsComposable = true)]
    public static byte[] dblCollClear(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "dfCollGetRange", IsComposable = true)]
    public static byte[] dblCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dfCollSetRange", IsComposable = true)]
    public static byte[] dblCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "dfCollSetRepeat", IsComposable = true)]
    public static byte[] dblCollSetRepeat(byte[] input, int index, Double? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "dfCollInsertRange", IsComposable = true)]
    public static byte[] dblCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "dfCollInsertRepeat", IsComposable = true)]
    public static byte[] dblCollInsertRepeat(byte[] input, int? index, Double? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "dfCollRemoveRange", IsComposable = true)]
    public static byte[] dblCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dfCollToArray", IsComposable = true)]
    public static byte[] dblCollToArray(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "dfCollEnumerate", IsComposable = true)]
    public static IQueryable<IndexedDoubleRow> dblCollEnumerate(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedDoubleRow>>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region DateTime collection

    [DbFunctionEx(StoreNamespace, "dtCollCreate", IsComposable = true)]
    public static byte[] dtmCollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dtCollParse", IsComposable = true)]
    public static byte[] dtmCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dtCollFormat", IsComposable = true)]
    public static String dtmCollFormat(byte[] input)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "dtCollCount", IsComposable = true)]
    public static int? dtmCollCount(byte[] input)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "dtCollIndexOf", IsComposable = true)]
    public static int? dtmCollIndexOf(byte[] input, DateTime? value)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "dtCollGet", IsComposable = true)]
    public static DateTime? dtmCollGet(byte[] input, int index)
    {
      return ExecuteUnsupported<DateTime?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "dtCollSet", IsComposable = true)]
    public static byte[] dtmCollSet(byte[] input, int index, DateTime? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dtCollInsert", IsComposable = true)]
    public static byte[] dtmCollInsert(byte[] input, int? index, DateTime? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dtCollRemove", IsComposable = true)]
    public static byte[] dtmCollRemove(byte[] input, DateTime? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "dtCollRemoveAt", IsComposable = true)]
    public static byte[] dtmCollRemoveAt(byte[] input, int index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "dtCollClear", IsComposable = true)]
    public static byte[] dtmCollClear(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "dtCollGetRange", IsComposable = true)]
    public static byte[] dtmCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dtCollSetRange", IsComposable = true)]
    public static byte[] dtmCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "dtCollSetRepeat", IsComposable = true)]
    public static byte[] dtmCollSetRepeat(byte[] input, int index, DateTime? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "dtCollInsertRange", IsComposable = true)]
    public static byte[] dtmCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "dtCollInsertRepeat", IsComposable = true)]
    public static byte[] dtmCollInsertRepeat(byte[] input, int? index, DateTime? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "dtCollRemoveRange", IsComposable = true)]
    public static byte[] dtmCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dtCollToArray", IsComposable = true)]
    public static byte[] dtmCollToArray(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "dtCollEnumerate", IsComposable = true)]
    public static IQueryable<IndexedDateTimeRow> dtmCollEnumerate(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedDateTimeRow>>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Guid collection

    [DbFunctionEx(StoreNamespace, "uidCollCreate", IsComposable = true)]
    public static byte[] guidCollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "uidCollParse", IsComposable = true)]
    public static byte[] guidCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "uidCollFormat", IsComposable = true)]
    public static String guidCollFormat(byte[] input)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "uidCollCount", IsComposable = true)]
    public static int? guidCollCount(byte[] input)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "uidCollIndexOf", IsComposable = true)]
    public static int? guidCollIndexOf(byte[] input, Guid? value)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "uidCollGet", IsComposable = true)]
    public static Guid? guidCollGet(byte[] input, int index)
    {
      return ExecuteUnsupported<Guid?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "uidCollSet", IsComposable = true)]
    public static byte[] guidCollSet(byte[] input, int index, Guid? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "uidCollInsert", IsComposable = true)]
    public static byte[] guidCollInsert(byte[] input, int? index, Guid? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "uidCollRemove", IsComposable = true)]
    public static byte[] guidCollRemove(byte[] input, Guid? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "uidCollRemoveAt", IsComposable = true)]
    public static byte[] guidCollRemoveAt(byte[] input, int index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "uidCollClear", IsComposable = true)]
    public static byte[] guidCollClear(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "uidCollGetRange", IsComposable = true)]
    public static byte[] guidCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "uidCollSetRange", IsComposable = true)]
    public static byte[] guidCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "uidCollSetRepeat", IsComposable = true)]
    public static byte[] guidCollSetRepeat(byte[] input, int index, Guid? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "uidCollInsertRange", IsComposable = true)]
    public static byte[] guidCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "uidCollInsertRepeat", IsComposable = true)]
    public static byte[] guidCollInsertRepeat(byte[] input, int? index, Guid? value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "uidCollRemoveRange", IsComposable = true)]
    public static byte[] guidCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "uidCollToArray", IsComposable = true)]
    public static byte[] guidCollToArray(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "uidCollEnumerate", IsComposable = true)]
    public static IQueryable<IndexedGuidRow> guidCollEnumerate(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedGuidRow>>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region String collection

    [DbFunctionEx(StoreNamespace, "strCollCreate", IsComposable = true)]
    public static byte[] strCollCreate(SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "strCollCreateByCpId", IsComposable = true)]
    public static byte[] strCollCreate(SizeEncoding countSizing, SizeEncoding itemSizing, int? cpId)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, itemSizing, cpId);
    }

    [DbFunctionEx(StoreNamespace, "strCollCreateByCpName", IsComposable = true)]
    public static byte[] strCollCreate(SizeEncoding countSizing, SizeEncoding itemSizing, string cpName)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, itemSizing, cpName);
    }

    [DbFunctionEx(StoreNamespace, "strCollParse", IsComposable = true)]
    public static byte[] strCollParse(String input, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "strCollParseByCpId", IsComposable = true)]
    public static byte[] strCollParse(String input, SizeEncoding countSizing, SizeEncoding itemSizing, int? cpId)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, itemSizing, cpId);
    }

    [DbFunctionEx(StoreNamespace, "strCollParseByCpName", IsComposable = true)]
    public static byte[] strCollParse(String input, SizeEncoding countSizing, SizeEncoding itemSizing, string cpName)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, itemSizing, cpName);
    }

    [DbFunctionEx(StoreNamespace, "strCollFormat", IsComposable = true)]
    public static String strCollFormat(byte[] input)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strCollCount", IsComposable = true)]
    public static int? strCollCount(byte[] input)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strCollIndexOf", IsComposable = true)]
    public static int? strCollIndexOf(byte[] input, String value)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "strCollGet", IsComposable = true)]
    public static String strCollGet(byte[] input, int index)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "strCollSet", IsComposable = true)]
    public static byte[] strCollSet(byte[] input, int index, String value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "strCollInsert", IsComposable = true)]
    public static byte[] strCollInsert(byte[] input, int? index, String value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "strCollRemove", IsComposable = true)]
    public static byte[] strCollRemove(byte[] input, String value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "strCollRemoveAt", IsComposable = true)]
    public static byte[] strCollRemoveAt(byte[] input, int index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "strCollClear", IsComposable = true)]
    public static byte[] strCollClear(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strCollGetRange", IsComposable = true)]
    public static byte[] strCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strCollSetRange", IsComposable = true)]
    public static byte[] strCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "strCollSetRepeat", IsComposable = true)]
    public static byte[] strCollSetRepeat(byte[] input, int index, String value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "strCollInsertRange", IsComposable = true)]
    public static byte[] strCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "strCollInsertRepeat", IsComposable = true)]
    public static byte[] strCollInsertRepeat(byte[] input, int? index, String value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "strCollRemoveRange", IsComposable = true)]
    public static byte[] strCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strCollToArray", IsComposable = true)]
    public static byte[] strCollToArray(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "strCollEnumerate", IsComposable = true)]
    public static IQueryable<IndexedStringRow> strCollEnumerate(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedStringRow>>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Binary collection

    [DbFunctionEx(StoreNamespace, "binCollCreate", IsComposable = true)]
    public static byte[] binCollCreate(SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "binCollParse", IsComposable = true)]
    public static byte[] binCollParse(String input, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "binCollFormat", IsComposable = true)]
    public static String binCollFormat(byte[] input)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binCollCount", IsComposable = true)]
    public static int? binCollCount(byte[] input)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binCollIndexOf", IsComposable = true)]
    public static int? binCollIndexOf(byte[] input, byte[] value)
    {
      return ExecuteUnsupported<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "binCollGet", IsComposable = true)]
    public static byte[] binCollGet(byte[] input, int index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binCollSet", IsComposable = true)]
    public static byte[] binCollSet(byte[] input, int index, byte[] value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "binCollInsert", IsComposable = true)]
    public static byte[] binCollInsert(byte[] input, int? index, byte[] value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "binCollRemove", IsComposable = true)]
    public static byte[] binCollRemove(byte[] input, byte[] value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "binCollRemoveAt", IsComposable = true)]
    public static byte[] binCollRemoveAt(byte[] input, int index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binCollClear", IsComposable = true)]
    public static byte[] binCollClear(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binCollGetRange", IsComposable = true)]
    public static byte[] binCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binCollSetRange", IsComposable = true)]
    public static byte[] binCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "binCollSetRepeat", IsComposable = true)]
    public static byte[] binCollSetRepeat(byte[] input, int index, byte[] value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "binCollInsertRange", IsComposable = true)]
    public static byte[] binCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "binCollInsertRepeat", IsComposable = true)]
    public static byte[] binCollInsertRepeat(byte[] input, int? index, byte[] value, int count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "binCollRemoveRange", IsComposable = true)]
    public static byte[] binCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binCollToArray", IsComposable = true)]
    public static byte[] binCollToArray(byte[] input)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binCollEnumerate", IsComposable = true)]
    public static IQueryable<IndexedBinaryRow> binCollEnumerate(byte[] input, int? index, int? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedBinaryRow>>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #endregion
    #region Array functions
    #region Boolean array functions

    [DbFunctionEx(StoreNamespace, "bArrayCreate", IsComposable = true)]
    public static byte[] boolArrayCreate(Int32 length, SizeEncoding countSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing);
    }

    [DbFunctionEx(StoreNamespace, "bArrayParse", IsComposable = true)]
    public static byte[] boolArrayParse(String str, SizeEncoding countSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing);
    }

    [DbFunctionEx(StoreNamespace, "bArrayFormat", IsComposable = true)]
    public static String boolArrayFormat(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "bArrayLength", IsComposable = true)]
    public static Int32? boolArrayLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "bArrayIndexOf", IsComposable = true)]
    public static Int32? boolArrayIndexOf(byte[] array, Boolean? value)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "bArrayGet", IsComposable = true)]
    public static Boolean? boolArrayGet(byte[] array, Int32 index)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "bArraySet", IsComposable = true)]
    public static byte[] boolArraySet(byte[] array, Int32 index, Boolean? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "bArrayGetRange", IsComposable = true)]
    public static byte[] boolArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "bArraySetRange", IsComposable = true)]
    public static byte[] boolArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "bArrayFillRange", IsComposable = true)]
    public static byte[] boolArrayFillRange(byte[] array, Int32? index, Int32? count, Boolean? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "bArrayToCollection", IsComposable = true)]
    public static byte[] boolArrayToCollection(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "bArrayEnumerate", IsComposable = true)]
    public static IQueryable<IndexedBooleanRow> boolArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedBooleanRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Byte array functions

    [DbFunctionEx(StoreNamespace, "tiArrayCreate", IsComposable = true)]
    public static byte[] byteArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayParse", IsComposable = true)]
    public static byte[] byteArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayFormat", IsComposable = true)]
    public static String byteArrayFormat(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayLength", IsComposable = true)]
    public static Int32? byteArrayLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayIndexOf", IsComposable = true)]
    public static Int32? byteArrayIndexOf(byte[] array, Byte? value)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayGet", IsComposable = true)]
    public static Byte? byteArrayGet(byte[] array, Int32 index)
    {
      return ExecuteUnsupported<Byte?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "tiArraySet", IsComposable = true)]
    public static byte[] byteArraySet(byte[] array, Int32 index, Byte? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayGetRange", IsComposable = true)]
    public static byte[] byteArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "tiArraySetRange", IsComposable = true)]
    public static byte[] byteArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayFillRange", IsComposable = true)]
    public static byte[] byteArrayFillRange(byte[] array, Int32? index, Int32? count, Byte? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayToCollection", IsComposable = true)]
    public static byte[] byteArrayToCollection(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "tiArrayEnumerate", IsComposable = true)]
    public static IQueryable<IndexedByteRow> byteArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedByteRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Int16 array functions

    [DbFunctionEx(StoreNamespace, "siArrayCreate", IsComposable = true)]
    public static byte[] int16ArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "siArrayParse", IsComposable = true)]
    public static byte[] int16ArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "siArrayFormat", IsComposable = true)]
    public static String int16ArrayFormat(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "siArrayLength", IsComposable = true)]
    public static Int32? int16ArrayLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "siArrayIndexOf", IsComposable = true)]
    public static Int32? int16ArrayIndexOf(byte[] array, Int16? value)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "siArrayGet", IsComposable = true)]
    public static Int16? int16ArrayGet(byte[] array, Int32 index)
    {
      return ExecuteUnsupported<Int16?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "siArraySet", IsComposable = true)]
    public static byte[] int16ArraySet(byte[] array, Int32 index, Int16? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "siArrayGetRange", IsComposable = true)]
    public static byte[] int16ArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "siArraySetRange", IsComposable = true)]
    public static byte[] int16ArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "siArrayFillRange", IsComposable = true)]
    public static byte[] int16ArrayFillRange(byte[] array, Int32? index, Int32? count, Int16? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "siArrayToCollection", IsComposable = true)]
    public static byte[] int16ArrayToCollection(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "siArrayEnumerate", IsComposable = true)]
    public static IQueryable<IndexedInt16Row> int16ArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedInt16Row>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Int32 array functions

    [DbFunctionEx(StoreNamespace, "iArrayCreate", IsComposable = true)]
    public static byte[] int32ArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "iArrayParse", IsComposable = true)]
    public static byte[] int32ArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "iArrayFormat", IsComposable = true)]
    public static String int32ArrayFormat(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "iArrayLength", IsComposable = true)]
    public static Int32? int32ArrayLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "iArrayIndexOf", IsComposable = true)]
    public static Int32? int32ArrayIndexOf(byte[] array, Int32? value)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "iArrayGet", IsComposable = true)]
    public static Int32? int32ArrayGet(byte[] array, Int32 index)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "iArraySet", IsComposable = true)]
    public static byte[] int32ArraySet(byte[] array, Int32 index, Int32? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "iArrayGetRange", IsComposable = true)]
    public static byte[] int32ArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "iArraySetRange", IsComposable = true)]
    public static byte[] int32ArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "iArrayFillRange", IsComposable = true)]
    public static byte[] int32ArrayFillRange(byte[] array, Int32? index, Int32? count, Int32? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "iArrayToCollection", IsComposable = true)]
    public static byte[] int32ArrayToCollection(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "iArrayEnumerate", IsComposable = true)]
    public static IQueryable<IndexedInt32Row> int32ArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedInt32Row>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Int64 array functions

    [DbFunctionEx(StoreNamespace, "biArrayCreate", IsComposable = true)]
    public static byte[] int64ArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "biArrayParse", IsComposable = true)]
    public static byte[] int64ArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "biArrayFormat", IsComposable = true)]
    public static String int64ArrayFormat(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "biArrayLength", IsComposable = true)]
    public static Int32? int64ArrayLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "biArrayIndexOf", IsComposable = true)]
    public static Int32? int64ArrayIndexOf(byte[] array, Int64? value)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "biArrayGet", IsComposable = true)]
    public static Int64? int64ArrayGet(byte[] array, Int32 index)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "biArraySet", IsComposable = true)]
    public static byte[] int64ArraySet(byte[] array, Int32 index, Int64? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "biArrayGetRange", IsComposable = true)]
    public static byte[] int64ArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "biArraySetRange", IsComposable = true)]
    public static byte[] int64ArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "biArrayFillRange", IsComposable = true)]
    public static byte[] int64ArrayFillRange(byte[] array, Int32? index, Int32? count, Int64? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "biArrayToCollection", IsComposable = true)]
    public static byte[] int64ArrayToCollection(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "biArrayEnumerate", IsComposable = true)]
    public static IQueryable<IndexedInt64Row> int64ArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedInt64Row>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Single array functions

    [DbFunctionEx(StoreNamespace, "sfArrayCreate", IsComposable = true)]
    public static byte[] sglArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayParse", IsComposable = true)]
    public static byte[] sglArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayFormat", IsComposable = true)]
    public static String sglArrayFormat(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayLength", IsComposable = true)]
    public static Int32? sglArrayLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayIndexOf", IsComposable = true)]
    public static Int32? sglArrayIndexOf(byte[] array, Single? value)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayGet", IsComposable = true)]
    public static Single? sglArrayGet(byte[] array, Int32 index)
    {
      return ExecuteUnsupported<Single?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "sfArraySet", IsComposable = true)]
    public static byte[] sglArraySet(byte[] array, Int32 index, Single? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayGetRange", IsComposable = true)]
    public static byte[] sglArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "sfArraySetRange", IsComposable = true)]
    public static byte[] sglArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayFillRange", IsComposable = true)]
    public static byte[] sglArrayFillRange(byte[] array, Int32? index, Int32? count, Single? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayToCollection", IsComposable = true)]
    public static byte[] sglArrayToCollection(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "sfArrayEnumerate", IsComposable = true)]
    public static IQueryable<IndexedSingleRow> sglArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedSingleRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Double array functions

    [DbFunctionEx(StoreNamespace, "dfArrayCreate", IsComposable = true)]
    public static byte[] dblArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayParse", IsComposable = true)]
    public static byte[] dblArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayFormat", IsComposable = true)]
    public static String dblArrayFormat(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayLength", IsComposable = true)]
    public static Int32? dblArrayLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayIndexOf", IsComposable = true)]
    public static Int32? dblArrayIndexOf(byte[] array, Double? value)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayGet", IsComposable = true)]
    public static Double? dblArrayGet(byte[] array, Int32 index)
    {
      return ExecuteUnsupported<Double?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "dfArraySet", IsComposable = true)]
    public static byte[] dblArraySet(byte[] array, Int32 index, Double? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayGetRange", IsComposable = true)]
    public static byte[] dblArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dfArraySetRange", IsComposable = true)]
    public static byte[] dblArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayFillRange", IsComposable = true)]
    public static byte[] dblArrayFillRange(byte[] array, Int32? index, Int32? count, Double? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayToCollection", IsComposable = true)]
    public static byte[] dblArrayToCollection(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "dfArrayEnumerate", IsComposable = true)]
    public static IQueryable<IndexedDoubleRow> dblArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedDoubleRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region DateTime array functions

    [DbFunctionEx(StoreNamespace, "dtArrayCreate", IsComposable = true)]
    public static byte[] dtmArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayParse", IsComposable = true)]
    public static byte[] dtmArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayFormat", IsComposable = true)]
    public static String dtmArrayFormat(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayLength", IsComposable = true)]
    public static Int32? dtmArrayLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayIndexOf", IsComposable = true)]
    public static Int32? dtmArrayIndexOf(byte[] array, DateTime? value)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayGet", IsComposable = true)]
    public static DateTime? dtmArrayGet(byte[] array, Int32 index)
    {
      return ExecuteUnsupported<DateTime?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "dtArraySet", IsComposable = true)]
    public static byte[] dtmArraySet(byte[] array, Int32 index, DateTime? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayGetRange", IsComposable = true)]
    public static byte[] dtmArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dtArraySetRange", IsComposable = true)]
    public static byte[] dtmArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayFillRange", IsComposable = true)]
    public static byte[] dtmArrayFillRange(byte[] array, Int32? index, Int32? count, DateTime? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayToCollection", IsComposable = true)]
    public static byte[] dtmArrayToCollection(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "dtArrayEnumerate", IsComposable = true)]
    public static IQueryable<IndexedDateTimeRow> dtmArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedDateTimeRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Guid array functions

    [DbFunctionEx(StoreNamespace, "uidArrayCreate", IsComposable = true)]
    public static byte[] guidArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayParse", IsComposable = true)]
    public static byte[] guidArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayFormat", IsComposable = true)]
    public static String guidArrayFormat(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayLength", IsComposable = true)]
    public static Int32? guidArrayLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayIndexOf", IsComposable = true)]
    public static Int32? guidArrayIndexOf(byte[] array, Guid? value)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayGet", IsComposable = true)]
    public static Guid? guidArrayGet(byte[] array, Int32 index)
    {
      return ExecuteUnsupported<Guid?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "uidArraySet", IsComposable = true)]
    public static byte[] guidArraySet(byte[] array, Int32 index, Guid? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayGetRange", IsComposable = true)]
    public static byte[] guidArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "uidArraySetRange", IsComposable = true)]
    public static byte[] guidArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayFillRange", IsComposable = true)]
    public static byte[] guidArrayFillRange(byte[] array, Int32? index, Int32? count, Guid? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayToCollection", IsComposable = true)]
    public static byte[] guidArrayToCollection(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "uidArrayEnumerate", IsComposable = true)]
    public static IQueryable<IndexedGuidRow> guidArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedGuidRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region String array functions

    [DbFunctionEx(StoreNamespace, "strArrayCreate", IsComposable = true)]
    public static byte[] strArrayCreate(Int32 length, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "strArrayCreateByCpId", IsComposable = true)]
    public static byte[] strArrayCreate(Int32 length, SizeEncoding countSizing, SizeEncoding itemSizing, Int32? cpId)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, itemSizing, cpId);
    }

    [DbFunctionEx(StoreNamespace, "strArrayCreateByCpName", IsComposable = true)]
    public static byte[] strArrayCreate(Int32 length, SizeEncoding countSizing, SizeEncoding itemSizing, String cpName)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, itemSizing, cpName);
    }

    [DbFunctionEx(StoreNamespace, "strArrayParse", IsComposable = true)]
    public static byte[] strArrayParse(String str, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "strArrayParseByCpId", IsComposable = true)]
    public static byte[] strArrayParse(String str, SizeEncoding countSizing, SizeEncoding itemSizing, Int32? cpId)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing, cpId);
    }

    [DbFunctionEx(StoreNamespace, "strArrayParseByCpName", IsComposable = true)]
    public static byte[] strArrayParse(String str, SizeEncoding countSizing, SizeEncoding itemSizing, String cpName)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing, cpName);
    }

    [DbFunctionEx(StoreNamespace, "strArrayFormat", IsComposable = true)]
    public static String strArrayFormat(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "strArrayLength", IsComposable = true)]
    public static Int32? strArrayLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "strArrayIndexOf", IsComposable = true)]
    public static Int32? strArrayIndexOf(byte[] array, String value)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "strArrayGet", IsComposable = true)]
    public static String strArrayGet(byte[] array, Int32 index)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "strArraySet", IsComposable = true)]
    public static byte[] strArraySet(byte[] array, Int32 index, String value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "strArrayGetRange", IsComposable = true)]
    public static byte[] strArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strArraySetRange", IsComposable = true)]
    public static byte[] strArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "strArrayFillRange", IsComposable = true)]
    public static byte[] strArrayFillRange(byte[] array, Int32? index, Int32? count, String value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "strArrayToCollection", IsComposable = true)]
    public static byte[] strArrayToCollection(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "strArrayEnumerate", IsComposable = true)]
    public static IQueryable<IndexedStringRow> strArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedStringRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Binary array functions

    [DbFunctionEx(StoreNamespace, "binArrayCreate", IsComposable = true)]
    public static byte[] binArrayCreate(Int32 length, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "binArrayParse", IsComposable = true)]
    public static byte[] binArrayParse(String str, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "binArrayFormat", IsComposable = true)]
    public static String binArrayFormat(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "binArrayLength", IsComposable = true)]
    public static Int32? binArrayLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "binArrayIndexOf", IsComposable = true)]
    public static Int32? binArrayIndexOf(byte[] array, Byte[] value)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "binArrayGet", IsComposable = true)]
    public static Byte[] binArrayGet(byte[] array, Int32 index)
    {
      return ExecuteUnsupported<Byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "binArraySet", IsComposable = true)]
    public static byte[] binArraySet(byte[] array, Int32 index, Byte[] value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "binArrayGetRange", IsComposable = true)]
    public static byte[] binArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binArraySetRange", IsComposable = true)]
    public static byte[] binArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "binArrayFillRange", IsComposable = true)]
    public static byte[] binArrayFillRange(byte[] array, Int32? index, Int32? count, Byte[] value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "binArrayToCollection", IsComposable = true)]
    public static byte[] binArrayToCollection(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "binArrayEnumerate", IsComposable = true)]
    public static IQueryable<IndexedBinaryRow> binArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<IndexedBinaryRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #endregion
    #region Regular array functions
    #region Boolean regular array functions

    [DbFunctionEx(StoreNamespace, "bArrayCreateRegular", IsComposable = true)]
    public static byte[] boolArrayCreateRegular(byte[] lengths, SizeEncoding countSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing);
    }

    [DbFunctionEx(StoreNamespace, "bArrayParseRegular", IsComposable = true)]
    public static byte[] boolArrayParseRegular(String str, SizeEncoding countSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing);
    }

    [DbFunctionEx(StoreNamespace, "bArrayFormatRegular", IsComposable = true)]
    public static String boolArrayFormatRegular(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "bArrayRank", IsComposable = true)]
    public static Int32? boolArrayRank(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "bArrayFlatLength", IsComposable = true)]
    public static Int32? boolArrayFlatLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "bArrayDimLengths", IsComposable = true)]
    public static byte[] boolArrayDimLengths(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "bArrayDimLength", IsComposable = true)]
    public static byte[] boolArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "bArrayGetFlat", IsComposable = true)]
    public static Boolean? boolArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "bArraySetFlat", IsComposable = true)]
    public static byte[] boolArraySetFlat(byte[] array, Int32? index, Boolean? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "bArrayGetFlatRange", IsComposable = true)]
    public static byte[] boolArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "bArraySetFlatRange", IsComposable = true)]
    public static byte[] boolArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "bArrayFillFlatRange", IsComposable = true)]
    public static byte[] boolArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Boolean? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "bArrayEnumerateFlat", IsComposable = true)]
    public static IQueryable<RegularIndexedBooleanRow> boolArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedBooleanRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "bArrayGetDim", IsComposable = true)]
    public static Boolean? boolArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "bArraySetDim", IsComposable = true)]
    public static byte[] boolArraySetDim(byte[] array, byte[] indices, Boolean? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "bArrayGetDimRange", IsComposable = true)]
    public static byte[] boolArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "bArraySetDimRange", IsComposable = true)]
    public static byte[] boolArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "bArrayFillDimRange", IsComposable = true)]
    public static byte[] boolArrayFillDimRange(byte[] array, byte[] ranges, Boolean? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "bArrayEnumerateDim", IsComposable = true)]
    public static IQueryable<RegularIndexedBooleanRow> boolArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedBooleanRow>>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlByte regular array functions

    [DbFunctionEx(StoreNamespace, "tiArrayCreateRegular", IsComposable = true)]
    public static byte[] byteArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayParseRegular", IsComposable = true)]
    public static byte[] byteArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayFormatRegular", IsComposable = true)]
    public static String byteArrayFormatRegular(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayRank", IsComposable = true)]
    public static Int32? byteArrayRank(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayFlatLength", IsComposable = true)]
    public static Int32? byteArrayFlatLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayDimLengths", IsComposable = true)]
    public static byte[] byteArrayDimLengths(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayDimLength", IsComposable = true)]
    public static byte[] byteArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayGetFlat", IsComposable = true)]
    public static Byte? byteArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<Byte?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "tiArraySetFlat", IsComposable = true)]
    public static byte[] byteArraySetFlat(byte[] array, Int32? index, Byte? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayGetFlatRange", IsComposable = true)]
    public static byte[] byteArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "tiArraySetFlatRange", IsComposable = true)]
    public static byte[] byteArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayFillFlatRange", IsComposable = true)]
    public static byte[] byteArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Byte? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "tiArrayEnumerateFlat", IsComposable = true)]
    public static IQueryable<RegularIndexedByteRow> byteArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedByteRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayGetDim", IsComposable = true)]
    public static Byte? byteArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteUnsupported<Byte?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "tiArraySetDim", IsComposable = true)]
    public static byte[] byteArraySetDim(byte[] array, byte[] indices, Byte? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayGetDimRange", IsComposable = true)]
    public static byte[] byteArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "tiArraySetDimRange", IsComposable = true)]
    public static byte[] byteArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayFillDimRange", IsComposable = true)]
    public static byte[] byteArrayFillDimRange(byte[] array, byte[] ranges, Byte? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "tiArrayEnumerateDim", IsComposable = true)]
    public static IQueryable<RegularIndexedByteRow> byteArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedByteRow>>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlInt16 regular array functions

    [DbFunctionEx(StoreNamespace, "siArrayCreateRegular", IsComposable = true)]
    public static byte[] int16ArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "siArrayParseRegular", IsComposable = true)]
    public static byte[] int16ArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "siArrayFormatRegular", IsComposable = true)]
    public static String int16ArrayFormatRegular(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "siArrayRank", IsComposable = true)]
    public static Int32? int16ArrayRank(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "siArrayFlatLength", IsComposable = true)]
    public static Int32? int16ArrayFlatLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "siArrayDimLengths", IsComposable = true)]
    public static byte[] int16ArrayDimLengths(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "siArrayDimLength", IsComposable = true)]
    public static byte[] int16ArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "siArrayGetFlat", IsComposable = true)]
    public static Int16? int16ArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<Int16?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "siArraySetFlat", IsComposable = true)]
    public static byte[] int16ArraySetFlat(byte[] array, Int32? index, Int16? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "siArrayGetFlatRange", IsComposable = true)]
    public static byte[] int16ArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "siArraySetFlatRange", IsComposable = true)]
    public static byte[] int16ArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "siArrayFillFlatRange", IsComposable = true)]
    public static byte[] int16ArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Int16? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "siArrayEnumerateFlat", IsComposable = true)]
    public static IQueryable<RegularIndexedInt16Row> int16ArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedInt16Row>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "siArrayGetDim", IsComposable = true)]
    public static Int16? int16ArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteUnsupported<Int16?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "siArraySetDim", IsComposable = true)]
    public static byte[] int16ArraySetDim(byte[] array, byte[] indices, Int16? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "siArrayGetDimRange", IsComposable = true)]
    public static byte[] int16ArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "siArraySetDimRange", IsComposable = true)]
    public static byte[] int16ArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "siArrayFillDimRange", IsComposable = true)]
    public static byte[] int16ArrayFillDimRange(byte[] array, byte[] ranges, Int16? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "siArrayEnumerateDim", IsComposable = true)]
    public static IQueryable<RegularIndexedInt16Row> int16ArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedInt16Row>>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region Int32 regular array functions

    [DbFunctionEx(StoreNamespace, "iArrayCreateRegular", IsComposable = true)]
    public static byte[] int32ArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "iArrayParseRegular", IsComposable = true)]
    public static byte[] int32ArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "iArrayFormatRegular", IsComposable = true)]
    public static String int32ArrayFormatRegular(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "iArrayRank", IsComposable = true)]
    public static Int32? int32ArrayRank(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "iArrayFlatLength", IsComposable = true)]
    public static Int32? int32ArrayFlatLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "iArrayDimLengths", IsComposable = true)]
    public static byte[] int32ArrayDimLengths(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "iArrayDimLength", IsComposable = true)]
    public static byte[] int32ArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "iArrayGetFlat", IsComposable = true)]
    public static Int32? int32ArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "iArraySetFlat", IsComposable = true)]
    public static byte[] int32ArraySetFlat(byte[] array, Int32? index, Int32? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "iArrayGetFlatRange", IsComposable = true)]
    public static byte[] int32ArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "iArraySetFlatRange", IsComposable = true)]
    public static byte[] int32ArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "iArrayFillFlatRange", IsComposable = true)]
    public static byte[] int32ArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Int32? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "iArrayEnumerateFlat", IsComposable = true)]
    public static IQueryable<RegularIndexedInt32Row> int32ArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedInt32Row>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "iArrayGetDim", IsComposable = true)]
    public static Int32? int32ArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "iArraySetDim", IsComposable = true)]
    public static byte[] int32ArraySetDim(byte[] array, byte[] indices, Int32? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "iArrayGetDimRange", IsComposable = true)]
    public static byte[] int32ArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "iArraySetDimRange", IsComposable = true)]
    public static byte[] int32ArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "iArrayFillDimRange", IsComposable = true)]
    public static byte[] int32ArrayFillDimRange(byte[] array, byte[] ranges, Int32? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "iArrayEnumerateDim", IsComposable = true)]
    public static IQueryable<RegularIndexedInt32Row> int32ArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedInt32Row>>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlInt64 regular array functions

    [DbFunctionEx(StoreNamespace, "biArrayCreateRegular", IsComposable = true)]
    public static byte[] int64ArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "biArrayParseRegular", IsComposable = true)]
    public static byte[] int64ArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "biArrayFormatRegular", IsComposable = true)]
    public static String int64ArrayFormatRegular(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "biArrayRank", IsComposable = true)]
    public static Int32? int64ArrayRank(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "biArrayFlatLength", IsComposable = true)]
    public static Int32? int64ArrayFlatLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "biArrayDimLengths", IsComposable = true)]
    public static byte[] int64ArrayDimLengths(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "biArrayDimLength", IsComposable = true)]
    public static byte[] int64ArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "biArrayGetFlat", IsComposable = true)]
    public static Int64? int64ArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "biArraySetFlat", IsComposable = true)]
    public static byte[] int64ArraySetFlat(byte[] array, Int32? index, Int64? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "biArrayGetFlatRange", IsComposable = true)]
    public static byte[] int64ArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "biArraySetFlatRange", IsComposable = true)]
    public static byte[] int64ArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "biArrayFillFlatRange", IsComposable = true)]
    public static byte[] int64ArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Int64? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "biArrayEnumerateFlat", IsComposable = true)]
    public static IQueryable<RegularIndexedInt64Row> int64ArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedInt64Row>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "biArrayGetDim", IsComposable = true)]
    public static Int64? int64ArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "biArraySetDim", IsComposable = true)]
    public static byte[] int64ArraySetDim(byte[] array, byte[] indices, Int64? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "biArrayGetDimRange", IsComposable = true)]
    public static byte[] int64ArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "biArraySetDimRange", IsComposable = true)]
    public static byte[] int64ArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "biArrayFillDimRange", IsComposable = true)]
    public static byte[] int64ArrayFillDimRange(byte[] array, byte[] ranges, Int64? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "biArrayEnumerateDim", IsComposable = true)]
    public static IQueryable<RegularIndexedInt64Row> int64ArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedInt64Row>>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlSingle regular array functions

    [DbFunctionEx(StoreNamespace, "sfArrayCreateRegular", IsComposable = true)]
    public static byte[] sglArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayParseRegular", IsComposable = true)]
    public static byte[] sglArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayFormatRegular", IsComposable = true)]
    public static String sglArrayFormatRegular(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayRank", IsComposable = true)]
    public static Int32? sglArrayRank(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayFlatLength", IsComposable = true)]
    public static Int32? sglArrayFlatLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayDimLengths", IsComposable = true)]
    public static byte[] sglArrayDimLengths(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayDimLength", IsComposable = true)]
    public static byte[] sglArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayGetFlat", IsComposable = true)]
    public static Single? sglArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<Single?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "sfArraySetFlat", IsComposable = true)]
    public static byte[] sglArraySetFlat(byte[] array, Int32? index, Single? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayGetFlatRange", IsComposable = true)]
    public static byte[] sglArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "sfArraySetFlatRange", IsComposable = true)]
    public static byte[] sglArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayFillFlatRange", IsComposable = true)]
    public static byte[] sglArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Single? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "sfArrayEnumerateFlat", IsComposable = true)]
    public static IQueryable<RegularIndexedSingleRow> sglArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedSingleRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayGetDim", IsComposable = true)]
    public static Single? sglArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteUnsupported<Single?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "sfArraySetDim", IsComposable = true)]
    public static byte[] sglArraySetDim(byte[] array, byte[] indices, Single? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayGetDimRange", IsComposable = true)]
    public static byte[] sglArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "sfArraySetDimRange", IsComposable = true)]
    public static byte[] sglArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayFillDimRange", IsComposable = true)]
    public static byte[] sglArrayFillDimRange(byte[] array, byte[] ranges, Single? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "sfArrayEnumerateDim", IsComposable = true)]
    public static IQueryable<RegularIndexedSingleRow> sglArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedSingleRow>>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlDouble regular array functions

    [DbFunctionEx(StoreNamespace, "dfArrayCreateRegular", IsComposable = true)]
    public static byte[] dblArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayParseRegular", IsComposable = true)]
    public static byte[] dblArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayFormatRegular", IsComposable = true)]
    public static String dblArrayFormatRegular(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayRank", IsComposable = true)]
    public static Int32? dblArrayRank(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayFlatLength", IsComposable = true)]
    public static Int32? dblArrayFlatLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayDimLengths", IsComposable = true)]
    public static byte[] dblArrayDimLengths(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayDimLength", IsComposable = true)]
    public static byte[] dblArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayGetFlat", IsComposable = true)]
    public static Double? dblArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<Double?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "dfArraySetFlat", IsComposable = true)]
    public static byte[] dblArraySetFlat(byte[] array, Int32? index, Double? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayGetFlatRange", IsComposable = true)]
    public static byte[] dblArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dfArraySetFlatRange", IsComposable = true)]
    public static byte[] dblArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayFillFlatRange", IsComposable = true)]
    public static byte[] dblArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Double? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "dfArrayEnumerateFlat", IsComposable = true)]
    public static IQueryable<RegularIndexedDoubleRow> dblArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedDoubleRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayGetDim", IsComposable = true)]
    public static Double? dblArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteUnsupported<Double?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "dfArraySetDim", IsComposable = true)]
    public static byte[] dblArraySetDim(byte[] array, byte[] indices, Double? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayGetDimRange", IsComposable = true)]
    public static byte[] dblArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "dfArraySetDimRange", IsComposable = true)]
    public static byte[] dblArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayFillDimRange", IsComposable = true)]
    public static byte[] dblArrayFillDimRange(byte[] array, byte[] ranges, Double? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "dfArrayEnumerateDim", IsComposable = true)]
    public static IQueryable<RegularIndexedDoubleRow> dblArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedDoubleRow>>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlDateTime regular array functions

    [DbFunctionEx(StoreNamespace, "dtArrayCreateRegular", IsComposable = true)]
    public static byte[] dtmArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayParseRegular", IsComposable = true)]
    public static byte[] dtmArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayFormatRegular", IsComposable = true)]
    public static String dtmArrayFormatRegular(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayRank", IsComposable = true)]
    public static Int32? dtmArrayRank(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayFlatLength", IsComposable = true)]
    public static Int32? dtmArrayFlatLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayDimLengths", IsComposable = true)]
    public static byte[] dtmArrayDimLengths(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayDimLength", IsComposable = true)]
    public static byte[] dtmArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayGetFlat", IsComposable = true)]
    public static DateTime? dtmArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<DateTime?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "dtArraySetFlat", IsComposable = true)]
    public static byte[] dtmArraySetFlat(byte[] array, Int32? index, DateTime? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayGetFlatRange", IsComposable = true)]
    public static byte[] dtmArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dtArraySetFlatRange", IsComposable = true)]
    public static byte[] dtmArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayFillFlatRange", IsComposable = true)]
    public static byte[] dtmArrayFillFlatRange(byte[] array, Int32? index, Int32? count, DateTime? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "dtArrayEnumerateFlat", IsComposable = true)]
    public static IQueryable<RegularIndexedDateTimeRow> dtmArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedDateTimeRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayGetDim", IsComposable = true)]
    public static DateTime? dtmArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteUnsupported<DateTime?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "dtArraySetDim", IsComposable = true)]
    public static byte[] dtmArraySetDim(byte[] array, byte[] indices, DateTime? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayGetDimRange", IsComposable = true)]
    public static byte[] dtmArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "dtArraySetDimRange", IsComposable = true)]
    public static byte[] dtmArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayFillDimRange", IsComposable = true)]
    public static byte[] dtmArrayFillDimRange(byte[] array, byte[] ranges, DateTime? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "dtArrayEnumerateDim", IsComposable = true)]
    public static IQueryable<RegularIndexedDateTimeRow> dtmArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedDateTimeRow>>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlGuid regular array functions

    [DbFunctionEx(StoreNamespace, "uidArrayCreateRegular", IsComposable = true)]
    public static byte[] guidArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayParseRegular", IsComposable = true)]
    public static byte[] guidArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayFormatRegular", IsComposable = true)]
    public static String guidArrayFormatRegular(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayRank", IsComposable = true)]
    public static Int32? guidArrayRank(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayFlatLength", IsComposable = true)]
    public static Int32? guidArrayFlatLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayDimLengths", IsComposable = true)]
    public static byte[] guidArrayDimLengths(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayDimLength", IsComposable = true)]
    public static byte[] guidArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayGetFlat", IsComposable = true)]
    public static Guid? guidArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<Guid?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "uidArraySetFlat", IsComposable = true)]
    public static byte[] guidArraySetFlat(byte[] array, Int32? index, Guid? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayGetFlatRange", IsComposable = true)]
    public static byte[] guidArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "uidArraySetFlatRange", IsComposable = true)]
    public static byte[] guidArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayFillFlatRange", IsComposable = true)]
    public static byte[] guidArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Guid? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "uidArrayEnumerateFlat", IsComposable = true)]
    public static IQueryable<RegularIndexedGuidRow> guidArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedGuidRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayGetDim", IsComposable = true)]
    public static Guid? guidArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteUnsupported<Guid?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "uidArraySetDim", IsComposable = true)]
    public static byte[] guidArraySetDim(byte[] array, byte[] indices, Guid? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayGetDimRange", IsComposable = true)]
    public static byte[] guidArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "uidArraySetDimRange", IsComposable = true)]
    public static byte[] guidArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayFillDimRange", IsComposable = true)]
    public static byte[] guidArrayFillDimRange(byte[] array, byte[] ranges, Guid? value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "uidArrayEnumerateDim", IsComposable = true)]
    public static IQueryable<RegularIndexedGuidRow> guidArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedGuidRow>>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region String regular array functions

    [DbFunctionEx(StoreNamespace, "strArrayCreateRegular", IsComposable = true)]
    public static byte[] strArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "strArrayCreateRegularByCpId", IsComposable = true)]
    public static byte[] strArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, SizeEncoding itemSizing, Int32? cpId)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, itemSizing, cpId);
    }

    [DbFunctionEx(StoreNamespace, "strArrayCreateRegularByCpName", IsComposable = true)]
    public static byte[] strArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, SizeEncoding itemSizing, String cpName)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, itemSizing, cpName);
    }

    [DbFunctionEx(StoreNamespace, "strArrayParseRegular", IsComposable = true)]
    public static byte[] strArrayParseRegular(String str, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "strArrayParseRegularByCpId", IsComposable = true)]
    public static byte[] strArrayParseRegular(String str, SizeEncoding countSizing, SizeEncoding itemSizing, Int32? cpId)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing, cpId);
    }

    [DbFunctionEx(StoreNamespace, "strArrayParseRegularByCpName", IsComposable = true)]
    public static byte[] strArrayParseRegular(String str, SizeEncoding countSizing, SizeEncoding itemSizing, String cpName)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing, cpName);
    }

    [DbFunctionEx(StoreNamespace, "strArrayFormatRegular", IsComposable = true)]
    public static String strArrayFormatRegular(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "strArrayRank", IsComposable = true)]
    public static Int32? strArrayRank(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "strArrayFlatLength", IsComposable = true)]
    public static Int32? strArrayFlatLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "strArrayDimLengths", IsComposable = true)]
    public static byte[] strArrayDimLengths(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "strArrayDimLength", IsComposable = true)]
    public static byte[] strArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "strArrayGetFlat", IsComposable = true)]
    public static String strArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "strArraySetFlat", IsComposable = true)]
    public static byte[] strArraySetFlat(byte[] array, Int32? index, String value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "strArrayGetFlatRange", IsComposable = true)]
    public static byte[] strArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strArraySetFlatRange", IsComposable = true)]
    public static byte[] strArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "strArrayFillFlatRange", IsComposable = true)]
    public static byte[] strArrayFillFlatRange(byte[] array, Int32? index, Int32? count, String value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "strArrayEnumerateFlat", IsComposable = true)]
    public static IQueryable<RegularIndexedStringRow> strArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedStringRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strArrayGetDim", IsComposable = true)]
    public static String strArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "strArraySetDim", IsComposable = true)]
    public static byte[] strArraySetDim(byte[] array, byte[] indices, String value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "strArrayGetDimRange", IsComposable = true)]
    public static byte[] strArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "strArraySetDimRange", IsComposable = true)]
    public static byte[] strArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "strArrayFillDimRange", IsComposable = true)]
    public static byte[] strArrayFillDimRange(byte[] array, byte[] ranges, String value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "strArrayEnumerateDim", IsComposable = true)]
    public static IQueryable<RegularIndexedStringRow> strArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedStringRow>>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlBinary regular array functions

    [DbFunctionEx(StoreNamespace, "binArrayCreateRegular", IsComposable = true)]
    public static byte[] binArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "binArrayParseRegular", IsComposable = true)]
    public static byte[] binArrayParseRegular(String str, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "binArrayFormatRegular", IsComposable = true)]
    public static String binArrayFormatRegular(byte[] array)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "binArrayRank", IsComposable = true)]
    public static Int32? binArrayRank(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "binArrayFlatLength", IsComposable = true)]
    public static Int32? binArrayFlatLength(byte[] array)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "binArrayDimLengths", IsComposable = true)]
    public static byte[] binArrayDimLengths(byte[] array)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "binArrayDimLength", IsComposable = true)]
    public static byte[] binArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "binArrayGetFlat", IsComposable = true)]
    public static Byte[] binArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteUnsupported<Byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "binArraySetFlat", IsComposable = true)]
    public static byte[] binArraySetFlat(byte[] array, Int32? index, Byte[] value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "binArrayGetFlatRange", IsComposable = true)]
    public static byte[] binArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binArraySetFlatRange", IsComposable = true)]
    public static byte[] binArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "binArrayFillFlatRange", IsComposable = true)]
    public static byte[] binArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Byte[] value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "binArrayEnumerateFlat", IsComposable = true)]
    public static IQueryable<RegularIndexedBinaryRow> binArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedBinaryRow>>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binArrayGetDim", IsComposable = true)]
    public static Byte[] binArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteUnsupported<Byte[]>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "binArraySetDim", IsComposable = true)]
    public static byte[] binArraySetDim(byte[] array, byte[] indices, Byte[] value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "binArrayGetDimRange", IsComposable = true)]
    public static byte[] binArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "binArraySetDimRange", IsComposable = true)]
    public static byte[] binArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "binArrayFillDimRange", IsComposable = true)]
    public static byte[] binArrayFillDimRange(byte[] array, byte[] ranges, Byte[] value)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "binArrayEnumerateDim", IsComposable = true)]
    public static IQueryable<RegularIndexedBinaryRow> binArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return ExecuteUnsupported<IQueryable<RegularIndexedBinaryRow>>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #endregion
    #region Uri functions

    [DbFunctionEx(StoreNamespace, "uriCheckHostName", IsComposable = true)]
    public static UriHostNameType uriCheckHostName(String hostName)
    {
      return ExecuteUnsupported<UriHostNameType>(MethodBase.GetCurrentMethod(), hostName);
    }

    [DbFunctionEx(StoreNamespace, "uriCheckSchemeName", IsComposable = true)]
    public static Boolean uriCheckSchemeName(String schemeName)
    {
      return ExecuteUnsupported<Boolean>(MethodBase.GetCurrentMethod(), schemeName);
    }

    [DbFunctionEx(StoreNamespace, "uriFromHex", IsComposable = true)]
    public static Int32 uriFromHex(String digit)
    {
      return ExecuteUnsupported<Int32>(MethodBase.GetCurrentMethod(), digit);
    }

    [DbFunctionEx(StoreNamespace, "uriHexEscape", IsComposable = true)]
    public static String uriHexEscape(String str)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), str);
    }

    [DbFunctionEx(StoreNamespace, "uriHexUnescape", IsComposable = true)]
    public static String uriHexUnescape(String pattern, Int32? index, Int32? count)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), pattern, index, count);
    }

    [DbFunctionEx(StoreNamespace, "uriIsHexDigit", IsComposable = true)]
    public static Boolean uriIsHexDigit(String character)
    {
      return ExecuteUnsupported<Boolean>(MethodBase.GetCurrentMethod(), character);
    }

    [DbFunctionEx(StoreNamespace, "uriIsHexEncoding", IsComposable = true)]
    public static Boolean uriIsHexEncoding(String pattern, Int32 index)
    {
      return ExecuteUnsupported<Boolean>(MethodBase.GetCurrentMethod(), pattern, index);
    }

    [DbFunctionEx(StoreNamespace, "uriIsWellFormedUriString", IsComposable = true)]
    public static Boolean uriIsWellFormedUriString(String uriString, UriKind uriKind)
    {
      return ExecuteUnsupported<Boolean>(MethodBase.GetCurrentMethod(), uriString, uriKind);
    }

    [DbFunctionEx(StoreNamespace, "uriEscape", IsComposable = true)]
    public static String uriEscapeUri(String str)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), str);
    }

    [DbFunctionEx(StoreNamespace, "uriEscapeData", IsComposable = true)]
    public static String uriEscapeData(String str)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), str);
    }

    [DbFunctionEx(StoreNamespace, "uriUnescapeData", IsComposable = true)]
    public static String uriUnescapeData(String str)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), str);
    }

    [DbFunctionEx(StoreNamespace, "uriGetAbsolutePath", IsComposable = true)]
    public static String uriGetAbsolutePath(String uriString)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetAbsoluteUri", IsComposable = true)]
    public static String uriGetAbsoluteUri(String uriString)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetAuthority", IsComposable = true)]
    public static String uriGetAuthority(String uriString)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetDnsSafeHost", IsComposable = true)]
    public static String uriGetDnsSafeHost(String uriString)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetFragment", IsComposable = true)]
    public static String uriGetFragment(String uriString)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetHost", IsComposable = true)]
    public static String uriGetHost(String uriString)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetHostNameType", IsComposable = true)]
    public static UriHostNameType uriGetHostNameType(String uriString)
    {
      return ExecuteUnsupported<UriHostNameType>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriIsAbsoluteUri", IsComposable = true)]
    public static Boolean uriIsAbsoluteUri(String uriString)
    {
      return ExecuteUnsupported<Boolean>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriIsDefaultPort", IsComposable = true)]
    public static Boolean uriIsDefaultPort(String uriString)
    {
      return ExecuteUnsupported<Boolean>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriIsFile", IsComposable = true)]
    public static Boolean uriIsFile(String uriString)
    {
      return ExecuteUnsupported<Boolean>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriIsLoopback", IsComposable = true)]
    public static Boolean uriIsLoopback(String uriString)
    {
      return ExecuteUnsupported<Boolean>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriIsUnc", IsComposable = true)]
    public static Boolean uriIsUnc(String uriString)
    {
      return ExecuteUnsupported<Boolean>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetLocalPath", IsComposable = true)]
    public static String uriGetLocalPath(String uriString)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetOriginalString", IsComposable = true)]
    public static String uriGetOriginalString(String uriString)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetPathAndQuery", IsComposable = true)]
    public static String uriGetPathAndQuery(String uriString)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetPort", IsComposable = true)]
    public static Int32 uriGetPort(String uriString)
    {
      return ExecuteUnsupported<Int32>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetQuery", IsComposable = true)]
    public static String uriGetQuery(String uriString)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetScheme", IsComposable = true)]
    public static String uriGetScheme(String uriString)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriUserEscaped", IsComposable = true)]
    public static Boolean uriUserEscaped(String uriString)
    {
      return ExecuteUnsupported<Boolean>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriGetUserInfo", IsComposable = true)]
    public static String uriGetUserInfo(String uriString)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), uriString);
    }

    [DbFunctionEx(StoreNamespace, "uriBuildString", IsComposable = true)]
    public static String uriBuild(String scheme, String userName, String password, String host, Int32 port, String path, String query, String fragment)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), scheme, userName, password, host, port, path, query, fragment);
    }

    [DbFunctionEx(ContextNamespace, "uriGetSegments", IsComposable = true)]
    public static IQueryable<StringRow> uriGetSegments(String uriString)
    {
      return ExecuteUnsupported<IQueryable<StringRow>>(MethodBase.GetCurrentMethod(), uriString);
    }

    #endregion
    #region File functions
    #region Xml file functions

    [DbFunctionEx(StoreNamespace, "fileReadAllXml", IsComposable = true)]
    public static String fileReadAllXml(String path)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileWriteAllXml", IsComposable = true)]
    public static Int64? fileWriteAllXml(String path, String xml)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, xml);
    }

    #endregion
    #region Binary file functions

    [DbFunctionEx(StoreNamespace, "fileReadAllBinary", IsComposable = true)]
    public static byte[] fileReadAllBinary(String path)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileReadBinary", IsComposable = true)]
    public static byte[] fileReadBinary(String path, Int64? offset, Int64? count)
    {
      return ExecuteUnsupported<byte[]>(MethodInfo.GetCurrentMethod(), path, offset, count);
    }

    [DbFunctionEx(ContextNamespace, "fileReadSizedBlocks", IsComposable = true)]
    public static IQueryable<FileBlockRow> fileReadSizedBlocks(String path, Int64? offset, SizeEncoding sizing, Int32? maxCount)
    {
      return ExecuteUnsupported<IQueryable<FileBlockRow>>(MethodInfo.GetCurrentMethod(), path, offset, sizing, maxCount);
    }

    [DbFunctionEx(ContextNamespace, "fileReadTerminatedBlocks", IsComposable = true)]
    public static IQueryable<FileBlockRow> fileReadTerminatedBlocks(String path, Int64? offset, byte[] searchTerminator, Boolean omitTerminator, Int32? maxCount)
    {
      return ExecuteUnsupported<IQueryable<FileBlockRow>>(MethodInfo.GetCurrentMethod(), path, offset, searchTerminator, omitTerminator, maxCount);
    }

    [DbFunctionEx(StoreNamespace, "fileWriteAllBinary", IsComposable = true)]
    public static Int64? fileWriteAllBinary(String path, byte[] bytes)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, bytes);
    }

    [DbFunctionEx(StoreNamespace, "fileWriteBinary", IsComposable = true)]
    public static Int64? fileWriteBinary(String path, byte[] bytes, Int64? offset, Boolean insert)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, bytes, offset, insert);
    }

    [DbFunctionEx(StoreNamespace, "fileWriteSizedBlock", IsComposable = true)]
    public static Int64? fileWriteSizedBlock(String path, byte[] bytes, SizeEncoding sizing, Int64? offset, Boolean insert)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, bytes, sizing, offset, insert);
    }

    [DbFunctionEx(StoreNamespace, "fileWriteTerminatedBlock", IsComposable = true)]
    public static Int64? fileWriteTerminatedBlock(String path, byte[] bytes, byte[] terminator, Int64? offset, Boolean insert)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, bytes, terminator, offset, insert);
    }

    [DbFunctionEx(StoreNamespace, "fileRemoveBlock", IsComposable = true)]
    public static Boolean? fileRemoveBlock(String path, Int64 offset, Int64? length)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path, offset, length);
    }

    [DbFunctionEx(StoreNamespace, "fileSearchBinary", IsComposable = true)]
    public static Int64? fileSearchBinary(String path, Int64? offset, byte[] pattern)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, offset, pattern);
    }

    [DbFunctionEx(StoreNamespace, "fileSearchBinaryLast", IsComposable = true)]
    public static Int64? fileSearchBinaryLast(String path, Int64? offset, byte[] pattern)
    {
      return ExecuteUnsupported<Int64>(MethodInfo.GetCurrentMethod(), path, offset, pattern);
    }

    #endregion
    #region Text file functions

    [DbFunctionEx(StoreNamespace, "fileDetectCodePage", IsComposable = true)]
    public static Int32? fileDetectCodePage(String path)
    {
      return ExecuteUnsupported<Int32?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileReadAllText", IsComposable = true)]
    public static String fileReadAllText(String path, Boolean detectEncoding)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path, detectEncoding);
    }

    [DbFunctionEx(StoreNamespace, "fileReadAllTextByCpId", IsComposable = true)]
    public static String fileReadAllTextByCpId(String path, Boolean detectEncoding, Int32? cpId)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path, detectEncoding, cpId);
    }

    [DbFunctionEx(StoreNamespace, "fileReadAllTextByCpName", IsComposable = true)]
    public static String fileReadAllTextByCpName(String path, Boolean detectEncoding, String cpName)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path, detectEncoding, cpName);
    }

    [DbFunctionEx(StoreNamespace, "fileReadText", IsComposable = true)]
    public static String fileReadText(String path, Int64? offset, Int64? count, Boolean detectEncoding)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path, offset, count, detectEncoding);
    }

    [DbFunctionEx(StoreNamespace, "fileReadTextByCpId", IsComposable = true)]
    public static String fileReadTextByCpId(String path, Int64? offset, Int64? count, Boolean detectEncoding, Int32? cpId)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path, offset, count, detectEncoding, cpId);
    }

    [DbFunctionEx(StoreNamespace, "fileReadTextByCpName", IsComposable = true)]
    public static String fileReadTextByCpName(String path, Int64? offset, Int64? count, Boolean detectEncoding, String cpName)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path, offset, count, detectEncoding, cpName);
    }

    [DbFunctionEx(ContextNamespace, "fileReadLines", IsComposable = true)]
    public static IQueryable<FileLineRow> fileReadLines(String path, Int64? offset, String searchTerminator, String delimiter, String newTerminator, Int32? maxCount, Boolean detectEncoding)
    {
      return ExecuteUnsupported<IQueryable<FileLineRow>>(MethodInfo.GetCurrentMethod(), path, offset, searchTerminator, delimiter, newTerminator, maxCount, detectEncoding);
    }

    [DbFunctionEx(ContextNamespace, "fileReadLinesByCpId", IsComposable = true)]
    public static IQueryable<FileLineRow> fileReadLinesByCpId(String path, Int64? offset, String searchTerminator, String delimiter, String newTerminator, Int32? maxCount, Boolean detectEncoding, Int32? cpId)
    {
      return ExecuteUnsupported<IQueryable<FileLineRow>>(MethodInfo.GetCurrentMethod(), path, offset, searchTerminator, delimiter, newTerminator, maxCount, detectEncoding, cpId);
    }

    [DbFunctionEx(ContextNamespace, "fileReadLinesByCpName", IsComposable = true)]
    public static IQueryable<FileLineRow> fileReadLinesByCpName(String path, Int64? offset, String searchTerminator, String delimiter, String newTerminator, Int32? maxCount, Boolean detectEncoding, String cpName)
    {
      return ExecuteUnsupported<IQueryable<FileLineRow>>(MethodInfo.GetCurrentMethod(), path, offset, searchTerminator, delimiter, newTerminator, maxCount, detectEncoding, cpName);
    }

    [DbFunctionEx(StoreNamespace, "fileWriteAllText", IsComposable = true)]
    public static Int64? fileWriteAllText(String path, String chars, Boolean writeEncoding)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, chars, writeEncoding);
    }

    [DbFunctionEx(StoreNamespace, "fileWriteAllTextByCpId", IsComposable = true)]
    public static Int64? fileWriteAllTextByCpId(String path, String chars, Boolean writeEncoding, Int32? cpId)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, chars, writeEncoding, cpId);
    }

    [DbFunctionEx(StoreNamespace, "fileWriteAllTextByCpName", IsComposable = true)]
    public static Int64? fileWriteAllTextByCpName(String path, String chars, Boolean writeEncoding, String cpName)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, chars, writeEncoding, cpName);
    }

    [DbFunctionEx(StoreNamespace, "fileWriteText", IsComposable = true)]
    public static Int64? fileWriteText(String path, String chars, String terminator, Int64? offset, Boolean insert, Byte useEncoding)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, chars, terminator, offset, insert, useEncoding);
    }

    [DbFunctionEx(StoreNamespace, "fileWriteTextByCpId", IsComposable = true)]
    public static Int64? fileWriteTextByCpId(String path, String chars, String terminator, Int64? offset, Boolean insert, Byte useEncoding, Int32? cpId)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, chars, terminator, offset, insert, useEncoding, cpId);
    }

    [DbFunctionEx(StoreNamespace, "fileWriteTextByCpName", IsComposable = true)]
    public static Int64? fileWriteTextByCpName(String path, String chars, String terminator, Int64? offset, Boolean insert, Byte useEncoding, String cpName)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, chars, terminator, offset, insert, useEncoding, cpName);
    }

    [DbFunctionEx(StoreNamespace, "fileSearchText", IsComposable = true)]
    public static Int64? fileSearchText(String path, Int64? offset, Int32? skip, String pattern, Boolean detectEncoding)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, offset, skip, pattern, detectEncoding);
    }

    [DbFunctionEx(StoreNamespace, "fileSearchTextByCpId", IsComposable = true)]
    public static Int64? fileSearchTextByCpId(String path, Int64? offset, Int32? skip, String pattern, Boolean detectEncoding, Int32? cpId)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, offset, skip, pattern, detectEncoding, cpId);
    }

    [DbFunctionEx(StoreNamespace, "fileSearchTextByCpName", IsComposable = true)]
    public static Int64? fileSearchTextByCpName(String path, Int64? offset, Int32? skip, String pattern, Boolean detectEncoding, String cpName)
    {
      return ExecuteUnsupported<Int64?>(MethodInfo.GetCurrentMethod(), path, offset, skip, pattern, detectEncoding, cpName);
    }

    #endregion
    #region Directory manipulation functions

    [DbFunctionEx(StoreNamespace, "dirCreate", IsComposable = true)]
    public static Boolean? dirCreate(String path)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "dirDelete", IsComposable = true)]
    public static Boolean? dirDelete(String path, Boolean? recursive)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path, recursive);
    }

    [DbFunctionEx(StoreNamespace, "dirMove", IsComposable = true)]
    public static Boolean? dirMove(String sourcePath, String targetPath)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), sourcePath, targetPath);
    }

    [DbFunctionEx(StoreNamespace, "dirExists", IsComposable = true)]
    public static Boolean? dirExists(String path)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(ContextNamespace, "dirEnumerate", IsComposable = true)]
    public static IQueryable<FileSystemRow> dirEnumerate(String path, String searchPattern, Int32? maxDepth, FileSystemTraversalOptions? traversalOptions)
    {
      return ExecuteUnsupported<IQueryable<FileSystemRow>>(MethodInfo.GetCurrentMethod(), path, searchPattern, maxDepth, traversalOptions);
    }

    #endregion
    #region File manipulation functions

    [DbFunctionEx(StoreNamespace, "fileDelete", IsComposable = true)]
    public static Boolean? fileDelete(String path)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileCopy", IsComposable = true)]
    public static Boolean? fileCopy(String sourcePath, String targetPath, Boolean overwrite)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), sourcePath, targetPath, overwrite);
    }

    [DbFunctionEx(StoreNamespace, "fileMove", IsComposable = true)]
    public static Boolean? fileMove(String sourcePath, String targetPath)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), sourcePath, targetPath);
    }

    [DbFunctionEx(StoreNamespace, "fileReplace", IsComposable = true)]
    public static Boolean? fileReplace(String sourcePath, String targetPath, String targetBackupFilename)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), sourcePath, targetPath, targetBackupFilename);
    }

    [DbFunctionEx(StoreNamespace, "fileEncrypt", IsComposable = true)]
    public static Boolean? fileEncrypt(String path)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileDecrypt", IsComposable = true)]
    public static Boolean? fileDecrypt(String path)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileExists", IsComposable = true)]
    public static Boolean? fileExists(String path)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileTruncate", IsComposable = true)]
    public static Boolean? fileTruncate(String path)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileGetAttributes", IsComposable = true)]
    public static FileAttributes? fileGetAttributes(String path)
    {
      return ExecuteUnsupported<FileAttributes?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileGetCreationTime", IsComposable = true)]
    public static DateTime? fileGetCreationTime(String path)
    {
      return ExecuteUnsupported<DateTime?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileGetCreationTimeUtc", IsComposable = true)]
    public static DateTime? fileGetCreationTimeUtc(String path)
    {
      return ExecuteUnsupported<DateTime?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileGetLastAccessTime", IsComposable = true)]
    public static DateTime? fileGetLastAccessTime(String path)
    {
      return ExecuteUnsupported<DateTime?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileGetLastAccessTimeUtc", IsComposable = true)]
    public static DateTime? fileGetLastAccessTimeUtc(String path)
    {
      return ExecuteUnsupported<DateTime?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileGetLastWriteTime", IsComposable = true)]
    public static DateTime? fileGetLastWriteTime(String path)
    {
      return ExecuteUnsupported<DateTime?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileGetLastWriteTimeUtc", IsComposable = true)]
    public static DateTime? fileGetLastWriteTimeUtc(String path)
    {
      return ExecuteUnsupported<DateTime?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "fileSetAttributes", IsComposable = true)]
    public static Boolean? fileSetAttributes(String path, FileAttributes attributes)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path, attributes);
    }

    [DbFunctionEx(StoreNamespace, "fileSetCreationTime", IsComposable = true)]
    public static Boolean? fileSetCreationTime(String path, DateTime creationTime)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path, creationTime);
    }

    [DbFunctionEx(StoreNamespace, "fileSetCreationTimeUtc", IsComposable = true)]
    public static Boolean? fileSetCreationTimeUtc(String path, DateTime creationTimeUtc)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path, creationTimeUtc);
    }

    [DbFunctionEx(StoreNamespace, "fileSetLastAccessTime", IsComposable = true)]
    public static Boolean? fileSetLastAccessTime(String path, DateTime lastAccessTime)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path, lastAccessTime);
    }

    [DbFunctionEx(StoreNamespace, "fileSetLastAccessTimeUtc", IsComposable = true)]
    public static Boolean? fileSetLastAccessTimeUtc(String path, DateTime lastAccessTimeUtc)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path, lastAccessTimeUtc);
    }

    [DbFunctionEx(StoreNamespace, "fileSetLastWriteTime", IsComposable = true)]
    public static Boolean? fileSetLastWriteTime(String path, DateTime lastWriteTime)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path, lastWriteTime);
    }

    [DbFunctionEx(StoreNamespace, "fileSetLastWriteTimeUtc", IsComposable = true)]
    public static Boolean? fileSetLastWriteTimeUtc(String path, DateTime lastWriteTimeUtc)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path, lastWriteTimeUtc);
    }

    #endregion
    #region Path manipulation functions

    [DbFunctionEx(StoreNamespace, "pathChangeExtension", IsComposable = true)]
    public static String pathChangeExtension(String path, String extension)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path, extension);
    }

    [DbFunctionEx(StoreNamespace, "pathGetDirectoryName", IsComposable = true)]
    public static String pathGetDirectoryName(String path)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "pathGetExtension", IsComposable = true)]
    public static String pathGetExtension(String path)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "pathGetFileName", IsComposable = true)]
    public static String pathGetFileName(String path)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "pathGetFileNameWithoutExtension", IsComposable = true)]
    public static String pathGetFileNameWithoutExtension(String path)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "pathGetRoot", IsComposable = true)]
    public static String pathGetPathRoot(String path)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "pathGetRandomFileName", IsComposable = true)]
    public static String pathGetRandomFileName()
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod());
    }

    [DbFunctionEx(StoreNamespace, "pathHasExtension", IsComposable = true)]
    public static Boolean? pathHasExtension(String path)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "pathIsRooted", IsComposable = true)]
    public static Boolean? pathIsPathRooted(String path)
    {
      return ExecuteUnsupported<Boolean?>(MethodInfo.GetCurrentMethod(), path);
    }

    [DbFunctionEx(StoreNamespace, "pathCombine", IsComposable = true)]
    public static String pathCombine(String path)
    {
      return ExecuteUnsupported<String>(MethodInfo.GetCurrentMethod(), path);
    }

    #endregion
    #endregion
    #endregion
  }
}
