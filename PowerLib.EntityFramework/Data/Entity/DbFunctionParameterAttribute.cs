using System;

namespace PowerLib.System.Data.Entity
{
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
  public class DbFunctionParameterAttribute : Attribute
  {
    public DbFunctionParameterAttribute()
      : this(null)
    {
    }

    public DbFunctionParameterAttribute(Type type)
    {
      Type = type;
    }

    public Type Type { get; private set; }

    public string ColumnName { get; set; }

    public string DbTypeName { get; set; }
  }
}
