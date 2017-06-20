using System;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// Представляет интерфейс коллекции объектов, основанную на принципе "первым поступил — первым покинул" с обеих концов.
	/// </summary>
	/// <typeparam name="T">Тип значения элементов коллекции</typeparam>
	public interface IDeque<T> : ICollection<T>
	{
		/// <summary>
		/// Элемент начала очереди
		/// </summary>
		T Front
		{
			get;
		}

		/// <summary>
		/// Элемент конца очереди
		/// </summary>
		T Back
		{
			get;
		}

		/// <summary>
		/// Помещает элемент в начало очереди
		/// </summary>
		/// <param name="value">Значение помещаемого элемента</param>
		void EnqueueFront(T value);

		/// <summary>
		/// Помещает элемент в конец очереди
		/// </summary>
		/// <param name="value">Значение помещаемого элемента</param>
		void EnqueueBack(T value);

		/// <summary>
		/// Извлекает элемент с начала очереди
		/// </summary>
		/// <returns>Значение извлекаемого элемента</returns>
		T DequeueFront();

		/// <summary>
		/// Извлекает элемент с конца очереди
		/// </summary>
		/// <returns>Значение извлекаемого элемента</returns>
		T DequeueBack();
	}
}
