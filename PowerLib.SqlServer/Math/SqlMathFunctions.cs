using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlTypes;
using System.Text;

namespace PowerLib.SqlServer.Math
{
  using Math = global::System.Math;

  public static class SqlMathFunctions
  {
    public static SqlDouble Sample()
    {
      return new Random().NextDouble();
    }

    public static SqlDouble Log(SqlDouble value)
    {
      return Math.Log(value.Value);
    }

    public static SqlDouble Log(SqlDouble value, SqlDouble numbase)
    {
      return value.IsNull || numbase.IsNull ? SqlDouble.Null : Math.Log(value.Value, numbase.Value);
    }
  }
}
