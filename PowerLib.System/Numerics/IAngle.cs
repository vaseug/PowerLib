using System;

namespace PowerLib.System.Numerics
{
	/// <summary>
	/// Represents interface for custom angles which can be transformed to radians.
	/// </summary>
	/// <typeparam name="T">Type of scalar in radians.</typeparam>
	public interface IAngle<T>
	{
		/// <summary>
		/// Return scalar angle value in radians.
		/// </summary>
		/// <returns>Scalar angle value in radians.</returns>
		T ToRadian();
	}
}
