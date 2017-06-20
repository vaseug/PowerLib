using System;
using System.Collections;
using System.Collections.Generic;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Elemementype.</typeparam>
	public class PwrSortedStack<T> : IStack<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICapacitySupport, IStampSupport
	{
    private IList<T> _innerList;
    private Comparison<T> _sortComparison;

		#region Constructors

		public PwrSortedStack(Comparison<T> sortComparison)
			: this(new PwrList<T>(), sortComparison)
		{
		}

		public PwrSortedStack(int capacity, Comparison<T> sortComparison)
			: this(new PwrList<T>(capacity), sortComparison)
		{
		}

		public PwrSortedStack(IEnumerable<T> coll, Comparison<T> sortComparison)
			: this((coll != null ? coll.PeekCount() : -1).WithCase(c => c >= 0, c => new PwrList<T>(c), c => new PwrList<T>()),
          sortComparison)
		{
      if (coll != null)
        PushRange(coll);
		}

    public PwrSortedStack(IComparer<T> sortComparer)
      : this(new PwrList<T>(), (sortComparer ?? Comparer<T>.Default).Compare)
    {
    }

    public PwrSortedStack(int capacity, IComparer<T> sortComparer)
      : this(new PwrList<T>(capacity), (sortComparer ?? Comparer<T>.Default).Compare)
    {
    }

    public PwrSortedStack(IEnumerable<T> coll, IComparer<T> sortComparer)
      : this((coll != null ? coll.PeekCount() : -1).WithCase(c => c >= 0, c => new PwrList<T>(c), c => new PwrList<T>()),
          (sortComparer ?? Comparer<T>.Default).Compare)
    {
      if (coll != null)
        PushRange(coll);
    }

    protected PwrSortedStack(IList<T> innerList, Comparison<T> sortComparison)
		{
      if (innerList == null)
        throw new ArgumentNullException("innerList");
      if (sortComparison == null)
				throw new ArgumentNullException("sortComparison");
      if (innerList.IsReadOnly)
        throw new ArgumentException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsReadOnly], "innerList");

      _innerList = innerList;
			_sortComparison = sortComparison;
		}

    #endregion
    #region Instance properties
    #region Internal properties

    protected IList<T> InnerList
    {
      get
      {
        return _innerList;
      }
    }

    protected Comparison<T> SortComparison
    {
      get
      {
        return _sortComparison;
      }
    }

    #endregion
    #region Public properties

    public bool IsReadOnly
    {
      get
      {
        return InnerList.IsReadOnly;
      }
    }

    public int Count
    {
      get
      {
        return InnerList.Count;
      }
    }

    public T Top
    {
      get
      {
        if (InnerList.Count == 0)
          throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsEmpty]);

        return InnerList[InnerList.Count - 1];
      }
    }

    #endregion
    #endregion
    #region Instance methods
    #region Public methods
    #region Manipulation methods

    public void Clear()
    {
      InnerList.Clear();
    }

    public void Push(T value)
    {
      InnerList.AddSorted(value, SortComparison, SortingOption.Last);
    }

    public T Pop()
    {
      if (InnerList.Count == 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsEmpty]);

      var index = InnerList.Count - 1;
      var value = InnerList[index];
      InnerList.RemoveAt(index);
      return value;
    }

    public void PushRange(IEnumerable<T> coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");

      using (IEnumerator<T> e = coll.GetEnumerator())
        while (e.MoveNext())
          InnerList.AddSorted(e.Current, SortComparison, SortingOption.Last);
    }

    public PwrList<T> PopRange(int count)
    {
      if (count < 0 || count > InnerList.Count)
        throw new ArgumentOutOfRangeException("count");

      var list = new PwrList<T>(count);
      var index = InnerList.Count - count;
      list.AddRange(InnerList.Enumerate(index, count, true));
      InnerList.RemoveRange(index, count);
      return list;
    }

    public PwrList<T> PopAll()
    {
      return PopRange(InnerList.Count);
    }

    #endregion
    #region Extraction methods

    public void CopyTo(T[] array, int arrayIndex)
    {
      InnerList.CopyTo(array, arrayIndex, true);
    }

    public T[] ToArray()
    {
      var array = new T[InnerList.Count];
      CopyTo(array, 0);
      return array;
    }

    #endregion
    #region Retrieval methods

    public bool Contains(T value)
    {
      return InnerList.FindLastIndex(t => EqualityComparer<T>.Default.Equals(t, value)) >= 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return InnerList.Enumerate(true).GetEnumerator();
    }

    #endregion
    #endregion
    #endregion
    #region Interfaces implementations
    #region ICollection<T> implementation

    bool ICollection<T>.IsReadOnly
    {
      get
      {
        return _innerList.IsReadOnly;
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
      Clear();
    }

    bool ICollection<T>.Contains(T value)
    {
      return Contains(value);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
      CopyTo(array, arrayIndex);
    }

    #endregion
    #region IEnumerable<T> implementation

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
    #region ICapacitySupport

    public void TrimExcess()
    {
      var capacitySupport = InnerList as ICapacitySupport;
      if (capacitySupport == null)
        throw new NotSupportedException();
      capacitySupport.TrimExcess();
    }

    public float LoadFactor
    {
      get
      {
        var capacitySupport = InnerList as ICapacitySupport;
        if (capacitySupport == null)
          throw new NotSupportedException();
        return capacitySupport.LoadFactor;
      }
    }

    public float GrowFactor
    {
      get
      {
        IAutoCapacitySupport capacitySupport = InnerList as IAutoCapacitySupport;
        if (capacitySupport == null)
          throw new NotSupportedException();
        return capacitySupport.GrowFactor;
      }
      set
      {
        IAutoCapacitySupport capacitySupport = InnerList as IAutoCapacitySupport;
        if (capacitySupport == null)
          throw new NotSupportedException();
        capacitySupport.GrowFactor = value;
      }
    }

    public float TrimFactor
    {
      get
      {
        IAutoCapacitySupport capacitySupport = InnerList as IAutoCapacitySupport;
        if (capacitySupport == null)
          throw new NotSupportedException();
        return capacitySupport.TrimFactor;
      }
      set
      {
        IAutoCapacitySupport capacitySupport = InnerList as IAutoCapacitySupport;
        if (capacitySupport == null)
          throw new NotSupportedException();
        capacitySupport.TrimFactor = value;
      }
    }

    public bool AutoTrim
    {
      get
      {
        IAutoCapacitySupport capacitySupport = InnerList as IAutoCapacitySupport;
        if (capacitySupport == null)
          throw new NotSupportedException();
        return capacitySupport.AutoTrim;
      }
      set
      {
        IAutoCapacitySupport capacitySupport = InnerList as IAutoCapacitySupport;
        if (capacitySupport == null)
          throw new NotSupportedException();
        capacitySupport.AutoTrim = value;
      }
    }

    public int Capacity
    {
      get
      {
        ICapacitySupport capacitySupport = InnerList as ICapacitySupport;
        if (capacitySupport == null)
          throw new NotSupportedException();
        return capacitySupport.Capacity;
      }
      set
      {
        ICapacitySupport capacitySupport = InnerList as ICapacitySupport;
        if (capacitySupport == null)
          throw new NotSupportedException();
        capacitySupport.Capacity = value;
      }
    }

    #endregion
    #region IChangeStamp implementation

    int IStampSupport.Stamp
    {
      get
      {
        IStampSupport stampSupport = InnerList as IStampSupport;
        if (stampSupport == null)
          throw new NotSupportedException();
        return stampSupport.Stamp;
      }
    }

    #endregion
    #endregion
  }
}
