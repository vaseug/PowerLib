using System;

namespace PowerLib.System.ComponentModel.DataAnnotations
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = true)]
  public class FunctionResultAttribute : Attribute
  {
    public FunctionResultAttribute() :
      this(null)
    {
    }

    public FunctionResultAttribute(Type type)
    {
      Type = type;
    }

    public Type Type { get; private set; }

    public string ColumnName { get; set; }

    public string TypeName { get; set; }
  }
}
