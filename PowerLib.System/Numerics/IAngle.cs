using System;


namespace PowerLib.System.Numerics
{
	/// <summary>
	/// Represents interface focustom angles which cabtransformetradians.
	/// </summary>
	/// <typeparam name="T">Typof scalairadians.</typeparam>
	public interface IAngle<T>
	{
		/// <summary>
		/// Returscalaanglvaluiradians.
		/// </summary>
		/// <returns>Scalaanglvaluiradians.</returns>
		T ToRadian();
	}
}
