using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace PowerLib.System.IO.Streamed
{
  public abstract class StreamedCollection<T> : IList<T>, IDisposable
  {
    private Stream _stream;
    private bool _leaveOpen;
    private volatile int _stamp;
    private int? _count;
    private List<long> _offsets;

    #region Constructor

    protected StreamedCollection(Stream stream, bool leaveOpen, bool boostAccess)
    {
      _stream = stream;
      _leaveOpen = leaveOpen;
      _offsets = boostAccess ? new List<long>() : null;
    }

    #endregion
    #region Properties
    #region Internal properties

    protected Stream BaseStream
    {
      get { return _stream; }
    }

    protected virtual ExpandMethod ExpandMethod
    {
      get { return ExpandMethod.None; }
    }

    protected abstract int DataOffset { get; }

    protected abstract int? FixedItemSize
    {
      get;
    }

    #endregion
    #region Public properties

    public virtual int Count
    {
      get
      {
        if (_count.HasValue)
          return _count.Value;
        int count = ReadCount();
        if (_offsets != null)
          _count = count;
        return count;
      }
      protected set
      {
        if (_count.HasValue && _count.Value == value)
          return;
        WriteCount(value);
        if (_offsets != null)
          _count = value;
      }
    }

    public virtual T this[int index]
    {
      get
      {
        BaseStream.Position = GetItemOffset(index);
        return ReadItem(index);
      }
      set
      {
        long offset = BaseStream.Position = GetItemOffset(index);
        int oldSize = FixedItemSize.HasValue ? FixedItemSize.Value : ReadItemSize(index);
        int newSize = FixedItemSize.HasValue ? FixedItemSize.Value : GetItemSize(value);
        if (oldSize != newSize)
        {
          _stamp++;
          BaseStream.Position = offset + oldSize;
          long length = BaseStream.Length - (offset + oldSize);
          BaseStream.Move(offset + newSize, length, 1024, ExpandMethod);
          BaseStream.SetLength(offset + newSize + length);
          //  Update offsets
          if (_offsets != null)
            if (_offsets.Count > index + 1)
              _offsets.RemoveRange(index + 1, _offsets.Count - index - 1);
        }
        BaseStream.Position = offset;
        WriteItem(index, value);
      }
    }

    #endregion
    #endregion
    #region Methods
    #region Internal methods

    protected virtual long GetItemOffset(int index)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");
      int total = ReadCount();
      if (index > total)
        throw new ArgumentOutOfRangeException("index");
      else if (index == total)
        return BaseStream.Length;
      //
      BaseStream.Position = DataOffset;
      //
      if (FixedItemSize.HasValue)
        return BaseStream.Position + FixedItemSize.Value * index;
      //
      if (_offsets != null && index < _offsets.Count)
        return _offsets[index];
      //
      int current = _offsets != null && _offsets.Count > 0 ? _offsets.Count - 1 : 0;
      long offset = _offsets != null && _offsets.Count > 0 ? _offsets[current] : BaseStream.Position;
      if (_offsets != null && _offsets.Count == 0)
        _offsets.Insert(0, offset);
      //
      while (current < index)
      {
        int size = ReadItemSize(current);
        BaseStream.Position = (offset += size);
        current++;
        if (_offsets != null)
          _offsets.Insert(current, offset);
      }
      return offset;
    }

    protected abstract bool EqualItem(T x, T y);

    protected abstract int GetItemSize(T value);

    protected abstract int ReadItemSize(int index);

    protected abstract T ReadItem(int index);

    protected abstract void WriteItem(int index, T value);

    protected abstract int ReadCount();

    protected abstract void WriteCount(int count);

    #endregion
    #region Public methods

    public virtual void CopyTo(T[] array, int arrayIndex)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (arrayIndex < 0 || arrayIndex > array.Length)
        throw new ArgumentOutOfRangeException("arrayIndex");
      int count = ReadCount();
      if (array.Length - arrayIndex < count)
        throw new ArgumentException("It is not enough space to store source collection.");

      BaseStream.Position = GetItemOffset(0);
      for (int i = 0; i < count; i++)
        array[arrayIndex + i] = ReadItem(i);
    }

    public virtual void Clear()
    {
      WriteCount(0);
      BaseStream.Position = GetItemOffset(0);
      BaseStream.SetLength(BaseStream.Position);
    }

    public virtual void Add(T value)
    {
      Insert(ReadCount(), value);
    }

    public virtual void AddRange(IEnumerable<T> coll)
    {
      InsertRange(ReadCount(), coll);
    }

    public virtual void Insert(int index, T value)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");
      int total = ReadCount();
      if (index > total)
        throw new ArgumentOutOfRangeException("index");

      _stamp++;
      //  Update offsets cache
      if (_offsets != null)
        if (_offsets.Count > index + 1)
          _offsets.RemoveRange(index + 1, _offsets.Count - index - 1);
      //
      int newSize = FixedItemSize.HasValue ? FixedItemSize.Value : GetItemSize(value);
      long offset = BaseStream.Position = GetItemOffset(index);
      long length = BaseStream.Length - offset;
      BaseStream.Move(offset + newSize, length, 1024, ExpandMethod);
      BaseStream.SetLength(offset + newSize + length);
      BaseStream.Position = offset;
      WriteItem(index, value);
      WriteCount(total + 1);
      //  Update offsets cache
      if (_offsets != null)
        if (_offsets.Count == index)
          _offsets.Insert(index, offset);
    }

    public virtual void InsertRange(int index, IEnumerable<T> coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");
      int total = ReadCount();
      if (index > total)
        throw new ArgumentOutOfRangeException("index");

      _stamp++;
      //  Update offsets cache
      if (_offsets != null)
        if (_offsets.Count > index + 1)
          _offsets.RemoveRange(index + 1, _offsets.Count - index - 1);
      //
      long newSize = coll.Aggregate(0L, (a, t) => a += FixedItemSize.HasValue ? FixedItemSize.Value : GetItemSize(t));
      long offset = BaseStream.Position = GetItemOffset(index);
      long length = BaseStream.Length - offset;
      BaseStream.Move(offset + newSize, length, 1024, ExpandMethod);
      BaseStream.SetLength(offset + newSize + length);
      BaseStream.Position = offset;
      int i = 0;
      foreach (var item in coll)
      {
        //  Update offsets cache
        if (_offsets != null)
          if ((_offsets.Count == 0 || i > 0) && _offsets.Count == (index + 1))
            _offsets.Insert(index + i, BaseStream.Position);
        //
        WriteItem(index + i++, item);
      }
      WriteCount(total + i);
    }

    public virtual void InsertRepeat(int index, T value, int count)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");
      int total = ReadCount();
      if (index > total)
        throw new ArgumentOutOfRangeException("index");
      if (count == 0)
        return;

      _stamp++;
      //  Update offsets cache
      if (_offsets != null)
        if (_offsets.Count > index + 1)
          _offsets.RemoveRange(index + 1, _offsets.Count - index - 1);
      //
      long newSize = (FixedItemSize.HasValue ? FixedItemSize.Value : GetItemSize(value)) * count;
      long offset = BaseStream.Position = GetItemOffset(index);
      long length = BaseStream.Length - offset;
      BaseStream.Move(offset + newSize, length, 1024, ExpandMethod);
      BaseStream.SetLength(offset + newSize + length);
      BaseStream.Position = offset;
      for (int i = 0; i < count; i++)
      {
        //  Update offsets cache
        if (_offsets != null)
          if ((_offsets.Count == 0 || i > 0) && _offsets.Count == (index + 1))
            _offsets.Insert(index + i, BaseStream.Position);
        //
        WriteItem(index + i, value);
      }
      WriteCount(total + count);
    }

    public virtual bool Remove(T value)
    {
      int index = IndexOf(value);
      if (index >= 0)
        RemoveAt(index);
      return index >= 0;
    }

    public virtual void RemoveAt(int index)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");
      int total = ReadCount();
      if (index >= total)
        throw new ArgumentOutOfRangeException("index");

      _stamp++;
      long offset = BaseStream.Position = GetItemOffset(index);
      int size = ReadItemSize(index);
      long length = BaseStream.Length - offset - size;
      BaseStream.Position = offset + size;
      BaseStream.Move(offset, length, 1024, ExpandMethod);
      BaseStream.SetLength(offset + length);
      WriteCount(total - 1);
      //  Update offsets cache
      if (_offsets != null)
        if (_offsets.Count > index + (index == total - 1 ? 0 : 1))
          _offsets.RemoveRange(index + (index == total - 1 ? 0 : 1), _offsets.Count - index - (index == total - 1 ? 0 : 1));
    }

    public virtual void RemoveRange(int index, int count)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");
      int total = ReadCount();
      if (index > total)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > total - index)
        throw new ArgumentOutOfRangeException("count");
      if (count == 0)
        return;

      _stamp++;
      long offset = GetItemOffset(index);
      BaseStream.Position = GetItemOffset(index + count);
      long length = BaseStream.Length - BaseStream.Position;
      BaseStream.Move(offset, length, 1024, ExpandMethod);
      BaseStream.SetLength(offset + length);
      WriteCount(total - count);
      //  Update offsets cache
      if (_offsets != null)
        if (_offsets.Count > index + (index + count == total ? 0 : 1))
          _offsets.RemoveRange(index + (index + count == total ? 0 : 1), _offsets.Count - index - (index + count == total ? 0 : 1));
    }

    public virtual void SetRange(int index, ICollection<T> coll)
    {
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");
      int total = ReadCount();
      if (index > total)
        throw new ArgumentOutOfRangeException("index");
      if (coll.Count > total - index)
        throw new ArgumentOutOfRangeException("coll");
      if (coll.Count == 0)
        return;

      _stamp++;
      //  Update offsets cache
      if (_offsets != null)
        if (_offsets.Count > index + 1)
          _offsets.RemoveRange(index + 1, _offsets.Count - index - 1);
      //
      long newSize = coll.Aggregate(0L, (a, t) => a += FixedItemSize.HasValue ? FixedItemSize.Value : GetItemSize(t));
      long offset = GetItemOffset(index);
      BaseStream.Position = GetItemOffset(index + coll.Count);
      long length = BaseStream.Length - BaseStream.Position;
      BaseStream.Move(offset + newSize, length, 1024, ExpandMethod);
      BaseStream.SetLength(offset + newSize + length);
      BaseStream.Position = offset;
      //
      int i = 0;
      foreach (var item in coll)
      {
        //  Update offsets cache
        if (_offsets != null)
          if (i > 0 && _offsets.Count >= index)
            _offsets.Insert(index + i, BaseStream.Position);
        //
        WriteItem(index + i++, item);
      }
    }

    public virtual void SetRepeat(int index, T value, int count)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");
      int total = ReadCount();
      if (index > total)
        throw new ArgumentOutOfRangeException("index");
      if (count > total - index)
        throw new ArgumentOutOfRangeException("coll");
      if (count == 0)
        return;

      _stamp++;
      //  Update offsets cache
      if (_offsets != null)
        if (_offsets.Count > index + 1)
          _offsets.RemoveRange(index + 1, _offsets.Count - index - 1);
      //
      long newSize = (FixedItemSize.HasValue ? FixedItemSize.Value : GetItemSize(value)) * count;
      long offset = GetItemOffset(index);
      BaseStream.Position = GetItemOffset(index + count);
      long length = BaseStream.Length - BaseStream.Position;
      BaseStream.Move(offset + newSize, length, 1024, ExpandMethod);
      BaseStream.SetLength(offset + newSize + length);
      BaseStream.Position = offset;
      //
      for (int i = 0; i < count; i++)
      {
        //  Update offsets cache
        if (_offsets != null)
          if (i > 0 && _offsets.Count >= index)
            _offsets.Insert(index + i, BaseStream.Position);
        //
        WriteItem(index + i, value);
      }
    }

    public virtual bool Contains(T value)
    {
      return IndexOf(value) >= 0;
    }

    public virtual int IndexOf(T value)
    {
      int count = ReadCount();
      BaseStream.Position = GetItemOffset(0);
      for (int i = 0; i < count; i++)
        if (EqualItem(value, ReadItem(i)))
          return i;
      return -1;
    }

    public virtual IEnumerable<T> EnumerateRange(int index, int count)
    {
      int total = ReadCount();
      if (index < 0 || index > total)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > total - index)
        throw new ArgumentOutOfRangeException("count");

      int stamp = _stamp;
      BaseStream.Position = GetItemOffset(index);
      while (count-- > 0)
      {
        if (_stamp != stamp)
          throw new InvalidOperationException();
        T item = ReadItem(index++);
        long offset = BaseStream.Position;
        yield return item;
        if (BaseStream.Position != offset)
          BaseStream.Position = offset;
      }
    }

    public virtual byte[] ToByteArray()
    {
      long offset = BaseStream.Position;
      try
      {
        BaseStream.Position = 0L;
        return BaseStream.ReadBytes((int)BaseStream.Length);
      }
      finally
      {
        BaseStream.Position = offset;
      }
    }

    #endregion
    #endregion
    #region Interfaces implementation

    void IDisposable.Dispose()
    {
      if (!_leaveOpen)
        _stream.Dispose();
    }

    bool ICollection<T>.IsReadOnly
    {
      get { return false; }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<T>)this).GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return EnumerateRange(0, Count).GetEnumerator();
    }

    #endregion
  }
}
