using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PowerLib.System.Linq.Expressions
{
	public class ExpressionVisitor<T> : ExpressionVisitor
		where T : Expression
	{
		private bool _visitNew;
		private Func<T, Expression> _visitor;

		#region Constructors

		public ExpressionVisitor(Func<T, Expression> visitor)
			: this(visitor, false)
		{
		}

		public ExpressionVisitor(Func<T, Expression> visitor, bool visitNew)
		{
			if (visitor == null)
				throw new ArgumentNullException("visitor");

			_visitor = visitor;
			_visitNew = visitNew;
		}

		#endregion
		#region Methods

		public override Expression Visit(Expression node)
		{
			if (node is T)
			{
				Expression result = _visitor((T)node);
				if (!object.ReferenceEquals(node, result))
					if (!_visitNew)
						return result;
					else
						node = result;
			}
			 return base.Visit(node);
		}

		public static Expression Visit(Expression expression, Func<T, Expression> visitor, bool visitNew)
		{
			return new ExpressionVisitor<T>(visitor, visitNew).Visit(expression);
		}

		public static Expression Visit(Expression expression, Func<T, Expression> visitor)
		{
			return new ExpressionVisitor<T>(visitor).Visit(expression);
		}

		#endregion
	}
}
