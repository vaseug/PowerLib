using System;
using System.Data.Entity;

namespace PowerLib.System.Data.Entity
{
  public static class DbModelBuilderExtension
  {
    public static FunctionConventionConfiguration Functions(this DbModelBuilder modelBuilder)
    {
      return new FunctionConventionConfiguration(modelBuilder);
    }
  }
}
