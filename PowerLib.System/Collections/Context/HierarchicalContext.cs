using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections;


namespace PowerLib.System.Collections.Context
{
	/// <summary>
	/// HierarchicalContext
	/// </summary>
	public class HierarchicalContext<T> : IHierarchicalContext<T>
	{
		private IList<T> _ancestors;
		private PwrList<T> _innerList;

		#region Constructors

		public HierarchicalContext()
		{
			_innerList = new PwrList<T>();
			_ancestors = new PwrListView<T>(_innerList);
		}

		#endregion
		#region Properties

		public IList<T> Ancestors
		{
			get
			{
				return _ancestors;
			}
		}

		#endregion
		#region Methods

		internal protected int PushAncestor(T ancestor)
		{
			_innerList.Insert(0, ancestor);
			return _innerList.Count;
		}

		internal protected T PopAncestor()
		{
			int index = _innerList.Count - 1;
			T item = _innerList[index];
			_innerList.RemoveAt(index);
			return item;
		}

		#endregion
	}
}
