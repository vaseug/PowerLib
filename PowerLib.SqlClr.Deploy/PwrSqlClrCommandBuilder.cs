using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
using System.Reflection;
using Microsoft.SqlServer.Server;
using PowerLib.System.ComponentModel.DataAnnotations;

namespace PowerLib.SqlClr.Deploy
{
  public static class PwrSqlClrCommandBuilder
  {
    private static Dictionary<Type, SqlTypeInfo> typesMap = null;

    private static SqlTypeInfo[] typesList = new[]
    {
      new SqlTypeInfo(typeof(SqlBoolean), "bit", null, SqlTypeFacet.None, null, null, null),
      new SqlTypeInfo(typeof(SqlByte), "tinyint", null, SqlTypeFacet.None, null, null, null),
      new SqlTypeInfo(typeof(SqlInt16), "smallint", null, SqlTypeFacet.None, null, null, null),
      new SqlTypeInfo(typeof(SqlInt32), "int", null, SqlTypeFacet.None, null, null, null),
      new SqlTypeInfo(typeof(SqlInt64), "bigint", null, SqlTypeFacet.None, null, null, null),
      new SqlTypeInfo(typeof(SqlSingle), "real", null, SqlTypeFacet.None, null, null, null),
      new SqlTypeInfo(typeof(SqlDouble), "float", null, SqlTypeFacet.None, null, null, null),
      new SqlTypeInfo(typeof(SqlMoney), "money", null, SqlTypeFacet.None, null, null, null),
      new SqlTypeInfo(typeof(SqlDecimal), "decimal", null, SqlTypeFacet.PrecisionScale, null, null, null),
      new SqlTypeInfo(typeof(SqlDateTime), "datetime", null, SqlTypeFacet.None, null, null, null),
      new SqlTypeInfo(typeof(SqlGuid), "uniqueidentifier", null, SqlTypeFacet.None, null, null, null),
      new SqlTypeInfo(typeof(SqlXml), "xml", null, SqlTypeFacet.None, null, null, null),
      new SqlTypeInfo(typeof(SqlChars), "nvarchar", "nchar", SqlTypeFacet.Size, -1, null, null),
      new SqlTypeInfo(typeof(SqlString), "nvarchar", "nchar", SqlTypeFacet.Size, 4000, null, null),
      new SqlTypeInfo(typeof(SqlBytes), "varbinary", "binary", SqlTypeFacet.Size, -1, null, null),
      new SqlTypeInfo(typeof(SqlBinary), "varbinary", "binary", SqlTypeFacet.Size, 8000, null, null),
    };

    static PwrSqlClrCommandBuilder()
    {
      typesMap = typesList.ToDictionary(t => t.FwType, t => t);
    }

    private static string GetPermissionSet_Text(PermissionSet permissionSet)
    {
      switch (permissionSet)
      {
        case PermissionSet.Safe:
          return "SAFE";
        case PermissionSet.ExternalAccess:
          return "EXTERNAL_ACCESS";
        case PermissionSet.Unsafe:
          return "UNSAFE";
        default:
          throw new ArgumentException("Invalid permission set value.", "permissionSet");
      }
    }

    public static string GetSqlType_Text(Type type, ICustomAttributeProvider provider, IDictionary<string, string> map)
    {
      StringBuilder sb = new StringBuilder();
      SqlTypeInfo sqlTypeInfo;
      if (type.IsByRef)
        type = type.GetElementType();
      type = Nullable.GetUnderlyingType(type) ?? type;
      if (type.IsEnum)
        type = Enum.GetUnderlyingType(type);

      if (typesMap.TryGetValue(type, out sqlTypeInfo))
      {
        FunctionParameterAttribute paramAttr = provider != null ? (FunctionParameterAttribute)provider.GetCustomAttributes(typeof(FunctionParameterAttribute), false).FirstOrDefault() : null;
        SqlFacetAttribute facetAttr = provider != null ? (SqlFacetAttribute)provider.GetCustomAttributes(typeof(SqlFacetAttribute), false).FirstOrDefault() : null;
        sb.Append(paramAttr != null && !string.IsNullOrEmpty(paramAttr.TypeName) ? paramAttr.TypeName : facetAttr != null && facetAttr.IsFixedLength ? sqlTypeInfo.TypeNameFixed : sqlTypeInfo.TypeName);
        //  Append type facets
        switch (sqlTypeInfo.Facet)
        {
          case SqlTypeFacet.Size:
            if (facetAttr != null && (facetAttr.MaxSize > 0 || facetAttr.MaxSize == -1))
              sb.AppendFormat(CultureInfo.InvariantCulture, "({0})", facetAttr.MaxSize == -1 ? (object)"max" : facetAttr.MaxSize);
            else if (sqlTypeInfo.MaxSize.HasValue)
              sb.AppendFormat(CultureInfo.InvariantCulture, "({0})", sqlTypeInfo.MaxSize == -1 ? (object)"max" : sqlTypeInfo.MaxSize);
            break;
          case SqlTypeFacet.PrecisionScale:
            if (facetAttr != null && facetAttr.Precision > 0)
              if (facetAttr.Scale > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "({0}, {1})", facetAttr.Precision, facetAttr.Scale);
              else
                sb.AppendFormat(CultureInfo.InvariantCulture, "({0})", facetAttr.Precision);
            else if (sqlTypeInfo.Precision.HasValue && sqlTypeInfo.Precision.Value > 0)
              if (sqlTypeInfo.Scale.HasValue && sqlTypeInfo.Scale.Value > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "({0}, {1})", sqlTypeInfo.Precision.Value, sqlTypeInfo.Scale.Value);
              else
                sb.AppendFormat(CultureInfo.InvariantCulture, "({0})", sqlTypeInfo.Precision.Value);
            break;
        }
      }
      else if (type.IsDefined(typeof(SqlUserDefinedTypeAttribute)))
      {
        SqlUserDefinedTypeAttribute udt = type.GetCustomAttribute<SqlUserDefinedTypeAttribute>();
        string schema;
        if (!map.TryGetValue(type.FullName, out schema))
          schema = "dbo";
        sb.AppendFormat("[{0}].[{1}]", schema, !string.IsNullOrEmpty(udt.Name) ? udt.Name : type.Name);
      }
      else
        throw new ArgumentException("Framework type is not mapped to sql type");
      return sb.ToString();
    }

    public static string GetSqlParameterResult_Text(ParameterInfo param, IDictionary<string, string> map)
    {
      StringBuilder sb = new StringBuilder();
      FunctionParameterAttribute paramAttr = param.GetCustomAttribute<FunctionParameterAttribute>();
      //  Append result parameter name with type
      sb.AppendFormat("[{0}] {1}", paramAttr != null && !string.IsNullOrEmpty(paramAttr.Name) ? paramAttr.Name : param.Name, GetSqlType_Text(param.ParameterType, param, map));
      return sb.ToString();
    }

    public static string GetSqlParameterArgument_Text(ParameterInfo param, IDictionary<string, string> map)
    {
      StringBuilder sb = new StringBuilder();
      FunctionParameterAttribute paramAttr = param.GetCustomAttribute<FunctionParameterAttribute>();
      //  Append parameter name with type
      sb.AppendFormat("@{0} {1}", paramAttr != null && !string.IsNullOrEmpty(paramAttr.Name) ? paramAttr.Name : param.Name, GetSqlType_Text(param.ParameterType, param, map));
      //  Append default value
      DefaultValueAttribute defaultValueAttr = param.GetCustomAttribute<DefaultValueAttribute>();
      if (defaultValueAttr != null)
        sb.AppendFormat(CultureInfo.InvariantCulture, " = {0}", defaultValueAttr.Value == null ? (object)"NULL" : 
          defaultValueAttr.Value.GetType().IsEnum ? ((Enum)defaultValueAttr.Value).ToString("D") : defaultValueAttr.Value);
      //  Append OUT parameter
      if (param.IsOut)
        sb.Append(" OUT");
      return sb.ToString();
    }

    public static string DropAssembly_CommandText(Assembly assm, string name = null)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat(CultureInfo.InvariantCulture, @"
IF EXISTS (
  SELECT *
    FROM sys.assemblies
    WHERE [name] = N'{0}')
  DROP ASSEMBLY [{0}];
GO", assm.GetName().Name);
      return sb.ToString();
    }

    public static string CreateAssembly_CommandText(Assembly assm, PermissionSet? permissionSet, string name = null, string owner = null)
    {
      StringBuilder sb = new StringBuilder();
      if (name == null)
        name = assm.GetName().Name;
      sb.AppendFormat(CultureInfo.InvariantCulture, @"
CREATE ASSEMBLY [{0}]", name);
      if (owner != null)
        sb.AppendFormat(CultureInfo.InvariantCulture, @"
  AUTHORIZATION [{0}]", owner);
      sb.AppendFormat(CultureInfo.InvariantCulture, @"
  FROM '{0}'", assm.Location);
      if (permissionSet.HasValue)
        sb.AppendFormat(CultureInfo.InvariantCulture, @"
  WITH PERMISSION_SET = {0}", GetPermissionSet_Text(permissionSet.Value));
      sb.Append(@";
GO");
      return sb.ToString();
    }

    public static string AlterAssembly_CommandText(Assembly assm, PermissionSet? permissionSet, bool visibility, bool uncheckedData, string name = null)
    {
      StringBuilder sb = new StringBuilder();
      if (name == null)
        name = assm.GetName().Name;
      sb.AppendFormat(CultureInfo.InvariantCulture, @"
ALTER ASSEMBLY [{0}]", name);
      sb.AppendFormat(CultureInfo.InvariantCulture, @"
  FROM '{0}'", assm.Location);
      sb.AppendFormat(CultureInfo.InvariantCulture, @"
  WITH VISIBILITY = {0}", visibility ? "ON" : "OFF");
      if (permissionSet.HasValue)
        sb.AppendFormat(@", PERMISSION_SET = {0}", GetPermissionSet_Text(permissionSet.Value));
      if (uncheckedData)
        sb.Append(@", UNCHECKED DATA");
      sb.Append(@";
GO");
      return sb.ToString();
    }

    public static string DropType_CommandText(Type type, string schema)
    {
      StringBuilder sb = new StringBuilder();

      SqlUserDefinedTypeAttribute typeAttr = type.GetCustomAttribute<SqlUserDefinedTypeAttribute>();
      if (typeAttr == null)
        throw new InvalidOperationException(string.Format("Type '{0}' is not sql clr user defined type.", type.FullName));
      string name = typeAttr.Name ?? type.Name;

      sb.AppendFormat(CultureInfo.InvariantCulture, @"
IF EXISTS (
  SELECT *
    FROM sys.types
    WHERE [name] = N'{0}' AND [is_assembly_type] = 1 AND [is_user_defined] = 1", name);
      if (!string.IsNullOrEmpty(schema))
        sb.AppendFormat(CultureInfo.InvariantCulture, @" AND EXISTS (
      SELECT *
        FROM sys.schemas
        WHERE sys.types.schema_id = sys.schemas.schema_id AND sys.schemas.name = N'{0}')", schema);
      sb.Append(@")
  DROP TYPE ");
      if (!string.IsNullOrEmpty(schema))
        sb.AppendFormat(CultureInfo.InvariantCulture, @"[{0}].[{1}]", schema, name);
      else
        sb.AppendFormat(CultureInfo.InvariantCulture, @"[{0}]", name);
      sb.Append(@";
GO");
      return sb.ToString();
    }

    public static string CreateType_CommandText(Type type, string schema, IDictionary<string, string> map)
    {
      StringBuilder sb = new StringBuilder();

      SqlUserDefinedTypeAttribute typeAttr = type.GetCustomAttribute<SqlUserDefinedTypeAttribute>();
      if (typeAttr == null)
        throw new ArgumentException(string.Format("Type '{0}' is not sql clr user defined type.", type.FullName));
      string name = typeAttr.Name ?? type.Name;

      sb.Append(@"
CREATE TYPE ");
      if (schema != null)
      {
        sb.AppendFormat(CultureInfo.InvariantCulture, @"[{0}].[{1}]", schema, name);
        map.Add(type.FullName, schema);
      }
      else
        sb.AppendFormat(CultureInfo.InvariantCulture, @"[{0}]", name);
      sb.AppendFormat(@"
  EXTERNAL NAME [{0}].[{1}];
GO", type.Assembly.GetName().Name, type.FullName);
      return sb.ToString();
    }

    public static string DropAggregate_CommandText(Type type, string schema)
    {
      StringBuilder sb = new StringBuilder();

      SqlUserDefinedAggregateAttribute aggregateAttr = type.GetCustomAttribute<SqlUserDefinedAggregateAttribute>();
      if (aggregateAttr == null)
        throw new InvalidOperationException(string.Format("Type '{0}' is not sql clr user defined aggregate.", type.FullName));
      string name = aggregateAttr.Name ?? type.Name;

      sb.AppendFormat(@"
IF EXISTS (
  SELECT *
    FROM sys.objects
    WHERE [name] = N'{0}' AND [type] = 'AF'", name);
      if (schema != null)
        sb.AppendFormat(@" AND EXISTS (
      SELECT *
        FROM sys.schemas
        WHERE sys.objects.schema_id = sys.schemas.schema_id AND sys.schemas.name = N'{0}')", schema);
      sb.Append(@")
  DROP AGGREGATE ");
      if (schema != null)
        sb.AppendFormat(@"[{0}].[{1}]", schema, name);
      else
        sb.AppendFormat(@"[{0}]", name);
      sb.Append(@";
GO");
      return sb.ToString();
    }

    public static string CreateAggregate_CommandText(Type type, string schema, IDictionary<string, string> map)
    {
      StringBuilder sb = new StringBuilder();

      SqlUserDefinedAggregateAttribute aggregateAttr = type.GetCustomAttribute<SqlUserDefinedAggregateAttribute>();
      if (aggregateAttr == null)
        throw new ArgumentException(string.Format("Type '{0}' is not sql clr user defined aggregate.", type.FullName));
      string name = aggregateAttr.Name ?? type.Name;
      MethodInfo miAccumulate = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
        .SingleOrDefault(t => t.Name == "Accumulate" && t.ReturnType == typeof(void));
      if (miAccumulate == null)
        throw new InvalidOperationException("Accumulate method is not found");
      MethodInfo miTerminate = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
        .SingleOrDefault(t => t.Name == "Terminate" && t.GetParameters().Length == 0);
      if (miTerminate == null)
        throw new InvalidOperationException("Terminate method is not found");

      sb.Append(@"
CREATE AGGREGATE ");
      if (schema != null)
        sb.AppendFormat(@"[{0}].[{1}]", schema, name);
      else
        sb.AppendFormat(@"[{0}]", name);
      sb.AppendFormat(@"({0})
  RETURNS {1}
  EXTERNAL NAME [{2}].[{3}];
GO", string.Join(", ", miAccumulate.GetParameters().Select(pi => GetSqlParameterArgument_Text(pi, map))),
      GetSqlType_Text(miTerminate.ReturnParameter.ParameterType, miTerminate.ReturnParameter, map), type.Assembly.GetName().Name, type.FullName);
      return sb.ToString();
    }

    public static string DropFunction_CommandText(MethodInfo method, string schema)
    {
      StringBuilder sb = new StringBuilder();

      SqlFunctionAttribute functionAttr = method.GetCustomAttribute<SqlFunctionAttribute>();
      if (functionAttr == null)
        throw new InvalidOperationException(string.Format("Method '{0}' is not sql clr function.", method.Name));
      string name = functionAttr.Name ?? method.Name;
      bool tableValue= method.ReturnType == typeof(IEnumerable) && !string.IsNullOrWhiteSpace(functionAttr.FillRowMethodName);

      sb.AppendFormat(@"
IF EXISTS (
  SELECT *
    FROM sys.objects
    WHERE [name] = N'{0}' AND [type] = '{1}'", name, tableValue? "FT" : "FS");
      if (schema != null)
        sb.AppendFormat(@" AND EXISTS (
      SELECT *
        FROM sys.schemas
        WHERE sys.objects.schema_id = sys.schemas.schema_id AND sys.schemas.name = N'{0}')", schema);
      sb.Append(@")
  DROP FUNCTION ");
      if (schema != null)
        sb.AppendFormat(@"[{0}].[{1}]", schema, name);
      else
        sb.AppendFormat(@"[{0}]", name);
      sb.Append(@";
GO");
      return sb.ToString();
    }

    public static string CreateFunction_CommandText(MethodInfo method, string schema, IDictionary<string, string> map)
    {
      StringBuilder sb = new StringBuilder();
      SqlFunctionAttribute functionAttr = method.GetCustomAttribute<SqlFunctionAttribute>();
      if (functionAttr == null)
        throw new InvalidOperationException(string.Format("Method '{0}' is not sql cll function.", method.Name));
      string name = functionAttr.Name ?? method.Name;
      bool tableValued = method.ReturnType == typeof(IEnumerable) && !string.IsNullOrWhiteSpace(functionAttr.FillRowMethodName);
      MethodInfo miFillRow = !tableValued ? null : method.DeclaringType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
        .SingleOrDefault(m => m.Name == functionAttr.FillRowMethodName && m.ReturnType == typeof(void));
      if (miFillRow == null && tableValued)
        throw new InvalidOperationException(string.Format("FillRow method '{0}' is not found.", functionAttr.FillRowMethodName));

      sb.AppendFormat(@"
CREATE FUNCTION ");
      if (schema != null)
        sb.AppendFormat(@"[{0}].[{1}]", schema, name);
      else
        sb.AppendFormat(@"[{0}]", name);
      sb.AppendFormat(@"({0})
  RETURNS ", string.Join(", ", method.GetParameters().Select(pi => GetSqlParameterArgument_Text(pi, map))));
      if (tableValued)
        sb.AppendFormat(@"TABLE ({0})", functionAttr.TableDefinition ?? string.Join(", ", miFillRow.GetParameters().Skip(1).Select(pi => GetSqlParameterResult_Text(pi, map))));
      else
        sb.AppendFormat(@"{0}", GetSqlType_Text(method.ReturnParameter.ParameterType, method.ReturnParameter, map));
      sb.AppendFormat(@"
  EXTERNAL NAME [{0}].[{1}].[{2}];
GO", method.DeclaringType.Assembly.GetName().Name, method.DeclaringType.FullName, method.Name);
      return sb.ToString();
    }

    public static string DropProcedure_CommandText(MethodInfo method, string schema)
    {
      StringBuilder sb = new StringBuilder();

      SqlProcedureAttribute procedureAttr = method.GetCustomAttribute<SqlProcedureAttribute>();
      if (procedureAttr == null)
        throw new InvalidOperationException(string.Format("Method '{0}' is not sql clr procedure.", method.Name));
      string name = procedureAttr.Name ?? method.Name;

      sb.AppendFormat(@"
IF EXISTS (
  SELECT *
    FROM sys.objects
    WHERE [name] = N'{0}' AND [type] = 'PC'", name);
      if (schema != null)
        sb.AppendFormat(@" AND EXISTS (
      SELECT *
        FROM sys.schemas
        WHERE sys.objects.schema_id = sys.schemas.schema_id AND sys.schemas.name = N'{0}')", schema);
      sb.Append(@")
  DROP PROCEDURE ");
      if (schema != null)
        sb.AppendFormat(@"[{0}].[{1}]", schema, name);
      else
        sb.AppendFormat(@"[{0}]", name);
      sb.Append(@";
GO");
      return sb.ToString();
    }

    public static string CreateProcedure_CommandText(MethodInfo method, string schema, IDictionary<string, string> map)
    {
      StringBuilder sb = new StringBuilder();

      SqlProcedureAttribute procedureAttr = method.GetCustomAttribute<SqlProcedureAttribute>();
      if (procedureAttr == null)
        throw new InvalidOperationException(string.Format("Method '{0}' is not sql clr procedure.", method.Name));
      string name = procedureAttr.Name ?? method.Name;

      sb.AppendFormat(@"
CREATE PROCEDURE ");
      if (schema != null)
        sb.AppendFormat(@"[{0}].[{1}]", schema, name);
      else
        sb.AppendFormat(@"[{0}]", name);
      sb.AppendFormat(@"({0})", string.Join(", ", method.GetParameters().Select(pi => GetSqlParameterArgument_Text(pi, map))));
      sb.AppendFormat(@"
AS
  EXTERNAL NAME [{0}].[{1}].[{2}];
GO", method.DeclaringType.Assembly.GetName().Name, method.DeclaringType.FullName, method.Name);
      return sb.ToString();
    }

    public static string DropTrigger_CommandText(MethodInfo method, string schema)
    {
      StringBuilder sb = new StringBuilder();

      SqlTriggerAttribute triggerAttr = method.GetCustomAttribute<SqlTriggerAttribute>();
      if (triggerAttr == null)
        throw new InvalidOperationException(string.Format("Method '{0}' is not sql clr trigger.", method.Name));
      string name = triggerAttr.Name ?? method.Name;

      sb.AppendFormat(@"
IF EXISTS (
  SELECT *
    FROM sys.objects
    WHERE [name] = N'{0}' AND [type] = 'TA'", name);
      if (schema != null)
        sb.AppendFormat(@" AND EXISTS (
      SELECT *
        FROM sys.schemas
        WHERE sys.objects.schema_id = sys.schemas.schema_id AND sys.schemas.name = N'{0}')", schema);
      sb.Append(@")
  DROP TRIGGER ");
      if (schema != null)
        sb.AppendFormat(@"[{0}].[{1}]", schema, name);
      else
        sb.AppendFormat(@"[{0}]", name);
      sb.Append(@";
GO");
      return sb.ToString();
    }

    public static string CreateTrigger_CommandText(MethodInfo method, string schema, IDictionary<string, string> map)
    {
      StringBuilder sb = new StringBuilder();

      SqlTriggerAttribute triggerAttr = method.GetCustomAttribute<SqlTriggerAttribute>();
      if (triggerAttr == null)
        throw new InvalidOperationException(string.Format("Method '{0}' is not sql clr trigger.", method.Name));
      string name = triggerAttr.Name ?? method.Name;

      sb.AppendFormat(@"
CREATE TRIGGER ");
      if (schema != null)
        sb.AppendFormat(@"[{0}].[{1}]", schema, name);
      else
        sb.AppendFormat(@"[{0}]", name);
      sb.AppendFormat(@"
ON {0}
{1}
", triggerAttr.Target, triggerAttr.Event);
      sb.AppendFormat(@"
  EXTERNAL NAME [{0}].[{1}].[{2}];
GO", method.DeclaringType.Assembly.GetName().Name, method.DeclaringType.FullName, method.Name);
      return sb.ToString();
    }

    public static string EnableClr_CommandText()
    {
      return @"
IF NOT EXISTS (
  SELECT *
    FROM sys.configurations
    WHERE [name] = 'clr enabled' AND [value] = 1)
BEGIN
  EXEC sp_configure 'clr enabled', 1
  RECONFIGURE
END;
GO";
    }

    public static string TrustworthyOn_CommandText()
    {
      return @"
IF NOT EXISTS (
  SELECT *
    FROM sys.databases
    WHERE [name] = DB_NAME() AND [is_trustworthy_on] = 1)
  ALTER DATABASE CURRENT
    SET TRUSTWORTHY ON;
GO";
    }
  }

  public class SqlTypeInfo
  {
    public SqlTypeInfo(Type fwType, string typeName, string typeNameFixed, SqlTypeFacet facet, int? maxSize, byte? precision, byte? scale)
    {
      FwType = fwType;
      TypeName = typeName;
      TypeNameFixed = typeNameFixed;
      Facet = facet;
      MaxSize = maxSize;
      Precision = precision;
      Scale = scale;
    }

    public Type FwType { get; private set; }

    public string TypeName { get; private set; }

    public string TypeNameFixed { get; private set; }

    public SqlTypeFacet Facet { get; private set; }

    public int? MaxSize { get; private set; }

    public byte? Precision { get; private set; }

    public byte? Scale { get; private set; }
  }

  public enum SqlTypeFacet
  {
    None = 0,
    Size = 1,
    PrecisionScale = 2,
  }

  public enum PermissionSet
  {
    Safe = 0,
    ExternalAccess = 1,
    Unsafe = 2,
  }
}
