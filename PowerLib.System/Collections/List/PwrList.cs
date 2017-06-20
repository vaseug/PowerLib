using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using PowerLib.System;
using PowerLib.System.Resources;
using PowerLib.System.Numerics;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// Represents strongly typed list of objects that can be accessed by index and organaized as ring list.
	/// </summary>
	/// <typeparam name="T">The type of elements in the list</typeparam>
	public sealed class PwrList<T> : IList<T>, IList, ICollection<T>, ICollection, IEnumerable<T>, IEnumerable, IAutoCapacitySupport, IStampSupport
	{
		private const float MinTrimFactor = 0.1f;
		private const float MaxTrimFactor = 0.9f;
		private const float MinGrowFactor = 1.1f;
		private const float MaxGrowFactor = 10;
		private const float DefaultTrimFactor = 0.9f;
		private const float DefaultGrowFactor = 2f;
		private const int DefaultCapacity = 4;
		private readonly static T[] _empty = new T[0];
		private T[] _array = _empty;
		private int _start = 0;
		private int _count = 0;
		private int _stamp = 0;
		private float _trimFactor = DefaultTrimFactor;
		private float _growFactor = DefaultGrowFactor;
		private bool _autoTrim = false;
		private CollectionRestrictions _restriction = CollectionRestrictions.None;
		private object _syncRoot = null;

		#region Constructors

		public PwrList()
		{
		}

		public PwrList(int capacity)
			: this()
		{
      Capacity = capacity;
    }

		public PwrList(IEnumerable<T> coll)
			: this()
		{
      AddRange(coll);
      _restriction = CollectionRestrictions.None;
    }

		public PwrList(IEnumerable<T> coll, CollectionRestrictions restriction)
			: this()
		{
      AddRange(coll);
      _restriction = restriction;
    }

		public PwrList(T value, int count)
			: this()
		{
			AddRepeat(value, count);
			_restriction = CollectionRestrictions.None;
		}

		public PwrList(T value, int count, CollectionRestrictions restriction)
			: this()
		{
			AddRepeat(value, count);
			_restriction = restriction;
		}

		#endregion
		#region Properties

		public CollectionRestrictions Restriction
		{
			get
			{
				return _restriction;
			}
		}

		public bool AutoTrim
		{
			get
			{
				return _autoTrim;
			}
			set
			{
				_autoTrim = value ;
			}
		}

		public float LoadFactor
		{
			get
			{
				return (_array.Length == 0) ? 0f : (float)_count / (float)_array.Length;
			}
		}

		public float TrimFactor
		{
			get
			{
				return _trimFactor;
			}
			set
			{
				if (value < MinTrimFactor || value > MaxTrimFactor)
					throw new ArgumentOutOfRangeException();
				//
				_trimFactor = value ;
			}
		}

		public float GrowFactor
		{
			get
			{
				return _growFactor;
			}
			set
			{
				if (value < MinGrowFactor || value > MaxGrowFactor)
					throw new ArgumentOutOfRangeException();
				//
				_growFactor = value ;
			}
		}

		public int Capacity
		{
			get
			{
				return _array.Length;
			}
			set
			{
				if (value < _count)
					throw new ArgumentOutOfRangeException();

				if (value != Capacity)
					Resize(value);
			}
		}

		public int Count
		{
			get
			{
				return _count;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= _count)
					throw new ArgumentOutOfRangeException("index");
				//
				return GetItem(index);
			}
			set
			{
				if (index < 0 || index >= _count)
					throw new ArgumentOutOfRangeException("index");
				if (_restriction == CollectionRestrictions.ReadOnly)
          throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsReadOnly]);
				//
				SetItem(index, value);
				_stamp++;
			}
		}

		#endregion
		#region Methods
		#region Internal methods

		private T GetItem(int index)
		{
			index += ((index < _array.Length - _start) ? _start : _start - _array.Length);
			return _array[index];
		}

		private void SetItem(int index, T value)
		{
			index += ((index < _array.Length - _start) ? _start : _start - _array.Length);
			_array[index] = value ;
		}

		private void AllocBlock(int index, int count)
		{
			int newCount = _count + count;
			if (newCount > Capacity)
			{
				int newCapacity = (Capacity == 0) ? DefaultCapacity : (int)(Capacity * GrowFactor + 1);
				Capacity = (newCount < newCapacity) ? newCapacity : newCount;
			}
			int oldCount = _count;
			_count = newCount;
			if (index < (oldCount >> 1))
			{
        int source = _start;
				int target = _start - count + (_start >= count ? 0 : _array.Length);
        RingBuffer.Copy(_array, source, 0, 0, _array, target, 0, 0, index, 0, 1, false);
        int clear = target + index - (index > _array.Length - target ? _array.Length : 0);
        RingBuffer.Clear(_array, clear, 0, 0, count, 0, 1);
        _start = target;
      }
      else
			{
        int source = _start + index - (index > _array.Length - _start ? _array.Length : 0);
        int target = _start + index + count - (index + count > _array.Length - _start ? _array.Length : 0);
				RingBuffer.Copy(_array, source, 0, 0, _array, target, 0, 0, oldCount - index, 0, 1, false);
        RingBuffer.Clear(_array, source, 0, 0, count, 0, 1);
      }
		}

		private void FreeBlock(int index, int count)
		{
			int newCount = _count - count;
			if (index < (newCount >> 1))
			{
        int source = _start;
        int target = _start + count - (count > _array.Length - _start ? _array.Length : 0);
        RingBuffer.Copy(_array, source, 0, 0, _array, target, 0, 0, index, 0, 1, false);
        RingBuffer.Clear(_array, source, 0, 0, count, 0, 1);
        _start = target;
      }
			else
			{
        int source = _start + index + count - (index + count > _array.Length - _start ? _array.Length : 0);
        int target = _start + index - (index > _array.Length - _start ? _array.Length : 0);
        RingBuffer.Copy(_array, source, 0, 0, _array, target, 0, 0, newCount - index, 0, 1, false);
        int clear = _start + newCount - (newCount > _array.Length - _start ? _array.Length : 0);
				RingBuffer.Clear(_array, clear, 0, 0, count, 0, 1);
			}
			_count = newCount;
			if (AutoTrim)
				TrimExcess();
		}

		private void Resize(int capacity)
		{
      if (capacity < _count || capacity == _array.Length)
        return;
			else if (capacity == 0)
				_array = _empty;
			else
			{
				T[] target = new T[capacity];
				RingBuffer.Copy(_array, _start, 0, 0, target, 0, 0, 0, _count, 0, 1, false);
				_array = target;
				_start = 0;
			}
		}

		internal void Restrict(CollectionRestrictions restriction)
		{
			if (_restriction != CollectionRestrictions.None)
				throw new InvalidOperationException("Collection is already restricted");
			//
			_restriction = restriction;
		}

		#endregion
		#region Control methods

		public PwrList<T> AsReadOnly()
		{
			return new PwrList<T>(this, CollectionRestrictions.ReadOnly);
		}

		public PwrList<T> AsFixedSize()
		{
			return new PwrList<T>(this, CollectionRestrictions.FixedSize);
		}

		public void TrimExcess()
		{
			if (_count > 0 && LoadFactor < TrimFactor)
				Resize(_count);
		}

		#endregion
		#region Manipulation methods

		public void Clear()
		{
			switch (_restriction)
			{
				case CollectionRestrictions.FixedSize:
					throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedSize]);
				case CollectionRestrictions.FixedLayout:
          throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);
				case CollectionRestrictions.ReadOnly:
          throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsReadOnly]);
			}
			//
			if (_count == 0)
				return;
		  FreeBlock(0, _count);
			_stamp++;
		}

		public void ExpandRange(int index, int count)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if ((_restriction & CollectionRestrictions.FixedSize) != 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedSize]);
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0 && index < _count)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);
			//
		  AllocBlock(index, count);
			_stamp++;
		}

		public void Add(T value)
		{
			Insert(_count, value);
		}

		public void AddRepeat(T value, int count)
		{
			InsertRepeat(_count, value, count);
		}

		public void AddRange(IEnumerable<T> coll)
		{
			InsertRange(_count, coll);
		}

		public void Insert(int index, T value)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if ((_restriction & CollectionRestrictions.FixedSize) != 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedSize]);
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0 && index < _count)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);

			AllocBlock(index, 1);
			SetItem(index, value);
			_stamp++;
		}

		public void InsertRepeat(int index, T value, int count)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0)
				throw new ArgumentOutOfRangeException("count");
			if ((_restriction & CollectionRestrictions.FixedSize) != 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedSize]);
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0 && index < _count)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);
			//
			if (count == 0)
				return;
			AllocBlock(index, count);
			for (int i = 0; i < count; i++)
				SetItem(index++, value);
			_stamp++;
		}

		public void InsertRange(int index, IEnumerable<T> coll)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (coll == null)
				throw new ArgumentNullException("source");
			if ((_restriction & CollectionRestrictions.FixedSize) != 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedSize]);
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0 && index < _count)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);
			//
			ICollection<T> c = coll as ICollection<T>;
			if (c != null)
			{
				if (c.Count == 0)
					return;
				AllocBlock(index, c.Count);
				using (IEnumerator<T> e = coll.GetEnumerator())
					while (e.MoveNext())
						SetItem(index++, e.Current);
			}
			else
			{
				using (IEnumerator<T> e = coll.GetEnumerator())
				{
					while (e.MoveNext())
					{
						AllocBlock(index, 1);
						SetItem(index++, e.Current);
					}
				}
			}
			_stamp++;
		}

		public void InsertRangeReverse(int index, IEnumerable<T> coll)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (coll == null)
				throw new ArgumentNullException("source");
			if ((_restriction & CollectionRestrictions.FixedSize) != 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedSize]);
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0 && index < _count)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);
			//
			ICollection<T> c = coll as ICollection<T>;
			if (c != null)
			{
				if (c.Count == 0)
					return;
				AllocBlock(index, c.Count);
				using (IEnumerator<T> e = coll.GetEnumerator())
				{
					if (e.MoveNext())
					{
						do
							SetItem(index, e.Current);
						while (e.MoveNext());
						_stamp++;
					}
				}
			}
			else
			{
				using (IEnumerator<T> e = coll.GetEnumerator())
				{
					if (e.MoveNext())
					{
						do
						{
							AllocBlock(index, 1);
							SetItem(index, e.Current);
						}
						while (e.MoveNext());
						_stamp++;
					}
				}
			}
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
			if (index < 0 || index >= _count)
				throw new ArgumentOutOfRangeException("index");
			if ((_restriction & CollectionRestrictions.FixedSize) != 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedSize]);
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);

			FreeBlock(index, 1);
			_stamp++;
		}

		public void RemoveRange(int index, int count)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if ((_restriction & CollectionRestrictions.FixedSize) != 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedSize]);
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);
			//
			if (count == 0)
				return;
			FreeBlock(index, count);
			_stamp++;
		}

		public void Move(int srcIndex, int dstIndex)
		{
			if (srcIndex < 0 || srcIndex >= _count)
				throw new ArgumentOutOfRangeException("srcIndex");
			if (dstIndex < 0 || dstIndex >= _count)
				throw new ArgumentOutOfRangeException("dstIndex");
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0)
        throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);
			//
			if (srcIndex == dstIndex)
				return;
			T temp = GetItem(srcIndex);
			if (srcIndex < dstIndex)
        RingBuffer.Copy(_array, _start + srcIndex + 1, 0, 0, _array, _start + srcIndex, 0, 0, dstIndex - srcIndex, 0, 1, false);
			else
        RingBuffer.Copy(_array, _start + dstIndex, 0, 0, _array, _start + dstIndex + 1, 0, 0, srcIndex - dstIndex, 0, 1, true);
			SetItem(dstIndex, temp);
			_stamp++;
		}

		public void MoveRange(int srcIndex, int dstIndex, int count)
		{
			if (srcIndex < 0 || srcIndex > _count)
				throw new ArgumentOutOfRangeException("srcIndex");
			if (dstIndex < 0 || dstIndex > _count)
				throw new ArgumentOutOfRangeException("dstIndex");
			if (count < 0 || count > _count - srcIndex || count > _count - dstIndex)
				throw new ArgumentOutOfRangeException("count");
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);
			//
			if (srcIndex == dstIndex)
				return;
			T[] buffer = new T[count];
      RingBuffer.Copy(_array, _start + srcIndex, 0, 0, buffer, 0, 0, 0, count, 0, 1, false);
			if (srcIndex < dstIndex)
        RingBuffer.Copy(_array, _start + srcIndex + count, 0, 0, _array, _start + srcIndex, 0, 0, dstIndex - srcIndex, 0, 1, false);
			else if (srcIndex > dstIndex)
        RingBuffer.Copy(_array, _start + dstIndex, 0, 0, _array, _start + dstIndex + count, 0, 0, srcIndex - dstIndex, 0, 1, true);
      RingBuffer.Copy(buffer, 0, 0, 0, _array, _start + dstIndex, 0, 0, count, 0, 1, false);
			_stamp++;
		}

		public void Swap(int xIndex, int yIndex)
		{
			if (xIndex < 0 || xIndex >= _count)
				throw new ArgumentOutOfRangeException("xIndex");
			if (yIndex < 0 || yIndex >= _count)
				throw new ArgumentOutOfRangeException("yIndex");
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);
			//
			if (xIndex == yIndex)
				return;
			T temp = this[xIndex];
			SetItem(xIndex, GetItem(yIndex));
			SetItem(yIndex, temp);
			_stamp++;
		}

		public void SwapRange(int xIndex, int yIndex, int count)
		{
			if (xIndex < 0 || xIndex > _count)
				throw new ArgumentOutOfRangeException("xIndex");
			if (yIndex < 0 || yIndex > _count)
				throw new ArgumentOutOfRangeException("yIndex");
			if (count < 0 || count > _count - xIndex || count > _count - yIndex)
				throw new ArgumentOutOfRangeException("count");
			if (xIndex == yIndex || count > (yIndex - xIndex).Abs())
				throw new ArgumentException("Ranges intersected");
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);
			//
			for (int i = 0; i < count; i++)
			{
				T temp = GetItem(xIndex + i);
				SetItem(xIndex + i, GetItem(yIndex + i));
				SetItem(yIndex + i, temp);
			}
			_stamp++;
		}

		public void SwapRanges(int xIndex, int xCount, int yIndex, int yCount)
		{
			if (xIndex < 0 || xIndex > _count)
				throw new ArgumentOutOfRangeException("xIndex");
			if (yIndex < 0 || yIndex > _count)
				throw new ArgumentOutOfRangeException("yIndex");
			if (xCount < 0 || xCount > _count - xIndex)
				throw new ArgumentOutOfRangeException("xCount");
			if (yCount < 0 || yCount > _count - yIndex)
				throw new ArgumentOutOfRangeException("yCount");
			if (xIndex == yIndex || xIndex < yIndex && xIndex + xCount > yIndex || xIndex > yIndex && yIndex + yCount > xIndex)
				throw new ArgumentException("Ranges intersected");
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);
			//
			for (int i = 0, c = Comparable.Min(xCount, yCount); i < c; i++)
			{
				T temp = GetItem(xIndex + i);
				SetItem(xIndex + i, GetItem(yIndex + i));
				SetItem(yIndex + i, temp);
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
        RingBuffer.Copy(_array, _start + lowerIndex + upperCount, 0, 0, buffer, 0, 0, 0, lowerCount - upperCount, 0, 1, false);
        RingBuffer.Copy(_array, _start + lowerIndex + lowerCount, 0, 0, _array, _start + lowerIndex + upperCount, 0, 0, upperIndex + upperCount - (lowerIndex + lowerCount), 0, 1, false);
        RingBuffer.Copy(buffer, 0, 0, 0, _array, _start + upperIndex + upperCount - (lowerCount - upperCount), 0, 0, lowerCount - upperCount, 0, 1, false);
			}
			else if (lowerCount < upperCount)
			{
				T[] buffer = new T[upperCount - lowerCount];
        RingBuffer.Copy(_array, _start + upperIndex + lowerCount, 0, 0, buffer, 0, 0, 0, upperCount - lowerCount, 0, 1, false);
        RingBuffer.Copy(_array, _start + lowerIndex + lowerCount, 0, 0, _array, _start + lowerIndex + upperCount, 0, 0, upperIndex - lowerIndex, 0, 1, true);
        RingBuffer.Copy(buffer, 0, 0, 0, _array, _start + lowerIndex + lowerCount, 0, 0, upperCount - lowerCount, 0, 1, false);
			}
			_stamp++;
		}

		public void Reverse()
		{
			Reverse(0, _count);
		}

		public void Reverse(int index, int count)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if ((_restriction & CollectionRestrictions.FixedLayout) != 0)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionHasFixedLayout]);
			//
			for (int i = 0, c = _count >> 1; i < c; i++)
			{
				T value = GetItem(index + i);
				SetItem(index + i, GetItem(index + count - i));
				SetItem(index + count - i, value);
			}
			_stamp++;
		}

		#endregion
		#region Transformation methods

		public void Fill(T value)
		{
			Fill(0, _count, value);
		}

		public void Fill(int index, int count, T value)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if ((_restriction & CollectionRestrictions.ReadOnlyValue) != 0)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsReadOnly]);
			//
			for (int i = index; i < index + count; i++)
				SetItem(i, value);
		}

		public void Apply(Action<T> action)
		{
			Apply(0, _count, action);
		}

		public void Apply(int index, int count, Action<T> action)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if (action == null)
				throw new ArgumentNullException("action");
			//
			for (int i = index; i < index + count; i++)
				action(GetItem(i));
		}

		public void Sort(Comparison<T> comparison)
		{
			Sort(0, _count, comparison);
		}

		public void Sort(int index, int count, Comparison<T> comparison)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			if (_restriction == CollectionRestrictions.ReadOnly)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsReadOnly]);
			//
			if (count == 0)
				return;
			int left = index;
			int right = index + count - 1;
			do
			{
				int lower = left;
				int upper = right;
				int middle = lower + ((upper - lower) >> 1);
				if (comparison(GetItem(lower), GetItem(middle)) > 0)
					Swap(lower, middle);
				if (comparison(GetItem(lower), GetItem(upper)) > 0)
					Swap(lower, upper);
				if (comparison(GetItem(middle), GetItem(upper)) > 0)
					Swap(middle, upper);
				T median = GetItem(middle);
				do
				{
					try
					{
						while (comparison(GetItem(lower), median) < 0)
							lower++;
						while (comparison(median, GetItem(upper)) < 0)
							upper--;
					}
					catch (Exception ex)
					{
						throw new InvalidOperationException("Invalicompare", ex);
					}
					if (lower > upper)
						break;
					else if (lower < upper)
						Swap(lower, upper);
					lower++;
					upper--;
				}
				while (lower <= upper);
				if (upper - left <= right - lower)
				{
					if (left < upper)
						Sort(left, upper - left + 1, comparison);
					left = lower;
				}
				else
				{
					if (lower < right)
						Sort(lower, right - lower + 1, comparison);
					right = upper;
				}
			}
			while (left < right);
		}

		public void Sort<K>(PwrList<K> keys, Comparison<K> comparison)
		{
			Sort(keys, 0, _count, comparison);
		}

		public void Sort<K>(PwrList<K> keys, int index, int count, Comparison<K> comparison)
		{
			if (keys == null)
				throw new ArgumentNullException("keys");
			if (keys._count != _count)
				throw new ArgumentException("Length of keys does not match length of values", "keys");
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			if (_restriction == CollectionRestrictions.ReadOnly)
				throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionIsReadOnly]);
			//
			if (count == 0)
				return;
			int left = index;
			int right = index + count - 1;
			do
			{
				int lower = left;
				int upper = right;
				int middle = lower + ((upper - lower) >> 1);
				if (comparison(keys.GetItem(lower), keys.GetItem(middle)) > 0)
				{
					keys.Swap(lower, middle);
					Swap(lower, middle);
				}
				if (comparison(keys.GetItem(lower), keys.GetItem(upper)) > 0)
				{
					keys.Swap(lower, upper);
					Swap(lower, upper);
				}
				if (comparison(keys.GetItem(middle), keys.GetItem(upper)) > 0)
				{
					keys.Swap(middle, upper);
					Swap(middle, upper);
				}
				K median = keys.GetItem(middle);
				do
				{
					try
					{
						while (comparison(keys.GetItem(lower), median) < 0)
							lower++;
						while (comparison(median, keys.GetItem(upper)) < 0)
							upper--;
					}
					catch (Exception ex)
					{
						throw new InvalidOperationException("Invalicompare", ex);
					}
					if (lower > upper)
						break;
					else if (lower < upper)
					{
						keys.Swap(lower, upper);
						Swap(lower, upper);
					}
					lower++;
					upper--;
				}
				while (lower <= upper);
				if (upper - left <= right - lower)
				{
					if (left < upper)
						Sort(keys, left, upper - left + 1, comparison);
					left = lower;
				}
				else
				{
					if (lower < right)
						Sort(keys, lower, right - lower + 1, comparison);
					right = upper;
				}
			}
			while (left < right);
		}

		#endregion
		#region Extraction methods

		public PwrList<TResult> Convert<TResult>(Converter<T, TResult> converter)
		{
			return Convert(0, _count, converter);
		}

		public PwrList<TResult> Convert<TResult>(int index, int count, Converter<T, TResult> converter)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if (converter == null)
				throw new ArgumentNullException("converter");
			//
			PwrList<TResult> list = new PwrList<TResult>(count);
			while (count-- > 0)
				list.Add(converter(GetItem(index++)));
			return list;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			CopyTo(array, arrayIndex, 0, _count);
		}

		public void CopyTo(T[] array, int arrayIndex, int index, int count)
		{
			if (array == null)
				throw new ArgumentNullException("list");
			if (index < 0 || index > _count)
				throw new ArgumentNullException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if (arrayIndex < 0 || array.Length - arrayIndex < count)
				throw new ArgumentOutOfRangeException("arrayIndex");
      //
      RingBuffer.Copy(_array, _start + index, 0, 0, array, arrayIndex, 0, 0, count, 0, 1, false);
		}

		public T[] ToArray()
		{
			return ToArray(0, _count);
		}

		public T[] ToArray(int index, int count)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			T[] array = new T[count];
      RingBuffer.Copy(_array, _start + index, 0, 0, array, 0, 0, 0, count, 0, 1, false);
			return array;
		}

		#endregion
		#region Retrieval methods

		public PwrList<T> GetRange(int index, int count)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			PwrList<T> list = new PwrList<T>(count);
			for (int i = index; i < index + count; i++)
				list.Insert(list.Count, GetItem(i));
			return list;
		}

		public PwrList<T> GetRangeReverse(int index, int count)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			PwrList<T> list = new PwrList<T>(count);
			for (int i = index; i < index + count; i++)
				list.Insert(0, GetItem(i));
			return list;
		}

		public bool Exists(Func<T, bool> match)
		{
			return Exists(0, _count, match);
		}

		public bool Exists(int index, Func<T, bool> match)
		{
			return Exists(index, _count - index, match);
		}

		public bool Exists(int index, int count, Func<T, bool> match)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			while (count-- > 0)
				if (match(GetItem(index++)))
					return true;
			return false;
		}

		public bool MatchAll(Func<T, bool> match)
		{
			return MatchAll(0, _count, match);
		}

		public bool MatchAll(int index, Func<T, bool> match)
		{
			return MatchAll(index, _count - index, match);
		}

		public bool MatchAll(int index, int count, Func<T, bool> match)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			while (count-- > 0)
				if (!match(GetItem(index++)))
					return false;
			return true;
		}

		public T Find(Func<T, bool> match)
		{
			return Find(0, _count, match);
		}

		public T Find(int index, Func<T, bool> match)
		{
			return Find(index, _count - index, match);
		}

		public T Find(int index, int count, Func<T, bool> match)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			while (count-- > 0)
			{
				T item = GetItem(index++);
				if (match(item))
					return item;
			}
			return default(T);
		}

		public T FindLast(Func<T, bool> match)
		{
			return FindLast(0, _count, match);
		}

		public T FindLast(int index, Func<T, bool> match)
		{
			return FindLast(index, _count - index, match);
		}

		public T FindLast(int index, int count, Func<T, bool> match)
		{
			if (index < -1 || index >= _count + (_count == 0 ? 1 : 0))
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > index + 1)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			while (count-- > 0)
			{
				T item = GetItem(index--);
				if (match(item))
					return item;
			}
			return default(T);
		}

		public PwrList<T> FindAll(Func<T, bool> match)
		{
			return FindAll(0, _count, match);
		}

		public PwrList<T> FindAll(int index, Func<T, bool> match)
		{
			return FindAll(index, _count - index, match);
		}

		public PwrList<T> FindAll(int index, int count, Func<T, bool> match)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			PwrList<T> list = new PwrList<T>();
			while (count-- > 0)
			{
				T item = GetItem(index++);
				if (match(item))
					list.Add(item);
			}
			list.Restrict(CollectionRestrictions.ReadOnly);
			return list;
		}

		public int FindIndex(Func<T, bool> match)
		{
			return FindIndex(0, _count, match);
		}

		public int FindIndex(int index, Func<T, bool> match)
		{
			return FindIndex(index, _count - index, match);
		}

		public int FindIndex(int index, int count, Func<T, bool> match)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			while (count-- > 0)
			{
				if (match(GetItem(index)))
					return index;
				index++;
			}
			return -1;
		}

		public int FindLastIndex(Func<T, bool> match)
		{
			return FindLastIndex(_count - 1, _count, match);
		}

		public int FindLastIndex(int index, Func<T, bool> match)
		{
			return FindLastIndex(index, index + 1, match);
		}

		public int FindLastIndex(int index, int count, Func<T, bool> match)
		{
			if (index < -1 || index >= _count + (_count == 0 ? 1 : 0))
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > index + 1)
				throw new ArgumentOutOfRangeException("count");
			//
			while (count-- > 0)
			{
				if (match(GetItem(index)))
					return index;
				index--;
			}
			return -1;
		}

		public PwrList<int> FindAllIndices(Func<T, bool> match)
		{
			return FindAllIndices(0, _count, match);
		}

		public PwrList<int> FindAllIndices(int index, Func<T, bool> match)
		{
			return FindAllIndices(index, _count - index, match);
		}

		public PwrList<int> FindAllIndices(int index, int count, Func<T, bool> match)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			if (match == null)
				throw new ArgumentNullException("match");
			//
			PwrList<int> list = new PwrList<int>();
			while (count-- > 0)
			{
				if (match(GetItem(index)))
					list.Add(index);
				index++;
			}
			list.Restrict(CollectionRestrictions.ReadOnly);
			return list;
		}

		public bool Contains(T value)
		{
			return IndexOf(value) >= 0;
		}

		public bool Contains(int index, T value)
		{
			return IndexOf(value, index) >= 0;
		}

		public bool Contains(int index, int count, T value)
		{
			return IndexOf(value, index, count) >= 0;
		}

		public int IndexOf(T value)
		{
			return IndexOf(value, 0, _count);
		}

		public int IndexOf(T value, int index)
		{
			return IndexOf(value, index, _count - index);
		}

		public int IndexOf(T value, int index, int count)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
				throw new ArgumentOutOfRangeException("count");
			//
			while (count-- > 0)
			{
				if (EqualityComparer<T>.Default.Equals(GetItem(index), value))
					return index;
				index++;
			}
			return -1;
		}

		public int LastIndexOf(T value)
		{
			return LastIndexOf(value, _count - 1, _count);
		}

		public int LastIndexOf(T value, int index)
		{
			return LastIndexOf(value, index, index + 1);
		}

		public int LastIndexOf(T value, int index, int count)
		{
			if (index < -1 || index >= _count + (_count == 0 ? 1 : 0))
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > index + 1)
				throw new ArgumentOutOfRangeException("count");
			//
			while (count-- > 0)
			{
				if (EqualityComparer<T>.Default.Equals(GetItem(index), value))
					return index;
				index--;
			}
			return -1;
		}

		public int BinarySearch(T value, Comparison<T> comparison)
		{
			return BinarySearch(0, _count, value, comparison);
		}

		public int BinarySearch(int index, T value, Comparison<T> comparison)
		{
			return BinarySearch(index, _count - index, value, comparison);
		}

		public int BinarySearch(int index, int count, T value, Comparison<T> comparison)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
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
					int result = comparison(value, GetItem(middle));
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

		public int BinarySearchFirst(T value, Comparison<T> comparison)
		{
			return BinarySearchFirst(0, _count, value, comparison);
		}

		public int BinarySearchFirst(int index, T value, Comparison<T> comparison)
		{
			return BinarySearchFirst(index, _count - index, value, comparison);
		}

		public int BinarySearchFirst(int index, int count, T value, Comparison<T> comparison)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > _count - index)
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
					int result = comparison(value, GetItem(middle));
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

		public int BinarySearchLast(T value, Comparison<T> comparison)
		{
			return BinarySearchLast(_count - 1, _count, value, comparison);
		}

		public int BinarySearchLast(int index, T value, Comparison<T> comparison)
		{
			return BinarySearchLast(index, index + 1, value, comparison);
		}

		public int BinarySearchLast(int index, int count, T value, Comparison<T> comparison)
		{
			if (index < -1 || index >= _count + (_count == 0 ? 1 : 0))
				throw new ArgumentOutOfRangeException("index");
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
					int result = comparison(value, GetItem(middle));
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

		#endregion
		#region Enumerable methods

		private IEnumerator<T> GetEnumerator(int index, int count, bool reverse)
		{
			if (!reverse)
			{
				if (index < 0 || index > _count)
					throw new ArgumentOutOfRangeException("index");
				if (count < 0 || count > _count - index)
					throw new ArgumentOutOfRangeException("count");
			}
			else
			{
				if (index < -1 || index >= _count)
					throw new ArgumentOutOfRangeException("index");
				if (count < 0 || count > index + 1)
					throw new ArgumentOutOfRangeException("count");
			}
			//
			int stamp = _stamp;
			for (int i = index; !reverse && i < count || reverse && i >= 0; i += reverse ? -1 : 1)
			{
				if (_stamp != stamp)
					throw new InvalidOperationException(CollectionResources.Default.Strings[CollectionMessage.InternalCollectionWasModified]);
				//
				yield return GetItem(i);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return GetEnumerator(0, _count, false);
		}

		public IEnumerator<T> GetReverseEnumerator()
		{
			return GetEnumerator(_count - 1, _count, true);
		}

		#endregion
		#endregion
		#region Supported interfaces implementation
		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator(0, _count, false);
		}

		#endregion
    #region ICollection implementation

    bool ICollection.IsSynchronized
    {
      get
      {
        return false;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        if (this._syncRoot == null)
          Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
        return this._syncRoot;
      }
    }

   void ICollection.CopyTo(Array array, int index)
    {
      _array.CopyTo(array, index);
    }

    #endregion
    #region ICollection<T> implementation

    bool ICollection<T>.IsReadOnly
    {
      get
      {
        return _restriction == CollectionRestrictions.ReadOnly;
      }
    }

    #endregion
    #region IList implementation

    bool IList.IsReadOnly
    {
      get
      {
        return _restriction == CollectionRestrictions.ReadOnly;
      }
    }

    bool IList.IsFixedSize
    {
      get
      {
        return (_restriction & CollectionRestrictions.FixedSize) != 0;
      }
    }

    object IList.this[int index]
    {
      get
      {
        return this[index];
      }
      set
      {
        this[index] = (T)value;
      }
    }

    int IList.Add(object item)
    {
      int index = Count;
      Insert(index, (T)item);
      return index;
    }

    void IList.Insert(int index, object item)
    {
      Insert(index, (T)item);
    }

    void IList.Remove(object item)
    {
      Remove((T)item);
    }

    int IList.IndexOf(object item)
    {
      return IndexOf((T)item);
    }

    bool IList.Contains(object item)
    {
      return Contains((T)item);
    }

    #endregion
    #region IStampSupport implementation

    int IStampSupport.Stamp
		{
			get
			{
				return _stamp;
			}
		}

		#endregion
		#endregion
	}
}
