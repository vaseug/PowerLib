using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerLib.System
{
	public struct TryOut<T>
	{
		private T _value ;
		private bool _success;

		#region Constructor

		public TryOut(bool success, T value)
    {
			_value = value ;
			_success = success;
		}

		#endregion
		#region Properties

		public T Value
		{
			get
			{
				return _value ;
			}
		}

		public bool Success
		{
			get
			{
				return _success;
			}
		}

		#endregion
	}

	public static class TryOut
	{
		public static TryOut<T> Create<T>(bool success, T value)
    {
			return new TryOut<T>(success, value);
		}

    public static TryOut<T> Fail<T>()
    {
      return new TryOut<T>(false, default(T));
    }

		public static TryOut<T> Fail<T>(T value)
		{
			return new TryOut<T>(false, value);
		}

		public static TryOut<T> Success<T>(T value)
		{
			return new TryOut<T>(true, value);
		}
	}
}
 