using System;
using System.Collections;
using System.Collections.Generic;
using PowerLib.System.Collections.Matching;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections
{
	public class PwrSortedDeque<T> : IDeque<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICapacitySupport, IStampSupport
	{
		private IList<T> _innerList;
		private Comparison<T> _sortComparison;

		#region Constructors

		public PwrSortedDeque(Comparison<T> sortComparison)
			: this(new PwrList<T>(), new InverseComparer<T>(sortComparison).Compare)
		{
		}

		public PwrSortedDeque(int capacity, Comparison<T> sortComparison)
			: this(new PwrList<T>(capacity), new InverseComparer<T>(sortComparison).Compare)
		{
		}

		public PwrSortedDeque(IEnumerable<T> coll, bool side, Comparison<T> sortComparison)
			: this((coll != null ? coll.PeekCount() : -1).WithCase(c => c >= 0, c => new PwrList<T>(c), c => new PwrList<T>()),
          new InverseComparer<T>(sortComparison).Compare)
		{
      if (coll != null)
        if (side)
          EnqueueRangeFront(coll);
        else
          EnqueueRangeBack(coll);
		}

    public PwrSortedDeque(IComparer<T> sortComparer)
      : this(new PwrList<T>(), new InverseComparer<T>(sortComparer ?? Comparer<T>.Default).Compare)
    {
    }

    public PwrSortedDeque(int capacity, IComparer<T> sortComparer)
      : this(new PwrList<T>(capacity), new InverseComparer<T>(sortComparer ?? Comparer<T>.Default).Compare)
    {
    }

    public PwrSortedDeque(IEnumerable<T> coll, bool side, IComparer<T> sortComparer)
      : this((coll != null ? coll.PeekCount() : -1).WithCase(c => c >= 0, c => new PwrList<T>(c), c => new PwrList<T>()),
          new InverseComparer<T>(sortComparer ?? Comparer<T>.Default).Compare)
    {
      if (coll != null)
        if (side)
          EnqueueRangeFront(coll);
        else
          EnqueueRangeBack(coll);
    }

    protected PwrSortedDeque(IList<T> innerList, Comparison<T> sortComparison)
		{
			if (innerList == null)
				throw new ArgumentNullException("innerList");
			if (innerList.IsReadOnly)
				throw new ArgumentException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsReadOnly]);

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

		public int Count
		{
			get
			{
				return InnerList.Count;
			}
		}

		public T Front
		{
			get
			{
				if (InnerList.Count == 0)
					throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsEmpty]);

        return InnerList[0];
			}
		}

		public T Back
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

		public void EnqueueFront(T value)
		{
			InnerList.AddSorted(value, SortComparison, SortingOption.First);
		}

		public void EnqueueBack(T value)
		{
			InnerList.AddSorted(value, SortComparison, SortingOption.Last);
		}

		public T DequeueFront()
		{
			if (InnerList.Count == 0)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsEmpty]);

      var value = InnerList[0];
			InnerList.RemoveAt(0);
			return value;
		}

		public T DequeueBack()
		{
			if (InnerList.Count == 0)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsEmpty]);

      var index = InnerList.Count - 1;
      var value = InnerList[index];
			InnerList.RemoveAt(index);
			return value;
		}

		public void EnqueueRangeFront(IEnumerable<T> coll)
		{
			if (coll == null)
				throw new ArgumentNullException("coll");

			using (IEnumerator<T> e = coll.GetEnumerator())
				while (e.MoveNext())
					InnerList.AddSorted(e.Current, SortComparison, SortingOption.First);
		}

		public void EnqueueRangeBack(IEnumerable<T> coll)
		{
			if (coll == null)
				throw new ArgumentNullException("coll");

			using (IEnumerator<T> e = coll.GetEnumerator())
				while (e.MoveNext())
					InnerList.AddSorted(e.Current, SortComparison, SortingOption.Last);
		}

    public PwrList<T> DequeueRangeFront(int count)
    {
      if (count < 0 || count > _innerList.Count)
        throw new ArgumentOutOfRangeException("count");

      var list = new PwrList<T>(InnerList.Enumerate(0, count, false));
      InnerList.RemoveRange(0, count);
      return list;
    }

    public PwrList<T> DequeueRangeBack(int count)
    {
      if (count < 0 || count > _innerList.Count)
        throw new ArgumentOutOfRangeException("count");

      var index = InnerList.Count - count;
      var list = new PwrList<T>(InnerList.Enumerate(index, count, true));
      InnerList.RemoveRange(index, count);
      return list;
    }

    public PwrList<T> DequeueAllFront()
    {
      return DequeueRangeFront(InnerList.Count);
    }

    public PwrList<T> DequeueAllBack()
    {
      return DequeueRangeBack(InnerList.Count);
    }

    #endregion
    #region Extraction methods

    public void CopyTo(T[] array, int arrayIndex)
    {
      InnerList.CopyTo(array, arrayIndex);
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
      return InnerList.FindIndex(t => EqualityComparer<T>.Default.Equals(t, value)) >= 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return InnerList.Enumerate().GetEnumerator();
    }

    #endregion
    #endregion
    #endregion
    #region Interfaces implementations
    #region IEnumerable<T> implementation

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
    #region IEnumerable implementation

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
    #region ICollection implementation

    bool ICollection<T>.IsReadOnly
    {
      get
      {
        return InnerList.IsReadOnly;
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
    #region IChangeStampSupport implementation

    int IStampSupport.Stamp
    {
      get
      {
        var stampSupport = InnerList as IStampSupport;
        if (stampSupport == null)
          throw new NotSupportedException();
        return stampSupport.Stamp;
      }
    }

    #endregion
    #endregion
  }
}
