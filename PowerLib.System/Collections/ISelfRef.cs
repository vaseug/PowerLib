using System;

namespace PowerLib.System.Collections
{
	public interface ISelfRef<T>
		where T : ISelfRef<T>
	{
		T This
		{
			get;
		}
	}
}
