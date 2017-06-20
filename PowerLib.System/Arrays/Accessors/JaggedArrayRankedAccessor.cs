using System;


namespace PowerLib.System.Access
{
	public sealed class JaggedArrayRankedAccessor<T> : ParamsAccessor<int[], T>
	{
		#region Constructors

		public JaggedArrayRankedAccessor(Array array)
			: this(array, false, false, false)
		{
		}

		public JaggedArrayRankedAccessor(Array array, bool asRanges, bool zeroBased)
			: this(array, asRanges, zeroBased, false)
		{
		}

		public JaggedArrayRankedAccessor(Array array, bool asRanges, bool zeroBased, bool readOnly)
			: this(array, new JaggedArrayInfo(array), asRanges, zeroBased, readOnly)
		{
		}

		private JaggedArrayRankedAccessor(Array array, JaggedArrayInfo arrayInfo, bool asRanges, bool zeroBased, bool readOnly)
			: base(array != null ? indices => GetValue(array, arrayInfo, asRanges, zeroBased, indices) : (FuncParams<int[], T>)null,
				array != null && !array.IsReadOnly ? (value, indices) => SetValue(array, arrayInfo, value, asRanges, zeroBased, indices) : (ActionParams<int[], T>)null)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (!typeof(T).IsAssignableFrom(array.GetJaggedArrayElementType()))
				throw new ArgumentException("Invalid array element type");
		}

		#endregion
		#region Methods

		private static T GetValue(Array array, JaggedArrayInfo arrayInfo, bool asRanges, bool zeroBased, int[][] indices)
		{
			return arrayInfo.GetValue<T>(array, asRanges, zeroBased, indices);
		}

		private static void SetValue(Array array, JaggedArrayInfo arrayInfo, T value, bool asRanges, bool zeroBased, int[][] indices)
		{
			arrayInfo.SetValue<T>(array, value, asRanges, zeroBased, indices);
		}

		#endregion
	}
}
