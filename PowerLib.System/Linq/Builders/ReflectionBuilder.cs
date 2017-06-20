using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerLib.System.Collections;
using PowerLib.System.Linq;
using PowerLib.System.Linq.Expressions;
using PowerLib.System.Resources;

namespace PowerLib.System.Linq.Builders
{
	public static class ReflectionBuilder
	{
		#region Private access methods

		private static Expression Unquote(Expression expression)
		{
			return expression.NodeType == ExpressionType.Quote ? ((UnaryExpression)expression).Operand : expression;
		}

		private static T GetConstant<T>(Expression expression)
		{
			if (typeof(T).IsAssignableFrom(expression.Type))
			{
				PwrStack<Expression> stack = null;
				if (expression.NodeType != ExpressionType.Constant)
				{
					stack = new PwrStack<Expression>();
					while (new[] { ExpressionType.MemberAccess, ExpressionType.Convert, ExpressionType.ConvertChecked }.Contains(expression.NodeType))
					{
						stack.Push(expression);
						switch (expression.NodeType)
						{
							case ExpressionType.Convert:
							case ExpressionType.ConvertChecked:
								expression = ((UnaryExpression)expression).Operand;
								break;
							case ExpressionType.MemberAccess:
								expression = ((MemberExpression)expression).Expression;
								break;
						}
					}
				}
				if (expression.NodeType == ExpressionType.Constant)
				{
					object obj = ((ConstantExpression)expression).Value;
					while (stack != null && stack.Count > 0)
					{
						expression = stack.Pop();
						switch (expression.NodeType)
						{
							case ExpressionType.Convert:
							case ExpressionType.ConvertChecked:
								UnaryExpression unaryExpr = (UnaryExpression)expression;
								obj = unaryExpr.Method.Invoke(obj, new object[0]);
								break;
							case ExpressionType.MemberAccess:
								MemberExpression memberExpr = (MemberExpression)expression;
								switch (memberExpr.Member.MemberType)
								{
									case MemberTypes.Field:
										obj = ((FieldInfo)memberExpr.Member).GetValue(obj);
										break;
									case MemberTypes.Property:
										obj = ((PropertyInfo)memberExpr.Member).GetValue(obj);
										break;
								}
								break;
						}
					}
					return (T)obj;
				}
				if (stack != null && stack.Count > 0)
					stack.Clear();
			}
			throw new ArgumentException("Invalid constant expression", "expression");
		}

		private static Expression<Func<T, K>> MemberAccessor<T, K>(MethodCallExpression expression)
		{
			ParameterExpression parInstance = Expression.Parameter(typeof(T));
			if (expression.Method.Name == "FieldOrProperty" && expression.Method.GetParameters().Length == 1)
			{
				ParameterExpression parMember = Expression.Parameter(typeof(MemberInfo));
				Func<MemberInfo, bool> predicate = Expression.Lambda<Func<MemberInfo, bool>>(Expression.Invoke(expression.Arguments[0], parMember), parMember).Compile();
				MemberInfo mb = typeof(T).GetMembers()
					.Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property && ((PropertyInfo)m).GetIndexParameters().Length == 0)
					.OrderBy(m => m.MemberType == MemberTypes.Field ? 0 : 1)
					.SingleOrDefault(predicate);
				if (mb == null)
					throw new ArgumentException(string.Format("Field or property with specified conditions is not found in type '{0}'", typeof(T).FullName));
				switch (mb.MemberType)
				{
					case MemberTypes.Field:
						FieldInfo fi = (FieldInfo)mb;
						if (expression.Method.DeclaringType == typeof(IAssignableMember<T>) && !fi.FieldType.IsAssignableFrom(typeof(K)) ||
							expression.Method.DeclaringType == typeof(IReturnableMember<T>) && !typeof(K).IsAssignableFrom(fi.FieldType))
							throw new ArgumentException(string.Format("Inconsistent field '{0}' type.", fi.Name));
						return Expression.Lambda<Func<T, K>>(Expression.Field(parInstance, fi), parInstance);
					case MemberTypes.Property:
						PropertyInfo pi = (PropertyInfo)mb;
						if (expression.Method.DeclaringType == typeof(IAssignableMember<T>) && !pi.PropertyType.IsAssignableFrom(typeof(K)) ||
							expression.Method.DeclaringType == typeof(IReturnableMember<T>) && !typeof(K).IsAssignableFrom(pi.PropertyType))
							throw new ArgumentException(string.Format("Inconsistent property '{0}' type.", pi.Name));
						return Expression.Lambda<Func<T, K>>(Expression.Property(parInstance, pi), parInstance);
				}
			}
			else if (expression.Method.Name == "Field" && expression.Method.GetParameters().Length == 1)
			{
				ParameterExpression parMember = Expression.Parameter(typeof(FieldInfo));
				Func<FieldInfo, bool> predicate = Expression.Lambda<Func<FieldInfo, bool>>(Expression.Invoke(expression.Arguments[0], parMember), parMember).Compile();
				FieldInfo fi = typeof(T).GetFields().SingleOrDefault(predicate);
				if (fi == null)
					throw new ArgumentException(string.Format("Field with specified conditions is not found in type '{0}'", typeof(T).FullName));
				if (expression.Method.DeclaringType == typeof(IAssignableMember<T>) && !fi.FieldType.IsAssignableFrom(typeof(K)) ||
					expression.Method.DeclaringType == typeof(IReturnableMember<T>) && !typeof(K).IsAssignableFrom(fi.FieldType))
					throw new ArgumentException(string.Format("Inconsistent field '{0}' type.", fi.Name));
				return Expression.Lambda<Func<T, K>>(Expression.Field(parInstance, fi), parInstance);
			}
			else if (expression.Method.Name == "Property" && expression.Method.GetParameters().Length == 1)
			{
				ParameterExpression parMember = Expression.Parameter(typeof(PropertyInfo));
				Func<PropertyInfo, bool> predicate = Expression.Lambda<Func<PropertyInfo, bool>>(Expression.Invoke(expression.Arguments[0], parMember), parMember).Compile();
				PropertyInfo pi = typeof(T).GetProperties().Where(p => p.GetIndexParameters().Length == 0).SingleOrDefault(predicate);
				if (pi == null)
					throw new ArgumentException(string.Format("Property with specified conditions is not found in type '{0}'", typeof(T).FullName));
				if (expression.Method.DeclaringType == typeof(IAssignableMember<T>) && !pi.PropertyType.IsAssignableFrom(typeof(K)) ||
					expression.Method.DeclaringType == typeof(IReturnableMember<T>) && !typeof(K).IsAssignableFrom(pi.PropertyType))
					throw new ArgumentException(string.Format("Inconsistent property '{0}' type.", pi.Name));
				return Expression.Lambda<Func<T, K>>(Expression.Property(parInstance, pi), parInstance);
			}
			else if (expression.Method.Name == "Property" && expression.Method.GetParameters().Length == 2)
			{
				ParameterExpression parMember = Expression.Parameter(typeof(PropertyInfo));
				Func<PropertyInfo, bool> predicate = Expression.Lambda<Func<PropertyInfo, bool>>(Expression.Invoke(expression.Arguments[0], parMember), parMember).Compile();
				PropertyInfo pi = typeof(T).GetProperties().SingleOrDefault(predicate);
				if (pi == null)
					throw new ArgumentException(string.Format("Property with specified conditions is not found in type '{0}'", typeof(T).FullName));
				if (expression.Method.DeclaringType == typeof(IAssignableMember<T>) && !pi.PropertyType.IsAssignableFrom(typeof(K)) ||
					expression.Method.DeclaringType == typeof(IReturnableMember<T>) && !typeof(K).IsAssignableFrom(pi.PropertyType))
					throw new ArgumentException(string.Format("Inconsistent property '{0}' type.", pi.Name));
				return Expression.Lambda<Func<T, K>>(Expression.Property(parInstance, pi,
					GetParameters<T>(pi.GetIndexParameters(), (Expression<Action<ICall<T>>>)Unquote(expression.Arguments[1])).Select(p => p.ReplaceParameters(0, parInstance)).ToArray()), parInstance);
			}
			else if (new[] { "Method", "InstanceMethod", "StaticMethod" }.Contains(expression.Method.Name) /*== "Method"*/)
			{
				ParameterExpression parMember = Expression.Parameter(typeof(MethodInfo));
				Func<MethodInfo, bool> predicate = Expression.Lambda<Func<MethodInfo, bool>>(Expression.Invoke(expression.Arguments[0], parMember), parMember).Compile();
				MethodInfo mi = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).SingleOrDefault(predicate);
				if (mi == null)
					throw new ArgumentException(string.Format("Method with specified conditions is not found in type '{0}'", typeof(T).FullName));
				if (expression.Method.DeclaringType == typeof(IReturnableMember<T>) && !typeof(K).IsAssignableFrom(mi.ReturnType))
					throw new ArgumentException(string.Format("Inconsistent method '{0}' return type.", mi.Name));
				return Expression.Lambda<Func<T, K>>(Expression.Call(parInstance, mi, expression.Method.GetParameters().Length == 2 ?
					GetParametersCore<T>(mi.GetParameters(), (LambdaExpression)Unquote(expression.Arguments[1])).Select(p => p.ReplaceParameters(0, parInstance)).ToArray() : new Expression[0]), parInstance);
			}
			else if (expression.Method.Name == "Member" && expression.Method.GetParameters().Length == 1)
			{
				ParameterExpression parMember = Expression.Parameter(typeof(MemberInfo));
				Func<MemberInfo, bool> predicate = Expression.Lambda<Func<MemberInfo, bool>>(Expression.Invoke(expression.Arguments[0], parMember), parMember).Compile();
				MemberInfo mb = typeof(T).GetMembers()
					.Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property && ((PropertyInfo)m).GetIndexParameters().Length == 0)
					.OrderBy(m => m.MemberType == MemberTypes.Field ? 0 : 1)
					.First(predicate);
				if (mb == null)
					throw new ArgumentException(string.Format("Member with specified conditions is not found in type '{0}'", typeof(T).FullName));
				switch (mb.MemberType)
				{
					case MemberTypes.Field:
						FieldInfo fi = (FieldInfo)mb;
						if (expression.Method.DeclaringType == typeof(IAssignableMember<T>) && !fi.FieldType.IsAssignableFrom(typeof(K)) ||
							expression.Method.DeclaringType == typeof(IReturnableMember<T>) && !typeof(K).IsAssignableFrom(fi.FieldType))
							throw new ArgumentException(string.Format("Inconsistent specified field type."));
						return Expression.Lambda<Func<T, K>>(Expression.Field(parInstance, fi), parInstance);
					case MemberTypes.Property:
						PropertyInfo pi = (PropertyInfo)mb;
						if (expression.Method.DeclaringType == typeof(IAssignableMember<T>) && !pi.PropertyType.IsAssignableFrom(typeof(K)) ||
							expression.Method.DeclaringType == typeof(IReturnableMember<T>) && !typeof(K).IsAssignableFrom(pi.PropertyType))
							throw new ArgumentException(string.Format("Inconsistent specified property type."));
						return Expression.Lambda<Func<T, K>>(Expression.Property(parInstance, pi), parInstance);
				}
			}
			else if (expression.Method.Name == "Member" && expression.Method.GetParameters().Length == 2)
			{
				ParameterExpression parMember = Expression.Parameter(typeof(MemberInfo));
				Func<MemberInfo, bool> predicate = Expression.Lambda<Func<MemberInfo, bool>>(Expression.Invoke(expression.Arguments[0], parMember), parMember).Compile();
				MemberInfo mb = typeof(T).GetMembers()
					.Where(m => m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Method)
					.OrderBy(m => m.MemberType == MemberTypes.Property ? 0 : 1)
					.First(predicate);
				if (mb == null)
					throw new ArgumentException(string.Format("Member with specified conditions is not found in type '{0}'", typeof(T).FullName));
				switch (mb.MemberType)
				{
					case MemberTypes.Property:
						PropertyInfo pi = (PropertyInfo)mb;
						if (expression.Method.ReflectedType == typeof(IAssignableMember<T>) && !pi.PropertyType.IsAssignableFrom(typeof(K)) ||
							expression.Method.ReflectedType == typeof(IReturnableMember<T>) && !typeof(K).IsAssignableFrom(pi.PropertyType))
							throw new ArgumentException(string.Format("Inconsistent property type."));
						return Expression.Lambda<Func<T, K>>(Expression.Property(parInstance, pi,
							GetParameters<T>(pi.GetIndexParameters(), (Expression<Action<ICall<T>>>)Unquote(expression.Arguments[1])).Select(p => p.ReplaceParameters(0, parInstance)).ToArray()), parInstance);
					case MemberTypes.Method:
						MethodInfo mi = (MethodInfo)mb;
						if (expression.Method.DeclaringType == typeof(IReturnableMember<T>) && !typeof(K).IsAssignableFrom(mi.ReturnType))
							throw new ArgumentException(string.Format("Inconsistent method return type."));
						return Expression.Lambda<Func<T, K>>(Expression.Call(parInstance, mi,
							GetParameters<T>(mi.GetParameters(), (Expression<Action<ICall<T>>>)Unquote(expression.Arguments[1])).Select(p => p.ReplaceParameters(0, parInstance)).ToArray()), parInstance);
				}
			}
			throw new ArgumentException("Invalid member declaration");
		}

		private static Expression<Action<T>> MemberActor<T>(MethodCallExpression expression)
		{
			ParameterExpression parInstance = Expression.Parameter(typeof(T));
			if (expression.Method.Name == "Method")
			{
				ParameterExpression parMember = Expression.Parameter(typeof(MethodInfo));
				Func<MethodInfo, bool> predicate = Expression.Lambda<Func<MethodInfo, bool>>(Expression.Invoke(expression.Arguments[0], parMember), parMember).Compile();
				MethodInfo mi = typeof(T).GetMethods().SingleOrDefault(predicate);
				if (mi == null)
					throw new ArgumentException(string.Format("Method with specified conditions is not found in type '{0}'", typeof(T).FullName));
				return Expression.Lambda<Action<T>>(Expression.Call(parInstance, mi, expression.Method.GetParameters().Length == 2 ?
					GetParameters<T>(mi.GetParameters(), (Expression<Action<ICall<T>>>)Unquote(expression.Arguments[1])).Select(p => p.ReplaceParameters(0, parInstance)).ToArray() : new Expression[0]), parInstance);
			}
			throw new ArgumentException("Invalid member declaration");
		}

		private static Expression<Func<T>> InstanceCreator<T>(MethodCallExpression expression)
		{
			if (expression.Method.Name == "Constructor" && expression.Arguments.Count == 1)
			{
				ParameterExpression parMember = Expression.Parameter(typeof(ConstructorInfo));
				Func<ConstructorInfo, bool> predicate = Expression.Lambda<Func<ConstructorInfo, bool>>(Expression.Invoke(expression.Arguments[0], parMember), parMember).Compile();
				ConstructorInfo ci = typeof(T).GetConstructors().SingleOrDefault(predicate);
				if (ci == null)
					throw new ArgumentException(string.Format("Constructor with specified conditions is not found in type '{0}'", typeof(T).FullName));
				return Expression.Lambda<Func<T>>(Expression.New(ci));
			}
			else if (expression.Method.Name == "Constructor" && expression.Arguments.Count == 2)
			{
				ParameterExpression parMember = Expression.Parameter(typeof(ConstructorInfo));
				Func<ConstructorInfo, bool> predicate = Expression.Lambda<Func<ConstructorInfo, bool>>(Expression.Invoke(expression.Arguments[0], parMember), parMember).Compile();
				ConstructorInfo ci = typeof(T).GetConstructors().SingleOrDefault(predicate);
				if (ci == null)
					throw new ArgumentException(string.Format("Constructor with specified conditions is not found in type '{0}'", typeof(T).FullName));
				return Expression.Lambda<Func<T>>(Expression.New(ci, GetParameters(ci.GetParameters(), (Expression<Action<ICall>>)Unquote(expression.Arguments[1])).Select(e => e.Body)));
			}
			throw new ArgumentException("Invalid member declaration");
		}

		#endregion
		#region Internal access methods

		internal static Expression<Func<T>> GetParameter<T>(Expression<Func<IValue, T>> expression)
		{
			return Expression.Lambda<Func<T>>(
				expression.Body.Visit((MethodCallExpression expr) =>
					expr.Method.DeclaringType == typeof(IValue) && expr.Method.IsGenericMethod && expr.Method.GetGenericArguments().With(args => args.Length == 1 && args[0] == typeof(T)) &&
						new[] { "Val", "Ref", "Out" }.Contains(expr.Method.Name) ? expr.Arguments[0] : expr));
		}

		internal static Expression<Func<T, K>> GetParameter<T, K>(Expression<Func<IValue<T>, K>> expression)
		{
			ParameterExpression parInstance = Expression.Parameter(typeof(T));
			return Expression.Lambda<Func<T, K>>(
				expression.Body.Visit((MethodCallExpression expr) =>
					expr.Method.DeclaringType == typeof(IValue<T>) && expr.Method.IsGenericMethod && expr.Method.GetGenericArguments().With(args => args.Length == 1 && args[0] == typeof(K)) ?
						expr.Method.Name == "Select" ? ((Expression<Func<T, K>>)Unquote(expr.Arguments[0])).ReplaceParameters(0, parInstance) :
						expr.Method.Name == "Return" ? ReturnableMember<T, K>((Expression<Func<IReturnableMember<T>, K>>)Unquote(expr.Arguments[0])).ReplaceParameters(0, parInstance) :
						expr.Method.Name == "Assign" ? AssignableMember<T, K>((Expression<Func<IAssignableMember<T>, K>>)Unquote(expr.Arguments[0])).ReplaceParameters(0, parInstance) :
						expr :
					expr.Method.DeclaringType == typeof(IValue) && expr.Method.IsGenericMethod && expr.Method.GetGenericArguments().With(args => args.Length == 1 && args[0] == typeof(K)) &&
						new[] { "Val", "Ref", "Out" }.Contains(expr.Method.Name) ? expr.Arguments[0] : expr),
				parInstance);
		}

		internal static Expression<Func<dynamic>>[] GetParameters(ParameterInfo[] parameterInfos)
		{
			if (parameterInfos == null)
				throw new ArgumentNullException("parameterInfos");

			return GetParametersCore(parameterInfos, null);
		}

		internal static Expression<Func<dynamic>>[] GetParameters(ParameterInfo[] parameterInfos, Expression<Action<ICall>> expression)
		{
			if (parameterInfos == null)
				throw new ArgumentNullException("parameterInfos");
			if (expression == null)
				throw new ArgumentNullException("expression");

			return GetParametersCore(parameterInfos, expression);
		}

		internal static Expression<Func<dynamic>>[] GetParameters<R>(ParameterInfo[] parameterInfos, Expression<Func<ICall, R>> expression)
		{
			if (parameterInfos == null)
				throw new ArgumentNullException("parameterInfos");
			if (expression == null)
				throw new ArgumentNullException("expression");

			return GetParametersCore(parameterInfos, expression);
		}

		internal static Expression<Func<T, dynamic>>[] GetParameters<T>(ParameterInfo[] parameterInfos, Expression<Action<ICall<T>>> expression)
		{
			if (parameterInfos == null)
				throw new ArgumentNullException("parameterInfos");
			if (expression == null)
				throw new ArgumentNullException("expression");

			return GetParametersCore<T>(parameterInfos, expression);
		}

		internal static Expression<Func<T, dynamic>>[] GetParameters<T, R>(ParameterInfo[] parameterInfos, Expression<Func<ICall<T>, R>> expression)
		{
			if (parameterInfos == null)
				throw new ArgumentNullException("parameterInfos");
			if (expression == null)
				throw new ArgumentNullException("expression");

			return GetParametersCore<T>(parameterInfos, expression);
		}

		private static Expression<Func<dynamic>>[] GetParametersCore(ParameterInfo[] parameterInfos, LambdaExpression expression)
		{
			Expression<Func<dynamic>>[] parameters = new Expression<Func<dynamic>>[parameterInfos.Length];
			if (expression != null)
				expression.Body.Visit((MethodCallExpression expr) =>
				{
					if (expr.Method.DeclaringType == typeof(ICall))
					{
						Func<ParameterInfo, bool> predicate = GetConstant<Func<ParameterInfo, bool>>(expr.Arguments[0]);
						ParameterInfo pi = parameterInfos.SingleOrDefault(predicate);
						if (pi == null)
							throw new InvalidOperationException("Parameter is not found in collection");
						if (pi.Position + expr.Arguments.Count - 1 > parameterInfos.Length)
							throw new ArgumentException("Inconsistent parameters index and length");
						for (int c = 0; c < expr.Arguments.Count - 1; c++)
							parameters[pi.Position + c] = Expression.Lambda<Func<dynamic>>(expr.Arguments[c + 1]);
					}
					return expr;
				});
			for (int i = 0; i < parameters.Length; i++)
				if (parameters[i] == null)
					if (parameterInfos[i].HasDefaultValue)
						parameters[i] = Expression.Lambda<Func<dynamic>>(Expression.Constant(parameterInfos[i].DefaultValue, parameterInfos[i].ParameterType));
					else
						throw new InvalidOperationException(string.Format("Parameter at {0} position is not initialized", i));
			return parameters;
		}

		private static Expression<Func<T, dynamic>>[] GetParametersCore<T>(ParameterInfo[] parameterInfos, LambdaExpression expression)
		{
			Expression<Func<T, dynamic>>[] parameters = new Expression<Func<T, dynamic>>[parameterInfos.Length];
			ParameterExpression parInstance = Expression.Parameter(typeof(T));
			if (expression != null)
				expression.Body.Visit((MethodCallExpression expr) =>
				{
					if (expr.Method.DeclaringType == typeof(ICall))
					{
						Func<ParameterInfo, bool> predicate = GetConstant<Func<ParameterInfo, bool>>(expr.Arguments[0]);
						ParameterInfo pi = parameterInfos.SingleOrDefault(predicate);
						if (pi == null)
							throw new InvalidOperationException("Parameter is not found in collection");
						if (pi.Position + expr.Arguments.Count - 1 > parameterInfos.Length)
							throw new ArgumentException("Inconsistent parameters index and length");
						for (int c = 0; c < expr.Arguments.Count - 1; c++)
							parameters[pi.Position + c] = Expression.Lambda<Func<T, dynamic>>(expr.Arguments[c + 1], parInstance);
					}
					else if (expr.Method.DeclaringType == typeof(ICall<T>))
					{
						Func<ParameterInfo, bool> predicate = GetConstant<Func<ParameterInfo, bool>>(expr.Arguments[0]);
						ParameterInfo pi = parameterInfos.SingleOrDefault(predicate);
						if (pi == null)
							throw new InvalidOperationException("Parameter is not found in collection");
						if (pi.Position >= parameterInfos.Length)
							throw new ArgumentException("Inconsistent parameters index and length");
						parameters[pi.Position] = Expression.Lambda<Func<T, dynamic>>(
							expr.Method.Name == "Select" ? ((LambdaExpression)Unquote(expr.Arguments[1])).ReplaceParameters(0, parInstance) :
							expr.Method.Name == "Return" ? ReturnableMember<T, dynamic>((Expression<Func<IReturnableMember<T>, dynamic>>)Unquote(expr.Arguments[1])).ReplaceParameters(0, parInstance) :
							expr.Method.Name == "Assign" ? AssignableMember<T, dynamic>((Expression<Func<IAssignableMember<T>, dynamic>>)Unquote(expr.Arguments[1])).ReplaceParameters(0, parInstance) :
							expr,
							parInstance);
					}
					return expr;
				});
			for (int i = 0; i < parameters.Length; i++)
				if (parameters[i] == null)
					if (parameterInfos[i].HasDefaultValue)
						parameters[i] = Expression.Lambda<Func<T, dynamic>>(Expression.Constant(parameterInfos[i].DefaultValue, parameterInfos[i].ParameterType), parInstance);
					else
						throw new InvalidOperationException(string.Format("Parameter at {0} position is not initialized", i));
			return parameters;
		}

		internal static Expression<Func<T, K>> AssignableMember<T, K>(Expression<Func<IAssignableMember<T>, K>> expression)
		{
				ParameterExpression parInstance = Expression.Parameter(typeof(T));
				return Expression.Lambda<Func<T, K>>(expression.Body.Visit((MethodCallExpression expr) =>
					expr.Method.DeclaringType == typeof(IAssignableMember<T>) ? MemberAccessor<T, K>(expr).ReplaceParameters(0, parInstance) : expr), parInstance);
		}

		internal static Expression<Func<T, K>> AssignableMember<T, K>(Expression<Action<IAssignableMember<T>>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			if (expression.Body.NodeType == ExpressionType.Call)
				return MemberAccessor<T, K>(expression.Body as MethodCallExpression);

			throw new ArgumentException("Invalid expression");
		}

		internal static Expression<Func<T, K>> ReturnableMember<T, K>(Expression<Func<IReturnableMember<T>, K>> expression)
		{
			ParameterExpression parInstance = Expression.Parameter(typeof(T));
			return Expression.Lambda<Func<T, K>>(expression.Body.Visit((MethodCallExpression expr) =>
				expr.Method.DeclaringType == typeof(IReturnableMember<T>) ? MemberAccessor<T, K>(expr).ReplaceParameters(0, parInstance) : expr), parInstance);
		}

		internal static Expression<Func<T, K>> ReturnableMember<T, K>(Expression<Action<IReturnableMember<T>>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			if (expression.Body.NodeType == ExpressionType.Call)
				return MemberAccessor<T, K>(expression.Body as MethodCallExpression);

			throw new ArgumentException("Invalid expression");
		}

		internal static Expression<Action<T>> ApplicableMember<T>(Expression<Action<IApplicableMember<T>>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			if (expression.Body.NodeType == ExpressionType.Call)
				return MemberActor<T>(expression.Body as MethodCallExpression);

			throw new ArgumentException("Invalid expression");
		}

		internal static Expression<Func<T>> ConstructibleMember<T>(Expression<Action<IConstructibleMember<T>>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			if (expression.Body.NodeType == ExpressionType.Call)
				return InstanceCreator<T>(expression.Body as MethodCallExpression);

			throw new ArgumentException("Invalid expression");
		}

		#endregion
		#region Public access methods

		public static Expression<Func<R>> StaticFieldOrPropertyAccess<T, R>(Func<MemberInfo, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			MemberInfo mb = typeof(T)
				.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(t => t.MemberType == MemberTypes.Field || t.MemberType == MemberTypes.Property)
				.SingleOrDefault(predicate);
			if (mb == null)
				throw new ArgumentException(string.Format("Field or property is not found in type '{0}'", typeof(T).FullName));

			return Expression.Lambda<Func<R>>(Expression.MakeMemberAccess(null, mb));
		}

		public static Expression<Func<R>> StaticFieldAccess<T, R>(Func<FieldInfo, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			FieldInfo fi = typeof(T)
				.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (fi == null)
				throw new ArgumentException(string.Format("Field is not found in type '{0}'", typeof(T).FullName));

			return Expression.Lambda<Func<R>>(Expression.Field(null, fi));
		}

		public static Expression<Func<R>> StaticPropertyAccess<T, R>(Func<PropertyInfo, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			PropertyInfo pi = typeof(T)
				.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (pi == null)
				throw new ArgumentException(string.Format("Property is not found in type '{0}'", typeof(T).FullName));

			return Expression.Lambda<Func<R>>(Expression.Property(null, pi));
		}

		public static Expression<Func<R>> StaticPropertyAccess<T, R>(Func<PropertyInfo, bool> predicate, Expression<Func<IResult, R>> result)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			if (result == null)
				throw new ArgumentNullException("parameters");

			PropertyInfo pi = typeof(T)
				.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (pi == null)
				throw new ArgumentException(string.Format("Property is not found in type '{0}'", typeof(T).FullName));

			return Expression.Lambda<Func<R>>(Expression.Property(null, pi));
		}

		public static Expression<Func<R>> StaticPropertyAccess<T, R>(Func<PropertyInfo, bool> predicate, Expression<Func<ICall, R>> call)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			if (call == null)
				throw new ArgumentNullException("call");
			PropertyInfo pi = typeof(T)
				.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (pi == null)
				throw new ArgumentException(string.Format("Property is not found in type '{0}'", typeof(T).FullName));

			return Expression.Lambda<Func<R>>(Expression.Property(null, pi, GetParameters(pi.GetIndexParameters(), call).Select(e => e.Body)));
		}

		public static Expression<Action> StaticMethodCall<T>(Func<MethodInfo, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			MethodInfo miCall = typeof(T)
				.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (miCall == null)
				throw new ArgumentException(string.Format("Method is not found in type '{0}'", typeof(T).FullName));

			return Expression.Lambda<Action>(Expression.Call(miCall, GetParameters(miCall.GetParameters(), null).Select(e => e.Body)));
		}

		public static Expression<Func<R>> StaticMethodCall<T, R>(Func<MethodInfo, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			MethodInfo miCall = typeof(T)
				.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (miCall == null)
				throw new ArgumentException(string.Format("Method is not found in type '{0}'", typeof(T).FullName));

			return Expression.Lambda<Func<R>>(Expression.Call(miCall, GetParameters(miCall.GetParameters(), null).Select(e => e.Body)));
		}

		public static Expression<Action> StaticMethodCall<T>(Func<MethodInfo, bool> predicate, Expression<Action<ICall>> call)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			if (call == null)
				throw new ArgumentNullException("call");

			MethodInfo miCall = typeof(T)
				.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (miCall == null)
				throw new ArgumentException(string.Format("Method is not found in type '{0}'", typeof(T).FullName));

			return Expression.Lambda<Action>(Expression.Call(miCall, GetParameters(miCall.GetParameters(), call).Select(e => e.Body)));
		}

		public static Expression<Func<R>> StaticMethodCall<T, R>(Func<MethodInfo, bool> predicate, Expression<Func<ICall, R>> call)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			if (call == null)
				throw new ArgumentNullException("call");

			MethodInfo miCall = typeof(T)
				.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (miCall == null)
				throw new ArgumentException(string.Format("Method is not found in type '{0}'", typeof(T).FullName));

			return Expression.Lambda<Func<R>>(Expression.Call(miCall, GetParameters(miCall.GetParameters(), call).Select(e => e.Body)));
		}

		public static Expression<Func<T, R>> InstanceFieldOrPropertyAccess<T, R>(Func<MemberInfo, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			MemberInfo mb = typeof(T)
				.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(t => t.MemberType == MemberTypes.Field || t.MemberType == MemberTypes.Property)
				.SingleOrDefault(predicate);
			if (mb == null)
				throw new ArgumentException(string.Format("Field or property is not found in type '{0}'", typeof(T).FullName));
			ParameterExpression parInstance = Expression.Parameter(typeof(T));

			return Expression.Lambda<Func<T, R>>(Expression.MakeMemberAccess(parInstance, mb), parInstance);
		}

		public static Expression<Func<T, R>> InstanceFieldAccess<T, R>(Func<FieldInfo, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			FieldInfo fi = typeof(T)
				.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (fi == null)
				throw new ArgumentException(string.Format("Field is not found in type '{0}'", typeof(T).FullName));
			ParameterExpression parInstance = Expression.Parameter(typeof(T));

			return Expression.Lambda<Func<T, R>>(Expression.Field(parInstance, fi), parInstance);
		}

		public static Expression<Func<T, R>> InstancePropertyAccess<T, R>(Func<PropertyInfo, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			PropertyInfo pi = typeof(T)
				.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (pi == null)
				throw new ArgumentException(string.Format("Property is not found in type '{0}'", typeof(T).FullName));
			ParameterExpression parInstance = Expression.Parameter(typeof(T));

			return Expression.Lambda<Func<T, R>>(Expression.Property(parInstance, pi), parInstance);
		}

		public static Expression<Func<T, R>> InstancePropertyAccess<T, R>(Func<PropertyInfo, bool> predicate, Expression<Func<IResult, R>> result)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			if (result == null)
				throw new ArgumentNullException("result");

			PropertyInfo pi = typeof(T)
				.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (pi == null)
				throw new ArgumentException(string.Format("Property is not found in type '{0}'", typeof(T).FullName));
			ParameterExpression parInstance = Expression.Parameter(typeof(T));

			return Expression.Lambda<Func<T, R>>(Expression.Property(parInstance, pi), parInstance);
		}

		public static Expression<Func<T, R>> InstancePropertyAccess<T, R>(Func<PropertyInfo, bool> predicate, Expression<Func<ICall<T>, R>> parameters)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			if (parameters == null)
				throw new ArgumentNullException("parameters");

			PropertyInfo pi = typeof(T)
				.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (pi == null)
				throw new ArgumentException(string.Format("Property is not found in type '{0}'", typeof(T).FullName));
			ParameterExpression parInstance = Expression.Parameter(typeof(T));

			return Expression.Lambda<Func<T, R>>(Expression.Property(parInstance, pi, GetParameters<T, R>(pi.GetIndexParameters(), parameters).Select(p => p.ReplaceParameters(0, parInstance)).ToArray()), parInstance);
		}
/*
		public static Expression<Action<T>> InstanceMethodCall<T>(Func<MethodInfo, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			MethodInfo miCall = typeof(T)
				.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (miCall == null)
				throw new ArgumentException(string.Format("Method is not found in type '{0}'", typeof(T).FullName));
			ParameterExpression parInstance = Expression.Parameter(typeof(T));

			return Expression.Lambda<Action<T>>(Expression.Call(parInstance, miCall, GetParameters<T>(miCall.GetParameters(), null).Select(p => p.ReplaceParameters(0, parInstance))), parInstance);
		}

		public static Expression<Func<T, R>> InstanceMethodCall<T, R>(Func<MethodInfo, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			MethodInfo miCall = typeof(T)
				.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (miCall == null)
				throw new ArgumentException(string.Format("Method is not found in type '{0}'", typeof(T).FullName));
			ParameterExpression parInstance = Expression.Parameter(typeof(T));

			return Expression.Lambda<Func<T, R>>(Expression.Call(parInstance, miCall, GetParameters<T>(miCall.GetParameters(), null).Select(p => p.ReplaceParameters(0, parInstance))), parInstance);
		}
*/
		public static Expression<Action<T>> InstanceMethodCall<T>(Func<MethodInfo, bool> predicate, Expression<Action<ICall<T>>> call)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			if (call == null)
				throw new ArgumentNullException("parameters");
			MethodInfo miCall = typeof(T)
				.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (miCall == null)
				throw new ArgumentException(string.Format("Method is not found in type '{0}'", typeof(T).FullName));
			ParameterExpression parInstance = Expression.Parameter(typeof(T));

			return Expression.Lambda<Action<T>>(Expression.Call(parInstance, miCall, GetParameters<T>(miCall.GetParameters(), call).Select(p => p.ReplaceParameters(0, parInstance))), parInstance);
		}

		public static Expression<Func<T, R>> InstanceMethodCall<T, R>(Func<MethodInfo, bool> predicate, Expression<Func<ICall<T>, R>> call)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			if (call == null)
				throw new ArgumentNullException("parameters");

			MethodInfo miCall = typeof(T)
				.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (miCall == null)
				throw new ArgumentException(string.Format("Method is not found in type '{0}'", typeof(T).FullName));
			ParameterExpression parInstance = Expression.Parameter(typeof(T));

			return Expression.Lambda<Func<T, R>>(Expression.Call(parInstance, miCall, GetParameters<T, R>(miCall.GetParameters(), call).Select(p => p.ReplaceParameters(0, parInstance))), parInstance);
		}

		public static Expression<Func<T>> Construct<T>(Func<ConstructorInfo, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			ConstructorInfo ci = typeof(T)
				.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(t => t.GetParameters().Length == 0);
			if (ci == null)
				throw new ArgumentException(string.Format("Constructor is not found in type '{0}'", typeof(T).FullName));

			return Expression.Lambda<Func<T>>(Expression.New(ci));
		}

		public static Expression<Func<T>> Construct<T>(Func<ConstructorInfo, bool> predicate, Expression<Action<ICall>> call)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			if (call == null)
				throw new ArgumentNullException("parameters");

			ConstructorInfo ci = typeof(T)
				.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.SingleOrDefault(predicate);
			if (ci == null)
				throw new ArgumentException(string.Format("Constructor is not found in type '{0}'", typeof(T).FullName));

			return Expression.Lambda<Func<T>>(Expression.New(ci, GetParameters(ci.GetParameters(), call).Select(e => e.Body)));
		}

		#endregion
	}

	#region Intarfaces

	public interface IValue
	{
		T Val<T>(T result);

		T Ref<T>(ref T result);

		T Out<T>(out T result);
	}

	public interface IValue<T> : IValue
	{
		R Select<R>(Expression<Func<T, R>> selector);

		R Return<R>(Expression<Func<IReturnableMember<T>, R>> returner);

		R Assign<R>(Expression<Func<IAssignableMember<T>, R>> assigner);
	}

	public interface IResult
	{
		R Return<R>();

		R Return<R>(Expression<Func<R>> result);
	}

	public interface ICall : IResult
	{
		ICall ByVal<T>(Func<ParameterInfo, bool> parameter, T value);

		ICall ByRef<T>(Func<ParameterInfo, bool> parameter, ref T value);

		ICall ByOut<T>(Func<ParameterInfo, bool> parameter, out T value);

		ICall Range(Func<ParameterInfo, bool> parameter, params dynamic[] values);

		void NoReturn();
	}

	public interface ICall<T> : ICall
	{
		ICall<T> Select<R>(Func<ParameterInfo, bool> predicate, Expression<Func<T, R>> selector);

		ICall<T> Return<R>(Func<ParameterInfo, bool> predicate, Expression<Func<IReturnableMember<T>, R>> returner);

		ICall<T> Assign<R>(Func<ParameterInfo, bool> predicate, Expression<Func<IAssignableMember<T>, R>> assigner);
	}

	public interface IMember<T>
	{
		void Member(Func<MemberInfo, bool> predicate);

		void MemberP(Func<MemberInfo, bool> predicate, Expression<Action<ICall>> call);

		void MemberPW(Func<MemberInfo, bool> predicate, Expression<Action<ICall<T>>> call);

		R Member<R>(Func<MemberInfo, bool> predicate);

		R Member<R>(Func<MemberInfo, bool> predicate, Expression<Func<IResult, R>> result);

		R MemberP<R>(Func<MemberInfo, bool> predicate, Expression<Func<ICall, R>> call);

		R MemberPW<R>(Func<MemberInfo, bool> predicate, Expression<Func<ICall<T>, R>> call);
	}

	public interface IAssignableMember<T> : IMember<T>
	{
		void FieldOrProperty(Func<MemberInfo, bool> predicate);

		void Field(Func<FieldInfo, bool> predicate);

		void Property(Func<PropertyInfo, bool> predicate);

		R FieldOrProperty<R>(Func<MemberInfo, bool> predicate);

		R FieldOrProperty<R>(Func<MemberInfo, bool> predicate, Expression<Func<IResult, R>> result);

		R Field<R>(Func<FieldInfo, bool> predicate);

		R Field<R>(Func<FieldInfo, bool> predicate, Expression<Func<IResult, R>> result);

		R Property<R>(Func<PropertyInfo, bool> predicate);

		R Property<R>(Func<PropertyInfo, bool> predicate, Expression<Func<IResult, R>> result);

		R PropertyP<R>(Func<PropertyInfo, bool> predicate, Expression<Func<ICall, R>> parameters);

		R PropertyPW<R>(Func<PropertyInfo, bool> predicate, Expression<Func<ICall<T>, R>> parameters);
	}

	public interface IApplicableMember<T> : IMember<T>
	{
		void Method(Func<MethodInfo, bool> predicate);

		void MethodP(Func<MethodInfo, bool> predicate, Expression<Action<ICall>> call);

		void MethodPW(Func<MethodInfo, bool> predicate, Expression<Action<ICall<T>>> call);
	}

	public interface IReturnableMember<T> : IMember<T>
	{
		void FieldOrProperty(Func<MemberInfo, bool> predicate);

		void Field(Func<FieldInfo, bool> predicate);

		void Property(Func<PropertyInfo, bool> predicate);

		void Method(Func<MethodInfo, bool> predicate);

		void MethodP(Func<MethodInfo, bool> predicate, Expression<Action<ICall>> call);

		void MethodPW(Func<MethodInfo, bool> predicate, Expression<Action<ICall<T>>> call);

		R FieldOrProperty<R>(Func<MemberInfo, bool> predicate);

		R FieldOrProperty<R>(Func<MemberInfo, bool> predicate, Expression<Func<IResult, R>> result);

		R Field<R>(Func<FieldInfo, bool> predicate);

		R Field<R>(Func<FieldInfo, bool> predicate, Expression<Func<IResult, R>> result);

		R Property<R>(Func<PropertyInfo, bool> predicate);

		R Property<R>(Func<PropertyInfo, bool> predicate, Expression<Func<IResult, R>> result);

		R PropertyP<R>(Func<PropertyInfo, bool> predicate, Expression<Func<ICall, R>> call);

		R PropertyPW<R>(Func<PropertyInfo, bool> predicate, Expression<Func<ICall<T>, R>> call);

		R Method<R>(Func<MethodInfo, bool> predicate);

		R Method<R>(Func<MethodInfo, bool> predicate, Expression<Func<IResult, R>> result);

		R MethodP<R>(Func<MethodInfo, bool> predicate, Expression<Func<ICall, R>> call);

		R MethodPW<R>(Func<MethodInfo, bool> predicate, Expression<Func<ICall<T>, R>> call);
	}

	public interface IConstructibleMember<T> : IMember<T>
	{
		void Constructor(Func<ConstructorInfo, bool> predicate);

		void Constructor(Func<ConstructorInfo, bool> predicate, Expression<Action<ICall>> parameters);
	}

	#endregion
}
