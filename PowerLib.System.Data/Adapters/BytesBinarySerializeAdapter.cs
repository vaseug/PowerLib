using System;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace PowerLib.System.Data.Adapters
{
  public class BytesBinarySerializeAdapter<T> : NullableDataTypeAdapter<Byte[], T>
    where T : IBinarySerialize, INullable
  {
    private static Func<T> nullFactory;
    private readonly Func<T> _typeFactory;

    public BytesBinarySerializeAdapter(Byte[] sval, Func<T> typeFactory)
      : this(sval, default(T), false, typeFactory)
    {
    }

    public BytesBinarySerializeAdapter(Byte[] sval, T vval, bool side, Func<T> typeFactory)
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

    protected override bool IsStoreUpdated(Byte[] sval)
    {
      return true;
    }

    protected override bool IsStoreNull(Byte[] sval)
    {
      return sval == null;
    }

    protected override Byte[] StoreNull
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

    protected override T StoreToViewReal(Byte[] sval)
    {
      using (var stream = new MemoryStream(sval, false))
      using (var reader = new BinaryReader(stream))
      {
        T value = _typeFactory();
        value.Read(reader);
        return value;
      }
    }

    protected override Byte[] ViewToStoreReal(T vval)
    {
      using (var stream = new MemoryStream())
      using (var writer = new BinaryWriter(stream))
      {
        vval.Write(writer);
        return stream.ToArray();
      }
    }
  }
}
