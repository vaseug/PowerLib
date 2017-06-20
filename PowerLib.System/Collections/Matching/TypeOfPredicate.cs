using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections.Matching
{
	public sealed class TypeOfPredicate<T> : IPredicate<object>
	{
		private static readonly Lazy<TypeOfPredicate<T>> instance = new Lazy<TypeOfPredicate<T>>(() => new TypeOfPredicate<T>());

		#region Constructors

		static TypeOfPredicate()
		{
		}

		private TypeOfPredicate()
		{
		}

		#endregion
		#region Properties

		public static TypeOfPredicate<T> Default
		{
			get
			{
				return instance.Value;
			}
		}

    #endregion
    #region Methods

    public bool Match(object value)
		{
			return value is T;
		}

		#endregion
	}
}
