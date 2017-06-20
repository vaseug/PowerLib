using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;


namespace PowerLib.System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	public class DisplayStringResourceAttribute : DisplayStringAttribute
	{
		#region Constructors

		public DisplayStringResourceAttribute(Type type, string baseName, string resourceName)
			: this(type, baseName, (Type)null, resourceName, (string)null)
		{
		}

		public DisplayStringResourceAttribute(Type type, string baseName, string resourceName, string cultureName)
			: this(type, baseName, (Type)null, resourceName, cultureName)
		{
		}

		public DisplayStringResourceAttribute(string resourceContainer, bool resourceFile, string baseName, string resourceName)
			: this(resourceContainer, resourceFile, baseName, (Type)null, resourceName, null)
		{
		}

		public DisplayStringResourceAttribute(string resourceContainer, bool resourceFile, string baseName, string resourceName, string cultureName)
			: this(resourceContainer, resourceFile, baseName, (Type)null, resourceName, cultureName)
		{
		}

		public DisplayStringResourceAttribute(Type type, string baseName, Type usingResourceSet, string resourceName)
			: this(type, baseName, usingResourceSet, resourceName, null)
		{
		}

		public DisplayStringResourceAttribute(string resourceContainer, bool resourceFile, string baseName, Type usingResourceSet, string resourceName)
			: this(resourceContainer, resourceFile, baseName, usingResourceSet, resourceName, null)
		{
		}

		public DisplayStringResourceAttribute(Type type, string baseName, Type usingResourceSet, string resourceName, string cultureName)
		{
			DisplayNameValue = AttributeValueHelper.GetStringFromResource(type, baseName, usingResourceSet, resourceName, cultureName);
		}

		public DisplayStringResourceAttribute(string resourceContainer, bool resourceFile, string baseName, Type usingResourceSet, string resourceName, string cultureName)
		{
			DisplayNameValue = (resourceFile) ?
				AttributeValueHelper.GetStringFromFileResource(resourceContainer, baseName, usingResourceSet, resourceName, cultureName) :
				AttributeValueHelper.GetStringFromResource(resourceContainer, baseName, usingResourceSet, resourceName, cultureName);
		}

		#endregion
	}
}
