using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace PowerLib.System.Data.Entity
{
  public class FunctionConventionConfiguration
  {
    private DbModelBuilder _modelBuilder;

    internal FunctionConventionConfiguration(DbModelBuilder modelBuilder)
    {
      if (modelBuilder == null)
        throw new ArgumentNullException("modelBuilder");

      _modelBuilder = modelBuilder;
    }

    public FunctionConventionConfiguration ImportFunction<T>(Func<MethodInfo, bool> predicate, Action<FunctionDescriptor> convention)
    {
      MethodInfo mi = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.InvokeMethod).SingleOrDefault(predicate);
      if (mi == null)
        throw new InvalidOperationException("Specified method not found");

      //_modelBuilder.Conventions.Add(new FunctionConvention(mi, ));
      return this;
    }
  }
}
