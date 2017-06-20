using System;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	public static class ListExtension
	{
		#region Manipulation methods

		public static void AddRange<T>(this IList<T> list, IEnumerable<T> coll)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).AddRange(coll);
			else if (list is List<T>)
				((List<T>)list).AddRange(coll);
			else
			{
				if (coll == null)
					throw new ArgumentNullException("source");
				if (coll is ICollection<T> && list is ICapacitySupport)
					((ICapacitySupport)list).Capacity = list.Count + ((ICollection<T>)coll).Count;
				//
				using (IEnumerator<T> e = coll.GetEnumerator())
				{
					while (e.MoveNext())
						list.Add(e.Current);
				}
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
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (coll == null)
					throw new ArgumentNullException("source");
				//
				using (IEnumerator<T> e = coll.GetEnumerator())
				{
					while (e.MoveNext())
						list.Insert(index++, e.Current);
				}
			}
		}

		public static void InsertRangeReverse<T>(this IList<T> list, int index, IEnumerable<T> coll)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).InsertRangeReverse(index, coll);
			else
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (coll == null)
					throw new ArgumentNullException("source");
				//
				using (IEnumerator<T> e = coll.GetEnumerator())
				{
					while (e.MoveNext())
						list.Insert(index, e.Current);
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
					throw new ArgumentOutOfRangeException("arrayIndex");
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
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			using (IEnumerator<T> e = coll.GetEnumerator())
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

		public static void UpdateRange<T>(this IList<T> list, int index, int count, IEnumerable<T> coll)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (coll == null)
				throw new ArgumentNullException("source");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			using (IEnumerator<T> e = coll.GetEnumerator())
			{
				while (count > 0 && e.MoveNext())
				{
					list[index++] = e.Current;
					count--;
				}
			}
		}

		public static void Move<T>(this IList<T> list, int srcIndex, int dstIndex)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).Move(srcIndex, dstIndex);
			else
			{
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
		}

		public static void MoveRange<T>(this IList<T> list, int srcIndex, int dstIndex, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).MoveRange(srcIndex, dstIndex, count);
			else
			{
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
		}

		public static void Swap<T>(this IList<T> list, int xIndex, int yIndex)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).Swap(xIndex, yIndex);
			else
			{
				if (xIndex < 0 || xIndex >= list.Count)
					throw new ArgumentOutOfRangeException("xIndex");
				if (yIndex < 0 || yIndex >= list.Count)
					throw new ArgumentOutOfRangeException("yIndex");
				if (xIndex == yIndex)
					return;
				//
				T value = list[xIndex];
				list[xIndex] = list[yIndex];
				list[yIndex] = value;
			}
		}

		public static void SwapRange<T>(this IList<T> list, int xIndex, int yIndex, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).SwapRange(xIndex, yIndex, count);
			else
			{
				if (xIndex < 0 || xIndex > list.Count)
					throw new ArgumentOutOfRangeException("xIndex");
				if (yIndex < 0 || yIndex > list.Count)
					throw new ArgumentOutOfRangeException("yIndex");
				if (count < 0 || count > list.Count - xIndex || count > list.Count - yIndex)
					throw new ArgumentOutOfRangeException("count");
				if (count > global::System.Math.Abs(yIndex - xIndex))
					throw new ArgumentException("Ranges intersected");
				//
				for (int i = 0; i < count; i++)
				{
					T value = list[xIndex + i];
					list[xIndex + i] = list[yIndex + i];
					list[yIndex + i] = value;
				}
			}
		}

		public static void SwapRanges<T>(this IList<T> list, int xIndex, int xCount, int yIndex, int yCount)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).SwapRanges(xIndex, xCount, yIndex, yCount);
			else
			{
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
				//
				for (int i = 0; i < global::System.Math.Min(xCount, yCount); i++)
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
					for (int i = 0, c = lowerCount - upperCount, s = lowerIndex + upperCount; i < c; i++)
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
		}

		public static void Reverse<T>(this IList<T> list, int index, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).Reverse();
			else if (list is global::System.Collections.Generic.List<T>)
				((global::System.Collections.Generic.List<T>)list).Reverse(index, count);
			else
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				//
				for (int i = 0; i < count / 2; i++)
				{
					T value = list[index + i];
					list[index + i] = list[index + count - i];
					list[index + count - i] = value;
				}
			}
		}

		#endregion
		#region Transformation methods
		#region Direct methods

		public static void Fill<T>(this IList<T> list, T value)
		{
			Fill(list, 0, (list != null) ? list.Count : 0, value);
		}

		public static void Fill<T>(this IList<T> list, int index, int count, T value)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).Fill(index, count, value);
			else
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				//
				for (int i = index, c = index + count; i < c; i++)
					list[i] = value;
			}
		}

		public static void Apply<T>(this IList<T> list, Action<T> action)
		{
			Apply(list, 0, (list != null) ? list.Count : 0, action);
		}

		public static void Apply<T>(this IList<T> list, int index, int count, Action<T> action)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).Apply(index, count, action);
			else
			{
				if (action == null)
					throw new ArgumentNullException("action");
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				//
				for (int i = index; i < index + count; i++)
					action(list[i]);
			}
		}

		public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
		{
			Sort(list, 0, (list != null) ? list.Count : 0, comparison);
		}

		public static void Sort<T>(this IList<T> list, int index, int count, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).Sort(index, count, comparison);
			else
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				if (comparison == null)
					throw new ArgumentNullException("comparer");
				//
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
						try
						{
							while (comparison(list[lower], median) < 0)
								lower++;
							while (comparison(median, list[upper]) < 0)
								upper--;
						}
						catch (Exception ex)
						{
							throw new InvalidOperationException("Invalid compare", ex);
						}
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
		}

		public static void Sort<K, T>(this IList<T> list, IList<K> keys, Comparison<K> comparison)
		{
			Sort(list, keys, 0, (list != null) ? list.Count : 0, comparison);
		}

		public static void Sort<K, T>(this IList<T> list, IList<K> keys, int index, int count, Comparison<K> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (keys == null)
				throw new ArgumentNullException("keys");
			if (keys.Count != list.Count)
				throw new ArgumentException("Length of keys does not match length of values", "keys");
			if (list is PwrList<T> && keys is PwrList<K>)
				((PwrList<T>)list).Sort((PwrList<K>)keys, index, count, comparison);
			else
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				if (comparison == null)
					throw new ArgumentNullException("comparer");
				//
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
						try
						{
							while (comparison(keys[lower], median) < 0)
								lower++;
							while (comparison(median, keys[upper]) < 0)
								upper--;
						}
						catch (Exception ex)
						{
							throw new InvalidOperationException("Invalid compare", ex);
						}
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
		}

		#endregion
		#region Accessor methods

		public static void Fill<T, TResult>(this IList<T> list, Action<T, TResult> filler, TResult value)
		{
			Fill(list, filler, 0, (list != null) ? list.Count : 0, value);
		}

		public static void Fill<T, TResult>(this IList<T> list, Action<T, TResult> filler, int index, int count, TResult value)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (filler == null)
				throw new ArgumentNullException("filler");
			//
			for (int i = index; i < index + count; i++)
				filler(list[i], value);
		}

		public static void Apply<T, TResult>(this IList<T> list, Func<T, TResult> selector, Action<TResult> action)
		{
			Apply(list, selector, 0, (list != null) ? list.Count : 0, action);
		}

		public static void Apply<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count, Action<TResult> action)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (action == null)
				throw new ArgumentNullException("action");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			for (int i = index; i < index + count; i++)
				action(selector(list[i]));
		}

		public static void Sort<T, TResult>(this IList<T> list, Func<T, TResult> selector, Comparison<TResult> comparison)
		{
			Sort(list, selector, 0, (list != null) ? list.Count : 0, comparison);
		}

		public static void Sort<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count, Comparison<TResult> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			if (count == 0)
				return;
			int left = index;
			int right = index + count - 1;
			do
			{
				int lower = left;
				int upper = right;
				int middle = (lower + upper) / 2;
				if (comparison(selector(list[lower]), selector(list[middle])) > 0)
					Swap(list, lower, middle);
				if (comparison(selector(list[lower]), selector(list[upper])) > 0)
					Swap(list, lower, upper);
				if (comparison(selector(list[middle]), selector(list[upper])) > 0)
					Swap(list, middle, upper);
				TResult median = selector(list[middle]);
				do
				{
					for (TResult item = selector(list[lower]); comparison(item, median) < 0; item = selector(list[lower]))
						lower++;
					for (TResult item = selector(list[upper]); comparison(median, item) < 0; item = selector(list[upper]))
						upper--;
					if (lower > upper)
						break;
					else if (lower < upper)
						Swap(list, lower, upper);
					lower++;
					upper--;
				}
				while (true);
				if (upper - left <= right - lower)
				{
					if (left < upper)
						Sort(list, selector, left, upper - left + 1, comparison);
					left = lower;
				}
				else
				{
					if (lower < right)
						Sort(list, selector, lower, right - lower + 1, comparison);
					right = upper;
				}
			}
			while (left < right);
		}

		#endregion
		#endregion
		#region Extraction methods
		#region Direct methods

		public static IEnumerable<TOutput> Convert<T, TOutput>(this IList<T> list, Converter<T, TOutput> converter)
		{
			return Convert(list, 0, (list != null) ? list.Count : 0, converter);
		}

		public static IEnumerable<TOutput> Convert<T, TOutput>(this IList<T> list, int index, int count, Converter<T, TOutput> converter)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).Convert(index, count, converter);
			else
			{
				if (converter == null)
					throw new ArgumentNullException("valueConverter");
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				for (int i = index; i < index + count; i++)
					yield return converter(list[i]);
			}
		}

		public static void CopyTo<T>(this IList<T> list, T[] array, int arrayIndex)
		{
			CopyTo(list, array, arrayIndex, 0, (list != null) ? list.Count : 0);
		}

		public static void CopyTo<T>(this IList<T> list, T[] array, int arrayIndex, int index, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (list is PwrList<T>)
				((PwrList<T>)list).CopyTo(array, arrayIndex, index, count);
			else
			{
				if (array == null)
					throw new ArgumentNullException("array");
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("index");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				if (arrayIndex < 0 || arrayIndex > array.Length)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count > array.Length - arrayIndex)
					throw new ArgumentException("The number of elements firstNode arrayIndex to the end of the source List is greater than the available space firstNode arrayIndex to the end of the destination list");
				//
				for (int i = 0; i < count; i++)
					array[arrayIndex++] = list[index + i];
			}
		}

		#endregion
		#region Accessor methods

		public static IEnumerable<TOutput> Convert<T, TResult, TOutput>(this IList<T> list, Func<T, TResult> selector, Converter<TResult, TOutput> converter)
		{
			return Convert(list, selector, 0, (list != null) ? list.Count : 0, converter);
		}

		public static IEnumerable<TOutput> Convert<T, TResult, TOutput>(this IList<T> list, Func<T, TResult> selector, int index, int count, Converter<TResult, TOutput> converter)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (converter == null)
				throw new ArgumentNullException("valueConverter");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			for (int i = index; i < index + count; i++)
				yield return converter(selector(list[i]));
		}

		public static void CopyTo<T, TResult>(this IList<T> list, Func<T, TResult> selector, TResult[] array, int arrayIndex)
		{
			CopyTo(list, selector, array, arrayIndex, 0, list != null ? list.Count : 0);
		}

		public static void CopyTo<T, TResult>(this IList<T> list, Func<T, TResult> selector, TResult[] array, int arrayIndex, int index, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (array == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (arrayIndex < 0 || arrayIndex > array.Length)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count > array.Length - arrayIndex)
				throw new ArgumentException("The number of elements firstNode arrayIndex to the end of the source List is greater than the available space firstNode arrayIndex to the end of the destination list");
			//
			for (int i = 0; i < count; i++)
				array[arrayIndex++] = selector(list[index + i]);
		}

		#endregion
		#endregion
		#region Retrieval methods
		#region Direct methods

		public static T[] GetRange<T>(this IList<T> list, int index, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			T[] array = new T[count];
			for (int i = 0; i < count; i++)
				array[i] = list[index + i];
			return array;
		}

		public static IEnumerable<T> GetRangeReverse<T>(this IList<T> list, int index, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			T[] array = new T[count];
			for (int i = 0; i < count; i++)
				array[i] = list[index + count - 1 - i ];
			return array;
		}

		public static bool Exists<T>(this IList<T> list, Func<T, bool> match)
		{
			return Exists(list, 0, (list != null) ? list.Count : 0, match);
		}

		public static bool Exists<T>(this IList<T> list, int index, Func<T, bool> match)
		{
			return Exists(list, index, (list != null) ? list.Count - index : 0, match);
		}

		public static bool Exists<T>(this IList<T> list, int index, int count, Func<T, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			//
			if (list is PwrList<T>)
				return ((PwrList<T>)list).Exists(index, count, match);
			else
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				if (match == null)
					throw new ArgumentNullException("match");
				//
				for (int i = index, c = index + count; i < c; i++)
					if (match(list[i]))
						return true;
				return false;
			}
		}

		public static bool MatchAll<T>(this IList<T> list, Func<T, bool> match)
		{
			return MatchAll(list, 0, (list != null) ? list.Count : 0, match);
		}

		public static bool MatchAll<T>(this IList<T> list, int index, Func<T, bool> match)
		{
			return MatchAll(list, index, (list != null) ? list.Count - index : 0, match);
		}

		public static bool MatchAll<T>(this IList<T> list, int index, int count, Func<T, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			for (int i = index, c = index + count; i < c; i++)
				if (!match(list[i]))
					return false;
			return true;
		}

		public static int FindIndex<T>(this IList<T> list, Func<T, bool> match)
		{
			return FindIndex(list, 0, (list != null) ? list.Count : 0, match);
		}

		public static int FindIndex<T>(this IList<T> list, int index, Func<T, bool> match)
		{
			return FindIndex(list, index, (list != null) ? list.Count - index : 0, match);
		}

		public static int FindIndex<T>(this IList<T> list, int index, int count, Func<T, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			//
			if (list is PwrList<T>)
				return ((PwrList<T>)list).FindIndex(index, count, match);
			else
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				if (match == null)
					throw new ArgumentNullException("match");
				//
				for (int i = index, c = index + count; i < c; i++)
					if (match(list[i]))
						return i;
				return -1;
			}
		}

		public static int FindLastIndex<T>(this IList<T> list,Func<T, bool> match)
		{
			return FindLastIndex(list, (list != null) ? list.Count - 1 : 0, (list != null) ? list.Count : 0, match);
		}

		public static int FindLastIndex<T>(this IList<T> list, int index, Func<T, bool> match)
		{
			return FindLastIndex(list, index, index + 1, match);
		}

		public static int FindLastIndex<T>(this IList<T> list, int index, int count, Func<T, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			//
			if (list is PwrList<T>)
				return ((PwrList<T>)list).FindLastIndex(index, count, match);
			else
			{
				if (index < -1 || index >= list.Count + (list.Count == 0 ? 1 : 0))
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > index + 1)
					throw new ArgumentOutOfRangeException("count");
				if (match == null)
					throw new ArgumentNullException("match");
				//
				for (int i = index, c = index + 1 - count; i >= c; i--)
					if (match(list[i]))
						return i;
				return -1;
			}
		}

		public static IEnumerable<T> FindAllMatches<T>(this IList<T> list, Func<T, bool> match)
		{
			return FindAllMatches(list, 0, list != null ? list.Count : 0, match);
		}

		public static IEnumerable<T> FindAllMatches<T>(this IList<T> list, int index, Func<T, bool> match)
		{
			return FindAllMatches(list, index, list != null ? list.Count - index : 0, match);
		}

		public static IEnumerable<T> FindAllMatches<T>(this IList<T> list, int index, int count, Func<T, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			//
			if (list is PwrList<T>)
				((PwrList<T>)list).FindAllMatches(index, count, match);
			else
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				if (match == null)
					throw new ArgumentNullException("match");
				//
				for (int i = index, c = index + count; i < c; i++)
					if (match(list[i]))
						yield return list[i];
			}
		}

		public static IEnumerable<int> FindAllIndices<T>(this IList<T> list, Func<T, bool> match)
		{
			return FindAllIndices(list, 0, list != null ? list.Count : 0, match);
		}

		public static IEnumerable<int> FindAllIndices<T>(this IList<T> list, int index, Func<T, bool> match)
		{
			return FindAllIndices(list, index, list != null ? list.Count - index : 0, match);
		}

		public static IEnumerable<int> FindAllIndices<T>(this IList<T> list, int index, int count, Func<T, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			for (int i = index, c = index + count; i < c; i++)
				if (match(list[i]))
					yield return i;
		}

		public static int BinarySearch<T>(this IList<T> list, T value, Comparison<T> comparison)
		{
			return BinarySearch(list, 0, (list != null) ? list.Count : 0, value, comparison);
		}

		public static int BinarySearch<T>(this IList<T> list, int index, T value, Comparison<T> comparison)
		{
			return BinarySearch(list, index, (list != null) ? list.Count - index : 0, value, comparison);
		}

		public static int BinarySearch<T>(this IList<T> list, int index, int count, T value, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			//
			if (list is PwrList<T>)
				return ((PwrList<T>)list).BinarySearch(index, count, value, comparison);
			else
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				if (comparison == null)
					throw new ArgumentNullException("comparison");
				//
				int lower = index;
				if (count > 0)
				{
					int upper = index + count - 1;
					while (lower <= upper)
					{
						int middle = (lower + upper) / 2;
						int result = comparison(value, list[middle]);
						if (result < 0)
							upper = middle - 1;
						else if (result > 0)
							lower = middle + 1;
						else
							return middle;
					}
				}
				return ~lower;
			}
		}

		public static int BinarySearchFirst<T>(this IList<T> list, T value, Comparison<T> comparison)
		{
			return BinarySearchFirst(list, 0, (list != null) ? list.Count : 0, value, comparison);
		}

		public static int BinarySearchFirst<T>(this IList<T> list, int index, T value, Comparison<T> comparison)
		{
			return BinarySearchFirst(list, index, (list != null) ? list.Count - index : 0, value, comparison);
		}

		public static int BinarySearchFirst<T>(this IList<T> list, int index, int count, T value, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			//
			if (list is PwrList<T>)
				return ((PwrList<T>)list).BinarySearchFirst(index, count, value, comparison);
			else
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > list.Count - index)
					throw new ArgumentOutOfRangeException("count");
				if (comparison == null)
					throw new ArgumentNullException("comparison");
				//
				int found = -1;
				int lower = index;
				if (count > 0)
				{
					int upper = index + count - 1;
					while (lower <= upper)
					{
						int middle = (lower + upper) / 2;
						int result = comparison(value, list[middle]);
						if (result < 0)
							upper = middle - 1;
						else if (result > 0)
							lower = middle + 1;
						else
						{
							found = middle;
							upper = middle - 1;
						}
					}
				}
				return (found < 0) ? ~lower : found;
			}
		}

		public static int BinarySearchLast<T>(this IList<T> list, T value, Comparison<T> comparison)
		{
			return BinarySearchLast(list, (list != null) ? list.Count - 1 : -1, (list != null) ? list.Count : 0, value, comparison);
		}

		public static int BinarySearchLast<T>(this IList<T> list, int index, T value, Comparison<T> comparison)
		{
			return BinarySearchLast(list, index, index + 1, value, comparison);
		}

		public static int BinarySearchLast<T>(this IList<T> list, int index, int count, T value, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			//
			if (list is PwrList<T>)
				return ((PwrList<T>)list).BinarySearchLast(index, count, value, comparison);
			else
			{
				if (index < -1 || index >= list.Count + (list.Count == 0 ? 1 : 0))
					throw new ArgumentOutOfRangeException("arrayIndex");
				if (count < 0 || count > index + 1)
					throw new ArgumentOutOfRangeException("count");
				if (comparison == null)
					throw new ArgumentNullException("comparison");
				//
				int found = -1;
				int lower = index - count;
				if (count > 0)
				{
					int upper = index;
					while (lower <= upper)
					{
						int middle = (lower + upper) / 2;
						int result = comparison(value, list[middle]);
						if (result < 0)
							upper = middle - 1;
						else if (result > 0)
							lower = middle + 1;
						else
						{
							found = middle;
							lower = middle + 1;
						}
					}
				}
				return (found < 0) ? ~lower : found;
			}
		}

		public static int SearchRangeSortedDuplicate<T>(this IList<T> list, int index, int count, IEnumerable<T> coll, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (coll == null)
				throw new ArgumentNullException("source");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			//
			using (IEnumerator<T> e = coll.GetEnumerator())
			{
				int i = 0;
				while (e.MoveNext())
				{
					if (BinarySearch(list, index, count, e.Current, comparison) >= 0)
						return i;
					i++;
				}
			}
			return -1;
		}

		#endregion
		#region Accessor methods

		public static IEnumerable<TResult> GetRange<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			for (int i = index; i < index + count; i++)
				yield return selector(list[i]);
		}

		public static IEnumerable<TResult> GetRangeReverse<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			for (int i = index + count - 1; i >= index; i--)
				yield return selector(list[i]);
		}

		public static bool Exists<T, TResult>(this IList<T> list, Func<T, TResult> selector, Func<TResult, bool> match)
		{
			return Exists(list, selector, 0, (list != null) ? list.Count : 0, match);
		}

		public static bool Exists<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, Func<TResult, bool> match)
		{
			return Exists(list, selector, index, (list != null) ? list.Count - index : 0, match);
		}

		public static bool Exists<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count, Func<TResult, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			for (int i = index, c = index + count; i < c; i++)
				if (match(selector(list[i])))
					return true;
			return false;
		}

		public static bool MatchAll<T, TResult>(this IList<T> list, Func<T, TResult> selector, Func<TResult, bool> match)
		{
			return MatchAll(list, selector, 0, (list != null) ? list.Count : 0, match);
		}

		public static bool MatchAll<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, Func<TResult, bool> match)
		{
			return MatchAll(list, selector, index, (list != null) ? list.Count - index : 0, match);
		}

		public static bool MatchAll<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count, Func<TResult, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			for (int i = index, c = index + count; i < c; i++)
				if (!match(selector(list[i])))
					return false;
			return true;
		}

		public static int FindIndex<T, TResult>(this IList<T> list, Func<T, TResult> selector, Func<TResult, bool> match)
		{
			return FindIndex(list, selector, 0, (list != null) ? list.Count : 0, match);
		}

		public static int FindIndex<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, Func<TResult, bool> match)
		{
			return FindIndex(list, selector, index, (list != null) ? list.Count - index : 0, match);
		}

		public static int FindIndex<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count, Func<TResult, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			for (int i = index, c = index + count; i < c; i++)
				if (match(selector(list[i])))
					return i;
			return -1;
		}

		public static int FindLastIndex<T, TResult>(this IList<T> list, Func<T, TResult> selector, Func<TResult, bool> match)
		{
			return FindLastIndex(list, selector, 0, (list != null) ? list.Count : 0, match);
		}

		public static int FindLastIndex<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, Func<TResult, bool> match)
		{
			return FindLastIndex(list, selector, index, (list != null) ? list.Count - index : 0, match);
		}

		public static int FindLastIndex<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count, Func<TResult, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (index < -1 || index >= list.Count + (list.Count == 0 ? 1 : 0))
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > index + 1)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			for (int i = index, c = index + 1 - count; i >= c; i--)
				if (match(selector(list[i])))
					return i;
			return -1;
		}

		public static IEnumerable<TResult> FindAllMatches<T, TResult>(this IList<T> list, Func<T, TResult> selector, Func<TResult, bool> match)
		{
			return FindAllMatches(list, selector, 0, list != null ? list.Count : 0, match);
		}

		public static IEnumerable<TResult> FindAllMatches<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, Func<TResult, bool> match)
		{
			return FindAllMatches(list, selector, index, list != null ? list.Count - index : 0, match);
		}

		public static IEnumerable<TResult> FindAllMatches<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count, Func<TResult, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			for (int i = index, c = index + count; i < c; i++)
				if (match(selector(list[i])))
					yield return selector(list[i]);
		}

		public static IEnumerable<int> FindAllIndices<T, TResult>(this IList<T> list, Func<T, TResult> selector, Func<TResult, bool> match)
		{
			return FindAllIndices(list, selector, 0, list != null ? list.Count : 0, match);
		}

		public static IEnumerable<int> FindAllIndices<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, Func<TResult, bool> match)
		{
			return FindAllIndices(list, selector, index, list != null ? list.Count - index : 0, match);
		}

		public static IEnumerable<int> FindAllIndices<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count, Func<TResult, bool> match)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			for (int i = index, c = index + count; i < c; i++)
				if (match(selector(list[i])))
					yield return i;
		}

		public static int BinarySearch<T, TResult>(this IList<T> list, Func<T, TResult> selector, TResult value, Comparison<TResult> comparison)
		{
			return BinarySearch(list, selector, 0, (list != null) ? list.Count : 0, value, comparison);
		}

		public static int BinarySearch<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, TResult value, Comparison<TResult> comparison)
		{
			return BinarySearch(list, selector, index, (list != null) ? list.Count - index : 0, value, comparison);
		}

		public static int BinarySearch<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count, TResult value, Comparison<TResult> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			//
			int lower = index;
			if (count > 0)
			{
				int upper = index + count - 1;
				while (lower <= upper)
				{
					int middle = (lower + upper) / 2;
					int result = comparison(value, selector(list[middle]));
					if (result < 0)
						upper = middle - 1;
					else if (result > 0)
						lower = middle + 1;
					else
						return middle;
				}
			}
			return ~lower;
		}

		public static int BinarySearchFirst<T, TResult>(this IList<T> list, Func<T, TResult> selector, TResult value, Comparison<TResult> comparison)
		{
			return BinarySearchFirst(list, selector, 0, (list != null) ? list.Count : 0, value, comparison);
		}

		public static int BinarySearchFirst<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, TResult value, Comparison<TResult> comparison)
		{
			return BinarySearchFirst(list, selector, index, (list != null) ? list.Count - index : 0, value, comparison);
		}

		public static int BinarySearchFirst<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count, TResult value, Comparison<TResult> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			//
			int found = -1;
			int lower = index;
			if (count > 0)
			{
				int upper = index + count - 1;
				while (lower <= upper)
				{
					int middle = (lower + upper) / 2;
					int result = comparison(value, selector(list[middle]));
					if (result < 0)
						upper = middle - 1;
					else if (result > 0)
						lower = middle + 1;
					else
					{
						found = middle;
						upper = middle - 1;
					}
				}
			}
			return (found < 0) ? ~lower : found;
		}

		public static int BinarySearchLast<T, TResult>(this IList<T> list, Func<T, TResult> selector, TResult value, Comparison<TResult> comparison)
		{
			return BinarySearchLast(list, selector, (list != null) ? list.Count - 1 : -1, (list != null) ? list.Count : 0, value, comparison);
		}

		public static int BinarySearchLast<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, TResult value, Comparison<TResult> comparison)
		{
			return BinarySearchLast(list, selector, index, index + 1, value, comparison);
		}

		public static int BinarySearchLast<T, TResult>(this IList<T> list, Func<T, TResult> selector, int index, int count, TResult value, Comparison<TResult> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (index < -1 || index >= list.Count + (list.Count == 0 ? 1 : 0))
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > index + 1)
				throw new ArgumentOutOfRangeException("count");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			//
			int found = -1;
			int lower = index - count;
			if (lower < 0)
				lower = 0;
			if (count > 0)
			{
				int upper = index;
				while (lower <= upper)
				{
					int middle = (lower + upper) / 2;
					int result = comparison(value, selector(list[middle]));
					if (result < 0)
						upper = middle - 1;
					else if (result > 0)
						lower = middle + 1;
					else
					{
						found = middle;
						lower = middle + 1;
					}
				}
			}
			return (found < 0) ? ~lower : found;
		}

		public static int SearchRangeSortedDuplicate<T, TResult>(this IList<T> list, Func<T, TResult> selector, IEnumerable<TResult> coll, Comparison<TResult> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (coll == null)
				throw new ArgumentNullException("source");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			using (IEnumerator<TResult> e = coll.GetEnumerator())
			{
				int i = 0;
				while (e.MoveNext())
				{
					if (BinarySearch(list, selector, e.Current, comparison) >= 0)
						return i;
					i++;
				}
			}
			return -1;
		}

		#endregion
		#endregion
		#region Sort methods

		public static int AddSortedFirst<T>(this IList<T> list, T item, Comparison<T> comparison)
		{
			return AddSortedFirst(list, 0, list.Count, item, comparison);
		}

		public static int AddSortedFirst<T>(this IList<T> list, int index, int count, T item, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			int i = ListExtension.BinarySearch(list, index, count, item, comparison);
			while (i >= 0)
				i = ListExtension.BinarySearch(list, index, i - index, item, comparison);
			i = ~i;
			list.Insert(i, item);
			return i;
		}

		public static int AddSortedLast<T>(this IList<T> list, T item, Comparison<T> comparison)
		{
			return AddSortedLast(list, 0, list.Count, item, comparison);
		}

		public static int AddSortedLast<T>(this IList<T> list, int index, int count, T item, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			int i = ListExtension.BinarySearch(list, index, count, item, comparison);
			while (i >= 0)
				i = ListExtension.BinarySearch(list, i + 1, index + count - (i + 1), item, comparison);
			i = ~i;
			list.Insert(i, item);
			return i;
		}

		public static Range AddRangeSortedFirst<T>(this IList<T> list, IEnumerable<T> coll, Comparison<T> comparison)
		{
			return AddRangeSortedFirst(list, 0, list.Count, coll, comparison);
		}

		public static Range AddRangeSortedFirst<T>(this IList<T> list, int index, int count, IEnumerable<T> coll, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (coll == null)
				throw new ArgumentNullException("source");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			int min = 0, max = 0, c = 0;
			if (coll is ICollection<T>)
			{
				if (list is ICapacitySupport)
					((ICapacitySupport)list).Capacity = list.Count + ((ICollection<T>)coll).Count;
				else if (list is List<T>)
					((List<T>)list).Capacity = list.Count + ((ICollection<T>)coll).Count;
			}
			using (IEnumerator<T> e = coll.GetEnumerator())
			{
				while (e.MoveNext())
				{
					int i = ListExtension.BinarySearch(list, index, count, e.Current, comparison);
					while (i >= 0)
						i = ListExtension.BinarySearch(list, index, i - index, e.Current, comparison);
					i = ~i;
					list.Insert(i, e.Current);
					if (c == 0)
						min = max = index;
					else if (i > max)
						max = i;
					else
					{
						if (i < min)
							min = i;
						max++;
					}
					c++;
					count++;
				}
			}
			return (max - min + 1 == c) ? new Range(min, c) : new Range(0, c);
		}

		public static Range AddRangeSortedLast<T>(this IList<T> list, IEnumerable<T> coll, Comparison<T> comparison)
		{
			return AddRangeSortedLast(list, 0, list.Count, coll, comparison);
		}

		public static Range AddRangeSortedLast<T>(this IList<T> list, int index, int count, IEnumerable<T> coll, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (coll == null)
				throw new ArgumentNullException("source");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			int min = 0, max = 0, c = 0;
			if (coll is ICollection<T>)
			{
				if (list is ICapacitySupport)
					((ICapacitySupport)list).Capacity = list.Count + ((ICollection<T>)coll).Count;
				else if (list is List<T>)
					((List<T>)list).Capacity = list.Count + ((ICollection<T>)coll).Count;
			}
			using (IEnumerator<T> e = coll.GetEnumerator())
      {
				while (e.MoveNext())
				{
					int i = ListExtension.BinarySearch(list, index, count, e.Current, comparison);
					while (i >= 0)
						i = ListExtension.BinarySearch(list, i + 1, index + count - (i + 1), e.Current, comparison);
					i = ~i;
					list.Insert(i, e.Current);
					if (c == 0)
						min = max = index;
					else if (i > max)
						max = i;
					else
					{
						if (i < min)
							min = i;
						max++;
					}
					c++;
					count++;
				}
      }
			return (max - min + 1 == c) ? new Range(min, c) : new Range(0, c);
		}

		public static bool InsertSorted<T>(this IList<T> list, int position, T item, Comparison<T> comparison)
		{
			return InsertSorted(list, 0, list.Count, position, item, comparison);
		}

		public static bool InsertSorted<T>(this IList<T> list, int index, int count, int position, T item, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (position < index || position > index + count)
				throw new ArgumentOutOfRangeException("position");
			//
			if (!(position > index && comparison(list[position - 1], item) > 0 ||
				position < index + count && comparison(list[position], item) < 0))
			{
				list.Insert(position, item);
				return true;
			}
			else
				return false;
		}

		public static int SetSorted<T>(this IList<T> list, int position, T item, Comparison<T> comparison)
		{
			return SetSorted(list, 0, list.Count, position, item, comparison);
		}

		public static int SetSorted<T>(this IList<T> list, int index, int count, int position, T item, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (count < 0 || count > list.Count - index)
				throw new ArgumentOutOfRangeException("count");
			if (position < index || position >= index + count)
				throw new ArgumentOutOfRangeException("position");
			//
			if (position > index && comparison(list[position - 1], item) <= 0 &&
				position < index + count - 1 && comparison(list[position + 1], item) >= 0)
			{
				list[position] = item;
			}
			else
			{
				list.RemoveAt(position);
				position = ListExtension.BinarySearch(list, index, count - 1, item, comparison);
				while (position >= 0)
					position = ListExtension.BinarySearch(list, position + 1, (index + count - 1) - (position + 1), item, comparison);
				position = ~position;
				list.Insert(position, item);
			}
			return position;
		}

		#endregion
	}
}
