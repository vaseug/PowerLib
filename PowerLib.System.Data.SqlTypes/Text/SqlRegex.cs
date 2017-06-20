using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.IO;

namespace PowerLib.System.Data.SqlTypes.Text
{
  [SqlUserDefinedType(Format.UserDefined, Name = "Regex", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlRegex : INullable, IBinarySerialize
  {
    private static readonly SqlRegex @null = new SqlRegex();

    private Regex _regex;

    #region Constructor

    public SqlRegex()
    {
      _regex = null;
    }

    public SqlRegex(string pattern)
    {
      _regex = new Regex(pattern);
    }

    public SqlRegex(string pattern, RegexOptions options)
    {
      _regex = new Regex(pattern, options);
    }

    #endregion
    #region Properties

    public Regex Regex
    {
      get { return _regex; }
    }

    public static SqlRegex Null
    {
      get { return @null; }
    }

    public bool IsNull
    {
      get { return _regex == null; }
    }

    public SqlInt32 Options
    {
      get { return !IsNull ? (int)_regex.Options : SqlInt32.Null; }
    }

    [SqlFacet(IsFixedLength = false, MaxSize = -1)]
    public SqlString Pattern
    {
      get { return !IsNull ? _regex.ToString() : SqlString.Null; }
    }

    #endregion
    #region Methods

    public static SqlRegex Parse(SqlString s)
    {
      return !s.IsNull ? new SqlRegex(s.Value, SqlRegexFunctions.DefaultRegexOptions) : Null;
    }

    public override String ToString()
    {
      return _regex != null ? _regex.ToString() : SqlFormatting.NullText;
    }

    [SqlMethod]
    public SqlBoolean IsMatch([SqlFacet(MaxSize = -1)] SqlString input)
    {
      if (IsNull || input.IsNull)
        return SqlBoolean.Null;

      return _regex.IsMatch(input.Value);
    }

    [SqlMethod]
    public SqlBoolean IsMatchAt([SqlFacet(MaxSize = -1)] SqlString input, SqlInt32 startat)
    {
      if (IsNull || input.IsNull)
        return SqlBoolean.Null;

      return _regex.IsMatch(input.Value, !startat.IsNull ? startat.Value : 0);
    }

    [SqlMethod]
    [return: SqlFacet(MaxSize = -1)]
    public SqlString Replace([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString replacement)
    {
      if (IsNull || input.IsNull || replacement.IsNull)
        return SqlString.Null;

      return _regex.Replace(input.Value, replacement.Value);
    }

    [SqlMethod]
    [return: SqlFacet(MaxSize = -1)]
    public SqlString ReplaceLim([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString replacement, SqlInt32 count)
    {
      if (IsNull || input.IsNull || replacement.IsNull)
        return SqlString.Null;

      return _regex.Replace(input.Value, replacement.Value, !count.IsNull ? count.Value : int.MaxValue);
    }

    [SqlMethod]
    [return: SqlFacet(MaxSize = -1)]
    public SqlString ReplaceLimAt([SqlFacet(MaxSize = -1)] SqlString input, [SqlFacet(MaxSize = -1)] SqlString replacement, SqlInt32 count, SqlInt32 startat)
    {
      if (IsNull || input.IsNull || replacement.IsNull)
        return SqlString.Null;

      return _regex.Replace(input.Value, replacement.Value, !count.IsNull ? count.Value : int.MaxValue, !startat.IsNull ? startat.Value : 0);
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _regex = new Regex(rd.BaseStream.ReadString(Encoding.UTF8, SizeEncoding.VB), rd.BaseStream.ReadEnum<RegexOptions>());
    }

    public void Write(BinaryWriter wr)
    {
      wr.BaseStream.WriteString(_regex.ToString(), Encoding.UTF8, SizeEncoding.VB);
      wr.BaseStream.WriteEnum(_regex.Options);
    }

    #endregion
  }
}
