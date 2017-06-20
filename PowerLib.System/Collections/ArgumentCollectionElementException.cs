using System;
using System.Runtime.Serialization;

namespace PowerLib.System.Collections
{
	/// <summary>
	/// ArgumentCollectionElementException exception
	/// </summary>
	public class ArgumentCollectionElementException : ArgumentException
	{
		protected static readonly string DefaultMessage;
		private int _index;

		#region Constructors

		static ArgumentCollectionElementException()
		{
      DefaultMessage = CollectionResources.Default.Strings[CollectionMessage.CollectionElementError];
		}

		public ArgumentCollectionElementException(int index)
			: base(string.Format(DefaultMessage, index))
		{
			_index = index;
		}

		public ArgumentCollectionElementException(string paramName, int index)
			: base(string.Format(DefaultMessage, index), paramName)
		{
			_index = index;
		}

		public ArgumentCollectionElementException(string paramName, string message, int index)
			: base(string.Format(message, index), paramName)
		{
			_index = index;
		}

		public ArgumentCollectionElementException(Exception innerException, int index)
			: base(string.Format(DefaultMessage, index), innerException)
		{
			_index = index;
		}

		public ArgumentCollectionElementException(string paramName, Exception innerException, int index)
			: base(string.Format(DefaultMessage, index), paramName, innerException)
		{
			_index = index;
		}

		public ArgumentCollectionElementException(string paramName, string message, Exception innerException, int index)
			: base(string.Format(message, index), paramName, innerException)
		{
			_index = index;
		}

		protected ArgumentCollectionElementException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
      _index = info.GetInt32("Index");
		}

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("Index", _index);
    }

		#endregion
		#region Properties

		/// <summary>
		/// Index of array element
		/// </summary>
		public int Index
		{
			get
			{
				return _index;
			}
		}

		#endregion
	}
}
