using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PowerLib.System.Data.Adapters
{
  public class XElementXmlSerializerAdapter : NullableDataTypeAdapter<XElement, Object>
  {
    private readonly Func<Object> _nullFactory;
    private readonly Func<Object, bool> _nullPredicate;
    private readonly XmlSerializer _serializer;

    public XElementXmlSerializerAdapter(XElement sval, XmlSerializer serializer)
      : this(sval, null, false, serializer, () => null, v => v == null)
    {
    }

    public XElementXmlSerializerAdapter(XElement sval, XmlSerializer serializer, Func<Object> nullFactory, Func<Object, bool> nullPredicate)
      : this(sval, null, false, serializer, nullFactory, nullPredicate)
    {
    }

    public XElementXmlSerializerAdapter(XElement sval, Object vval, bool side, XmlSerializer serializer)
      : this(sval, null, false, serializer, () => null, v => v == null)
    {
    }

    public XElementXmlSerializerAdapter(XElement sval, Object vval, bool side, XmlSerializer serializer, Func<Object> nullFactory, Func<Object, bool> nullPredicate)
      : base(sval, vval, side)
    {
      if (serializer == null)
        throw new ArgumentNullException("serializer");
      if (nullFactory == null)
        throw new ArgumentNullException("nullFactory");
      if (nullPredicate == null)
        throw new ArgumentNullException("nullPredicate");

      _serializer = serializer;
      _nullFactory = nullFactory;
      _nullPredicate = nullPredicate;
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

    protected override bool IsViewUpdated(Object vval)
    {
      return true;
    }

    protected override bool IsViewNull(Object vval)
    {
      return _nullPredicate(vval);
    }

    protected override Object ViewNull
    {
      get
      {
        return _nullFactory();
      }
    }

    protected override Object StoreToViewReal(XElement sval)
    {
      using (var reader = sval.CreateReader())
        return _serializer.Deserialize(reader);
    }

    protected override XElement ViewToStoreReal(Object vval)
    {
      using (MemoryStream stream = new MemoryStream())
      {
        _serializer.Serialize(stream, vval);
        stream.Position = 0;
        using (var reader = XmlReader.Create(stream))
          return XElement.Load(reader);
      }
    }
  }
}
