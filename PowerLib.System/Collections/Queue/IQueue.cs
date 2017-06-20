using System;
using System.Collections.Generic;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// Представляет интерфейс коллекции объектов, основанную на принципе "первым поступил — первым покинул".
	/// </summary>
	/// <typeparam name="T">Тип значения элементов коллекции</typeparam>
	public interface IQueue<T> : ICollection<T>
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
		void Enqueue(T value);

		/// <summary>
		/// Извлекает элемент с конца очереди
		/// </summary>
		/// <returns>Значение извлекаемого элемента</returns>
		T Dequeue();
	}
}
