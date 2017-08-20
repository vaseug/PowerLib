using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerLib.System.Collections
{
	public static class ListExtension
	{
    #region Manipulation methods

    public static void AddRepeat<T>(this IList<T> list, T value, int count)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (count < 0 || count > int.MaxValue - list.Count)
        throw new ArgumentOutOfRangeException("count");

      while (count-- > 0)
        list.Add(value);
    }

    public static void InsertRepeat<T>(this IList<T> list, int index, T value, int count)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (index < 0 || index > list.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > int.MaxValue - index)
        throw new ArgumentOutOfRangeException("count");

      while (count-- > 0)
        list.Insert(index++, value);
    }

    public static void SetRepeat<T>(this IList<T> list, int index, T value, int count)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (index < 0 || index > list.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > list.Count - index)
        throw new ArgumentOutOfRangeException("count");

      while (count-- > 0)
        list[index++] = value;
    }

    public static void AddRange<T>(this IList<T> list, IEnumerable<T> coll)
		{
			if (list == null)
				throw new ArgumentNullException("list");
      if (coll == null)
        throw new ArgumentNullException("coll");

			if (list is PwrList<T>)
				((PwrList<T>)list).AddRange(coll);
			else if (list is List<T>)
				((List<T>)list).AddRange(coll);
			else
			{
        int count = coll.PeekCount();
				if (count >= 0 && list is ICapacitySupport)
					((ICapacitySupport)list).Capacity = list.Count + count;
				using (var e = coll.GetEnumerator())
					while (e.MoveNext())
						list.Add(e.Current);
			}
		}

		public static void InsertRange<T>(this IList<T> list, int index, IEnumerable<T> coll)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).InsertRange(index, coll);
			else if (list is global::System.Collections.Generic.List<T>)
				((global::System.Collections.Generic.List<T>)list).InsertRange(index, coll);
			else
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("index");
				if (coll == null)
					throw new ArgumentNullException("source");

				using (IEnumerator<T> e = coll.GetEnumerator())
				{
					while (e.MoveNext())
						list.Insert(index++, e.Current);
				}
			}
		}

		public static void RemoveRange<T>(this IList<T> list, int index, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).RemoveRange(index, count);
			else if (list is global::System.Collections.Generic.List<T>)
				((global::System.Collections.Generic.List<T>)list).RemoveRange(index, count);
			else
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("index");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				//
				for (int i = index + count - 1; i >= index; i--)
					list.RemoveAt(i);
			}
		}

		public static void ReplaceRange<T>(this IList<T> list, int index, int count, IEnumerable<T> coll)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (coll == null)
				throw new ArgumentNullException("source");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");

			using (var e = coll.GetEnumerator())
      {
				while (count > 0 && e.MoveNext())
				{
					list[index++] = e.Current;
					count--;
				}
				if (count > 0)
					RemoveRange(list, index, count);
				else
					while (e.MoveNext())
						list.Insert(index++, e.Current);
      }
		}

    public static void SetRange<T>(this IList<T> list, int index, IEnumerable<T> coll)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (coll == null)
				throw new ArgumentNullException("source");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("index");
      if (coll.Count() > list.Count - index)
        throw new ArgumentOutOfRangeException("coll");

      using (var e = coll.GetEnumerator())
        while (e.MoveNext())
          list[index++] = e.Current;
		}

    public static void Move<T>(this IList<T> list, int srcIndex, int dstIndex)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (srcIndex < 0 || srcIndex >= list.Count)
				throw new ArgumentOutOfRangeException("srcIndex");
			if (dstIndex < 0 || dstIndex >= list.Count)
				throw new ArgumentOutOfRangeException("dstIndex");

      if (srcIndex == dstIndex)
        return;
      T value = list[srcIndex];
      if (srcIndex < dstIndex)
        for (int i = srcIndex; i < dstIndex; i++)
          list[i] = list[i + 1];
      else if (srcIndex > dstIndex)
        for (int i = srcIndex - 1; i >= dstIndex; i--)
          list[i + 1] = list[i];
      list[dstIndex] = value;
    }

		public static void MoveRange<T>(this IList<T> list, int srcIndex, int dstIndex, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (srcIndex < 0 || srcIndex > list.Count)
				throw new ArgumentOutOfRangeException("srcIndex");
			if (dstIndex < 0 || dstIndex > list.Count)
				throw new ArgumentOutOfRangeException("dstIndex");
			if (count < 0 || count > list.Count - srcIndex || count > list.Count - dstIndex)
				throw new ArgumentOutOfRangeException("count");

      if (srcIndex == dstIndex)
        return;
      T[] values = new T[count];
      CopyTo(list, values, 0, srcIndex, count);
      if (srcIndex < dstIndex)
        for (int i = srcIndex; i < dstIndex; i++)
          list[i] = list[i + count];
      else if (srcIndex > dstIndex)
        for (int i = srcIndex - 1; i >= dstIndex; i--)
          list[i + count] = list[i];
      for (int i = 0; i < count; i++)
        list[dstIndex + i] = values[i];
    }

		public static void Swap<T>(this IList<T> list, int xIndex, int yIndex)
		{
			if (list == null)
				throw new ArgumentNullException("list");
		  if (xIndex < 0 || xIndex >= list.Count)
			  throw new ArgumentOutOfRangeException("xIndex");
			if (yIndex < 0 || yIndex >= list.Count)
				throw new ArgumentOutOfRangeException("yIndex");
			if (xIndex == yIndex)
				return;

      T value = list[xIndex];
      list[xIndex] = list[yIndex];
      list[yIndex] = value;
    }

		public static void SwapRange<T>(this IList<T> list, int xIndex, int yIndex, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (xIndex < 0 || xIndex > list.Count)
				throw new ArgumentOutOfRangeException("xIndex");
			if (yIndex < 0 || yIndex > list.Count)
				throw new ArgumentOutOfRangeException("yIndex");
			if (count < 0 || count > list.Count - xIndex || count > list.Count - yIndex)
				throw new ArgumentOutOfRangeException("count");
			if (count > global::System.Math.Abs(yIndex - xIndex))
				throw new ArgumentException("Ranges intersected");

      for (int i = 0; i < count; i++)
      {
        T value = list[xIndex + i];
        list[xIndex + i] = list[yIndex + i];
        list[yIndex + i] = value;
      }
    }

		public static void SwapRanges<T>(this IList<T> list, int xIndex, int xCount, int yIndex, int yCount)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (xIndex < 0 || xIndex > list.Count)
				throw new ArgumentOutOfRangeException("xIndex");
			if (yIndex < 0 || yIndex > list.Count)
				throw new ArgumentOutOfRangeException("yIndex");
			if (xCount < 0 || xCount > list.Count - xIndex)
				throw new ArgumentOutOfRangeException("xCount");
			if (yCount < 0 || yCount > list.Count - yIndex)
				throw new ArgumentOutOfRangeException("yCount");
			if (xIndex == yIndex || xIndex < yIndex && xIndex + xCount > yIndex || xIndex > yIndex && yIndex + yCount > xIndex)
				throw new ArgumentException("Ranges intersected");

      for (int i = 0; i < Comparable.Min(xCount, yCount); i++)
      {
        T temp = list[xIndex + i];
        list[xIndex + i] = list[yIndex + i];
        list[yIndex + i] = temp;
      }
      if (xCount == yCount)
        return;
      int lowerIndex, lowerCount, upperIndex, upperCount;
      if (xIndex < yIndex)
      {
        lowerIndex = xIndex;
        lowerCount = xCount;
        upperIndex = yIndex;
        upperCount = yCount;
      }
      else
      {
        lowerIndex = yIndex;
        lowerCount = yCount;
        upperIndex = xIndex;
        upperCount = xCount;
      }
      if (lowerCount > upperCount)
      {
        T[] buffer = new T[lowerCount - upperCount];
        for (int i = 0, c = lowerCount - upperCount, s = lowerIndex + upperCount; c < c; i++)
          buffer[i] = list[s + i];
        for (int i = 0, c = upperIndex + upperCount - (lowerIndex + lowerCount),
          s = lowerIndex + lowerCount, d = lowerIndex + upperCount; i < c; i++)
          list[d + i] = list[s + i];
        for (int i = 0, c = lowerCount - upperCount, d = upperIndex + upperCount - (lowerCount - upperCount); i < c; i++)
          list[d + i] = buffer[i];
      }
      else if (lowerCount < upperCount)
      {
        T[] buffer = new T[upperCount - lowerCount];
        for (int i = 0, c = upperCount - lowerCount, s = upperIndex + lowerCount; i < c; i++)
          buffer[i] = list[s + i];
        for (int i = upperIndex - lowerIndex - 1,
          s = lowerIndex + lowerCount, d = lowerIndex + upperCount; i >= 0; i--)
          list[d + i] = list[s + i];
        for (int i = 0, c = upperCount - lowerCount, d = lowerIndex + lowerCount; i < c; i++)
          buffer[i] = list[d + i];
      }
    }

    public static void Reverse<T>(this IList<T> list)
    {
      Reverse(list, 0, list != null ? list.Count : 0);
    }

		public static void Reverse<T>(this IList<T> list, int index, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");

      for (int i = 0; i < count / 2; i++)
      {
        T value = list[index + i];
        list[index + i] = list[index + count - i];
        list[index + count - i] = value;
      }
    }

    public static int AddSorted<T>(this IList<T> list, T item, Comparison<T> comparison, SortingOption option = SortingOption.None)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      int position = list.BinarySearch(t => comparison(t, item), option);
      if (position < 0)
        position = ~position;
      else if (option == SortingOption.Unique)
        return ~position;
      else if (option == SortingOption.Last)
        position++;
      list.Insert(position, item);
      return position;
    }

    public static bool InsertSorted<T>(this IList<T> list, int position, T item, Comparison<T> comparison, SortingOption option = SortingOption.None)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (comparison == null)
        throw new ArgumentNullException("comparison");
      if (position < 0 || position > list.Count)
        throw new ArgumentOutOfRangeException("position");

      if (position > 0 && (comparison(list[position - 1], item) > 0 || option == SortingOption.Unique && comparison(list[position - 1], item) == 0) ||
        position < list.Count && (comparison(list[position], item) < 0 || option == SortingOption.Unique && comparison(list[position], item) == 0))
        return false;
      list.Insert(position, item);
      return true;
    }

    public static bool SetSorted<T>(this IList<T> list, int position, T item, Comparison<T> comparison, SortingOption option = SortingOption.None)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (comparison == null)
        throw new ArgumentNullException("comparison");
      if (position < 0 || position >= list.Count)
        throw new ArgumentOutOfRangeException("position");

      if (position > 0 && (comparison(list[position - 1], item) > 0 || option == SortingOption.Unique && comparison(list[position - 1], item) == 0) ||
        position < list.Count - 1 && (comparison(list[position + 1], item) < 0 || option == SortingOption.Unique && comparison(list[position + 1], item) == 0))
        return false;
      list[position] = item;
      return true;
    }

    public static T Get<T>(this IList<T> list, int index)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");
      if (list.Count == 0)
        throw new ArgumentException("Empty list", "list");
      if (index >= list.Count)
        throw new ArgumentOutOfRangeException("index");

      T item = list[index];
      list.RemoveAt(index);
      return item;
    }

    public static T Pop<T>(this IList<T> list, bool direction)
    {
      return list.Get<T>(list != null && list.Count > 0 && direction ? list.Count - 1 : 0);
    }

    public static T PopFirst<T>(this IList<T> list)
    {
      return list.Pop<T>(false);
    }

    public static T PopLast<T>(this IList<T> list)
    {
      return list.Pop<T>(true);
    }

    public static int Push<T>(this IList<T> list, T item, bool direction)
    {
      if (list == null)
        throw new ArgumentNullException("list");

      list.Insert(direction ? list.Count : 0, item);
      return list.Count;
    }

    public static int PushFirst<T>(this IList<T> list, T item)
    {
      return list.Push(item, false);
    }

    public static int PushLast<T>(this IList<T> list, T item)
    {
      return list.Push(item, true);
    }

    #endregion
    #region Transformation methods

    public static void Fill<T>(this IList<T> list, T value)
		{
      Fill(list, value, 0, list != null ? list.Count : 0);
		}

		public static void Fill<T>(this IList<T> list, T value, int index, int count)
    {
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");

      while (count-- > 0)
        list[index++] = value;
    }

		public static void Apply<T>(this IList<T> list, Action<T> action)
		{
      Apply(list, 0, list != null ? list.Count : 0, action);
    }

		public static void Apply<T>(this IList<T> list, int index, int count, Action<T> action)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
      if (action == null)
        throw new ArgumentNullException("action");

      while (count-- > 0)
        action(list[index++]);
    }

		public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
		{
      Sort(list, 0, list != null ? list.Count : 0, comparison);
		}

		public static void Sort<T>(this IList<T> list, int index, int count, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (comparison == null)
				throw new ArgumentNullException("comparison");

      int left = index;
      int right = index + count - 1;
      do
      {
        int lower = left;
        int upper = right;
        int middle = lower + ((upper - lower) >> 1);
        if (comparison(list[lower], list[middle]) > 0)
          Swap(list, lower, middle);
        if (comparison(list[lower], list[upper]) > 0)
          Swap(list, lower, upper);
        if (comparison(list[middle], list[upper]) > 0)
          Swap(list, middle, upper);
        T median = list[middle];
        do
        {
          while (comparison(list[lower], median) < 0)
            lower++;
          while (comparison(median, list[upper]) < 0)
            upper--;
          if (lower > upper)
            break;
          else if (lower < upper)
            Swap(list, lower, upper);
          lower++;
          upper--;
        }
        while (lower <= upper);
        if (upper - left <= right - lower)
        {
          if (left < upper)
            Sort(list, left, upper - left + 1, comparison);
          left = lower;
        }
        else
        {
          if (lower < right)
            Sort(list, lower, right - lower + 1, comparison);
          right = upper;
        }
      }
      while (left < right);
    }

		public static void Sort<K, T>(this IList<T> list, IList<K> keys, Comparison<K> comparison)
		{
      Sort(list, keys, 0, list != null ? list.Count : 0, comparison);
    }

		public static void Sort<K, T>(this IList<T> list, IList<K> keys, int index, int count, Comparison<K> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (keys == null)
				throw new ArgumentNullException("keys");
			if (keys.Count != list.Count)
				throw new ArgumentException("Length of keys does not match length of values", "keys");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (comparison == null)
				throw new ArgumentNullException("comparer");

      int left = index;
      int right = index + count - 1;
      do
      {
        int lower = left;
        int upper = right;
        int middle = lower + ((upper - lower) >> 1);
        if (comparison(keys[lower], keys[middle]) > 0)
        {
          Swap(keys, lower, middle);
          Swap(list, lower, middle);
        }
        if (comparison(keys[lower], keys[upper]) > 0)
        {
          Swap(keys, lower, upper);
          Swap(list, lower, upper);
        }
        if (comparison(keys[middle], keys[upper]) > 0)
        {
          Swap(keys, middle, upper);
          Swap(list, middle, upper);
        }
        K median = keys[middle];
        do
        {
          while (comparison(keys[lower], median) < 0)
            lower++;
          while (comparison(median, keys[upper]) < 0)
            upper--;
          if (lower > upper)
            break;
          else if (lower < upper)
          {
            Swap(keys, lower, upper);
            Swap(list, lower, upper);
          }
          lower++;
          upper--;
        }
        while (lower <= upper);
        if (upper - left <= right - lower)
        {
          if (left < upper)
            Sort(list, keys, left, upper - left + 1, comparison);
          left = lower;
        }
        else
        {
          if (lower < right)
            Sort(list, keys, lower, right - lower + 1, comparison);
          right = upper;
        }
      }
      while (left < right);
    }

    #endregion
    #region Extraction methods

    public static IEnumerable<T> Enumerate<T>(this IList<T> list, bool reverse = false)
    {
      return Enumerate(list, 0, list != null ? list.Count : 0, reverse);
    }

    public static IEnumerable<T> Enumerate<T>(this IList<T> list, int index, int count, bool reverse = false)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (index < 0 || index > list.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > list.Count - index)
        throw new ArgumentOutOfRangeException("count");

      for (int i = reverse ? index + count - 1 : index; count > 0; i += reverse ? -1 : 1, count--)
        yield return list[i];
    }

    public static void CopyTo<T>(this IList<T> list, T[] array, int arrayIndex, bool reverse = false)
		{
      CopyTo(list, array, arrayIndex, 0, list != null ? list.Count : 0, reverse);
    }

		public static void CopyTo<T>(this IList<T> list, T[] array, int arrayIndex, int index, int count, bool reverse = false)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (array == null)
				throw new ArgumentNullException("array");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (arrayIndex < 0 || arrayIndex > array.Length)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count > array.Length - arrayIndex)
				throw new ArgumentException("The number of elements from arrayIndex to the end of the source List is greater than the available space from arrayIndex to the end of the destination list");

      for (int i = reverse ? index + count - 1 : index; count > 0; i += reverse ? -1 : 1, count--)
        array[arrayIndex++] = list[i];
    }

    #endregion
    #region Retrieval methods

    public static bool Match<T>(this IList<T> list, Func<T, bool> match, bool all)
		{
      return list.Match(0, list != null ? list.Count : 0, match, all);
    }

    public static bool Match<T>(this IList<T> list, int index, Func<T, bool> match, bool all)
		{
      return list.Match(index, list != null ? list.Count - index : 0, match, all);
    }

    public static bool Match<T>(this IList<T> list, int index, int count, Func<T, bool> match, bool all)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");

      bool result = all;
      for (int i = index; (result ^ !all) && i < index + count; i++)
        result = match(list[i]);
      return result;
    }

    public static int FindIndex<T>(this IList<T> list, Func<T, bool> match)
		{
      return list.FindIndex(0, list != null ? list.Count : 0, match);
    }

		public static int FindIndex<T>(this IList<T> list, int index, Func<T, bool> match)
		{
      return list.FindIndex(index, list != null ? list.Count - index : 0, match);
    }

		public static int FindIndex<T>(this IList<T> list, int index, int count, Func<T, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");

      for (int i = index; i < index + count; i++)
        if (match(list[i]))
          return i;
      return -1;
    }

    public static int FindLastIndex<T>(this IList<T> list, Func<T, bool> match)
		{
      return list.FindLastIndex(0, list != null ? list.Count : 0, match);
		}

		public static int FindLastIndex<T>(this IList<T> list, int index, Func<T, bool> match)
		{
      return list.FindLastIndex(index, list != null ? list.Count -  index : 0, match);
    }

		public static int FindLastIndex<T>(this IList<T> list, int index, int count, Func<T, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
      if (index < 0 || index > list.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > list.Count - index)
        throw new ArgumentOutOfRangeException("count");
      if (match == null)
				throw new ArgumentNullException("match");

      for (int i = index + count - 1; i >= index; i--)
        if (match(list[i]))
          return i;
      return -1;
    }

    public static IEnumerable<T> FindAll<T>(this IList<T> list, Func<T, bool> match)
		{
      return list.FindAll(0, list != null ? list.Count : 0, match);
    }

		public static IEnumerable<T> FindAll<T>(this IList<T> list, int index, Func<T, bool> match)
		{
      return list.FindAll(index, list != null ? list.Count - index : 0, match);
    }

		public static IEnumerable<T> FindAll<T>(this IList<T> list, int index, int count, Func<T, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
      if (index < 0 || index > list.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > list.Count - index)
        throw new ArgumentOutOfRangeException("count");
      if (match == null)
        throw new ArgumentNullException("match");

      for (int i = index; i < index + count; i++)
        if (match(list[i]))
          yield return list[i];
    }

    public static IEnumerable<int> FindAllIndices<T>(this IList<T> list, Func<T, bool> match)
		{
      return list.FindAllIndices(0, list != null ? list.Count : 0, match);
    }

		public static IEnumerable<int> FindAllIndices<T>(this IList<T> list, int index, Func<T, bool> match)
		{
      return list.FindAllIndices(index, list != null ? list.Count - index : 0, match);
    }

		public static IEnumerable<int> FindAllIndices<T>(this IList<T> list, int index, int count, Func<T, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");

      for (int i = index, c = index + count; i < c; i++)
        if (match(list[i]))
          yield return i;
    }

    public static int BinarySearch<T>(this IList<T> list, Func<T, int> comparison, SortingOption option = SortingOption.None)
    {
      return list.BinarySearch(0, list != null ? list.Count : 0, comparison, option);
    }

    public static int BinarySearch<T>(this IList<T> list, int index, Func<T, int> comparison, SortingOption option = SortingOption.None)
    {
      return list.BinarySearch(index, list != null ? list.Count - index : 0, comparison, option);
    }

    public static int BinarySearch<T>(this IList<T> list, int index, int count, Func<T, int> comparison, SortingOption option = SortingOption.None)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (index < 0 || index > list.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > list.Count - index)
        throw new ArgumentOutOfRangeException("count");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      int found = -1;
      int lower = index;
      if (count > 0)
      {
        int upper = index + count - 1;
        while (lower <= upper)
        {
          int middle = (lower + upper) / 2;
          int result = comparison(list[middle]);
          if (result > 0)
            upper = middle - 1;
          else if (result < 0)
            lower = middle + 1;
          else
          {
            found = middle;
            if (option == SortingOption.First)
              upper = middle - 1;
            else if (option == SortingOption.Last)
              lower = middle + 1;
            else
              break;
          }
        }
      }
      return (found < 0) ? ~lower : found;
    }

    public static int SequenceFind<T>(this IList<T> list, IList<T> search, bool partial, Equality<T> equality)
    {
      return list.SequenceFind(0, list != null ? list.Count : 0, search, partial, equality);
    }

    public static int SequenceFind<T>(this IList<T> list, int index, IList<T> search, bool partial, Equality<T> equality)
    {
      return list.SequenceFind(index, list != null ? list.Count - index : 0, search, partial, equality);
    }

    public static int SequenceFind<T>(this IList<T> list, int index, int count, IList<T> search, bool partial, Equality<T> equality)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (index < 0 || index > list.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > list.Count - index)
        throw new ArgumentOutOfRangeException("count");
      if (search == null)
        throw new ArgumentNullException("search");
      if (equality == null)
        throw new ArgumentNullException("equality");

      int matched = 0;
      while (count > 0 && matched < count && matched < search.Count && (partial || count >= search.Count))
      {
        if (equality(list[index + matched], search[matched]))
          matched++;
        else
        {
          matched = 0;
          count--;
          index++;
        }
      }
      return matched == search.Count ? index : -matched - 1;
    }

    public static int SequenceFindLast<T>(this IList<T> list, IList<T> search, bool partial, Equality<T> equality)
    {
      return list.SequenceFindLast(0, list != null ? list.Count : 0, search, partial, equality);
    }

    public static int SequenceFindLast<T>(this IList<T> list, int index, IList<T> search, bool partial, Equality<T> equality)
    {
      return list.SequenceFindLast(index, list != null ? list.Count - index : 0, search, partial, equality);
    }

    public static int SequenceFindLast<T>(this IList<T> list, int index, int count, IList<T> search, bool partial, Equality<T> equality)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (index < 0 || index > list.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > list.Count - index)
        throw new ArgumentOutOfRangeException("count");
      if (search == null)
        throw new ArgumentNullException("search");
      if (equality == null)
        throw new ArgumentNullException("equality");

      int matched = 0;
      while (count > 0 && matched < count && matched < search.Count && (partial || count >= search.Count))
      {
        if (equality(list[index + count - 1 - matched], search[search.Count - 1 - matched]))
          matched++;
        else
        {
          matched = 0;
          count--;
        }
      }
      return matched == search.Count ? index + count - matched : -matched - 1;
    }

    public static int SequenceFind<T>(this IList<T> list, int search, bool partial, Func<T, int, bool> match)
    {
      return list.SequenceFind(0, list != null ? list.Count : 0, search, partial, match);
    }

    public static int SequenceFind<T>(this IList<T> list, int index, int search, bool partial, Func<T, int, bool> match)
    {
      return list.SequenceFind(index, list != null ? list.Count - index : 0, search, partial, match);
    }

    public static int SequenceFind<T>(this IList<T> list, int index, int count, int search, bool partial, Func<T, int, bool> match)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (index < 0 || index > list.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > list.Count - index)
        throw new ArgumentOutOfRangeException("count");
      if (match == null)
        throw new ArgumentNullException("match");

      int matched = 0;
      while (count > 0 && matched < count && matched < search && (partial || count >= search))
      {
        if (match(list[index + matched], matched))
          matched++;
        else
        {
          matched = 0;
          count--;
          index++;
        }
      }
      return matched == search ? index : -matched - 1;
    }

    public static int SequenceFindLast<T>(this IList<T> list, int search, bool partial, Func<T, int, bool> match)
    {
      return list.SequenceFindLast(0, list != null ? list.Count : 0, search, partial, match);
    }

    public static int SequenceFindLast<T>(this IList<T> list, int index, int search, bool partial, Func<T, int, bool> match)
    {
      return list.SequenceFindLast(index, list != null ? list.Count - index : 0, search, partial, match);
    }

    public static int SequenceFindLast<T>(this IList<T> list, int index, int count, int search, bool partial, Func<T, int, bool> match)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (index < 0 || index > list.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > list.Count - index)
        throw new ArgumentOutOfRangeException("count");
      if (search < 0)
        throw new ArgumentOutOfRangeException("search");
      if (match == null)
        throw new ArgumentNullException("match");

      int matched = 0;
      while (count > 0 && matched < count && matched < search && (partial || count >= search))
      {
        if (match(list[index + count - 1 - matched], search - 1 - matched))
          matched++;
        else
        {
          matched = 0;
          count--;
        }
      }
      return matched == search ? index + count - matched : -matched - 1;
    }

    public static Range SequenceFind<T>(this IList<T> list, Func<IList<T>, int> match)
    {
      return list.SequenceFind<T>(0, list != null ? list.Count : 0, match);
    }

    public static Range SequenceFind<T>(this IList<T> list, int index, Func<IList<T>, int> match)
    {
      return list.SequenceFind<T>(index, list != null ? list.Count - index : 0, match);
    }

    public static Range SequenceFind<T>(this IList<T> list, int index, int count, Func<IList<T>, int> match)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (index < 0 || index > list.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > list.Count - index)
        throw new ArgumentOutOfRangeException("count");
      if (match == null)
        throw new ArgumentNullException("match");

      PwrFrameListView<T> view = new PwrFrameListView<T>(list);
      int matched = 0;
      int accepted = 0;
      while (count > 0 && matched < count)
      {
        view.Frame = new Range(index, matched + 1);
        accepted = match(view);
        if (accepted > 0)
          break;
        else if (accepted == 0)
          matched++;
        else
        {
          matched = 0;
          count--;
          index++;
        }
      }
      return new Range(index, accepted > 0 ? accepted : -matched - 1);
    }

    public static Range SequenceFindLast<T>(this IList<T> list, Func<IList<T>, int> match)
    {
      return SequenceFindLast(list, 0, list != null ? list.Count : 0, match);
    }

    public static Range SequenceFindLast<T>(this IList<T> list, int index, Func<IList<T>, int> match)
    {
      return SequenceFindLast(list, index, list != null ? list.Count - index : 0, match);
    }

    public static Range SequenceFindLast<T>(this IList<T> list, int index, int count, Func<IList<T>, int> match)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      if (index < 0 || index > list.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > list.Count - index)
        throw new ArgumentOutOfRangeException("count");
      if (match == null)
        throw new ArgumentNullException("match");

      PwrFrameListView<T> view = new PwrFrameListView<T>(list);
      int matched = 0;
      int accepted = 0;
      while (count > 0 && matched < count)
      {
        view.Frame = new Range(index + count - 1 - matched, matched + 1);
        accepted = match(view);
        if (accepted > 0)
          break;
        else if (accepted == 0)
          matched++;
        else
        {
          matched = 0;
          count--;
        }
      }
      return new Range(index + count - accepted > 0 ? accepted : matched, accepted > 0 ? accepted : -matched - 1);
    }

    public static int SequenceCompare<T>(this IList<T> xList, IList<T> yList, Comparison<T> comparison)
    {
      return SequenceCompare(xList, 0, yList, 0, int.MaxValue, comparison);
    }

    public static int SequenceCompare<T>(this IList<T> xList, int xIndex, IList<T> yList, int yIndex, Comparison<T> comparison)
    {
      return SequenceCompare(xList, xIndex, yList, yIndex, int.MaxValue, comparison);
    }

    public static int SequenceCompare<T>(this IList<T> xList, int xIndex, IList<T> yList, int yIndex, int count, Comparison<T> comparison)
    {
      if (xList == null)
        throw new ArgumentNullException("xList");
      if (yList == null)
        throw new ArgumentNullException("yList");
      if (xIndex < 0 || xIndex > xList.Count)
        throw new ArgumentOutOfRangeException("xIndex");
      if (yIndex < 0 || yIndex > yList.Count)
        throw new ArgumentOutOfRangeException("yIndex");
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      int result = 0;
      for (; count > 0 && xIndex < xList.Count && yIndex < yList.Count; count--, xIndex++, yIndex++)
        if ((result = comparison(xList[xIndex], yList[yIndex])) != 0)
          break;
      return !(xIndex < xList.Count ^ yIndex < yList.Count) ? (result > 0 ? 1 : result < 0 ? -1 : 0) : xIndex < xList.Count ? 1 : -1;
    }

    public static int SequenceCompare<T>(this IList<T> xList, IList<T> yList, IComparer<T> comparer = null)
    {
      return SequenceCompare(xList, 0, yList, 0, int.MaxValue, comparer != null ? (Comparison<T>)comparer.Compare : (Comparison<T>)Comparer<T>.Default.Compare);
    }

    public static int SequenceCompare<T>(this IList<T> xList, int xIndex, IList<T> yList, int yIndex, IComparer<T> comparer = null)
    {
      return SequenceCompare(xList, xIndex, yList, yIndex, int.MaxValue, comparer != null ? (Comparison<T>)comparer.Compare : (Comparison<T>)Comparer<T>.Default.Compare);
    }

    public static int SequenceCompare<T>(this IList<T> xList, int xIndex, IList<T> yList, int yIndex, int count, IComparer<T> comparer = null)
    {
      return SequenceCompare(xList, xIndex, yList, yIndex, count, comparer != null ? (Comparison<T>)comparer.Compare : (Comparison<T>)Comparer<T>.Default.Compare);
    }

    public static bool SequenceEqual<T>(this IList<T> xList, IList<T> yList, Equality<T> equality)
    {
      return SequenceEqual(xList, 0, yList, 0, int.MaxValue, equality);
    }

    public static bool SequenceEqual<T>(this IList<T> xList, int xIndex, IList<T> yList, int yIndex, Equality<T> equality)
    {
      return SequenceEqual(xList, xIndex, yList, yIndex, int.MaxValue, equality);
    }

    public static bool SequenceEqual<T>(this IList<T> xList, int xIndex, IList<T> yList, int yIndex, int count, Equality<T> equality)
    {
      if (xList == null)
        throw new ArgumentNullException("xList");
      if (yList == null)
        throw new ArgumentNullException("yList");
      if (xIndex < 0 || xIndex > xList.Count)
        throw new ArgumentOutOfRangeException("xIndex");
      if (yIndex < 0 || yIndex > yList.Count)
        throw new ArgumentOutOfRangeException("yIndex");
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      if (equality == null)
        throw new ArgumentNullException("equality");

      if (Comparable.Min(xList.Count - xIndex, count) != Comparable.Min(yList.Count - yIndex, count))
        return false;
      bool equals = true;
      for (; equals && count > 0 && xIndex < xList.Count && yIndex < yList.Count; count--, xIndex++, yIndex++)
        equals = equality(xList[xIndex], yList[yIndex]);
      return equals;
    }

    public static bool SequenceEqual<T>(this IList<T> xList, IList<T> yList, IEqualityComparer<T> equalityComparer = null)
    {
      return SequenceEqual(xList, 0, yList, 0, int.MaxValue, equalityComparer != null ? (Equality<T>)equalityComparer.Equals : (Equality<T>)EqualityComparer<T>.Default.Equals);
    }

    public static bool SequenceEqual<T>(this IList<T> xList, int xIndex, IList<T> yList, int yIndex, IEqualityComparer<T> equalityComparer = null)
    {
      return SequenceEqual(xList, xIndex, yList, yIndex, int.MaxValue, equalityComparer != null ? (Equality<T>)equalityComparer.Equals : (Equality<T>)EqualityComparer<T>.Default.Equals);
    }

    public static bool SequenceEqual<T>(this IList<T> xList, int xIndex, IList<T> yList, int yIndex, int count, IEqualityComparer<T> equalityComparer = null)
    {
      return SequenceEqual(xList, xIndex, yList, yIndex, count, equalityComparer != null ? (Equality<T>)equalityComparer.Equals : (Equality<T>)EqualityComparer<T>.Default.Equals);
    }

    #endregion
	}
}
