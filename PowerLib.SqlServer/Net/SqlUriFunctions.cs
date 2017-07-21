using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace PowerLib.SqlServer.Net
{
	public static class SqlUriFunctions
	{
    #region Uri functions

    [SqlFunction(Name = "uriCheckHostName", IsDeterministic = true)]
    public static SqlInt32 CheckHostName(SqlString hostName)
    {
      return hostName.IsNull ? SqlInt32.Null : (Int32)Uri.CheckHostName(hostName.Value);
    }

    [SqlFunction(Name = "uriCheckSchemeName", IsDeterministic = true)]
    public static SqlBoolean CheckSchemeName(SqlString schemeName)
    {
      return schemeName.IsNull ? SqlBoolean.Null : Uri.CheckSchemeName(schemeName.Value);
    }

    [SqlFunction(Name = "uriFromHex", IsDeterministic = true)]
    public static SqlInt32 FromHex([SqlFacet(IsFixedLength = true, MaxSize = 1)]SqlString digit)
    {
      return digit.IsNull || digit.Value.Length != 1 ? SqlInt32.Null : (Int32)Uri.FromHex(digit.Value[0]);
    }

    [SqlFunction(Name = "uriHexEscape", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString HexEscape([SqlFacet(MaxSize = -1)]SqlString str)
    {
      if (str.IsNull)
        return SqlString.Null;

      var sb = new StringBuilder();
      for (int i = 0; i < str.Value.Length; i++)
        sb.Append(Uri.HexEscape(str.Value[i]));
      return sb.ToString();
    }

    [SqlFunction(Name = "uriHexUnescape", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString HexUnescape([SqlFacet(MaxSize = -1)]SqlString pattern, SqlInt32 index, SqlInt32 count)
    {
      if (pattern.IsNull || index.IsNull)
        return SqlString.Null;
      if (index.Value < 0)
        throw new ArgumentOutOfRangeException();

      var sb = new StringBuilder();
      for (int indexValue = index.IsNull ? 0 : index.Value, countValue = count.IsNull ? int.MaxValue : count.Value; indexValue < pattern.Value.Length && countValue > 0; countValue--)
        sb.Append(Uri.HexUnescape(pattern.Value, ref indexValue));
      return sb.ToString();
    }

    [SqlFunction(Name = "uriIsHexDigit", IsDeterministic = true)]
    public static SqlBoolean IsHexDigit([SqlFacet(IsFixedLength = true, MaxSize = 1)]SqlString character)
    {
      return character.IsNull || character.Value.Length != 1 ? SqlBoolean.Null : Uri.IsHexDigit(character.Value[0]);
    }

    [SqlFunction(Name = "uriIsHexEncoding", IsDeterministic = true)]
    public static SqlBoolean IsHexEncoding([SqlFacet(MaxSize = -1)]SqlString pattern, SqlInt32 index)
    {
      return pattern.IsNull || index.IsNull ? SqlBoolean.Null : Uri.IsHexEncoding(pattern.Value, index.Value);
    }

    [SqlFunction(Name = "uriIsWellFormedUriString", IsDeterministic = true)]
    public static SqlBoolean IsWellFormedUriString([SqlFacet(MaxSize = -1)]SqlString uriString, SqlInt32 uriKind)
    {
      return uriString.IsNull || uriKind.IsNull ? SqlBoolean.Null : Uri.IsWellFormedUriString(uriString.Value, (UriKind)uriKind.Value);
    }

    [SqlFunction(Name = "uriEscape", IsDeterministic = true)]
    [return:SqlFacet(MaxSize = -1)]
    public static SqlString EscapeUri([SqlFacet(MaxSize = -1)] SqlString str)
    {
      return str.IsNull ? SqlString.Null : Uri.EscapeUriString(str.Value);
    }

    [SqlFunction(Name = "uriEscapeData", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString EscapeData([SqlFacet(MaxSize = -1)] SqlString str)
    {
      return str.IsNull ? SqlString.Null : Uri.EscapeDataString(str.Value);
    }

    [SqlFunction(Name = "uriUnescapeData", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString UnescapeData([SqlFacet(MaxSize = -1)] SqlString str)
    {
      return str.IsNull ? SqlString.Null : Uri.UnescapeDataString(str.Value);
    }

    [SqlFunction(Name = "uriGetAbsolutePath", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GetAbsolutePath([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlString.Null : new Uri(uriString.Value).AbsolutePath;
    }

    [SqlFunction(Name = "uriGetAbsoluteUri", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GetAbsoluteUri([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlString.Null : new Uri(uriString.Value).AbsoluteUri;
    }

    [SqlFunction(Name = "uriGetAuthority", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GetAuthority([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlString.Null : new Uri(uriString.Value).Authority;
    }

    [SqlFunction(Name = "uriGetDnsSafeHost", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GetDnsSafeHost([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlString.Null : new Uri(uriString.Value).DnsSafeHost;
    }

    [SqlFunction(Name = "uriGetFragment", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GetFragment([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlString.Null : new Uri(uriString.Value).Fragment;
    }

    [SqlFunction(Name = "uriGetHost", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GetHost([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlString.Null : new Uri(uriString.Value).Host;
    }

    [SqlFunction(Name = "uriGetHostNameType", IsDeterministic = true)]
    public static SqlInt32 GetHostNameType([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlInt32.Null : (Int32)new Uri(uriString.Value).HostNameType;
    }

    [SqlFunction(Name = "uriIsAbsoluteUri", IsDeterministic = true)]
    public static SqlBoolean IsAbsoluteUri([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlBoolean.Null : new Uri(uriString.Value).IsAbsoluteUri;
    }

    [SqlFunction(Name = "uriIsDefaultPort", IsDeterministic = true)]
    public static SqlBoolean IsDefaultPort([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlBoolean.Null : new Uri(uriString.Value).IsDefaultPort;
    }

    [SqlFunction(Name = "uriIsFile", IsDeterministic = true)]
    public static SqlBoolean IsFile([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlBoolean.Null : new Uri(uriString.Value).IsFile;
    }
  
    [SqlFunction(Name = "uriIsLoopback", IsDeterministic = true)]
    public static SqlBoolean IsLoopback([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlBoolean.Null : new Uri(uriString.Value).IsLoopback;
    }

    [SqlFunction(Name = "uriIsUnc", IsDeterministic = true)]
    public static SqlBoolean IsUnc([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlBoolean.Null : new Uri(uriString.Value).IsUnc;
    }

    [SqlFunction(Name = "uriGetLocalPath", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GetLocalPath([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlString.Null : new Uri(uriString.Value).LocalPath;
    }

    [SqlFunction(Name = "uriGetOriginalString", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GetOriginalString([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlString.Null : new Uri(uriString.Value).OriginalString;
    }

    [SqlFunction(Name = "uriGetPathAndQuery", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GetPathAndQuery([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlString.Null : new Uri(uriString.Value).PathAndQuery;
    }

    [SqlFunction(Name = "uriGetPort", IsDeterministic = true)]
    public static SqlInt32 GetPort([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      if (uriString.IsNull)
        return SqlInt32.Null;
      var uriPort = new Uri(uriString.Value).Port;
      return uriPort < 0 ? SqlInt32.Null : uriPort;
    }

    [SqlFunction(Name = "uriGetQuery", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GetQuery([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlString.Null : new Uri(uriString.Value).Query;
    }

    [SqlFunction(Name = "uriGetScheme", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GetScheme([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlString.Null : new Uri(uriString.Value).Scheme;
    }

    [SqlFunction(Name = "uriUserEscaped", IsDeterministic = true)]
    public static SqlBoolean UserEscaped([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlBoolean.Null : new Uri(uriString.Value).UserEscaped;
    }

    [SqlFunction(Name = "uriGetUserInfo", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GetUserInfo([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlString.Null : new Uri(uriString.Value).UserInfo;
    }

    [SqlFunction(Name = "uriBuildString", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString Build([SqlFacet(MaxSize = -1)] SqlString scheme,
      [SqlFacet(MaxSize = -1)] SqlString userName, [SqlFacet(MaxSize = -1)] SqlString password,
      [SqlFacet(MaxSize = -1)] SqlString host, [SqlFacet(MaxSize = -1)] SqlInt32 port,
      [SqlFacet(MaxSize = -1)] SqlString path,
      [SqlFacet(MaxSize = -1)] SqlString query, [SqlFacet(MaxSize = -1)] SqlString fragment)
    {
      var urib = new UriBuilder();
      if (!scheme.IsNull)
        urib.Scheme = scheme.Value;
      if (!userName.IsNull)
        urib.UserName = userName.Value;
      if (!password.IsNull)
        urib.Password = password.Value;
      if (!host.IsNull)
        urib.Host = host.Value;
      if (!port.IsNull)
        urib.Port = port.Value;
      if (!path.IsNull)
        urib.Path = path.Value;
      if (!query.IsNull)
        urib.Query = query.Value;
      if (!fragment.IsNull)
        urib.Fragment = fragment.Value;
      return urib.ToString();
    }

    [SqlFunction(Name = "uriGetSegments", IsDeterministic = true, FillRowMethodName = "SegmentsFillRow")]
    public static IEnumerable GetSegments([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      if (uriString.IsNull)
        yield break;

      var results = new Uri(uriString.Value).Segments;
      for (int i = 0; i < results.Length; i++)
        yield return results[i];
    }

    #endregion
    #region FillRow functions

    private static void SegmentsFillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlString Value)
		{
			Value = (string)obj;
		}

		#endregion
	}
}
