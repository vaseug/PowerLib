using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerLib.System.Linq.Expressions;

namespace PowerLib.System.Linq.Builders
{
	public static class AccessorBuilder
	{
		#region Starting methods

		public static IAccessor<T> Accessing<T>()
		{
			return new AccessorImpl<T>();
		}

		public static IAccessor<T> Accessing<T>(Expression<Func<T>> definer)
		{
			if (definer == null)
				throw new ArgumentNullException("definer");
			if (!typeof(T).IsAnonymous() && (definer.Body.NodeType == ExpressionType.MemberInit || definer.Body.NodeType == ExpressionType.New))
				throw new ArgumentException("Invalid anonymous type definition");

			return new AccessorImpl<T>();
		}

		public static ICreator<T> Creating<T>(Expression<Func<T>> functor)
		{
			if (functor == null)
				throw new ArgumentNullException("functor");

			return new CreatorImpl<T>(functor);
		}

		public static IConverter<T, R> Converting<T, R>(Expression<Func<T, R>> selector)
		{
			if (selector == null)
				throw new ArgumentNullException("selector");

			return new ConverterImpl<T, R>(selector);
		}

		public static ICopier<T, R> Coping<T, R>()
		{
			return new CopierImpl<T, R>();
		}

		#endregion
		#region Internal methods

		private static Expression<D> Lambda<D>(Expression<D> expression)
		{
			return expression;
		}

		private static Expression<Action<T>> GetExpression<T>(Expression<Action<IAccessor<T>>> accessor)
		{
			ParameterExpression prmInstance = Expression.Parameter(typeof(T));
			return Expression.Lambda<Action<T>>(
				Expression.Invoke(Lambda<Func<IAccessor<T>, Expression<Action<T>>>>(a => a.Expression).ReplaceParameters(0, accessor.ReplaceParameters(0, Expression.Constant(new AccessorImpl<T>()))), prmInstance),
				prmInstance);
		}

		private static Expression<Action<T>> AssignCore<T, K>(Expression<Action<T>> expression, Expression<Func<T, K>> selector, Expression<Func<T, K>> param)
		{
			return Expression.Lambda<Action<T>>(
				Expression.Block(
					expression.Body,
					Expression.Assign(selector.ReplaceParameters(0, expression.Parameters[0]), param.ReplaceParameters(0, expression.Parameters[0]))),
				expression.Parameters[0]);
		}

		private static Expression<Action<T>> ReturnCore<T, K>(Expression<Action<T>> expression, Expression<Func<T, K>> selector, Expression<Func<T, K>> param)
		{
			return Expression.Lambda<Action<T>>(
					Expression.Block(
						expression.Body,
						Expression.Assign(param.ReplaceParameters(0, expression.Parameters[0]), selector.ReplaceParameters(0, expression.Parameters[0]))),
					expression.Parameters[0]);
		}

		private static Expression<Action<T>> ApplyCore<T, K>(Expression<Action<T>> expression, Expression<Func<T, K>> selector, Expression<Action<K>> actor)
		{
			return Expression.Lambda<Action<T>>(
				Expression.Block(
					expression.Body,
					actor.ReplaceParameters(0, selector.ReplaceParameters(0, expression.Parameters[0]))),
				expression.Parameters[0]);
		}

		private static Expression<Func<T>> ApplyCore<T>(Expression<Func<T>> expression, Expression<Action<T>> actor)
		{
			ParameterExpression varResult = Expression.Variable(typeof(T));
			return Expression.Lambda<Func<T>>(
				Expression.Block(
					new[] { varResult },
					Expression.Assign(varResult, expression.Body),
					actor.ReplaceParameters(0, varResult),
					varResult));
		}

		private static Expression<Func<T>> CreateCore<T>(Expression<Action<T>> expression, Expression<Func<T>> creator)
		{
			ParameterExpression varResult = Expression.Variable(typeof(T));
			return Expression.Lambda<Func<T>>(
				Expression.Block(
					new[] { varResult },
					Expression.Assign(varResult, creator.Body),
					expression.ReplaceParameters(0, varResult),
					varResult));
		}

		private static Expression<Func<T, R>> SelectCore<T, R>(Expression<Action<T>> expression, Expression<Func<T, R>> selector)
		{
			return Expression.Lambda<Func<T, R>>(
				Expression.Block(
					expression.Body,
					selector.ReplaceParameters(0, expression.Parameters[0])),
					expression.Parameters[0]);
		}

		private static Expression<Func<T, N>> SelectCore<T, R, N>(Expression<Func<T, R>> expression, Expression<Func<R, N>> selector)
		{
			return Expression.Lambda<Func<T, N>>(
				Expression.Block(
					selector.ReplaceParameters(0, expression.Body)),
					expression.Parameters[0]);
		}

		private static Expression<Func<T, R>> ConvertCore<T, R>(Expression<Action<T>> expression, Expression<Func<R>> creator)
		{
			return Expression.Lambda<Func<T, R>>(
				Expression.Block(
					expression.Body,
					creator.Body),
				expression.Parameters[0]);
		}

		private static Expression<Func<T, N>> ConvertCore<T, R, N>(Expression<Func<T, R>> expression, Expression<Func<N>> creator)
		{
			return Expression.Lambda<Func<T, N>>(
				Expression.Block(
					expression.Body,
					creator.Body),
				expression.Parameters[0]);
		}

		private static Expression<Action<T, R>> CopyCore<T, R>(Expression<Action<T>> expression)
		{
			return Expression.Lambda<Action<T, R>>(
				expression.Body,
				expression.Parameters[0], Expression.Parameter(typeof(R)));
		}

		private static Expression<Func<T, R>> ApplySourceCore<T, R>(Expression<Func<T, R>> expression, Expression<Action<T>> actor)
		{
			ParameterExpression varResult = Expression.Variable(typeof(R));
			return Expression.Lambda<Func<T, R>>(
				Expression.Block(
					new[] { varResult },
					Expression.Assign(varResult, expression.Body),
					actor.ReplaceParameters(0, expression.Parameters[0]),
					varResult),
				expression.Parameters[0]);
		}

		private static Expression<Func<T, R>> ApplyTargetCore<T, R>(Expression<Func<T, R>> expression, Expression<Action<R>> actor)
		{
			ParameterExpression varResult = Expression.Variable(typeof(R));
			return Expression.Lambda<Func<T, R>>(
				Expression.Block(
					new[] { varResult },
					Expression.Assign(varResult, expression.Body),
					actor.ReplaceParameters(0, varResult),
					varResult),
				expression.Parameters[0]);
		}

		private static Expression<Action<T, R>> ApplySourceCore<T, R>(Expression<Action<T, R>> expression, Expression<Action<T>> actor)
		{
			return Expression.Lambda<Action<T, R>>(
				Expression.Block(
					expression.Body,
					actor.ReplaceParameters(0, expression.Parameters[0])),
				expression.Parameters[0], expression.Parameters[1]);
		}

		private static Expression<Action<T, R>> ApplyTargetCore<T, R>(Expression<Action<T, R>> expression, Expression<Action<R>> actor)
		{
			return Expression.Lambda<Action<T, R>>(
				Expression.Block(
					expression.Body,
					actor.ReplaceParameters(0, expression.Parameters[1])),
				expression.Parameters[0], expression.Parameters[1]);
		}

		private static Expression<Func<T, R>> CopyCore<T, R, K>(Expression<Func<T, R>> expression, Expression<Func<T, K>> source, Expression<Func<R, K>> target)
		{
			ParameterExpression varResult = Expression.Variable(typeof(R));
			return Expression.Lambda<Func<T, R>>(
				Expression.Block(
					new[] { varResult },
					Expression.Assign(varResult, expression.Body),
					Expression.Assign(target.ReplaceParameters(0, varResult), source.ReplaceParameters(0, expression.Parameters[0]))),
				expression.Parameters[0]);
		}

		private static Expression<Action<T, R>> CopyCore<T, R, K>(Expression<Action<T, R>> expression, Expression<Func<T, K>> source, Expression<Func<R, K>> target)
		{
			return Expression.Lambda<Action<T, R>>(
				Expression.Block(
					expression.Body,
					Expression.Assign(target.ReplaceParameters(0, expression.Parameters[1]), source.ReplaceParameters(0, expression.Parameters[0]))),
				expression.Parameters[0], expression.Parameters[1]);
		}


		#endregion
		#region IAccessor<T> operations

		public static IAccessor<T> Return<T, K>(this IAccessor<T> accessor, Expression<Func<T, K>> selector, Expression<Func<IValue<T>, K>> parameter)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (parameter == null)
				throw new ArgumentNullException("parameter");

			return new AccessorImpl<T>(ReturnCore<T, K>(accessor.Expression, selector, ReflectionBuilder.GetParameter<T, K>(parameter)));
		}

		//public static IAccessor<T> Return<T, K>(this IAccessor<T> accessor, Expression<Action<IReturnableMember<T>>> returner, Expression<Func<IValue<T>, K>> parameter)
		//{
		//	if (accessor == null)
		//		throw new NullReferenceException();
		//	if (returner == null)
		//		throw new ArgumentNullException("returner");
		//	if (parameter == null)
		//		throw new ArgumentNullException("parameter");

		//	return new AccessorImpl<T>(ReturnCore<T, K>(accessor.Expression, ReflectionBuilder.ReturnableMember<T, K>(returner), ReflectionBuilder.GetParameter<T, K>(parameter)));
		//}

		public static IAccessor<T> Return<T, K>(this IAccessor<T> accessor, Expression<Func<IReturnableMember<T>, K>> returner, Expression<Func<IValue<T>, K>> parameter)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (returner == null)
				throw new ArgumentNullException("returner");
			if (parameter == null)
				throw new ArgumentNullException("parameter");

			return new AccessorImpl<T>(ReturnCore<T, K>(accessor.Expression, ReflectionBuilder.ReturnableMember<T, K>(returner), ReflectionBuilder.GetParameter<T, K>(parameter)));
		}

		public static IAccessor<T> Assign<T, K>(this IAccessor<T> accessor, Expression<Func<T, K>> selector, Expression<Func<IValue<T>, K>> parameter)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (parameter == null)
				throw new ArgumentNullException("parameter");

			return new AccessorImpl<T>(AssignCore<T, K>(accessor.Expression, selector, ReflectionBuilder.GetParameter<T, K>(parameter)));
		}

		public static IAccessor<T> Assign<T, K>(this IAccessor<T> accessor, Expression<Action<IAssignableMember<T>>> assigner, Expression<Func<IValue<T>, K>> parameter)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (assigner == null)
				throw new ArgumentNullException("assigner");
			if (parameter == null)
				throw new ArgumentNullException("parameter");

			return new AccessorImpl<T>(AssignCore<T, K>(accessor.Expression, ReflectionBuilder.AssignableMember<T, K>(assigner), ReflectionBuilder.GetParameter<T, K>(parameter)));
		}

		public static IAccessor<T> Assign<T, K>(this IAccessor<T> accessor, Expression<Func<IAssignableMember<T>, K>> assigner, Expression<Func<IValue<T>, K>> parameter)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (assigner == null)
				throw new ArgumentNullException("assigner");
			if (parameter == null)
				throw new ArgumentNullException("parameter");

			return new AccessorImpl<T>(AssignCore<T, K>(accessor.Expression, ReflectionBuilder.AssignableMember<T, K>(assigner), ReflectionBuilder.GetParameter<T, K>(parameter)));
		}

		public static IAccessor<T> Assign<T, K>(this IAccessor<T> accessor, Expression<Func<T, K>> selector, ICreator<K> creator)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (creator == null)
				throw new ArgumentNullException("creator");

			return new AccessorImpl<T>(AssignCore<T, K>(accessor.Expression, selector, Expression.Lambda<Func<T, K>>(creator.Expression.Body, Expression.Parameter(typeof(T)))));
		}

		public static IAccessor<T> Assign<T, K>(this IAccessor<T> accessor, Expression<Action<IAssignableMember<T>>> assigner, ICreator<K> creator)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (assigner == null)
				throw new ArgumentNullException("assigner");
			if (creator == null)
				throw new ArgumentNullException("creator");

			return new AccessorImpl<T>(AssignCore<T, K>(accessor.Expression, ReflectionBuilder.AssignableMember<T, K>(assigner), Expression.Lambda<Func<T, K>>(creator.Expression.Body, Expression.Parameter(typeof(T)))));
		}

		public static IAccessor<T> Assign<T, K>(this IAccessor<T> accessor, Expression<Func<IAssignableMember<T>, K>> assigner, ICreator<K> creator)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (assigner == null)
				throw new ArgumentNullException("assigner");
			if (creator == null)
				throw new ArgumentNullException("creator");

			return new AccessorImpl<T>(AssignCore<T, K>(accessor.Expression, ReflectionBuilder.AssignableMember<T, K>(assigner), Expression.Lambda<Func<T, K>>(creator.Expression.Body, Expression.Parameter(typeof(T)))));
		}

		public static IAccessor<T> Apply<T, K>(this IAccessor<T> accessor, Expression<Func<T, K>> selector, IAccessor<K> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, K>(accessor.Expression, selector, actor.Expression));
		}

		public static IAccessor<T> Apply<T, K>(this IAccessor<T> accessor, Expression<Action<IReturnableMember<T>>> returner, IAccessor<K> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (returner == null)
				throw new ArgumentNullException("returner");
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, K>(accessor.Expression, ReflectionBuilder.ReturnableMember<T, K>(returner), actor.Expression));
		}

		public static IAccessor<T> Apply<T, K>(this IAccessor<T> accessor, Expression<Func<IReturnableMember<T>, K>> returner, IAccessor<K> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (returner == null)
				throw new ArgumentNullException("returner");
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, K>(accessor.Expression, ReflectionBuilder.ReturnableMember<T, K>(returner), actor.Expression));
		}

		public static IAccessor<T> Apply<T, K>(this IAccessor<T> accessor, Expression<Func<T, K>> selector, Expression<Action<K>> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, K>(accessor.Expression, selector, actor));
		}

		public static IAccessor<T> Apply<T, K>(this IAccessor<T> accessor, Expression<Action<IReturnableMember<T>>> returner, Expression<Action<K>> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (returner == null)
				throw new ArgumentNullException("returner");
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, K>(accessor.Expression, ReflectionBuilder.ReturnableMember<T, K>(returner), actor));
		}

		public static IAccessor<T> Apply<T, K>(this IAccessor<T> accessor, Expression<Func<IReturnableMember<T>, K>> returner, Expression<Action<K>> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (returner == null)
				throw new ArgumentNullException("returner");
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, K>(accessor.Expression, ReflectionBuilder.ReturnableMember<T, K>(returner), actor));
		}

		public static IAccessor<T> Apply<T, K>(this IAccessor<T> accessor, Expression<Func<T, K>> selector, Expression<Action<IAccessor<K>>> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, K>(accessor.Expression, selector, GetExpression(actor)));
		}

		public static IAccessor<T> Apply<T, K>(this IAccessor<T> accessor, Expression<Action<IReturnableMember<T>>> returner, Expression<Action<IAccessor<K>>> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (returner == null)
				throw new ArgumentNullException("returner");
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, K>(accessor.Expression, ReflectionBuilder.ReturnableMember<T, K>(returner), GetExpression<K>(actor)));
		}

		public static IAccessor<T> Apply<T, K>(this IAccessor<T> accessor, Expression<Func<IReturnableMember<T>, K>> returner, Expression<Action<IAccessor<K>>> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (returner == null)
				throw new ArgumentNullException("returner");
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, K>(accessor.Expression, ReflectionBuilder.ReturnableMember<T, K>(returner), GetExpression<K>(actor)));
		}

		public static IAccessor<T> Apply<T, K>(this IAccessor<T> accessor, Expression<Func<T, K>> selector, Expression<Action<IApplicableMember<K>>> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, K>(accessor.Expression, selector, ReflectionBuilder.ApplicableMember<K>(actor)));
		}

		public static IAccessor<T> Apply<T, K>(this IAccessor<T> accessor, Expression<Action<IReturnableMember<T>>> returner, Expression<Action<IApplicableMember<K>>> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (returner == null)
				throw new ArgumentNullException("returner");
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, K>(accessor.Expression, ReflectionBuilder.ReturnableMember<T, K>(returner), ReflectionBuilder.ApplicableMember<K>(actor)));
		}

		public static IAccessor<T> Apply<T, K>(this IAccessor<T> accessor, Expression<Func<IReturnableMember<T>, K>> returner, Expression<Action<IApplicableMember<K>>> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (returner == null)
				throw new ArgumentNullException("returner");
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, K>(accessor.Expression, ReflectionBuilder.ReturnableMember<T, K>(returner), ReflectionBuilder.ApplicableMember<K>(actor)));
		}

		public static IAccessor<T> Apply<T>(this IAccessor<T> accessor, Expression<Action<IApplicableMember<T>>> actor)
		{
			if (accessor == null)
				throw new NullReferenceException();
			if (actor == null)
				throw new ArgumentNullException("actor");

			return new AccessorImpl<T>(ApplyCore<T, T>(accessor.Expression, t => t, ReflectionBuilder.ApplicableMember<T>(actor)));
		}

		#endregion
		#region IAccessor<T> transform operations

		public static ICreator<T> Create<T>(this IAccessor<T> accessor, Expression<Func<T>> creator)
		{
			return new CreatorImpl<T>(CreateCore<T>(accessor.Expression, creator));
		}

		public static ICreator<T> Create<T>(this IAccessor<T> accessor, Expression<Action<IConstructibleMember<T>>> constructMember)
		{
			return new CreatorImpl<T>(CreateCore<T>(accessor.Expression, ReflectionBuilder.ConstructibleMember<T>(constructMember)));
		}

		public static ICreator<T> Create<T, S>(this IAccessor<T> accessor, Expression<Func<IReturnableMember<S>, T>> constructMember)
		{
			return null; //new CreatorImpl<T>(CreateCore<T>(accessor.Expression, ReflectionHelper.ReturnableMember<S, T>(constructMember)));
		}

		public static IConverter<T, R> Convert<T, R>(this IAccessor<T> accessor, Expression<Func<T, R>> selector)
		{
			return new ConverterImpl<T, R>(SelectCore(accessor.Expression, selector));
		}

		public static IConverter<T, R> Convert<T, R>(this IAccessor<T> accessor, Expression<Func<IReturnableMember<T>, R>> returnMember)
		{
			return new ConverterImpl<T, R>(SelectCore(accessor.Expression, ReflectionBuilder.ReturnableMember<T, R>(returnMember)));
		}

		public static IConverter<T, R> Convert<T, R>(this IAccessor<T> accessor, Expression<Func<R>> creator)
		{
			return new ConverterImpl<T, R>(ConvertCore(accessor.Expression, creator));
		}

		public static IConverter<T, R> Convert<T, R>(this IAccessor<T> accessor, Expression<Action<IConstructibleMember<R>>> constructMember)
		{
			return new ConverterImpl<T, R>(ConvertCore(accessor.Expression, ReflectionBuilder.ConstructibleMember<R>(constructMember)));
		}

		public static ICopier<T, R> Copy<T, R>(this IAccessor<T> accessor)
		{
			return new CopierImpl<T, R>(
				Expression.Lambda<Action<T, R>>(
					accessor.Expression.Body,
					accessor.Expression.Parameters[0], Expression.Parameter(typeof(R))));
		}

		public static ICopier<T, R> Copy<T, R>(this IAccessor<T> accessor, Expression<Func<R>> definer)
		{
			return new CopierImpl<T, R>(
				Expression.Lambda<Action<T, R>>(
					accessor.Expression.Body,
					accessor.Expression.Parameters[0], Expression.Parameter(typeof(R))));
		}

		#endregion
		#region ICreator<T> operations

		public static ICreator<T> Apply<T>(this ICreator<T> creator, IAccessor<T> accessor)
		{
			if (creator == null)
				throw new NullReferenceException();
			if (accessor == null)
				throw new ArgumentNullException("accessor");

			return new CreatorImpl<T>(ApplyCore(creator.Expression, accessor.Expression));
		}

		public static ICreator<T> Apply<T>(this ICreator<T> creator, Expression<Action<IAccessor<T>>> accessor)
		{
			if (creator == null)
				throw new NullReferenceException();
			if (accessor == null)
				throw new ArgumentNullException("accessor");

			return new CreatorImpl<T>(ApplyCore(creator.Expression, GetExpression(accessor)));
		}

		#endregion
		#region IConverter<T, R> operations

		public static IConverter<T, R> ApplySource<T, R>(this IConverter<T, R> converter, Expression<Action<T>> actor)
		{
			return new ConverterImpl<T, R>(ApplySourceCore<T, R>(converter.Expression, actor));
		}

		public static IConverter<T, R> ApplySource<T, R>(this IConverter<T, R> converter, IAccessor<T> accessor)
		{
			return new ConverterImpl<T, R>(ApplySourceCore<T, R>(converter.Expression, accessor.Expression));
		}

		public static IConverter<T, R> ApplySource<T, R>(this IConverter<T, R> converter, Expression<Action<IAccessor<T>>> accessor)
		{
			return new ConverterImpl<T, R>(ApplySourceCore<T, R>(converter.Expression, GetExpression<T>(accessor)));
		}

		public static IConverter<T, R> ApplyTarget<T, R>(this IConverter<T, R> converter, Expression<Action<R>> actor)
		{
			return new ConverterImpl<T, R>(ApplyTargetCore<T, R>(converter.Expression, actor));
		}

		public static IConverter<T, R> ApplyTarget<T, R>(this IConverter<T, R> converter, IAccessor<R> accessor)
		{
			return new ConverterImpl<T, R>(ApplyTargetCore<T, R>(converter.Expression, accessor.Expression));
		}

		public static IConverter<T, R> ApplyTarget<T, R>(this IConverter<T, R> converter, Expression<Action<IAccessor<R>>> accessor)
		{
			return new ConverterImpl<T, R>(ApplyTargetCore<T, R>(converter.Expression, GetExpression<R>(accessor)));
		}

		public static IConverter<T, R> Copy<T, R, K>(this IConverter<T, R> converter, Expression<Func<T, K>> source, Expression<Func<R, K>> target)
		{
			return new ConverterImpl<T, R>(CopyCore<T, R, K>(converter.Expression, source, target));
		}

		public static IConverter<T, R> Copy<T, R, K>(this IConverter<T, R> converter, Expression<Func<T, K>> source, Expression<Func<IAssignableMember<R>, K>> target)
		{
			return new ConverterImpl<T, R>(CopyCore<T, R, K>(converter.Expression, source, ReflectionBuilder.AssignableMember<R, K>(target)));
		}

		public static IConverter<T, R> Copy<T, R, K>(this IConverter<T, R> converter, Expression<Func<IReturnableMember<T>, K>> source, Expression<Func<R, K>> target)
		{
			return new ConverterImpl<T, R>(CopyCore<T, R, K>(converter.Expression, ReflectionBuilder.ReturnableMember<T, K>(source), target));
		}

		public static IConverter<T, R> Copy<T, R, K>(this IConverter<T, R> converter, Expression<Func<IReturnableMember<T>, K>> source, Expression<Func<IAssignableMember<R>, K>> target)
		{
			return new ConverterImpl<T, R>(CopyCore<T, R, K>(converter.Expression, ReflectionBuilder.ReturnableMember<T, K>(source), ReflectionBuilder.AssignableMember<R, K>(target)));
		}

		public static IConverter<T, N> Convert<T, R, N>(this IConverter<T, R> converter, Expression<Func<R, N>> selector)
		{
			return new ConverterImpl<T, N>(SelectCore(converter.Expression, selector));
		}

		public static IConverter<T, N> Convert<T, R, N>(this IConverter<T, R> converter, Expression<Func<IReturnableMember<R>, N>> returnMember)
		{
			return new ConverterImpl<T, N>(SelectCore(converter.Expression, ReflectionBuilder.ReturnableMember<R, N>(returnMember)));
		}

		public static IConverter<T, N> Convert<T, R, N>(this IConverter<T, R> converter, Expression<Func<N>> creator)
		{
			return new ConverterImpl<T, N>(ConvertCore(converter.Expression, creator));
		}

		public static IConverter<T, N> Convert<T, R, N>(this IConverter<T, R> converter, Expression<Action<IConstructibleMember<N>>> constructMember)
		{
			return new ConverterImpl<T, N>(ConvertCore(converter.Expression, ReflectionBuilder.ConstructibleMember<N>(constructMember)));
		}

		#endregion
		#region ICopier<T, R> operations

		public static ICopier<T, R> ApplySource<T, R>(this ICopier<T, R> copier, IAccessor<T> accessor)
		{
			return new CopierImpl<T, R>(ApplySourceCore<T, R>(copier.Expression, accessor.Expression));
		}

		public static ICopier<T, R> ApplySource<T, R>(this ICopier<T, R> copier, Expression<Action<IAccessor<T>>> accessor)
		{
			return new CopierImpl<T, R>(ApplySourceCore<T, R>(copier.Expression, GetExpression<T>(accessor)));
		}

		public static ICopier<T, R> ApplyTarget<T, R>(this ICopier<T, R> copier, IAccessor<R> accessor)
		{
			return new CopierImpl<T, R>(ApplyTargetCore<T, R>(copier.Expression, accessor.Expression));
		}

		public static ICopier<T, R> ApplyTarget<T, R>(this ICopier<T, R> copier, Expression<Action<IAccessor<R>>> accessor)
		{
			return new CopierImpl<T, R>(ApplyTargetCore<T, R>(copier.Expression, GetExpression<R>(accessor)));
		}

		public static ICopier<T, R> Copy<T, R, K>(this ICopier<T, R> copier, Expression<Func<T, K>> source, Expression<Func<R, K>> target)
		{
			return new CopierImpl<T, R>(CopyCore<T, R, K>(copier.Expression, source, target));
		}

		public static ICopier<T, R> Copy<T, R, K>(this ICopier<T, R> copier, Expression<Func<T, K>> source, Expression<Func<IAssignableMember<R>, K>> target)
		{
			return new CopierImpl<T, R>(CopyCore<T, R, K>(copier.Expression, source, ReflectionBuilder.AssignableMember<R, K>(target)));
		}

		public static ICopier<T, R> Copy<T, R, K>(this ICopier<T, R> copier, Expression<Func<IReturnableMember<T>, K>> source, Expression<Func<R, K>> target)
		{
			return new CopierImpl<T, R>(CopyCore<T, R, K>(copier.Expression, ReflectionBuilder.ReturnableMember<T, K>(source), target));
		}

		public static ICopier<T, R> Copy<T, R, K>(this ICopier<T, R> copier, Expression<Func<IReturnableMember<T>, K>> source, Expression<Func<IAssignableMember<R>, K>> target)
		{
			return new CopierImpl<T, R>(CopyCore<T, R, K>(copier.Expression, ReflectionBuilder.ReturnableMember<T, K>(source), ReflectionBuilder.AssignableMember<R, K>(target)));
		}

		#endregion
		#region Embedded types

		private class AccessorImpl<T> : IAccessor<T>
		{
			private Expression<Action<T>> _expression;

			#region Constructors

			internal AccessorImpl()
				: this(Expression.Lambda<Action<T>>(Expression.Empty(), Expression.Parameter(typeof(T))))
			{
			}

			internal AccessorImpl(Expression<Action<T>> expression)
			{
				if (expression == null)
					throw new ArgumentNullException("expression");

				_expression = expression;
			}

			#endregion
			#region IAccessor<T> interface implementation

			Expression<Action<T>> IAccessor<T>.Expression
			{
				get { return _expression; }
			}

			#endregion
		}

		private class CreatorImpl<T> : ICreator<T>
		{
			private Expression<Func<T>> _expression;

			#region Constructors

			internal CreatorImpl(Expression<Func<T>> expression)
			{
				if (expression == null)
					throw new ArgumentNullException("expression");

				_expression = expression;
			}

			#endregion
			#region ICreator<T> interface implementation

			Expression<Func<T>> ICreator<T>.Expression
			{
				get { return _expression; }
			}

			#endregion
		}

		private class ConverterImpl<T, R> : IConverter<T, R>
		{
			private Expression<Func<T, R>> _expression;

			#region Constructors

			internal ConverterImpl(Expression<Func<R>> expression)
			{
				if (expression == null)
					throw new ArgumentNullException("expression");

				_expression = Expression.Lambda<Func<T, R>>(expression.Body, Expression.Parameter(typeof(T)));
			}

			internal ConverterImpl(Expression<Func<T, R>> expression)
			{
				if (expression == null)
					throw new ArgumentNullException("expression");

				_expression = expression;
			}

			#endregion
			#region IConverter<T, R> interface implementation

			Expression<Func<T, R>> IConverter<T, R>.Expression
			{
				get { return _expression; }
			}

			#endregion
		}

		private class CopierImpl<T, R> : ICopier<T, R>
		{
			private Expression<Action<T, R>> _expression;

			#region Constructors

			internal CopierImpl()
				: this(Expression.Lambda<Action<T, R>>(Expression.Empty(), Expression.Parameter(typeof(T)), Expression.Parameter(typeof(R))))
			{
			}

			internal CopierImpl(Expression<Action<T, R>> expression)
			{
				if (expression == null)
					throw new ArgumentNullException("expression");

				_expression = expression;
			}

			#endregion
			#region ICopier<T, R> interface implementation

			Expression<Action<T, R>> ICopier<T, R>.Expression
			{
				get { return _expression; }
			}

			#endregion
		}

		#endregion
		#region Compile methods

		public static Action<T> Build<T>(this IAccessor<T> accessor)
		{
			if (accessor == null)
				throw new ArgumentNullException("accessor");

			return accessor.Expression.Compile();
		}

		public static Func<T> Build<T>(this IAccessor<T> accessor, Expression<Func<T>> creator)
		{
			if (accessor == null)
				throw new ArgumentNullException("accessor");

			ParameterExpression varResult = Expression.Variable(typeof(T));
			return Expression.Lambda<Func<T>>(
				Expression.Block(
					new[] { varResult },
					Expression.Assign(varResult, creator.Body),
					accessor.Expression.ReplaceParameters(0, varResult),
					varResult)).Compile();
		}

		public static Func<T> Build<T>(this ICreator<T> creator)
		{
			if (creator == null)
				throw new ArgumentNullException("creator");

			return creator.Expression.Compile();
		}

		public static Func<T, R> Build<T, R>(this IConverter<T, R> converter)
		{
			if (converter == null)
				throw new ArgumentNullException("converter");

			return converter.Expression.Compile();
		}

		public static Action<T, R> Build<T, R>(this ICopier<T, R> copier)
		{
			if (copier == null)
				throw new ArgumentNullException("copier");

			return copier.Expression.Compile();
		}

		public static void Evaluate<T>(this IAccessor<T> accessor, T obj)
		{
			Build(accessor)(obj);
		}

		public static T Evaluate<T>(this IAccessor<T> accessor, Func<T> creator)
		{
			return Build(accessor, () => creator())();
		}

		public static T Evaluate<T>(this ICreator<T> creator)
		{
			return Build(creator)();
		}

		public static R Evaluate<T, R>(this IConverter<T, R> converter, T obj)
		{
			return Build(converter)(obj);
		}

		public static void Evaluate<T, R>(this ICopier<T, R> copier, T source, R target)
		{
			Build(copier)(source, target);
		}

		#endregion
	}

	#region Access interfaces

	public interface IAccessor<T>
	{
		Expression<Action<T>> Expression { get; }
	}

	public interface ICreator<T>
	{
		Expression<Func<T>> Expression { get; }
	}

	public interface IConverter<T, R>
	{
		Expression<Func<T, R>> Expression { get; }
	}

	public interface ICopier<T, R>
	{
		Expression<Action<T, R>> Expression { get; }
	}

	#endregion
}
