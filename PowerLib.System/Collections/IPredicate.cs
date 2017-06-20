using System;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// Represents interface ttesobjects osatisfying condition.
	/// </summary>
	/// <typeparam name="T">Typof matcheobject.</typeparam>
	public interface IPredicate<T>
	{
		/// <summary>
		/// Determines whethe<paramref name="value "/> satisfy condition.
		/// </summary>
		/// <param name="value ">Valutmatch.</param>
		/// <returns>truif <paramref name="value "/> meets thcriteridefinewithithmethorepresenteby this interface ; otherwise, false.</returns>
		bool Match(T value);
	}
}
