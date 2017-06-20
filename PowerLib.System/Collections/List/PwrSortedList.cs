using System;
using System.Collections;
using System.Collections.Generic;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections
{
	public class PwrSortedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IAutoCapacitySupport, IStampSupport
	{
		private IList<T> _innerList;
		private Comparison<T> _sortComparison;
		private SortingOption _sortingOption;

		#region Constructors

		public PwrSortedList(Comparison<T> sortComparison, SortingOption sortingOption = SortingOption.None)
			: this(new PwrList<T>(), sortComparison, sortingOption)
		{
		}

		public PwrSortedList(int capacity, Comparison<T> sortComparison, SortingOption sortingOption = SortingOption.None)
			: this(new PwrList<T>(capacity), sortComparison, sortingOption)
		{
		}

		public PwrSortedList(IEnumerable<T> coll, Comparison<T> sortComparison, SortingOption sortingOption = SortingOption.None, bool readOnly = false)
			: this(coll != null ? coll
          .ToSortedPwrList(sortComparison, sortingOption)
          .WithIf(list => readOnly, list => list.Restrict(CollectionRestrictions.ReadOnly)) : new PwrList<T>(), sortComparison, sortingOption)
		{
		}

		public PwrSortedList(IComparer<T> sortComparer, SortingOption sortingOption = SortingOption.None)
			: this(new PwrList<T>(), (sortComparer ?? Comparer<T>.Default).Compare, sortingOption)
		{
		}

    public PwrSortedList(int capacity, IComparer<T> sortComparer, SortingOption sortingOption = SortingOption.None)
			: this(new PwrList<T>(capacity), (sortComparer ?? Comparer<T>.Default).Compare, sortingOption)
		{
		}

		public PwrSortedList(IEnumerable<T> coll, IComparer<T> sortComparer, SortingOption sortingOption = SortingOption.None, bool readOnly = false)
			: this(coll != null ? coll
          .ToSortedPwrList((sortComparer ?? Comparer<T>.Default).Compare, sortingOption)
          .WithIf(list => readOnly, list => list.Restrict(CollectionRestrictions.ReadOnly)) : new PwrList<T>(),
          (sortComparer != null ? sortComparer : Comparer<T>.Default).Compare, sortingOption)
		{
		}

		protected PwrSortedList(IList<T> innerList, Comparison<T> sortComparison, SortingOption sortingOption = SortingOption.None)
		{
			if (innerList == null)
				throw new ArgumentNullException("innerList");
			if (sortComparison == null)
				throw new ArgumentNullException("sortComparison");

			_innerList = innerList;
			_sortComparison = sortComparison;
			_sortingOption = sortingOption;
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

    protected SortingOption SortingOption
    {
      get
      {
        return _sortingOption;
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

		public T this[int index]
		{
			get
			{
				return InnerList[index];
			}
			set
			{
				if (!InnerList.SetSorted(index, value, SortComparison, SortingOption))
          throw new InvalidOperationException("Cannot set item at specified position");
			}
		}

    #endregion
    #endregion
    #region Instance methods
		#region Manipulation methods

		public void Clear()
		{
			InnerList.Clear();
		}

		public void Add(T item)
		{
      if (InnerList.AddSorted(item, SortComparison, SortingOption) < 0)
        throw new ArgumentException("Duplicate item found in list.");
    }

		public void AddRange(IEnumerable<T> coll)
		{
      if (SortingOption == SortingOption.Unique)
      {
        var list = coll.ToSortedPwrList(SortComparison, SortingOption.Unique);
        for (int i = 0, index = 0; i < list.Count; i++)
          if ((index = InnerList.BinarySearch(t => SortComparison(t, list[i]), SortingOption)) >= 0)
            throw new ArgumentException("Duplicate item found in list.");
        coll = list;
      }
      foreach (var item in coll)
        if (InnerList.AddSorted(item, SortComparison, SortingOption) < 0)
          throw new InvalidOperationException("Duplicate item found in list.");
		}

		public void Insert(int index, T item)
		{
			if (!InnerList.InsertSorted(index, item, SortComparison, SortingOption))
				throw new ArgumentException("Cannot insert item at specified position");
		}

		public bool Remove(T value)
		{
			int index = IndexOf(value);
			if (index >= 0)
				RemoveAt(index);
			return (index >= 0);
		}

		public void RemoveAt(int index)
		{
			InnerList.RemoveAt(index);
		}

		public void RemoveRange(int index, int count)
		{
			InnerList.RemoveRange(index, count);
		}

		#endregion
		#region Extraction methods

		public void CopyTo(T[] array, int arrayIndex)
		{
			CopyTo(array, arrayIndex, 0, InnerList.Count);
		}

		public void CopyTo(T[] array, int arrayIndex, int index, int count)
		{
      InnerList.CopyTo(array, arrayIndex, index, count);
		}

		public T[] ToArray()
		{
			return ToArray(0, InnerList.Count);
		}

		public T[] ToArray(int index, int count)
		{
			T[] array = new T[count];
			CopyTo(array, 0, index, count);
			return array;
		}

		#endregion
		#region Retrieval methods

		public bool Match(Func<T, bool> match, bool all)
		{
			return Match(0, InnerList.Count, match, all);
		}

    public bool Match(int index, Func<T, bool> match, bool all)
		{
			return Match(index, InnerList.Count - index, match, all);
		}

    public bool Match(int index, int count, Func<T, bool> match, bool all)
		{
			return InnerList.Match(index, count, match, all);
		}

		public T Find(Func<T, bool> match)
		{
      return Find(0, InnerList.Count, match);
		}

		public T Find(int index, Func<T, bool> match)
		{
      return Find(index, InnerList.Count - index, match);
		}

		public T Find(int index, int count, Func<T, bool> match)
		{
			int i = InnerList.FindIndex(index, count, match);
			return i >= 0 ? InnerList[i] : default(T);
		}

		public T FindLast(Func<T, bool> match)
		{
      return FindLast(0, InnerList.Count, match);
    }

		public T FindLast(int index, Func<T, bool> match)
		{
			return FindLast(index, InnerList.Count - index, match);
		}

		public T FindLast(int index, int count, Func<T, bool> match)
		{
			int i = InnerList.FindLastIndex(index, count, match);
			return i >= 0 ? InnerList[i] : default(T);
		}

		public PwrList<T> FindAll(Func<T, bool> match)
		{
      return FindAll(0, InnerList.Count, match);
    }

		public PwrList<T> FindAll(int index, Func<T, bool> match)
		{
			return FindAll(index, InnerList.Count - index, match);
		}

		public PwrList<T> FindAll(int index, int count, Func<T, bool> match)
		{
			return new PwrList<T>(InnerList.FindAll(index, count, match), CollectionRestrictions.ReadOnly);
		}

		public int FindIndex(Func<T, bool> match)
		{
			return FindIndex(0, InnerList.Count, match);
		}

		public int FindIndex(int index, Func<T, bool> match)
		{
			return FindIndex(index, InnerList.Count - index, match);
		}

		public int FindIndex(int index, int count, Func<T, bool> match)
		{
			return InnerList.FindIndex(index, count, match);
		}

		public int FindLastIndex(Func<T, bool> match)
		{
			return FindLastIndex(0, InnerList.Count, match);
		}

		public int FindLastIndex(int index, Func<T, bool> match)
		{
			return FindLastIndex(index, InnerList.Count - index, match);
		}

		public int FindLastIndex(int index, int count, Func<T, bool> match)
		{
			return InnerList.FindLastIndex(index, count, match);
		}

		public PwrList<int> FindAllIndices(Func<T, bool> match)
		{
      return FindAllIndices(0, InnerList.Count, match);
    }

    public PwrList<int> FindAllIndices(int index, Func<T, bool> match)
		{
			return FindAllIndices(index, InnerList.Count - index, match);
		}

		public PwrList<int> FindAllIndices(int index, int count, Func<T, bool> match)
		{
			return new PwrList<int>(InnerList.FindAllIndices(index, count, match), CollectionRestrictions.ReadOnly);
		}

		public bool Contains(T value)
		{
      return Contains(value, 0, InnerList.Count);
		}

		public bool Contains(T value, int index)
		{
      return Contains(value, index, InnerList.Count - index);
    }

    public bool Contains(T value, int index, int count)
		{
			int i = InnerList.BinarySearch(index, count, t => SortComparison(t, value), SortingOption.First);
      if (i >= 0)
        for (; i < InnerList.Count && SortComparison(InnerList[i], value) == 0; i++)
          if (EqualityComparer<T>.Default.Equals(InnerList[i], value))
            return true;
      return false;
    }

    public int IndexOf(T value)
		{
      return IndexOf(value, 0, InnerList.Count);
    }

    public int IndexOf(T value, int index)
		{
      return IndexOf(value, index, InnerList.Count - index);
		}

		public int IndexOf(T value, int index, int count)
		{
			int i = InnerList.BinarySearch(index, count, t => SortComparison(t, value), SortingOption.First);
      if (i >= 0)
        for (; i < InnerList.Count && SortComparison(InnerList[i], value) == 0; i++)
          if (EqualityComparer<T>.Default.Equals(InnerList[i], value))
            return i;
      return -1;
    }

    public int LastIndexOf(T value)
		{
      return LastIndexOf(value, 0, InnerList.Count);
    }

    public int LastIndexOf(T value, int index)
		{
      return LastIndexOf(value, index, InnerList.Count - index);
    }

    public int LastIndexOf(T value, int index, int count)
		{
			int i = InnerList.BinarySearch(index, count, t => SortComparison(t, value), SortingOption.Last);
      if (i >= 0)
        for (; i >= 0 && SortComparison(InnerList[i], value) == 0; i--)
          if (EqualityComparer<T>.Default.Equals(InnerList[i], value))
            return i;
      return -1;
    }

    public int BinarySearch(T value, SortingOption sortingOption = SortingOption.None)
		{
      return BinarySearch(value, 0, InnerList.Count, sortingOption);
    }

    public int BinarySearch(T value, int index, SortingOption sortingOption = SortingOption.None)
		{
			return BinarySearch(value, index, InnerList.Count - index, sortingOption);
		}

		public int BinarySearch(T value, int index, int count, SortingOption sortingOption = SortingOption.None)
		{
			return InnerList.BinarySearch(index, count, t => SortComparison(t, value), sortingOption);
		}

		#endregion
		#endregion
		#region Interface implementation
		#region IEnumerable<T> implementation

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return InnerList.GetEnumerator();
		}

		#endregion
		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator()
		{
			return InnerList.GetEnumerator();
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
				capacitySupport.GrowFactor = value ;
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
        capacitySupport.TrimFactor = value ;
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
        capacitySupport.AutoTrim = value ;
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
		#region IChangeStampSupport

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
