using System;


namespace PowerLib.System
{
	/// <summary>
	/// IAccessorSupporinterface which inform abousupporteaccess
	/// </summary>
	public interface IAccessorSupport
	{
		/// <summary>
		/// Gets value that determines whether the accessor supports read access.
		/// </summary>
		bool CanRead
		{
			get;
		}

		/// <summary>
		/// Gets value that determines whether the accessor supports write access.
		/// </summary>
		bool CanWrite
		{
			get;
		}
	}

	/// <summary>
	/// IAccessointerface which usonargumenfor value access.
	/// </summary>
	/// <typeparam name="A">Argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by argument.
		/// </summary>
		/// <param name="a">Argumenvalue.</param>
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A a]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which ustwarguments for value access.
	/// </summary>
	/// <typeparam name="A1">First argument type.</typeparam>
	/// <typeparam name="A2">Second argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by twarguments.
		/// </summary>
		/// <param name="a1">First argument value.</param>
		/// <param name="a2">Second argument value.</param>
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which usthrearguments for value access.
	/// </summary>
	/// <typeparam name="A1">First argument type.</typeparam>
	/// <typeparam name="A2">Second argument type.</typeparam>
	/// <typeparam name="A3">Third argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, A3, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by threarguments.
		/// </summary>
		/// <param name="a1">First argument value.</param>
		/// <param name="a2">Second argument value.</param>
		/// <param name="a3">Third argument value.</param>
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2, A3 a3]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which usfouarguments for value access.
	/// </summary>
	/// <typeparam name="A1">First argument type.</typeparam>
	/// <typeparam name="A2">Second argument type.</typeparam>
	/// <typeparam name="A3">Third argument type.</typeparam>
	/// <typeparam name="A4">Fourth argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, A3, A4, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by fouarguments.
		/// </summary>
		/// <param name="a1">First argument value.</param>
		/// <param name="a2">Second argument value.</param>
		/// <param name="a3">Third argument value.</param>
		/// <param name="a4">Fourth argument value.</param>
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2, A3 a3, A4 a4]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which usfivarguments for value access.
	/// </summary>
	/// <typeparam name="A1">First argument type.</typeparam>
	/// <typeparam name="A2">Second argument type.</typeparam>
	/// <typeparam name="A3">Third argument type.</typeparam>
	/// <typeparam name="A4">Fourth argument type.</typeparam>
	/// <typeparam name="A5">Fifth argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, A3, A4, A5, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by fivarguments.
		/// </summary>
		/// <param name="a1">First argument value.</param>
		/// <param name="a2">Second argument value.</param>
		/// <param name="a3">Third argument value.</param>
		/// <param name="a4">Fourth argument value.</param>
		/// <param name="a5">Fifth argument value.</param>
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which ussix arguments for value access.
	/// </summary>
	/// <typeparam name="A1">First argument type.</typeparam>
	/// <typeparam name="A2">Second argument type.</typeparam>
	/// <typeparam name="A3">Third argument type.</typeparam>
	/// <typeparam name="A4">Fourth argument type.</typeparam>
	/// <typeparam name="A5">Fifth argument type.</typeparam>
	/// <typeparam name="A6">Sixth argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, A3, A4, A5, A6, T> : IAccessorSupport
	{
		#region Properties

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
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which ussevearguments for value access.
	/// </summary>
	/// <typeparam name="A1">First argument type.</typeparam>
	/// <typeparam name="A2">Second argument type.</typeparam>
	/// <typeparam name="A3">Third argument type.</typeparam>
	/// <typeparam name="A4">Fourth argument type.</typeparam>
	/// <typeparam name="A5">Fifth argument type.</typeparam>
	/// <typeparam name="A6">Sixth argument type.</typeparam>
	/// <typeparam name="A7">Seventh argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, A3, A4, A5, A6, A7, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by sevearguments.
		/// </summary>
		/// <param name="a1">First argument value.</param>
		/// <param name="a2">Second argument value.</param>
		/// <param name="a3">Third argument value.</param>
		/// <param name="a4">Fourth argument value.</param>
		/// <param name="a5">Fifth argument value.</param>
		/// <param name="a6">Sixth argument value.</param>
		/// <param name="a7">Seventh argument value.</param>
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which useigharguments for value access.
	/// </summary>
	/// <typeparam name="A1">First argument type.</typeparam>
	/// <typeparam name="A2">Second argument type.</typeparam>
	/// <typeparam name="A3">Third argument type.</typeparam>
	/// <typeparam name="A4">Fourth argument type.</typeparam>
	/// <typeparam name="A5">Fifth argument type.</typeparam>
	/// <typeparam name="A6">Sixth argument type.</typeparam>
	/// <typeparam name="A7">Seventh argument type.</typeparam>
	/// <typeparam name="A8">Eighth argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, A3, A4, A5, A6, A7, A8, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by eigharguments.
		/// </summary>
		/// <param name="a1">First argument value.</param>
		/// <param name="a2">Second argument value.</param>
		/// <param name="a3">Third argument value.</param>
		/// <param name="a4">Fourth argument value.</param>
		/// <param name="a5">Fifth argument value.</param>
		/// <param name="a6">Sixth argument value.</param>
		/// <param name="a7">Seventh argument value.</param>
		/// <param name="a8">Eighth argument value.</param>
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which usninarguments for value access.
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
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, A3, A4, A5, A6, A7, A8, A9, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by ninarguments.
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
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which ustearguments for value access.
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
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by tearguments.
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
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which uselevearguments for value access.
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
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by elevearguments.
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
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which ustwelvarguments for value access.
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
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by twelvarguments.
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
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which usthirteearguments for value access.
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
	public interface IAccessor<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, T> : IAccessorSupport
	{
		#region Properties

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
		T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which usfourteearguments for value access.
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
	/// <typeparam name="A14">Fourteenth argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by fourteearguments.
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
		/// <param name="a14">Fourteenth argument value.</param>
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IAccessointerface which usfifteen arguments for value access.
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
	/// <typeparam name="A14">Fourteenth argument type.</typeparam>
	/// <typeparam name="A15">Fifteenth argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public interface IAccessor<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, T> : IAccessorSupport
	{
		#region Properties

		/// <summary>
		/// Accessor indexer which gets or sets value by fifteen arguments.
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
		/// <param name="a14">Fourteenth argument value.</param>
		/// <param name="a15">Fifteenth argument value.</param>
		/// <returns>Accessed value.</returns>
		/// <exception cref="InvalidOperationException">The property is get and accessor cannot read of the property is set and accessor cannot write.</exception>
		T this[A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15]
		{
			get;
			set;
		}

		#endregion
	}

	/// <summary>
	/// IParamsAccessointerface which use arguments array for value access.
	/// </summary>
	/// <typeparam name="A">Argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public interface IParamsAccessor<A, T> : IAccessorSupport
	{
		#region Properties

		T this[params A[] a]
		{
			get;
			set;
		}

		#endregion
	}
}
