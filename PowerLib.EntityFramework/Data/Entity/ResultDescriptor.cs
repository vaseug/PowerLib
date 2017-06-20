using System;

namespace PowerLib.System.Data.Entity
{
  public class ResultDescriptor : SimpleTypeDescriptor
  {
    internal ResultDescriptor(Type type)
      : base(type)
    {
    }

    public string ColumnName { get; set; }
  }
}
