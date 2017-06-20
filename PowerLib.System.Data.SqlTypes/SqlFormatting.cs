using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.Collections;

namespace PowerLib.System.Data.SqlTypes
{
  public static class SqlFormatting
  {
    static SqlFormatting()
    {
      CurrentStringFormatting = DefaultStringFormatting;// SqlConfiguration.Init(typeof(StringFormatting).Name, DefaultStringFormatting);
      CurrentCollectionFormatting = DefaultCollectionFormatting;// SqlConfiguration.Init(typeof(CollectionFormatting).Name, DefaultCollectionFormatting);
      CurrentArrayFormatting = DefaultArrayFormatting;// SqlConfiguration.Init(typeof(ArrayFormatting).Name, DefaultArrayFormatting);
    }

    #region Formatting properties

    public const string NullText = "Null";

    public static readonly StringFormatting DefaultStringFormatting = new StringFormatting('\"', '\"', new[] { ' ', '\t', '\r', '\n' });

    public static readonly CollectionFormatting DefaultCollectionFormatting = new CollectionFormatting(',', '\\', new[] { ' ', '\t', '\r', '\n' });

    public static readonly ArrayFormatting DefaultArrayFormatting = new ArrayFormatting('{', '}', ',', '\\', new[] { ' ', '\t', '\r', '\n' });

    public static StringFormatting CurrentStringFormatting
    {
      get;// { return SqlConfiguration.Get<StringFormatting>(typeof(StringFormatting).Name); }
      set;// { SqlConfiguration.Set(typeof(StringFormatting).Name, value); }
    }

    public static CollectionFormatting CurrentCollectionFormatting
    {
      get;// { return SqlConfiguration.Get<CollectionFormatting>(typeof(CollectionFormatting).Name); }
      set;// { SqlConfiguration.Set(typeof(CollectionFormatting).Name, value); }
    }

    public static ArrayFormatting CurrentArrayFormatting
    {
      get;// { return SqlConfiguration.Get<ArrayFormatting>(typeof(ArrayFormatting).Name); }
      set;// { SqlConfiguration.Set(typeof(ArrayFormatting).Name, value); }
    }

    #endregion
    #region String methods

    public static string Quote(string s)
    {
      var stringFormatting = CurrentStringFormatting;
      return s.Quote(stringFormatting.QuoteChar, stringFormatting.EscapeChar);
    }

    public static string Unquote(string s)
    {
      var stringFormatting = CurrentStringFormatting;
      return s.Unquote(stringFormatting.QuoteChar, stringFormatting.EscapeChar);
    }

    #endregion
    #region Array methods

    public static T[] ParseArray<T>(string s, Func<string, T> itemParser, string itemPattern = null)
    {
      var arrayFormatting = CurrentArrayFormatting;
      var array = PwrArray.Parse<T>(s, itemParser, itemPattern,
        new[] { arrayFormatting.DelimiterChar },
        arrayFormatting.SpaceChars.ToArray(),
        new[] { arrayFormatting.EscapeChar },
        new[] { arrayFormatting.OpenBracketChar },
        new[] { arrayFormatting.CloseBracketChar });
      if (array == null)
        throw new FormatException();
      return array;
    }

    public static T[] ParseArray<T>(string s, Func<string, int, T> itemParser, string itemPattern = null)
    {
      var arrayFormatting = CurrentArrayFormatting;
      var array = PwrArray.Parse<T>(s, itemParser, itemPattern,
        new[] { arrayFormatting.DelimiterChar },
        arrayFormatting.SpaceChars.ToArray(),
        new[] { arrayFormatting.EscapeChar },
        new[] { arrayFormatting.OpenBracketChar },
        new[] { arrayFormatting.CloseBracketChar });
      if (array == null)
        throw new FormatException();
      return array;
    }

    public static Array ParseRegular<T>(string s, Func<string, T> itemParser, string itemPattern = null)
    {
      var arrayFormatting = CurrentArrayFormatting;
      var array = PwrArray.ParseAsRegular(s, itemParser, itemPattern,
        new[] { arrayFormatting.DelimiterChar },
        arrayFormatting.SpaceChars.ToArray(),
        new[] { arrayFormatting.EscapeChar },
        new[] { arrayFormatting.OpenBracketChar },
        new[] { arrayFormatting.CloseBracketChar });
      if (array == null)
        throw new FormatException();
      return array;
    }

    public static Array ParseRegular<T>(string s, Func<string, int, int[], T> itemParser, string itemPattern = null)
    {
      var arrayFormatting = CurrentArrayFormatting;
      var array = PwrArray.ParseAsRegular(s, itemParser, itemPattern,
        new[] { arrayFormatting.DelimiterChar },
        arrayFormatting.SpaceChars.ToArray(),
        new[] { arrayFormatting.EscapeChar },
        new[] { arrayFormatting.OpenBracketChar },
        new[] { arrayFormatting.CloseBracketChar });
      if (array == null)
        throw new FormatException();
      return array;
    }

    public static string Format<T>(T[] array, Func<T, string> itemFormatter)
    {
      var arrayFormatting = CurrentArrayFormatting;
      return PwrArray.Format(array, itemFormatter,
        arrayFormatting.DelimiterChar.ToString(),
        arrayFormatting.OpenBracketChar.ToString(),
        arrayFormatting.CloseBracketChar.ToString(), 0, array.Length);
    }

    public static string Format<T>(T[] array, Func<T, int, string> itemFormatter)
    {
      var arrayFormatting = CurrentArrayFormatting;
      return PwrArray.Format(array, itemFormatter,
        arrayFormatting.DelimiterChar.ToString(),
        arrayFormatting.OpenBracketChar.ToString(),
        arrayFormatting.CloseBracketChar.ToString(), 0, array.Length);
    }

    public static string Format<T>(T[] array, Func<T, string> itemFormatter, int index, int count)
    {
      var arrayFormatting = CurrentArrayFormatting;
      return PwrArray.Format(array, itemFormatter,
        arrayFormatting.DelimiterChar.ToString(),
        arrayFormatting.OpenBracketChar.ToString(),
        arrayFormatting.CloseBracketChar.ToString(), index, count);
    }

    public static string Format<T>(T[] array, Func<T, int, string> itemFormatter, int index, int count)
    {
      var arrayFormatting = CurrentArrayFormatting;
      return PwrArray.Format(array, itemFormatter,
        arrayFormatting.DelimiterChar.ToString(),
        arrayFormatting.OpenBracketChar.ToString(),
        arrayFormatting.CloseBracketChar.ToString(), index, count);
    }

    public static string Format<T>(T[] array, Func<T, string> itemFormatter, Range range)
    {
      var arrayFormatting = CurrentArrayFormatting;
      return PwrArray.Format(array, itemFormatter,
        arrayFormatting.DelimiterChar.ToString(),
        arrayFormatting.OpenBracketChar.ToString(),
        arrayFormatting.CloseBracketChar.ToString(), range);
    }

    public static string Format<T>(T[] array, Func<T, int, string> itemFormatter, Range range)
    {
      var arrayFormatting = CurrentArrayFormatting;
      return PwrArray.Format(array, itemFormatter,
        arrayFormatting.DelimiterChar.ToString(),
        arrayFormatting.OpenBracketChar.ToString(),
        arrayFormatting.CloseBracketChar.ToString(), range);
    }

    public static string FormatRegular<T>(Array array, Func<T, string> itemFormatter)
    {
      var arrayFormatting = CurrentArrayFormatting;
      return PwrArray.FormatAsRegular<T>(array, itemFormatter,
        i => arrayFormatting.DelimiterChar.ToString(),
        i => arrayFormatting.OpenBracketChar.ToString(),
        i => arrayFormatting.CloseBracketChar.ToString());
    }

    public static string FormatRegular<T>(Array array, Func<T, int, int[], string> itemFormatter)
    {
      var arrayFormatting = CurrentArrayFormatting;
      return PwrArray.FormatAsRegular<T>(array, itemFormatter,
        i => arrayFormatting.DelimiterChar.ToString(),
        i => arrayFormatting.OpenBracketChar.ToString(),
        i => arrayFormatting.CloseBracketChar.ToString());
    }

    #endregion
    #region Collection methods

    public static T[] ParseCollection<T>(string s, Func<string, T> itemParser, string itemPattern = null)
    {
      var collFormatting = CurrentArrayFormatting;
      var coll = PwrArray.Parse<T>(s, itemParser, itemPattern,
        new[] { collFormatting.DelimiterChar },
        collFormatting.SpaceChars.ToArray(),
        new[] { collFormatting.EscapeChar });
      if (coll == null)
        throw new FormatException();
      return coll;
    }

    public static T[] ParseCollection<T>(string s, Func<string, int, T> itemParser, string itemPattern = null)
    {
      var collFormatting = CurrentArrayFormatting;
      var coll = PwrArray.Parse<T>(s, itemParser, itemPattern,
        new[] { collFormatting.DelimiterChar },
        collFormatting.SpaceChars.ToArray(),
        new[] { collFormatting.EscapeChar });
      if (coll == null)
        throw new FormatException();
      return coll;
    }

    public static string Format<T>(IEnumerable<T> coll, Func<T, string> itemFormatter)
    {
      var collFormatting = CurrentCollectionFormatting;
      return coll.Format(itemFormatter, collFormatting.DelimiterChar.ToString());
    }

    public static string Format<T>(IEnumerable<T> coll, Func<T, int, string> itemFormatter)
    {
      var collFormatting = CurrentCollectionFormatting;
      return coll.Format(itemFormatter, collFormatting.DelimiterChar.ToString());
    }

    #endregion
    #region Configuration methods

    [SqlProcedure(Name = "setStringFormatting")]
    public static void SetStringFormatting(
      [SqlFacet(IsFixedLength = true, IsNullable = false, MaxSize = 1)] SqlString quoteChar,
      [SqlFacet(IsFixedLength = true, IsNullable = false, MaxSize = 1)] SqlString escapeChar,
      [SqlFacet(IsFixedLength = false, IsNullable = false, MaxSize = 128)] SqlString spaceChars)
    {
      var dsf = DefaultStringFormatting;
      CurrentStringFormatting = new StringFormatting(
        !quoteChar.IsNull && !string.IsNullOrEmpty(quoteChar.Value) ? quoteChar.Value[0] : dsf.QuoteChar,
        !escapeChar.IsNull && !string.IsNullOrEmpty(escapeChar.Value) ? escapeChar.Value[0] : dsf.EscapeChar,
        !spaceChars.IsNull && !string.IsNullOrEmpty(spaceChars.Value) ? spaceChars.Value.ToCharArray() : dsf.SpaceChars.ToArray());
    }

    [SqlProcedure(Name = "getStringFormatting")]
    public static void GetStringFormatting(
      [SqlFacet(IsFixedLength = true, IsNullable = false, MaxSize = 1)] out SqlString quoteChar,
      [SqlFacet(IsFixedLength = true, IsNullable = false, MaxSize = 1)] out SqlString escapeChar,
      [SqlFacet(IsFixedLength = false, IsNullable = false, MaxSize = 128)] out SqlString spaceChars)
    {
      var csf = CurrentStringFormatting;
      quoteChar = csf.QuoteChar.ToString();
      escapeChar = csf.EscapeChar.ToString();
      spaceChars = string.Join("", csf.SpaceChars);
    }

    [SqlProcedure(Name = "setArrayFormatting")]
    public static void SetArrayFormatting(
      [SqlFacet(IsFixedLength = true, IsNullable = false, MaxSize = 1)] SqlString openBracketChar,
      [SqlFacet(IsFixedLength = true, IsNullable = false, MaxSize = 1)] SqlString closeBracketChar,
      [SqlFacet(IsFixedLength = true, IsNullable = false, MaxSize = 1)] SqlString delimiterChar,
      [SqlFacet(IsFixedLength = true, IsNullable = false, MaxSize = 1)] SqlString escapeChar,
      [SqlFacet(IsFixedLength = false, IsNullable = false, MaxSize = 128)] SqlString spaceChars)
    {
      var daf = DefaultArrayFormatting;
      CurrentArrayFormatting = new ArrayFormatting(
        !openBracketChar.IsNull && !string.IsNullOrEmpty(openBracketChar.Value) ? openBracketChar.Value[0] : daf.OpenBracketChar,
        !closeBracketChar.IsNull && !string.IsNullOrEmpty(closeBracketChar.Value) ? closeBracketChar.Value[0] : daf.CloseBracketChar,
        !delimiterChar.IsNull && !string.IsNullOrEmpty(delimiterChar.Value) ? delimiterChar.Value[0] : daf.DelimiterChar,
        !escapeChar.IsNull && !string.IsNullOrEmpty(escapeChar.Value) ? escapeChar.Value[0] : daf.EscapeChar,
        !spaceChars.IsNull && !string.IsNullOrEmpty(spaceChars.Value) ? spaceChars.Value.ToCharArray() : daf.SpaceChars.ToArray());
    }

    [SqlProcedure(Name = "getArrayFormatting")]
    public static void GetArrayFormatting(
      [SqlFacet(IsFixedLength = true, IsNullable = false, MaxSize = 1)] out SqlString openBracketChar,
      [SqlFacet(IsFixedLength = true, IsNullable = false, MaxSize = 1)] out SqlString closeBracketChar,
      [SqlFacet(IsFixedLength = true, IsNullable = false, MaxSize = 1)] out SqlString delimiterChar,
      [SqlFacet(IsFixedLength = true, IsNullable = false, MaxSize = 1)] out SqlString escapeChar,
      [SqlFacet(IsFixedLength = false, IsNullable = false, MaxSize = 128)] out SqlString spaceChars)
    {
      var caf = CurrentArrayFormatting;
      openBracketChar = caf.OpenBracketChar.ToString();
      closeBracketChar = caf.CloseBracketChar.ToString();
      delimiterChar = caf.DelimiterChar.ToString();
      escapeChar = caf.EscapeChar.ToString();
      spaceChars = string.Join("", caf.SpaceChars);
    }

    #endregion
  }

  public class ElementFormatting
  {
    public ElementFormatting(char escapeChar, char[] spaceChars)
    {
      EscapeChar = escapeChar;
      SpaceChars = new ReadOnlyCollection<char>(spaceChars);
    }

    public char EscapeChar
    {
      get;
      private set;
    }

    public IReadOnlyList<char> SpaceChars
    {
      get;
      private set;
    }
  }

  public class StringFormatting : ElementFormatting
  {
    public StringFormatting(char quoteChar, char escapeChar, char[] spaceChars)
      : base(escapeChar, spaceChars)
    {
      QuoteChar = quoteChar;
    }

    public char QuoteChar
    {
      get;
      private set;
    }
  }

  public class CollectionFormatting : ElementFormatting
  {
    public CollectionFormatting(char delimiterChar, char escapeChar, char[] spaceChars)
      : base(escapeChar, spaceChars)
    {
      DelimiterChar = delimiterChar;
    }

    public char DelimiterChar
    {
      get;
      private set;
    }
  }

  public class ArrayFormatting : CollectionFormatting
  {
    public ArrayFormatting(char openBracketChar, char closeBracketChar, char delimiterChar, char escapeChar, char[] spaceChars)
      : base(delimiterChar, escapeChar, spaceChars)
    {
      OpenBracketChar = openBracketChar;
      CloseBracketChar = closeBracketChar;
    }

    public char OpenBracketChar
    {
      get;
      private set;
    }

    public char CloseBracketChar
    {
      get;
      private set;
    }
  }
}
