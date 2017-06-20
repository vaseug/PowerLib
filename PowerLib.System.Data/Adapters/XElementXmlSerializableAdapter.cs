using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Data.SqlTypes;

namespace PowerLib.System.Data.Adapters
{
  public class XElementXmlSerializableAdapter<T> : NullableDataTypeAdapter<XElement, T>
    where T : IXmlSerializable, INullable
  {
    private static Func<T> nullFactory;
    private readonly Func<T> _typeFactory;

    public XElementXmlSerializableAdapter(XElement sval, Func<T> typeFactory)
      : this(sval, default(T), false, typeFactory)
    {
    }

    public XElementXmlSerializableAdapter(XElement sval, T vval, bool side, Func<T> typeFactory)
      : base(sval, vval, side)
    {
      if (typeFactory == null)
        throw new ArgumentNullException("typeFactory");

      _typeFactory = typeFactory;
    }

    public static Func<T> NullFactory
    {
      set
      {
        if (value == null)
          throw new ArgumentNullException(null);
        if (nullFactory != null)
          throw new InvalidOperationException(string.Format("Null factory is already initialized for type '{0}'.", typeof(T).FullName));

        nullFactory = value;
      }
    }

    protected override bool IsStoreUpdated(XElement sval)
    {
      return true;
    }

    protected override bool IsStoreNull(XElement sval)
    {
      return sval == null;
    }

    protected override XElement StoreNull
    {
      get
      {
        return null;
      }
    }

    protected override bool IsViewUpdated(T vval)
    {
      return true;
    }

    protected override bool IsViewNull(T vval)
    {
      return vval == null || vval.IsNull;
    }

    protected override sealed T ViewNull
    {
      get
      {
        if (nullFactory == null)
          throw new InvalidOperationException(string.Format("Null factory is not initialized for type '{0}'.", typeof(T).FullName));

        return nullFactory();
      }
    }

    protected override T StoreToViewReal(XElement sval)
    {
      T value = _typeFactory();
      using (var reader = sval.CreateReader())
        value.ReadXml(reader);
      return value;
    }

    protected override XElement ViewToStoreReal(T vval)
    {
      using (MemoryStream stream = new MemoryStream())
      using (var writer = XmlWriter.Create(stream))
      {
        vval.WriteXml(writer);
        stream.Position = 0;
        using (var reader = XmlReader.Create(stream))
          return XElement.Load(reader);
      }
    }
  }
}
