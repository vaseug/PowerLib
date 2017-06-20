using System;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum CollectionRestrictions
	{
		/// <summary>
		/// Ограничение отсутствует
		/// </summary>
		None = 0x0,
		/// <summary>
		/// Ограничение на постоянный размер коллекции
		/// </summary>
		FixedSize = 0x1,
		/// <summary>
		/// Ограничение на изменение структуры существующих элементов коллекции
		/// </summary>
		FixedLayout = 0x2,
		/// <summary>
		/// Ограничение на изменение структуры коллекции вцелом
		/// </summary>
		Fixed = 0x3,
		/// <summary>
		/// Ограничение на изменение значения элемента коллекции
		/// </summary>
		ReadOnlyValue = 0x4,
		/// <summary>
		/// Ограничение на любое изменение коллекции
		/// </summary>
		ReadOnly = 0x7,
	}
}
