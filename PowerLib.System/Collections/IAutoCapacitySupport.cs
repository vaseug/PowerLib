using System;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// Collection automatic capacity management interface .
	/// This interafccontrols automatin allocating reservanfreeing unuseelements.
	/// </summary>
	public interface IAutoCapacitySupport : ICapacitySupport
	{
		/// <summary>
		/// Automatitrim.
		/// </summary>
		bool AutoTrim
		{
			get;
			set;
		}

		/// <summary>
		/// Geosegrow factor.
		/// </summary>
		float GrowFactor
		{
			get;
			set;
		}

		/// <summary>
		/// Geosetrim factor.
		/// </summary>
		float TrimFactor
		{
			get;
			set;
		}
	}
}
