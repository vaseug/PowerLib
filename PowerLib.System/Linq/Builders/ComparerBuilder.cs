using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using PowerLib.System.Collections.Matching;
using PowerLib.System.Linq.Expressions;

namespace PowerLib.System.Linq.Builders
{
	public static class ComparerBuilder
	{
		#region Constants

		private const string HasValueProperty = "HasValue";
		private const string ValueProperty = "Value";
		private const string CompareMethod = "Compare";
		private const string CompareToMethod = "CompareTo";

		#endregion
		#region Public methods
		#region Starting methods

		public static IComparisonDeclaration<T> Comparison<T>(this IParametersDeclaration<T, T> parameters)
		{
			return new ComparisonDeclarationImpl<T>(parameters);
		}

		public static IComparisonDeclaration<T> Comparison<T>()
		{
			return ParametersBuilder.Declare<T, T>().Comparison();
		}

		public static IComparisonDeclaration<T> Comparison<T>(Expression<Func<T>> definition)
		{
			return  ParametersBuilder.Declare(definition, definition).Comparison();
		}

		#endregion
		#region Comparable general compare operations

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, bool order)
			where K : IComparable<K>
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new ComparisonExpressionImpl<T>(CompareCore(parameters.Parameters, selector, order ? ReverseComparison(ComparableComparison<K>()) : ComparableComparison<K>()));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector)
			where K : IComparable<K>
		{
			return Compare(parameters, selector, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector)
			where K : IComparable<K>
		{
			return Compare(parameters, selector, true);
		}

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, bool order)
			where K : IComparable<K>
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new ComparisonExpressionImpl<T>(CompareCore(expression.Expression, selector,
				order ? ReverseComparison(ComparableComparison<K>()) : ComparableComparison<K>()));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector)
			where K : IComparable<K>
		{
			return Compare(expression, selector, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector)
			where K : IComparable<K>
		{
			return Compare(expression, selector, true);
		}

		#endregion
		#region Comparable object compare operations

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, bool nullOrder, bool order)
			where K : class, IComparable<K>
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new ComparisonExpressionImpl<T>(CompareCore(parameters.Parameters, selector, order ? ReverseComparison(ObjectComparison(ComparableComparison<K>(), nullOrder)) : ObjectComparison(ComparableComparison<K>(), nullOrder)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, bool nullOrder)
			where K : class, IComparable<K>
		{
			return Compare(parameters, selector, nullOrder, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, bool nullOrder)
			where K : class, IComparable<K>
		{
			return Compare(parameters, selector, nullOrder, true);
		}

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, bool nullOrder, bool order)
			where K : class, IComparable<K>
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new ComparisonExpressionImpl<T>(CompareCore(expression.Expression, selector,
				order ? ReverseComparison(ObjectComparison(ComparableComparison<K>(), nullOrder)) : ObjectComparison(ComparableComparison<K>(), nullOrder)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, bool nullOrder)
			where K : class, IComparable<K>
		{
			return Compare(expression, selector, nullOrder, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, bool nullOrder)
			where K : class, IComparable<K>
		{
			return Compare(expression, selector, nullOrder, true);
		}

		#endregion
		#region Comparable nullable compare operations

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K?>> selector, bool nullOrder, bool order)
			where K : struct, IComparable<K>
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new ComparisonExpressionImpl<T>(CompareCore(parameters.Parameters, selector, order ? ReverseComparison(NullableComparison(ComparableComparison<K>(), nullOrder)) : NullableComparison(ComparableComparison<K>(), nullOrder)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K?>> selector, bool nullOrder)
			where K : struct, IComparable<K>
		{
			return Compare(parameters, selector, nullOrder, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K?>> selector, bool nullOrder)
			where K : struct, IComparable<K>
		{
			return Compare(parameters, selector, nullOrder, true);
		}

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K?>> selector, bool nullOrder, bool order)
			where K : struct, IComparable<K>
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new ComparisonExpressionImpl<T>(CompareCore(expression.Expression, selector,
				order ? ReverseComparison(NullableComparison(ComparableComparison<K>(), nullOrder)) : NullableComparison(ComparableComparison<K>(), nullOrder)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K?>> selector, bool nullOrder)
			where K : struct, IComparable<K>
		{
			return Compare(expression, selector, nullOrder, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K?>> selector, bool nullOrder)
			where K : struct, IComparable<K>
		{
			return Compare(expression, selector, nullOrder, false);
		}

		#endregion
		#region General compare operations

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison, bool order)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			return new ComparisonExpressionImpl<T>(CompareCore(parameters.Parameters, selector, order ? ReverseComparison(comparison) : comparison));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison)
		{
			return Compare(parameters, selector, comparison, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison)
		{
			return Compare(parameters, selector, comparison, true);
		}

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, IComparer<K> comparer, bool order)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparer == null)
				throw new ArgumentNullException("comparison");

			return new ComparisonExpressionImpl<T>(CompareCore(parameters.Parameters, selector, order ? ReverseComparison(ComparerComparison(comparer)) : ComparerComparison(comparer)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, IComparer<K> comparer)
		{
			return Compare(parameters, selector, comparer, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, IComparer<K> comparer)
		{
			return Compare(parameters, selector, comparer, true);
		}

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison, bool order)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			return new ComparisonExpressionImpl<T>(CompareCore(expression.Expression, selector,
				order ? ReverseComparison(comparison) : comparison));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison)
		{
			return Compare(expression, selector, comparison, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison)
		{
			return Compare(expression, selector, comparison, true);
		}

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, IComparer<K> comparer, bool order)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return new ComparisonExpressionImpl<T>(CompareCore(expression.Expression, selector,
				order ? ReverseComparison(ComparerComparison(comparer)) : ComparerComparison(comparer)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, IComparer<K> comparer)
		{
			return Compare(expression, selector, comparer, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, IComparer<K> comparer)
		{
			return Compare(expression, selector, comparer, true);
		}

		#endregion
		#region Object compare operations

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison, bool nullOrder, bool order)
			where K : class
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			return new ComparisonExpressionImpl<T>(CompareCore(parameters.Parameters, selector, order ? ReverseComparison(ObjectComparison(comparison, nullOrder)) : ObjectComparison(comparison, nullOrder)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison, bool nullOrder)
			where K : class
		{
			return Compare(parameters, selector, comparison, nullOrder, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison, bool nullOrder)
			where K : class
		{
			return Compare(parameters, selector, comparison, nullOrder, true);
		}

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, IComparer<K> comparer, bool nullOrder, bool order)
			where K : class
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return new ComparisonExpressionImpl<T>(CompareCore(parameters.Parameters, selector, order ? ReverseComparison(ObjectComparison(ComparerComparison(comparer), nullOrder)) : ObjectComparison(ComparerComparison(comparer), nullOrder)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, IComparer<K> comparer, bool nullOrder)
			where K : class
		{
			return Compare(parameters, selector, comparer, nullOrder, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K>> selector, IComparer<K> comparer, bool nullOrder)
			where K : class
		{
			return Compare(parameters, selector, comparer, nullOrder, true);
		}

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison, bool nullOrder, bool order)
			where K : class
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			return new ComparisonExpressionImpl<T>(CompareCore(expression.Expression, selector,
				order ? ReverseComparison(ObjectComparison(comparison, nullOrder)) : ObjectComparison(comparison, nullOrder)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison, bool nullOrder)
			where K : class
		{
			return Compare(expression, selector, comparison, nullOrder, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison, bool nullOrder)
			where K : class
		{
			return Compare(expression, selector, comparison, nullOrder, false);
		}

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, IComparer<K> comparer, bool nullOrder, bool order)
			where K : class
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return new ComparisonExpressionImpl<T>(CompareCore(expression.Expression, selector,
				order ? ReverseComparison(ObjectComparison(ComparerComparison(comparer), nullOrder)) : ObjectComparison(ComparerComparison(comparer), nullOrder)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, IComparer<K> comparer, bool nullOrder)
			where K : class
		{
			return Compare(expression, selector, comparer, nullOrder, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K>> selector, IComparer<K> comparer, bool nullOrder)
			where K : class
		{
			return Compare(expression, selector, comparer, nullOrder, true);
		}

		#endregion
		#region Nullable compare operations

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K?>> selector, Expression<Comparison<K>> comparison, bool nullOrder, bool order)
			where K : struct
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			return new ComparisonExpressionImpl<T>(CompareCore(parameters.Parameters, selector, order ? ReverseComparison(NullableComparison(comparison, nullOrder)) : NullableComparison(comparison, nullOrder)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K?>> selector, Expression<Comparison<K>> comparison, bool nullOrder)
			where K : struct
		{
			return Compare(parameters, selector, comparison, nullOrder, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K?>> selector, Expression<Comparison<K>> comparison, bool nullOrder)
			where K : struct
		{
			return Compare(parameters, selector, comparison, nullOrder, true);
		}

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K?>> selector, IComparer<K> comparer, bool nullOrder, bool order)
			where K : struct
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return new ComparisonExpressionImpl<T>(CompareCore(parameters.Parameters, selector, order ? ReverseComparison(NullableComparison(ComparerComparison(comparer), nullOrder)) : NullableComparison(ComparerComparison(comparer), nullOrder)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K?>> selector, IComparer<K> comparer, bool nullOrder)
			where K : struct
		{
			return Compare(parameters, selector, comparer, nullOrder, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonDeclaration<T> parameters, Expression<Func<T, K?>> selector, IComparer<K> comparer, bool nullOrder)
			where K : struct
		{
			return Compare(parameters, selector, comparer, nullOrder, true);
		}

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K?>> selector, Expression<Comparison<K>> comparison, bool nullOrder, bool order)
			where K : struct
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			return new ComparisonExpressionImpl<T>(CompareCore(expression.Expression, selector,
				order ? ReverseComparison(NullableComparison(comparison, nullOrder)) : NullableComparison(comparison, nullOrder)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K?>> selector, Expression<Comparison<K>> comparison, bool nullOrder)
			where K : struct
		{
			return Compare(expression, selector, comparison, nullOrder, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K?>> selector, Expression<Comparison<K>> comparison, bool nullOrder)
			where K : struct
		{
			return Compare(expression, selector, comparison, nullOrder, true);
		}

		public static IComparisonExpression<T> Compare<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K?>> selector, IComparer<K> comparer, bool nullOrder, bool order)
			where K : struct
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return new ComparisonExpressionImpl<T>(CompareCore(expression.Expression, selector,
				order ? ReverseComparison(NullableComparison(ComparerComparison(comparer), nullOrder)) : NullableComparison(ComparerComparison(comparer), nullOrder)));
		}

		public static IComparisonExpression<T> Ascend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K?>> selector, IComparer<K> comparer, bool nullOrder)
			where K : struct
		{
			return Compare(expression, selector, comparer, nullOrder, false);
		}

		public static IComparisonExpression<T> Descend<T, K>(this IComparisonExpression<T> expression, Expression<Func<T, K?>> selector, IComparer<K> comparer, bool nullOrder)
			where K : struct
		{
			return Compare(expression, selector, comparer, nullOrder, true);
		}

		#endregion
		#region Comparison helper methods

		public static Expression<Comparison<T>> ComparableComparison<T>()
			where T : IComparable<T>
		{
			MethodInfo miCompareTo = typeof(IComparable<T>).GetMethod(CompareToMethod, new[] { typeof(T) });
			ParameterExpression parLeft = Expression.Parameter(typeof(T));
			ParameterExpression parRight = Expression.Parameter(typeof(T));

			return Expression.Lambda<Comparison<T>>(
				typeof(T).IsValueType ?
					(Expression)Expression.Call(parLeft, miCompareTo, parRight) :
					(Expression)Expression.Condition(
						Expression.NotEqual(parLeft, Expression.Constant(null, typeof(T))),
						Expression.Condition(
							Expression.NotEqual(parRight, Expression.Constant(null, typeof(T))),
							Expression.Call(parLeft, miCompareTo, parRight),
							Expression.Constant(1, typeof(int))),
						Expression.Condition(
							Expression.NotEqual(parRight, Expression.Constant(null, typeof(T))),
							Expression.Constant(-1, typeof(int)),
							Expression.Constant(-0, typeof(int)))),
				parLeft, parRight);
		}

		public static Expression<Comparison<T>> ComparerComparison<T>(IComparer<T> comparer)
		{
			MethodInfo miCompare = typeof(IComparer<T>).GetMethod(CompareMethod, BindingFlags.Public | BindingFlags.Instance);
			ParameterExpression parLeft = Expression.Parameter(typeof(T));
			ParameterExpression parRight = Expression.Parameter(typeof(T));

			return Expression.Lambda<Comparison<T>>(
				Expression.Call(Expression.Constant(comparer), miCompare, parLeft, parRight),
				parLeft, parRight);
		}

		public static Expression<Comparison<T>> ObjectComparison<T>(Expression<Comparison<T>> comparison, bool nullOrder)
			where T : class
		{
			return Expression.Lambda<Comparison<T>>(
				Expression.Condition(
					Expression.NotEqual(comparison.Parameters[0], Expression.Constant(null, typeof(T))),
					Expression.Condition(
						Expression.NotEqual(comparison.Parameters[1], Expression.Constant(null, typeof(T))),
						comparison.Body,
						Expression.Constant(nullOrder ? -1 : 1, typeof(int))),
					Expression.Condition(
						Expression.NotEqual(comparison.Parameters[1], Expression.Constant(null, typeof(T))),
						Expression.Constant(nullOrder ? 1 : -1, typeof(int)),
						Expression.Constant(0, typeof(int)))),
				comparison.Parameters);
		}

		public static Expression<Comparison<T?>> NullableComparison<T>(Expression<Comparison<T>> comparison, bool nullOrder)
			where T : struct
		{
			MemberInfo mbHasValue = typeof(T?).GetProperty(HasValueProperty);
			MemberInfo mbValue = typeof(T?).GetProperty(ValueProperty);
			ParameterExpression parLeft = Expression.Parameter(typeof(T?));
			ParameterExpression parRight = Expression.Parameter(typeof(T?));

			return Expression.Lambda<Comparison<T?>>(
				Expression.Condition(
					Expression.MakeMemberAccess(parLeft, mbHasValue),
					Expression.Condition(
						Expression.MakeMemberAccess(parRight, mbHasValue),
						comparison.ReplaceParameters(0, Expression.MakeMemberAccess(parLeft, mbValue), Expression.MakeMemberAccess(parRight, mbValue)),
						Expression.Constant(nullOrder ? -1 : 1, typeof(int))),
					Expression.Condition(
						Expression.MakeMemberAccess(parRight, mbHasValue),
						Expression.Constant(nullOrder ? 1 : -1, typeof(int)),
						Expression.Constant(0, typeof(int)))),
				parLeft, parRight);
		}

		public static Expression<Comparison<T>> ReverseComparison<T>(Expression<Comparison<T>> comparison)
		{
			return Expression.Lambda<Comparison<T>>(
				comparison.ReplaceParameters(0, comparison.Parameters[1], comparison.Parameters[0]),
				comparison.Parameters);
		}

		#endregion
		#endregion
		#region Internal methods

		private static Expression<Comparison<T>> CompareCore<T, K>(IReadOnlyList<ParameterExpression> parameters, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison)
		{
			return Expression.Lambda<Comparison<T>>(
				comparison.ReplaceParameters(0, selector.ReplaceParameters(0, parameters[0]), selector.ReplaceParameters(0, parameters[1])),
				parameters);
		}

		private static Expression<Comparison<T>> CompareCore<T, K>(Expression<Comparison<T>> comparison1, Expression<Func<T, K>> selector, Expression<Comparison<K>> comparison2)
		{
			ParameterExpression varResult = Expression.Variable(typeof(int));

			return Expression.Lambda<Comparison<T>>(
				Expression.Block(
					new[] { varResult },
					Expression.Assign(varResult, comparison1.Body),
					Expression.IfThen(Expression.Equal(varResult, Expression.Constant(0)),
						Expression.Assign(varResult, comparison2.ReplaceParameters(0, selector.ReplaceParameters(0, comparison1.Parameters[0]), selector.ReplaceParameters(0, comparison1.Parameters[1])))),
					varResult),
				comparison1.Parameters);
		}

		#endregion
		#region Internal types

		class ComparisonDeclarationImpl<T> : ParametersDeclaration, IComparisonDeclaration<T>
		{
			internal ComparisonDeclarationImpl()
				: base(Expression.Parameter(typeof(T)), Expression.Parameter(typeof(T)))
			{
			}

			internal ComparisonDeclarationImpl(IParametersDeclaration<T, T> parameters)
				: base(parameters.Parameters)
			{
			}
		}

		class ComparisonExpressionImpl<T> : IComparisonExpression<T>
		{
			private Expression<Comparison<T>> _comparisonExpression;

			internal ComparisonExpressionImpl(Expression<Comparison<T>> comparisonExpression)
				: base()
			{
				if (comparisonExpression == null)
					throw new ArgumentNullException("expression");

				_comparisonExpression = comparisonExpression;
			}

			Expression<Comparison<T>> IComparisonExpression<T>.Expression
			{
				get { return _comparisonExpression; }
			}

			IReadOnlyList<ParameterExpression> IParametersDeclaration.Parameters
			{
				get { return _comparisonExpression.Parameters; }
			}
		}

		#endregion
		#region Compile operations

		public static Comparison<T> Comparison<T>(this IComparisonExpression<T> expression)
		{
			if (expression == null)
				throw new NullReferenceException();

			return expression.Expression.Compile();
		}

		public static IComparer<T> Comparer<T>(this IComparisonExpression<T> expression)
		{
			if (expression == null)
				throw new NullReferenceException();

			return new CustomComparer<T>(expression.Expression.Compile());
		}

		#endregion
	}

	#region Interfaces

	public interface IComparisonDeclaration<T> : IParametersDeclaration<T, T>
	{
	}

	public interface IComparisonExpression<T> : IParametersDeclaration<T, T>
	{
		Expression<Comparison<T>> Expression { get; }
	}

	#endregion
}
