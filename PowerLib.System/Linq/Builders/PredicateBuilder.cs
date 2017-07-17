using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerLib.System.Collections;
using PowerLib.System.Collections.Matching;
using PowerLib.System.Linq.Expressions;

namespace PowerLib.System.Linq.Builders
{
	public static class PredicateBuilder
	{
		#region Constants

		private const string ExpressionProperty = "Expression";
		private const string MatchMethod = "Match";
		private const string AllMethod = "All";
		private const string AnyMethod = "Any";
		private const string CountMethod = "Count";

		#endregion
		#region Public starting methods

		public static IPredicateDeclaration<T> Matching<T>(this IParametersDeclaration<T> parameters)
		{
			return new PredicateDeclarationImpl<T>(parameters);
		}

		public static IPredicateDeclaration<T> Matching<T>()
		{
			return ParametersBuilder.Declare<T>().Matching();
		}

		public static IPredicateDeclaration<T> Matching<T>(Expression<Func<T>> definition)
		{
			return ParametersBuilder.Declare(definition).Matching();
		}

		#endregion
		#region Public methods
		#region Match operations

		public static IPredicateExpression<T> Match<T>(this IPredicateDeclaration<T> parameters, Expression<Func<T, bool>> match)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					((LambdaExpression)Normalize(match)).ReplaceParameters(0, parameters.Parameters[0]),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Match<T, K>(this IPredicateDeclaration<T> parameters, Expression<Func<T, K>> selector, Expression<Func<K, bool>> match)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					((LambdaExpression)Normalize(match)).ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> MatchWith<T, T1>(this IPredicateDeclaration<T> parameters,
			IParametersDeclaration<T1> param1,
			Expression<Func<T, T1, bool>> match)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (param1 == null)
				throw new ArgumentNullException("param1");
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					((LambdaExpression)Normalize(match)).ReplaceParameters(0, parameters.Parameters[0], param1.Parameters[0]),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> MatchWith<T, T1, T2>(this IPredicateDeclaration<T> parameters,
			IParametersDeclaration<T1> param1, IParametersDeclaration<T2> param2,
			Expression<Func<T, T1, T2, bool>> match)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (param1 == null)
				throw new ArgumentNullException("param1");
			if (param2 == null)
				throw new ArgumentNullException("param2");
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					((LambdaExpression)Normalize(match)).ReplaceParameters(0, parameters.Parameters[0], param1.Parameters[0], param2.Parameters[0]),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> MatchWith<T, T1, T2, T3>(this IPredicateDeclaration<T> parameters,
			IParametersDeclaration<T1> param1, IParametersDeclaration<T2> param2, IParametersDeclaration<T3> param3,
			Expression<Func<T, T1, T2, T3, bool>> match)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (param1 == null)
				throw new ArgumentNullException("param1");
			if (param2 == null)
				throw new ArgumentNullException("param2");
			if (param3 == null)
				throw new ArgumentNullException("param3");
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					((LambdaExpression)Normalize(match)).ReplaceParameters(0, parameters.Parameters[0], param1.Parameters[0], param2.Parameters[0], param3.Parameters[0]),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> MatchWith<T, T1, T2, T3, T4>(this IPredicateDeclaration<T> parameters,
			IParametersDeclaration<T1> param1, IParametersDeclaration<T2> param2, IParametersDeclaration<T3> param3, IParametersDeclaration<T4> param4,
			Expression<Func<T, T1, T2, T3, T4, bool>> match)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (param1 == null)
				throw new ArgumentNullException("param1");
			if (param2 == null)
				throw new ArgumentNullException("param2");
			if (param3 == null)
				throw new ArgumentNullException("param3");
			if (param4 == null)
				throw new ArgumentNullException("param4");
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					((LambdaExpression)Normalize(match)).ReplaceParameters(0, parameters.Parameters[0], param1.Parameters[0], param2.Parameters[0], param3.Parameters[0],
					param4.Parameters[0]),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> MatchWith<T, T1, T2, T3, T4, T5>(this IPredicateDeclaration<T> parameters,
			IParametersDeclaration<T1> param1, IParametersDeclaration<T2> param2, IParametersDeclaration<T3> param3, IParametersDeclaration<T4> param4, IParametersDeclaration<T5> param5,
			Expression<Func<T, T1, T2, T3, T4, T5, bool>> match)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (param1 == null)
				throw new ArgumentNullException("param1");
			if (param2 == null)
				throw new ArgumentNullException("param2");
			if (param3 == null)
				throw new ArgumentNullException("param3");
			if (param4 == null)
				throw new ArgumentNullException("param4");
			if (param5 == null)
				throw new ArgumentNullException("param5");
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					((LambdaExpression)Normalize(match)).ReplaceParameters(0, parameters.Parameters[0], param1.Parameters[0], param2.Parameters[0], param3.Parameters[0],
					param4.Parameters[0], param5.Parameters[0]),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> MatchWith<T, T1, T2, T3, T4, T5, T6>(this IPredicateDeclaration<T> parameters,
			IParametersDeclaration<T1> param1, IParametersDeclaration<T2> param2, IParametersDeclaration<T3> param3, IParametersDeclaration<T4> param4, IParametersDeclaration<T5> param5, IParametersDeclaration<T6> param6,
			Expression<Func<T, T1, T2, T3, T4, T5, T6, bool>> match)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (param1 == null)
				throw new ArgumentNullException("param1");
			if (param2 == null)
				throw new ArgumentNullException("param2");
			if (param3 == null)
				throw new ArgumentNullException("param3");
			if (param4 == null)
				throw new ArgumentNullException("param4");
			if (param5 == null)
				throw new ArgumentNullException("param5");
			if (param6 == null)
				throw new ArgumentNullException("param6");
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					((LambdaExpression)Normalize(match)).ReplaceParameters(0, parameters.Parameters[0], param1.Parameters[0], param2.Parameters[0], param3.Parameters[0],
					param4.Parameters[0], param5.Parameters[0], param6.Parameters[0]),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> MatchWith<T, T1, T2, T3, T4, T5, T6, T7>(this IPredicateDeclaration<T> parameters,
			IParametersDeclaration<T1> param1, IParametersDeclaration<T2> param2, IParametersDeclaration<T3> param3, IParametersDeclaration<T4> param4, IParametersDeclaration<T5> param5, IParametersDeclaration<T6> param6, IParametersDeclaration<T7> param7,
			Expression<Func<T, T1, T2, T3, T4, T5, T6, T7, bool>> match)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (param1 == null)
				throw new ArgumentNullException("param1");
			if (param2 == null)
				throw new ArgumentNullException("param2");
			if (param3 == null)
				throw new ArgumentNullException("param3");
			if (param4 == null)
				throw new ArgumentNullException("param4");
			if (param5 == null)
				throw new ArgumentNullException("param5");
			if (param6 == null)
				throw new ArgumentNullException("param6");
			if (param7 == null)
				throw new ArgumentNullException("param7");
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					match.ReplaceParameters(0, parameters.Parameters[0], param1.Parameters[0], param2.Parameters[0], param3.Parameters[0],
					param4.Parameters[0], param5.Parameters[0], param6.Parameters[0], param7.Parameters[0]),
					parameters.Parameters[0]));
		}

		#endregion
		#region Composite operations

		public static IPredicateExpression<T> And<T>(this IPredicateExpression<T> expression, Expression<Func<T, bool>> match)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.And(expression.Expression.Body, ((LambdaExpression)Normalize(match)).ReplaceParameters(0, expression.Expression.Parameters[0])),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> And<T, K>(this IPredicateExpression<T> expression, Expression<Func<T, K>> selector, Expression<Func<K, bool>> match)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.And(expression.Expression.Body, ((LambdaExpression)Normalize(match)).ReplaceParameters(0, selector.ReplaceParameters(0, expression.Expression.Parameters[0]))),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> And<T>(this IPredicateExpression<T> expression, IPredicateExpression<T> other)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (other == null)
				throw new ArgumentNullException("other");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.And(expression.Expression.Body, other.Expression.ReplaceParameters(0, expression.Expression.Parameters[0])),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> Or<T>(this IPredicateExpression<T> expression, Expression<Func<T, bool>> match)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.Or(expression.Expression.Body, ((LambdaExpression)Normalize(match)).ReplaceParameters(0, expression.Expression.Parameters[0])),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> Or<T, K>(this IPredicateExpression<T> expression, Expression<Func<T, K>> selector, Expression<Func<K, bool>> match)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (match == null)
				throw new ArgumentNullException("match");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.Or(expression.Expression.Body, ((LambdaExpression)Normalize(match)).ReplaceParameters(0, selector.ReplaceParameters(0, expression.Expression.Parameters[0]))),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> Or<T>(this IPredicateExpression<T> expression, IPredicateExpression<T> other)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (other == null)
				throw new ArgumentNullException("other");

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.Or(expression.Expression.Body, other.Expression.ReplaceParameters(0, expression.Expression.Parameters[0])),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> Not<T>(this IPredicateExpression<T> expression)
		{
			if (expression == null)
				throw new NullReferenceException();

			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.Not(expression.Expression.Body),
					expression.Expression.Parameters[0]));
		}

		#endregion
		#region Equals operations

		public static IPredicateExpression<T> Equals<T, K>(this IPredicateDeclaration<T> parameters, Expression<Func<T, K>> selector, K value)
			where K : IEquatable<K>
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					EqualityPredicate(EqualityComparerBuilder.EquatableEquality<K>(), Expression.Lambda<Func<K>>(Expression.Constant(value)))
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Equals<T, K>(this IPredicateDeclaration<T> parameters, Expression<Func<T, K>> selector, IParametersDeclaration<K> variable)
			where K : IEquatable<K>
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					EqualityPredicate(EqualityComparerBuilder.EquatableEquality<K>(), Expression.Lambda<Func<K>>(variable.Parameters[0]))
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Equals<T, K>(this IPredicateDeclaration<T> parameters, IEqualityComparer<K> equalityComparer, Expression<Func<T, K>> selector, K value)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					EqualityPredicate(EqualityComparerBuilder.ComparerEquality(equalityComparer), Expression.Lambda<Func<K>>(Expression.Constant(value)))
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Equals<T, K>(this IPredicateDeclaration<T> parameters, IEqualityComparer<K> equalityComparer, Expression<Func<T, K>> selector, IParametersDeclaration<K> variable)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					EqualityPredicate(EqualityComparerBuilder.ComparerEquality(equalityComparer), Expression.Lambda<Func<K>>(variable.Parameters[0]))
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Equals<T, K>(this IPredicateDeclaration<T> parameters, Expression<Equality<K>> equality, Expression<Func<T, K>> selector, K value)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					EqualityPredicate(equality, Expression.Lambda<Func<K>>(Expression.Constant(value)))
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Equals<T, K>(this IPredicateDeclaration<T> parameters, Expression<Equality<K>> equality, Expression<Func<T, K>> selector, IParametersDeclaration<K> variable)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					EqualityPredicate(equality, Expression.Lambda<Func<K>>(variable.Parameters[0]))
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		#endregion
		#region Compare operations

		public static IPredicateExpression<T> Compare<T, K>(this IPredicateDeclaration<T> parameters, Expression<Func<T, K>> selector, K value, ComparisonCriteria criteria)
			where K : IComparable<K>
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					ComparisonPredicate(ComparerBuilder.ComparableComparison<K>(), Expression.Lambda<Func<K>>(Expression.Constant(value)), criteria)
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Compare<T, K>(this IPredicateDeclaration<T> parameters, Expression<Func<T, K>> selector, IParametersDeclaration<K> variable, ComparisonCriteria criteria)
			where K : IComparable<K>
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					ComparisonPredicate(ComparerBuilder.ComparableComparison<K>(), Expression.Lambda<Func<K>>(variable.Parameters[0]), criteria)
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Compare<T, K>(this IPredicateDeclaration<T> parameters, IComparer<K> comparer, Expression<Func<T, K>> selector, K value, ComparisonCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					ComparisonPredicate(ComparerBuilder.ComparerComparison(comparer), Expression.Lambda<Func<K>>(Expression.Constant(value)), criteria)
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Compare<T, K>(this IPredicateDeclaration<T> parameters, IComparer<K> comparer, Expression<Func<T, K>> selector, IParametersDeclaration<K> variable, ComparisonCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					ComparisonPredicate(ComparerBuilder.ComparerComparison(comparer), Expression.Lambda<Func<K>>(variable.Parameters[0]), criteria)
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Compare<T, K>(this IPredicateDeclaration<T> parameters, Expression<Comparison<K>> comparison, Expression<Func<T, K>> selector, K value, ComparisonCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					ComparisonPredicate(comparison, Expression.Lambda<Func<K>>(Expression.Constant(value)), criteria)
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Compare<T, K>(this IPredicateDeclaration<T> parameters, Expression<Comparison<K>> comparison, Expression<Func<T, K>> selector, IParametersDeclaration<K> variable, ComparisonCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					ComparisonPredicate(comparison, Expression.Lambda<Func<K>>(variable.Parameters[0]), criteria)
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		#endregion
		#region Between operations

		public static IPredicateExpression<T> Between<T, K>(this IPredicateDeclaration<T> parameters, IComparer<K> comparer, Expression<Func<T, K>> selector, K lower, K upper, BetweenCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					BetweenPredicate(ComparerBuilder.ComparerComparison(comparer), Expression.Lambda<Func<K>>(Expression.Constant(lower)), Expression.Lambda<Func<K>>(Expression.Constant(upper)), criteria)
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Between<T, K>(this IPredicateDeclaration<T> parameters, IComparer<K> comparer, Expression<Func<T, K>> selector, IParametersDeclaration<K> lower, IParametersDeclaration<K> upper, BetweenCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					BetweenPredicate(ComparerBuilder.ComparerComparison(comparer), Expression.Lambda<Func<K>>(lower.Parameters[0]), Expression.Lambda<Func<K>>(upper.Parameters[0]), criteria)
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Between<T, K>(this IPredicateDeclaration<T> parameters, Expression<Comparison<K>> comparison, Expression<Func<T, K>> selector, K lower, K upper, BetweenCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					BetweenPredicate(comparison, Expression.Lambda<Func<K>>(Expression.Constant(lower)), Expression.Lambda<Func<K>>(Expression.Constant(upper)), criteria)
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Between<T, K>(this IPredicateDeclaration<T> parameters, Expression<Comparison<K>> comparison, Expression<Func<T, K>> selector, IParametersDeclaration<K> lower, IParametersDeclaration<K> upper, BetweenCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					BetweenPredicate(comparison, Expression.Lambda<Func<K>>(lower.Parameters[0]), Expression.Lambda<Func<K>>(upper.Parameters[0]), criteria)
						.ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		#endregion
		#region Quantify operations

		public static IPredicateExpression<T> Quantify<T, K>(this IPredicateDeclaration<T> parameters, Expression<Func<T, IEnumerable<K>>> selector, Expression<Func<K, bool>> match, QuantifyCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					EnumerableQuantifyPredicate((Expression<Func<K, bool>>)Normalize(match), criteria).ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Quantify<T, K>(this IPredicateDeclaration<T> parameters, Expression<Func<T, IEnumerable<K>>> selector, Expression<Func<K, bool>> match, Expression<Func<int, bool>> quantification)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					EnumerableQuantifyPredicate((Expression<Func<K, bool>>)Normalize(match), quantification).ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Quantify<T, K>(this IPredicateDeclaration<T> parameters, Expression<Func<T, IQueryable<K>>> selector, Expression<Func<K, bool>> match, QuantifyCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					QueryableQuantifyPredicate((Expression<Func<K, bool>>)Normalize(match), criteria).ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> Quantify<T, K>(this IPredicateDeclaration<T> parameters, Expression<Func<T, IQueryable<K>>> selector, Expression<Func<K, bool>> match, Expression<Func<int, bool>> quantification)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					QueryableQuantifyPredicate((Expression<Func<K, bool>>)Normalize(match), quantification).ReplaceParameters(0, selector.ReplaceParameters(0, parameters.Parameters[0])),
					parameters.Parameters[0]));
		}

		public static IPredicateExpression<T> AndQuantify<T, K>(this IPredicateExpression<T> expression, Expression<Func<T, IEnumerable<K>>> selector, Expression<Func<K, bool>> match, QuantifyCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.AndAlso(
						expression.Expression.Body,
						EnumerableQuantifyPredicate((Expression<Func<K, bool>>)Normalize(match), criteria).ReplaceParameters(0, selector.ReplaceParameters(0, expression.Expression.Parameters[0]))),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> AndQuantify<T, K>(this IPredicateExpression<T> expression, Expression<Func<T, IEnumerable<K>>> selector, Expression<Func<K, bool>> match, Expression<Func<int, bool>> quantification)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.AndAlso(
						expression.Expression.Body,
						EnumerableQuantifyPredicate((Expression<Func<K, bool>>)Normalize(match), quantification).ReplaceParameters(0, selector.ReplaceParameters(0, expression.Expression.Parameters[0]))),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> AndQuantify<T, K>(this IPredicateExpression<T> expression, Expression<Func<T, IQueryable<K>>> selector, Expression<Func<K, bool>> match, QuantifyCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.AndAlso(
						expression.Expression.Body,
						QueryableQuantifyPredicate((Expression<Func<K, bool>>)Normalize(match), criteria).ReplaceParameters(0, selector.ReplaceParameters(0, expression.Expression.Parameters[0]))),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> AndQuantify<T, K>(this IPredicateExpression<T> expression, Expression<Func<T, IQueryable<K>>> selector, Expression<Func<K, bool>> match, Expression<Func<int, bool>> quantification)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.AndAlso(
						expression.Expression.Body,
						QueryableQuantifyPredicate((Expression<Func<K, bool>>)Normalize(match), quantification).ReplaceParameters(0, selector.ReplaceParameters(0, expression.Expression.Parameters[0]))),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> OrQuantify<T, K>(this IPredicateExpression<T> expression, Expression<Func<T, IEnumerable<K>>> selector, Expression<Func<K, bool>> match, QuantifyCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.OrElse(
						expression.Expression.Body,
						EnumerableQuantifyPredicate((Expression<Func<K, bool>>)Normalize(match), criteria).ReplaceParameters(0, selector.ReplaceParameters(0, expression.Expression.Parameters[0]))),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> OrQuantify<T, K>(this IPredicateExpression<T> expression, Expression<Func<T, IEnumerable<K>>> selector, Expression<Func<K, bool>> match, Expression<Func<int, bool>> quantification)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.OrElse(
						expression.Expression.Body,
						EnumerableQuantifyPredicate((Expression<Func<K, bool>>)Normalize(match), quantification).ReplaceParameters(0, selector.ReplaceParameters(0, expression.Expression.Parameters[0]))),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> OrQuantify<T, K>(this IPredicateExpression<T> expression, Expression<Func<T, IQueryable<K>>> selector, Expression<Func<K, bool>> match, QuantifyCriteria criteria)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.OrElse(
						expression.Expression.Body,
						QueryableQuantifyPredicate((Expression<Func<K, bool>>)Normalize(match), criteria).ReplaceParameters(0, selector.ReplaceParameters(0, expression.Expression.Parameters[0]))),
					expression.Expression.Parameters[0]));
		}

		public static IPredicateExpression<T> OrQuantify<T, K>(this IPredicateExpression<T> expression, Expression<Func<T, IQueryable<K>>> selector, Expression<Func<K, bool>> match, Expression<Func<int, bool>> quantification)
		{
			return new PredicateExpressionImpl<T>(
				Expression.Lambda<Func<T, bool>>(
					Expression.OrElse(
						expression.Expression.Body,
						QueryableQuantifyPredicate((Expression<Func<K, bool>>)Normalize(match), quantification).ReplaceParameters(0, selector.ReplaceParameters(0, expression.Expression.Parameters[0]))),
					expression.Expression.Parameters[0]));
		}

		#endregion
		#region Predicate helper operations

		public static Expression<Func<T, bool>> Predicate<T>(IPredicate<T> predicate)
		{
			MethodInfo miMatch = typeof(IPredicate<T>).GetMethod(MatchMethod, BindingFlags.Public | BindingFlags.Instance);
			ParameterExpression parValue = Expression.Parameter(typeof(T));

			return Expression.Lambda<Func<T, bool>>(
				Expression.Call(Expression.Constant(predicate), miMatch, parValue),
				parValue);
		}

		public static Expression<Func<T, bool>> EqualityPredicate<T>(Expression<Equality<T>> equality, Expression<Func<T>> value)
		{
			return Expression.Lambda<Func<T, bool>>(
				equality.ReplaceParameters(1, value.Body),
				equality.Parameters[0]);
		}

		public static Expression<Func<T, bool>> ComparisonPredicate<T>(Expression<Comparison<T>> comparison, Expression<Func<T>> value, ComparisonCriteria criteria)
		{
			Expression expression;
			switch (criteria)
			{
				case ComparisonCriteria.Equal:
					expression = Expression.Equal(comparison.ReplaceParameters(1, value.Body), Expression.Constant(0));
					break;
				case ComparisonCriteria.NotEqual:
					expression = Expression.NotEqual(comparison.ReplaceParameters(1, value.Body), Expression.Constant(0));
					break;
				case ComparisonCriteria.LessThan:
					expression = Expression.LessThan(comparison.ReplaceParameters(1, value.Body), Expression.Constant(0));
					break;
				case ComparisonCriteria.GreaterThan:
					expression = Expression.GreaterThan(comparison.ReplaceParameters(1, value.Body), Expression.Constant(0));
					break;
				case ComparisonCriteria.LessThanOrEqual:
					expression = Expression.LessThanOrEqual(comparison.ReplaceParameters(1, value.Body), Expression.Constant(0));
					break;
				case ComparisonCriteria.GreaterThanOrEqual:
					expression = Expression.GreaterThanOrEqual(comparison.ReplaceParameters(1, value.Body), Expression.Constant(0));
					break;
				default:
					throw new ArgumentException("Invalid comparison criteria");
			}

			return Expression.Lambda<Func<T, bool>>(
				expression,
				comparison.Parameters[0]);
		}

		public static Expression<Func<T, bool>> BetweenPredicate<T>(Expression<Comparison<T>> comparison, Expression<Func<T>> lowerValue, Expression<Func<T>> upperValue, BetweenCriteria criteria)
		{
			Expression expression;
			switch (criteria)
			{
				case BetweenCriteria.ExcludeBoth:
					expression = Expression.AndAlso(
						Expression.GreaterThan(comparison.ReplaceParameters(1, lowerValue.Body), Expression.Constant(0)),
						Expression.LessThan(comparison.ReplaceParameters(1, upperValue.Body), Expression.Constant(0)));
					break;
				case BetweenCriteria.ExcludeLower:
					expression = Expression.AndAlso(
						Expression.GreaterThan(comparison.ReplaceParameters(1, lowerValue.Body), Expression.Constant(0)),
						Expression.LessThanOrEqual(comparison.ReplaceParameters(1, upperValue.Body), Expression.Constant(0)));
					break;
				case BetweenCriteria.ExcludeUpper:
					expression = Expression.AndAlso(
						Expression.GreaterThanOrEqual(comparison.ReplaceParameters(1, lowerValue.Body), Expression.Constant(0)),
						Expression.LessThan(comparison.ReplaceParameters(1, upperValue.Body), Expression.Constant(0)));
					break;
				case BetweenCriteria.IncludeBoth:
					expression = Expression.AndAlso(
						Expression.GreaterThanOrEqual(comparison.ReplaceParameters(1, lowerValue.Body), Expression.Constant(0)),
						Expression.LessThanOrEqual(comparison.ReplaceParameters(1, upperValue.Body), Expression.Constant(0)));
					break;
				default:
					throw new ArgumentException("Invalid between criteria");
			}

			return Expression.Lambda<Func<T, bool>>(
				expression,
				comparison.Parameters[0]);
		}

		public static Expression<Func<IEnumerable<T>, bool>> EnumerableQuantifyPredicate<T>(Expression<Func<T, bool>> predication, QuantifyCriteria criteria)
		{
			string method;
			switch (criteria)
			{
				case QuantifyCriteria.Any:
					method = AnyMethod;
					break;
				case QuantifyCriteria.All:
					method = AllMethod;
					break;
				default:
					throw new ArgumentException("Invalid quantify criteria");
			}
			MethodInfo miQuantify = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Where(mi =>
				{
					if (mi.Name != method || !mi.IsGenericMethodDefinition)
						return false;
					Type[] arguments = mi.GetGenericArguments();
					if (arguments.Length != 1)
						return false;
					ParameterInfo[] parameters = mi.GetParameters();
					if (parameters.Length != 2)
						return false;
					if (parameters[0].ParameterType != typeof(IEnumerable<>).MakeGenericType(arguments))
						return false;
					if (parameters[1].ParameterType != typeof(Func<,>).MakeGenericType(arguments[0], typeof(bool)))
						return false;
					return true;
				})
				.FirstOrDefault();
			ParameterExpression parColl = Expression.Parameter(typeof(IEnumerable<T>));

			return Expression.Lambda<Func<IEnumerable<T>, bool>>(
				Expression.Call(miQuantify.MakeGenericMethod(typeof(T)), parColl, Expression.Constant(predication.Compile())),
				parColl);
		}

		public static Expression<Func<IEnumerable<T>, bool>> EnumerableQuantifyPredicate<T>(Expression<Func<T, bool>> predication, Expression<Func<int, bool>> quantification)
		{
			MethodInfo miCount = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Where(mi =>
				{
					if (mi.Name != CountMethod || !mi.IsGenericMethodDefinition)
						return false;
					Type[] arguments = mi.GetGenericArguments();
					if (arguments.Length != 1)
						return false;
					ParameterInfo[] parameters = mi.GetParameters();
					if (parameters.Length != 2)
						return false;
					if (parameters[0].ParameterType != typeof(IEnumerable<>).MakeGenericType(arguments))
						return false;
					if (parameters[1].ParameterType != typeof(Func<,>).MakeGenericType(arguments[0], typeof(bool)))
						return false;
					return true;
				})
				.FirstOrDefault();
			ParameterExpression parColl = Expression.Parameter(typeof(IEnumerable<T>));

			return Expression.Lambda<Func<IEnumerable<T>, bool>>(
				quantification.ReplaceParameters(0, Expression.Call(miCount.MakeGenericMethod(typeof(T)), parColl, Expression.Constant(predication.Compile()))),
				parColl);
		}

		public static Expression<Func<IQueryable<T>, bool>> QueryableQuantifyPredicate<T>(Expression<Func<T, bool>> predication, QuantifyCriteria criteria)
		{
			string method;
			switch (criteria)
			{
				case QuantifyCriteria.Any:
					method = AnyMethod;
					break;
				case QuantifyCriteria.All:
					method = AllMethod;
					break;
				default:
					throw new ArgumentException("Invalid quantify criteria");
			}
			MethodInfo miQuantify = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Where(mi =>
				{
					if (mi.Name != method || !mi.IsGenericMethodDefinition)
						return false;
					Type[] arguments = mi.GetGenericArguments();
					if (arguments.Length != 1)
						return false;
					ParameterInfo[] parameters = mi.GetParameters();
					if (parameters.Length != 2)
						return false;
					if (parameters[0].ParameterType != typeof(IQueryable<>).MakeGenericType(arguments))
						return false;
					if (parameters[1].ParameterType != typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(arguments[0], typeof(bool))))
						return false;
					return true;
				})
				.FirstOrDefault();
			ParameterExpression parColl = Expression.Parameter(typeof(IQueryable<T>));

			return Expression.Lambda<Func<IQueryable<T>, bool>>(
				Expression.Call(miQuantify.MakeGenericMethod(typeof(T)), parColl, Expression.Quote(predication)),
				parColl);
		}

		public static Expression<Func<IQueryable<T>, bool>> QueryableQuantifyPredicate<T>(Expression<Func<T, bool>> predication, Expression<Func<int, bool>> quantification)
		{
			MethodInfo miCount = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Where(mi =>
				{
					if (mi.Name != CountMethod || !mi.IsGenericMethodDefinition)
						return false;
					Type[] arguments = mi.GetGenericArguments();
					if (arguments.Length != 1)
						return false;
					ParameterInfo[] parameters = mi.GetParameters();
					if (parameters.Length != 2)
						return false;
					if (parameters[0].ParameterType != typeof(IQueryable<>).MakeGenericType(arguments))
						return false;
					if (parameters[1].ParameterType != typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(arguments[0], typeof(bool))))
						return false;
					return true;
				})
				.FirstOrDefault();
			ParameterExpression parColl = Expression.Parameter(typeof(IQueryable<T>));

			return Expression.Lambda<Func<IQueryable<T>, bool>>(
				quantification.ReplaceParameters(0, Expression.Call(miCount.MakeGenericMethod(typeof(T)), parColl, Expression.Quote(predication))),
				parColl);
		}

		#endregion
		#endregion
		#region Internal methods

		private static Expression Normalize(Expression expression)
		{
			return expression.Visit((Expression e) =>
			{
				if (e.NodeType == ExpressionType.MemberAccess)
				{
					MemberExpression me = (MemberExpression)e;
					if (me.Member.MemberType == MemberTypes.Property && me.Member.DeclaringType.IsGenericType && me.Member.DeclaringType.GetGenericTypeDefinition() == typeof(IPredicateExpression<>) && me.Member.Name == ExpressionProperty)
					{
						PwrStack<MemberExpression> stack = new PwrStack<MemberExpression>();
						do
						{
							stack.Push(me);
							me = me.Expression as MemberExpression;
						}
						while (me != null);
						me = stack.Pop();
						if (me.Expression.NodeType == ExpressionType.Constant)
						{
							object obj = ((ConstantExpression)me.Expression).Value;
							do
							{
								switch (me.Member.MemberType)
								{
									case MemberTypes.Field:
										obj = ((FieldInfo)me.Member).GetValue(obj);
										break;
									case MemberTypes.Property:
										obj = ((PropertyInfo)me.Member).GetValue(obj);
										break;
								}
								me = stack.Count > 0 ? stack.Pop() : null;
							}
							while (me != null);
							return Expression.Quote(((Expression)obj));
						}
						else
							stack.Clear();
					}
				}
				return e;
			}, false);
		}

		#endregion
		#region Internal types

		class PredicateDeclarationImpl<T> : ParametersDeclaration, IPredicateDeclaration<T>
		{
			internal PredicateDeclarationImpl()
				: base(Expression.Parameter(typeof(T)))
			{
			}

			internal PredicateDeclarationImpl(IParametersDeclaration<T> parameters)
				: base(parameters.Parameters)
			{
			}
		}

		class PredicateExpressionImpl<T> : IPredicateExpression<T>
		{
			private Expression<Func<T, bool>> _matchExpression;

			internal PredicateExpressionImpl(Expression<Func<T, bool>> matchExpression)
			{
				if (matchExpression == null)
					throw new ArgumentNullException("matchExpression");

				_matchExpression = matchExpression;
			}

			IReadOnlyList<ParameterExpression> IParametersDeclaration.Parameters
			{
				get { return _matchExpression.Parameters; }
			}

			Expression<Func<T, bool>> IPredicateExpression<T>.Expression
			{
				get { return _matchExpression; }
			}
		}

		#endregion
		#region Compile operations

		public static Func<T, bool> Match<T>(this IPredicateExpression<T> expression)
		{
			if (expression == null)
				throw new NullReferenceException();

			return expression.Expression.Compile();
		}

		public static IPredicate<T> Predicate<T>(this IPredicateExpression<T> expression)
		{
			if (expression == null)
				throw new NullReferenceException();

			return new CustomPredicate<T>(expression.Expression.Compile());
		}

		#endregion
	}

	#region Interfaces

	public interface IPredicateDeclaration<T> : IParametersDeclaration<T>
	{
	}

	public interface IPredicateExpression<T> : IParametersDeclaration<T>
	{
		Expression<Func<T, bool>> Expression { get; }
	}

	#endregion
}
