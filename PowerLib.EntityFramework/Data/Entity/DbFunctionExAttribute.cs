using System;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;

namespace PowerLib.System.Data.Entity
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class DbFunctionExAttribute : DbFunctionAttribute
  {
    public DbFunctionExAttribute(string namespaceName, string functionName)
      : base(namespaceName, functionName)
    {
    }

    public bool IsComposable { get; set; }

    public bool IsBuiltIn { get; set; }

    public bool IsAggregate { get; set; }

    public bool IsNiladic { get; set; }

    public ParameterTypeSemantics ParameterTypeSemantics { get; set; }

    public string Schema { get; set; }
  }
}
