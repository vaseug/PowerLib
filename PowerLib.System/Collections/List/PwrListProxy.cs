using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerLib.System.Collections
{
  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T">Type of item</typeparam>
  public class PwrListProxy<T> : IList<T>, IReadOnlyList<T>
  {
    private Func<int> _counter;
    private Func<int, T> _selector;

    #region Constructors

    public PwrListProxy(Func<int> counter, Func<int, T> selector)
    {
      if (counter == null)
        throw new ArgumentNullException("counter");
      if (selector == null)
        throw new ArgumentNullException("selector");

      _counter = counter;
      _selector = selector;
    }

    #endregion
    #region Properties

    public int Count
    {
      get
      {
        return _counter();
      }
    }

    public T this[int index]
    {
      get
      {
        if (index < 0 || index >= _counter())
          throw new ArgumentOutOfRangeException("index");

        return _selector(index);
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    #endregion
    #region Methods

    public void CopyTo(T[] array, int arrayIndex)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (arrayIndex < 0 || arrayIndex > array.Length - _counter())
        throw new ArgumentOutOfRangeException("arrayIndex");

      for (int i = 0, c = _counter(); i < c; i++)
        array[arrayIndex++] = _selector(i);
    }

    public int IndexOf(T value)
    {
      for (int i = 0, c = _counter(); i < c; i++)
        if (EqualityComparer<T>.Default.Equals(_selector(i), value))
          return i;
      return -1;
    }

    public bool Contains(T value)
    {
      return IndexOf(value) >= 0;
    }

    #endregion
    #region Interfaces
    #region IEnumerable<T> implementation

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<T>)this).GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      for (int i = 0, c = _counter(); i < c; i++)
        yield return _selector(i);
    }

    #endregion
    #region ICollection<T> interface

    bool ICollection<T>.IsReadOnly
    {
      get
      {
        return true;
      }
    }

    void ICollection<T>.Add(T value)
    {
      throw new NotSupportedException();
    }

    bool ICollection<T>.Remove(T value)
    {
      throw new NotSupportedException();
    }

    void ICollection<T>.Clear()
    {
      throw new NotSupportedException();
    }

    #endregion
    #region ICollection<T> interface

    void IList<T>.Insert(int index, T item)
    {
      throw new NotSupportedException();
    }

    void IList<T>.RemoveAt(int index)
    {
      throw new NotSupportedException();
    }

    #endregion
    #endregion
  }
}
