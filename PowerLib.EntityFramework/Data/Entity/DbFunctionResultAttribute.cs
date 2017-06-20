using System;

namespace PowerLib.System.Data.Entity
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = true)]
  public class DbFunctionResultAttribute : Attribute
  {
    public DbFunctionResultAttribute() :
      this(null)
    {
    }

    public DbFunctionResultAttribute(Type type)
    {
      Type = type;
    }

    public Type Type { get; private set; }

    public string ColumnName { get; set; }

    public string DbTypeName { get; set; }
  }
}
