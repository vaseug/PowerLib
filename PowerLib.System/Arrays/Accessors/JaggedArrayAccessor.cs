using System;


namespace PowerLib.System.Access
{
	public sealed class JaggedArrayAccessor<T> : ParamsAccessor<int, T>
	{
		#region Constructors

		public JaggedArrayAccessor(Array array)
			: this(array, false, false, null, false)
		{
		}

		public JaggedArrayAccessor(Array array, bool asRanges, bool zeroBased)
			: this(array, asRanges, zeroBased, null, false)
		{
		}

		public JaggedArrayAccessor(Array array, bool asRanges, bool zeroBased, bool readOnly)
			: this(array, asRanges, zeroBased, null, readOnly)
		{
		}

		public JaggedArrayAccessor(Array array, bool asRanges, bool zeroBased, int[] transpose)
			: this(array, asRanges, zeroBased, transpose, false)
		{
		}

		public JaggedArrayAccessor(Array array, bool asRanges, bool zeroBased, int[] transpose, bool readOnly)
			: this(array, array != null ? new JaggedArrayInfo(array) : null, asRanges, zeroBased, transpose, readOnly)
		{
		}

		private JaggedArrayAccessor(Array array, JaggedArrayInfo arrayInfo, bool asRanges, bool zeroBased, int[] transpose, bool readOnly)
			: base(array != null ? indices => GetValue(array, arrayInfo, asRanges, zeroBased, transpose, indices) : (FuncParams<int, T>)null,
				array != null && !array.IsReadOnly ? (value, indices) => SetValue(array, arrayInfo, value, asRanges, zeroBased, transpose, indices) : (ActionParams<int, T>)null)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (!typeof(T).IsAssignableFrom(array.GetJaggedArrayElementType()))
				throw new ArgumentException("Invalid array element type");
			//
			if (transpose != null)
			{
				bool[] occurs = new bool[transpose.Length];
				for (int i = 0; i < transpose.Length; i++)
					if (transpose[i] < 0 && transpose[i] >= transpose.Length)
						throw new ArgumentRegularArrayElementException("transpose", "Transpose index is out of range", i);
					else if (occurs[transpose[i]])
						throw new ArgumentRegularArrayElementException("transpose", "Duplicate transpose index", i);
					else
						occurs[transpose[i]] = true;
			}
		}

		#endregion
		#region Methods

		private static int[] Transpose(int[] args, int[] transpose)
		{
			int[] targets = new int[args.Length];
			for (int i = 0; i < args.Length; i++)
				targets[transpose[i]] = args[i];
			return targets;
		}

		private static T GetValue(Array array, JaggedArrayInfo arrayInfo, bool asRanges, bool zeroBased, int[] transpose, int[] indices)
		{
			return arrayInfo.GetValue<T>(array, asRanges, zeroBased, transpose != null ? Transpose(indices, transpose) : indices);
		}

		private static void SetValue(Array array, JaggedArrayInfo arrayInfo, T value, bool asRanges, bool zeroBased, int[] transpose, int[] indices)
		{
			arrayInfo.SetValue<T>(array, value, asRanges, zeroBased, transpose != null ? Transpose(indices, transpose) : indices);
		}

		#endregion
	}
}
