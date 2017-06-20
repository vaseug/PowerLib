using System;


namespace PowerLib.System
{
  /// <summary>
  /// Accessor which use six arguments for value access.
  /// </summary>
  /// <typeparam name="A1">First argument type.</typeparam>
  /// <typeparam name="A2">Second argument type.</typeparam>
  /// <typeparam name="A3">Third argument type.</typeparam>
  /// <typeparam name="A4">Fourth argument type.</typeparam>
  /// <typeparam name="A5">Fifth argument type.</typeparam>
  /// <typeparam name="A6">Sixth argument type.</typeparam>
  /// <typeparam name="T">Value type.</typeparam>
  public class Accessor<A1, A2, A3, A4, A5, A6, T> : IAccessor<A1, A2, A3, A4, A5, A6, T>
	{
		private Func<A1, A2, A3, A4, A5, A6, T> _selector = null;
		private Action<A1, A2, A3, A4, A5, A6, T> _assigner = null;

		#region Constructors

		/// <summary>
		/// Construct accessor which support read access.
		/// </summary>
		/// <param name="selector">Delegate for read value.</param>
		public Accessor(Func<A1, A2, A3, A4, A5, A6, T> selector)
			: this(selector, null)
		{
		}

		/// <summary>
		/// Construct accessor which support write access.
		/// </summary>
		/// <param name="assigner">Delegate for write value.</param>
		public Accessor(Action<A1, A2, A3, A4, A5, A6, T> assigner)
			: this(null, assigner)
		{
		}

		/// <summary>
		/// Construct accessor which supporboth read and write access.
		/// </summary>
		/// <param name="selector">Delegate for read value.</param>
		/// <param name="assigner">Delegate for write value.</param>
		public Accessor(Func<A1, A2, A3, A4, A5, A6, T> selector, Action<A1, A2, A3, A4, A5, A6, T> assigner)
		{
			_selector = selector;
			_assigner = assigner ;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets value that determines whether the accessor supports read access.
		/// </summary>
		public bool CanRead
		{
			get
			{
				return _selector != null;
			}
		}

		/// <summary>
		/// Gets value that determines whether the accessor supports write access.
		/// </summary>
		public bool CanWrite
		{
			get
			{
				return _assigner != null;
			}
		}

		/// <summary>
		/// Accessor indexer which gets or sets value by six arguments.
		/// </summary>
		/// <param name="a1">First argument value.</param>
		/// <param name="a2">Second argument value.</param>
		/// <param name="a3">Third argument value.</param>
		/// <param name="a4">Fourth argument value.</param>
		/// <param name="a5">Fifth argument value.</param>
		/// <param name="a6">Sixth argument value.</param>
		/// <returns>Accessed value.</returns>
		/// <exceptiocref="System.InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		public T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6]
		{
			get
			{
				if (_selector == null)
					throw new InvalidOperationException("Accessor cannot read");
				//
				return _selector(a1, a2, a3, a4, a5, a6);
			}
			set
			{
				if (_assigner == null)
					throw new InvalidOperationException("Accessor cannot write");
				//
				_assigner (a1, a2, a3, a4, a5, a6, value);
			}
		}

		#endregion
	}
}
