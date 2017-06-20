using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Reflection;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using PowerLib.System.Linq;

namespace PowerLib.SqlClr.Deploy.Utility
{
  class Program
  {
    private const string help_text = @"
Microsoft SQL server CLR assembly deployment utility.

syntax:

  sqlclrdu.exe <command> <options> <assembly>

commands:

  create - Create assembly on SQL server database.

  alter - Alter assembly on SQL server database.

  drop - Drop assembly on SQL server database.

  manage - Manage database (-enable-clr, -trustworthy keys accepted).

options:

  -assembly:<assembly_name> - Assembly name in SQL server.

  -script:<script_file> - SQL script file path.

  -map:<map_file> - Map referenced types to SQL Server database objects.

  -encoding:<script_encoding> - SQL script file encoding.

  -connection:<connection_string> - SQL server connection string.

  -permission:[safe|ext_access|unsafe] - One of three different levels of security in which your code can run.

  -owner:<owner> - Database user or role.

  -schema:<schema> - Database schema.

  -unchecked - Applied with ALTER ASSEMBLY command and add UNCHECKED DATA option.

  -invisible - Only register assembly without its contents.

  -append - Append to SQL script file.

  -strong - Specifies that the assembly is defined by a strong name.

  -enable-clr - Sets 'clr enabled' option for SQL server.

  -trustworthy - Sets 'trustworthy' option for database.

comments:
  If string values contain spaces, they must be enclosed in double quotes.
  For example, -connection:""Data Source = MASTER\MSSQLSERVER2016;Initial Catalog = AdventureWorks2016; Integrated Security = True""
";

    private const string permission_Safe = "safe";
    private const string permission_ExternalAccess = "ext_access";
    private const string permission_Unsafe = "unsafe";

    private const string cmd_Create = "create";
    private const string cmd_Drop = "drop";
    private const string cmd_Alter = "alter";
    private const string cmd_Manage = "manage";

    private const string key_Connection = "-connection:";
    private const string key_Script = "-script:";
    private const string key_Encoding = "-encoding:";
    private const string key_Assembly = "-assembly:";
    private const string key_Permission = "-permission:";
    private const string key_Owner = "-owner:";
    private const string key_Schema = "-schema:";
    private const string map_Script = "-map:";

    private const string key_Unchecked = "-unchecked";
    private const string key_Invisible = "-invisible";
    private const string key_Append = "-append";
    private const string key_Strong = "-strong";

    private const string key_EnableClr = "-enable-clr";
    private const string key_Trustworthy = "-trustworthy";

    static void Main(string[] args)
    {
      try
      {
        if (args.Length == 0)
          Console.Error.Write(help_text);
        else if (args.Length < 2)
          throw new ApplicationException("To small arguments.");
        else
          Deploy(args);

        Environment.ExitCode = 0;
      }
      catch (Exception ex)
      {
        for (int i = 0; ex != null; i++)
        {
          Console.WriteLine(new string(' ', i << 1) + ex.Message);
          ex = ex.InnerException;
        }
        Environment.ExitCode = -1;
      }
    }

    private static void Deploy(string[] args)
    {
      DeployCommand deployCommand = ParseDeployCommand(args[0]);
      string scriptText = string.Empty;

      string connectionString = args
        .Skip(1)
        .Take(args.Length - 2)
        .FirstOrDefault(t => t.ToLower().StartsWith(key_Connection))
        .With(t => t == null ? null : t.Substring(key_Connection.Length));

      string scriptPath = args
        .Skip(1)
        .Take(args.Length - (deployCommand == DeployCommand.Manage ? 1 : 2))
        .FirstOrDefault(t => t.ToLower().StartsWith(key_Script))
        .With(t => t == null ? null : t.Substring(key_Script.Length));

      if (deployCommand == DeployCommand.Manage)
      {
        ManageParams manageParams = new ManageParams();

        manageParams.EnableClr = args
          .Skip(1)
          .Take(args.Length - 2)
          .Any(t => t.ToLower().Equals(key_EnableClr));

        manageParams.TrustworthyOn = args
          .Skip(1)
          .Take(args.Length - 2)
          .Any(t => t.ToLower().Equals(key_Trustworthy));

        scriptText = GetManageScript(manageParams);
      }
      else
      {
        DeployParams deployParams = new DeployParams();

        deployParams.PermissionSet = args
          .Skip(1)
          .Take(args.Length - 2)
          .FirstOrDefault(t => t.ToLower().StartsWith(key_Permission))
          .With(t => t == null ? default(PermissionSet?) : ParsePermissionSet(t.Substring(key_Permission.Length)));

        deployParams.Owner = args
          .Skip(1)
          .Take(args.Length - 2)
          .FirstOrDefault(t => t.ToLower().StartsWith(key_Owner))
          .With(t => t == null ? null : t.Substring(key_Owner.Length));

        deployParams.Schema = args
          .Skip(1)
          .Take(args.Length - 2)
          .FirstOrDefault(t => t.ToLower().StartsWith(key_Schema))
          .With(t => t == null ? null : t.Substring(key_Schema.Length));

        deployParams.UncheckedData = args
          .Skip(1)
          .Take(args.Length - 2)
          .Any(t => t.ToLower().Equals(key_Unchecked));

        deployParams.Assembly = args
          .Skip(1)
          .Take(args.Length - 2)
          .FirstOrDefault(t => t.ToLower().StartsWith(key_Assembly))
          .With(t => t == null ? null : t.Substring(key_Assembly.Length));

        deployParams.Reference = args
          .Skip(1)
          .Take(args.Length - 2)
          .Any(t => t.ToLower().Equals(key_Invisible));

        deployParams.Strong = args
          .Skip(1)
          .Take(args.Length - 2)
          .Any(t => t.ToLower().Equals(key_Strong));

        string mapPath = args
          .Skip(1)
          .Take(args.Length - 2)
          .FirstOrDefault(t => t.ToLower().StartsWith(map_Script))
          .With(t => t == null ? null : t.Substring(map_Script.Length));

        deployParams.Map = string.IsNullOrEmpty(mapPath) ? new Dictionary<string, string>() :
          File.ReadAllLines(mapPath)
            .Select(s => s.Split(';'))
            .Where(t => t.Length == 2 && !string.IsNullOrWhiteSpace(t[0]) && !string.IsNullOrWhiteSpace(t[1]))
            .ToDictionary(t => t[0], t => t[1]);

        Assembly assembly = deployParams.Strong ? Assembly.Load(args[args.Length - 1]) : Assembly.LoadFrom(args[args.Length - 1]);

        switch (deployCommand)
        {
          case DeployCommand.Register:
            scriptText = GetCreateScript(assembly, deployParams);
            break;
          case DeployCommand.Unregister:
            scriptText = GetDropScript(assembly, deployParams);
            break;
          case DeployCommand.Modify:
            scriptText = GetAlterScript(assembly, deployParams);
            break;
        }
      }

      if (!string.IsNullOrWhiteSpace(scriptPath))
      {
        bool append = args
          .Skip(1)
          .Take(args.Length - 2)
          .Any(t => t.ToLower().Equals(key_Append));

        string encoding = args
          .Skip(1)
          .Take(args.Length - 2)
          .FirstOrDefault(t => t.ToLower().StartsWith(key_Encoding))
          .With(t => t == null ? null : t.Substring(key_Encoding.Length));

        SaveScript(scriptPath, scriptText, encoding, append);
      }

      if (!string.IsNullOrWhiteSpace(connectionString))
        ExecuteScript(connectionString, scriptText);
    }

    private static DeployCommand ParseDeployCommand(string deployCommand)
    {
      if (string.IsNullOrWhiteSpace(deployCommand))
        throw new ApplicationException("Deploy command is not specified.");

      switch (deployCommand.ToLower())
      {
        case cmd_Manage:
          return DeployCommand.Manage;
        case cmd_Create:
          return DeployCommand.Register;
        case cmd_Drop:
          return DeployCommand.Unregister;
        case cmd_Alter:
          return DeployCommand.Modify;
        default:
          throw new ApplicationException("Deploy command is invalid.");
      }
    }

    private static PermissionSet ParsePermissionSet(string permissionSet)
    {
      if (string.IsNullOrWhiteSpace(permissionSet))
        throw new ApplicationException("Permission set parameter is not specified.");

      switch (permissionSet.ToLower())
      {
        case permission_Safe:
          return PermissionSet.Safe;
        case permission_ExternalAccess:
          return PermissionSet.ExternalAccess;
        case permission_Unsafe:
          return PermissionSet.Unsafe;
        default:
          throw new ApplicationException("Permission set parameter is invalid.");
      }
    }

    private static string GetDropScript(Assembly assembly, DeployParams deployParams)
    {
      StringBuilder sb = new StringBuilder();

      if (!deployParams.Reference)
      {
        foreach (Type type in PwrSqlClrObjectsExplorer.GetTypes(assembly))
        {
          foreach (MethodInfo miTrigger in PwrSqlClrObjectsExplorer.GetTriggers(type))
            sb.AppendLine(PwrSqlClrCommandBuilder.DropTrigger_CommandText(miTrigger, deployParams.Schema));

          foreach (MethodInfo miProcedure in PwrSqlClrObjectsExplorer.GetProcedures(type))
            sb.AppendLine(PwrSqlClrCommandBuilder.DropProcedure_CommandText(miProcedure, deployParams.Schema));

          foreach (MethodInfo miFunction in PwrSqlClrObjectsExplorer.GetFunctions(type))
            sb.AppendLine(PwrSqlClrCommandBuilder.DropFunction_CommandText(miFunction, deployParams.Schema));
        }

        foreach (Type userDefinedAggregate in PwrSqlClrObjectsExplorer.GetUserDefinedAggregates(assembly))
          sb.AppendLine(PwrSqlClrCommandBuilder.DropAggregate_CommandText(userDefinedAggregate, deployParams.Schema));

        foreach (Type userDefinedType in PwrSqlClrObjectsExplorer.GetUserDefinedTypes(assembly))
          sb.AppendLine(PwrSqlClrCommandBuilder.DropType_CommandText(userDefinedType, deployParams.Schema));
      }

      sb.AppendLine(PwrSqlClrCommandBuilder.DropAssembly_CommandText(assembly, deployParams.Assembly));

      return sb.ToString();
    }

    private static string GetCreateScript(Assembly assembly, DeployParams deployParams)
    {
      StringBuilder sb = new StringBuilder();

      sb.AppendLine(PwrSqlClrCommandBuilder.CreateAssembly_CommandText(assembly, deployParams.PermissionSet, deployParams.Assembly, deployParams.Owner));

      if (!deployParams.Reference)
      {
        foreach (Type userDefinedType in PwrSqlClrObjectsExplorer.GetUserDefinedTypes(assembly))
          sb.AppendLine(PwrSqlClrCommandBuilder.CreateType_CommandText(userDefinedType, deployParams.Schema, deployParams.Map));

        foreach (Type userDefinedAggregate in PwrSqlClrObjectsExplorer.GetUserDefinedAggregates(assembly))
          sb.AppendLine(PwrSqlClrCommandBuilder.CreateAggregate_CommandText(userDefinedAggregate, deployParams.Schema, deployParams.Map));

        foreach (Type type in PwrSqlClrObjectsExplorer.GetTypes(assembly))
        {
          foreach (MethodInfo miFunction in PwrSqlClrObjectsExplorer.GetFunctions(type))
            sb.AppendLine(PwrSqlClrCommandBuilder.CreateFunction_CommandText(miFunction, deployParams.Schema, deployParams.Map));

          foreach (MethodInfo miProcedure in PwrSqlClrObjectsExplorer.GetProcedures(type))
            sb.AppendLine(PwrSqlClrCommandBuilder.CreateProcedure_CommandText(miProcedure, deployParams.Schema, deployParams.Map));

          foreach (MethodInfo miTrigger in PwrSqlClrObjectsExplorer.GetTriggers(type))
            sb.AppendLine(PwrSqlClrCommandBuilder.CreateTrigger_CommandText(miTrigger, deployParams.Schema, deployParams.Map));
        }
      }
      return sb.ToString();
    }

    private static string GetAlterScript(Assembly assembly, DeployParams deployParams)
    {
      StringBuilder sb = new StringBuilder();

      sb.AppendLine(PwrSqlClrCommandBuilder.AlterAssembly_CommandText(assembly, deployParams.PermissionSet, !deployParams.Reference, deployParams.UncheckedData, deployParams.Assembly));

      return sb.ToString();
    }

    private static string GetManageScript(ManageParams manageParams)
    {
      StringBuilder sb = new StringBuilder();

      if (manageParams.EnableClr)
        sb.AppendLine(PwrSqlClrCommandBuilder.EnableClr_CommandText());

      if (manageParams.TrustworthyOn)
        sb.AppendLine(PwrSqlClrCommandBuilder.TrustworthyOn_CommandText());

      return sb.ToString();
    }

    private static void SaveScript(string path, string commandText, string encodingParam, bool append)
    {
      if (encodingParam == null)
      {
        if (append)
          File.AppendAllText(path, commandText);
        else
          File.WriteAllText(path, commandText);
      }
      else
      {
        int codepageId;
        Encoding encoding = int.TryParse(encodingParam, out codepageId) ? Encoding.GetEncoding(codepageId) : Encoding.GetEncoding(encodingParam);
        if (append)
          File.AppendAllText(path, commandText, encoding);
        else
          File.WriteAllText(path, commandText, encoding);
      }
    }

    private static void ExecuteScript(string connectionString, string commandText)
    {
      using (SqlConnection con = new SqlConnection(connectionString))
      {
        con.Open();
        Server server = new Server(new ServerConnection(con));
        server.ConnectionContext.BeginTransaction();
        try
        {
          server.ConnectionContext.ExecuteNonQuery(commandText);
          server.ConnectionContext.CommitTransaction();
        }
        catch (Exception)
        {
          server.ConnectionContext.RollBackTransaction();
          throw;
        }
      }
    }
  }

  internal enum DeployCommand
  {
    Manage,
    Register,
    Unregister,
    Modify,
  }

  internal class ManageParams
  {
    public bool EnableClr { get; set; }

    public bool TrustworthyOn { get; set; }
  }

  internal class DeployParams
  {
    public string Assembly { get; set; }

    public string Owner { get; set; }

    public string Schema { get; set; }
     
    public Dictionary<string, string> Map { get; set; }

    public PermissionSet? PermissionSet { get; set; }

    public bool UncheckedData { get; set; }

    public bool Reference { get; set; }

    public bool Strong { get; set; }
  }
}
