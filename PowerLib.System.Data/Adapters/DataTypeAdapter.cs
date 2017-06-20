using System;

namespace PowerLib.System.Data.Adapters
{
  public abstract class DataTypeAdapter<S, V>
  {
    private S _sval;
    private bool _sin, _sout;
    private V _vval;
    private bool _vin, _vout;

    protected DataTypeAdapter(S sval, V vval, bool side)
    {
      _sval = sval;
      _vval = vval;
      _sin = !side;
      _vin = side;
      _sout = _vout = false;
    }

    protected virtual void ValidateStore(S avl)
    {
    }

    protected virtual void ValidateView(V bvl)
    {
    }

    protected abstract bool IsStoreUpdated(S sval);

    protected abstract bool IsViewUpdated(V vval);

    protected abstract V StoreToView(S sval);

    protected abstract S ViewToStore(V vval);

    public S StoreValue
    {
      get
      {
        if (_vin || _vout && IsViewUpdated(_vval))
        {
          _sval = ViewToStore(_vval);
          _sout = true;
          if (_vin)
            _vin = false;
        }
        return _sval;
      }
      set
      {
        ValidateStore(value);
        _sval = value;
        _sin = true;
        _vin = false;
        _vout = false;
      }
    }

    public V ViewValue
    {
      get
      {
        if (_sin || _sout && IsStoreUpdated(_sval))
        {
          _vval = StoreToView(_sval);
          _vout = true;
          if (_sin)
            _sin = false;
        }
        return _vval;
      }
      set
      {
        ValidateView(value);
        _vval = value;
        _vin = true;
        _sin = false;
        _sout = false;
      }
    }
  }
}
