 using System;
using System.Globalization;
using System.Resources;

namespace PowerLib.System.Resources
{
	public class SelfTypeResourceAccessor<K, T> : TypeResourceAccessor<K, T>
		where T : SelfTypeResourceAccessor<K, T>, new()
	{
		private static Lazy<T> _default = new Lazy<T>(() => new T());

		protected SelfTypeResourceAccessor(Func<K, string> selector)
			: base(selector)
		{
		}

		protected SelfTypeResourceAccessor(Func<K, string> selector, CultureInfo ci)
			: base(selector, ci)
		{
		}

		public static T Default
		{
			get
			{
				return _default.Value;
			}
		}
	}
}
