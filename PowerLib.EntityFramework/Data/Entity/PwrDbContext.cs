using System;
using System.Linq;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Metadata.Edm;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO.Compression;
using System.Security.Cryptography;
using PowerLib.System.Linq;

namespace PowerLib.System.Data.Entity
{
  public class PwrDbContext : DbContext
  {
    private const string ContextNamespace = "PwrDbContext";
    private const string StoreNamespace = "PwrDbStore";

    #region Constructors

    public PwrDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
      : base(objectContext, dbContextOwnsObjectContext)
    {
    }

    public PwrDbContext(DbConnection existingConnection, bool contextOwnsConnection)
      : base(existingConnection, contextOwnsConnection)
    {
    }

    public PwrDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
      : base(existingConnection, model, contextOwnsConnection)
    {
    }

    public PwrDbContext(string nameOrConnectionString)
      : base(nameOrConnectionString)
    {
    }

    protected PwrDbContext(string nameOrConnectionString, DbCompiledModel model)
      : base(model)
    {
    }

    protected PwrDbContext(DbCompiledModel model)
      : base(model)
    {
    }

    protected PwrDbContext()
      : base()
    {
    }

    static PwrDbContext()
    {
      Database.SetInitializer<PwrDbContext>(null);
    }

    #endregion
    #region Internal methods

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
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
      modelBuilder.Conventions.Add(new FunctionsConvention(typeof(PwrDbContext), "pwrlib", "value", StoreNamespace));
      base.OnModelCreating(modelBuilder);
    }

    private T ExecuteUnsupported<T>(MethodBase method, params object[] args)
    {
      DbFunctionAttribute attrFunction = method.GetCustomAttribute<DbFunctionAttribute>();
      if (attrFunction == null)
        throw new InvalidOperationException("Method must be marked by'DbFunction' attribute.");
      throw new NotSupportedException(string.Format("Function '{0}' execution is not supported in runtime.", attrFunction.FunctionName));
    }

    private T ExecuteScalarQuery<T>(MethodBase method, params object[] args)
    {
      DbFunctionAttribute attrFunction = method.GetCustomAttribute<DbFunctionAttribute>();
      if (attrFunction == null)
        throw new InvalidOperationException(string.Format("Method '{0}' must be marked by 'DbFunction' attribute.", method.Name));
      DbFunctionExAttribute attrFunctionEx = attrFunction as DbFunctionExAttribute;
      IList<EdmFunction> functions = ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace.GetFunctions(attrFunction.FunctionName, attrFunction.NamespaceName, DataSpace.SSpace, true);
      if (functions.Count == 0)
        throw new InvalidOperationException(string.Format("Function '{0}' is not found in metadata.", attrFunction.FunctionName));
      return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<T>(
        (args == null || args.Length == 0) && attrFunctionEx != null && attrFunctionEx.IsNiladic ?
          string.Format("SELECT [{0}]", functions[0].Name) :
          string.Format("SELECT [{1}].[{0}]({2})", functions[0].Name, functions[0].Schema, string.Join(", ", Enumerable.Range(0, args.Length).Select(i => string.Format("@p{0}", i)))),
        args).First();
    }

    private IQueryable<T> CreateQuery<T>(MethodBase method, params object[] args)
    {
      DbFunctionAttribute attrFunction = method.GetCustomAttribute<DbFunctionAttribute>();
      if (attrFunction == null)
        throw new InvalidOperationException("Method must be marked by 'DbFunction' attribute.");
      ParameterInfo[] parameters = method.GetParameters();
      return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<T>(
        string.Format("[{1}].[{0}]({2})", attrFunction.FunctionName, attrFunction.NamespaceName, string.Join(", ", parameters.Select(p => string.Format("@{0}", p.Name)))),
        args
          .Select((v, i) => v != null ?
            new ObjectParameter(parameters[i].Name, v) :
            new ObjectParameter(parameters[i].Name, parameters[i].ParameterType.With(t => Nullable.GetUnderlyingType(t) ?? t).With(t => t.IsEnum ? Enum.GetUnderlyingType(t) : t)))
          .ToArray());
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
    public Boolean? regexIsMatch(String input, String pattern, RegexOptions options)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, pattern, options);
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
    public String regexReplace(String input, String pattern, String replacement, RegexOptions options)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, pattern, replacement, options);
    }

    /// <summary>
    /// Escapes a minimal set of characters (\, *, +, ?, |, {, [, (,), ^, $,., #, and white space) by replacing them with their escape codes. This instructs the regular expression engine to interpret these characters literally rather than as metacharacters.
    /// </summary>
    /// <param name="input">The input string that contains the text to convert.</param>
    /// <returns>A string of characters with metacharacters converted to their escaped form.</returns>
    [DbFunctionEx(StoreNamespace, "regexEscape", IsComposable = true)]
    public String regexEscape(String input)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Converts any escaped characters in the input string.
    /// </summary>
    /// <param name="input">The input string containing the text to convert.</param>
    /// <returns>A string of characters with any escaped characters converted to their unescaped form.</returns>
    [DbFunctionEx(StoreNamespace, "regexUnescape", IsComposable = true)]
    public String regexUnescape(String input)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Splits an input string into an array of substrings at the positions defined by a specified regular expression pattern. Specified options modify the matching operation.
    /// </summary>
    /// <param name="input">The string to split.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
    /// <returns></returns>
    [DbFunctionEx(StoreNamespace, "regexSplit", IsComposable = true)]
    public IQueryable<StringRow> regexSplit(String input, String pattern, RegexOptions options)
    {
      return CreateQuery<StringRow>(MethodBase.GetCurrentMethod(), input, pattern, (int)options);
    }

    /// <summary>
    /// Searches the specified input string for all occurrences of a specified regular expression, using the specified matching options.
    /// </summary>
    /// <param name="input">The string to search for a match.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specify options for matching.</param>
    /// <returns>A collection of the RegexMatchRow objects found by the search. If no matches are found, the method returns an empty collection object.</returns>
		[DbFunctionEx(StoreNamespace, "regexMatches", IsComposable = true)]
    public IQueryable<RegexMatchRow> regexMatches(String input, String pattern, RegexOptions options)
    {
      return CreateQuery<RegexMatchRow>(MethodBase.GetCurrentMethod(), input, pattern, (int)options);
    }

    #endregion
    #region String functions
    #region String manipulation functions

    [DbFunctionEx(StoreNamespace, "strInsert", IsComposable = true)]
    public String strInsert(String input, Int32? index, String value)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "strRemove", IsComposable = true)]
    public String strRemove(String input, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strReplace", IsComposable = true)]
    public String strReplace(String input, String pattern, String replacement)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, pattern, replacement);
    }

    [DbFunctionEx(StoreNamespace, "strSubstring", IsComposable = true)]
    public String strSubstring(String input, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strReverse", IsComposable = true)]
    public String strReverse(String input)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strReplicate", IsComposable = true)]
    public String strReplicate(String input, Int32 count)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, count);
    }

    [DbFunctionEx(StoreNamespace, "strPadLeft", IsComposable = true)]
    public String strPadLeft(String input, String padding, Int32 width)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, padding, width);
    }

    [DbFunctionEx(StoreNamespace, "strPadRight", IsComposable = true)]
    public String strPadRight(String input, String padding, Int32 width)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, padding, width);
    }

    [DbFunctionEx(StoreNamespace, "strCutLeft", IsComposable = true)]
    public String strCutLeft(String input, Int32 width)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, width);
    }

    [DbFunctionEx(StoreNamespace, "strCutRight", IsComposable = true)]
    public String strCutRight(String input, Int32 width)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, width);
    }

    [DbFunctionEx(StoreNamespace, "strTrim", IsComposable = true)]
    public String strTrim(String input, String trimming)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, trimming);
    }

    [DbFunctionEx(StoreNamespace, "strTrimLeft", IsComposable = true)]
    public String strTrimLeft(String input, String trimming)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, trimming);
    }

    [DbFunctionEx(StoreNamespace, "strTrimRight", IsComposable = true)]
    public String strTrimRight(String input, String trimming)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, trimming);
    }

    [DbFunctionEx(StoreNamespace, "strQuote", IsComposable = true)]
    public String strQuote(String input, [DbFunctionParameter(DbTypeName = "nchar"), MaxLength(1), MinLength(1)] String quote, [DbFunctionParameter(DbTypeName = "nchar"), MaxLength(1), MinLength(1)] String escape)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, quote, escape);
    }

    [DbFunctionEx(StoreNamespace, "strUnquote", IsComposable = true)]
    public String strUnquote(String input, [DbFunctionParameter(DbTypeName = "nchar"), MaxLength(1), MinLength(1)] String quote, [DbFunctionParameter(DbTypeName = "nchar"), MaxLength(1), MinLength(1)] String escape)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, quote, escape);
    }

    [DbFunctionEx(StoreNamespace, "strEscape", IsComposable = true)]
    public String strEscape(String input, [DbFunctionParameter(DbTypeName = "nchar"), MaxLength(1), MinLength(1)] String escape, String symbols)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, escape, symbols);
    }

    [DbFunctionEx(StoreNamespace, "strUnescape", IsComposable = true)]
    public String strUnescape(String input, [DbFunctionParameter(DbTypeName = "nchar"), MaxLength(1), MinLength(1)] String escape, String symbols)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, escape, symbols);
    }

    #endregion
    #region String convert functions

    [DbFunctionEx(StoreNamespace, "strToLower", IsComposable = true)]
    public String strToLower(String input)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strToLowerByLcId", IsComposable = true)]
    public String strToLower(String input, Int32? lcId)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strToLowerByLcName", IsComposable = true)]
    public String strToLower(String input, String lcName)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strToUpper", IsComposable = true)]
    public String strToUpper(String input)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strToUpperByLcId", IsComposable = true)]
    public String strToUpper(String input, Int32? lcId)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strToUpperByLcName", IsComposable = true)]
    public String strToUpper(String input, String lcName)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, lcName);
    }

    #endregion
    #region String retrieve functions

    [DbFunctionEx(StoreNamespace, "strLength", IsComposable = true)]
    public Int32? strLength(String input)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOf", IsComposable = true)]
    public Int32? strIndexOf(String input, String value, CompareOptions compareOptions)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, value, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfRange", IsComposable = true)]
    public Int32? strIndexOf(String input, String value, Int32? index, Int32? count, CompareOptions compareOptions)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, value, index, count, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfByLcId", IsComposable = true)]
    public Int32? strIndexOf(String input, String value, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfRangeByLcId", IsComposable = true)]
    public Int32? strIndexOf(String input, String value, Int32? index, Int32? count, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, value, index, count, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfByLcName", IsComposable = true)]
    public Int32? strIndexOf(String input, String value, CompareOptions compareOptions, String lcName)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfRangeByLcName", IsComposable = true)]
    public Int32? strIndexOf(String input, String value, Int32? index, Int32? count, CompareOptions compareOptions, String lcName)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, value, index, count, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOf", IsComposable = true)]
    public Int32? strLastIndexOf(String input, String value, CompareOptions compareOptions)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, value, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfRange", IsComposable = true)]
    public Int32? strLastIndexOf(String input, String value, Int32? index, Int32? count, CompareOptions compareOptions)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, value, index, count, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfByLcId", IsComposable = true)]
    public Int32? strLastIndexOf(String input, String value, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfRangeByLcId", IsComposable = true)]
    public Int32? strLastIndexOf(String input, String value, Int32? index, Int32? count, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, value, index, count, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfByLcName", IsComposable = true)]
    public Int32? strLastIndexOf(String input, String value, CompareOptions compareOptions, String lcName)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfRangeByLcName", IsComposable = true)]
    public Int32? strLastIndexOf(String input, String value, Int32? index, Int32? count, CompareOptions compareOptions, String lcName)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, value, index, count, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strContains", IsComposable = true)]
    public Boolean? strContains(String input, String value, CompareOptions compareOptions)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strContainsByLcId", IsComposable = true)]
    public Boolean? strContains(String input, String value, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strContainsByLcName", IsComposable = true)]
    public Boolean? strContains(String input, String value, CompareOptions compareOptions, String lcName)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strStartsWith", IsComposable = true)]
    public Boolean? strStartsWith(String input, String value, CompareOptions compareOptions)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strStartsWithByLcId", IsComposable = true)]
    public Boolean? strStartsWith(String input, String value, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strStartsWithByLcName", IsComposable = true)]
    public Boolean? strStartsWith(String input, String value, CompareOptions compareOptions, String lcName)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strEndsWith", IsComposable = true)]
    public Boolean? strEndsWith(String input, String value, CompareOptions compareOptions)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strEndsWithByLcId", IsComposable = true)]
    public Boolean? strEndsWith(String input, String value, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strEndsWithByLcName", IsComposable = true)]
    public Boolean? strEndsWith(String input, String value, CompareOptions compareOptions, String lcName)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, value, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfAny", IsComposable = true)]
    public Int32? strIndexOfAny(String input, String anyOf)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, anyOf);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfAnyRange", IsComposable = true)]
    public Int32? strIndexOfAny(String input, String anyOf, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, anyOf, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfAny", IsComposable = true)]
    public Int32? strLastIndexOfAny(String input, String anyOf)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, anyOf);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfAnyRange", IsComposable = true)]
    public Int32? strLastIndexOfAny(String input, String anyOf, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, anyOf, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfExcept", IsComposable = true)]
    public Int32? strIndexOfExcept(String input, String exceptOf)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, exceptOf);
    }

    [DbFunctionEx(StoreNamespace, "strIndexOfExceptRange", IsComposable = true)]
    public Int32? strIndexOfExcept(String input, String exceptOf, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, exceptOf, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfExcept", IsComposable = true)]
    public Int32? strLastIndexOfExcept(String input, String exceptOf)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, exceptOf);
    }

    [DbFunctionEx(StoreNamespace, "strLastIndexOfExceptRange", IsComposable = true)]
    public Int32? strLastIndexOfExcept(String input, String exceptOf, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, exceptOf, index, count);
    }

    #endregion
    #region String comparison functons

    [DbFunctionEx(StoreNamespace, "strCompare", IsComposable = true)]
    public Int32? strCompare(String value1, String value2, CompareOptions compareOptions)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), value1, value2, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strCompareRange", IsComposable = true)]
    public Int32? strCompare(String value1, Int32? index1, Int32? count1, String value2, Int32? index2, Int32? count2, CompareOptions compareOptions)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strCompareByLcId", IsComposable = true)]
    public Int32? strCompare(String value1, String value2, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), value1, value2, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strCompareRangeByLcId", IsComposable = true)]
    public Int32? strCompare(String value1, Int32? index1, Int32? count1, String value2, Int32? index2, Int32? count2, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strCompareByLcName", IsComposable = true)]
    public Int32? strCompare(String value1, String value2, CompareOptions compareOptions, String lcName)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), value1, value2, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strCompareRangeByLcName", IsComposable = true)]
    public Int32? strCompare(String value1, Int32? index1, Int32? count1, String value2, Int32? index2, Int32? count2, CompareOptions compareOptions, String lcName)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strEqual", IsComposable = true)]
    public Boolean? strEqual(String value1, String value2, CompareOptions compareOptions)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), value1, value2, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strEqualRange", IsComposable = true)]
    public Boolean? strEqual(String value1, Int32? index1, Int32? count1, String value2, Int32? index2, Int32? count2, CompareOptions compareOptions)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions);
    }

    [DbFunctionEx(StoreNamespace, "strEqualByLcId", IsComposable = true)]
    public Boolean? strEqual(String value1, String value2, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), value1, value2, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strEqualRangeByLcId", IsComposable = true)]
    public Boolean? strEqual(String value1, Int32? index1, Int32? count1, String value2, Int32? index2, Int32? count2, CompareOptions compareOptions, Int32? lcId)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions, lcId);
    }

    [DbFunctionEx(StoreNamespace, "strEqualByLcName", IsComposable = true)]
    public Boolean? strEqual(String value1, String value2, CompareOptions compareOptions, String lcName)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), value1, value2, compareOptions, lcName);
    }

    [DbFunctionEx(StoreNamespace, "strEqualRangeByLcName", IsComposable = true)]
    public Boolean? strEqual(String value1, Int32? index1, Int32? count1, String value2, Int32? index2, Int32? count2, CompareOptions compareOptions, String lcName)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), value1, index1, count1, value2, index2, count2, compareOptions, lcName);
    }

    #endregion
    #region String split functions

    [DbFunctionEx(ContextNamespace, "strSplitToChars", IsComposable = true)]
    public IQueryable<StringRow> strSplitToChars(String input, Int32? index, Int32? count)
    {
      return CreateQuery<StringRow>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(ContextNamespace, "strSplitByChars", IsComposable = true)]
    public IQueryable<StringRow> strSplitByChars(String input, String delimitChars, Int32? count, StringSplitOptions options)
    {
      return CreateQuery<StringRow>(MethodBase.GetCurrentMethod(), input, delimitChars, count, (int)options);
    }

    [DbFunctionEx(ContextNamespace, "strSplitByWords", IsComposable = true)]
    public IQueryable<StringRow> strSplitByWords(String input, String separators, String delimitChars, Int32? count, StringSplitOptions options)
    {
      return CreateQuery<StringRow>(MethodBase.GetCurrentMethod(), input, separators, delimitChars, count, (int)options);
    }

    [DbFunctionEx(ContextNamespace, "strSplit", IsComposable = true)]
    public IQueryable<StringRow> strSplit(String input, String separator, Int32? count, StringSplitOptions options)
    {
      return CreateQuery<StringRow>(MethodBase.GetCurrentMethod(), input, separator, count, (int)options);
    }

    [DbFunctionEx(ContextNamespace, "strSplitSmart", IsComposable = true)]
    public IQueryable<StringRow> strSplit(String input, String separators, String trims, String controlSeparators, String controlEscapes, String controls, Int32? count, StringSplitOptionsEx options)
    {
      return CreateQuery<StringRow>(MethodBase.GetCurrentMethod(), input, separators, trims, controlSeparators, controlEscapes, controls, count, (int)options);
    }

    #endregion
    #region String aggregate functions

    [DbFunctionEx(StoreNamespace, "strConcat", IsComposable = true, IsAggregate = true)]
    public String strConcat(String value)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "strJoin", IsComposable = true, IsAggregate = true)]
    public String strJoin(String value, String delimiter)
    {
      return ExecuteUnsupported<String>(MethodBase.GetCurrentMethod(), value, delimiter);
    }

    #endregion
    #endregion
    #region Binary functions
    #region Binary manipulation functions

    [DbFunctionEx(StoreNamespace, "binInsert", IsComposable = true)]
    public Byte[] binInsert(Byte[] input, Int64? index, Byte[] value)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "binRemove", IsComposable = true)]
    public Byte[] binRemove(Byte[] input, Int64? index, Int64? count)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binReplicate", IsComposable = true)]
    public Byte[] binReplicate(Byte[] input, Int64 count)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, count);
    }

    [DbFunctionEx(StoreNamespace, "binRange", IsComposable = true)]
    public Byte[] binRange(Byte[] input, Int64? index, Int64? count)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binReverse", IsComposable = true)]
    public Byte[] binReverse(Byte[] input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binReverseRange", IsComposable = true)]
    public Byte[] binReverse(Byte[] input, Int64? index, Int64? count)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binReplace", IsComposable = true)]
    public Byte[] binReplace(Byte[] input, Byte[] value, Byte[] replacement)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, value, replacement);
    }

    [DbFunctionEx(StoreNamespace, "binReplaceRange", IsComposable = true)]
    public Byte[] binReplace(Byte[] input, Byte[] value, Byte[] replacement, Int64? index, Int64? count)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, value, replacement, index, count);
    }

    #endregion
    #region Binary convert functions

    [DbFunctionEx(StoreNamespace, "binToString", IsComposable = true)]
    public String binToString(Byte[] input, Int64? index, Int64? count)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binToStringByCpId", IsComposable = true)]
    public String binToString(Byte[] input, Int64? index, Int64? count, Int32? cpId)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, index, count, cpId);
    }

    [DbFunctionEx(StoreNamespace, "binToStringByCpName", IsComposable = true)]
    public String binToString(Byte[] input, Int64? index, Int64? count, String cpName)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, index, count, cpName);
    }

    [DbFunctionEx(StoreNamespace, "binToSmallInt", IsComposable = true)]
    public Int16? binToInt16(Byte[] input, Int64? index)
    {
      return ExecuteScalarQuery<Int16?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binToInt", IsComposable = true)]
    public Int32? binToInt32(Byte[] input, Int64? index)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binToBigInt", IsComposable = true)]
    public Int64? binToInt64(Byte[] input, Int64? index)
    {
      return ExecuteScalarQuery<Int64?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binToSingle", IsComposable = true)]
    public Single? binToSingle(Byte[] input, Int64? index)
    {
      return ExecuteScalarQuery<Single?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binToDouble", IsComposable = true)]
    public Double? binToDouble(Byte[] input, Int64? index)
    {
      return ExecuteScalarQuery<Double?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binToDateTime", IsComposable = true)]
    public DateTime? binToDateTime(Byte[] input, Int64? index)
    {
      return ExecuteScalarQuery<DateTime?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binToUid", IsComposable = true)]
    public Guid? binToGuid(Byte[] input, Int64? index)
    {
      return ExecuteScalarQuery<Guid?>(MethodBase.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binFromString", IsComposable = true)]
    public Byte[] binFromString(String input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromStringByCpId", IsComposable = true)]
    public Byte[] binFromString(String input, Int32? cpId)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, cpId);
    }

    [DbFunctionEx(StoreNamespace, "binFromStringByCpName", IsComposable = true)]
    public Byte[] binFromString(String input, String cpName)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, cpName);
    }

    [DbFunctionEx(StoreNamespace, "binFromBase64String", IsComposable = true)]
    public Byte[] binFromBase64String(String input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromTinyInt", IsComposable = true)]
    public Byte[] binFromByte(Byte? input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromSmallInt", IsComposable = true)]
    public Byte[] binFromInt16(Int16? input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromInt", IsComposable = true)]
    public Byte[] binFromInt32(Int32? input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromBigInt", IsComposable = true)]
    public Byte[] binFromInt64(Int64? input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromSingleFloat", IsComposable = true)]
    public Byte[] binFromSingle(Single? input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromDoubleFloat", IsComposable = true)]
    public Byte[] binFromDouble(Double? input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromDateTime", IsComposable = true)]
    public Byte[] binFromDateTime(DateTime? input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binFromUid", IsComposable = true)]
    public Byte[] binFromGuid(Guid? input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    #endregion
    #region Binary retrieve functions

    [DbFunctionEx(StoreNamespace, "binLength", IsComposable = true)]
    public Int64? binLength(Byte[] input)
    {
      return ExecuteScalarQuery<Int64?>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binIndexOf", IsComposable = true)]
    public Int64? binIndexOf(Byte[] input, Byte[] value)
    {
      return ExecuteScalarQuery<Int64?>(MethodBase.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "binIndexOfRange", IsComposable = true)]
    public Int64? binIndexOf(Byte[] input, Byte[] value, Int64? index, Int64? count)
    {
      return ExecuteScalarQuery<Int64?>(MethodBase.GetCurrentMethod(), input, value, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binLastIndexOf", IsComposable = true)]
    public Int64? binLastIndexOf(Byte[] input, Byte[] value)
    {
      return ExecuteScalarQuery<Int64?>(MethodBase.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "binLastIndexOfRange", IsComposable = true)]
    public Int64? binLastIndexOf(Byte[] input, Byte[] value, Int64? index, Int64? count)
    {
      return ExecuteScalarQuery<Int64?>(MethodBase.GetCurrentMethod(), input, value, index, count);
    }

    #endregion
    #region Binary comparison functions

    [DbFunctionEx(StoreNamespace, "binCompare", IsComposable = true)]
    public Int64? binCompare(Byte[] xValue, Byte[] yValue)
    {
      return ExecuteScalarQuery<Int64?>(MethodBase.GetCurrentMethod(), xValue, yValue);
    }

    [DbFunctionEx(StoreNamespace, "binCompareRange", IsComposable = true)]
    public Int64? binCompare(Byte[] xValue, Int64? xIndex, Byte[] yValue, Int64? yIndex, Int64? count)
    {
      return ExecuteScalarQuery<Int64?>(MethodBase.GetCurrentMethod(), xValue, xIndex, yValue, yIndex, count);
    }

    [DbFunctionEx(StoreNamespace, "binEqual", IsComposable = true)]
    public Boolean? binEqual(Byte[] xValue, Byte[] yValue)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), xValue, yValue);
    }

    [DbFunctionEx(StoreNamespace, "binEqualRange", IsComposable = true)]
    public Boolean? binEqual(Byte[] xValue, Int64? xIndex, Byte[] yValue, Int64? yIndex, Int64? count)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), xValue, xIndex, yValue, yIndex, count);
    }

    #endregion
    #region Binary split functions

    [DbFunctionEx(ContextNamespace, "binSplitToBit", IsComposable = true)]
    public IQueryable<BooleanRow> binSplitToBoolean(Byte[] input)
    {
      return CreateQuery<BooleanRow>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToBinary", IsComposable = true)]
    public IQueryable<BinaryRow> binSplitToBinary(Byte[] input)
    {
      return CreateQuery<BinaryRow>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToString", IsComposable = true)]
    public IQueryable<StringRow> binSplitToString(Byte[] input)
    {
      return CreateQuery<StringRow>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToTinyint", IsComposable = true)]
    public IQueryable<ByteRow> binSplitToByte(Byte[] input)
    {
      return CreateQuery<ByteRow>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToSmallInt", IsComposable = true)]
    public IQueryable<Int16Row> binSplitToInt16(Byte[] input)
    {
      return CreateQuery<Int16Row>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToInt", IsComposable = true)]
    public IQueryable<Int32Row> binSplitToInt32(Byte[] input)
    {
      return CreateQuery<Int32Row>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToBigInt", IsComposable = true)]
    public IQueryable<Int64Row> binSplitToInt64(Byte[] input)
    {
      return CreateQuery<Int64Row>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToSingle", IsComposable = true)]
    public IQueryable<SingleRow> binSplitToSingle(Byte[] input)
    {
      return CreateQuery<SingleRow>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToDouble", IsComposable = true)]
    public IQueryable<DoubleRow> binSplitToDouble(Byte[] input)
    {
      return CreateQuery<DoubleRow>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToDateTime", IsComposable = true)]
    public IQueryable<DateTimeRow> binSplitToDateTime(Byte[] input)
    {
      return CreateQuery<DateTimeRow>(MethodBase.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binSplitToUid", IsComposable = true)]
    public IQueryable<GuidRow> binSplitToGuid(Byte[] input)
    {
      return CreateQuery<GuidRow>(MethodBase.GetCurrentMethod(), input);
    }

    #endregion
    #region Binary aggregate functions

    [DbFunctionEx(StoreNamespace, "binConcat", IsComposable = true, IsAggregate = true)]
    public Byte[] binConcat(Byte[] value)
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
    public String xmlEvaluate(String input, String xpath, String nsmap)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns String value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as String.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsString", IsComposable = true)]
    public String xmlEvaluateAsString(String input, String xpath, String nsmap)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Boolean value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Boolean.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsBit", IsComposable = true)]
    public Boolean? xmlEvaluateAsBoolean(String input, String xpath, String nsmap)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Byte value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Byte.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsTinyInt", IsComposable = true)]
    public Byte? xmlEvaluateAsByte(String input, String xpath, String nsmap)
    {
      return ExecuteScalarQuery<Byte?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Int16 value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Int16.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsSmallInt", IsComposable = true)]
    public Int16? xmlEvaluateAsInt16(String input, String xpath, String nsmap)
    {
      return ExecuteScalarQuery<Int16?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Int32 value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="path">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsMap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Int32.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsInt", IsComposable = true)]
    public Int32? xmlEvaluateAsInt32(String input, String xpath, String nsmap)
    {
      return ExecuteScalarQuery<Int32?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Int64 value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Int64.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsBigInt", IsComposable = true)]
    public Int64? xmlEvaluateAsInt64(String input, String xpath, String nsmap)
    {
      return ExecuteScalarQuery<Int64?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Single value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Single.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsSingle", IsComposable = true)]
    public Single? xmlEvaluateAsSingle(String input, String xpath, String nsmap)
    {
      return ExecuteScalarQuery<Single?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns Double value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as Double.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsDouble", IsComposable = true)]
    public Double? xmlEvaluateAsDouble(String input, String xpath, String nsmap)
    {
      return ExecuteScalarQuery<Double?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Evaluates the xpath expression and returns DateTime value.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as DateTime.</returns>
    [DbFunctionEx(StoreNamespace, "xmlEvaluateAsDateTime", IsComposable = true)]
    public DateTime? xmlEvaluateAsDateTime(String input, String xpath, String nsmap)
    {
      return ExecuteScalarQuery<DateTime?>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Transforms XML data using an XSLT style sheet.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="stylesheet">Xslt style sheet.</param>
    /// <returns>Output xml document.</returns>
    [DbFunctionEx(StoreNamespace, "xmlTransform", IsComposable = true)]
    public String xmlTransform(String input, String stylesheet)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input, stylesheet);
    }

    /// <summary>
    /// Convert XML data to JSON.
    /// </summary>
    /// <param name="input">Input XML data.</param>
    /// <returns>Output JSON data.</returns>
    [DbFunctionEx(StoreNamespace, "xmlToJson", IsComposable = true)]
    public String xmlToJson(String input)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Convert JSON data to XML.
    /// </summary>
    /// <param name="input">Input JSON data.</param>
    /// <returns>Output XML data.</returns>
    [DbFunctionEx(StoreNamespace, "xmlFromJson", IsComposable = true)]
    public String xmlFromJson(String input)
    {
      return ExecuteScalarQuery<String>(MethodBase.GetCurrentMethod(), input);
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
    public IQueryable<StringRow> xmlSelect(String input, String xpath, String nsmap)
    {
      return CreateQuery<StringRow>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects String values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of String.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsString", IsComposable = true)]
    public IQueryable<StringRow> xmlSelectAsString(String input, String xpath, String nsmap)
    {
      return CreateQuery<StringRow>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Boolean values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Boolean.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsBit", IsComposable = true)]
    public IQueryable<StringRow> xmlSelectAsBoolean(String input, String xpath, String nsmap)
    {
      return CreateQuery<StringRow>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Byte values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Byte.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsTinyInt", IsComposable = true)]
    public IQueryable<ByteRow> xmlSelectAsByte(String input, String xpath, String nsmap)
    {
      return CreateQuery<ByteRow>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Int16 values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="path">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsMap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Int16.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsSmallInt", IsComposable = true)]
    public IQueryable<Int16Row> xmlSelectAsInt16(String input, String xpath, String nsmap)
    {
      return CreateQuery<Int16Row>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Int32 values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Int32.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsInt", IsComposable = true)]
    public IQueryable<Int32Row> xmlSelectAsInt32(String input, String xpath, String nsmap)
    {
      return CreateQuery<Int32Row>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Int64 values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Int64.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsBigInt", IsComposable = true)]
    public IQueryable<Int64Row> xmlSelectAsInt64(String input, String xpath, String nsmap)
    {
      return CreateQuery<Int64Row>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Single values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Single.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsSingle", IsComposable = true)]
    public IQueryable<SingleRow> xmlSelectAsSingle(String input, String xpath, String nsmap)
    {
      return CreateQuery<SingleRow>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects Double values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of Double.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsDouble", IsComposable = true)]
    public IQueryable<DoubleRow> xmlSelectAsDouble(String input, String xpath, String nsmap)
    {
      return CreateQuery<DoubleRow>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
    }

    /// <summary>
    /// Selects DateTime values, using the specified xpath expression.
    /// </summary>
    /// <param name="input">Input xml document.</param>
    /// <param name="xpath">A string representing an xpath expression that can be evaluated.</param>
    /// <param name="nsmap">Namespaces map contains semi delimited prefix=namespace pairs.</param>
    /// <returns>The result as collection of DateTime.</returns>
    [DbFunctionEx(ContextNamespace, "xmlSelectAsDateTime", IsComposable = true)]
    public IQueryable<DateTimeRow> xmlSelectAsDateTime(String input, String xpath, String nsmap)
    {
      return CreateQuery<DateTimeRow>(MethodBase.GetCurrentMethod(), input, xpath, nsmap);
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
    public Byte[] comprDeflateCompress(Byte[] input, CompressionLevel compressionLevel)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, compressionLevel);
    }

    /// <summary>
    /// Decompress input data compressed by Deflate algorithm.
    /// </summary>
    /// <param name="input">Compressed data to decompress.</param>
    /// <returns>Decompressed data.</returns>
    [DbFunctionEx(StoreNamespace, "comprDeflateDecompress", IsComposable = true)]
    public Byte[] comprDeflateDecompress(Byte[] input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Compress input data by GZip algorithm.
    /// </summary>
    /// <param name="input">Input data to compress.</param>
    /// <param name="compressionLevel">One of the enumeration values that indicates whether to emphasize speed or compression effectiveness when creating the entry.</param>
    /// <returns>Compressed data.</returns>
    [DbFunctionEx(StoreNamespace, "comprGZipCompress", IsComposable = true)]
    public Byte[] comprGZipCompress(Byte[] input, CompressionLevel compressionLevel)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, compressionLevel);
    }

    /// <summary>
    /// Decompress input data compressed by GZip algorithm.
    /// </summary>
    /// <param name="input">Compressed data to decompress.</param>
    /// <returns>Decompressed data.</returns>
    [DbFunctionEx(StoreNamespace, "comprGZipDecompress", IsComposable = true)]
    public Byte[] comprGZipDecompress(Byte[] input)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Gets the collection of entry names that are currently in the zip archive.
    /// </summary>
    /// <param name="input">Zip archive data.</param>
    /// <returns>collection of entry names that are currently in the zip archive.</returns>
    [DbFunctionEx(StoreNamespace, "zipArchiveGetEntries", IsComposable = true)]
    public IQueryable<ZipArchiveEntryRow> zipArchiveGetEntries(Byte[] input)
    {
      return CreateQuery<ZipArchiveEntryRow>(MethodBase.GetCurrentMethod(), input);
    }

    /// <summary>
    /// Gets the entry data that are currently in the zip archive.
    /// </summary>
    /// <param name="input">Zip archive data.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that specifies the name of the entry to be created.</param>
    /// <returns>Zip archive entry data.</returns>
    [DbFunctionEx(StoreNamespace, "zipArchiveGetEntry", IsComposable = true)]
    public Byte[] zipArchiveGetEntry(Byte[] input, String entryName)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, entryName);
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
    public Byte[] zipArchiveAddEntry(Byte[] input, String entryName, Byte[] entryData, CompressionLevel compressionLevel)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, entryName, entryData, compressionLevel);
    }

    /// <summary>
    /// Deletes the entry from the zip archive.
    /// </summary>
    /// <param name="input">Zip archive data.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that specifies the name of the entry in archive.</param>
    /// <returns>Zip archive data after delete entry operation.</returns>
    [DbFunctionEx(StoreNamespace, "zipArchiveDeleteEntry", IsComposable = true)]
    public Byte[] zipArchiveDeleteEntry(Byte[] input, String entryName)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, entryName);
    }

    #endregion
    #region Cryptography functions

    /// <summary>
    /// Generate cryptographically strong sequence of random values.
    /// </summary>
    /// <param name="count">Generated sequence length.</param>
    /// <returns>Bytes array with a cryptographically strong sequence of random values.</returns>
    [DbFunctionEx(StoreNamespace, "cryptRandom", IsComposable = true)]
    public Byte[] cryptRandom(int count)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), count);
    }

    /// <summary>
    /// Generate cryptographically strong sequence of random nonzero values.
    /// </summary>
    /// <param name="count">Generated sequence length.</param>
    /// <returns>Bytes array with a cryptographically strong sequence of random nonzero values.</returns>
    [DbFunctionEx(StoreNamespace, "cryptNonZeroRandom", IsComposable = true)]
    public Byte[] cryptNonZeroRandom(int count)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), count);
    }

    /// <summary>
    /// Computes the hash value for the specified data.
    /// </summary>
    /// <param name="input">The input data to compute the hash code for.</param>
    /// <param name="algorithmName">The hash algorithm implementation to use.</param>
    /// <returns>The computed hash value.</returns>
    [DbFunctionEx(StoreNamespace, "cryptComputeHash", IsComposable = true)]
    public Byte[] cryptComputeHash(Byte[] input, String algorithmName)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, algorithmName);
    }

    /// <summary>
    /// Verifies the hash value for the specified data.
    /// </summary>
    /// <param name="input">The input data to compute the hash code for.</param>
    /// <param name="hash">The hash value to verify.</param>
    /// <param name="algorithmName">The hash algorithm implementation to use.</param>
    /// <returns>Hash verification result.</returns>
    [DbFunctionEx(StoreNamespace, "cryptVerifyHash", IsComposable = true)]
    public Boolean? cryptVerifyHash(Byte[] input, Byte[] hash, String algorithmName)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, hash, algorithmName);
    }

    /// <summary>
    /// Computes the hash value for the specified key and data.
    /// </summary>
    /// <param name="input">The input data to compute the hash code for.</param>
    /// <param name="key">The key to use in the hash algorithm.</param>
    /// <param name="algorithmName">The keyed hash algorithm implementation to use.</param>
    /// <returns>The computed hash value.</returns>
    [DbFunctionEx(StoreNamespace, "cryptComputeKeyedHash", IsComposable = true)]
    public Byte[] cryptComputeKeyedHash(Byte[] input, Byte[] key, String algorithmName)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, key, algorithmName);
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
    public Boolean? cryptVerifyKeyedHash(Byte[] input, Byte[] key, Byte[] hash, String algorithmName)
    {
      return ExecuteScalarQuery<Boolean?>(MethodBase.GetCurrentMethod(), input, key, hash, algorithmName);
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
    public Byte[] cryptEncryptSymmetric(Byte[] input, Byte[] key, Byte[] iv, String algorithmName, CipherMode mode, PaddingMode padding)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, key, iv, algorithmName, mode, padding);
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
    public Byte[] cryptDecryptSymmetric(Byte[] input, Byte[] key, Byte[] iv, String algorithmName, CipherMode mode, PaddingMode padding)
    {
      return ExecuteScalarQuery<Byte[]>(MethodBase.GetCurrentMethod(), input, key, iv, algorithmName, mode, padding);
    }

    #endregion
    #region Collect functions

    [DbFunctionEx(StoreNamespace, "bCollect", IsComposable = true, IsAggregate = true)]
    public Byte[] boolCollect(Boolean? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "tiCollect", IsComposable = true, IsAggregate = true)]
    public Byte[] byteCollect(Byte? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "siCollect", IsComposable = true, IsAggregate = true)]
    public Byte[] int16Collect(Int16? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "iCollect", IsComposable = true, IsAggregate = true)]
    public Byte[] int32Collect(Int32? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "biCollect", IsComposable = true, IsAggregate = true)]
    public Byte[] int64Collect(Int64? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "sfCollect", IsComposable = true, IsAggregate = true)]
    public Byte[] sglCollect(Single? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "dfCollect", IsComposable = true, IsAggregate = true)]
    public Byte[] dblCollect(Double? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "dtCollect", IsComposable = true, IsAggregate = true)]
    public Byte[] dtmCollect(DateTime? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "uidCollect", IsComposable = true, IsAggregate = true)]
    public Byte[] guidCollect(Guid? value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "binCollect", IsComposable = true, IsAggregate = true)]
    public Byte[] binCollect(Byte[] value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    [DbFunctionEx(StoreNamespace, "strCollect", IsComposable = true, IsAggregate = true)]
    public Byte[] strCollect(String value)
    {
      return ExecuteUnsupported<Byte[]>(MethodBase.GetCurrentMethod(), value);
    }

    #endregion
    #region Collection functions
    #region Boolean collection

    [DbFunctionEx(StoreNamespace, "bCollCreate", IsComposable = true)]
    public byte[] boolCollCreate(SizeEncoding countSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing);
    }

    [DbFunctionEx(StoreNamespace, "bCollParse", IsComposable = true)]
    public byte[] boolCollParse(String input, SizeEncoding countSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing);
    }

    [DbFunctionEx(StoreNamespace, "bCollFormat", IsComposable = true)]
    public byte[] boolCollFormat(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "bCollCount", IsComposable = true)]
    public int? boolCollCount(byte[] input)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "bCollIndexOf", IsComposable = true)]
    public int? boolCollIndexOf(byte[] input, Boolean? value)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "bCollGet", IsComposable = true)]
    public Boolean? boolCollGet(byte[] input, int index)
    {
      return ExecuteScalarQuery<Boolean?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "bCollSet", IsComposable = true)]
    public byte[] boolCollSet(byte[] input, int index, Boolean? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "bCollInsert", IsComposable = true)]
    public byte[] boolCollInsert(byte[] input, int? index, Boolean? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "bCollRemove", IsComposable = true)]
    public byte[] boolCollRemove(byte[] input, Boolean? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "bCollRemoveAt", IsComposable = true)]
    public byte[] boolCollRemoveAt(byte[] input, int index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "bCollClear", IsComposable = true)]
    public byte[] boolCollClear(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "bCollGetRange", IsComposable = true)]
    public byte[] boolCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "bCollSetRange", IsComposable = true)]
    public byte[] boolCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "bCollSetRepeat", IsComposable = true)]
    public byte[] boolCollSetRepeat(byte[] input, int index, Boolean? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "bCollInsertRange", IsComposable = true)]
    public byte[] boolCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "bCollInsertRepeat", IsComposable = true)]
    public byte[] boolCollInsertRepeat(byte[] input, int? index, Boolean? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "bCollRemoveRange", IsComposable = true)]
    public byte[] boolCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "bCollToArray", IsComposable = true)]
    public byte[] boolCollToArray(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "bCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedBooleanRow> boolCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateQuery<IndexedBooleanRow>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Byte collection

    [DbFunctionEx(StoreNamespace, "tiCollCreate", IsComposable = true)]
    public byte[] byteCollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "tiCollParse", IsComposable = true)]
    public byte[] byteCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "tiCollFormat", IsComposable = true)]
    public String byteCollFormat(byte[] input)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "tiCollCount", IsComposable = true)]
    public int? byteCollCount(byte[] input)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "tiCollIndexOf", IsComposable = true)]
    public int? byteCollIndexOf(byte[] input, Byte? value)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "tiCollGet", IsComposable = true)]
    public Byte? byteCollGet(byte[] input, int index)
    {
      return ExecuteScalarQuery<Byte?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "tiCollSet", IsComposable = true)]
    public byte[] byteCollSet(byte[] input, int index, Byte? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "tiCollInsert", IsComposable = true)]
    public byte[] byteCollInsert(byte[] input, int? index, Byte? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "tiCollRemove", IsComposable = true)]
    public byte[] byteCollRemove(byte[] input, Byte? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "tiCollRemoveAt", IsComposable = true)]
    public byte[] byteCollRemoveAt(byte[] input, int index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "tiCollClear", IsComposable = true)]
    public byte[] byteCollClear(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "tiCollGetRange", IsComposable = true)]
    public byte[] byteCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "tiCollSetRange", IsComposable = true)]
    public byte[] byteCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "tiCollSetRepeat", IsComposable = true)]
    public byte[] byteCollSetRepeat(byte[] input, int index, Byte? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "tiCollInsertRange", IsComposable = true)]
    public byte[] byteCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "tiCollInsertRepeat", IsComposable = true)]
    public byte[] byteCollInsertRepeat(byte[] input, int? index, Byte? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "tiCollRemoveRange", IsComposable = true)]
    public byte[] byteCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "tiCollToArray", IsComposable = true)]
    public byte[] byteCollToArray(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "tiCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedByteRow> byteCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateQuery<IndexedByteRow>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Int16 collection

    [DbFunctionEx(StoreNamespace, "siCollCreate", IsComposable = true)]
    public byte[] int16CollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "siCollParse", IsComposable = true)]
    public byte[] int16CollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "siCollFormat", IsComposable = true)]
    public String int16CollFormat(byte[] input)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "siCollCount", IsComposable = true)]
    public int? int16CollCount(byte[] input)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "siCollIndexOf", IsComposable = true)]
    public int? int16CollIndexOf(byte[] input, Int16? value)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "siCollGet", IsComposable = true)]
    public Int16? int16CollGet(byte[] input, int index)
    {
      return ExecuteScalarQuery<Int16?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "siCollSet", IsComposable = true)]
    public byte[] int16CollSet(byte[] input, int index, Int16? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "siCollInsert", IsComposable = true)]
    public byte[] int16CollInsert(byte[] input, int? index, Int16? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "siCollRemove", IsComposable = true)]
    public byte[] int16CollRemove(byte[] input, Int16? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "siCollRemoveAt", IsComposable = true)]
    public byte[] int16CollRemoveAt(byte[] input, int index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "siCollClear", IsComposable = true)]
    public byte[] int16CollClear(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "siCollGetRange", IsComposable = true)]
    public byte[] int16CollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "siCollSetRange", IsComposable = true)]
    public byte[] int16CollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "siCollSetRepeat", IsComposable = true)]
    public byte[] int16CollSetRepeat(byte[] input, int index, Int16? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "siCollInsertRange", IsComposable = true)]
    public byte[] int16CollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "siCollInsertRepeat", IsComposable = true)]
    public byte[] int16CollInsertRepeat(byte[] input, int? index, Int16? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "siCollRemoveRange", IsComposable = true)]
    public byte[] int16CollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "siCollToArray", IsComposable = true)]
    public byte[] int16CollToArray(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "siCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedInt16Row> int16CollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateQuery<IndexedInt16Row>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Int32 collection

    [DbFunctionEx(StoreNamespace, "iCollCreate", IsComposable = true)]
    public byte[] int32CollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "iCollParse", IsComposable = true)]
    public byte[] int32CollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "iCollFormat", IsComposable = true)]
    public String int32CollFormat(byte[] input)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "iCollCount", IsComposable = true)]
    public int? int32CollCount(byte[] input)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "iCollIndexOf", IsComposable = true)]
    public int? int32CollIndexOf(byte[] input, Int32? value)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "iCollGet", IsComposable = true)]
    public Int32? int32CollGet(byte[] input, int index)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "iCollSet", IsComposable = true)]
    public byte[] int32CollSet(byte[] input, int index, Int32? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "iCollInsert", IsComposable = true)]
    public byte[] int32CollInsert(byte[] input, int? index, Int32? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "iCollRemove", IsComposable = true)]
    public byte[] int32CollRemove(byte[] input, Int32? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "iCollRemoveAt", IsComposable = true)]
    public byte[] int32CollRemoveAt(byte[] input, int index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "iCollClear", IsComposable = true)]
    public byte[] int32CollClear(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "iCollGetRange", IsComposable = true)]
    public byte[] int32CollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "iCollSetRange", IsComposable = true)]
    public byte[] int32CollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "iCollSetRepeat", IsComposable = true)]
    public byte[] int32CollSetRepeat(byte[] input, int index, Int32? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "iCollInsertRange", IsComposable = true)]
    public byte[] int32CollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "iCollInsertRepeat", IsComposable = true)]
    public byte[] int32CollInsertRepeat(byte[] input, int? index, Int32? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "iCollRemoveRange", IsComposable = true)]
    public byte[] int32CollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "iCollToArray", IsComposable = true)]
    public byte[] int32CollToArray(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "iCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedInt32Row> int32CollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateQuery<IndexedInt32Row>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Int64 collection

    [DbFunctionEx(StoreNamespace, "biCollCreate", IsComposable = true)]
    public byte[] int64CollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "biCollParse", IsComposable = true)]
    public byte[] int64CollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "biCollFormat", IsComposable = true)]
    public String int64CollFormat(byte[] input)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "biCollCount", IsComposable = true)]
    public int? int64CollCount(byte[] input)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "biCollIndexOf", IsComposable = true)]
    public int? int64CollIndexOf(byte[] input, Int64? value)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "biCollGet", IsComposable = true)]
    public Int64? int64CollGet(byte[] input, int index)
    {
      return ExecuteScalarQuery<Int64?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "biCollSet", IsComposable = true)]
    public byte[] int64CollSet(byte[] input, int index, Int64? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "biCollInsert", IsComposable = true)]
    public byte[] int64CollInsert(byte[] input, int? index, Int64? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "biCollRemove", IsComposable = true)]
    public byte[] int64CollRemove(byte[] input, Int64? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "biCollRemoveAt", IsComposable = true)]
    public byte[] int64CollRemoveAt(byte[] input, int index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "biCollClear", IsComposable = true)]
    public byte[] int64CollClear(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "biCollGetRange", IsComposable = true)]
    public byte[] int64CollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "biCollSetRange", IsComposable = true)]
    public byte[] int64CollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "biCollSetRepeat", IsComposable = true)]
    public byte[] int64CollSetRepeat(byte[] input, int index, Int64? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "biCollInsertRange", IsComposable = true)]
    public byte[] int64CollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "biCollInsertRepeat", IsComposable = true)]
    public byte[] int64CollInsertRepeat(byte[] input, int? index, Int64? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "biCollRemoveRange", IsComposable = true)]
    public byte[] int64CollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "biCollToArray", IsComposable = true)]
    public byte[] int64CollToArray(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "biCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedInt64Row> int64CollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateQuery<IndexedInt64Row>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Single collection

    [DbFunctionEx(StoreNamespace, "sfCollCreate", IsComposable = true)]
    public byte[] sglCollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "sfCollParse", IsComposable = true)]
    public byte[] sglCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "sfCollFormat", IsComposable = true)]
    public String sglCollFormat(byte[] input)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "sfCollCount", IsComposable = true)]
    public int? sglCollCount(byte[] input)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "sfCollIndexOf", IsComposable = true)]
    public int? sglCollIndexOf(byte[] input, Single? value)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "sfCollGet", IsComposable = true)]
    public Single? sglCollGet(byte[] input, int index)
    {
      return ExecuteScalarQuery<Single?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "sfCollSet", IsComposable = true)]
    public byte[] sglCollSet(byte[] input, int index, Single? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "sfCollInsert", IsComposable = true)]
    public byte[] sglCollInsert(byte[] input, int? index, Single? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "sfCollRemove", IsComposable = true)]
    public byte[] sglCollRemove(byte[] input, Single? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "sfCollRemoveAt", IsComposable = true)]
    public byte[] sglCollRemoveAt(byte[] input, int index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "sfCollClear", IsComposable = true)]
    public byte[] sglCollClear(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "sfCollGetRange", IsComposable = true)]
    public byte[] sglCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "sfCollSetRange", IsComposable = true)]
    public byte[] sglCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "sfCollSetRepeat", IsComposable = true)]
    public byte[] sglCollSetRepeat(byte[] input, int index, Single? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "sfCollInsertRange", IsComposable = true)]
    public byte[] sglCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "sfCollInsertRepeat", IsComposable = true)]
    public byte[] sglCollInsertRepeat(byte[] input, int? index, Single? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "sfCollRemoveRange", IsComposable = true)]
    public byte[] sglCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "sfCollToArray", IsComposable = true)]
    public byte[] sglCollToArray(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "sfCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedSingleRow> sglCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateQuery<IndexedSingleRow>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Double collection

    [DbFunctionEx(StoreNamespace, "dfCollCreate", IsComposable = true)]
    public byte[] dblCollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dfCollParse", IsComposable = true)]
    public byte[] dblCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dfCollFormat", IsComposable = true)]
    public String dblCollFormat(byte[] input)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "dfCollCount", IsComposable = true)]
    public int? dblCollCount(byte[] input)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "dfCollIndexOf", IsComposable = true)]
    public int? dblCollIndexOf(byte[] input, Double? value)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "dfCollGet", IsComposable = true)]
    public Double? dblCollGet(byte[] input, int index)
    {
      return ExecuteScalarQuery<Double?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "dfCollSet", IsComposable = true)]
    public byte[] dblCollSet(byte[] input, int index, Double? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dfCollInsert", IsComposable = true)]
    public byte[] dblCollInsert(byte[] input, int? index, Double? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dfCollRemove", IsComposable = true)]
    public byte[] dblCollRemove(byte[] input, Double? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "dfCollRemoveAt", IsComposable = true)]
    public byte[] dblCollRemoveAt(byte[] input, int index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "dfCollClear", IsComposable = true)]
    public byte[] dblCollClear(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "dfCollGetRange", IsComposable = true)]
    public byte[] dblCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dfCollSetRange", IsComposable = true)]
    public byte[] dblCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "dfCollSetRepeat", IsComposable = true)]
    public byte[] dblCollSetRepeat(byte[] input, int index, Double? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "dfCollInsertRange", IsComposable = true)]
    public byte[] dblCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "dfCollInsertRepeat", IsComposable = true)]
    public byte[] dblCollInsertRepeat(byte[] input, int? index, Double? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "dfCollRemoveRange", IsComposable = true)]
    public byte[] dblCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dfCollToArray", IsComposable = true)]
    public byte[] dblCollToArray(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "dfCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedDoubleRow> dblCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateQuery<IndexedDoubleRow>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region DateTime collection

    [DbFunctionEx(StoreNamespace, "dtCollCreate", IsComposable = true)]
    public byte[] dtmCollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dtCollParse", IsComposable = true)]
    public byte[] dtmCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dtCollFormat", IsComposable = true)]
    public String dtmCollFormat(byte[] input)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "dtCollCount", IsComposable = true)]
    public int? dtmCollCount(byte[] input)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "dtCollIndexOf", IsComposable = true)]
    public int? dtmCollIndexOf(byte[] input, DateTime? value)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "dtCollGet", IsComposable = true)]
    public DateTime? dtmCollGet(byte[] input, int index)
    {
      return ExecuteScalarQuery<DateTime?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "dtCollSet", IsComposable = true)]
    public byte[] dtmCollSet(byte[] input, int index, DateTime? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dtCollInsert", IsComposable = true)]
    public byte[] dtmCollInsert(byte[] input, int? index, DateTime? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dtCollRemove", IsComposable = true)]
    public byte[] dtmCollRemove(byte[] input, DateTime? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "dtCollRemoveAt", IsComposable = true)]
    public byte[] dtmCollRemoveAt(byte[] input, int index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "dtCollClear", IsComposable = true)]
    public byte[] dtmCollClear(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "dtCollGetRange", IsComposable = true)]
    public byte[] dtmCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dtCollSetRange", IsComposable = true)]
    public byte[] dtmCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "dtCollSetRepeat", IsComposable = true)]
    public byte[] dtmCollSetRepeat(byte[] input, int index, DateTime? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "dtCollInsertRange", IsComposable = true)]
    public byte[] dtmCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "dtCollInsertRepeat", IsComposable = true)]
    public byte[] dtmCollInsertRepeat(byte[] input, int? index, DateTime? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "dtCollRemoveRange", IsComposable = true)]
    public byte[] dtmCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dtCollToArray", IsComposable = true)]
    public byte[] dtmCollToArray(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "dtCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedDateTimeRow> dtmCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateQuery<IndexedDateTimeRow>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Guid collection

    [DbFunctionEx(StoreNamespace, "uidCollCreate", IsComposable = true)]
    public byte[] guidCollCreate(SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "uidCollParse", IsComposable = true)]
    public byte[] guidCollParse(String input, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "uidCollFormat", IsComposable = true)]
    public String guidCollFormat(byte[] input)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "uidCollCount", IsComposable = true)]
    public int? guidCollCount(byte[] input)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "uidCollIndexOf", IsComposable = true)]
    public int? guidCollIndexOf(byte[] input, Guid? value)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "uidCollGet", IsComposable = true)]
    public Guid? guidCollGet(byte[] input, int index)
    {
      return ExecuteScalarQuery<Guid?>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "uidCollSet", IsComposable = true)]
    public byte[] guidCollSet(byte[] input, int index, Guid? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "uidCollInsert", IsComposable = true)]
    public byte[] guidCollInsert(byte[] input, int? index, Guid? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "uidCollRemove", IsComposable = true)]
    public byte[] guidCollRemove(byte[] input, Guid? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "uidCollRemoveAt", IsComposable = true)]
    public byte[] guidCollRemoveAt(byte[] input, int index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "uidCollClear", IsComposable = true)]
    public byte[] guidCollClear(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "uidCollGetRange", IsComposable = true)]
    public byte[] guidCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "uidCollSetRange", IsComposable = true)]
    public byte[] guidCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "uidCollSetRepeat", IsComposable = true)]
    public byte[] guidCollSetRepeat(byte[] input, int index, Guid? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "uidCollInsertRange", IsComposable = true)]
    public byte[] guidCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "uidCollInsertRepeat", IsComposable = true)]
    public byte[] guidCollInsertRepeat(byte[] input, int? index, Guid? value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "uidCollRemoveRange", IsComposable = true)]
    public byte[] guidCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "uidCollToArray", IsComposable = true)]
    public byte[] guidCollToArray(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "uidCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedGuidRow> guidCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateQuery<IndexedGuidRow>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region String collection

    [DbFunctionEx(StoreNamespace, "strCollCreate", IsComposable = true)]
    public byte[] strCollCreate(SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "strCollCreateByCpId", IsComposable = true)]
    public byte[] strCollCreate(SizeEncoding countSizing, SizeEncoding itemSizing, int? cpId)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, itemSizing, cpId);
    }

    [DbFunctionEx(StoreNamespace, "strCollCreateByCpName", IsComposable = true)]
    public byte[] strCollCreate(SizeEncoding countSizing, SizeEncoding itemSizing, string cpName)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, itemSizing, cpName);
    }

    [DbFunctionEx(StoreNamespace, "strCollParse", IsComposable = true)]
    public byte[] strCollParse(String input, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "strCollParseByCpId", IsComposable = true)]
    public byte[] strCollParse(String input, SizeEncoding countSizing, SizeEncoding itemSizing, int? cpId)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, itemSizing, cpId);
    }

    [DbFunctionEx(StoreNamespace, "strCollParseByCpName", IsComposable = true)]
    public byte[] strCollParse(String input, SizeEncoding countSizing, SizeEncoding itemSizing, string cpName)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, itemSizing, cpName);
    }

    [DbFunctionEx(StoreNamespace, "strCollFormat", IsComposable = true)]
    public String strCollFormat(byte[] input)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strCollCount", IsComposable = true)]
    public int? strCollCount(byte[] input)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strCollIndexOf", IsComposable = true)]
    public int? strCollIndexOf(byte[] input, String value)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "strCollGet", IsComposable = true)]
    public String strCollGet(byte[] input, int index)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "strCollSet", IsComposable = true)]
    public byte[] strCollSet(byte[] input, int index, String value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "strCollInsert", IsComposable = true)]
    public byte[] strCollInsert(byte[] input, int? index, String value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "strCollRemove", IsComposable = true)]
    public byte[] strCollRemove(byte[] input, String value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "strCollRemoveAt", IsComposable = true)]
    public byte[] strCollRemoveAt(byte[] input, int index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "strCollClear", IsComposable = true)]
    public byte[] strCollClear(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "strCollGetRange", IsComposable = true)]
    public byte[] strCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strCollSetRange", IsComposable = true)]
    public byte[] strCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "strCollSetRepeat", IsComposable = true)]
    public byte[] strCollSetRepeat(byte[] input, int index, String value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "strCollInsertRange", IsComposable = true)]
    public byte[] strCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "strCollInsertRepeat", IsComposable = true)]
    public byte[] strCollInsertRepeat(byte[] input, int? index, String value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "strCollRemoveRange", IsComposable = true)]
    public byte[] strCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strCollToArray", IsComposable = true)]
    public byte[] strCollToArray(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "strCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedStringRow> strCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateQuery<IndexedStringRow>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #region Binary collection

    [DbFunctionEx(StoreNamespace, "binCollCreate", IsComposable = true)]
    public byte[] binCollCreate(SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "binCollParse", IsComposable = true)]
    public byte[] binCollParse(String input, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "binCollFormat", IsComposable = true)]
    public String binCollFormat(byte[] input)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binCollCount", IsComposable = true)]
    public int? binCollCount(byte[] input)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binCollIndexOf", IsComposable = true)]
    public int? binCollIndexOf(byte[] input, byte[] value)
    {
      return ExecuteScalarQuery<int?>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "binCollGet", IsComposable = true)]
    public byte[] binCollGet(byte[] input, int index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binCollSet", IsComposable = true)]
    public byte[] binCollSet(byte[] input, int index, byte[] value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "binCollInsert", IsComposable = true)]
    public byte[] binCollInsert(byte[] input, int? index, byte[] value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value);
    }

    [DbFunctionEx(StoreNamespace, "binCollRemove", IsComposable = true)]
    public byte[] binCollRemove(byte[] input, byte[] value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, value);
    }

    [DbFunctionEx(StoreNamespace, "binCollRemoveAt", IsComposable = true)]
    public byte[] binCollRemoveAt(byte[] input, int index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index);
    }

    [DbFunctionEx(StoreNamespace, "binCollClear", IsComposable = true)]
    public byte[] binCollClear(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(StoreNamespace, "binCollGetRange", IsComposable = true)]
    public byte[] binCollGetRange(byte[] input, int? index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binCollSetRange", IsComposable = true)]
    public byte[] binCollSetRange(byte[] input, int index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "binCollSetRepeat", IsComposable = true)]
    public byte[] binCollSetRepeat(byte[] input, int index, byte[] value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "binCollInsertRange", IsComposable = true)]
    public byte[] binCollInsertRange(byte[] input, int? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, range);
    }

    [DbFunctionEx(StoreNamespace, "binCollInsertRepeat", IsComposable = true)]
    public byte[] binCollInsertRepeat(byte[] input, int? index, byte[] value, int count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, value, count);
    }

    [DbFunctionEx(StoreNamespace, "binCollRemoveRange", IsComposable = true)]
    public byte[] binCollRemoveRange(byte[] input, int index, int? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binCollToArray", IsComposable = true)]
    public byte[] binCollToArray(byte[] input)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), input);
    }

    [DbFunctionEx(ContextNamespace, "binCollEnumerate", IsComposable = true)]
    public IQueryable<IndexedBinaryRow> binCollEnumerate(byte[] input, int? index, int? count)
    {
      return CreateQuery<IndexedBinaryRow>(MethodInfo.GetCurrentMethod(), input, index, count);
    }

    #endregion
    #endregion
    #region Array functions
    #region Boolean array methods

    [DbFunctionEx(StoreNamespace, "bArrayCreate", IsComposable = true)]
    public byte[] boolArrayCreate(Int32 length, SizeEncoding countSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing);
    }

    [DbFunctionEx(StoreNamespace, "bArrayParse", IsComposable = true)]
    public byte[] boolArrayParse(String str, SizeEncoding countSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing);
    }

    [DbFunctionEx(StoreNamespace, "bArrayFormat", IsComposable = true)]
    public String boolArrayFormat(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "bArrayLength", IsComposable = true)]
    public Int32? boolArrayLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "bArrayIndexOf", IsComposable = true)]
    public Int32? boolArrayIndexOf(byte[] array, Boolean? value)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "bArrayGet", IsComposable = true)]
    public Boolean? boolArrayGet(byte[] array, Int32 index)
    {
      return ExecuteScalarQuery<Boolean?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "bArraySet", IsComposable = true)]
    public byte[] boolArraySet(byte[] array, Int32 index, Boolean? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "bArrayGetRange", IsComposable = true)]
    public byte[] boolArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "bArraySetRange", IsComposable = true)]
    public byte[] boolArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "bArrayFillRange", IsComposable = true)]
    public byte[] boolArrayFillRange(byte[] array, Int32? index, Int32? count, Boolean? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "bArrayToCollection", IsComposable = true)]
    public byte[] boolArrayToCollection(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "bArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedBooleanRow> boolArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<IndexedBooleanRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Byte array methods

    [DbFunctionEx(StoreNamespace, "tiArrayCreate", IsComposable = true)]
    public byte[] byteArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayParse", IsComposable = true)]
    public byte[] byteArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayFormat", IsComposable = true)]
    public String byteArrayFormat(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayLength", IsComposable = true)]
    public Int32? byteArrayLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayIndexOf", IsComposable = true)]
    public Int32? byteArrayIndexOf(byte[] array, Byte? value)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayGet", IsComposable = true)]
    public Byte? byteArrayGet(byte[] array, Int32 index)
    {
      return ExecuteScalarQuery<Byte?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "tiArraySet", IsComposable = true)]
    public byte[] byteArraySet(byte[] array, Int32 index, Byte? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayGetRange", IsComposable = true)]
    public byte[] byteArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "tiArraySetRange", IsComposable = true)]
    public byte[] byteArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayFillRange", IsComposable = true)]
    public byte[] byteArrayFillRange(byte[] array, Int32? index, Int32? count, Byte? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayToCollection", IsComposable = true)]
    public byte[] byteArrayToCollection(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "tiArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedByteRow> byteArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<IndexedByteRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Int16 array methods

    [DbFunctionEx(StoreNamespace, "siArrayCreate", IsComposable = true)]
    public byte[] int16ArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "siArrayParse", IsComposable = true)]
    public byte[] int16ArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "siArrayFormat", IsComposable = true)]
    public String int16ArrayFormat(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "siArrayLength", IsComposable = true)]
    public Int32? int16ArrayLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "siArrayIndexOf", IsComposable = true)]
    public Int32? int16ArrayIndexOf(byte[] array, Int16? value)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "siArrayGet", IsComposable = true)]
    public Int16? int16ArrayGet(byte[] array, Int32 index)
    {
      return ExecuteScalarQuery<Int16?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "siArraySet", IsComposable = true)]
    public byte[] int16ArraySet(byte[] array, Int32 index, Int16? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "siArrayGetRange", IsComposable = true)]
    public byte[] int16ArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "siArraySetRange", IsComposable = true)]
    public byte[] int16ArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "siArrayFillRange", IsComposable = true)]
    public byte[] int16ArrayFillRange(byte[] array, Int32? index, Int32? count, Int16? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "siArrayToCollection", IsComposable = true)]
    public byte[] int16ArrayToCollection(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "siArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedInt16Row> int16ArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<IndexedInt16Row>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Int32 array methods

    [DbFunctionEx(StoreNamespace, "iArrayCreate", IsComposable = true)]
    public byte[] int32ArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "iArrayParse", IsComposable = true)]
    public byte[] int32ArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "iArrayFormat", IsComposable = true)]
    public String int32ArrayFormat(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "iArrayLength", IsComposable = true)]
    public Int32? int32ArrayLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "iArrayIndexOf", IsComposable = true)]
    public Int32? int32ArrayIndexOf(byte[] array, Int32? value)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "iArrayGet", IsComposable = true)]
    public Int32? int32ArrayGet(byte[] array, Int32 index)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "iArraySet", IsComposable = true)]
    public byte[] int32ArraySet(byte[] array, Int32 index, Int32? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "iArrayGetRange", IsComposable = true)]
    public byte[] int32ArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "iArraySetRange", IsComposable = true)]
    public byte[] int32ArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "iArrayFillRange", IsComposable = true)]
    public byte[] int32ArrayFillRange(byte[] array, Int32? index, Int32? count, Int32? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "iArrayToCollection", IsComposable = true)]
    public byte[] int32ArrayToCollection(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "iArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedInt32Row> int32ArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<IndexedInt32Row>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Int64 array methods

    [DbFunctionEx(StoreNamespace, "biArrayCreate", IsComposable = true)]
    public byte[] int64ArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "biArrayParse", IsComposable = true)]
    public byte[] int64ArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "biArrayFormat", IsComposable = true)]
    public String int64ArrayFormat(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "biArrayLength", IsComposable = true)]
    public Int32? int64ArrayLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "biArrayIndexOf", IsComposable = true)]
    public Int32? int64ArrayIndexOf(byte[] array, Int64? value)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "biArrayGet", IsComposable = true)]
    public Int64? int64ArrayGet(byte[] array, Int32 index)
    {
      return ExecuteScalarQuery<Int64?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "biArraySet", IsComposable = true)]
    public byte[] int64ArraySet(byte[] array, Int32 index, Int64? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "biArrayGetRange", IsComposable = true)]
    public byte[] int64ArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "biArraySetRange", IsComposable = true)]
    public byte[] int64ArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "biArrayFillRange", IsComposable = true)]
    public byte[] int64ArrayFillRange(byte[] array, Int32? index, Int32? count, Int64? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "biArrayToCollection", IsComposable = true)]
    public byte[] int64ArrayToCollection(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "biArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedInt64Row> int64ArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<IndexedInt64Row>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Single array methods

    [DbFunctionEx(StoreNamespace, "sfArrayCreate", IsComposable = true)]
    public byte[] sglArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayParse", IsComposable = true)]
    public byte[] sglArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayFormat", IsComposable = true)]
    public String sglArrayFormat(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayLength", IsComposable = true)]
    public Int32? sglArrayLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayIndexOf", IsComposable = true)]
    public Int32? sglArrayIndexOf(byte[] array, Single? value)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayGet", IsComposable = true)]
    public Single? sglArrayGet(byte[] array, Int32 index)
    {
      return ExecuteScalarQuery<Single?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "sfArraySet", IsComposable = true)]
    public byte[] sglArraySet(byte[] array, Int32 index, Single? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayGetRange", IsComposable = true)]
    public byte[] sglArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "sfArraySetRange", IsComposable = true)]
    public byte[] sglArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayFillRange", IsComposable = true)]
    public byte[] sglArrayFillRange(byte[] array, Int32? index, Int32? count, Single? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayToCollection", IsComposable = true)]
    public byte[] sglArrayToCollection(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "sfArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedSingleRow> sglArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<IndexedSingleRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Double array methods

    [DbFunctionEx(StoreNamespace, "dfArrayCreate", IsComposable = true)]
    public byte[] dblArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayParse", IsComposable = true)]
    public byte[] dblArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayFormat", IsComposable = true)]
    public String dblArrayFormat(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayLength", IsComposable = true)]
    public Int32? dblArrayLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayIndexOf", IsComposable = true)]
    public Int32? dblArrayIndexOf(byte[] array, Double? value)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayGet", IsComposable = true)]
    public Double? dblArrayGet(byte[] array, Int32 index)
    {
      return ExecuteScalarQuery<Double?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "dfArraySet", IsComposable = true)]
    public byte[] dblArraySet(byte[] array, Int32 index, Double? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayGetRange", IsComposable = true)]
    public byte[] dblArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dfArraySetRange", IsComposable = true)]
    public byte[] dblArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayFillRange", IsComposable = true)]
    public byte[] dblArrayFillRange(byte[] array, Int32? index, Int32? count, Double? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayToCollection", IsComposable = true)]
    public byte[] dblArrayToCollection(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "dfArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedDoubleRow> dblArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<IndexedDoubleRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region DateTime array methods

    [DbFunctionEx(StoreNamespace, "dtArrayCreate", IsComposable = true)]
    public byte[] dtmArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayParse", IsComposable = true)]
    public byte[] dtmArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayFormat", IsComposable = true)]
    public String dtmArrayFormat(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayLength", IsComposable = true)]
    public Int32? dtmArrayLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayIndexOf", IsComposable = true)]
    public Int32? dtmArrayIndexOf(byte[] array, DateTime? value)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayGet", IsComposable = true)]
    public DateTime? dtmArrayGet(byte[] array, Int32 index)
    {
      return ExecuteScalarQuery<DateTime?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "dtArraySet", IsComposable = true)]
    public byte[] dtmArraySet(byte[] array, Int32 index, DateTime? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayGetRange", IsComposable = true)]
    public byte[] dtmArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dtArraySetRange", IsComposable = true)]
    public byte[] dtmArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayFillRange", IsComposable = true)]
    public byte[] dtmArrayFillRange(byte[] array, Int32? index, Int32? count, DateTime? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayToCollection", IsComposable = true)]
    public byte[] dtmArrayToCollection(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "dtArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedDateTimeRow> dtmArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<IndexedDateTimeRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Guid array methods

    [DbFunctionEx(StoreNamespace, "uidArrayCreate", IsComposable = true)]
    public byte[] guidArrayCreate(Int32 length, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayParse", IsComposable = true)]
    public byte[] guidArrayParse(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayFormat", IsComposable = true)]
    public String guidArrayFormat(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayLength", IsComposable = true)]
    public Int32? guidArrayLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayIndexOf", IsComposable = true)]
    public Int32? guidArrayIndexOf(byte[] array, Guid? value)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayGet", IsComposable = true)]
    public Guid? guidArrayGet(byte[] array, Int32 index)
    {
      return ExecuteScalarQuery<Guid?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "uidArraySet", IsComposable = true)]
    public byte[] guidArraySet(byte[] array, Int32 index, Guid? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayGetRange", IsComposable = true)]
    public byte[] guidArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "uidArraySetRange", IsComposable = true)]
    public byte[] guidArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayFillRange", IsComposable = true)]
    public byte[] guidArrayFillRange(byte[] array, Int32? index, Int32? count, Guid? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayToCollection", IsComposable = true)]
    public byte[] guidArrayToCollection(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "uidArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedGuidRow> guidArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<IndexedGuidRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region String array methods

    [DbFunctionEx(StoreNamespace, "strArrayCreate", IsComposable = true)]
    public byte[] strArrayCreate(Int32 length, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "strArrayCreateByCpId", IsComposable = true)]
    public byte[] strArrayCreate(Int32 length, SizeEncoding countSizing, SizeEncoding itemSizing, Int32? cpId)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, itemSizing, cpId);
    }

    [DbFunctionEx(StoreNamespace, "strArrayCreateByCpName", IsComposable = true)]
    public byte[] strArrayCreate(Int32 length, SizeEncoding countSizing, SizeEncoding itemSizing, String cpName)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, itemSizing, cpName);
    }

    [DbFunctionEx(StoreNamespace, "strArrayParse", IsComposable = true)]
    public byte[] strArrayParse(String str, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "strArrayParseByCpId", IsComposable = true)]
    public byte[] strArrayParse(String str, SizeEncoding countSizing, SizeEncoding itemSizing, Int32? cpId)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing, cpId);
    }

    [DbFunctionEx(StoreNamespace, "strArrayParseByCpName", IsComposable = true)]
    public byte[] strArrayParse(String str, SizeEncoding countSizing, SizeEncoding itemSizing, String cpName)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing, cpName);
    }

    [DbFunctionEx(StoreNamespace, "strArrayFormat", IsComposable = true)]
    public String strArrayFormat(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "strArrayLength", IsComposable = true)]
    public Int32? strArrayLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "strArrayIndexOf", IsComposable = true)]
    public Int32? strArrayIndexOf(byte[] array, String value)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "strArrayGet", IsComposable = true)]
    public String strArrayGet(byte[] array, Int32 index)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "strArraySet", IsComposable = true)]
    public byte[] strArraySet(byte[] array, Int32 index, String value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "strArrayGetRange", IsComposable = true)]
    public byte[] strArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strArraySetRange", IsComposable = true)]
    public byte[] strArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "strArrayFillRange", IsComposable = true)]
    public byte[] strArrayFillRange(byte[] array, Int32? index, Int32? count, String value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "strArrayToCollection", IsComposable = true)]
    public byte[] strArrayToCollection(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "strArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedStringRow> strArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<IndexedStringRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #region Binary array methods

    [DbFunctionEx(StoreNamespace, "binArrayCreate", IsComposable = true)]
    public byte[] binArrayCreate(Int32 length, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), length, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "binArrayParse", IsComposable = true)]
    public byte[] binArrayParse(String str, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "binArrayFormat", IsComposable = true)]
    public String binArrayFormat(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "binArrayLength", IsComposable = true)]
    public Int32? binArrayLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "binArrayIndexOf", IsComposable = true)]
    public Int32? binArrayIndexOf(byte[] array, Byte[] value)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, value);
    }

    [DbFunctionEx(StoreNamespace, "binArrayGet", IsComposable = true)]
    public Byte[] binArrayGet(byte[] array, Int32 index)
    {
      return ExecuteScalarQuery<Byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "binArraySet", IsComposable = true)]
    public byte[] binArraySet(byte[] array, Int32 index, Byte[] value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "binArrayGetRange", IsComposable = true)]
    public byte[] binArrayGetRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binArraySetRange", IsComposable = true)]
    public byte[] binArraySetRange(byte[] array, Int32 index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, range);
    }

    [DbFunctionEx(StoreNamespace, "binArrayFillRange", IsComposable = true)]
    public byte[] binArrayFillRange(byte[] array, Int32? index, Int32? count, Byte[] value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(StoreNamespace, "binArrayToCollection", IsComposable = true)]
    public byte[] binArrayToCollection(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(ContextNamespace, "binArrayEnumerate", IsComposable = true)]
    public IQueryable<IndexedBinaryRow> binArrayEnumerate(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<IndexedBinaryRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    #endregion
    #endregion
    #region Regular array functions
    #region SqlBoolean regular array methods

    [DbFunctionEx(StoreNamespace, "bArrayCreateRegular", IsComposable = true)]
    public byte[] boolArrayCreateRegular(byte[] lengths, SizeEncoding countSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing);
    }

    [DbFunctionEx(StoreNamespace, "bArrayParseRegular", IsComposable = true)]
    public byte[] boolArrayParseRegular(String str, SizeEncoding countSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing);
    }

    [DbFunctionEx(StoreNamespace, "bArrayFormatRegular", IsComposable = true)]
    public String boolArrayFormatRegular(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "bArrayRank", IsComposable = true)]
    public Int32? boolArrayRank(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "bArrayFlatLength", IsComposable = true)]
    public Int32? boolArrayFlatLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "bArrayDimLengths", IsComposable = true)]
    public byte[] boolArrayDimLengths(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "bArrayDimLength", IsComposable = true)]
    public byte[] boolArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "bArrayGetFlat", IsComposable = true)]
    public Boolean? boolArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<Boolean?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "bArraySetFlat", IsComposable = true)]
    public byte[] boolArraySetFlat(byte[] array, Int32? index, Boolean? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "bArrayGetFlatRange", IsComposable = true)]
    public byte[] boolArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "bArraySetFlatRange", IsComposable = true)]
    public byte[] boolArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "bArrayFillFlatRange", IsComposable = true)]
    public byte[] boolArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Boolean? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "bArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedBooleanRow> boolArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<RegularIndexedBooleanRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "bArrayGetDim", IsComposable = true)]
    public Boolean? boolArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteScalarQuery<Boolean?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "bArraySetDim", IsComposable = true)]
    public byte[] boolArraySetDim(byte[] array, byte[] indices, Boolean? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "bArrayGetDimRange", IsComposable = true)]
    public byte[] boolArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "bArraySetDimRange", IsComposable = true)]
    public byte[] boolArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "bArrayFillDimRange", IsComposable = true)]
    public byte[] boolArrayFillDimRange(byte[] array, byte[] ranges, Boolean? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "bArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedBooleanRow> boolArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateQuery<RegularIndexedBooleanRow>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlByte regular array methods

    [DbFunctionEx(StoreNamespace, "tiArrayCreateRegular", IsComposable = true)]
    public byte[] byteArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayParseRegular", IsComposable = true)]
    public byte[] byteArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayFormatRegular", IsComposable = true)]
    public String byteArrayFormatRegular(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayRank", IsComposable = true)]
    public Int32? byteArrayRank(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayFlatLength", IsComposable = true)]
    public Int32? byteArrayFlatLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayDimLengths", IsComposable = true)]
    public byte[] byteArrayDimLengths(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayDimLength", IsComposable = true)]
    public byte[] byteArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayGetFlat", IsComposable = true)]
    public Byte? byteArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<Byte?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "tiArraySetFlat", IsComposable = true)]
    public byte[] byteArraySetFlat(byte[] array, Int32? index, Byte? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayGetFlatRange", IsComposable = true)]
    public byte[] byteArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "tiArraySetFlatRange", IsComposable = true)]
    public byte[] byteArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayFillFlatRange", IsComposable = true)]
    public byte[] byteArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Byte? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "tiArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedByteRow> byteArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<RegularIndexedByteRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayGetDim", IsComposable = true)]
    public Byte? byteArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteScalarQuery<Byte?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "tiArraySetDim", IsComposable = true)]
    public byte[] byteArraySetDim(byte[] array, byte[] indices, Byte? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayGetDimRange", IsComposable = true)]
    public byte[] byteArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "tiArraySetDimRange", IsComposable = true)]
    public byte[] byteArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "tiArrayFillDimRange", IsComposable = true)]
    public byte[] byteArrayFillDimRange(byte[] array, byte[] ranges, Byte? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "tiArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedByteRow> byteArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateQuery<RegularIndexedByteRow>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlInt16 regular array methods

    [DbFunctionEx(StoreNamespace, "siArrayCreateRegular", IsComposable = true)]
    public byte[] int16ArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "siArrayParseRegular", IsComposable = true)]
    public byte[] int16ArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "siArrayFormatRegular", IsComposable = true)]
    public String int16ArrayFormatRegular(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "siArrayRank", IsComposable = true)]
    public Int32? int16ArrayRank(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "siArrayFlatLength", IsComposable = true)]
    public Int32? int16ArrayFlatLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "siArrayDimLengths", IsComposable = true)]
    public byte[] int16ArrayDimLengths(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "siArrayDimLength", IsComposable = true)]
    public byte[] int16ArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "siArrayGetFlat", IsComposable = true)]
    public Int16? int16ArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<Int16?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "siArraySetFlat", IsComposable = true)]
    public byte[] int16ArraySetFlat(byte[] array, Int32? index, Int16? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "siArrayGetFlatRange", IsComposable = true)]
    public byte[] int16ArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "siArraySetFlatRange", IsComposable = true)]
    public byte[] int16ArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "siArrayFillFlatRange", IsComposable = true)]
    public byte[] int16ArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Int16? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "siArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedInt16Row> int16ArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<RegularIndexedInt16Row>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "siArrayGetDim", IsComposable = true)]
    public Int16? int16ArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteScalarQuery<Int16?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "siArraySetDim", IsComposable = true)]
    public byte[] int16ArraySetDim(byte[] array, byte[] indices, Int16? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "siArrayGetDimRange", IsComposable = true)]
    public byte[] int16ArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "siArraySetDimRange", IsComposable = true)]
    public byte[] int16ArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "siArrayFillDimRange", IsComposable = true)]
    public byte[] int16ArrayFillDimRange(byte[] array, byte[] ranges, Int16? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "siArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedInt16Row> int16ArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateQuery<RegularIndexedInt16Row>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlInt32 regular array methods

    [DbFunctionEx(StoreNamespace, "iArrayCreateRegular", IsComposable = true)]
    public byte[] int32ArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "iArrayParseRegular", IsComposable = true)]
    public byte[] int32ArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "iArrayFormatRegular", IsComposable = true)]
    public String int32ArrayFormatRegular(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "iArrayRank", IsComposable = true)]
    public Int32? int32ArrayRank(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "iArrayFlatLength", IsComposable = true)]
    public Int32? int32ArrayFlatLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "iArrayDimLengths", IsComposable = true)]
    public byte[] int32ArrayDimLengths(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "iArrayDimLength", IsComposable = true)]
    public byte[] int32ArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "iArrayGetFlat", IsComposable = true)]
    public Int32? int32ArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "iArraySetFlat", IsComposable = true)]
    public byte[] int32ArraySetFlat(byte[] array, Int32? index, Int32? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "iArrayGetFlatRange", IsComposable = true)]
    public byte[] int32ArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "iArraySetFlatRange", IsComposable = true)]
    public byte[] int32ArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "iArrayFillFlatRange", IsComposable = true)]
    public byte[] int32ArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Int32? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "iArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedInt32Row> int32ArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<RegularIndexedInt32Row>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "iArrayGetDim", IsComposable = true)]
    public Int32? int32ArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "iArraySetDim", IsComposable = true)]
    public byte[] int32ArraySetDim(byte[] array, byte[] indices, Int32? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "iArrayGetDimRange", IsComposable = true)]
    public byte[] int32ArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "iArraySetDimRange", IsComposable = true)]
    public byte[] int32ArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "iArrayFillDimRange", IsComposable = true)]
    public byte[] int32ArrayFillDimRange(byte[] array, byte[] ranges, Int32? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "iArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedInt32Row> int32ArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateQuery<RegularIndexedInt32Row>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlInt64 regular array methods

    [DbFunctionEx(StoreNamespace, "biArrayCreateRegular", IsComposable = true)]
    public byte[] int64ArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "biArrayParseRegular", IsComposable = true)]
    public byte[] int64ArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "biArrayFormatRegular", IsComposable = true)]
    public String int64ArrayFormatRegular(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "biArrayRank", IsComposable = true)]
    public Int32? int64ArrayRank(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "biArrayFlatLength", IsComposable = true)]
    public Int32? int64ArrayFlatLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "biArrayDimLengths", IsComposable = true)]
    public byte[] int64ArrayDimLengths(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "biArrayDimLength", IsComposable = true)]
    public byte[] int64ArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "biArrayGetFlat", IsComposable = true)]
    public Int64? int64ArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<Int64?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "biArraySetFlat", IsComposable = true)]
    public byte[] int64ArraySetFlat(byte[] array, Int32? index, Int64? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "biArrayGetFlatRange", IsComposable = true)]
    public byte[] int64ArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "biArraySetFlatRange", IsComposable = true)]
    public byte[] int64ArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "biArrayFillFlatRange", IsComposable = true)]
    public byte[] int64ArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Int64? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "biArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedInt64Row> int64ArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<RegularIndexedInt64Row>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "biArrayGetDim", IsComposable = true)]
    public Int64? int64ArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteScalarQuery<Int64?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "biArraySetDim", IsComposable = true)]
    public byte[] int64ArraySetDim(byte[] array, byte[] indices, Int64? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "biArrayGetDimRange", IsComposable = true)]
    public byte[] int64ArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "biArraySetDimRange", IsComposable = true)]
    public byte[] int64ArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "biArrayFillDimRange", IsComposable = true)]
    public byte[] int64ArrayFillDimRange(byte[] array, byte[] ranges, Int64? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "biArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedInt64Row> int64ArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateQuery<RegularIndexedInt64Row>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlSingle regular array methods

    [DbFunctionEx(StoreNamespace, "sfArrayCreateRegular", IsComposable = true)]
    public byte[] sglArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayParseRegular", IsComposable = true)]
    public byte[] sglArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayFormatRegular", IsComposable = true)]
    public String sglArrayFormatRegular(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayRank", IsComposable = true)]
    public Int32? sglArrayRank(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayFlatLength", IsComposable = true)]
    public Int32? sglArrayFlatLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayDimLengths", IsComposable = true)]
    public byte[] sglArrayDimLengths(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayDimLength", IsComposable = true)]
    public byte[] sglArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayGetFlat", IsComposable = true)]
    public Single? sglArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<Single?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "sfArraySetFlat", IsComposable = true)]
    public byte[] sglArraySetFlat(byte[] array, Int32? index, Single? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayGetFlatRange", IsComposable = true)]
    public byte[] sglArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "sfArraySetFlatRange", IsComposable = true)]
    public byte[] sglArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayFillFlatRange", IsComposable = true)]
    public byte[] sglArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Single? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "sfArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedSingleRow> sglArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<RegularIndexedSingleRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayGetDim", IsComposable = true)]
    public Single? sglArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteScalarQuery<Single?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "sfArraySetDim", IsComposable = true)]
    public byte[] sglArraySetDim(byte[] array, byte[] indices, Single? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayGetDimRange", IsComposable = true)]
    public byte[] sglArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "sfArraySetDimRange", IsComposable = true)]
    public byte[] sglArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "sfArrayFillDimRange", IsComposable = true)]
    public byte[] sglArrayFillDimRange(byte[] array, byte[] ranges, Single? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "sfArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedSingleRow> sglArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateQuery<RegularIndexedSingleRow>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlDouble regular array methods

    [DbFunctionEx(StoreNamespace, "dfArrayCreateRegular", IsComposable = true)]
    public byte[] dblArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayParseRegular", IsComposable = true)]
    public byte[] dblArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayFormatRegular", IsComposable = true)]
    public String dblArrayFormatRegular(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayRank", IsComposable = true)]
    public Int32? dblArrayRank(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayFlatLength", IsComposable = true)]
    public Int32? dblArrayFlatLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayDimLengths", IsComposable = true)]
    public byte[] dblArrayDimLengths(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayDimLength", IsComposable = true)]
    public byte[] dblArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayGetFlat", IsComposable = true)]
    public Double? dblArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<Double?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "dfArraySetFlat", IsComposable = true)]
    public byte[] dblArraySetFlat(byte[] array, Int32? index, Double? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayGetFlatRange", IsComposable = true)]
    public byte[] dblArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dfArraySetFlatRange", IsComposable = true)]
    public byte[] dblArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayFillFlatRange", IsComposable = true)]
    public byte[] dblArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Double? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "dfArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedDoubleRow> dblArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<RegularIndexedDoubleRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayGetDim", IsComposable = true)]
    public Double? dblArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteScalarQuery<Double?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "dfArraySetDim", IsComposable = true)]
    public byte[] dblArraySetDim(byte[] array, byte[] indices, Double? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayGetDimRange", IsComposable = true)]
    public byte[] dblArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "dfArraySetDimRange", IsComposable = true)]
    public byte[] dblArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "dfArrayFillDimRange", IsComposable = true)]
    public byte[] dblArrayFillDimRange(byte[] array, byte[] ranges, Double? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "dfArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedDoubleRow> dblArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateQuery<RegularIndexedDoubleRow>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlDateTime regular array methods

    [DbFunctionEx(StoreNamespace, "dtArrayCreateRegular", IsComposable = true)]
    public byte[] dtmArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayParseRegular", IsComposable = true)]
    public byte[] dtmArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayFormatRegular", IsComposable = true)]
    public String dtmArrayFormatRegular(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayRank", IsComposable = true)]
    public Int32? dtmArrayRank(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayFlatLength", IsComposable = true)]
    public Int32? dtmArrayFlatLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayDimLengths", IsComposable = true)]
    public byte[] dtmArrayDimLengths(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayDimLength", IsComposable = true)]
    public byte[] dtmArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayGetFlat", IsComposable = true)]
    public DateTime? dtmArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<DateTime?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "dtArraySetFlat", IsComposable = true)]
    public byte[] dtmArraySetFlat(byte[] array, Int32? index, DateTime? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayGetFlatRange", IsComposable = true)]
    public byte[] dtmArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dtArraySetFlatRange", IsComposable = true)]
    public byte[] dtmArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayFillFlatRange", IsComposable = true)]
    public byte[] dtmArrayFillFlatRange(byte[] array, Int32? index, Int32? count, DateTime? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "dtArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedDateTimeRow> dtmArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<RegularIndexedDateTimeRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayGetDim", IsComposable = true)]
    public DateTime? dtmArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteScalarQuery<DateTime?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "dtArraySetDim", IsComposable = true)]
    public byte[] dtmArraySetDim(byte[] array, byte[] indices, DateTime? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayGetDimRange", IsComposable = true)]
    public byte[] dtmArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "dtArraySetDimRange", IsComposable = true)]
    public byte[] dtmArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "dtArrayFillDimRange", IsComposable = true)]
    public byte[] dtmArrayFillDimRange(byte[] array, byte[] ranges, DateTime? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "dtArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedDateTimeRow> dtmArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateQuery<RegularIndexedDateTimeRow>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlGuid regular array methods

    [DbFunctionEx(StoreNamespace, "uidArrayCreateRegular", IsComposable = true)]
    public byte[] guidArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayParseRegular", IsComposable = true)]
    public byte[] guidArrayParseRegular(String str, SizeEncoding countSizing, bool compact)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, compact);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayFormatRegular", IsComposable = true)]
    public String guidArrayFormatRegular(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayRank", IsComposable = true)]
    public Int32? guidArrayRank(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayFlatLength", IsComposable = true)]
    public Int32? guidArrayFlatLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayDimLengths", IsComposable = true)]
    public byte[] guidArrayDimLengths(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayDimLength", IsComposable = true)]
    public byte[] guidArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayGetFlat", IsComposable = true)]
    public Guid? guidArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<Guid?>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "uidArraySetFlat", IsComposable = true)]
    public byte[] guidArraySetFlat(byte[] array, Int32? index, Guid? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayGetFlatRange", IsComposable = true)]
    public byte[] guidArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "uidArraySetFlatRange", IsComposable = true)]
    public byte[] guidArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayFillFlatRange", IsComposable = true)]
    public byte[] guidArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Guid? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "uidArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedGuidRow> guidArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<RegularIndexedGuidRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayGetDim", IsComposable = true)]
    public Guid? guidArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteScalarQuery<Guid?>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "uidArraySetDim", IsComposable = true)]
    public byte[] guidArraySetDim(byte[] array, byte[] indices, Guid? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayGetDimRange", IsComposable = true)]
    public byte[] guidArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "uidArraySetDimRange", IsComposable = true)]
    public byte[] guidArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "uidArrayFillDimRange", IsComposable = true)]
    public byte[] guidArrayFillDimRange(byte[] array, byte[] ranges, Guid? value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "uidArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedGuidRow> guidArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateQuery<RegularIndexedGuidRow>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlString regular array methods

    [DbFunctionEx(StoreNamespace, "strArrayCreateRegular", IsComposable = true)]
    public byte[] strArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "strArrayCreateRegularByCpId", IsComposable = true)]
    public byte[] strArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, SizeEncoding itemSizing, Int32? cpId)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, itemSizing, cpId);
    }

    [DbFunctionEx(StoreNamespace, "strArrayCreateRegularByCpName", IsComposable = true)]
    public byte[] strArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, SizeEncoding itemSizing, String cpName)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, itemSizing, cpName);
    }

    [DbFunctionEx(StoreNamespace, "strArrayParseRegular", IsComposable = true)]
    public byte[] strArrayParseRegular(String str, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "strArrayParseRegularByCpId", IsComposable = true)]
    public byte[] strArrayParseRegular(String str, SizeEncoding countSizing, SizeEncoding itemSizing, Int32? cpId)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing, cpId);
    }

    [DbFunctionEx(StoreNamespace, "strArrayParseRegularByCpName", IsComposable = true)]
    public byte[] strArrayParseRegular(String str, SizeEncoding countSizing, SizeEncoding itemSizing, String cpName)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing, cpName);
    }

    [DbFunctionEx(StoreNamespace, "strArrayFormatRegular", IsComposable = true)]
    public String strArrayFormatRegular(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "strArrayRank", IsComposable = true)]
    public Int32? strArrayRank(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "strArrayFlatLength", IsComposable = true)]
    public Int32? strArrayFlatLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "strArrayDimLengths", IsComposable = true)]
    public byte[] strArrayDimLengths(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "strArrayDimLength", IsComposable = true)]
    public byte[] strArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "strArrayGetFlat", IsComposable = true)]
    public String strArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "strArraySetFlat", IsComposable = true)]
    public byte[] strArraySetFlat(byte[] array, Int32? index, String value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "strArrayGetFlatRange", IsComposable = true)]
    public byte[] strArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strArraySetFlatRange", IsComposable = true)]
    public byte[] strArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "strArrayFillFlatRange", IsComposable = true)]
    public byte[] strArrayFillFlatRange(byte[] array, Int32? index, Int32? count, String value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "strArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedStringRow> strArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<RegularIndexedStringRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "strArrayGetDim", IsComposable = true)]
    public String strArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "strArraySetDim", IsComposable = true)]
    public byte[] strArraySetDim(byte[] array, byte[] indices, String value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "strArrayGetDimRange", IsComposable = true)]
    public byte[] strArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "strArraySetDimRange", IsComposable = true)]
    public byte[] strArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "strArrayFillDimRange", IsComposable = true)]
    public byte[] strArrayFillDimRange(byte[] array, byte[] ranges, String value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "strArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedStringRow> strArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateQuery<RegularIndexedStringRow>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #region SqlBinary regular array methods

    [DbFunctionEx(StoreNamespace, "binArrayCreateRegular", IsComposable = true)]
    public byte[] binArrayCreateRegular(byte[] lengths, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), lengths, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "binArrayParseRegular", IsComposable = true)]
    public byte[] binArrayParseRegular(String str, SizeEncoding countSizing, SizeEncoding itemSizing)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), str, countSizing, itemSizing);
    }

    [DbFunctionEx(StoreNamespace, "binArrayFormatRegular", IsComposable = true)]
    public String binArrayFormatRegular(byte[] array)
    {
      return ExecuteScalarQuery<String>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "binArrayRank", IsComposable = true)]
    public Int32? binArrayRank(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "binArrayFlatLength", IsComposable = true)]
    public Int32? binArrayFlatLength(byte[] array)
    {
      return ExecuteScalarQuery<Int32?>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "binArrayDimLengths", IsComposable = true)]
    public byte[] binArrayDimLengths(byte[] array)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array);
    }

    [DbFunctionEx(StoreNamespace, "binArrayDimLength", IsComposable = true)]
    public byte[] binArrayDimLength(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "binArrayGetFlat", IsComposable = true)]
    public Byte[] binArrayGetFlat(byte[] array, Int32? index)
    {
      return ExecuteScalarQuery<Byte[]>(MethodInfo.GetCurrentMethod(), array, index);
    }

    [DbFunctionEx(StoreNamespace, "binArraySetFlat", IsComposable = true)]
    public byte[] binArraySetFlat(byte[] array, Int32? index, Byte[] value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, value);
    }

    [DbFunctionEx(StoreNamespace, "binArrayGetFlatRange", IsComposable = true)]
    public byte[] binArrayGetFlatRange(byte[] array, Int32? index, Int32? count)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binArraySetFlatRange", IsComposable = true)]
    public byte[] binArraySetFlatRange(byte[] array, Int32? index, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, range);
    }

    [DbFunctionEx(StoreNamespace, "binArrayFillFlatRange", IsComposable = true)]
    public byte[] binArrayFillFlatRange(byte[] array, Int32? index, Int32? count, Byte[] value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, index, count, value);
    }

    [DbFunctionEx(ContextNamespace, "binArrayEnumerateFlat", IsComposable = true)]
    public IQueryable<RegularIndexedBinaryRow> binArrayEnumerateFlat(byte[] array, Int32? index, Int32? count)
    {
      return CreateQuery<RegularIndexedBinaryRow>(MethodInfo.GetCurrentMethod(), array, index, count);
    }

    [DbFunctionEx(StoreNamespace, "binArrayGetDim", IsComposable = true)]
    public Byte[] binArrayGetDim(byte[] array, byte[] indices)
    {
      return ExecuteScalarQuery<Byte[]>(MethodInfo.GetCurrentMethod(), array, indices);
    }

    [DbFunctionEx(StoreNamespace, "binArraySetDim", IsComposable = true)]
    public byte[] binArraySetDim(byte[] array, byte[] indices, Byte[] value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, value);
    }

    [DbFunctionEx(StoreNamespace, "binArrayGetDimRange", IsComposable = true)]
    public byte[] binArrayGetDimRange(byte[] array, byte[] ranges)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    [DbFunctionEx(StoreNamespace, "binArraySetDimRange", IsComposable = true)]
    public byte[] binArraySetDimRange(byte[] array, byte[] indices, byte[] range)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, indices, range);
    }

    [DbFunctionEx(StoreNamespace, "binArrayFillDimRange", IsComposable = true)]
    public byte[] binArrayFillDimRange(byte[] array, byte[] ranges, Byte[] value)
    {
      return ExecuteScalarQuery<byte[]>(MethodInfo.GetCurrentMethod(), array, ranges, value);
    }

    [DbFunctionEx(ContextNamespace, "binArrayEnumerateDim", IsComposable = true)]
    public IQueryable<RegularIndexedBinaryRow> binArrayEnumerateDim(byte[] array, byte[] ranges)
    {
      return CreateQuery<RegularIndexedBinaryRow>(MethodInfo.GetCurrentMethod(), array, ranges);
    }

    #endregion
    #endregion
    #endregion
  }
}
