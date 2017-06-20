using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Threading;
using PowerLib.System.Data.SqlTypes.IO;

namespace PowerLib.SqlServer
{
  public class SqlRuntime
  {
    public static int GetLcid()
    {
      using (SqlConnection connection = new SqlConnection("context connection=true"))
      {
        connection.Open();
        SqlCommand command = new SqlCommand(@"select lcid from sys.syslanguages where e langid = @@LANGID", connection);
        return (int)command.ExecuteScalar();
      }
    }

    public static T GetVariable<T>(string variable)
    {
      using (SqlConnection connection = new SqlConnection("context connection=true"))
      {
        connection.Open();
        SqlCommand command = new SqlCommand(string.Format(@"select @@{0}", variable), connection);
        return (T)command.ExecuteScalar();
      }
    }

    #region Max buffer size

    private const long PresetMaxBufferSize = int.MaxValue - 1;

    private static long maxBufferSize = PresetMaxBufferSize;

    internal static long MaxBufferSize
    {
      get { return maxBufferSize; }
    }

    [SqlProcedure(Name = "setMaxBufferSize")]
    public static void SetMaxBufferSize(SqlInt64 size)
    {
      if (!size.IsNull && size.Value > PresetMaxBufferSize)
        throw new ArgumentOutOfRangeException("size");

      maxBufferSize = size.IsNull ? PresetMaxBufferSize : size.Value;
    }

    #endregion
    #region IO buffer size

    private const int PresetIoBufferSize = 32768;

    [SqlProcedure(Name = "setIoBufferSize")]
    public static void SetIoBufferSize(SqlInt32 size)
    {
      SqlTypesIOExtension.IoBufferSize = size.IsNull ? PresetIoBufferSize : size.Value;
    }

    #endregion
  }
}
