using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class EquatableEqualityComparer<T> : IEqualityComparer<T>
		where T : IEquatable<T>
	{
		private static readonly Lazy<EquatableEqualityComparer<T>> instance = new Lazy<EquatableEqualityComparer<T>>(() => new EquatableEqualityComparer<T>());

		#region Constructors

		static EquatableEqualityComparer()
		{
		}

		private EquatableEqualityComparer()
		{
		}

		#endregion
		#region Properties

		public static EquatableEqualityComparer<T> Default
		{
			get
			{
				return instance.Value;
			}
		}

		#endregion
		#region Methods

		public bool Equals(T first, T second)
		{
			return first == null && second == null || first != null && second != null && first.Equals(second);
		}

		public int GetHashCode(T value)
		{
			return value == null ? 0 : value.GetHashCode();
		}

		#endregion
	}
}
