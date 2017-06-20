using System;

namespace PowerLib.System.Data.Entity
{
  public class SimpleTypeDescriptor
  {
    internal SimpleTypeDescriptor(Type type)
    {
      Type = type;
    }

    public Type Type { get; private set; }

    public string StoreTypeName { get; set; }

    public byte? Precision { get; set; }

    public byte? Scale { get; set; }

    public int? Length { get; set; }

    public bool? IsFixedLength { get; set; }
  }
}
