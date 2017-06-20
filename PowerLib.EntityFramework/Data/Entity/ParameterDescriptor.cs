using System;
using System.Data;

namespace PowerLib.System.Data.Entity
{
  public class ParameterDescriptor : SimpleTypeDescriptor
  {
    internal ParameterDescriptor(int position, ParameterDirection direction, Type type)
      : base(type)
    {
      Position = position;
      Direction = direction;
    }

    public int Position { get; private set; }

    public ParameterDirection Direction { get; private set; }

    public string Name { get; set; }
  }
}
