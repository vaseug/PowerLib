using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace PowerLib.System.Data.SqlTypes.Net
{
	public static class SqlUriFunctions
	{
    #region Uri functions

    [SqlFunction(Name = "uriBuild", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlUri Build([SqlFacet(MaxSize = -1)] SqlString scheme,
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
      return new SqlUri(urib.Uri);
    }

    [SqlFunction(Name = "uriCreate", IsDeterministic = true)]
    public static SqlUri Create([SqlFacet(MaxSize = -1)] SqlString uriString)
    {
      return uriString.IsNull ? SqlUri.Null : new SqlUri(uriString.Value);
    }

    [SqlFunction(Name = "uriSegments", IsDeterministic = true, FillRowMethodName = "SegmentsFillRow")]
    public static IEnumerable Segments(SqlUri uri)
    {
      if (uri.IsNull)
        yield break;

      var results = uri.Uri.Segments;
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
