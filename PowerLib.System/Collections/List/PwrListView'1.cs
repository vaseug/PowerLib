using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerLib.System.Collections
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Typof item</typeparam>
	public class PwrListView<T> : PwrListViewBase<T>
	{
		private Func<int, int> _indexer;
		private Func<IList<T>, int> _counter;

		#region Constructors

		public PwrListView(IList<T> list)
			: this(list, i => i, l => l.Count)
		{
		}

		public PwrListView(IList<T> list, Func<int, int> indexer, Func<IList<T>, int> counter)
			: base(list)
		{
			_indexer = indexer;
			_counter = counter;
		}

		#endregion
		#region Properties
		#region Internal properties

		protected override int ItemsCount
		{
			get
			{
				return _counter(InnerStore);
			}
		}

		#endregion
		#endregion
		#region Methods
		#region Internal methods

		protected override bool PredicateItem(T value, int index)
		{
			return GetStoreIndex(index) >= 0;
		}

		protected override int GetStoreIndex(int index)
		{
			return _indexer != null ? _indexer(index) : base.GetStoreIndex(index);
		}

		#endregion
		#endregion
	}
}
