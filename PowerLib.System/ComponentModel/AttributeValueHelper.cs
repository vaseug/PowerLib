using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace PowerLib.System.ComponentModel
{
	public static class AttributeValueHelper
	{
		public static object GetObjectFromString(Type type, string str)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			if (!converter.CanConvertFrom(typeof(string)))
				throw new InvalidOperationException(string.Format("Cannot convert string to type: {0}", type.FullName));
			return converter.ConvertFromString(str);
		}

		public static object GetObjectByConversion(Type type, object obj)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			//
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			if (converter == null || obj != null && !converter.CanConvertFrom(obj.GetType()))
				throw new InvalidOperationException((obj != null) ?
					string.Format("Cannot convert object from type: {0} to type: {1}", obj.GetType().FullName, type.FullName) :
					string.Format("Cannot convert null object to type: {0}", type.FullName));
			try
			{
				return converter.ConvertFrom(obj);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException((obj != null) ?
					string.Format("Error on conversion object from type: {0} to type: {1}", obj.GetType().FullName, type.FullName) :
					string.Format("Error on conversion null object to type: {0}", type.FullName), ex);
			}
		}

		public static object GetObjectByActivation(Type type, object[] args)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			//
			try
			{
				return Activator.CreateInstance(type, args);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(string.Format("Cannot activate object of type: {0}", type.FullName), ex);
			}
		}

		public static object GetObjectByDeserialization(Type type, byte[] bytes)
		{
			object value;
			using (MemoryStream stream = new MemoryStream(bytes, false))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				try
				{
					value = formatter.Deserialize(stream);
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException(string.Format("Cannot deserialize bytes to type: {0}", type.FullName), ex);
				}
				if (!type.IsInstanceOfType(value))
					throw new InvalidOperationException(string.Format("Cannot deserialize bytes to type: {0}", type.FullName));
			}
			return value;
		}

		public static object GetObjectFromResource(string assemblyString, string baseName, Type usingResourceSet, string resourceName, string cultureName)
		{
			if (string.IsNullOrEmpty(baseName))
				throw new ArgumentException("String is not specified", "baseName");
			if (string.IsNullOrEmpty(resourceName))
				throw new ArgumentException("String is not specified", "resourceName");
			if (string.IsNullOrEmpty(assemblyString))
				throw new ArgumentException("String is not specified", "assemblyString");
			//
			ResourceManager rm = new ResourceManager(baseName, Assembly.Load(assemblyString), usingResourceSet);
			try
			{
				return rm.GetObject(resourceName, (cultureName != null) ? CultureInfo.GetCultureInfo(cultureName) : null);
			}
			finally
			{
				rm.ReleaseAllResources();
			}
		}

		public static object GetObjectFromResource(Type type, string baseName, Type usingResourceSet, string resourceName, string cultureName)
		{
			if (string.IsNullOrEmpty(baseName))
				throw new ArgumentException("String is not specified", "baseName");
			if (string.IsNullOrEmpty(resourceName))
				throw new ArgumentException("String is not specified", "assemblyString");
			if (type == null)
				throw new ArgumentNullException("type");
			//
			ResourceManager rm = new ResourceManager(baseName, type.Assembly, usingResourceSet);
			try
			{
				return rm.GetObject(resourceName, (cultureName != null) ? CultureInfo.GetCultureInfo(cultureName) : null);
			}
			finally
			{
				rm.ReleaseAllResources();
			}
		}

		public static object GetObjectFromFileResource(string resourceDir, string baseName, Type usingResourceSet, string resourceName, string cultureName)
		{
			if (string.IsNullOrEmpty(baseName))
				throw new ArgumentException("String is not specified", "baseName");
			if (string.IsNullOrEmpty(resourceName))
				throw new ArgumentException("String is not specified", "resourceName");
			if (string.IsNullOrEmpty(resourceDir))
				throw new ArgumentException("String is not specified", "resourceDir");
			//
			ResourceManager rm = ResourceManager.CreateFileBasedResourceManager(baseName, resourceDir, usingResourceSet);
			try
			{
				return rm.GetObject(resourceName, (cultureName != null) ? CultureInfo.GetCultureInfo(cultureName) : null);
			}
			finally
			{
				rm.ReleaseAllResources();
			}
		}

		public static string GetStringFromResource(string assemblyString, string baseName, Type usingResourceSet, string resourceName, string cultureName)
		{
			if (string.IsNullOrEmpty(baseName))
				throw new ArgumentException("String is not specified", "baseName");
			if (string.IsNullOrEmpty(resourceName))
				throw new ArgumentException("String is not specified", "resourceName");
			if (string.IsNullOrEmpty(assemblyString))
				throw new ArgumentException("String is not specified", "assemblyString");
			//
			ResourceManager rm = new ResourceManager(baseName, Assembly.Load(assemblyString), usingResourceSet);
			try
			{
			return rm.GetString(resourceName, (cultureName != null) ? CultureInfo.GetCultureInfo(cultureName) : null);
			}
			finally
			{
				rm.ReleaseAllResources();
			}
		}

		public static string GetStringFromResource(Type type, string baseName, Type usingResourceSet, string resourceName, string cultureName)
		{
			if (string.IsNullOrEmpty(baseName))
				throw new ArgumentException("String is not specified", "baseName");
			if (string.IsNullOrEmpty(resourceName))
				throw new ArgumentException("String is not specified", "assemblyString");
			if (type == null)
				throw new ArgumentNullException("type");
			//
			ResourceManager rm = new ResourceManager(baseName, type.Assembly, usingResourceSet);
			try
			{
				return rm.GetString(resourceName, (cultureName != null) ? CultureInfo.GetCultureInfo(cultureName) : null);
			}
			finally
			{
				rm.ReleaseAllResources();
			}
		}

		public static string GetStringFromFileResource(string resourceDir, string baseName, Type usingResourceSet, string resourceName, string cultureName)
		{
			if (string.IsNullOrEmpty(baseName))
				throw new ArgumentException("String is not specified", "baseName");
			if (string.IsNullOrEmpty(resourceName))
				throw new ArgumentException("String is not specified", "resourceName");
			if (string.IsNullOrEmpty(resourceDir))
				throw new ArgumentException("String is not specified", "resourceDir");
			//
			ResourceManager rm = ResourceManager.CreateFileBasedResourceManager(baseName, resourceDir, usingResourceSet);
			try
			{
				return rm.GetString(resourceName, (cultureName != null) ? CultureInfo.GetCultureInfo(cultureName) : null);
			}
			finally
			{
				rm.ReleaseAllResources();
			}
		}
	}
}
