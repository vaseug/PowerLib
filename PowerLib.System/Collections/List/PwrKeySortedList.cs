using System;
using System.Collections;
using System.Collections.Generic;
using PowerLib.System.Collections.Matching;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections
{
	public class PwrKeySortedList<K, T> : PwrSortedList<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICapacitySupport, IStampSupport
	{
		private Func<T, K> _keySelector;
		private Comparison<K> _keyComparison;

    #region Constructors

    public PwrKeySortedList(Func<T, K> keySelector, Comparison<K> keyComparison, SortingOption sortingOption = SortingOption.None)
      : this(new PwrList<T>(), keySelector, keyComparison, sortingOption)
    {
    }

    public PwrKeySortedList(int capacity, Func<T, K> keySelector, Comparison<K> keyComparison, SortingOption sortingOption = SortingOption.None)
      : this(new PwrList<T>(capacity), keySelector, keyComparison, sortingOption)
    {
    }

    public PwrKeySortedList(IEnumerable<T> coll, Func<T, K> keySelector, Comparison<K> keyComparison, SortingOption sortingOption = SortingOption.None, bool readOnly = false)
      : this(coll != null ? coll
          .ToSortedPwrList(new SelectComparer<T, K>(keySelector, keyComparison).Compare, sortingOption)
          .WithIf(l => readOnly, l => l.Restrict(CollectionRestrictions.ReadOnly)) : new PwrList<T>(), keySelector, keyComparison, sortingOption)
    {
    }

    public PwrKeySortedList(Func<T, K> keySelector, IComparer<K> keyComparer, SortingOption sortingOption = SortingOption.None)
      : this(new PwrList<T>(), keySelector, (keyComparer ?? Comparer<K>.Default).Compare, sortingOption)
    {
    }

    public PwrKeySortedList(int capacity, Func<T, K> keySelector, IComparer<K> keyComparer, SortingOption sortingOption = SortingOption.None)
      : this(new PwrList<T>(capacity), keySelector, (keyComparer ?? Comparer<K>.Default).Compare, sortingOption)
    {
    }

    public PwrKeySortedList(IEnumerable<T> coll, Func<T, K> keySelector, IComparer<K> keyComparer, SortingOption sortingOption = SortingOption.None, bool readOnly = false)
      : this(coll != null ? coll
          .ToSortedPwrList(new SelectComparer<T, K>(keySelector, keyComparer ?? Comparer<K>.Default).Compare, sortingOption)
          .WithIf(l => readOnly, l => l.Restrict(CollectionRestrictions.ReadOnly)) : new PwrList<T>(), keySelector, (keyComparer ?? Comparer<K>.Default).Compare, sortingOption)
    {
    }

		protected PwrKeySortedList(IList<T> innerList, Func<T, K> keySelector, Comparison<K> keyComparison, SortingOption sortingOption = SortingOption.None)
      : base(innerList, new SelectComparer<T, K>(keySelector, keyComparison).Compare, sortingOption)
		{
      if (keyComparison == null)
        throw new ArgumentNullException("keyComparison");
      if (keySelector == null)
				throw new ArgumentNullException("keySelector");

      _keyComparison = keyComparison;
      _keySelector = keySelector;
		}

		#endregion
		#region Instance properties
		#region Public properties

		protected Func<T, K> KeySelector
		{
			get
			{
				return _keySelector;
			}
		}

		protected Comparison<K> KeyComparison
		{
			get
			{
				return _keyComparison;
			}
		}

		#endregion
		#endregion
		#region Instance methods
		#region Retrieval methods

		public bool ContainsKey(K key)
		{
      return ContainsKey(key, 0, InnerList.Count);
    }

    public bool ContainsKey(K key, int index)
		{
			return ContainsKey(key, index, InnerList.Count - index);
		}

		public bool ContainsKey(K key, int index, int count)
		{
			int i = InnerList.BinarySearch(index, count, t => KeyComparison(KeySelector(t), key), SortingOption.None);
      if (i >= 0)
        for (; i < InnerList.Count && KeyComparison(KeySelector(InnerList[i]), key) == 0; i++)
          if (EqualityComparer<K>.Default.Equals(KeySelector(InnerList[i]), key))
            return true;
      return false;
		}

		public int IndexOfKey(K key)
		{
      return IndexOfKey(key, 0, InnerList.Count);
		}

		public int IndexOfKey(K key, int index)
		{
			return IndexOfKey(key, index, InnerList.Count - index);
		}

		public int IndexOfKey(K key, int index, int count)
		{
      int i = InnerList.BinarySearch(index, count, t => KeyComparison(KeySelector(t), key), SortingOption.First);
      if (i >= 0)
        for (; i < InnerList.Count && KeyComparison(KeySelector(InnerList[i]), key) == 0; i++)
          if (EqualityComparer<K>.Default.Equals(KeySelector(InnerList[i]), key))
            return i;
      return -1;
    }

    public int LastIndexOfKey(K key)
		{
      return LastIndexOfKey(key, 0, InnerList.Count);
    }

    public int LastIndexOfKey(K key, int index)
		{
      return LastIndexOfKey(key, index, InnerList.Count - index);
    }

    public int LastIndexOfKey(K key, int index, int count)
		{
      int i = InnerList.BinarySearch(index, count, t => KeyComparison(KeySelector(t), key), SortingOption.Last);
      if (i >= 0)
        for (; i >= 0 && KeyComparison(KeySelector(InnerList[i]), key) == 0; i--)
          if (EqualityComparer<K>.Default.Equals(KeySelector(InnerList[i]), key))
            return i;
      return -1;
    }

    public int BinarySearchKey(K key, SortingOption sortingOption = SortingOption.None)
    {
      return BinarySearchKey(key, 0, InnerList.Count, sortingOption);
    }

    public int BinarySearchKey(K key, int index, SortingOption sortingOption = SortingOption.None)
    {
      return BinarySearchKey(key, index, InnerList.Count - index, sortingOption);
    }

    public int BinarySearchKey(K key, int index, int count, SortingOption sortingOption = SortingOption.None)
    {
      return InnerList.BinarySearch(index, count, t => KeyComparison(KeySelector(t), key), sortingOption);
    }

    #endregion
    #endregion
  }
}
