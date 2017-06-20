using System;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// Collection capacity interface.
	/// This interface supports inforamtion about capacity and load factor, and allow trim excess memory.
	/// </summary>
	public interface ICapacitySupport
	{
		/// <summary>
		/// Gets or sets the number of elements that collection can contain before resizing is required.
		/// </summary>
		int Capacity
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the current load factor - loaded to capacity ratio.
		/// </summary>
		float LoadFactor
		{
			get;
		}

		/// <summary>
		/// Sets the capacity to the actual number of elements in the collection.
		/// </summary>
		void TrimExcess();
	}
}
