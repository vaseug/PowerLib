using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.Linq;
using PowerLib.System.Data.SqlTypes.Collections;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  public static class SqlNameValueCollectionFunctions
  {
    [SqlFunction(Name = "nvCollCreate", IsDeterministic = true)]
    public static SqlNameValueCollection NameValueCollCreate()
    {
      return new SqlNameValueCollection(new NameValueCollection());
    }

    [SqlFunction(Name = "nvCollParse", IsDeterministic = true)]
    public static SqlNameValueCollection NameValueCollParse([SqlFacet(MaxSize = -1, IsNullable = false)]SqlString str,
      SqlString keyDelimiter, SqlString itemDelimiter)
    {
      if (str.IsNull || keyDelimiter.IsNull || itemDelimiter.IsNull)
        return SqlNameValueCollection.Null;

      var coll = new NameValueCollection();
      var items = str.Value.Split(new[] { itemDelimiter.Value }, StringSplitOptions.RemoveEmptyEntries);
      items.ForEach(t =>
      {
        var parts = t.Split(new[] { keyDelimiter.Value }, 2, StringSplitOptions.RemoveEmptyEntries);
        coll.Add(parts[0], parts.Length > 1 ? parts[1] : null);
      });
      return new SqlNameValueCollection(coll);
    }

    [SqlFunction(Name = "nvCollFormat", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString NameValueCollFormat(SqlNameValueCollection coll, SqlString keyDelimiter, SqlString itemDelimiter)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull || keyDelimiter.IsNull || itemDelimiter.IsNull)
        return SqlFormatting.NullText;

      var sb = new StringBuilder();
      coll.Value.OfType<String>().ForEach((t, i) =>
      {
        if (i > 0)
          sb.Append(itemDelimiter.Value);
        sb.Append(t);
        sb.Append(keyDelimiter.Value);
        sb.Append(coll.Value[t]);
      });
      return sb.ToString();
    }

    [SqlFunction(Name = "nvCollCount", IsDeterministic = true)]
    public static SqlInt32 NameValueCollCount(SqlNameValueCollection coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlInt32.Null;

      return coll.Value.Count;
    }

    [SqlFunction(Name = "nvCollGet", IsDeterministic = true)]
    public static SqlString NameValueCollGet(SqlNameValueCollection coll, [SqlFacet(MaxSize = -1)]SqlString name)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        return SqlString.Null;

      return coll.Value[name.IsNull ? default(String) : name.Value] ?? SqlString.Null;
    }

    [SqlFunction(Name = "nvCollSet", IsDeterministic = true)]
    public static SqlNameValueCollection NameValueCollSet(SqlNameValueCollection coll, [SqlFacet(MaxSize = -1)]SqlString name, [SqlFacet(MaxSize = -1)]SqlString value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (!coll.IsNull)
        coll.Value.Set(name.IsNull ? default(String) : name.Value, value.IsNull ? default(String) : value.Value);
      return coll;
    }

    [SqlFunction(Name = "nvCollAdd", IsDeterministic = true)]
    public static SqlNameValueCollection NameValueCollAdd(SqlNameValueCollection coll, [SqlFacet(MaxSize = -1)]SqlString name, [SqlFacet(MaxSize = -1)]SqlString value)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (!coll.IsNull)
        coll.Value.Add(name.IsNull ? default(String) : name.Value, value.IsNull ? default(String) : value.Value);
      return coll;
    }

    [SqlFunction(Name = "nvCollRemove", IsDeterministic = true)]
    public static SqlNameValueCollection NameValueCollRemove(SqlNameValueCollection coll, [SqlFacet(MaxSize = -1)]SqlString name)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (!coll.IsNull)
        coll.Value.Remove(name.IsNull ? default(String) : name.Value);
      return coll;
    }

    [SqlFunction(Name = "nvCollClear", IsDeterministic = true)]
    public static SqlNameValueCollection NameValueCollClear(SqlNameValueCollection coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (!coll.IsNull)
        coll.Value.Clear();
      return coll;
    }

    [SqlFunction(Name = "nvCollAddRange", IsDeterministic = true)]
    public static SqlNameValueCollection NameValueCollAddRange(SqlNameValueCollection coll, SqlNameValueCollection range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range == null)
        throw new ArgumentNullException("range");

      if (!coll.IsNull && !range.IsNull)
        coll.Value.Add(range.Value);
      return coll;
    }

    [SqlFunction(Name = "nvCollGetValues", IsDeterministic = true, FillRowMethodName = "NameValueCollFillRow")]
    public static IEnumerable NameValueCollGetValues(SqlNameValueCollection coll, [SqlFacet(MaxSize = -1)]SqlString name)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;
      foreach (String value in coll.Value.GetValues(name.IsNull ? default(String) : name.Value))
        yield return new Tuple<String, String>(name.IsNull ? default(String) : name.Value, value);
    }

    [SqlFunction(Name = "nvCollEnumerate", IsDeterministic = true, FillRowMethodName = "NameValueCollFillRow")]
    public static IEnumerable NameValueCollEnumerate(SqlNameValueCollection coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      if (coll.IsNull)
        yield break;
      foreach (String name in coll.Value)
      {
        var values = coll.Value.GetValues(name);
        if (values == null)
          yield return new Tuple<String, String>(name, default(String));
        else
          foreach (String value in values)
            yield return new Tuple<String, String>(name, value);
      }
    }

    private static void NameValueCollFillRow(object obj, [SqlFacet(MaxSize = -1)]out SqlString Name, [SqlFacet(MaxSize = -1)]out SqlString Value)
    {
      Tuple<String, String> tuple = (Tuple<String, String>)obj;
      Name = tuple.Item1 ?? SqlString.Null;
      Value = tuple.Item2 ?? SqlString.Null;
    }
  }
}
