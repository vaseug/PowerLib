using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using PowerLib.System.Collections;
using PowerLib.System.Collections.Matching;
using PowerLib.System.Linq.Expressions;

namespace PowerLib.System.Linq.Builders
{
	public static class EqualityComparerBuilder
	{
		#region Constants

		private const string HasValueProperty = "HasValue";
		private const string ValueProperty = "Value";
		private const string EqualsMethod = "Equals";
		private const string GetHashCodeMethod = "GetHashCode";
		private const string CompareMethod = "Compare";

		#endregion
		#region Public methods
		#region Public starting methods

		public static IHashingDeclaration<T> Hashing<T>(this IParametersDeclaration<T> typeDef)
		{
			return typeDef.Hashing((a, c) => unchecked(a * 31 + c), () => 23);
		}

		public static IHashingDeclaration<T> Hashing<T>(this IParametersDeclaration<T> parameters, Expression<Func<int, int, int>> composer, Expression<Func<int>> seed)
		{
			return new HashingDeclarationImpl<T>(parameters, composer, seed);
		}

		public static IHashingDeclaration<T> Hashing<T>()
		{
			return ParametersBuilder.Declare<T>().Hashing();
		}

		public static IHashingDeclaration<T> Hashing<T>(Expression<Func<int, int, int>> composer, Expression<Func<int>> seed)
		{
			return ParametersBuilder.Declare<T>().Hashing(composer, seed);
		}

		public static IHashingDeclaration<T> Hashing<T>(Expression<Func<T>> typeDef)
		{
			return ParametersBuilder.Declare(typeDef).Hashing();
		}

		public static IHashingDeclaration<T> Hashing<T>(Expression<Func<T>> typeDef, Expression<Func<int, int, int>> composer, Expression<Func<int>> seed)
		{
			return ParametersBuilder.Declare(typeDef).Hashing(composer, seed);
		}

		public static IEqualityDeclaration<T> Equality<T>(this IParametersDeclaration<T, T> parameters)
		{
			return new EqualityDeclarationImpl<T>(parameters);
		}

		public static IEqualityDeclaration<T> Equality<T>()
		{
			return ParametersBuilder.Declare<T, T>().Equality();
		}

		public static IEqualityDeclaration<T> Equality<T>(Expression<Func<T>> typeDef)
		{
			return ParametersBuilder.Declare(typeDef, typeDef).Equality();
		}

		#endregion
		#region Public hashing operations

		public static IHashingExpression<T> Hash<T, K>(this IHashingDeclaration<T> parameters, Expression<Func<T, K>> selector)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new HashingExpressionImpl<T>(HashCore(parameters.Parameters, parameters.Composer, parameters.Seed, selector, DefaultHashing<K>()), parameters.Composer);
		}

		public static IHashingExpression<T> Hash<T, K>(this IHashingExpression<T> expression, Expression<Func<T, K>> selector)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new HashingExpressionImpl<T>(HashCore(expression.Expression, expression.Composer, selector, DefaultHashing<K>()), expression.Composer);
		}

		public static IHashingExpression<T> Hash<T, K>(this IHashingDeclaration<T> parameters, Expression<Func<T, K>> selector, IEqualityComparer<K> equalityComparer)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			return new HashingExpressionImpl<T>(HashCore(parameters.Parameters, parameters.Composer, parameters.Seed, selector, ComparerHashing<K>(equalityComparer)), parameters.Composer);
		}

		public static IHashingExpression<T> Hash<T, K>(this IHashingExpression<T> expression, Expression<Func<T, K>> selector, IEqualityComparer<K> equalityComparer)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			return new HashingExpressionImpl<T>(HashCore(expression.Expression, expression.Composer, selector, ComparerHashing<K>(equalityComparer)), expression.Composer);
		}

		public static IHashingExpression<T> ObjectHash<T, K>(this IHashingDeclaration<T> parameters, Expression<Func<T, K>> selector, IEqualityComparer<K> equalityComparer)
			where K : class
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			return new HashingExpressionImpl<T>(HashCore(parameters.Parameters, parameters.Composer, parameters.Seed, selector, ObjectHashing<K>(ComparerHashing<K>(equalityComparer))), parameters.Composer);
		}

		public static IHashingExpression<T> ObjectHash<T, K>(this IHashingExpression<T> expression, Expression<Func<T, K>> selector, IEqualityComparer<K> equalityComparer)
			where K : class
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			return new HashingExpressionImpl<T>(HashCore(expression.Expression, expression.Composer, selector, ObjectHashing<K>(ComparerHashing<K>(equalityComparer))), expression.Composer);
		}

		public static IHashingExpression<T> NullableHash<T, K>(this IHashingDeclaration<T> parameters, Expression<Func<T, K?>> selector, IEqualityComparer<K> equalityComparer)
			where K : struct
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			return new HashingExpressionImpl<T>(HashCore(parameters.Parameters, parameters.Composer, parameters.Seed, selector, NullableHashing<K>(ComparerHashing<K>(equalityComparer))), parameters.Composer);
		}

		public static IHashingExpression<T> NullableHash<T, K>(this IHashingExpression<T> expression, Expression<Func<T, K?>> selector, IEqualityComparer<K> equalityComparer)
			where K : struct
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			return new HashingExpressionImpl<T>(HashCore(expression.Expression, expression.Composer, selector, NullableHashing<K>(ComparerHashing<K>(equalityComparer))), expression.Composer);
		}

		public static IHashingExpression<T> Hash<T, K>(this IHashingDeclaration<T> parameters, Expression<Func<T, K>> selector, Expression<Func<K, int>> hashing)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (hashing == null)
				throw new ArgumentNullException("hashing");

			return new HashingExpressionImpl<T>(HashCore(parameters.Parameters, parameters.Composer, parameters.Seed, selector, hashing), parameters.Composer);
		}

		public static IHashingExpression<T> Hash<T, K>(this IHashingExpression<T> expression, Expression<Func<T, K>> selector, Expression<Func<K, int>> hashing)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (hashing == null)
				throw new ArgumentNullException("hashing");

			return new HashingExpressionImpl<T>(HashCore(expression.Expression, expression.Composer, selector, hashing), expression.Composer);
		}

		public static IHashingExpression<T> ObjectHash<T, K>(this IHashingDeclaration<T> parameters, Expression<Func<T, K>> selector, Expression<Func<K, int>> hashing)
			where K : class
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (hashing == null)
				throw new ArgumentNullException("hashing");

			return new HashingExpressionImpl<T>(HashCore(parameters.Parameters, parameters.Composer, parameters.Seed, selector, ObjectHashing<K>(hashing)), parameters.Composer);
		}

		public static IHashingExpression<T> ObjectHash<T, K>(this IHashingExpression<T> expression, Expression<Func<T, K>> selector, Expression<Func<K, int>> hashing)
			where K : class
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (hashing == null)
				throw new ArgumentNullException("hashing");

			return new HashingExpressionImpl<T>(HashCore(expression.Expression, expression.Composer, selector, ObjectHashing<K>(hashing)), expression.Composer);
		}

		public static IHashingExpression<T> NullableHash<T, K>(this IHashingDeclaration<T> parameters, Expression<Func<T, K?>> selector, Expression<Func<K, int>> hashing)
			where K : struct
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (hashing == null)
				throw new ArgumentNullException("hashing");

			return new HashingExpressionImpl<T>(HashCore(parameters.Parameters, parameters.Composer, parameters.Seed, selector, NullableHashing<K>(hashing)), parameters.Composer);
		}

		public static IHashingExpression<T> NullableHash<T, K>(this IHashingExpression<T> expression, Expression<Func<T, K?>> selector, Expression<Func<K, int>> hashing)
			where K : struct
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (hashing == null)
				throw new ArgumentNullException("hashing");

			return new HashingExpressionImpl<T>(HashCore(expression.Expression, expression.Composer, selector, NullableHashing<K>(hashing)), expression.Composer);
		}

		#endregion
		#region Public equatable general equals operations

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityDeclaration<T> parameters, Expression<Func<T, K>> selector)
			where K : IEquatable<K>
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new EqualityExpressionImpl<T>(EqualsCore(parameters.Parameters, selector, EquatableEquality<K>()));
		}

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityExpression<T> expression, Expression<Func<T, K>> selector)
			where K : IEquatable<K>
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new EqualityExpressionImpl<T>(EqualsCore(expression.Expression, selector, EquatableEquality<K>()));
		}

		#endregion
		#region Public equatable object equals operations

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityDeclaration<T> parameters, Expression<Func<T, K>> selector, bool nullInequal)
			where K : class, IEquatable<K>
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new EqualityExpressionImpl<T>(EqualsCore(parameters.Parameters, selector, ObjectEquality<K>(EquatableEquality<K>(), nullInequal)));
		}

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityExpression<T> expression, Expression<Func<T, K>> selector, bool nullInequal)
			where K : class, IEquatable<K>
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new EqualityExpressionImpl<T>(EqualsCore(expression.Expression, selector, ObjectEquality<K>(EquatableEquality<K>(), nullInequal)));
		}

		#endregion
		#region Public equatable nullable equals operations

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityDeclaration<T> parameters, Expression<Func<T, K?>> selector, bool nullInequal)
			where K : struct, IEquatable<K>
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new EqualityExpressionImpl<T>(EqualsCore(parameters.Parameters, selector, NullableEquality<K>(EquatableEquality<K>(), nullInequal)));
		}

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityExpression<T> expression, Expression<Func<T, K?>> selector, bool nullInequal)
			where K : struct, IEquatable<K>
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new EqualityExpressionImpl<T>(EqualsCore(expression.Expression, selector, NullableEquality<K>(EquatableEquality<K>(), nullInequal)));
		}

		#endregion
		#region Public general equals operations

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityDeclaration<T> parameters, Expression<Func<T, K>> selector, Expression<Equality<K>> equality)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equality == null)
				throw new ArgumentNullException("equality");

			return new EqualityExpressionImpl<T>(EqualsCore(parameters.Parameters, selector, equality));
		}

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityDeclaration<T> parameters, Expression<Func<T, K>> selector, IEqualityComparer<K> equalityComparer)
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			return new EqualityExpressionImpl<T>(EqualsCore(parameters.Parameters, selector, ComparerEquality(equalityComparer)));
		}

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityExpression<T> expression, Expression<Func<T, K>> selector, Expression<Equality<K>> equality)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equality == null)
				throw new ArgumentNullException("comparison");

			return new EqualityExpressionImpl<T>(EqualsCore(expression.Expression, selector, equality));
		}

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityExpression<T> expression, Expression<Func<T, K>> selector, IEqualityComparer<K> equalityComparer)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			return new EqualityExpressionImpl<T>(EqualsCore(expression.Expression, selector, ComparerEquality(equalityComparer)));
		}

		#endregion
		#region Public object equals operations

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityDeclaration<T> parameters, Expression<Func<T, K>> selector, Expression<Equality<K>> equality, bool nullInequal)
			where K : class
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equality == null)
				throw new ArgumentNullException("equality");

			return new EqualityExpressionImpl<T>(EqualsCore(parameters.Parameters, selector, ObjectEquality(equality, nullInequal)));
		}

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityDeclaration<T> parameters, Expression<Func<T, K>> selector, IEqualityComparer<K> equalityComparer, bool nullInequal)
			where K : class
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			return new EqualityExpressionImpl<T>(EqualsCore(parameters.Parameters, selector, ObjectEquality(ComparerEquality(equalityComparer), nullInequal)));
		}

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityExpression<T> expression, Expression<Func<T, K>> selector, Expression<Equality<K>> equality, bool nullInequal)
			where K : class
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equality == null)
				throw new ArgumentNullException("comparison");

			return new EqualityExpressionImpl<T>(EqualsCore(expression.Expression, selector, ObjectEquality(equality, nullInequal)));
		}

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityExpression<T> expression, Expression<Func<T, K>> selector, IEqualityComparer<K> equalityComparer, bool nullInequal)
			where K : class
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			return new EqualityExpressionImpl<T>(EqualsCore(expression.Expression, selector, ObjectEquality(ComparerEquality(equalityComparer), nullInequal)));
		}

		#endregion
		#region Public nullable equals operations

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityDeclaration<T> parameters, Expression<Func<T, K?>> selector, Expression<Equality<K>> equality, bool nullInequal)
			where K : struct
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equality == null)
				throw new ArgumentNullException("equality");

			return new EqualityExpressionImpl<T>(EqualsCore(parameters.Parameters, selector, NullableEquality(equality, nullInequal)));
		}

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityDeclaration<T> parameters, Expression<Func<T, K?>> selector, IEqualityComparer<K> equalityComparer, bool nullInequal)
			where K : struct
		{
			if (parameters == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			return new EqualityExpressionImpl<T>(EqualsCore(parameters.Parameters, selector, NullableEquality(ComparerEquality(equalityComparer), nullInequal)));
		}

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityExpression<T> expression, Expression<Func<T, K?>> selector, Expression<Equality<K>> equality, bool nullInequal)
			where K : struct
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equality == null)
				throw new ArgumentNullException("comparison");

			return new EqualityExpressionImpl<T>(EqualsCore(expression.Expression, selector, NullableEquality(equality, nullInequal)));
		}

		public static IEqualityExpression<T> Equals<T, K>(this IEqualityExpression<T> expression, Expression<Func<T, K?>> selector, IEqualityComparer<K> equalityComparer, bool nullInequal)
			where K : struct
		{
			if (expression == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			return new EqualityExpressionImpl<T>(EqualsCore(expression.Expression, selector, NullableEquality(ComparerEquality(equalityComparer), nullInequal)));
		}

		#endregion
		#region Hashing helper methods

		public static Expression<Func<T, int>> DefaultHashing<T>()
		{
			MethodInfo miHashing = typeof(object).GetMethod(GetHashCodeMethod, BindingFlags.Public | BindingFlags.Instance);
			ParameterExpression parValue = Expression.Parameter(typeof(T));

			return Expression.Lambda<Func<T, int>>(
				typeof(T).IsValueType ?
					(Expression)Expression.Call(parValue, miHashing) :
					(Expression)Expression.Condition(Expression.NotEqual(parValue, Expression.Constant(null, typeof(T))), Expression.Call(parValue, miHashing), Expression.Constant(0)),
				parValue);
		}

		public static Expression<Func<T, int>> ComparerHashing<T>(IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			MethodInfo miHashing = typeof(IEqualityComparer<T>).GetMethod(GetHashCodeMethod, BindingFlags.Public | BindingFlags.Instance);
			ParameterExpression parValue = Expression.Parameter(typeof(T));

			return Expression.Lambda<Func<T, int>>(
				Expression.Call(Expression.Constant(equalityComparer), miHashing, parValue),
				parValue);
		}

		public static Expression<Func<T, int>> ObjectHashing<T>(Expression<Func<T, int>> hashing)
			where T : class
		{
			if (hashing == null)
				throw new ArgumentNullException("hashing");

			return Expression.Lambda<Func<T, int>>(
				Expression.Condition(Expression.NotEqual(hashing.Parameters[0], Expression.Constant(null, typeof(T))), hashing.Body, Expression.Constant(0)),
				hashing.Parameters);
		}

		public static Expression<Func<T?, int>> NullableHashing<T>(Expression<Func<T, int>> hashing)
			where T : struct
		{
			if (hashing == null)
				throw new ArgumentNullException("hashing");

			PropertyInfo piHasValue = typeof(Nullable<T>).GetProperty(HasValueProperty, BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo piValue = typeof(Nullable<T>).GetProperty(ValueProperty, BindingFlags.Instance | BindingFlags.Public);
			ParameterExpression parValue = Expression.Parameter(typeof(T?));

			return Expression.Lambda<Func<T?, int>>(
				Expression.Condition(Expression.MakeMemberAccess(parValue, piHasValue), hashing.ReplaceParameters(0, Expression.MakeMemberAccess(parValue, piValue)), Expression.Constant(0)),
				parValue);
		}

		#endregion
		#region Equality helper methods

		public static Expression<Equality<T>> EquatableEquality<T>()
			where T : IEquatable<T>
		{
			MethodInfo miEquals = typeof(IEquatable<T>).GetMethod(EqualsMethod, new[] { typeof(T) });
			ParameterExpression parLeft = Expression.Parameter(typeof(T));
			ParameterExpression parRight = Expression.Parameter(typeof(T));

			return Expression.Lambda<Equality<T>>(
				typeof(T).IsValueType ?
					(Expression)Expression.Call(parLeft, miEquals, parRight) :
					(Expression)Expression.Condition(
						Expression.NotEqual(parLeft, Expression.Constant(null, typeof(T))),
						Expression.Condition(
							Expression.NotEqual(parRight, Expression.Constant(null, typeof(T))),
							Expression.Call(parLeft, miEquals, parRight),
							Expression.Constant(false)),
						Expression.Condition(
							Expression.NotEqual(parRight, Expression.Constant(null, typeof(T))),
							Expression.Constant(false),
							Expression.Constant(true))),
				parLeft, parRight);
		}

		public static Expression<Equality<T>> ComparerEquality<T>(IEqualityComparer<T> equalityComparer)
		{
			MethodInfo miEquals = typeof(IEqualityComparer<T>).GetMethod(EqualsMethod, BindingFlags.Public | BindingFlags.Instance);
			ParameterExpression parLeft = Expression.Parameter(typeof(T));
			ParameterExpression parRight = Expression.Parameter(typeof(T));

			return Expression.Lambda<Equality<T>>(
				Expression.Call(Expression.Constant(equalityComparer), miEquals, parLeft, parRight),
				parLeft, parRight);
		}

		public static Expression<Equality<T>> ComparerEquality<T>(IComparer<T> comparer)
		{
			MethodInfo miCompare = typeof(IComparer<T>).GetMethod(CompareMethod, BindingFlags.Public | BindingFlags.Instance);
			ParameterExpression parLeft = Expression.Parameter(typeof(T));
			ParameterExpression parRight = Expression.Parameter(typeof(T));

			return Expression.Lambda<Equality<T>>(
				Expression.Equal(Expression.Call(Expression.Constant(comparer), miCompare, parLeft, parRight), Expression.Constant(0)),
				parLeft, parRight);
		}

		public static Expression<Equality<T>> ComparisonEquality<T>(Expression<Comparison<T>> comparison)
		{
			return Expression.Lambda<Equality<T>>(
				Expression.Equal(comparison.Body, Expression.Constant(0)),
				comparison.Parameters);
		}

		public static Expression<Equality<T>> ObjectEquality<T>(Expression<Equality<T>> equality, bool nullInequal)
			where T : class
		{
			return Expression.Lambda<Equality<T>>(
				Expression.OrElse(
					Expression.AndAlso(
						Expression.AndAlso(Expression.Equal(equality.Parameters[0], Expression.Constant(null, typeof(T))), Expression.Equal(equality.Parameters[1], Expression.Constant(null, typeof(T)))),
						Expression.Constant(!nullInequal)),
					Expression.AndAlso(
						Expression.AndAlso(Expression.NotEqual(equality.Parameters[0], Expression.Constant(null, typeof(T))), Expression.NotEqual(equality.Parameters[1], Expression.Constant(null, typeof(T)))),
						equality.Body)),
				equality.Parameters);
		}

		public static Expression<Equality<T?>> NullableEquality<T>(Expression<Equality<T>> equality, bool nullInequal)
			where T : struct
		{
			MemberInfo mbHasValue = typeof(T?).GetProperty(HasValueProperty);
			MemberInfo mbValue = typeof(T?).GetProperty(ValueProperty);

			ParameterExpression parLeft = Expression.Parameter(typeof(T?));
			ParameterExpression parRight = Expression.Parameter(typeof(T?));
			return Expression.Lambda<Equality<T?>>(
				Expression.OrElse(
					Expression.AndAlso(
						Expression.AndAlso(Expression.Not(Expression.MakeMemberAccess(parLeft, mbHasValue)), Expression.Not(Expression.MakeMemberAccess(parRight, mbHasValue))),
						Expression.Constant(!nullInequal)),
					Expression.AndAlso(
						Expression.AndAlso(Expression.MakeMemberAccess(parLeft, mbHasValue), Expression.MakeMemberAccess(parRight, mbHasValue)),
						equality.ReplaceParameters(0, Expression.MakeMemberAccess(parLeft, mbValue), Expression.MakeMemberAccess(parRight, mbValue)))),
				parLeft, parRight);
		}

		#endregion
		#endregion
		#region Internal methods
		#region Hashing operations

		private static Expression<Func<T, int>> HashCore<T, K>(IReadOnlyList<ParameterExpression> parameters, Expression<Func<int, int, int>> composer, Expression<Func<int>> seed, Expression<Func<T, K>> selector, Expression<Func<K, int>> hashing)
		{
			return Expression.Lambda<Func<T, int>>(
				composer.ReplaceParameters(0, seed.Body, hashing.ReplaceParameters(0, selector.ReplaceParameters(0, parameters[0]))),
				parameters[0]);
		}

		private static Expression<Func<T, int>> HashCore<T, K>(Expression<Func<T, int>> expression, Expression<Func<int, int, int>> composer, Expression<Func<T, K>> selector, Expression<Func<K, int>> hashing)
		{
			return Expression.Lambda<Func<T, int>>(
				composer.ReplaceParameters(0, expression.Body, hashing.ReplaceParameters(0, selector.ReplaceParameters(0, expression.Parameters[0]))),
				expression.Parameters);
		}

		#endregion
		#region Equals methods

		private static Expression<Equality<T>> EqualsCore<T, K>(IReadOnlyList<ParameterExpression> parameters, Expression<Func<T, K>> selector, Expression<Equality<K>> equality)
		{
			return Expression.Lambda<Equality<T>>(
				equality.ReplaceParameters(0, selector.ReplaceParameters(0, parameters[0]), selector.ReplaceParameters(0, parameters[1])),
				parameters);
		}

		private static Expression<Equality<T>> EqualsCore<T, K>(Expression<Equality<T>> expression, Expression<Func<T, K>> selector, Expression<Equality<K>> equality)
		{
			ParameterExpression varResult = Expression.Variable(typeof(bool));

			return Expression.Lambda<Equality<T>>(
				Expression.Block(
					new[] { varResult },
					Expression.Assign(varResult, expression.Body),
					Expression.IfThen(varResult, Expression.Assign(varResult, equality.ReplaceParameters(0, selector.ReplaceParameters(0, expression.Parameters[0]), selector.ReplaceParameters(0, expression.Parameters[1])))),
					varResult),
				expression.Parameters);
		}

		#endregion
		#endregion
		#region Internal types

		class HashingDeclarationImpl<T> : ParametersDeclaration, IHashingDeclaration<T>
		{
			private Expression<Func<int, int, int>> _hashComposer;
			private Expression<Func<int>> _hashSeed;

			internal HashingDeclarationImpl(IParametersDeclaration<T> parameters, Expression<Func<int, int, int>> composer, Expression<Func<int>> seed)
				: base(parameters.Parameters)
			{
				_hashComposer = composer;
				_hashSeed = seed;
			}

			Expression<Func<int, int, int>> IHashingDeclaration<T>.Composer
			{
				get { return _hashComposer; }
			}

			Expression<Func<int>> IHashingDeclaration<T>.Seed
			{
				get { return _hashSeed; }
			}
		}

		class HashingExpressionImpl<T> : IHashingExpression<T>
		{
			private Expression<Func<T, int>> _hashingExpression;
			private Expression<Func<int, int, int>> _hashComposer;

			internal HashingExpressionImpl(Expression<Func<T, int>> expression, Expression<Func<int, int, int>> composer)
			{
				_hashingExpression = expression;
				_hashComposer = composer;
			}

			IReadOnlyList<ParameterExpression> IParametersDeclaration.Parameters
			{
				get { return _hashingExpression.Parameters; }
			}

			Expression<Func<T, int>> IHashingExpression<T>.Expression
			{
				get { return _hashingExpression; }
			}

			Expression<Func<int, int, int>> IHashingExpression<T>.Composer
			{
				get { return _hashComposer; }
			}
		}

		class EqualityDeclarationImpl<T> : ParametersDeclaration, IEqualityDeclaration<T>
		{
			internal EqualityDeclarationImpl()
				: base(Expression.Parameter(typeof(T)), Expression.Parameter(typeof(T)))
			{
			}

			internal EqualityDeclarationImpl(IParametersDeclaration<T, T> parameters)
				: base(parameters.Parameters)
			{
			}
		}

		class EqualityExpressionImpl<T> : IEqualityExpression<T>
		{
			private Expression<Equality<T>> _equalityExpression;

			internal EqualityExpressionImpl(Expression<Equality<T>> equalityExpression)
			{
				_equalityExpression = equalityExpression;
			}

			IReadOnlyList<ParameterExpression> IParametersDeclaration.Parameters
			{
				get { return _equalityExpression.Parameters; }
			}

			Expression<Equality<T>> IEqualityExpression<T>.Expression
			{
				get { return _equalityExpression; }
			}
		}

		#endregion
		#region Compile operations

		public static Equality<T> Equality<T>(this IEqualityExpression<T> equality)
		{
			if (equality == null)
				throw new NullReferenceException();

			return equality.Expression.Compile();
		}

		public static Func<T, int> Hashing<T>(this IHashingExpression<T> hashing)
		{
			if (hashing == null)
				throw new NullReferenceException();

			return hashing.Expression.Compile();
		}

		public static IEqualityComparer<T> EqualityComparer<T>(this IEqualityExpression<T> equality, IHashingExpression<T> hashing)
		{
			if (equality == null)
				throw new NullReferenceException();
			if (hashing == null)
				throw new ArgumentNullException("hashing");

			return new CustomEqualityComparer<T>(equality.Expression.Compile(), hashing.Expression.Compile());
		}

		#endregion
	}

	#region Interfaces

	public interface IEqualityDeclaration<T> : IParametersDeclaration<T, T>
	{
	}

	public interface IEqualityExpression<T> : IParametersDeclaration<T, T>
	{
		Expression<Equality<T>> Expression { get; }
	}

	public interface IHashingDeclaration<T> : IParametersDeclaration<T>
	{
		Expression<Func<int>> Seed { get; }

		Expression<Func<int, int, int>> Composer { get; }
	}

	public interface IHashingExpression<T> : IParametersDeclaration<T>
	{
		Expression<Func<T, int>> Expression { get; }

		Expression<Func<int, int, int>> Composer { get; }
	}

	#endregion
}
