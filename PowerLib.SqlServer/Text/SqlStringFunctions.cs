using System;
using System.Collections;
using System.Text;
using System.Globalization;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;

namespace PowerLib.SqlServer.Text
{
	public static class SqlStringFunctionith
	{
    #region Scalar-valued functions
    #region Manipulation functions

    [SqlFunction(Name = "strAppend", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Append([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (input.IsNull || value.IsNull)
        return SqlString.Null;

      return string.Concat(input.Value, value.Value);
    }

    [SqlFunction(Name = "strInsert", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Insert([SqlFacet(MaxSize = -1)] SqlString input, SqlInt32 index, [SqlFacet(MaxSize = -1)] SqlString value)
		{
      if (input.IsNull || value.IsNull)
        return SqlString.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < 0))
        throw new ArgumentOutOfRangeException("index");

      int indexValue = index.IsNull ? input.Value.Length : index.Value;
      return input.Value.Insert(indexValue, value.Value);
		}

		[SqlFunction(Name = "strRemove", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Remove([SqlFacet(MaxSize = -1)] SqlString input, SqlInt32 index, SqlInt32 count)
		{
      if (input.IsNull)
        return SqlString.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < 0))
        throw new ArgumentOutOfRangeException("index");
      if (!input.IsNull && !count.IsNull && (count.Value > input.Value.Length - (index.IsNull ? 0 : index.Value) || count.Value < 0))
        throw new ArgumentOutOfRangeException("count");

      int indexValue = index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value;
      int countValue = count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value;
      return input.Value.Remove(indexValue, countValue);
		}

		[SqlFunction(Name = "strReplace", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Replace([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString pattern, [SqlFacet(MaxSize = -1)] SqlString replacement)
		{
      if (input.IsNull || pattern.IsNull || replacement.IsNull)
        return SqlString.Null;

      return input.Value.Replace(pattern.Value, replacement.Value);
		}

		[SqlFunction(Name = "strSubstring", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Substring([SqlFacet(MaxSize = -1)] SqlString input, SqlInt32 index, SqlInt32 count)
		{
      if (input.IsNull)
        return SqlString.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < 0))
        throw new ArgumentOutOfRangeException("index");
      if (!input.IsNull && !count.IsNull && (count.Value > input.Value.Length - (index.IsNull ? 0 : index.Value) || count.Value < 0))
        throw new ArgumentOutOfRangeException("count");

      int indexValue = index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value;
      int countValue = count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value;
      return input.Value.Substring(indexValue, countValue);
		}

    [SqlFunction(Name = "strReverse", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Reverse([SqlFacet(MaxSize = -1)] SqlString input)
    {
      if (input.IsNull)
        return SqlString.Null;

      var array = input.Value.ToCharArray();
      array.Reverse();
      return new String(array);
    }

    [SqlFunction(Name = "strReplicate", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Replicate([SqlFacet(MaxSize = -1)] SqlString input, SqlInt32 count)
		{
			if (count.IsNull || input.IsNull)
				return SqlString.Null;

			StringBuilder sb = new StringBuilder(input.Value.Length * count.Value);
			for (int i = 0; i < count.Value; i++)
				sb.Append(input.Value);
			return sb.ToString();
		}

		[SqlFunction(Name = "strPadLeft", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString PadLeft([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString padding, SqlInt32 width)
		{
			if (input.IsNull || width.IsNull || padding.IsNull)
				return SqlString.Null;
			if (input.Value.Length >= width.Value)
				return input;

			StringBuilder sb = new StringBuilder(width.Value);
			sb.Insert(0, padding.Value, (width.Value - input.Value.Length) / padding.Value.Length);
			sb.Append(padding.Value.Substring(0, (width.Value - input.Value.Length) % padding.Value.Length));
			sb.Append(input.Value);
			return sb.ToString();
		}

		[SqlFunction(Name = "strPadRight", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString PadRight([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString padding, SqlInt32 width)
		{
			if (input.IsNull || width.IsNull || padding.IsNull)
				return SqlString.Null;
      if (input.Value.Length >= width.Value)
        return input;

      StringBuilder sb = new StringBuilder(width.Value);
			sb.Append(input.Value);
			sb.Insert(sb.Length, padding.Value, (width.Value - input.Value.Length) / padding.Value.Length);
			sb.Append(padding.Value.Substring(0, (width.Value - input.Value.Length) % padding.Value.Length));
			return sb.ToString();
		}

    [SqlFunction(Name = "strCutLeft", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString CutLeft([SqlFacet(MaxSize = -1)] SqlString input, SqlInt32 width)
    {
      if (input.IsNull || width.IsNull)
        return SqlString.Null;
      if (width.Value < 0)
        throw new ArgumentOutOfRangeException("width");

      return input.Value.Length <= width.Value ? input : input.Value.Remove(0, input.Value.Length - width.Value);
    }

    [SqlFunction(Name = "strCutRight", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString CutRight([SqlFacet(MaxSize = -1)] SqlString input, SqlInt32 width)
    {
      if (input.IsNull || width.IsNull)
        return SqlString.Null;
      if (width.Value < 0)
        throw new ArgumentOutOfRangeException("width");

      return input.Value.Length <= width.Value ? input : input.Value.Remove(width.Value, input.Value.Length - width.Value);
    }

    [SqlFunction(Name = "strTrimLeft", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString TrimLeft([SqlFacet(MaxSize = -1)] SqlString input,  SqlString trimming)
		{
      if (input.IsNull || trimming.IsNull)
        return SqlString.Null;
      
      return new SqlString(input.Value.TrimStart(trimming.Value.ToCharArray()));
		}

		[SqlFunction(Name = "strTrimRight", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString TrimRight([SqlFacet(MaxSize = -1)] SqlString input,  SqlString trimming)
		{
      if (input.IsNull || trimming.IsNull)
        return SqlString.Null;

      return new SqlString(input.Value.TrimEnd(trimming.Value.ToCharArray()));
		}

    [SqlFunction(Name = "strTrim", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Trim([SqlFacet(MaxSize = -1)] SqlString input,  SqlString trimming)
    {
      if (input.IsNull || trimming.IsNull)
        return SqlString.Null;

      return new SqlString(input.Value.Trim(trimming.Value.ToCharArray()));
    }

    [SqlFunction(Name = "strQuote", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Quote([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(IsFixedLength = true, MaxSize = 1)] SqlString quote, [SqlFacet(IsFixedLength = true, MaxSize = 1)] SqlString escape)
    {
      if (input.IsNull || quote.IsNull)
        return SqlString.Null;

      return new SqlString(input.Value.Quote(quote.Value[0], !escape.IsNull ? escape.Value[0] : quote.Value[0]));
    }

    [SqlFunction(Name = "strUnquote", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Unquote([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(IsFixedLength = true, MaxSize = 1)] SqlString quote, [SqlFacet(IsFixedLength = true, MaxSize = 1)] SqlString escape)
    {
      if (input.IsNull || quote.IsNull)
        return SqlString.Null;

      return new SqlString(input.Value.Unquote(quote.Value[0], !escape.IsNull ? escape.Value[0] : quote.Value[0]));
    }

    [SqlFunction(Name = "strEscape", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Escape([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(IsFixedLength = true, MaxSize = 1)] SqlString escape,  SqlString symbols)
    {
      if (input.IsNull || escape.IsNull)
        return SqlString.Null;

      return new SqlString(input.Value.Escape(escape.Value[0], !symbols.IsNull ? symbols.Value.ToCharArray() : null));
    }

    [SqlFunction(Name = "strUnescape", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Unescape([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(IsFixedLength = true, MaxSize = 1)] SqlString escape,  SqlString symbols)
    {
      if (input.IsNull || escape.IsNull)
        return SqlString.Null;

      return new SqlString(input.Value.Unescape(escape.Value[0], !symbols.IsNull ? symbols.Value.ToCharArray() : null));
    }

    #endregion
    #region Convert functions

    [SqlFunction(Name = "strToLower", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ToLower([SqlFacet(MaxSize = -1)] SqlString input)
    {
      return !input.IsNull ? input.Value.ToLower(CultureInfo.CurrentCulture) : SqlString.Null;
    }

    [SqlFunction(Name = "strToLowerByLcId", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ToLowerByLcId([SqlFacet(MaxSize = -1)] SqlString input, SqlInt32 lcId)
    {
      return !input.IsNull ? input.Value.ToLower(!lcId.IsNull ? CultureInfo.GetCultureInfo(lcId.Value) : CultureInfo.CurrentCulture) : SqlString.Null;
    }

    [SqlFunction(Name = "strToLowerByLcName", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ToLowerByLcName([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      return !input.IsNull ? input.Value.ToLower(!lcName.IsNull ? CultureInfo.GetCultureInfo(lcName.Value) : CultureInfo.CurrentCulture) : SqlString.Null;
    }

    [SqlFunction(Name = "strToUpper", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ToUpper([SqlFacet(MaxSize = -1)] SqlString input)
    {
      return !input.IsNull ? input.Value.ToUpper(CultureInfo.CurrentCulture) : SqlString.Null;
    }

    [SqlFunction(Name = "strToUpperByLcId", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ToUpperByLcId([SqlFacet(MaxSize = -1)] SqlString input, SqlInt32 lcId)
    {
      return !input.IsNull ? input.Value.ToUpper(!lcId.IsNull ? CultureInfo.GetCultureInfo(lcId.Value) : CultureInfo.CurrentCulture) : SqlString.Null;
    }

    [SqlFunction(Name = "strToUpperByLcName", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ToUpperByLcName([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      return !input.IsNull ? input.Value.ToUpper(!lcName.IsNull ? CultureInfo.GetCultureInfo(lcName.Value) : CultureInfo.CurrentCulture) : SqlString.Null;
    }

    #endregion
    #region Retrieve functions

    [SqlFunction(Name = "strLength", IsDeterministic = true)]
    public static SqlInt32 Length([SqlFacet(MaxSize = -1)] SqlString input)
    {
      return input.IsNull ? SqlInt32.Null : new SqlInt32(input.Value.Length);
    }

    [SqlFunction(Name = "strIndexOf", IsDeterministic = true)]
    public static SqlInt32 IndexOf([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 compareOptions)
    {
      return IndexOfRange(input, value, SqlInt32.Null, SqlInt32.Null, compareOptions);
    }

    [SqlFunction(Name = "strIndexOfRange", IsDeterministic = true)]
    public static SqlInt32 IndexOfRange([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value,
      SqlInt32 index, SqlInt32 count, SqlInt32 compareOptions)
    {
      if (input.IsNull || value.IsNull)
        return SqlInt32.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < 0))
        throw new ArgumentOutOfRangeException("index");
      if (!input.IsNull && !count.IsNull && (count.Value > input.Value.Length - (index.IsNull ? 0 : index.Value) || count.Value < 0))
        throw new ArgumentOutOfRangeException("count");

      int indexValue = index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value;
      int countValue = count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value;
      CompareInfo compareInfo = CompareInfo.GetCompareInfo(CultureInfo.CurrentCulture.LCID);
      return new SqlInt32(compareInfo.IndexOf(input.Value, value.Value, indexValue, countValue,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value));
    }

    [SqlFunction(Name = "strIndexOfByLcId", IsDeterministic = true)]
    public static SqlInt32 IndexOfByLcId([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 compareOptions, SqlInt32 lcId)
    {
      return IndexOfRangeByLcId(input, value, SqlInt32.Null, SqlInt32.Null, compareOptions, lcId);
    }

    [SqlFunction(Name = "strIndexOfRangeByLcId", IsDeterministic = true)]
    public static SqlInt32 IndexOfRangeByLcId([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value,
      SqlInt32 index, SqlInt32 count, SqlInt32 compareOptions, SqlInt32 lcId)
    {
      if (input.IsNull || value.IsNull)
        return SqlInt32.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < 0))
        throw new ArgumentOutOfRangeException("index");
      if (!input.IsNull && !count.IsNull && (count.Value > input.Value.Length - (index.IsNull ? 0 : index.Value) || count.Value < 0))
        throw new ArgumentOutOfRangeException("count");

      int indexValue = index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value;
      int countValue = count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value;
      CompareInfo compareInfo = CompareInfo.GetCompareInfo(!lcId.IsNull ? lcId.Value : CultureInfo.CurrentCulture.LCID);
      return new SqlInt32(compareInfo.IndexOf(input.Value, value.Value, indexValue, countValue,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value));
    }

    [SqlFunction(Name = "strIndexOfByLcName", IsDeterministic = true)]
    public static SqlInt32 IndexOfByLcName([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 compareOptions, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      return IndexOfRangeByLcName(input, value, SqlInt32.Null, SqlInt32.Null, compareOptions, lcName);
    }

    [SqlFunction(Name = "strIndexOfRangeByLcName", IsDeterministic = true)]
    public static SqlInt32 IndexOfRangeByLcName([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value,
      SqlInt32 index, SqlInt32 count, SqlInt32 compareOptions, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      if (input.IsNull || value.IsNull)
        return SqlInt32.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < 0))
        throw new ArgumentOutOfRangeException("index");
      if (!input.IsNull && !count.IsNull && (count.Value > input.Value.Length - (index.IsNull ? 0 : index.Value) || count.Value < 0))
        throw new ArgumentOutOfRangeException("count");

      int indexValue = index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value;
      int countValue = count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value;
      CompareInfo compareInfo = CompareInfo.GetCompareInfo(!lcName.IsNull ? lcName.Value : CultureInfo.CurrentCulture.Name);
      return new SqlInt32(compareInfo.IndexOf(input.Value, value.Value, indexValue, countValue,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value));
    }

    [SqlFunction(Name = "strLastIndexOf", IsDeterministic = true)]
    public static SqlInt32 LastIndexOf([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value,
      SqlInt32 index, SqlInt32 count, SqlInt32 compareOptions)
    {
      return LastIndexOfRange(input, value, SqlInt32.Null, SqlInt32.Null, compareOptions);
    }

    [SqlFunction(Name = "strLastIndexOfRange", IsDeterministic = true)]
    public static SqlInt32 LastIndexOfRange([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value,
      SqlInt32 index, SqlInt32 count, SqlInt32 compareOptions)
    {
      if (input.IsNull || value.IsNull)
        return SqlInt32.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < 0))
        throw new ArgumentOutOfRangeException("index");
      if (!input.IsNull && !count.IsNull && (count.Value > input.Value.Length - (index.IsNull ? 0 : index.Value) || count.Value < 0))
        throw new ArgumentOutOfRangeException("count");

      int indexValue = index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value;
      int countValue = count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value;
      CompareInfo compareInfo = CompareInfo.GetCompareInfo(CultureInfo.CurrentCulture.LCID);
      return new SqlInt32(compareInfo.LastIndexOf(input.Value, value.Value, indexValue, countValue,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value));
    }

    [SqlFunction(Name = "strLastIndexOfByLcId", IsDeterministic = true)]
    public static SqlInt32 LastIndexOfByLcId([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value,
      SqlInt32 index, SqlInt32 count, SqlInt32 compareOptions, SqlInt32 lcId)
    {
      return LastIndexOfRangeByLcId(input, value, SqlInt32.Null, SqlInt32.Null, compareOptions, lcId);
    }

    [SqlFunction(Name = "strLastIndexOfRangeByLcId", IsDeterministic = true)]
    public static SqlInt32 LastIndexOfRangeByLcId([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value,
      SqlInt32 index, SqlInt32 count, SqlInt32 compareOptions, SqlInt32 lcId)
    {
      if (input.IsNull || value.IsNull)
        return SqlInt32.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < -1))
        throw new ArgumentOutOfRangeException("index");
      if (!input.IsNull && !count.IsNull && (count.Value > input.Value.Length - (index.IsNull ? 0 : index.Value) || count.Value < 0))
        throw new ArgumentOutOfRangeException("count");

      int indexValue = index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value;
      int countValue = count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value;
      CompareInfo compareInfo = CompareInfo.GetCompareInfo(!lcId.IsNull ? lcId.Value : CultureInfo.CurrentCulture.LCID);
      return new SqlInt32(compareInfo.LastIndexOf(input.Value, value.Value, indexValue, countValue,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value));
    }

    [SqlFunction(Name = "strLastIndexOfByLcName", IsDeterministic = true)]
    public static SqlInt32 LastIndexOfByLcName([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value,
      SqlInt32 index, SqlInt32 count, SqlInt32 compareOptions, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      return LastIndexOfRangeByLcName(input, value, SqlInt32.Null, SqlInt32.Null, compareOptions, lcName);
    }

    [SqlFunction(Name = "strLastIndexOfRangeByLcName", IsDeterministic = true)]
    public static SqlInt32 LastIndexOfRangeByLcName([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value,
      SqlInt32 index, SqlInt32 count, SqlInt32 compareOptions, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      if (input.IsNull || value.IsNull)
        return SqlInt32.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < -1))
        throw new ArgumentOutOfRangeException("index");
      if (!input.IsNull && !count.IsNull && (count.Value > input.Value.Length - (index.IsNull ? 0 : index.Value) || count.Value < 0))
        throw new ArgumentOutOfRangeException("count");

      int indexValue = index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value;
      int countValue = count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value;
      CompareInfo compareInfo = CompareInfo.GetCompareInfo(!lcName.IsNull ? lcName.Value : CultureInfo.CurrentCulture.Name);
      return new SqlInt32(compareInfo.LastIndexOf(input.Value, value.Value, indexValue, countValue,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value));
    }

    [SqlFunction(Name = "strContains", IsDeterministic = true)]
    public static SqlBoolean Contains([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 compareOptions)
    {
      if (input.IsNull || value.IsNull)
        return SqlBoolean.Null;

      return new SqlBoolean(IndexOfRange(input, value, SqlInt32.Null, SqlInt32.Null, compareOptions).Value >= 0);
    }

    [SqlFunction(Name = "strContainsByLcId", IsDeterministic = true)]
    public static SqlBoolean ContainsByLcId([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 compareOptions, SqlInt32 lcId)
    {
      if (input.IsNull || value.IsNull)
        return SqlBoolean.Null;

      return new SqlBoolean(IndexOfRangeByLcId(input, value, SqlInt32.Null, SqlInt32.Null, compareOptions, lcId).Value >= 0);
    }

    [SqlFunction(Name = "strContainsByLcName", IsDeterministic = true)]
    public static SqlBoolean ContainsByLcName([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 compareOptions, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      if (input.IsNull || value.IsNull)
        return SqlBoolean.Null;

      return new SqlBoolean(IndexOfRangeByLcName(input, value, SqlInt32.Null, SqlInt32.Null, compareOptions, lcName).Value >= 0);
    }

    [SqlFunction(Name = "strStartsWith", IsDeterministic = true)]
    public static SqlBoolean StartsWith([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 compareOptions)
    {
      if (input.IsNull || value.IsNull)
        return SqlBoolean.Null;

      CompareInfo compareInfo = CompareInfo.GetCompareInfo(CultureInfo.CurrentCulture.LCID);
      return new SqlBoolean(input.Value.Length >= value.Value.Length && compareInfo.Compare(input.Value, 0, value.Value.Length, value.Value, 0, value.Value.Length,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value) == 0);
    }

    [SqlFunction(Name = "strStartsWithByLcId", IsDeterministic = true)]
    public static SqlBoolean StartsWithByLcId([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 compareOptions, SqlInt32 lcId)
    {
      if (input.IsNull || value.IsNull)
        return SqlBoolean.Null;

      CompareInfo compareInfo = CompareInfo.GetCompareInfo(!lcId.IsNull ? lcId.Value : CultureInfo.CurrentCulture.LCID);
      return new SqlBoolean(input.Value.Length >= value.Value.Length && compareInfo.Compare(input.Value, 0, value.Value.Length, value.Value, 0, value.Value.Length,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value) == 0);
    }

    [SqlFunction(Name = "strStartsWithByLcName", IsDeterministic = true)]
    public static SqlBoolean StartsWithByLcName([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 compareOptions, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      if (input.IsNull || value.IsNull)
        return SqlBoolean.Null;

      CompareInfo compareInfo = CompareInfo.GetCompareInfo(!lcName.IsNull ? lcName.Value : CultureInfo.CurrentCulture.Name);
      return new SqlBoolean(input.Value.Length >= value.Value.Length && compareInfo.Compare(input.Value, 0, value.Value.Length, value.Value, 0, value.Value.Length,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value) == 0);
    }

    [SqlFunction(Name = "strEndsWith", IsDeterministic = true)]
    public static SqlBoolean EndsWith([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 compareOptions)
    {
      if (input.IsNull || value.IsNull)
        return SqlBoolean.Null;

      CompareInfo compareInfo = CompareInfo.GetCompareInfo(CultureInfo.CurrentCulture.LCID);
      return new SqlBoolean(input.Value.Length >= value.Value.Length && compareInfo.Compare(input.Value, input.Value.Length - value.Value.Length, value.Value.Length, value.Value, 0, value.Value.Length,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value) == 0);
    }

    [SqlFunction(Name = "strEndsWithByLcId", IsDeterministic = true)]
    public static SqlBoolean EndsWithByLcId([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 compareOptions, SqlInt32 lcId)
    {
      if (input.IsNull || value.IsNull)
        return SqlBoolean.Null;

      CompareInfo compareInfo = CompareInfo.GetCompareInfo(!lcId.IsNull ? lcId.Value : CultureInfo.CurrentCulture.LCID);
      return new SqlBoolean(input.Value.Length >= value.Value.Length && compareInfo.Compare(input.Value, input.Value.Length - value.Value.Length, value.Value.Length, value.Value, 0, value.Value.Length,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value) == 0);
    }

    [SqlFunction(Name = "strEndsWithByLcName", IsDeterministic = true)]
    public static SqlBoolean EndsWithByLcName([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 compareOptions, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      if (input.IsNull || value.IsNull)
        return SqlBoolean.Null;

      CompareInfo compareInfo = CompareInfo.GetCompareInfo(!lcName.IsNull ? lcName.Value : CultureInfo.CurrentCulture.Name);
      return new SqlBoolean(input.Value.Length >= value.Value.Length && compareInfo.Compare(input.Value, input.Value.Length - value.Value.Length, value.Value.Length, value.Value, 0, value.Value.Length,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value) == 0);
    }

    [SqlFunction(Name = "strIndexOfAny", IsDeterministic = true)]
    public static SqlInt32 IndexOfAny([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString anyOf)
    {
      return IndexOfAnyRange(input, anyOf, SqlInt32.Null, SqlInt32.Null);
    }

    [SqlFunction(Name = "strIndexOfAnyRange", IsDeterministic = true)]
		public static SqlInt32 IndexOfAnyRange([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString anyOf, SqlInt32 index, SqlInt32 count)
		{
      if (input.IsNull || anyOf.IsNull)
        return SqlInt32.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < 0))
        throw new ArgumentOutOfRangeException("index");
      if (!input.IsNull && !count.IsNull && (count.Value > input.Value.Length - (index.IsNull ? 0 : index.Value) || count.Value < 0))
        throw new ArgumentOutOfRangeException("count");

      int indexValue = index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value;
      int countValue = count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value;
      return new SqlInt32(input.Value.IndexOfAny(anyOf.Value.ToCharArray(), indexValue, countValue));
		}

    [SqlFunction(Name = "strIndexOfExcept", IsDeterministic = true)]
    public static SqlInt32 IndexOfExcept([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString exceptOf)
    {
      return IndexOfExceptRange(input, exceptOf, SqlInt32.Null, SqlInt32.Null);
    }

    [SqlFunction(Name = "strIndexOfExceptRange", IsDeterministic = true)]
		public static SqlInt32 IndexOfExceptRange([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString exceptOf, SqlInt32 index, SqlInt32 count)
		{
      if (input.IsNull || exceptOf.IsNull)
        return SqlInt32.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < 0))
        throw new ArgumentOutOfRangeException("index");
      if (!input.IsNull && !count.IsNull && (count.Value > input.Value.Length - (index.IsNull ? 0 : index.Value) || count.Value < 0))
        throw new ArgumentOutOfRangeException("count");

      int indexValue = index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value;
      int countValue = count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value;
      return new SqlInt32(input.Value.IndexOfExcept(exceptOf.Value.ToCharArray(), indexValue, countValue));
    }

    [SqlFunction(Name = "strLastIndexOfAny", IsDeterministic = true)]
    public static SqlInt32 LastIndexOfAny([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString anyOf)
    {
      return LastIndexOfAnyRange(input, anyOf, SqlInt32.Null, SqlInt32.Null);
    }

    [SqlFunction(Name = "strLastIndexOfAnyRange", IsDeterministic = true)]
		public static SqlInt32 LastIndexOfAnyRange([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString anyOf, SqlInt32 index, SqlInt32 count)
		{
      if (input.IsNull || anyOf.IsNull)
        return SqlInt32.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < 0))
        throw new ArgumentOutOfRangeException("index");
      if (!input.IsNull && !count.IsNull && (count.Value > input.Value.Length - (index.IsNull ? 0 : index.Value) || count.Value < 0))
        throw new ArgumentOutOfRangeException("count");

      int indexValue = index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value;
      int countValue = count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value;
      return new SqlInt32(input.Value.LastIndexOfAny(anyOf.Value.ToCharArray(), indexValue, countValue));
		}

    [SqlFunction(Name = "strLastIndexOfExcept", IsDeterministic = true)]
    public static SqlInt32 LastIndexOfExcept([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString exceptOf)
    {
      return LastIndexOfExceptRange(input, exceptOf, SqlInt32.Null, SqlInt32.Null);
    }

    [SqlFunction(Name = "strLastIndexOfExceptRange", IsDeterministic = true)]
		public static SqlInt32 LastIndexOfExceptRange([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString exceptOf, SqlInt32 index, SqlInt32 count)
		{
      if (input.IsNull || exceptOf.IsNull)
        return SqlInt32.Null;
      if (!input.IsNull && !index.IsNull && (index.Value > input.Value.Length || index.Value < 0))
        throw new ArgumentOutOfRangeException("index");
      if (!input.IsNull && !count.IsNull && (count.Value > input.Value.Length - (index.IsNull ? 0 : index.Value) || count.Value < 0))
        throw new ArgumentOutOfRangeException("count");

      int indexValue = index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value;
      int countValue = count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value;
      return new SqlInt32(input.Value.LastIndexOfExcept(exceptOf.Value.ToCharArray(), indexValue, countValue));
    }

    #endregion
    #region Comparison functions

    [SqlFunction(Name = "strCompare", IsDeterministic = true)]
    public static SqlInt32 Compare([SqlFacet(MaxSize = -1)] SqlString value1, [SqlFacet(MaxSize = -1)] SqlString value2, SqlInt32 compareOptions)
    {
      return CompareRange(value1, SqlInt32.Null, SqlInt32.Null, value2, SqlInt32.Null, SqlInt32.Null, compareOptions);
    }

    [SqlFunction(Name = "strCompareRange", IsDeterministic = true)]
    public static SqlInt32 CompareRange([SqlFacet(MaxSize = -1)] SqlString value1, SqlInt32 index1, SqlInt32 count1,
      [SqlFacet(MaxSize = -1)] SqlString value2, SqlInt32 index2, SqlInt32 count2, SqlInt32 compareOptions)
    {
      if (value1.IsNull || value2.IsNull)
        return SqlInt32.Null;
      if (!value1.IsNull && !index1.IsNull && (index1.Value > value1.Value.Length || index1.Value < 0))
        throw new ArgumentOutOfRangeException("index1");
      if (!value1.IsNull && !count1.IsNull && (count1.Value > value1.Value.Length - index1.Value || count1.Value < 0))
        throw new ArgumentOutOfRangeException("count1");
      if (!value2.IsNull && !index2.IsNull && (index2.Value > value2.Value.Length || index2.Value < 0))
        throw new ArgumentOutOfRangeException("index2");
      if (!value2.IsNull && !count2.IsNull && (count2.Value > value2.Value.Length - index2.Value || count2.Value < 0))
        throw new ArgumentOutOfRangeException("count2");

      int indexValue1 = index1.IsNull ? value1.Value.Length - (count1.IsNull ? value1.Value.Length : count1.Value) : index1.Value;
      int countValue1 = count1.IsNull ? value1.Value.Length - (index1.IsNull ? 0 : index1.Value) : count1.Value;
      int indexValue2 = index2.IsNull ? value2.Value.Length - (count2.IsNull ? value2.Value.Length : count2.Value) : index2.Value;
      int countValue2 = count2.IsNull ? value2.Value.Length - (index2.IsNull ? 0 : index2.Value) : count2.Value;
      CompareInfo compareInfo = CompareInfo.GetCompareInfo(CultureInfo.CurrentCulture.LCID);
      return new SqlInt32(compareInfo.Compare(value1.Value, indexValue1, countValue1, value2.Value, indexValue2, countValue2,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value));
    }

    [SqlFunction(Name = "strCompareByLcId", IsDeterministic = true)]
    public static SqlInt32 CompareByLcId([SqlFacet(MaxSize = -1)] SqlString value1, [SqlFacet(MaxSize = -1)] SqlString value2, SqlInt32 compareOptions, SqlInt32 lcId)
    {
      return CompareRangeByLcId(value1, SqlInt32.Null, SqlInt32.Null, value2, SqlInt32.Null, SqlInt32.Null, compareOptions, lcId);
    }

    [SqlFunction(Name = "strCompareRangeByLcId", IsDeterministic = true)]
    public static SqlInt32 CompareRangeByLcId([SqlFacet(MaxSize = -1)] SqlString value1, SqlInt32 index1, SqlInt32 count1,
      [SqlFacet(MaxSize = -1)] SqlString value2, SqlInt32 index2, SqlInt32 count2, SqlInt32 compareOptions, SqlInt32 lcId)
    {
      if (value1.IsNull || value2.IsNull)
        return SqlInt32.Null;
      if (!value1.IsNull && !index1.IsNull && (index1.Value > value1.Value.Length || index1.Value < 0))
        throw new ArgumentOutOfRangeException("index1");
      if (!value1.IsNull && !count1.IsNull && (count1.Value > value1.Value.Length - index1.Value || count1.Value < 0))
        throw new ArgumentOutOfRangeException("count1");
      if (!value2.IsNull && !index2.IsNull && (index2.Value > value2.Value.Length || index2.Value < 0))
        throw new ArgumentOutOfRangeException("index2");
      if (!value2.IsNull && !count2.IsNull && (count2.Value > value2.Value.Length - index2.Value || count2.Value < 0))
        throw new ArgumentOutOfRangeException("count2");

      int indexValue1 = index1.IsNull ? value1.Value.Length - (count1.IsNull ? value1.Value.Length : count1.Value) : index1.Value;
      int countValue1 = count1.IsNull ? value1.Value.Length - (index1.IsNull ? 0 : index1.Value) : count1.Value;
      int indexValue2 = index2.IsNull ? value2.Value.Length - (count2.IsNull ? value2.Value.Length : count2.Value) : index2.Value;
      int countValue2 = count2.IsNull ? value2.Value.Length - (index2.IsNull ? 0 : index2.Value) : count2.Value;
      CompareInfo compareInfo = CompareInfo.GetCompareInfo(!lcId.IsNull ? lcId.Value : CultureInfo.CurrentCulture.LCID);
      return new SqlInt32(compareInfo.Compare(value1.Value, indexValue1, countValue1, value2.Value, indexValue2, countValue2,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value));
    }

    [SqlFunction(Name = "strCompareByLcName", IsDeterministic = true)]
    public static SqlInt32 CompareByLcName([SqlFacet(MaxSize = -1)] SqlString value1, [SqlFacet(MaxSize = -1)] SqlString value2, SqlInt32 compareOptions, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      return CompareRangeByLcName(value1, SqlInt32.Null, SqlInt32.Null, value2, SqlInt32.Null, SqlInt32.Null, compareOptions, lcName);
    }

    [SqlFunction(Name = "strCompareRangeByLcName", IsDeterministic = true)]
    public static SqlInt32 CompareRangeByLcName([SqlFacet(MaxSize = -1)] SqlString value1, SqlInt32 index1, SqlInt32 count1,
      [SqlFacet(MaxSize = -1)] SqlString value2, SqlInt32 index2, SqlInt32 count2, SqlInt32 compareOptions, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      if (value1.IsNull || value2.IsNull)
        return SqlInt32.Null;
      if (!value1.IsNull && !index1.IsNull && (index1.Value > value1.Value.Length || index1.Value < 0))
        throw new ArgumentOutOfRangeException("index1");
      if (!value1.IsNull && !count1.IsNull && (count1.Value > value1.Value.Length - index1.Value || count1.Value < 0))
        throw new ArgumentOutOfRangeException("count1");
      if (!value2.IsNull && !index2.IsNull && (index2.Value > value2.Value.Length || index2.Value < 0))
        throw new ArgumentOutOfRangeException("index2");
      if (!value2.IsNull && !count2.IsNull && (count2.Value > value2.Value.Length - index2.Value || count2.Value < 0))
        throw new ArgumentOutOfRangeException("count2");

      int indexValue1 = index1.IsNull ? value1.Value.Length - (count1.IsNull ? value1.Value.Length : count1.Value) : index1.Value;
      int countValue1 = count1.IsNull ? value1.Value.Length - (index1.IsNull ? 0 : index1.Value) : count1.Value;
      int indexValue2 = index2.IsNull ? value2.Value.Length - (count2.IsNull ? value2.Value.Length : count2.Value) : index2.Value;
      int countValue2 = count2.IsNull ? value2.Value.Length - (index2.IsNull ? 0 : index2.Value) : count2.Value;
      CompareInfo compareInfo = CompareInfo.GetCompareInfo(!lcName.IsNull ? lcName.Value : CultureInfo.CurrentCulture.Name);
      return new SqlInt32(compareInfo.Compare(value1.Value, indexValue1, countValue1, value2.Value, indexValue2, countValue2,
        compareOptions.IsNull ? CompareOptions.None : (CompareOptions)compareOptions.Value));
    }

    [SqlFunction(Name = "strEqual", IsDeterministic = true)]
    public static SqlBoolean Equal([SqlFacet(MaxSize = -1)] SqlString value1, [SqlFacet(MaxSize = -1)] SqlString value2, SqlInt32 compareOptions)
    {
      SqlInt32 result = CompareRange(value1, SqlInt32.Null, SqlInt32.Null, value2, SqlInt32.Null, SqlInt32.Null, compareOptions);
      return !result.IsNull ? result.Value == 0 : SqlBoolean.Null;
    }

    [SqlFunction(Name = "strEqualRange", IsDeterministic = true)]
    public static SqlBoolean EqualRange([SqlFacet(MaxSize = -1)] SqlString value1, SqlInt32 index1, SqlInt32 count1,
      [SqlFacet(MaxSize = -1)] SqlString value2, SqlInt32 index2, SqlInt32 count2, SqlInt32 compareOptions)
    {
      SqlInt32 result = CompareRange(value1, index1, count1, value2, index2, count2, compareOptions);
      return !result.IsNull ? result.Value == 0 : SqlBoolean.Null;
    }

    [SqlFunction(Name = "strEqualByLcId", IsDeterministic = true)]
    public static SqlBoolean EqualByLcId([SqlFacet(MaxSize = -1)] SqlString value1, [SqlFacet(MaxSize = -1)] SqlString value2, SqlInt32 compareOptions, SqlInt32 lcId)
    {
      SqlInt32 result = CompareRangeByLcId(value1, SqlInt32.Null, SqlInt32.Null, value2, SqlInt32.Null, SqlInt32.Null, compareOptions, lcId);
      return !result.IsNull ? result.Value == 0 : SqlBoolean.Null;
    }

    [SqlFunction(Name = "strEqualRangeByLcId", IsDeterministic = true)]
    public static SqlBoolean EqualRangeByLcId([SqlFacet(MaxSize = -1)] SqlString value1, SqlInt32 index1, SqlInt32 count1,
      [SqlFacet(MaxSize = -1)] SqlString value2, SqlInt32 index2, SqlInt32 count2, SqlInt32 compareOptions, SqlInt32 lcId)
    {
      SqlInt32 result = CompareRangeByLcId(value1, index1, count1, value2, index2, count2, compareOptions, lcId);
      return !result.IsNull ? result.Value == 0 : SqlBoolean.Null;
    }

    [SqlFunction(Name = "strEqualByLcName", IsDeterministic = true)]
    public static SqlBoolean EqualByLcName([SqlFacet(MaxSize = -1)] SqlString value1, [SqlFacet(MaxSize = -1)] SqlString value2, SqlInt32 compareOptions, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      SqlInt32 result = CompareRangeByLcName(value1, SqlInt32.Null, SqlInt32.Null, value2, SqlInt32.Null, SqlInt32.Null, compareOptions, lcName);
      return !result.IsNull ? result.Value == 0 : SqlBoolean.Null;
    }

    [SqlFunction(Name = "strEqualRangeByLcName", IsDeterministic = true)]
    public static SqlBoolean EqualRangeByLcName([SqlFacet(MaxSize = -1)] SqlString value1, SqlInt32 index1, SqlInt32 count1,
      [SqlFacet(MaxSize = -1)] SqlString value2, SqlInt32 index2, SqlInt32 count2, SqlInt32 compareOptions, [SqlFacet(MaxSize = 128)] SqlString lcName)
    {
      SqlInt32 result = CompareRangeByLcName(value1, index1, count1, value2, index2, count2, compareOptions, lcName);
      return !result.IsNull ? result.Value == 0 : SqlBoolean.Null;
    }

    #endregion
    #endregion
    #region Table-valued functions

    [SqlFunction(Name = "strSplitToChars", IsDeterministic = true, FillRowMethodName = "SplitFillRow")]
		public static IEnumerable SplitToChars([SqlFacet(MaxSize = -1)] SqlString input, SqlInt32 index, SqlInt32 count)
		{
			if (input.IsNull)
				yield break;

			foreach (char c in input.Value.ToCharArray(
        index.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : index.Value, count.IsNull ? input.Value.Length - (index.IsNull ? 0 : index.Value) : count.Value))
				yield return c.ToString();
		}

    [SqlFunction(Name = "strSplitByChars", IsDeterministic = true, FillRowMethodName = "SplitFillRow")]
    public static IEnumerable SplitByChars([SqlFacet(MaxSize = -1)] SqlString input,  SqlString delimitChars, SqlInt32 count, SqlInt32 options)
    {
      if (input.IsNull || delimitChars.IsNull)
        yield break;
      //
      foreach (string s in input.Value.Split(delimitChars.Value.ToCharArray(), count.IsNull ? int.MaxValue : count.Value, options.IsNull ? StringSplitOptions.None : (StringSplitOptions)options.Value))
        yield return s;
    }

		[SqlFunction(Name = "strSplitByWords", IsDeterministic = true, FillRowMethodName = "SplitFillRow")]
    public static IEnumerable SplitByWords([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString separators,  SqlString delimitChars, SqlInt32 count, SqlInt32 options)
		{
			if (input.IsNull || separators.IsNull)
				yield break;

      foreach (string s in input.Value.Split(delimitChars.IsNull ? new string[] { separators.Value } : separators.Value.Split(delimitChars.Value.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
        count.IsNull ? int.MaxValue : count.Value, options.IsNull ? StringSplitOptions.None : (StringSplitOptions)options.Value))
				yield return s;
		}

    [SqlFunction(Name = "strSplit", IsDeterministic = true, FillRowMethodName = "SplitFillRow")]
    public static IEnumerable Split([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString separator, SqlInt32 count, SqlInt32 options)
    {
      if (input.IsNull || separator.IsNull)
        yield break;
      //
      foreach (string s in input.Value.Split(new string[] { separator.Value }, count.IsNull ? int.MaxValue : count.Value, options.IsNull ? StringSplitOptions.None : (StringSplitOptions)options.Value))
        yield return s;
    }

    [SqlFunction(Name = "strSplitSmart", IsDeterministic = true, FillRowMethodName = "SplitFillRow")]
    public static IEnumerable SplitSmart([SqlFacet(MaxSize = -1)] SqlString input,  SqlString separators,  SqlString trims,
       SqlString controlSeparators,  SqlString controlEscapes, [SqlFacet(MaxSize = -1)] SqlString controls, SqlInt32 count, SqlInt32 options)
    {
      if (input.IsNull || controlSeparators.IsNull || separators.IsNull)
        yield break;
      //
      foreach (string s in input.Value.Split(separators.IsNull ? null : separators.Value.ToCharArray(), trims.IsNull ? null : trims.Value.ToCharArray(),
        controlSeparators.IsNull ? null : controlSeparators.Value.ToCharArray(), controlEscapes.IsNull ? null : controlEscapes.Value.ToCharArray(), controls.IsNull ? null : controls.Value,
        count.IsNull ? int.MaxValue : count.Value, options.IsNull ? StringSplitOptionsEx.None : (StringSplitOptionsEx)options.Value))
        yield return s;
    }

		#endregion
		#region FillRow functions

		private static void SplitFillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlString Value)
		{
			Value = (string)obj;
		}

    #endregion
  }
}
