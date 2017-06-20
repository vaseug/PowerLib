using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
using System.Reflection;
using System.Xml;
using Microsoft.SqlServer.Server;
using PowerLib.System.Linq;

namespace PowerLib.SqlClr.Deploy
{
  public static class PwrSqlClrObjectsExplorer
  {
    public static IEnumerable<Type> GetTypes(Assembly assm)
    {
      return assm.DefinedTypes
        .Where(t => t.IsPublic && !t.IsDefined(typeof(SqlUserDefinedTypeAttribute)) && !t.IsDefined(typeof(SqlUserDefinedAggregateAttribute)));
    }

    public static IEnumerable<Type> GetUserDefinedTypes(Assembly assm)
    {
      return assm.DefinedTypes
        .Where(t => t.IsPublic && !(t.IsAbstract && t.IsSealed) && t.IsDefined(typeof(SqlUserDefinedTypeAttribute)) &&
          t.GetInterfaces().Any(f => f == typeof(INullable)) && (t.IsValueType || t.IsClass && t.GetConstructor(Type.EmptyTypes) != null));
    }

    public static IEnumerable<Type> GetUserDefinedAggregates(Assembly assm)
    {
      return assm.DefinedTypes
        .Where(t => t.IsPublic && !(t.IsAbstract && t.IsSealed) && t.IsDefined(typeof(SqlUserDefinedAggregateAttribute)) &&
          t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Count(m =>
              m.Name == "Init" && m.GetParameters().Length == 0 && m.ReturnType == typeof(void) ||
              m.Name == "Accumulate" && m.GetParameters().Length >= 1 && m.ReturnType == typeof(void) ||
              m.Name == "Merge" && m.GetParameters().With(p => p.Length == 1 && p[0].ParameterType == t) && m.ReturnType == typeof(void) ||
              m.Name == "Terminate" && m.GetParameters().Length == 0) == 4);
    }

    public static IEnumerable<MethodInfo> GetProcedures(Type type)
    {
      return type
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .Where(t => t.IsDefined(typeof(SqlProcedureAttribute)));
    }

    public static IEnumerable<MethodInfo> GetFunctions(Type type)
    {
      return type
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .Where(t => t.IsDefined(typeof(SqlFunctionAttribute)));
    }

    public static IEnumerable<MethodInfo> GetTriggers(Type type)
    {
      return type
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .Where(t => t.IsDefined(typeof(SqlTriggerAttribute)));
    }
  }
}
