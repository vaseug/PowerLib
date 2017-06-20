using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PowerLib.System.Collections;
using PowerLib.System.Collections.Matching;

namespace PowerLib.System.Linq
{
  public static class PwrEnumerable
  {
    #region Collection methods

    public static int ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (action == null)
        throw new ArgumentNullException("action");
      //
      using (IEnumerator<TSource> e = source.GetEnumerator())
      {
        int i = 0;
        while (e.MoveNext())
        {
          try
          {
            action(e.Current);
          }
          catch (Exception ex)
          {
            throw new ArgumentCollectionElementException("source", ex, i);
          }
          i++;
        }
        return i;
      }
    }

    public static int ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (action == null)
        throw new ArgumentNullException("action");
      //
      using (IEnumerator<TSource> e = source.GetEnumerator())
      {
        int i = 0;
        while (e.MoveNext())
        {
          try
          {
            action(e.Current, i);
          }
          catch (Exception ex)
          {
            throw new ArgumentCollectionElementException("source", ex, i);
          }
          i++;
        }
        return i;
      }
    }

    public static IEnumerable<TSource> Apply<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (action == null)
        throw new ArgumentNullException("action");
      //
      int i = 0;
      using (IEnumerator<TSource> e = source.GetEnumerator())
      {
        while (e.MoveNext())
        {
          try
          {
            action(e.Current);
          }
          catch (Exception ex)
          {
            throw new ArgumentCollectionElementException("source", ex, i);
          }
          yield return e.Current;
          i++;
        }
      }
    }

    public static IEnumerable<TSource> Apply<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (action == null)
        throw new ArgumentNullException("action");
      //
      int i = 0;
      using (IEnumerator<TSource> e = source.GetEnumerator())
      {
        while (e.MoveNext())
        {
          try
          {
            action(e.Current, i);
          }
          catch (Exception ex)
          {
            throw new ArgumentCollectionElementException("source", ex, i);
          }
          yield return e.Current;
          i++;
        }
      }
    }

    public static IEnumerable<TSource> Produce<TSource>(int count, Func<TSource> producer)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      if (producer == null)
        throw new ArgumentNullException("producer");

      while (count-- > 0)
        yield return producer();
    }

    public static IEnumerable<TSource> Produce<TSource>(int count, Func<int, TSource> producer)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      if (producer == null)
        throw new ArgumentNullException("producer");

      for (int i = 0; i < count; i++)
        yield return producer(i);
    }

    public static T Case<K, T>(this IEnumerable<KeyValuePair<K, T>> coll, K key, T defaultValue)
    {
      IDictionary<K, T> dic = coll as IDictionary<K, T>;
      if (dic == null)
      {
        dic = new Dictionary<K, T>();
        dic.AddRange(coll);
      }
      T resultValue;
      return dic.TryGetValue(key, out resultValue) ? resultValue : defaultValue;
    }

    #endregion
    #region Sort methods

    public static TList ToSortedList<TSource, TList>(this IEnumerable<TSource> source, Func<int, TList> factory, Comparison<TSource> comparison, SortingOption sortingOption = SortingOption.None)
      where TList : IList<TSource>
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparison == null)
        throw new ArgumentNullException("comparison");
      if (factory == null)
        throw new ArgumentNullException("factory");

      int capacity = 0;
      ICollection<TSource> coll = source as ICollection<TSource>;
      if (coll != null)
        capacity = coll.Count;
      else
      {
        IReadOnlyCollection<TSource> roColl = source as IReadOnlyCollection<TSource>;
        if (roColl != null)
          capacity = roColl.Count;
      }
      TList list = factory(capacity);
      int index = 0;
      foreach (var item in coll)
        if (list.AddSorted(item, comparison, sortingOption) < 0)
          throw new ArgumentCollectionElementException("source", "Duplicate item found.", index);
      return list;
    }

    public static List<TSource> ToSortedList<TSource>(this IEnumerable<TSource> source, Comparison<TSource> comparison, SortingOption sortingOption = SortingOption.None)
    {
      return source.ToSortedList(capacity => new List<TSource>(capacity), comparison, sortingOption);
    }

    public static PwrList<TSource> ToSortedPwrList<TSource>(this IEnumerable<TSource> source, Comparison<TSource> comparison, SortingOption sortingOption = SortingOption.None)
    {
      return source.ToSortedList(capacity => new PwrList<TSource>(capacity), comparison, sortingOption);
    }

    public static IEnumerable<TSource> Sort<TSource>(this IEnumerable<TSource> source, SortingOption sortingOption = SortingOption.None)
    {
      foreach (var item in source.ToSortedList(Comparer<TSource>.Default.Compare, sortingOption))
        yield return item;
    }

    public static IEnumerable<TSource> Sort<TSource>(this IEnumerable<TSource> source, Comparison<TSource> comparison, SortingOption sortingOption = SortingOption.None)
    {
      foreach (var item in source.ToSortedList(comparison, sortingOption))
        yield return item;
    }

    public static IEnumerable<TSource> Sort<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer, SortingOption sortingOption = SortingOption.None)
    {
      if (comparer == null)
        throw new ArgumentNullException("comparer");
      foreach (var item in source.ToSortedList(comparer.Compare, sortingOption))
        yield return item;
    }

    #endregion
    #region Compare methods

    private static int SequenceCompareCore<T>(IEnumerable<T> source, IEnumerable<T> other, Comparison<T> comparison)
    {
      using (IEnumerator<T> es = source.GetEnumerator())
      using (IEnumerator<T> eo = other.GetEnumerator())
      {
        int result;
        bool fs, fo;
        do
        {
          fs = es.MoveNext();
          fo = eo.MoveNext();
          result = fs ? fo ? comparison(es.Current, eo.Current) : 1 : fo ? -1 : 0;
        }
        while (fs && fo && result == 0);
        return result;
      }
    }

		public static int SequenceCompare<T>(this IEnumerable<T> source, IEnumerable<T> other)
		{
      if (source == null)
        throw new ArgumentNullException("source");
      if (other == null)
        throw new ArgumentNullException("other");
      //
      return SequenceCompareCore(source, other, Comparer<T>.Default.Compare);
		}

    public static int SequenceCompare<T>(this IEnumerable<T> source, IEnumerable<T> other, IComparer<T> comparer)
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (other == null)
        throw new ArgumentNullException("other");
      if (comparer == null)
        throw new ArgumentNullException("comparer");
      //
      return SequenceCompareCore(source, other, comparer.Compare);
    }

		public static int SequenceCompare<T>(this IEnumerable<T> source, IEnumerable<T> other, Comparison<T> comparison)
		{
			if (source == null)
        throw new ArgumentNullException("source");
			if (other == null)
        throw new ArgumentNullException("other");
			if (comparison == null)
        throw new ArgumentNullException("comparison");
      //
      return SequenceCompareCore(source, other, comparison);
		}

		#endregion
		#region Aggregate methods
    #region Bound

    private static T BoundCore<T>(IEnumerable<T> source, Comparison<T> comparison, bool bound)
    {
      using (IEnumerator<T> e = source.GetEnumerator())
      {
        if (!e.MoveNext())
          throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.SourceCollectionIsEmpty]);
        T value = e.Current;
        while (e.MoveNext())
          if (!bound && comparison(value, e.Current) < 0 || bound && comparison(value, e.Current) > 0)
            value = e.Current;
        return value ;
      }
    }

    public static T Bound<T>(this IEnumerable<T> source, bool bound)
      where T : IComparable<T>
    {
      if (source == null)
        throw new ArgumentNullException("source");

      return BoundCore(source, ((IComparer<T>)ComparableComparer<T>.Default).Compare, bound);
    }

    public static T? Bound<T>(this IEnumerable<T?> source, bool nullOrder, bool bound)
      where T : struct, IComparable<T>
    {
      if (source == null)
        throw new ArgumentNullException("source");

      return BoundCore(source, ((IComparer<T?>)new NullableComparer<T>(ComparableComparer<T>.Default, nullOrder)).Compare, bound);
    }

    public static T Bound<T>(this IEnumerable<T> source, bool nullOrder, bool bound)
      where T : class, IComparable<T>
    {
      if (source == null)
        throw new ArgumentNullException("source");

      return BoundCore(source, ((IComparer<T>)new ObjectComparer<T>(ComparableComparer<T>.Default, nullOrder)).Compare, bound);
    }

    public static T Bound<T>(this IEnumerable<T> source, Comparison<T> comparison, bool bound)
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      return BoundCore(source, comparison, bound);
    }

    public static T Bound<T>(this IEnumerable<T> source, IComparer<T> comparer, bool bound)
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparer == null)
        throw new ArgumentNullException("comparer");

      return BoundCore(source, comparer.Compare, bound);
    }

    public static T? Bound<T>(this IEnumerable<T?> source, Comparison<T> comparison, bool nullOrder, bool bound)
      where T : struct
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      return BoundCore(source, ((IComparer<T?>)new NullableComparer<T>(comparison, nullOrder)).Compare, bound);
    }

    public static T? Bound<T>(this IEnumerable<T?> source, IComparer<T> comparer, bool nullOrder, bool bound)
      where T : struct
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparer == null)
        throw new ArgumentNullException("comparer");

      return BoundCore(source, ((IComparer<T?>)new NullableComparer<T>(comparer, nullOrder)).Compare, bound);
    }

    public static T Bound<T>(this IEnumerable<T> source, Comparison<T> comparison, bool nullOrder, bool bound)
      where T : class
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      return BoundCore(source, ((IComparer<T>)new ObjectComparer<T>(comparison, nullOrder)).Compare, bound);
    }

    public static T Bound<T>(this IEnumerable<T> source, IComparer<T> comparer, bool nullOrder, bool bound)
      where T : class
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparer == null)
        throw new ArgumentNullException("comparer");

      return BoundCore(source, ((IComparer<T>)new ObjectComparer<T>(comparer, nullOrder)).Compare, bound);
    }

    #endregion
    #region Maximum

		public static T Max<T>(this IEnumerable<T> source)
			where T : IComparable<T>
		{
      if (source == null)
        throw new ArgumentNullException("source");

      return BoundCore(source, ((IComparer<T>)ComparableComparer<T>.Default).Compare, false);
    }

		public static T? Max<T>(this IEnumerable<T?> source, bool nullOrder)
			where T : struct, IComparable<T>
		{
      if (source == null)
        throw new ArgumentNullException("source");

      return BoundCore(source, ((IComparer<T?>)new NullableComparer<T>(ComparableComparer<T>.Default, nullOrder)).Compare, false);
    }

    public static T Max<T>(this IEnumerable<T> source, bool nullOrder)
      where T : class, IComparable<T>
    {
      if (source == null)
        throw new ArgumentNullException("source");

      return BoundCore(source, ((IComparer<T>)new ObjectComparer<T>(ComparableComparer<T>.Default, nullOrder)).Compare, false);
    }

		public static T Max<T>(this IEnumerable<T> source, Comparison<T> comparison)
		{
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      return BoundCore(source, comparison, false);
    }

		public static T Max<T>(this IEnumerable<T> source, IComparer<T> comparer)
		{
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparer == null)
        throw new ArgumentNullException("comparer");

      return BoundCore(source, comparer.Compare, false);
    }

    public static T? Max<T>(this IEnumerable<T?> source, Comparison<T> comparison, bool nullOrder)
      where T : struct
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      return BoundCore(source, ((IComparer<T?>)new NullableComparer<T>(comparison, nullOrder)).Compare, false);
    }

    public static T? Max<T>(this IEnumerable<T?> source, IComparer<T> comparer, bool nullOrder)
      where T : struct
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparer == null)
        throw new ArgumentNullException("comparer");

      return BoundCore(source, ((IComparer<T?>)new NullableComparer<T>(comparer, nullOrder)).Compare, false);
    }

		public static T Max<T>(this IEnumerable<T> source, Comparison<T> comparison, bool nullOrder)
			where T : class
		{
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      return BoundCore(source, ((IComparer<T>)new ObjectComparer<T>(comparison, nullOrder)).Compare, false);
    }

		public static T Max<T>(this IEnumerable<T> source, IComparer<T> comparer, bool nullOrder)
			where T : class
		{
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparer == null)
        throw new ArgumentNullException("comparer");

      return BoundCore(source, ((IComparer<T>)new ObjectComparer<T>(comparer, nullOrder)).Compare, false);
    }

		#endregion
		#region Minimum

		public static T Min<T>(this IEnumerable<T> source)
			where T : IComparable<T>
		{
      if (source == null)
        throw new ArgumentNullException("source");

      return BoundCore(source, ((IComparer<T>)ComparableComparer<T>.Default).Compare, true);
    }

		public static T? Min<T>(this IEnumerable<T?> source, bool nullOrder)
			where T : struct, IComparable<T>
		{
      if (source == null)
        throw new ArgumentNullException("source");

      return BoundCore(source, ((IComparer<T?>)new NullableComparer<T>(ComparableComparer<T>.Default, nullOrder)).Compare, true);
    }

    public static T Min<T>(this IEnumerable<T> source, bool nullOrder)
      where T : class, IComparable<T>
    {
      if (source == null)
        throw new ArgumentNullException("source");

      return BoundCore(source, ((IComparer<T>)new ObjectComparer<T>(ComparableComparer<T>.Default, nullOrder)).Compare, true);
    }

		public static T Min<T>(this IEnumerable<T> source, Comparison<T> comparison)
		{
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      return BoundCore(source, comparison, true);
    }

		public static T Min<T>(this IEnumerable<T> source, IComparer<T> comparer)
		{
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparer == null)
        throw new ArgumentNullException("comparer");

      return BoundCore(source, comparer.Compare, true);
    }

    public static T? Min<T>(this IEnumerable<T?> source, Comparison<T> comparison, bool nullOrder)
      where T : struct
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      return BoundCore(source, ((IComparer<T?>)new NullableComparer<T>(comparison, nullOrder)).Compare, true);
    }

    public static T? Min<T>(this IEnumerable<T?> source, IComparer<T> comparer, bool nullOrder)
      where T : struct
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparer == null)
        throw new ArgumentNullException("comparer");

      return BoundCore(source, ((IComparer<T?>)new NullableComparer<T>(comparer, nullOrder)).Compare, true);
    }

		public static T Min<T>(this IEnumerable<T> source, Comparison<T> comparison, bool nullOrder)
			where T : class
		{
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      return BoundCore(source, ((IComparer<T>)new ObjectComparer<T>(comparison, nullOrder)).Compare, true);
    }

		public static T Min<T>(this IEnumerable<T> source, IComparer<T> comparer, bool nullOrder)
			where T : class
		{
      if (source == null)
        throw new ArgumentNullException("source");
      if (comparer == null)
        throw new ArgumentNullException("comparer");

      return BoundCore(source, ((IComparer<T>)new ObjectComparer<T>(comparer, nullOrder)).Compare, false);
    }

		#endregion
		#endregion
		#region Hierarchical object extensions

    private static IEnumerable<T> AscendantsCore<T, K>(IEnumerable<T> source, Func<T, K> keySelector, Func<T, K?> parentKeySelector, Equality<K> equality, K startKey, bool ascendantsOnly = false, int? levelsCount = null)
      where T : class
      where K : struct
    {
      PwrList<T> list = new PwrList<T>();
      T item = source.SingleOrDefault(t => equality(keySelector(t), startKey));
      if (item != null && ascendantsOnly)
      {
        K? parentKey = parentKeySelector(item);
        item = parentKey.HasValue? source.SingleOrDefault(t => equality(keySelector(t), parentKey.Value)) : null;
      }
      if (item != null)
        list.Add(item);
      for (int passCount = 1; (!levelsCount.HasValue|| levelsCount.Value> passCount) && item != null; passCount++)
      {
        K? parentKey = parentKeySelector(item);
        item = parentKey.HasValue? source.SingleOrDefault(t => equality(keySelector(t), parentKey.Value)) : null;
        if (item != null)
          list.Add(item);
      }
      return list.AsEnumerable();
    }

    private static IEnumerable<T> AscendantsCore<T, K>(IEnumerable<T> source, Func<T, K> keySelector, Func<T, K> parentKeySelector, Equality<K> equality, K startKey, bool ascendantsOnly = false, int? levelsCount = null)
      where T : class
      where K : class
    {
      PwrList<T> list = new PwrList<T>();
      T item = source.SingleOrDefault(t => equality(keySelector(t), startKey));
      if (item != null && ascendantsOnly)
      {
        K parentKey = parentKeySelector(item);
        item = parentKey != null ? source.SingleOrDefault(t => equality(keySelector(t), parentKey)) : null;
      }
      if (item != null)
        list.Add(item);
      for (int passCount = 1; (!levelsCount.HasValue|| levelsCount.Value> passCount) && item != null; passCount++)
      {
        K parentKey = parentKeySelector(item);
        item = parentKey != null ? source.SingleOrDefault(t => equality(keySelector(t), parentKey)) : null;
        if (item != null)
          list.Add(item);
      }
      return list.AsEnumerable();
    }

    private static IEnumerable<T> DescendantsCore<T, K>(IEnumerable<T> source, Func<T, K> keySelector, Func<T, K?> parentKeySelector, Equality<K> equality, K? startKey, bool descendantsOnly = false, int? levelsCount = null)
      where T : class
      where K : struct
    {
      PwrList<T> list = new PwrList<T>();
      PwrFrameListView<T> view = new PwrFrameListView<T>(list);
      int priorCount = list.Count;
      if (descendantsOnly)
        list.AddRange(source.Where(t => !parentKeySelector(t).HasValue&& !startKey.HasValue|| parentKeySelector(t).HasValue&& startKey.HasValue&& equality(parentKeySelector(t).Value, startKey.Value)));
      else if (startKey.HasValue)
        list.AddRange(source.Where(t => equality(keySelector(t), startKey.Value)));
      int takeCount = list.Count - priorCount;
      for (int passCount = 1; (!levelsCount.HasValue|| levelsCount.Value> passCount) && takeCount > 0; passCount++)
      {
        view.FrameOffset = priorCount;
        view.FrameSize = takeCount;
        priorCount = list.Count;
        list.AddRange(source.Where(t1 => parentKeySelector(t1).HasValue&& view.Any(t2 => equality(keySelector(t2), parentKeySelector(t1).Value))));
        takeCount = list.Count - priorCount;
      }
      return list.AsEnumerable();
    }

    private static IEnumerable<T> DescendantsCore<T, K>(IEnumerable<T> source, Func<T, K> keySelector, Func<T, K> parentKeySelector, Equality<K> equality, K startKey, bool descendantsOnly = false, int? levelsCount = null)
      where T : class
      where K : class
    {
      PwrList<T> list = new PwrList<T>();
      PwrFrameListView<T> view = new PwrFrameListView<T>(list);
      int priorCount = list.Count;
      if (descendantsOnly)
        list.AddRange(source.Where(t => equality(parentKeySelector(t), startKey)));
      else if (startKey != null)
        list.AddRange(source.Where(t => equality(keySelector(t), startKey)));
      int takeCount = list.Count - priorCount;
      for (int passCount = 1; (!levelsCount.HasValue|| levelsCount.Value> passCount) && takeCount > 0; passCount++)
      {
        view.FrameOffset = priorCount;
        view.FrameSize = takeCount;
        priorCount = list.Count;
        list.AddRange(source.Where(t1 => parentKeySelector(t1) != null && view.Any(t2 => equality(keySelector(t2), parentKeySelector(t1)))));
        takeCount = list.Count - priorCount;
      }
      return list.AsEnumerable();
    }

		public static IEnumerable<T> Ascendants<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K?> parentKeySelector, K startKey, bool ascendantsOnly = false, int? levelsCount = null)
			where T : class
			where K : struct
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (keySelector == null)
				throw new ArgumentNullException("keySelector");
			if (parentKeySelector == null)
				throw new ArgumentNullException("parentKeySelector");
			if (levelsCount.HasValue)
				if (levelsCount.Value< 0)
					throw new ArgumentOutOfRangeException("levelsCount");
				else if (levelsCount.Value== 0)
					return Enumerable.Empty<T>();

      return AscendantsCore(source, keySelector, parentKeySelector, (x, y) => x.Equals(y), startKey, ascendantsOnly, levelsCount);
		}

		public static IEnumerable<T> Ascendants<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K> parentKeySelector, K startKey, bool ascendantsOnly = false, int? levelsCount = null)
			where T : class
			where K : class
		{
      if (source == null)
        throw new ArgumentNullException("source");
      if (keySelector == null)
        throw new ArgumentNullException("keySelector");
      if (parentKeySelector == null)
        throw new ArgumentNullException("parentKeySelector");
      if (levelsCount.HasValue)
        if (levelsCount.Value< 0)
          throw new ArgumentOutOfRangeException("levelsCount");
        else if (levelsCount.Value== 0)
          return Enumerable.Empty<T>();

      return AscendantsCore(source, keySelector, parentKeySelector, EqualityComparer<K>.Default.Equals, startKey, ascendantsOnly, levelsCount);
    }

		public static IEnumerable<T> Descendants<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K?> parentKeySelector, K? startKey, bool descendantsOnly = false, int? levelsCount = null)
			where T : class
			where K : struct
		{
      if (source == null)
        throw new ArgumentNullException("source");
      if (keySelector == null)
				throw new ArgumentNullException("keySelector");
			if (parentKeySelector == null)
				throw new ArgumentNullException("parentKeySelector");
			if (levelsCount.HasValue)
				if (levelsCount.Value< 0)
					throw new ArgumentOutOfRangeException("levelsCount");
				else if (levelsCount.Value== 0)
					return Enumerable.Empty<T>();

      return DescendantsCore(source, keySelector, parentKeySelector, (x, y) => x.Equals(y), startKey, descendantsOnly, levelsCount);
		}

		public static IEnumerable<T> Descendants<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K> parentKeySelector, K startKey, bool descendantsOnly = false, int? levelsCount = null)
			where T : class
			where K : class
		{
      if (source == null)
        throw new ArgumentNullException("source");
      if (keySelector == null)
				throw new ArgumentNullException("keySelector");
			if (parentKeySelector == null)
				throw new ArgumentNullException("parentKeySelector");
			if (levelsCount.HasValue)
				if (levelsCount.Value< 0)
					throw new ArgumentOutOfRangeException("levelsCount");
				else if (levelsCount.Value== 0)
					return Enumerable.Empty<T>();

      return DescendantsCore(source, keySelector, parentKeySelector, EqualityComparer<K>.Default.Equals, startKey, descendantsOnly, levelsCount);
		}

    public static IEnumerable<T> Ascendants<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K?> parentKeySelector, Equality<K> equality, K startKey, bool ascendantsOnly = false, int? levelsCount = null)
      where T : class
      where K : struct
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (keySelector == null)
        throw new ArgumentNullException("keySelector");
      if (parentKeySelector == null)
        throw new ArgumentNullException("parentKeySelector");
      if (equality == null)
        throw new ArgumentNullException("equality");
      if (levelsCount.HasValue)
        if (levelsCount.Value< 0)
          throw new ArgumentOutOfRangeException("levelsCount");
        else if (levelsCount.Value== 0)
          return Enumerable.Empty<T>();

      return AscendantsCore(source, keySelector, parentKeySelector, equality, startKey, ascendantsOnly, levelsCount);
    }

    public static IEnumerable<T> Ascendants<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K> parentKeySelector, Equality<K> equality, K startKey, bool ascendantsOnly = false, int? levelsCount = null)
      where T : class
      where K : class
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (keySelector == null)
        throw new ArgumentNullException("keySelector");
      if (parentKeySelector == null)
        throw new ArgumentNullException("parentKeySelector");
      if (equality == null)
        throw new ArgumentNullException("equality");
      if (levelsCount.HasValue)
        if (levelsCount.Value< 0)
          throw new ArgumentOutOfRangeException("levelsCount");
        else if (levelsCount.Value== 0)
          return Enumerable.Empty<T>();

      return AscendantsCore(source, keySelector, parentKeySelector, equality, startKey, ascendantsOnly, levelsCount);
    }

    public static IEnumerable<T> Descendants<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K?> parentKeySelector, Equality<K> equality, K? startKey, bool descendantsOnly = false, int? levelsCount = null)
      where T : class
      where K : struct
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (keySelector == null)
        throw new ArgumentNullException("keySelector");
      if (parentKeySelector == null)
        throw new ArgumentNullException("parentKeySelector");
      if (equality == null)
        throw new ArgumentNullException("equality");
      if (levelsCount.HasValue)
        if (levelsCount.Value< 0)
          throw new ArgumentOutOfRangeException("levelsCount");
        else if (levelsCount.Value== 0)
          return Enumerable.Empty<T>();

      return DescendantsCore(source, keySelector, parentKeySelector, equality, startKey, descendantsOnly, levelsCount);
    }

    public static IEnumerable<T> Descendants<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K> parentKeySelector, Equality<K> equality, K startKey, bool descendantsOnly = false, int? levelsCount = null)
      where T : class
      where K : class
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (keySelector == null)
        throw new ArgumentNullException("keySelector");
      if (parentKeySelector == null)
        throw new ArgumentNullException("parentKeySelector");
      if (equality == null)
        throw new ArgumentNullException("equality");
      if (levelsCount.HasValue)
        if (levelsCount.Value< 0)
          throw new ArgumentOutOfRangeException("levelsCount");
        else if (levelsCount.Value== 0)
          return Enumerable.Empty<T>();

      return DescendantsCore(source, keySelector, parentKeySelector, equality, startKey, descendantsOnly, levelsCount);
    }

    public static IEnumerable<T> Ascendants<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K?> parentKeySelector, IEqualityComparer<K> equalityComparer, K startKey, bool ascendantsOnly = false, int? levelsCount = null)
      where T : class
      where K : struct
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (keySelector == null)
        throw new ArgumentNullException("keySelector");
      if (parentKeySelector == null)
        throw new ArgumentNullException("parentKeySelector");
      if (equalityComparer == null)
        throw new ArgumentNullException("equalityComparer");
      if (levelsCount.HasValue)
        if (levelsCount.Value< 0)
          throw new ArgumentOutOfRangeException("levelsCount");
        else if (levelsCount.Value== 0)
          return Enumerable.Empty<T>();

      return AscendantsCore(source, keySelector, parentKeySelector, equalityComparer.Equals, startKey, ascendantsOnly, levelsCount);
    }

    public static IEnumerable<T> Ascendants<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K> parentKeySelector, IEqualityComparer<K> equalityComparer, K startKey, bool ascendantsOnly = false, int? levelsCount = null)
      where T : class
      where K : class
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (keySelector == null)
        throw new ArgumentNullException("keySelector");
      if (parentKeySelector == null)
        throw new ArgumentNullException("parentKeySelector");
      if (equalityComparer == null)
        throw new ArgumentNullException("equalityComparer");
      if (levelsCount.HasValue)
        if (levelsCount.Value< 0)
          throw new ArgumentOutOfRangeException("levelsCount");
        else if (levelsCount.Value== 0)
          return Enumerable.Empty<T>();

      return AscendantsCore(source, keySelector, parentKeySelector, equalityComparer.Equals, startKey, ascendantsOnly, levelsCount);
    }

    public static IEnumerable<T> Descendants<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K?> parentKeySelector, IEqualityComparer<K> equalityComparer, K? startKey, bool descendantsOnly = false, int? levelsCount = null)
      where T : class
      where K : struct
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (keySelector == null)
        throw new ArgumentNullException("keySelector");
      if (parentKeySelector == null)
        throw new ArgumentNullException("parentKeySelector");
      if (equalityComparer == null)
        throw new ArgumentNullException("equalityComparer");
      if (levelsCount.HasValue)
        if (levelsCount.Value< 0)
          throw new ArgumentOutOfRangeException("levelsCount");
        else if (levelsCount.Value== 0)
          return Enumerable.Empty<T>();

      return DescendantsCore(source, keySelector, parentKeySelector, equalityComparer.Equals, startKey, descendantsOnly, levelsCount);
    }

    public static IEnumerable<T> Descendants<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K> parentKeySelector, IEqualityComparer<K> equalityComparer, K startKey, bool descendantsOnly = false, int? levelsCount = null)
      where T : class
      where K : class
    {
      if (source == null)
        throw new ArgumentNullException("source");
      if (keySelector == null)
        throw new ArgumentNullException("keySelector");
      if (parentKeySelector == null)
        throw new ArgumentNullException("parentKeySelector");
      if (equalityComparer == null)
        throw new ArgumentNullException("equalityComparer");
      if (levelsCount.HasValue)
        if (levelsCount.Value< 0)
          throw new ArgumentOutOfRangeException("levelsCount");
        else if (levelsCount.Value== 0)
          return Enumerable.Empty<T>();

      return DescendantsCore(source, keySelector, parentKeySelector, equalityComparer.Equals, startKey, descendantsOnly, levelsCount);
    }

    #endregion
	}
}
