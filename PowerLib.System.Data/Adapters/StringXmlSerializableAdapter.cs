using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Data.SqlTypes;

namespace PowerLib.System.Data.Adapters
{
  public class StringXmlSerializableAdapter<T> : NullableDataTypeAdapter<String, T>
    where T : IXmlSerializable, INullable
  {
    private static Func<T> nullFactory;
    private readonly Func<T> _typeFactory;

    public StringXmlSerializableAdapter(String sval, Func<T> typeFactory)
      : this(sval, default(T), false, typeFactory)
    {
    }

    public StringXmlSerializableAdapter(String sval, T vval, bool side, Func<T> typeFactory)
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

    protected override T StoreToViewReal(String sval)
    {
      using (MemoryStream stream = new MemoryStream())
      using (var writer = new StreamWriter(stream) { AutoFlush = true })
      {
        writer.Write(sval);
        using (var reader = XmlReader.Create(stream))
        {
          stream.Position = 0L;
          T value = _typeFactory();
          value.ReadXml(reader);
          return value;
        }
      }
    }

    protected override String ViewToStoreReal(T vval)
    {
      using (var swriter = new StringWriter())
      using (var xwriter = XmlWriter.Create(swriter))
      {
        vval.WriteXml(xwriter);
        return swriter.GetStringBuilder().ToString();
      }
    }
  }
}
