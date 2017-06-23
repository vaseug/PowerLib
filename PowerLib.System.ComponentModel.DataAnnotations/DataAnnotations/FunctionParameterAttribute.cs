using System;

namespace PowerLib.System.ComponentModel.DataAnnotations
{
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
  public class FunctionParameterAttribute : Attribute
  {
    public FunctionParameterAttribute()
      : this(null)
    {
    }

    public FunctionParameterAttribute(Type type)
    {
      Type = type;
    }

    public Type Type { get; private set; }

    public string Name { get; set; }

    public string TypeName { get; set; }
  }
}
