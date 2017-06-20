using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace PowerLib.System
{
	/// <summary>
	/// ArgumentRegularArrayLongElementException exception
	/// </summary>
	public class ArgumentRegularArrayLongElementException : ArgumentException
	{
		private static readonly string DefaultMessage;
		private long[] _indices;
		private IReadOnlyList<long> _indicesAccessor;

		#region Constructors

		static ArgumentRegularArrayLongElementException()
		{
			DefaultMessage = ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElement];
		}

		public ArgumentRegularArrayLongElementException(params long[] indices)
			: base(string.Format(DefaultMessage, PwrArray.FormatAsLongRegularIndices(indices)))
		{
			_indices = (long[])indices.Clone();
		}

		public ArgumentRegularArrayLongElementException(long index)
			: base(string.Format(DefaultMessage, PwrArray.FormatAsLongRegularIndices(new long[] { index })))
		{
			_indices = new long	[] { index };
		}

		public ArgumentRegularArrayLongElementException(string paramName, params long[] indices)
			: base(string.Format(DefaultMessage, PwrArray.FormatAsLongRegularIndices(indices)), paramName)
		{
			_indices = (long[])indices.Clone();
		}

		public ArgumentRegularArrayLongElementException(string paramName, int index)
			: base(string.Format(DefaultMessage, PwrArray.FormatAsLongRegularIndices(new long[] { index })), paramName)
		{
			_indices = new long[] { index };
		}

		public ArgumentRegularArrayLongElementException(string paramName, Exception innerException, params long[] indices)
			: base(string.Format(DefaultMessage, PwrArray.FormatAsLongRegularIndices(indices)), paramName, innerException)
		{
			_indices = (long[])indices.Clone();
		}

		public ArgumentRegularArrayLongElementException(string paramName, Exception innerException, long index)
			: base(string.Format(DefaultMessage, PwrArray.FormatAsLongRegularIndices(new long[] { index })), paramName, innerException)
		{
			_indices = new long[] { index };
		}

		public ArgumentRegularArrayLongElementException(string paramName, string message, params long[] indices)
			: base(string.Format(message, PwrArray.FormatAsLongRegularIndices(indices)), paramName)
		{
			_indices = (long[])indices.Clone();
		}

		public ArgumentRegularArrayLongElementException(string paramName, string message, long index)
			: base(string.Format(message, PwrArray.FormatAsLongRegularIndices(new long[] { index })), paramName)
		{
			_indices = new long[] { index };
		}

		public ArgumentRegularArrayLongElementException(string paramName, string message, Exception innerException, params long[] indices)
			: base(string.Format(message, PwrArray.FormatAsLongRegularIndices(indices)), paramName, innerException)
		{
			_indices = (long[])indices.Clone();
		}

		public ArgumentRegularArrayLongElementException(string paramName, string message, Exception innerException, long index)
			: base(string.Format(message, PwrArray.FormatAsLongRegularIndices(new long[] { index })), paramName, innerException)
		{
			_indices = new long[] { index };
		}

		protected ArgumentRegularArrayLongElementException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			_indices = (long[])info.GetValue("Indices", typeof(long[]));
		}

		#endregion
		#region Properties

		/// <summary>
		/// Array elemenindices
		/// </summary>
		public IReadOnlyList<long> Indices
		{
			get
			{
				if (_indicesAccessor == null)
					_indicesAccessor = new ReadOnlyCollection<long>(_indices);
				return _indicesAccessor;
			}
		}

		#endregion
		#region Methods

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			base.GetObjectData(info, context);
			info.AddValue("Indices", _indices);
		}

		#endregion
	}
}
