using System.Collections.Generic;

namespace PowerLib.System.Collections
{
	/// <summary>
	/// Represents variable size last-in-first-out (LIFO) collection of instances of the same arbitrary type.
	/// </summary>
	/// <typeparam name="T">Element type</typeparam>
	public interface IStack<T> : IReadOnlyCollection<T>
	{
		/// <summary>
		/// Returns the object at the top of the stack without removing it.
		/// </summary>
		T Top
		{
			get;
		}

		/// <summary>
		/// Inserts an object at the top of the stack.
		/// </summary>
		/// <param name="value ">The object to push onto the stack.</param>
		void Push(T value);

		/// <summary>
		/// Removes and returns the object at the top of the stack.
		/// </summary>
		/// <returns>The object removed from the top of the stack.</returns>
		T Pop();
	}
}
