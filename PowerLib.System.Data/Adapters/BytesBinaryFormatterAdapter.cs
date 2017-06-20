using System;
using System.IO;
using System.Runtime.Serialization;

namespace PowerLib.System.Data.Adapters
{
  public class BytesBinaryFormatterAdapter : NullableDataTypeAdapter<Byte[], Object>
  {
    private readonly Func<Object> _nullFactory;
    private readonly Func<Object, bool> _nullPredicate;
    private readonly IFormatter _formatter;

    public BytesBinaryFormatterAdapter(Byte[] sval, IFormatter formatter)
      : this(sval, null, false, formatter, () => null, v => v == null)
    {
    }

    public BytesBinaryFormatterAdapter(Byte[] sval, IFormatter formatter, Func<Object> nullFactory, Func<Object, bool> nullPredicate)
      : this(sval, null, false, formatter, nullFactory, nullPredicate)
    {
    }

    public BytesBinaryFormatterAdapter(Byte[] sval, Object vval, bool side, IFormatter formatter)
      : this(sval, vval, side, formatter, () => null, v => v == null)
    {
    }

    public BytesBinaryFormatterAdapter(Byte[] sval, Object vval, bool side, IFormatter formatter, Func<Object> nullFactory, Func<Object, bool> nullPredicate)
      : base(sval, vval, side)
    {
      if (formatter == null)
        throw new ArgumentNullException("formatter");
      if (nullFactory == null)
        throw new ArgumentNullException("nullFactory");
      if (nullPredicate == null)
        throw new ArgumentNullException("nullPredicate");

      _formatter = formatter;
      _nullFactory = nullFactory;
      _nullPredicate = nullPredicate;
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

    protected override sealed bool IsViewUpdated(Object vval)
    {
      return true;
    }

    protected override bool IsViewNull(Object vval)
    {
      return _nullPredicate(vval);
    }

    protected override sealed Object ViewNull
    {
      get
      {
        return _nullFactory();
      }
    }

    protected override Object StoreToViewReal(Byte[] sval)
    {
      using (var stream = new MemoryStream(sval))
      {
        return _formatter.Deserialize(stream);
      }
    }

    protected override Byte[] ViewToStoreReal(Object vval)
    {
      using (var stream = new MemoryStream())
      {
        _formatter.Serialize(stream, vval);
        return stream.ToArray();
      }
    }
  }
}
