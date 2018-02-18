using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Text;

namespace PowerLib.SqlServer
{
  public static class SqlRuntime
  {
    private const string MaxBufferSizeName = nameof(MaxBufferSize);
    private const string IoBufferSizeName = nameof(IoBufferSize);
    private const string TextEncodingName = nameof(TextEncoding);
    private const string FileEncodingName = nameof(FileEncoding);
    private const string WebEncodingName = nameof(WebEncoding);

    #region Constructor

    static SqlRuntime()
    {
      SqlConfiguration.Init(MaxBufferSizeName, (long)(int.MaxValue - 1));
      SqlConfiguration.Init(IoBufferSizeName, 4096);
      SqlConfiguration.Init(TextEncodingName, Encoding.Unicode);
      SqlConfiguration.Init(FileEncodingName, Encoding.UTF8);
      SqlConfiguration.Init(WebEncodingName, Encoding.UTF8);
    }

    #endregion
    #region Get sql server configuration

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

    #endregion
    #region Max buffer size
    public static long MaxBufferSize
    {
      get { return SqlConfiguration.Get<long>(MaxBufferSizeName); }
    }

    [SqlProcedure(Name = "setMaxBufferSize")]
    public static void SetMaxBufferSize(SqlInt64 size)
    {
      if (!size.IsNull)
        SqlConfiguration.Set(MaxBufferSizeName, size.Value);
    }

    #endregion
    #region IO buffer size

    public static int IoBufferSize
    {
      get { return SqlConfiguration.Get<int>(IoBufferSizeName); }
    }

    [SqlProcedure(Name = "setIoBufferSize")]
    public static void SetIoBufferSize(SqlInt32 size)
    {
      if (!size.IsNull)
        SqlConfiguration.Set(IoBufferSizeName, size.Value);
    }

    #endregion
    #region Text encoding
    public static Encoding TextEncoding
    {
      get { return SqlConfiguration.Get<Encoding>(TextEncodingName); }
    }

    [SqlProcedure(Name = "setTextEncodingByCpId")]
    public static void SetTextEncodingByCpId(SqlInt32 cpId)
    {
      if (!cpId.IsNull)
        SqlConfiguration.Set(TextEncodingName, Encoding.GetEncoding(cpId.Value));
    }

    [SqlProcedure(Name = "setTextEncodingByCpName")]
    public static void SetTextEncodingByCpName(SqlString cpName)
    {
      if (!cpName.IsNull)
        SqlConfiguration.Set(TextEncodingName, Encoding.GetEncoding(cpName.Value));
    }

    #endregion
    #region File encoding

    public static Encoding FileEncoding
    {
      get { return SqlConfiguration.Get<Encoding>(FileEncodingName); }
    }

    [SqlProcedure(Name = "setFileEncodingByCpId")]
    public static void SetFileEncodingByCpId(SqlInt32 cpId)
    {
      if (!cpId.IsNull)
        SqlConfiguration.Set(FileEncodingName, Encoding.GetEncoding(cpId.Value));
    }

    [SqlProcedure(Name = "setFileEncodingByCpName")]
    public static void SetFileEncodingByCpName(SqlString cpName)
    {
      if (!cpName.IsNull)
        SqlConfiguration.Set(FileEncodingName, Encoding.GetEncoding(cpName.Value));
    }

    #endregion
    #region Web encoding

    public static Encoding WebEncoding
    {
      get { return SqlConfiguration.Get<Encoding>(WebEncodingName); }
    }

    [SqlProcedure(Name = "setWebEncodingByCpId")]
    public static void SetWebEncodingByCpId(SqlInt32 cpId)
    {
      if (!cpId.IsNull)
        SqlConfiguration.Set(WebEncodingName, Encoding.GetEncoding(cpId.Value));
    }

    [SqlProcedure(Name = "setWebEncodingByCpName")]
    public static void SetWebEncodingByCpName(SqlString cpName)
    {
      if (!cpName.IsNull)
        SqlConfiguration.Set(WebEncodingName, Encoding.GetEncoding(cpName.Value));
    }

    #endregion
  }
}
