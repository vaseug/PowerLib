using System;


namespace PowerLib.System
{
	/// <summary>
	/// Accessor which use arguments array for value access.
	/// </summary>
	/// <typeparam name="A">Argument type.</typeparam>
	/// <typeparam name="T">Value type.</typeparam>
	public class ParamsAccessor<A, T> : IParamsAccessor<A, T>
	{
		private FuncParams<A, T> _selector = null;
		private ActionParams<A, T> _assigner = null;

		#region Constructors

		public ParamsAccessor(FuncParams<A, T> selector)
		{
			if (selector == null)
				throw new ArgumentNullException("selector");
			//
			_selector = selector;
		}

		public ParamsAccessor(ActionParams<A, T> assigner)
		{
			if (assigner == null)
				throw new ArgumentNullException("assigner");
			//
			_assigner = assigner ;
		}

		public ParamsAccessor(FuncParams<A, T> selector, ActionParams<A, T> assigner)
		{
			if (selector == null && assigner == null)
				throw new ArgumentException("Selector or assigner must specified");
			//
			_selector = selector;
			_assigner = assigner ;
		}

		#endregion
		#region Properties

		public bool CanRead
		{
			get
			{
				return _selector != null;
			}
		}

		public bool CanWrite
		{
			get
			{
				return _assigner != null;
			}
		}

		public T this[params A[] args]
		{
			get
			{
				if (_selector == null)
					throw new InvalidOperationException("Accessor cannot read");
				ValidateArgs(args);
				//
				return _selector(args);
			}
			set
			{
				if (_assigner == null)
					throw new InvalidOperationException("Accessor cannot write");
				ValidateArgs(args);
				//
				_assigner (value, args);
			}
		}

		#endregion
		#region Methods

		/// <summary>
		/// Validatarguments.
		/// </summary>
		/// <param name="args">Arguments array tvalidating.</param>
		protected virtual void ValidateArgs(A[] args)
		{
		}

		#endregion
	}

	public delegate T FuncParams<A, T>(params A[] args);

	public delegate void ActionParams<A, T>(T value, params A[] args);
}
