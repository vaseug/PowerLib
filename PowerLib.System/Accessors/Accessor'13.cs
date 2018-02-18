using System;


namespace PowerLib.System
{
	/// <summary>
	/// Accessor which use thirteen arguments for value access.
	/// </summary>
	/// <typeparam name="A1">First argument type.</typeparam>
	/// <typeparam name="A2">Second argument type.</typeparam>
	/// <typeparam name="A3">Third argument type.</typeparam>
	/// <typeparam name="A4">Fourth argument type.</typeparam>
	/// <typeparam name="A5">Fifth argument type.</typeparam>
	/// <typeparam name="A6">Sixth argument type.</typeparam>
	/// <typeparam name="A7">Seventh argument type.</typeparam>
	/// <typeparam name="A8">Eighth argument type.</typeparam>
	/// <typeparam name="A9">Nine argument type.</typeparam>
	/// <typeparam name="A10">Tenth argument type.</typeparam>
	/// <typeparam name="A11">Eleventh argument type.</typeparam>
	/// <typeparam name="A12">Twelfth argument type.</typeparam>
	/// <typeparam name="A13">Thirteenth argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public class Accessor<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, T> : IAccessor<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, T>
	{
		private Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, T> _selector = null;
		private Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, T> _assigner = null;

		#region Constructors

		/// <summary>
		/// Construct accessor which support read access.
		/// </summary>
		/// <param name="selector">Delegate for read value.</param>
		public Accessor(Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, T> selector)
			: this(selector, null)
		{
		}

		/// <summary>
		/// Construct accessor which support write access.
		/// </summary>
		/// <param name="assigner">Delegate for write value.</param>
		public Accessor(Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, T> assigner)
			: this(null, assigner)
		{
		}

		/// <summary>
		/// Construct accessor which supporboth read and write access.
		/// </summary>
		/// <param name="selector">Delegate for read value.</param>
		/// <param name="assigner">Delegate for write value.</param>
		public Accessor(Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, T> selector, Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, T> assigner)
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
		/// Accessor indexer which gets or sets value by thirteearguments.
		/// </summary>
		/// <param name="a1">First argument value.</param>
		/// <param name="a2">Second argument value.</param>
		/// <param name="a3">Third argument value.</param>
		/// <param name="a4">Fourth argument value.</param>
		/// <param name="a5">Fifth argument value.</param>
		/// <param name="a6">Sixth argument value.</param>
		/// <param name="a7">Seventh argument value.</param>
		/// <param name="a8">Eighth argument value.</param>
		/// <param name="a9">Ninth argument value.</param>
		/// <param name="a10">Tenth argument value.</param>
		/// <param name="a11">Eleventh argument value.</param>
		/// <param name="a12">Twelfth argument value.</param>
		/// <param name="a13">Thirteenth argument value.</param>
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		public T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13]
		{
			get
			{
				if (_selector == null)
					throw new InvalidOperationException("Accessor cannot read");
				//
				return _selector(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
			}
			set
			{
				if (_assigner == null)
					throw new InvalidOperationException("Accessor cannot write");
				//
				_assigner (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, value);
			}
		}

		#endregion
	}
}
