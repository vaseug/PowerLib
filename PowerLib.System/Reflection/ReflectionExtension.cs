using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;


namespace PowerLib.System.Reflection
{
	public static class ReflectionExtension
	{
    #region Instance fields

    public static bool TryGetField(this object source, string name, out object value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", "name");

      FieldInfo fi = source.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      value = fi != null ? fi.GetValue(source) : null;
      return fi != null;
    }

    public static bool TryGetField<TSource>(this TSource source, string name, out object value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", "name");

      FieldInfo fi = typeof(TSource).GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      value = fi != null ? fi.GetValue(source) : null;
      return fi != null;
    }

    public static bool TryGetField<TResult>(this object source, string name, out TResult value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", "name");

      FieldInfo fi = source.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      value = fi != null ? (TResult)fi.GetValue(source) : default(TResult);
      return fi != null;
    }

    public static bool TryGetField<TSource, TResult>(this TSource source, string name, out TResult value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", "name");

      FieldInfo fi = typeof(TSource).GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      value = fi != null ? (TResult)fi.GetValue(source) : default(TResult);
      return fi != null;
    }

    public static bool TrySetField(this object source, string name, object value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", "name");

      FieldInfo fi = source.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (fi != null)
        fi.SetValue(source, value);
      return fi != null;
    }

    public static bool TrySetField<TSource>(this TSource source, string name, object value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", "name");

      FieldInfo fi = typeof(TSource).GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (fi != null)
        fi.SetValue(source, value);
      return fi != null;
    }

    public static bool TrySetField<TResult>(this object source, string name, TResult value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", "name");

      FieldInfo fi = source.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (fi != null)
        fi.SetValue(source, value);
      return fi != null;
    }

    public static bool TrySetField<TSource, TResult>(this TSource source, string name, TResult value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", "name");

      FieldInfo fi = typeof(TSource).GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (fi != null)
        fi.SetValue(source, value);
      return fi != null;
    }

    public static object GetField(this object source, string name)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Field name is not specified", "name");

			Type type = source.GetType();
			FieldInfo fi = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi == null)
				throw new InvalidOperationException(string.Format("Field with name {0} is not found", name));
			return fi.GetValue(source);
		}

		public static object GetField<TSource>(this TSource source, string name)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Field name is not specified", "name");

			FieldInfo fi = typeof(TSource).GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi == null)
				throw new InvalidOperationException(string.Format("Field with name {1} of type {0} is not found", typeof(TSource).FullName, name));
			return fi.GetValue(source);
		}

		public static TResult GetField<TResult>(this object source, string name)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Field name is not specified", "name");

			Type type = source.GetType();
			FieldInfo fi = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi == null)
				throw new InvalidOperationException(string.Format("Field with name {0} is not found", name));
			return (TResult)fi.GetValue(source);
		}

		public static TResult GetField<TSource, TResult>(this TSource source, string name)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Field name is not specified", "name");

			FieldInfo fi = typeof(TSource).GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi == null)
				throw new InvalidOperationException(string.Format("Field with name {1} of type {0} is not found", typeof(TSource).FullName, name));
			return (TResult)fi.GetValue(source);
		}

		public static void SetField(this object source, string name, object value)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Field name is not specified", "name");

			Type type = source.GetType();
			FieldInfo fi = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi == null)
				throw new InvalidOperationException(string.Format("Field with name {0} is not found", name));
			fi.SetValue(source, value);
		}

		public static void SetField<TSource>(this TSource source, string name, object value)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Field name is not specified", "name");

			FieldInfo fi = typeof(TSource).GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi == null)
				throw new InvalidOperationException(string.Format("Field with name {1} of type {0} is not found", typeof(TSource).FullName, name));
			fi.SetValue(source, value);
		}

		public static void SetField<TResult>(this object source, string name, TResult value)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Field name is not specified", "name");

			Type type = source.GetType();
			FieldInfo fi = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi == null)
				throw new InvalidOperationException(string.Format("Field with name {0} is not found", name));
			fi.SetValue(source, value);
		}

		public static void SetField<TSource, TResult>(this TSource source, string name, TResult value)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Field name is not specified", "name");

			FieldInfo fi = typeof(TSource).GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi == null)
				throw new InvalidOperationException(string.Format("Field with name {1} of type {0} is not found", typeof(TSource).FullName, name));
			fi.SetValue(source, value);
		}

    #endregion
    #region Static fields

    public static bool TryGetField(this Type type, string name, out object value)
    {
      if (type == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", "name");

      FieldInfo fi = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      value = fi != null ? fi.GetValue(null) : null;
      return fi != null;
    }

    public static bool TryGetField<TResult>(this Type type, string name, out TResult value)
    {
      if (type == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", "name");

      FieldInfo fi = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      value = fi != null ? (TResult)fi.GetValue(null) : default(TResult);
      return fi != null;
    }

    public static bool TrySetField(this Type type, string name, object value)
    {
      if (type == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", "name");

      FieldInfo fi = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      if (fi != null)
        fi.SetValue(null, value);
      return fi != null;
    }

    public static bool TrySetField<TResult>(this Type type, string name, TResult value)
    {
      if (type == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", "name");

      FieldInfo fi = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      if (fi != null)
        fi.SetValue(null, value);
      return fi != null;
    }

    public static object GetField(this Type type, string name)
		{
			if (type == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Field name is not specified", "name");

			FieldInfo fi = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi == null)
				throw new InvalidOperationException(string.Format("Field with name {1} of type {0} is not found", type.FullName, name));
			return fi.GetValue(null);
		}

		public static TResult GetField<TResult>(this Type type, string name)
		{
			if (type == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Field name is not specified", "name");

			FieldInfo fi = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi == null)
				throw new InvalidOperationException(string.Format("Field with name {1} of type {0} is not found", type.FullName, name));
			return (TResult)fi.GetValue(null);
		}

		public static void SetField(this Type type, string name, object value)
		{
			if (type == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Field name is not specified", "name");

			FieldInfo fi = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi == null)
				throw new InvalidOperationException(string.Format("Field with name {1} of type {0} is not found", type.FullName, name));
			fi.SetValue(null, value);
		}

		public static void SetField<TResult>(this Type type, string name, TResult value)
		{
			if (type == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Field name is not specified", "name");

			FieldInfo fi = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (fi == null)
				throw new InvalidOperationException(string.Format("Field with name {1} of type {0} is not found", type.FullName, name));
      fi.SetValue(null, value);
		}

    #endregion
    #region Instance properties

    public static bool TryGetProperty(this object source, string name, out object value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Property name is not specified", "name");

      PropertyInfo pi = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      value = pi != null ? pi.GetValue(source, null) : null;
      return pi != null;
    }

    public static bool TryGetProperty<TSource>(this TSource source, string name, out object value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Property name is not specified", "name");

      PropertyInfo pi = typeof(TSource).GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      value = pi != null ? pi.GetValue(source, null) : null;
      return pi != null;
    }

    public static bool TryGetProperty<TResult>(this object source, string name, out TResult value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Property name is not specified", "name");

      PropertyInfo pi = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      value = pi != null ? (TResult)pi.GetValue(source, null) : default(TResult);
      return pi != null;
    }

    public static bool TryGetProperty<TSource, TResult>(this TSource source, string name, out TResult value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Property name is not specified", "name");

      PropertyInfo pi = typeof(TSource).GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      value = pi != null ? (TResult)pi.GetValue(source, null) : default(TResult);
      return pi != null;
    }

    public static bool TrySetProperty(this object source, string name, object value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Property name is not specified", "name");

      PropertyInfo pi = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (pi != null)
        pi.SetValue(null, value, null);
      return pi != null;
    }

    public static bool TrySetProperty<TSource>(this TSource source, string name, object value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Property name is not specified", "name");

      PropertyInfo pi = typeof(TSource).GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (pi != null)
        pi.SetValue(null, value, null);
      return pi != null;
    }

    public static bool TrySetProperty<TResult>(this object source, string name, TResult value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Property name is not specified", "name");

      PropertyInfo pi = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (pi != null)
        pi.SetValue(source, value, null);
      return pi != null;
    }

    public static bool TrySetProperty<TSource, TResult>(this TSource source, string name, TResult value)
    {
      if (source == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format("Property name {1} of type {0} is not specified", typeof(TSource).FullName, name), "name");

      PropertyInfo pi = typeof(TSource).GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (pi != null)
        pi.SetValue(source, value, null);
      return pi != null;
    }

    public static object GetProperty(this object source, string name)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Property name is not specified", "name");

			PropertyInfo pi = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new InvalidOperationException("Property is not found");
			return pi.GetValue(source, null);
		}

		public static object GetProperty<TSource>(this TSource source, string name)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Property name is not specified", "name");

			PropertyInfo pi = typeof(TSource).GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new InvalidOperationException("Property is not found");
			return pi.GetValue(source, null);
		}

		public static TResult GetProperty<TResult>(this object source, string name)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Property name is not specified", "name");

			PropertyInfo pi = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new InvalidOperationException("Property is not found");
			return (TResult)pi.GetValue(source, null);
		}

		public static TResult GetProperty<TSource, TResult>(this TSource source, string name)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Property name is not specified", "name");

			PropertyInfo pi = typeof(TSource).GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new InvalidOperationException("Property is not found");
			return (TResult)pi.GetValue(source, null);
		}

		public static void SetProperty(this object source, string name, object value)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Property name is not specified", "name");

			PropertyInfo pi = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new InvalidOperationException("Property is not found");
			pi.SetValue(null, value, null);
		}

		public static void SetProperty<TSource>(this TSource source, string name, object value)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Property name is not specified", "name");

			PropertyInfo pi = typeof(TSource).GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new InvalidOperationException("Property is not found");
			pi.SetValue(null, value, null);
		}

		public static void SetProperty<TResult>(this object source, string name, TResult value)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Property name is not specified", "name");

			PropertyInfo pi = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new InvalidOperationException("Property is not found");
			pi.SetValue(source, value, null);
		}

		public static void SetProperty<TSource, TResult>(this TSource source, string name, TResult value)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException(string.Format("Property name {1} of type {0} is not specified", typeof(TSource).FullName, name), "name");

			PropertyInfo pi = typeof(TSource).GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new InvalidOperationException("Property is not found");
			pi.SetValue(source, value, null);
		}

    #endregion
    #region Static properties

    public static bool TryGetProperty(this Type type, string name, out object value)
    {
      if (type == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Property name is not specified", "name");

      PropertyInfo pi = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      value = pi != null ? pi.GetValue(null, null) : null;
      return pi != null;
    }

    public static bool TryGetProperty<TResult>(this Type type, string name, out TResult value)
    {
      if (type == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Property name is not specified", "name");

      PropertyInfo pi = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      value = pi != null ? (TResult)pi.GetValue(null, null) : default(TResult);
      return pi != null;
    }

    public static bool TrySetProperty(this Type type, string name, object value)
    {
      if (type == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Property name is not specified", "name");

      PropertyInfo pi = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      if (pi != null)
        pi.SetValue(null, value, null);
      return pi != null;
    }

    public static bool TrySetProperty<TResult>(this Type type, string name, TResult value)
    {
      if (type == null)
        throw new NullReferenceException();
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Property name is not specified", "name");

      PropertyInfo pi = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      if (pi != null)
        pi.SetValue(null, value, null);
      return pi != null;
    }

    public static object GetProperty(this Type type, string name)
		{
			if (type == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Property name is not specified", "name");

			PropertyInfo pi = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new InvalidOperationException("Property is not found");
			return pi.GetValue(null, null);
		}

		public static TResult GetProperty<TResult>(this Type type, string name)
		{
			if (type == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Property name is not specified", "name");

			PropertyInfo pi = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new InvalidOperationException("Property is not found");
			return (TResult)pi.GetValue(null, null);
		}

		public static void SetProperty(this Type type, string name, object value)
		{
			if (type == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Property name is not specified", "name");

			PropertyInfo pi = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new InvalidOperationException("Property is not found");
			pi.SetValue(null, value, null);
		}

		public static void SetProperty<TResult>(this Type type, string name, TResult value)
		{
			if (type == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Property name is not specified", "name");

			PropertyInfo pi = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new InvalidOperationException("Property is not found");
			pi.SetValue(null, value, null);
		}

    #endregion
    #region Instance methods

    public static void InvokeMethod<TSource>(this TSource source, string name, params object[] args)
		{
			return;
		}

		public static TResult InvokeMethod<TSource, TResult>(this TSource source, string name, params object[] args)
		{
			if (source == null)
				throw new NullReferenceException();
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Method name is not specified", "name");

			return default(TResult);
		}

		#endregion
		#region Custom attributes

		public static IEnumerable<Attribute> GetCustomAttributes(this ICustomAttributeProvider provider, Type type)
		{
			return GetCustomAttributes(provider, type, false);
		}

		public static IEnumerable<Attribute> GetCustomAttributes(this ICustomAttributeProvider provider, Type type, bool inherit)
		{
			if (provider == null)
				throw new NullReferenceException();

			return provider.GetCustomAttributes(type, inherit).Cast<Attribute>();
		}

		public static Attribute GetCustomAttribute(this ICustomAttributeProvider provider, Type type)
		{
			return GetCustomAttribute(provider, type, false);
		}

		public static Attribute GetCustomAttribute(this ICustomAttributeProvider provider, Type type, bool inherit)
		{
			if (provider == null)
				throw new NullReferenceException();

			return provider.GetCustomAttributes(type, inherit).Cast<Attribute>().FirstOrDefault();
		}

		public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider provider)
			where T : Attribute
		{
			return GetCustomAttributes<T>(provider, false);
		}

		public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider provider, bool inherit)
			where T : Attribute
		{
			if (provider == null)
				throw new NullReferenceException();

			return provider.GetCustomAttributes(typeof(T), inherit).Cast<T>();
		}

		public static T GetCustomAttribute<T>(this ICustomAttributeProvider provider)
      where T : Attribute
    {
			return GetCustomAttribute<T>(provider, false);
		}

		public static T GetCustomAttribute<T>(this ICustomAttributeProvider provider, bool inherit)
      where T : Attribute
		{
			if (provider == null)
				throw new NullReferenceException();

			return provider.GetCustomAttributes(typeof(T), inherit).Cast<T>().FirstOrDefault();
		}

		public static bool IsDefined<T>(this ICustomAttributeProvider provider)
      where T : Attribute
		{
			return IsDefined<T>(provider, false);
		}

		public static bool IsDefined<T>(this ICustomAttributeProvider provider, bool inherit)
      where T : Attribute
    {
			if (provider == null)
				throw new NullReferenceException();

			return provider.IsDefined(typeof(T), inherit);
		}

		#endregion
	}
}
