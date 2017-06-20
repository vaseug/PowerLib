using System;
using System.ComponentModel;


namespace PowerLib.System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	public class DisplayStringAttribute : DisplayNameAttribute
	{
		#region Constructors

		protected DisplayStringAttribute()
			: base()
		{
		}

		public DisplayStringAttribute(string displayString)
			: base(displayString)
		{
		}

		#endregion
	}
}
