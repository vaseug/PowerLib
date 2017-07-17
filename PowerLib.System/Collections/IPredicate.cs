using System;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// Represents interface to test objects on satisfying condition.
	/// </summary>
	/// <typeparam name="T">Type of matched object.</typeparam>
	public interface IPredicate<T>
	{
		/// <summary>
		/// Determines when the<paramref name="value "/> satisfy condition.
		/// </summary>
		/// <param name="value ">Value to match.</param>
		/// <returns>truif <paramref name="value "/> meets the criteria defined within the method represented by this interface ; otherwise, false.</returns>
		bool Match(T value);
	}
}
