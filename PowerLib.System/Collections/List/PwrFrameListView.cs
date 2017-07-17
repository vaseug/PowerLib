using System;
using System.Collections;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Type of item.</typeparam>
	public class PwrFrameListView<T> : PwrListViewBase<T>
	{
		private int _frameOffset;
		private int _frameSize;

		#region Constructors

		public PwrFrameListView(IList<T> list)
			: this(list, 0, list != null ? list.Count : 0)
		{
		}

    public PwrFrameListView(IList<T> list, Range frame)
      : base(list)
    {
      if (frame.Index < 0)
        throw new ArgumentOutOfRangeException("frame", "Frame offset is out of range.");
      if (frame.Count < 0)
        throw new ArgumentOutOfRangeException("frame", "Frame size is out of range.");

      _frameOffset = frame.Index;
      _frameSize = frame.Count;
    }

		public PwrFrameListView(IList<T> list, int frameOffset, int frameSize)
			: base(list)
		{
			if (frameOffset < 0)
				throw new ArgumentOutOfRangeException("frameOffset");
			if (frameSize < 0)
				throw new ArgumentOutOfRangeException("frameSize");

			_frameOffset = frameOffset;
			_frameSize = frameSize;
		}

		#endregion
		#region Properties
		#region Internal properties

		protected override int ItemsCount
		{
			get
			{
				return _frameOffset >= InnerStore.Count ? 0 : Comparable.Min(_frameSize, InnerStore.Count - _frameOffset);
			}
		}

		#endregion
		#region Public properties

		public int FrameOffset
		{
			get
			{
        return _frameOffset;// _frameOffset > InnerStore.Count ? InnerStore.Count : _frameOffset;
			}
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();

				_frameOffset = value ;
			}
		}

		public int FrameSize
		{
			get
			{
				return _frameSize;
			}
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();

				_frameSize = value ;
			}
		}

    public Range Frame
    {
      get
      {
        return new Range(_frameOffset, _frameSize);
      }
      set
      {
        if (value.Index < 0)
          throw new ArgumentOutOfRangeException(null, "Frame offset is out of range.");
        if (value.Count < 0)
          throw new ArgumentOutOfRangeException(null, "Frame size is out of range.");

        _frameOffset = value.Index;
        _frameSize = value.Count;
      }
    }

		#endregion
		#endregion
		#region Methods
		#region Internal methods

		protected override int GetStoreIndex(int index)
		{
			return _frameOffset >= InnerStore.Count || index < 0 || index >= ItemsCount ? -1 : _frameOffset + index;
		}

		#endregion
		#endregion
	}
}
