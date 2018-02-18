using System;


namespace PowerLib.System
{
	/// <summary>
	/// Accessor which use one argument for value access.
	/// </summary>
	/// <typeparam name="A">Argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public class Accessor<A, T> : IAccessor<A, T>
	{
		private Func<A, T> _selector = null;
		private Action<A, T> _assigner = null;

		#region Constructors

		/// <summary>
		/// Construct accessor which support read access.
		/// </summary>
		/// <param name="selector">Delegate for read value.</param>
		public Accessor(Func<A, T> selector)
			: this(selector, null)
		{
		}

		/// <summary>
		/// Construct accessor which support write access.
		/// </summary>
		/// <param name="assigner">Delegate for write value.</param>
		public Accessor(Action<A, T> assigner)
			: this(null, assigner)
		{
		}

		/// <summary>
		/// Construct accessor which supporboth read and write access.
		/// </summary>
		/// <param name="selector">Delegate for read value.</param>
		/// <param name="assigner">Delegate for write value.</param>
		public Accessor(Func<A, T> selector, Action<A, T> assigner)
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
		/// Accessor indexer which gets or sets value by argument.
		/// </summary>
		/// <param name="a">Argumenvalue.</param>
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		public T this[A a]
		{
			get
			{
				if (_selector == null)
					throw new InvalidOperationException("Accessor cannot read");
				//
				return _selector(a);
			}
			set
			{
				if (_assigner == null)
					throw new InvalidOperationException("Accessor cannot write");
				//
				_assigner (a, value);
			}
		}

		#endregion
	}
}
