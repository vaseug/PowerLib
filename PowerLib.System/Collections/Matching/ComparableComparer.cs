using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class ComparableComparer<T> : IComparer<T>
		where T : IComparable<T>
	{
		private static readonly Lazy<ComparableComparer<T>> instance = new Lazy<ComparableComparer<T>>(() => new ComparableComparer<T>());

		#region Constructors

		private ComparableComparer()
		{
		}

		#endregion
		#region Properties

		public static ComparableComparer<T> Default
		{
			get
			{
				return instance.Value;
			}
		}

		#endregion
		#region Methods

		public int Compare(T first, T second)
		{
			if (first == null)
				throw new ArgumentNullException("first");
			if (second == null)
				throw new ArgumentNullException("second");

			return first.CompareTo(second);
		}

		#endregion
	}
}
