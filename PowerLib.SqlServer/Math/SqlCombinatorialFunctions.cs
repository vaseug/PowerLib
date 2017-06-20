using System;
using System.Collections;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Math.Combinatorial;

namespace PowerLib.SqlServer.Math
{
  public static class SqlCombinatorialFunctions
  {
    #region Combinatorial function methods

    [SqlFunction(Name = "mathArrangements", IsDeterministic = true, FillRowMethodName = "CombinatorialInt32FillRow")]
    public static IEnumerable Arrangements(SqlInt32 cardinal, SqlInt32 variation)
    {
      if (cardinal.IsNull || variation.IsNull)
        yield break;

      var c = new Arrangement(cardinal.Value, variation.Value);
      for (var success = c.First(); success; success = c.Next())
        yield return c;
    }

    [SqlFunction(Name = "mathArrangementsWithRepeats", IsDeterministic = true, FillRowMethodName = "CombinatorialInt32FillRow")]
    public static IEnumerable ArrangementsWithRepeats(SqlInt32 cardinal, SqlInt32 variation)
    {
      if (cardinal.IsNull || variation.IsNull)
        yield break;

      var c = new ArrangementWithRepeats(cardinal.Value, variation.Value);
      for (var success = c.First(); success; success = c.Next())
        yield return c;
    }

    [SqlFunction(Name = "mathCombinations", IsDeterministic = true, FillRowMethodName = "CombinatorialInt32FillRow")]
    public static IEnumerable Combinations(SqlInt32 cardinal, SqlInt32 variation)
    {
      if (cardinal.IsNull || variation.IsNull)
        yield break;

      var c = new Combination(cardinal.Value, variation.Value);
      for (var success = c.First(); success; success = c.Next())
        yield return c;
    }

    [SqlFunction(Name = "mathPermutations", IsDeterministic = true, FillRowMethodName = "CombinatorialInt32FillRow")]
    public static IEnumerable Permutations(SqlInt32 cardinal)
    {
      if (cardinal.IsNull)
        yield break;

      var c = new Permutation(cardinal.Value);
      for (var success = c.First(); success; success = c.Next())
        yield return c;
    }

    [SqlFunction(Name = "mathSubsets", IsDeterministic = true, FillRowMethodName = "CombinatorialBooleanFillRow")]
    public static IEnumerable Subsets(SqlInt32 cardinal)
    {
      if (cardinal.IsNull)
        yield break;

      var c = new Subset(cardinal.Value);
      for (var success = c.First(); success; success = c.Next())
        yield return c;
    }

    #endregion
    #region FillRow methods

    private static void CombinatorialBooleanFillRow(object obj, out SqlBytes Value)
    {
      if (obj == null)
        Value = SqlBytes.Null;
      else
      {
        var combin = ((Combinatorial<Boolean>)obj).Select(t => (Boolean?)t).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (new NulBooleanStreamedArray(result.Stream, System.SizeEncoding.B4, combin, true, false)) ;
        Value = result;
      }
    }

    private static void CombinatorialInt32FillRow(object obj, out SqlBytes Value)
    {
      if (obj == null)
        Value = SqlBytes.Null;
      else
      {
        var combin = ((Combinatorial<Int32>)obj).Select(t => (Int32?)t).ToArray();
        var result = new SqlBytes(new MemoryStream());
        using (new NulInt32StreamedArray(result.Stream, System.SizeEncoding.B4, true, combin, true, false)) ;
        Value = result;
      }
    }

    #endregion
  }
}
