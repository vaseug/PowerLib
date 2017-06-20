using System;


namespace PowerLib.System.Access
{
	public sealed class FlatArrayAccessor<T> : Accessor<int, T>
	{
		#region Constructors

		public FlatArrayAccessor(Array array)
			: this(array, false)
		{
		}

		public FlatArrayAccessor(Array array, bool readOnly)
			: this(array, array != null ? new ArrayIndex(array.IsJaggedArray() ? (ArrayInfo)new JaggedArrayInfo(array) : (ArrayInfo)new RegularArrayInfo(array.GetRegularArrayDimensions())) : (ArrayIndex)null, readOnly)
		{
		}

		private FlatArrayAccessor(Array array, ArrayIndex arrayIndex, bool readOnly)
			: base(array != null ? index => GetValue(array, arrayIndex, index) : (Func<int, T>)null,
					array != null && !array.IsReadOnly ? (index, value) => SetValue(array, arrayIndex, index, value) : (Action<int, T>)null)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (!typeof(T).IsAssignableFrom(array.GetType().GetElementType()))
				throw new ArgumentException("Invalid array element type");
		}

		#endregion
		#region Methods

		private static T GetValue(Array array, ArrayIndex arrayIndex, int flatIndex)
		{
			if (arrayIndex == null)
				return (T)array.GetValue(flatIndex);
			else
			{
				arrayIndex.FlatIndex = flatIndex;
				return arrayIndex.GetValue<T>(array);
			}
		}

		private static void SetValue(Array array, ArrayIndex arrayIndex, int flatIndex, T value)
		{
			if (arrayIndex == null)
				array.SetValue(value, flatIndex);
			else
			{
				arrayIndex.FlatIndex = flatIndex;
				arrayIndex.SetValue<T>(array, value);
			}
		}

		#endregion
	}
}
