using System;

namespace PowerLib.System.Data.Adapters
{
  public abstract class NullableDataTypeAdapter<S, V> : DataTypeAdapter<S, V>
  {
    protected NullableDataTypeAdapter(S sval, V vval, bool side)
      : base(sval, vval, side)
    {
    }

    protected abstract bool IsStoreNull(S sval);

    protected abstract bool IsViewNull(V vval);

    protected abstract S StoreNull { get; }

    protected abstract V ViewNull { get; }

    protected abstract V StoreToViewReal(S sval);

    protected abstract S ViewToStoreReal(V vval);

    protected override sealed V StoreToView(S sval)
    {
      return IsStoreNull(sval) ? ViewNull : StoreToViewReal(sval);
    }

    protected override sealed S ViewToStore(V vval)
    {
      return IsViewNull(vval) ? StoreNull : ViewToStoreReal(vval);
    }
  }
}
