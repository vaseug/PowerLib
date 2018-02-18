using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace PowerLib.System.ComponentModel
{
  public abstract class TypeInitializer
  {
    public abstract void Initialize(object obj, NameValueCollection coll);

    public abstract void Retrieve(object obj, NameValueCollection coll);

    private static readonly Lazy<TypeInitializer> @default = new Lazy<TypeInitializer>(() => new DefaultTypeInitializer());

    public static TypeInitializer Default { get { return @default.Value; } }

    private class DefaultTypeInitializer : TypeInitializer
    {
      public override void Initialize(object obj, NameValueCollection coll)
      {
        if (obj == null)
          return;

        foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(obj))
        {
          if (prop.IsReadOnly)
            continue;
          if (coll.Keys.OfType<string>().Any(k => k == prop.Name))
            prop.SetValue(obj, prop.Converter.ConvertFromString(coll.Get(prop.Name)));
        }
      }

      public override void Retrieve(object obj, NameValueCollection coll)
      {
        if (obj == null)
          return;

        foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(obj))
        {
          if (coll.Keys.OfType<string>().Any(k => k == prop.Name))
            coll[prop.Name] = prop.Converter.ConvertToString(prop.GetValue(obj));
        }
      }
    }
  }
}
