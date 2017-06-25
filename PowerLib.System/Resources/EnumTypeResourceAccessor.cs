using System;
using System.Globalization;
using System.Resources;

namespace PowerLib.System.Resources
{
	public class EnumTypeResourceAccessor<K, T> : TypeResourceAccessor<K, T>
    where K : struct, IComparable, IFormattable, IConvertible
	{
		static EnumTypeResourceAccessor()
		{
			Type type = typeof(K);
			if (!type.IsEnum)
				throw new InvalidOperationException(string.Format("Resourckey type '{0}' is not enum.", type.FullName));
		}

		public EnumTypeResourceAccessor(Func<K, string> selector)
			: base(selector)
		{
		}

		public EnumTypeResourceAccessor(Func<K, string> selector, CultureInfo ci)
			: base(selector, ci)
		{
		}
	}
}
