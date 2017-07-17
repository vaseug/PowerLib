using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using PowerLib.System.Collections;

namespace PowerLib.System
{
	public static class TypeExtension
	{
		public static bool IsNullable(this Type type)
		{
			if (type == null)
				throw new NullReferenceException();

			return type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public static bool IsNullAssignable(this Type type)
		{
			if (type == null)
				throw new NullReferenceException();

			return type.IsClass || type.IsNullable();
		}

		public static bool IsValueAssignable(this Type type, object value)
		{
			if (type == null)
				throw new NullReferenceException();

			return value != null && type.IsAssignableFrom(value.GetType()) || (value == null && type.IsNullAssignable());
		}

		public static bool IsValueAssignable<T>(this Type type, T value)
		{
			if (type == null)
				throw new NullReferenceException();

			return value != null && (type.IsAssignableFrom(value.GetType()) || type.IsAssignableFrom(typeof(T))) || (value == null && type.IsNullAssignable());
		}

		public static bool IsOfType(this Type type, string typeFullName, params Type[] typeArgs)
		{
			if (type == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(typeFullName))
				throw new ArgumentException("Fullname of type is not specified", "typeFullName");
			if (typeArgs != null && typeArgs.Length > 0 && !typeFullName.Contains("`"))
				typeFullName = string.Format("{0}`{1}", typeFullName, typeArgs.Length);

			int p = type.FullName.IndexOfAny(new char[] { '[', ',' });
			if (typeFullName == type.FullName.Substring(0, p >= 0 ? p : type.FullName.Length))
				if (typeArgs == null || typeArgs.Length == 0)
					return true;
				else if (type.IsGenericType)
				{
					Type[] types = type.GetGenericArguments();
					if (types.Length != typeArgs.Length)
						return false;
					for (int i = 0; i < types.Length; i++)
						if (types[i] != typeArgs[i])
							return false;
					return true;
				}
			return false;
		}

		public static bool IsSubclassOfType(this Type type, string typeFullName, params Type[] typeArgs)
		{
			if (type == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(typeFullName))
				throw new ArgumentException("Fullname of type is not specified", "typeFullName");
			if (typeArgs != null && typeArgs.Length > 0 && !typeFullName.Contains("`"))
				typeFullName = string.Format("{0}`{1}", typeFullName, typeArgs.Length);

			for (Type t = type; t != null; t = t.BaseType)
			{
				int p = type.FullName.IndexOfAny(new char[] { '[', ',' });
				if (typeFullName == t.FullName.Substring(0, p >= 0 ? p : t.FullName.Length))
					if (typeArgs == null || typeArgs.Length == 0)
						return true;
					else if (type.IsGenericType)
					{
						Type[] types = t.GetGenericArguments();
						if (types.Length != typeArgs.Length)
							return false;
						for (int i = 0; i < types.Length; i++)
							if (types[i] != typeArgs[i])
								return false;
						return true;
					}
			}
			return false;
		}

    public static bool IsAnonymous(this Type type)
    {
      if (type == null)
        throw new NullReferenceException();

      return type.IsGenericType && type.IsClass && type.IsSealed && type.IsNotPublic && Attribute.IsDefined((type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition()), typeof(CompilerGeneratedAttribute), false);
    }

    public static Type MakeNullable(this Type type)
		{
			if (type == null)
				throw new NullReferenceException();
			if (!type.IsValueType)
				throw new ArgumentException("It is not value type", "type");

			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? type : typeof(Nullable<>).MakeGenericType(type);
		}

		public static Func<dynamic> GetActivator(this Type type)
		{
			return GetActivator(type, null, null);
		}

		public static Func<dynamic> GetActivator(this Type type, params object[] args)
		{
			return GetActivator(type,
				args.Select((a, i) =>
				{
					if (a == null)
						throw new ArgumentCollectionElementException(null, "Argument value cannot be null", i);
					return a.GetType();
				}).ToArray(), args);
		}

		public static Func<dynamic> GetActivator(this Type type, Type[] argsTypes, object[] argsValues)
		{
			if (type == null)
				throw new NullReferenceException();
			if (!type.IsClass && !type.IsValueType)
				throw new ArgumentException("Ivalid type", "type");
			if (!(argsTypes == null && argsValues == null || argsTypes != null && argsValues != null && argsTypes.Length == argsValues.Length))
				throw new ArgumentException("Incorrect specifying arguments types and values");
			if (argsTypes != null)
				for (int i = 0; i < argsTypes.Length; i++)
					if (argsTypes[i].IsInstanceOfType(argsValues[i]))
						throw new ArgumentCollectionElementException("argsValues", "Invalid argument value type", i);

			ConstructorInfo ci = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.SingleOrDefault(c =>
				{
					ParameterInfo[] pis = c.GetParameters();
					return pis.Length == (argsTypes != null ? argsTypes.Length : 0) && pis.Where((pi, i) => pi.ParameterType.IsAssignableFrom(argsTypes[i])).Count() == (argsTypes != null ? argsTypes.Length : 0);
				});
			if (ci == null)
				throw new InvalidOperationException(string.Format("Constructor of type '{0}' with specified parameters is not found"));

			return Expression.Lambda(typeof(Func<>).MakeGenericType(type),
				Expression.New(ci, argsValues != null ? argsValues.Select(v => Expression.Constant(v)).ToArray() : new Expression[0])).Compile() as Func<dynamic>;
		}
	}
}
