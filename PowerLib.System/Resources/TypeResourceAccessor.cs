 using System;
using System.Globalization;
using System.Resources;

namespace PowerLib.System.Resources
{
	public class TypeResourceAccessor<K, T> : ResourceAccessor<K>
	{
		public TypeResourceAccessor(Func<K, string> selector)
			: base(selector, new ResourceManager(typeof(T)))
		{
		}

		public TypeResourceAccessor(Func<K, string> selector, CultureInfo ci)
			: base(selector, new ResourceManager(typeof(T)), ci)
		{
		}
	}
}
