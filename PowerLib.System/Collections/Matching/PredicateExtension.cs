using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PowerLib.System.Collections.Matching
{
	public static class Predicate
	{
		public static IPredicate<T> And<T>(this IPredicate<T> predicate, params IPredicate<T>[] predicates)
		{
			return new GroupPredicate<T>(Enumerable.Repeat(predicate, 1).Concat(predicates), GroupCriteria.And);
		}

		public static IPredicate<T> Or<T>(this IPredicate<T> predicate, params IPredicate<T>[] predicates)
		{
			return new GroupPredicate<T>(Enumerable.Repeat(predicate, 1).Concat(predicates), GroupCriteria.Or);
		}
	}
}
