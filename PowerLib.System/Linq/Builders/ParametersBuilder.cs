using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace PowerLib.System.Linq.Builders
{
	public static class ParametersBuilder
	{
		#region Internal methods

		private static bool IsAnonymousDefinition<T>(Expression<Func<T>> definer)
		{
			return typeof(T).IsAnonymous() && (definer.Body.NodeType == ExpressionType.MemberInit || definer.Body.NodeType == ExpressionType.New);
		}

		#endregion
		#region Public methods

		public static IParametersDeclaration Declare(params Type[] types)
		{
			if (types.Any(t => t == null))
				throw new ArgumentException("Empty type element", "types");

			return new ParametersDeclarationImpl(types);
		}

		public static IParametersDeclaration<T> Declare<T>()
		{
			return new ParametersDeclarationImpl<T>();
		}

		public static IParametersDeclaration<T> Declare<T>(Expression<Func<T>> definition)
		{
			return Declare<T>();
		}

		public static IParametersDeclaration<T1, T2> Declare<T1, T2>()
		{
			return new ParametersDeclarationImpl<T1, T2>();
		}

		public static IParametersDeclaration<T1, T2> Declare<T1, T2>(Expression<Func<T1>> definition1, Expression<Func<T2>> definition2)
		{
			return Declare<T1, T2>();
		}

		public static IParametersDeclaration<T1, T2, T3> Declare<T1, T2, T3>()
		{
			return new ParametersDeclarationImpl<T1, T2, T3>();
		}

		public static IParametersDeclaration<T1, T2, T3> Declare<T1, T2, T3>(Expression<Func<T1>> definition1, Expression<Func<T2>> definition2, Expression<Func<T3>> definition3)
		{
			return Declare<T1, T2, T3>();
		}

		public static IParametersDeclaration<T1, T2, T3, T4> Declare<T1, T2, T3, T4>()
		{
			return new ParametersDeclarationImpl<T1, T2, T3, T4>();
		}

		public static IParametersDeclaration<T1, T2, T3, T4> Declare<T1, T2, T3, T4>(Expression<Func<T1>> definition1, Expression<Func<T2>> definition2, Expression<Func<T3>> definition3, Expression<Func<T4>> definition4)
		{
			return Declare<T1, T2, T3, T4>();
		}

		public static IParametersDeclaration<T1, T2, T3, T4, T5> Declare<T1, T2, T3, T4, T5>()
		{
			return new ParametersDeclarationImpl<T1, T2, T3, T4, T5>();
		}

		public static IParametersDeclaration<T1, T2, T3, T4, T5> Declare<T1, T2, T3, T4, T5>(Expression<Func<T1>> definition1, Expression<Func<T2>> definition2, Expression<Func<T3>> definition3, Expression<Func<T4>> definition4, Expression<Func<T5>> definition5)
		{
			return Declare<T1, T2, T3, T4, T5>();
		}

		public static IParametersDeclaration<T1, T2, T3, T4, T5, T6> Declare<T1, T2, T3, T4, T5, T6>()
		{
			return new ParametersDeclarationImpl<T1, T2, T3, T4, T5, T6>();
		}

		public static IParametersDeclaration<T1, T2, T3, T4, T5, T6> Declare<T1, T2, T3, T4, T5, T6>(Expression<Func<T1>> definition1, Expression<Func<T2>> definition2, Expression<Func<T3>> definition3, Expression<Func<T4>> definition4, Expression<Func<T5>> definition5, Expression<Func<T6>> definition6)
		{
			return Declare<T1, T2, T3, T4, T5, T6>();
		}

		public static IParametersDeclaration<T1, T2, T3, T4, T5, T6, T7> Declare<T1, T2, T3, T4, T5, T6, T7>()
		{
			return new ParametersDeclarationImpl<T1, T2, T3, T4, T5, T6, T7>();
		}

		public static IParametersDeclaration<T1, T2, T3, T4, T5, T6, T7> Declare<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1>> definition1, Expression<Func<T2>> definition2, Expression<Func<T3>> definition3, Expression<Func<T4>> definition4, Expression<Func<T5>> definition5, Expression<Func<T6>> definition6, Expression<Func<T7>> definition7)
		{
			return Declare<T1, T2, T3, T4, T5, T6, T7>();
		}

		#endregion
		#region Internal types

		class ParametersDeclarationImpl : ParametersDeclaration
		{
			internal ParametersDeclarationImpl(IEnumerable<Type> parameters)
				: base(parameters.Select(t => Expression.Parameter(t)))
			{
			}

			internal ParametersDeclarationImpl(params Type[] parameters)
				: base(parameters.Select(t => Expression.Parameter(t)))
			{
			}
		}

		class ParametersDeclarationImpl<T> : ParametersDeclarationImpl, IParametersDeclaration<T>
		{
			internal ParametersDeclarationImpl()
				: base(typeof(T))
			{
			}
		}

		class ParametersDeclarationImpl<T1, T2> : ParametersDeclarationImpl, IParametersDeclaration<T1, T2>
		{
			internal ParametersDeclarationImpl()
				: base(typeof(T1), typeof(T2))
			{
			}
		}

		class ParametersDeclarationImpl<T1, T2, T3> : ParametersDeclarationImpl, IParametersDeclaration<T1, T2, T3>
		{
			internal ParametersDeclarationImpl()
				: base(typeof(T1), typeof(T2), typeof(T3))
			{
			}
		}

		class ParametersDeclarationImpl<T1, T2, T3, T4> : ParametersDeclarationImpl, IParametersDeclaration<T1, T2, T3, T4>
		{
			internal ParametersDeclarationImpl()
				: base(typeof(T1), typeof(T2), typeof(T3), typeof(T4))
			{
			}
		}

		class ParametersDeclarationImpl<T1, T2, T3, T4, T5> : ParametersDeclarationImpl, IParametersDeclaration<T1, T2, T3, T4, T5>
		{
			internal ParametersDeclarationImpl()
				: base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5))
			{
			}
		}

		class ParametersDeclarationImpl<T1, T2, T3, T4, T5, T6> : ParametersDeclarationImpl, IParametersDeclaration<T1, T2, T3, T4, T5, T6>
		{
			internal ParametersDeclarationImpl()
				: base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))
			{
			}
		}

		class ParametersDeclarationImpl<T1, T2, T3, T4, T5, T6, T7> : ParametersDeclarationImpl, IParametersDeclaration<T1, T2, T3, T4, T5, T6, T7>
		{
			internal ParametersDeclarationImpl()
				: base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7))
			{
			}
		}

		#endregion
	}

	internal class ParametersDeclaration : IParametersDeclaration
	{
		private IReadOnlyList<ParameterExpression> _parameters;

		public ParametersDeclaration(IList<ParameterExpression> parameters)
		{
			_parameters = new ReadOnlyCollection<ParameterExpression>(parameters);
		}

		public ParametersDeclaration(IEnumerable<ParameterExpression> parameters)
			: this(parameters.ToArray())
		{
		}

		public ParametersDeclaration(params ParameterExpression[] parameters)
			: this((IList<ParameterExpression>)parameters)
		{
		}

		IReadOnlyList<ParameterExpression> IParametersDeclaration.Parameters
		{
			get { return _parameters; }
		}
	}

	#region Interfaces

	public interface IParametersDeclaration
	{
		IReadOnlyList<ParameterExpression> Parameters { get; }
	}

	public interface IParametersDeclaration<T> : IParametersDeclaration
	{
	}

	public interface IParametersDeclaration<T1, T2> : IParametersDeclaration
	{
	}

	public interface IParametersDeclaration<T1, T2, T3> : IParametersDeclaration
	{
	}

	public interface IParametersDeclaration<T1, T2, T3, T4> : IParametersDeclaration
	{
	}

	public interface IParametersDeclaration<T1, T2, T3, T4, T5> : IParametersDeclaration
	{
	}

	public interface IParametersDeclaration<T1, T2, T3, T4, T5, T6> : IParametersDeclaration
	{
	}

	public interface IParametersDeclaration<T1, T2, T3, T4, T5, T6, T7> : IParametersDeclaration
	{
	}

	#endregion
}
