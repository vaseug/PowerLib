using System;

namespace PowerLib.System.Collections.Matching
{
	public sealed class NullPredicate<T> : IPredicate<T>
	{
		private static readonly Lazy<NullPredicate<T>> instance = new Lazy<NullPredicate<T>>();

		#region Constructors

		static NullPredicate()
		{
		}

		private NullPredicate()
		{
		}

		#endregion
		#region Properties

		public static NullPredicate<T> Default
		{
			get
			{
				return instance.Value;
			}
		}

    #endregion
    #region Methods

    public bool Match(T value)
		{
			return value == null;
		}

		#endregion
	}
}
