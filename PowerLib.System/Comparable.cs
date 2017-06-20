using System;

namespace PowerLib.System
{
	public static class Comparable
	{
		#region IComparable<T> methods
		#region Compare methods

		public static int Compare<T>(T first, T second)
			where T : struct, IComparable<T>
		{
			return first.CompareTo(second);
		}

		public static int Compare<T>(T? first, T? second, bool nullOrder)
			where T : struct, IComparable<T>
		{
			return first.HasValue? second.HasValue? first.Value.CompareTo(second.Value) : nullOrder ? -1 : 1 : second.HasValue? nullOrder ? 1 : -1 : 0;
		}

		public static int Compare<T>(T first, T second, bool nullOrder)
			where T : class, IComparable<T>
		{
			return first != null ? second != null ? first.CompareTo(second) : nullOrder ? -1 : 1 : second != null ? nullOrder ? 1 : -1 : 0;
		}

		public static bool Compare<T>(this T testValue, T otherValue, ComparisonCriteria criteria)
			where T : struct, IComparable<T>
		{
			switch (criteria)
			{
				case ComparisonCriteria.Equal:
					return testValue.CompareTo(otherValue) == 0;
				case ComparisonCriteria.NotEqual:
					return testValue.CompareTo(otherValue) != 0;
				case ComparisonCriteria.GreaterThan:
					return testValue.CompareTo(otherValue) > 0;
				case ComparisonCriteria.GreaterThanOrEqual:
					return testValue.CompareTo(otherValue) >= 0;
				case ComparisonCriteria.LessThan:
					return testValue.CompareTo(otherValue) < 0;
				case ComparisonCriteria.LessThanOrEqual:
					return testValue.CompareTo(otherValue) <= 0;
				default:
					throw new ArgumentException("Invalicomparisocriteria", "criteria");
			}
		}

		public static bool Compare<T>(this T? testValue, T? otherValue, ComparisonCriteria criteria, bool nullOrder)
			where T : struct, IComparable<T>
		{
			switch (criteria)
			{
				case ComparisonCriteria.Equal:
					return testValue.HasValue? otherValue.HasValue? testValue.Value.CompareTo(otherValue.Value) == 0 : false : otherValue.HasValue? false : true;
				case ComparisonCriteria.NotEqual:
					return testValue.HasValue? otherValue.HasValue? testValue.Value.CompareTo(otherValue.Value) != 0 : true : otherValue.HasValue? true : false;
				case ComparisonCriteria.GreaterThan:
					return testValue.HasValue? otherValue.HasValue? testValue.Value.CompareTo(otherValue.Value) > 0 : !nullOrder : otherValue.HasValue? nullOrder : false;
				case ComparisonCriteria.GreaterThanOrEqual:
					return testValue.HasValue? otherValue.HasValue? testValue.Value.CompareTo(otherValue.Value) >= 0 : !nullOrder : otherValue.HasValue? nullOrder : true;
				case ComparisonCriteria.LessThan:
					return testValue.HasValue? otherValue.HasValue? testValue.Value.CompareTo(otherValue.Value) < 0 : nullOrder : otherValue.HasValue? !nullOrder : false;
				case ComparisonCriteria.LessThanOrEqual:
					return testValue.HasValue? otherValue.HasValue? testValue.Value.CompareTo(otherValue.Value) <= 0 : nullOrder : otherValue.HasValue? !nullOrder : true;
				default:
					throw new ArgumentException("Invalicomparisocriteria", "criteria");
			}
		}

		public static bool Compare<T>(this T testValue, T otherValue, ComparisonCriteria criteria, bool nullOrder)
			where T : class, IComparable<T>
		{
			switch (criteria)
			{
				case ComparisonCriteria.Equal:
					return testValue!= null ? otherValue!= null ? testValue.CompareTo(otherValue) == 0 : false : otherValue!= null ? false : true;
				case ComparisonCriteria.NotEqual:
					return testValue!= null ? otherValue!= null ? testValue.CompareTo(otherValue) != 0 : true : otherValue!= null ? true : false;
				case ComparisonCriteria.GreaterThan:
					return testValue!= null ? otherValue!= null ? testValue.CompareTo(otherValue) > 0 : !nullOrder : otherValue!= null ? nullOrder : false;
				case ComparisonCriteria.GreaterThanOrEqual:
					return testValue!= null ? otherValue!= null ? testValue.CompareTo(otherValue) >= 0 : !nullOrder : otherValue!= null ? nullOrder : true;
				case ComparisonCriteria.LessThan:
					return testValue!= null ? otherValue!= null ? testValue.CompareTo(otherValue) < 0 : nullOrder : otherValue!= null ? !nullOrder : false;
				case ComparisonCriteria.LessThanOrEqual:
					return testValue!= null ? otherValue!= null ? testValue.CompareTo(otherValue) <= 0 : nullOrder : otherValue!= null ? !nullOrder : true;
				default:
					throw new ArgumentException("Invalid comparison criteria", "criteria");
			}
		}

		#endregion
		#region Greater than methods

		public static bool GreaterThan<T>(this T testValue, T otherValue)
			where T : struct, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.GreaterThan);
		}

		public static bool GreaterThan<T>(this T? testValue, T? otherValue, bool nullOrder)
			where T : struct, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.GreaterThan, nullOrder);
		}

		public static bool GreaterThan<T>(this T testValue, T otherValue, bool nullOrder)
			where T : class, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.GreaterThan, nullOrder);
		}

		#endregion
		#region Greater than or equal methods

		public static bool GreaterThanOrEqual<T>(this T testValue, T otherValue)
			where T : struct, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.GreaterThanOrEqual);
		}

		public static bool GreaterThanOrEqual<T>(this T? testValue, T? otherValue, bool nullOrder)
			where T : struct, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.GreaterThanOrEqual, nullOrder);
		}

		public static bool GreaterThanOrEqual<T>(this T testValue, T otherValue, bool nullOrder)
			where T : class, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.GreaterThanOrEqual, nullOrder);
		}

		#endregion
		#region Less than methods

		public static bool LessThan<T>(this T testValue, T otherValue)
			where T : struct, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.LessThan);
		}

		public static bool LessThan<T>(this T? testValue, T? otherValue, bool nullOrder)
			where T : struct, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.LessThan, nullOrder);
		}

		public static bool LessThan<T>(this T testValue, T otherValue, bool nullOrder)
			where T : class, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.LessThan, nullOrder);
		}

		#endregion
		#region Less than or equal methods

		public static bool LessThanOrEqual<T>(this T testValue, T otherValue)
			where T : struct, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.LessThanOrEqual);
		}

		public static bool LessThanOrEqual<T>(this T? testValue, T? otherValue, bool nullOrder)
			where T : struct, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.LessThanOrEqual, nullOrder);
		}

		public static bool LessThanOrEqual<T>(this T testValue, T otherValue, bool nullOrder)
			where T : class, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.LessThanOrEqual, nullOrder);
		}

		#endregion
		#region Equal methods

		public static bool Equal<T>(this T testValue, T otherValue)
			where T : struct, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.Equal);
		}

		public static bool Equal<T>(this T? testValue, T? otherValue, bool nullOrder)
			where T : struct, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.Equal, nullOrder);
		}

		public static bool Equal<T>(this T testValue, T otherValue, bool nullOrder)
			where T : class, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.Equal, nullOrder);
		}

		#endregion
		#region Not equal methods

		public static bool NotEqual<T>(this T testValue, T otherValue)
			where T : struct, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.NotEqual);
		}

		public static bool NotEqual<T>(this T? testValue, T? otherValue, bool nullOrder)
			where T : struct, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.NotEqual, nullOrder);
		}

		public static bool NotEqual<T>(this T testValue, T otherValue, bool nullOrder)
			where T : class, IComparable<T>
		{
			return Compare(testValue, otherValue, ComparisonCriteria.NotEqual, nullOrder);
		}

		#endregion
		#region Between methods

		public static bool Between<T>(this T testValue, T lowerValue, T upperValue, BetweenCriteria criteria)
			where T : struct, IComparable<T>
		{
			switch (criteria)
			{
				case BetweenCriteria.IncludeBoth:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThanOrEqual) && Compare(testValue, upperValue, ComparisonCriteria.LessThanOrEqual);
				case BetweenCriteria.ExcludeLower:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThan) && Compare(testValue, upperValue, ComparisonCriteria.LessThanOrEqual);
				case BetweenCriteria.ExcludeUpper:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThanOrEqual) && Compare(testValue, upperValue, ComparisonCriteria.LessThan);
				case BetweenCriteria.ExcludeBoth:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThan) && Compare(testValue, upperValue, ComparisonCriteria.LessThan);
				default:
					throw new InvalidOperationException();
			}
		}

		public static bool Between<T>(this T testValue, T lowerValue, T upperValue)
			where T : struct, IComparable<T>
		{
			return Between(testValue, lowerValue, upperValue, BetweenCriteria.IncludeBoth);
		}

		public static bool Between<T>(this T? testValue, T? lowerValue, T? upperValue, BetweenCriteria criteria, bool nullOrder)
			where T : struct, IComparable<T>
		{
			switch (criteria)
			{
				case BetweenCriteria.IncludeBoth:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThanOrEqual, nullOrder) && Compare(testValue, upperValue, ComparisonCriteria.LessThanOrEqual, nullOrder);
				case BetweenCriteria.ExcludeLower:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThan, nullOrder) && Compare(testValue, upperValue, ComparisonCriteria.LessThanOrEqual, nullOrder);
				case BetweenCriteria.ExcludeUpper:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThanOrEqual, nullOrder) && Compare(testValue, upperValue, ComparisonCriteria.LessThan, nullOrder);
				case BetweenCriteria.ExcludeBoth:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThan, nullOrder) && Compare(testValue, upperValue, ComparisonCriteria.LessThan, nullOrder);
				default:
					throw new InvalidOperationException();
			}
		}

		public static bool Between<T>(this T? testValue, T? lowerValue, T? upperValue, bool nullOrder)
			where T : struct, IComparable<T>
		{
			return Between(testValue, lowerValue, upperValue, BetweenCriteria.IncludeBoth, nullOrder);
		}

		public static bool Between<T>(this T testValue, T lowerValue, T upperValue, BetweenCriteria criteria, bool nullOrder)
			where T : class, IComparable<T>
		{
			switch (criteria)
			{
				case BetweenCriteria.IncludeBoth:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThanOrEqual, nullOrder) && Compare(testValue, upperValue, ComparisonCriteria.LessThanOrEqual, nullOrder);
				case BetweenCriteria.ExcludeLower:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThan, nullOrder) && Compare(testValue, upperValue, ComparisonCriteria.LessThanOrEqual, nullOrder);
				case BetweenCriteria.ExcludeUpper:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThanOrEqual, nullOrder) && Compare(testValue, upperValue, ComparisonCriteria.LessThan, nullOrder);
				case BetweenCriteria.ExcludeBoth:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThan, nullOrder) && Compare(testValue, upperValue, ComparisonCriteria.LessThan, nullOrder);
				default:
					throw new InvalidOperationException();
			}
		}

		public static bool Between<T>(this T testValue, T lowerValue, T upperValue, bool nullOrder)
			where T : class, IComparable<T>
		{
			return Between(testValue, lowerValue, upperValue, BetweenCriteria.IncludeBoth, nullOrder);
		}

		#endregion
		#region Max methods

		public static T Max<T>(params T[] values)
			where T : struct, IComparable<T>
		{
			if (values == null)
				throw new ArgumentNullException("values");
			if (values.Length == 0)
				throw new ArgumentException("Array is empty", "values");

			T result = values[0];
			for (int i = 1; i < values.Length; i++)
				if (Compare(result, values[i], ComparisonCriteria.LessThan))
					result = values[i];
			return result;
		}

		public static T? Max<T>(bool nullOrder, params T?[] values)
			where T : struct, IComparable<T>
		{
			if (values == null)
				throw new ArgumentNullException("values");
			if (values.Length == 0)
				throw new ArgumentException("Array is empty", "values");

			T? result = values[0];
			for (int i = 1; i < values.Length; i++)
				if (Compare(result, values[i], ComparisonCriteria.LessThan, nullOrder))
					result = values[i];
			return result;
		}

		public static T Max<T>(bool nullOrder, params T[] values)
			where T : class, IComparable<T>
		{
			if (values == null)
				throw new ArgumentNullException("values");
			if (values.Length == 0)
				throw new ArgumentException("Array is empty", "values");

			T result = values[0];
			for (int i = 1; i < values.Length; i++)
				if (Compare(result, values[i], ComparisonCriteria.LessThan, nullOrder))
					result = values[i];
			return result;
		}

		#endregion
		#region Min methods

		public static T Min<T>(params T[] values)
			where T : struct, IComparable<T>
		{
			if (values == null)
				throw new ArgumentNullException("values");
			if (values.Length == 0)
				throw new ArgumentException("Array is empty", "values");

			T result = values[0];
			for (int i = 1; i < values.Length; i++)
				if (Compare(result, values[i], ComparisonCriteria.GreaterThan))
					result = values[i];
			return result;
		}

		public static T? Min<T>(bool nullOrder, params T?[] values)
			where T : struct, IComparable<T>
		{
			if (values == null)
				throw new ArgumentNullException("values");
			if (values.Length == 0)
				throw new ArgumentException("Array is empty", "values");

			T? result = values[0];
			for (int i = 1; i < values.Length; i++)
				if (Compare(result, values[i], ComparisonCriteria.GreaterThan, nullOrder))
					result = values[i];
			return result;
		}

		public static T Min<T>(bool nullOrder, params T[] values)
			where T : class, IComparable<T>
		{
			if (values == null)
				throw new ArgumentNullException("values");
			if (values.Length == 0)
				throw new ArgumentException("Array is empty", "values");

			T result = values[0];
			for (int i = 1; i < values.Length; i++)
				if (Compare(result, values[i], ComparisonCriteria.GreaterThan, nullOrder))
					result = values[i];
			return result;
		}

		#endregion
		#endregion
		#region IComparable methods
		#region Compare methods

		public static int Compare(IComparable first, IComparable second, bool nullOrder = false)
		{
			return first != null ? second != null ? first.CompareTo(second) : nullOrder ? -1 : 1 : second != null ? nullOrder ? 1 : -1 : 0;
		}

		public static bool Compare(this IComparable testValue, object otherValue, ComparisonCriteria criteria, bool nullOrder = false)
		{
			switch (criteria)
			{
				case ComparisonCriteria.Equal:
					return testValue!= null ? otherValue!= null ? testValue.CompareTo(otherValue) == 0 : false : otherValue!= null ? false : true;
				case ComparisonCriteria.NotEqual:
					return testValue!= null ? otherValue!= null ? testValue.CompareTo(otherValue) != 0 : true : otherValue!= null ? true : false;
				case ComparisonCriteria.GreaterThan:
					return testValue!= null ? otherValue!= null ? testValue.CompareTo(otherValue) > 0 : !nullOrder : otherValue!= null ? nullOrder : false;
				case ComparisonCriteria.GreaterThanOrEqual:
					return testValue!= null ? otherValue!= null ? testValue.CompareTo(otherValue) >= 0 : !nullOrder : otherValue!= null ? nullOrder : true;
				case ComparisonCriteria.LessThan:
					return testValue!= null ? otherValue!= null ? testValue.CompareTo(otherValue) < 0 : nullOrder : otherValue!= null ? !nullOrder : false;
				case ComparisonCriteria.LessThanOrEqual:
					return testValue!= null ? otherValue!= null ? testValue.CompareTo(otherValue) <= 0 : nullOrder : otherValue!= null ? !nullOrder : true;
				default:
					throw new ArgumentException("Invalicomparisocriteria", "criteria");
			}
		}

		#endregion
		#region Greater than methods

		public static bool GreaterThan(this IComparable testValue, object otherValue, bool nullOrder = false)
		{
			return Compare(testValue, otherValue, ComparisonCriteria.GreaterThan, nullOrder);
		}

		#endregion
		#region Greater than or equal methods

		public static bool GreaterThanOrEqual(this IComparable testValue, object otherValue, bool nullOrder = false)
		{
			return Compare(testValue, otherValue, ComparisonCriteria.GreaterThanOrEqual, nullOrder);
		}

		#endregion
		#region Less than methods

		public static bool LessThan(this IComparable testValue, object otherValue, bool nullOrder = false)
		{
			return Compare(testValue, otherValue, ComparisonCriteria.LessThan, nullOrder);
		}

		#endregion
		#region Less than or equal methods

		public static bool LessThanOrEqual(this IComparable testValue, object otherValue, bool nullOrder = false)
		{
			return Compare(testValue, otherValue, ComparisonCriteria.LessThanOrEqual, nullOrder);
		}

		#endregion
		#region Equal methods

		public static bool Equal(this IComparable testValue, object otherValue, bool nullOrder = false)
		{
			return Compare(testValue, otherValue, ComparisonCriteria.Equal, nullOrder);
		}

		#endregion
		#region Not equal methods

		public static bool NotEqual(this IComparable testValue, object otherValue, bool nullOrder = false)
		{
			return Compare(testValue, otherValue, ComparisonCriteria.NotEqual, nullOrder);
		}

		#endregion
		#region Between methods

		public static bool Between(this IComparable testValue, object lowerValue, object upperValue, BetweenCriteria criteria, bool nullOrder = false)
		{
			switch (criteria)
			{
				case BetweenCriteria.IncludeBoth:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThanOrEqual, nullOrder) && Compare(testValue, upperValue, ComparisonCriteria.LessThanOrEqual, nullOrder);
				case BetweenCriteria.ExcludeLower:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThan, nullOrder) && Compare(testValue, upperValue, ComparisonCriteria.LessThanOrEqual, nullOrder);
				case BetweenCriteria.ExcludeUpper:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThanOrEqual, nullOrder) && Compare(testValue, upperValue, ComparisonCriteria.LessThan, nullOrder);
				case BetweenCriteria.ExcludeBoth:
					return Compare(testValue, lowerValue, ComparisonCriteria.GreaterThan, nullOrder) && Compare(testValue, upperValue, ComparisonCriteria.LessThan, nullOrder);
				default:
					throw new InvalidOperationException();
			}
		}

		public static bool Between(this IComparable testValue, object lowerValue, object upperValue, bool nullOrder = false)
		{
			return Between(testValue, lowerValue, upperValue, BetweenCriteria.IncludeBoth, nullOrder);
		}

		#endregion
		#region Max methods

		public static IComparable Max(bool nullOrder, params IComparable[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			if (values.Length == 0)
				throw new ArgumentException("Array is empty", "values");

			IComparable result = values[0];
			for (int i = 1; i < values.Length; i++)
				if (Compare(result, (object)values[i], ComparisonCriteria.LessThan, nullOrder))
					result = values[i];
			return result;
		}

		#endregion
		#region Min methods

		public static IComparable Min(bool nullOrder, params IComparable[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			if (values.Length == 0)
				throw new ArgumentException("Array is empty", "values");

			IComparable result = values[0];
			for (int i = 1; i < values.Length; i++)
				if (Compare(result, (object)values[i], ComparisonCriteria.GreaterThan, nullOrder))
					result = values[i];
			return result;
		}

		#endregion
		#endregion
	}
}
