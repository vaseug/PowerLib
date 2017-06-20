using System;
using System.IO;
using System.Xml.Serialization;

namespace PowerLib.System.Data.Adapters
{
  public class StringXmlSerializerAdapter : NullableDataTypeAdapter<String, Object>
  {
    private readonly Func<Object> _nullFactory;
    private readonly Func<Object, bool> _nullPredicate;
    private readonly XmlSerializer _serializer;

    public StringXmlSerializerAdapter(String sval, XmlSerializer serializer)
      : this(sval, null, false, serializer, () => null, v => v == null)
    {
    }

    public StringXmlSerializerAdapter(String sval, XmlSerializer serializer, Func<Object> nullFactory, Func<Object, bool> nullPredicate)
      : this(sval, null, false, serializer, nullFactory, nullPredicate)
    {
    }

    public StringXmlSerializerAdapter(String sval, Object vval, bool side, XmlSerializer serializer)
      : this(sval, null, false, serializer, () => null, v => v == null)
    {
    }

    public StringXmlSerializerAdapter(String sval, Object vval, bool side, XmlSerializer serializer, Func<Object> nullFactory, Func<Object, bool> nullPredicate)
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

    protected override bool IsStoreUpdated(String sval)
    {
      return false;
    }

    protected override bool IsStoreNull(String sval)
    {
      return sval == null;
    }

    protected override String StoreNull
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

    protected override Object StoreToViewReal(String sval)
    {
      using (var reader = new StringReader(sval))
        return _serializer.Deserialize(reader);
    }

    protected override String ViewToStoreReal(Object vval)
    {
      using (var writer = new StringWriter())
      {
        _serializer.Serialize(writer, vval);
        return writer.GetStringBuilder().ToString();
      }
    }
  }
}
