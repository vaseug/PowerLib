using System;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	public interface IHierarchicalContext<T>
	{
		IList<T> Ancestors
		{
			get;
		}
	}
}
