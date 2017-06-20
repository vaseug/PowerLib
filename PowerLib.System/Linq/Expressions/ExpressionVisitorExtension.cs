using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PowerLib.System.Collections;

namespace PowerLib.System.Linq.Expressions
{
	public static class ExpressionVisitorExtension
	{
		public static Expression Visit<T>(this Expression expression, Func<T, Expression> visitor, bool visitNew)
			where T : Expression
		{
			return ExpressionVisitor<T>.Visit(expression, visitor, visitNew);
		}

		public static Expression Visit<T>(this Expression expression, Func<T, Expression> visitor)
			where T : Expression
		{
			return ExpressionVisitor<T>.Visit(expression, visitor);
		}

		public static Expression ReplaceParameters(this LambdaExpression expression, int startIndex, params Expression[] substitutions)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (startIndex < 0 || startIndex > expression.Parameters.Count)
				throw new ArgumentOutOfRangeException("startIndex");
			if (substitutions == null)
				throw new ArgumentNullException("substitutions");
			if (startIndex + substitutions.Length > expression.Parameters.Count)
				throw new ArgumentException("Invalid array length");

			return expression.Body.Visit((ParameterExpression p) =>
			{
				int index = expression.Parameters.FindIndex(startIndex, substitutions.Length, t => object.ReferenceEquals(t, p));
				return (index >= 0) ? substitutions[index - startIndex] : p;
			}, false);
		}

		public static Expression ReplaceParameters<TDelegate>(this Expression<TDelegate> expression, int startIndex, params Expression[] substitutions)
		{
			if (expression == null)
				throw new NullReferenceException();
			if (startIndex < 0 || startIndex > expression.Parameters.Count)
				throw new ArgumentOutOfRangeException("startIndex");
			if (substitutions == null)
				throw new ArgumentNullException("substitutions");
			if (startIndex + substitutions.Length > expression.Parameters.Count)
				throw new ArgumentException("Invalid array length");

			return expression.Body.Visit((ParameterExpression p) =>
				{
					int index = expression.Parameters.FindIndex(startIndex, substitutions.Length, t => object.ReferenceEquals(t, p));
					return (index >= 0) ? substitutions[index - startIndex] : p;
				}, false);
		}
	}
}
