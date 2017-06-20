using System;

namespace PowerLib.System.Collections.Matching
{
	public sealed class PersistentPredicate<T> : IPredicate<T>
	{
		private bool _value ;

		private static readonly Lazy<PersistentPredicate<T>> falsep = new Lazy<PersistentPredicate<T>>(() => new PersistentPredicate<T>(false));
		private static readonly Lazy<PersistentPredicate<T>> truep = new Lazy<PersistentPredicate<T>>(() => new PersistentPredicate<T>(true));

		#region Constructors

		static PersistentPredicate()
		{
		}

		private PersistentPredicate(bool value)
		{
			_value = value ;
		}

		#endregion
		#region Properties

		public static PersistentPredicate<T> False
		{
			get
			{
				return falsep.Value;
			}
		}

		public static PersistentPredicate<T> True
		{
			get
			{
				return truep.Value;
			}
		}

    #endregion
    #region Methods

    public bool Match(T value)
		{
			return _value ;
		}

		#endregion
	}
}
