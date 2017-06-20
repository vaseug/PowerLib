using System;
using System.Collections;
using System.Collections.Generic;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections
{
	public class PwrQueue<T> : IQueue<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IAutoCapacitySupport, IStampSupport
	{
		private IList<T> _innerList;

		#region Constructors

		public PwrQueue()
			: this(new PwrList<T>())
		{
		}

		public PwrQueue(int capacity)
			: this(new PwrList<T>(capacity))
		{
		}

		public PwrQueue(IEnumerable<T> coll)
			: this((coll != null ? coll.PeekCount() : -1).WithCase(c => c >= 0, c => new PwrList<T>(c), c => new PwrList<T>()))
		{
      if (coll != null)
        EnqueueRange(coll);
		}

		protected PwrQueue(IList<T> innerList)
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

		public void Enqueue(T value)
		{
			InnerList.Add(value);
		}

		public T Dequeue()
		{
			if (InnerList.Count == 0)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsEmpty]);

			var value = InnerList[0];
			InnerList.RemoveAt(0);
			return value;
		}

		public void EnqueueRange(IEnumerable<T> coll)
		{
			if (coll == null)
				throw new ArgumentNullException("coll");

      InnerList.AddRange(coll);
		}

		public PwrList<T> DequeueRange(int count)
		{
			if (count < 0 || count > InnerList.Count)
				throw new ArgumentOutOfRangeException("count");

			var list = new PwrList<T>(count);
      list.AddRange(InnerList.Enumerate(0, count));
      list.RemoveRange(0, count);
			return list;
		}

		public PwrList<T> DequeueAll()
		{
      return DequeueRange(InnerList.Count);
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
