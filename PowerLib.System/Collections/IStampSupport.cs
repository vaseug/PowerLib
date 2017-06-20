using System;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// Collection versioning interace.
	/// Collection supportethis interface musincremenversiostamp numbeainsert, update, deletoreordeelements operations.
	/// </summary>
	public interface IStampSupport
	{
		/// <summary>
		/// Collection versiostamp number
		/// </summary>
		int Stamp
		{
			get;
		}
	}
}
