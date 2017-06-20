using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections
{
	public class PwrDeque<T> : IDeque<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICapacitySupport, IStampSupport
	{
		private IList<T> _innerList;

		#region Constructors

		public PwrDeque()
			: this(new PwrList<T>())
		{
		}

		public PwrDeque(int capacity)
			: this(new PwrList<T>(capacity))
		{
		}

		public PwrDeque(IEnumerable<T> coll, bool reverse)
			: this((coll != null ? coll.PeekCount() : -1).WithCase(c => c >= 0, c => new PwrList<T>(c), c => new PwrList<T>()))
		{
      if (coll != null)
        if (reverse)
          EnqueueRangeFront(coll);
        else
          EnqueueRangeBack(coll);
		}

		protected PwrDeque(IList<T> innerList)
		{
			if (innerList == null)
				throw new ArgumentNullException("innerList");
			if (innerList.IsReadOnly)
				throw new ArgumentException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsReadOnly], "innerList");

      _innerList = innerList;
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
			InnerList.Insert(0, value);
		}

		public void EnqueueBack(T value)
		{
			InnerList.Insert(InnerList.Count, value);
		}

		public T DequeueFront()
		{
			if (InnerList.Count == 0)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsEmpty]);

			T value = InnerList[0];
			InnerList.RemoveAt(0);
			return value;
		}

		public T DequeueBack()
		{
			if (InnerList.Count == 0)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsEmpty]);

			T value = InnerList[InnerList.Count - 1];
			InnerList.RemoveAt(InnerList.Count - 1);
			return value;
		}

		public void EnqueueRangeFront(IEnumerable<T> coll)
		{
			if (coll == null)
				throw new ArgumentNullException("coll");

			InnerList.InsertRange(0, coll.Reverse());
		}

		public void EnqueueRangeBack(IEnumerable<T> coll)
		{
      if (coll == null)
        throw new ArgumentNullException("coll");

      InnerList.InsertRange(InnerList.Count, coll);
		}

		public PwrList<T> DequeueRangeFront(int count)
		{
			if (count < 0 || count > _innerList.Count)
				throw new ArgumentOutOfRangeException("count");

      PwrList<T> list = new PwrList<T>(InnerList.Enumerate(0, count));
      InnerList.RemoveRange(0, count);
      return list;
		}

		public PwrList<T> DequeueRangeBack(int count)
		{
			if (count < 0 || count > _innerList.Count)
				throw new ArgumentOutOfRangeException("count");

      int index = InnerList.Count - count;
      PwrList<T> list = new PwrList<T>(InnerList.Enumerate(index, count, true));
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
      T[] array = new T[InnerList.Count];
      CopyTo(array, 0);
      return array;
    }

    #endregion
    #region Retrieval methods

    public bool Contains(T value)
    {
      return InnerList.Contains(value);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return InnerList.Enumerate().GetEnumerator();
    }

    #endregion
    #endregion
    #endregion
    #region Interfaces implementations
    #region IEnumerable implementation

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

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
				return false;
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
    #region IChangeStamp implementation

    int IStampSupport.Stamp
		{
			get
			{
				return ((IStampSupport)_innerList).Stamp;
			}
		}

		#endregion
		#endregion
	}
}
