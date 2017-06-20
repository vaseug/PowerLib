using System;
using System.Collections;
using System.Collections.Generic;
using PowerLib.System.Collections.Matching;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="K">Element key type.</typeparam>
	/// <typeparam name="T">Element type.</typeparam>
	public class PwrKeySortedStack<K, T> : PwrSortedStack<T>, IStack<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICapacitySupport, IStampSupport
	{
		private Func<T, K> _keySelector;
		private Comparison<K> _keyComparison;

		#region Constructors

		public PwrKeySortedStack(Func<T, K> keySelector, Comparison<K> keyComparison)
			: this(new PwrList<T>(), keySelector, keyComparison)
		{
		}

		public PwrKeySortedStack(int capacity, Func<T, K> keySelector, Comparison<K> keyComparison)
			: this(new PwrList<T>(capacity), keySelector, keyComparison)
		{
		}

		public PwrKeySortedStack(IEnumerable<T> coll, Func<T, K> keySelector, Comparison<K> keyComparison)
			: this((coll != null ? coll.PeekCount() : -1).WithCase(c => c >= 0, c => new PwrList<T>(c), c => new PwrList<T>()),
          keySelector, keyComparison)
		{
      if (coll != null)
        PushRange(coll);
		}

    public PwrKeySortedStack(Func<T, K> keySelector, IComparer<K> keyComparer)
      : this(new PwrList<T>(), keySelector, (keyComparer ?? Comparer<K>.Default).Compare)
    {
    }

    public PwrKeySortedStack(int capacity, Func<T, K> keySelector, IComparer<K> keyComparer)
      : this(new PwrList<T>(capacity), keySelector, (keyComparer ?? Comparer<K>.Default).Compare)
    {
    }

    public PwrKeySortedStack(IEnumerable<T> coll, Func<T, K> keySelector, IComparer<K> keyComparer)
      : this((coll != null ? coll.PeekCount() : -1).WithCase(c => c >= 0, c => new PwrList<T>(c), c => new PwrList<T>()),
          keySelector, (keyComparer ?? Comparer<K>.Default).Compare)
    {
      if (coll != null)
        PushRange(coll);
    }

    protected PwrKeySortedStack(IList<T> innerList, Func<T, K> keySelector, Comparison<K> keyComparison)
			: base(innerList, new SelectComparer<T, K>(keySelector, keyComparison).Compare)
		{
			if (keySelector == null)
				throw new ArgumentNullException("keySelector");
			if (keyComparison == null)
				throw new ArgumentNullException("keyComparison");

			_keySelector = keySelector;
			_keyComparison = keyComparison;
		}

		#endregion
		#region Instance properties
		#region Internal properties

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
    #region Public methods

    public bool ContainsKey(K key)
    {
      return InnerList.FindLastIndex(t => EqualityComparer<K>.Default.Equals(KeySelector(t), key)) >= 0;
    }

    public T Pop(K key)
    {
      if (InnerList.Count == 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsEmpty]);

      var index = InnerList.BinarySearch(t => KeyComparison(KeySelector(t), key), SortingOption.Last);
      if (index < 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.NoElementMatchedInPredicate]);
      var value = InnerList[index];
      InnerList.RemoveAt(index);
      return value;
    }

    public PwrList<T> PopRange(K key, int count)
    {
      if (count < 0 || count > InnerList.Count)
        throw new ArgumentOutOfRangeException("count");

      var list = new PwrList<T>(count);
      var index = InnerList.BinarySearch(t => KeyComparison(KeySelector(t), key), SortingOption.Last);
      while (index >= 0 && KeyComparison(KeySelector(InnerList[index]), key) == 0 && count-- > 0)
      {
        list.Add(InnerList[index]);
        InnerList.RemoveAt(index--);
      }
      return list;
    }

    public PwrList<T> PopAll(K key)
    {
      return PopRange(key, InnerList.Count);
    }

    #endregion
    #endregion
  }
}
