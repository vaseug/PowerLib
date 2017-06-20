using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerLib.System.Collections
{
	public static class CollectionExtension
	{
    #region Methods

    public static void ValidateRange(this ICollection coll, int index, int count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (index < 0 || index > coll.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > coll.Count - index)
        throw new ArgumentOutOfRangeException("count");
    }

    public static void ValidateRange<T>(this ICollection<T> coll, int index, int count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (index < 0 || index > coll.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > coll.Count - index)
        throw new ArgumentOutOfRangeException("count");
    }

    public static void ValidateRange<T>(this IReadOnlyCollection<T> coll, int index, int count)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (index < 0 || index > coll.Count)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > coll.Count - index)
        throw new ArgumentOutOfRangeException("count");
    }

    public static void ValidateRange(this ICollection coll, Range range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range.Index < 0 || range.Index > coll.Count)
        throw new ArgumentOutOfRangeException("Index is out of range", "range");
      if (range.Count < 0 || range.Count > coll.Count - range.Index)
        throw new ArgumentOutOfRangeException("Count is out of range", "range");
    }

    public static void ValidateRange<T>(this ICollection<T> coll, Range range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range.Index < 0 || range.Index > coll.Count)
        throw new ArgumentOutOfRangeException("Index is out of range", "range");
      if (range.Count < 0 || range.Count > coll.Count - range.Index)
        throw new ArgumentOutOfRangeException("Count is out of range", "range");
    }

    public static void ValidateRange<T>(this IReadOnlyCollection<T> coll, Range range)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (range.Index < 0 || range.Index > coll.Count)
        throw new ArgumentOutOfRangeException("Index is out of range", "range");
      if (range.Count < 0 || range.Count > coll.Count - range.Index)
        throw new ArgumentOutOfRangeException("Counit is out of range", "range");
    }

    public static void AddRange<T>(this ICollection<T> coll, IEnumerable<T> other)
		{
			if (coll == null)
				throw new ArgumentNullException("coll");
			if (other == null)
				throw new ArgumentNullException("other");
			//
			using (IEnumerator<T> e = other.GetEnumerator())
				while (e.MoveNext())
					coll.Add(e.Current);
		}

    public static void RemoveRange<T>(this ICollection<T> coll, IEnumerable<T> other)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (other == null)
        throw new ArgumentNullException("other");
      //
      using (IEnumerator<T> e = other.GetEnumerator())
        while (e.MoveNext())
          coll.Remove(e.Current);
    }

    public static ICollection<T> Repeat<T>(this T @default, int count)
    {
      return new CountedCollection<T>(Enumerable.Repeat<T>(@default, count), count);
    }

    public static IReadOnlyCollection<T> RepeatReadOnly<T>(this T @default, int count)
    {
      return new CountedCollection<T>(Enumerable.Repeat<T>(@default, count), count);
    }

    public static ICollection<T> Counted<T>(this IEnumerable<T> coll, int count)
    {
      return new CountedCollection<T>(coll, count);
    }

    public static IReadOnlyCollection<T> CountedReadOnly<T>(this IEnumerable<T> coll, int count)
    {
      return new CountedCollection<T>(coll, count);
    }

    public static int PeekCount(this IEnumerable coll)
    {
      var c = coll as ICollection;
      return c != null ? c.Count : -1;
    }

    public static int PeekCount<T>(this IEnumerable<T> coll)
    {
      var c = coll as ICollection<T>;
      if (c != null)
        return c.Count;
      var roc = coll as IReadOnlyCollection<T>;
      if (roc != null)
        return roc.Count;
      return -1;
    }

    public static string Format<T>(this IEnumerable<T> coll, Func<T, string> itemFormatter, string itemDelimiter)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (itemFormatter == null)
        throw new ArgumentNullException("itemFormatter");

      return Format<T>(coll, (t, i) => itemFormatter(t), itemDelimiter);
    }

    public static string Format<T>(this IEnumerable<T> coll, Func<T, int, string> itemFormatter, string itemDelimiter)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (itemFormatter == null)
        throw new ArgumentNullException("itemFormatter");

      StringBuilder sb = new StringBuilder();
      using (var e = coll.GetEnumerator())
      {
        if (e.MoveNext())
        {
          int i = 0;
          sb.Append(itemFormatter(e.Current, i++));
          while (e.MoveNext())
          {
            sb.Append(itemDelimiter);
            sb.Append(itemFormatter(e.Current, i++));
          }
        }
      }
      return sb.ToString();
    }

    #endregion
    #region Embedded types

    private sealed class CountedCollection<T> : ICollection<T>, IReadOnlyCollection<T>
    {
      private readonly IEnumerable<T> _coll;
      private readonly int _count;

      internal CountedCollection(IEnumerable<T> coll, int count)
      {
        if (coll == null)
          throw new ArgumentNullException("coll");
        if (count < 0)
          throw new ArgumentOutOfRangeException("count");

        _coll = coll;
        _count = count;
      }

      public IEnumerable<T> GetEnumerator()
      {
        int index = 0;
        using (var e = _coll.GetEnumerator())
          for (; index < _count && e.MoveNext(); index++)
            yield return e.Current;
        for (; index < _count; index++)
          yield return default(T);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return _coll.GetEnumerator();
      }

      IEnumerator<T> IEnumerable<T>.GetEnumerator()
      {
        return _coll.GetEnumerator();
      }

      int IReadOnlyCollection<T>.Count
      {
        get { return _count; }
      }

      int ICollection<T>.Count
      {
        get { return _count; }
      }

      bool ICollection<T>.IsReadOnly
      {
        get { return true; }
      }

      void ICollection<T>.Add(T item)
      {
        throw new NotSupportedException();
      }

      void ICollection<T>.Clear()
      {
        throw new NotSupportedException();
      }

      bool ICollection<T>.Remove(T item)
      {
        throw new NotSupportedException();
      }

      bool ICollection<T>.Contains(T item)
      {
        return _coll.Contains(item);
      }

      void ICollection<T>.CopyTo(T[] array, int arrayIndex)
      {
        if (array == null)
          throw new ArgumentNullException("array");
        if (arrayIndex < 0 || _count > array.Length - arrayIndex)
          throw new ArgumentOutOfRangeException("arrayIndex");

        foreach (var item in _coll)
          array[arrayIndex++] = item;
      }
    }

    #endregion
  }
}
