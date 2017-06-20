using System;

namespace PowerLib.System.Collections
{
	public interface IParentRef<T>
		where T : IParentRef<T>
	{
		T Parent
		{
			get;
		}
	}
}
