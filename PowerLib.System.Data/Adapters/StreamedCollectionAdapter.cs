using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using PowerLib.System.IO.Streamed;

namespace PowerLib.System.Data.Adapters
{
  public class StreamedCollectionAdapter<T, V> : NullableDataTypeAdapter<Byte[], V>
    where V : class
  {
    private readonly Func<Stream, StreamedCollection<T>> _fromStore;
    private readonly Func<StreamedCollection<T>, V> _toView;
    private readonly Func<Stream, V, StreamedCollection<T>> _fromView;

    public StreamedCollectionAdapter(Byte[] sval, V vval, bool side,
      Func<Stream, StreamedCollection<T>> fromStore, Func<StreamedCollection<T>, V> toView, Func<Stream, V, StreamedCollection<T>> fromView)
      : base(sval, vval, side)
    {
      if (fromStore == null)
        throw new ArgumentNullException("fromStore");
      if (fromView == null)
        throw new ArgumentNullException("fromView");
      if (toView == null)
        throw new ArgumentNullException("toView");

      _fromStore = fromStore;
      _fromView = fromView;
      _toView = toView;
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

    protected override sealed bool IsViewUpdated(V vval)
    {
      return true;
    }

    protected override bool IsViewNull(V vval)
    {
      return vval == null;
    }

    protected override sealed V ViewNull
    {
      get
      {
        return null;
      }
    }

    protected override V StoreToViewReal(Byte[] sval)
    {
      using (var ms = new MemoryStream(sval))
      using (var sc = _fromStore(ms))
        return _toView(sc);
    }

    protected override Byte[] ViewToStoreReal(V vval)
    {
      using (var ms = new MemoryStream())
      using (var sc = _fromView(ms, vval))
        return ms.ToArray();
    }
  }
}
