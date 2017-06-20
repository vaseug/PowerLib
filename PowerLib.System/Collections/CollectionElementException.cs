using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Resources;
using System.Globalization;
using System.Runtime.Serialization;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// CollectionElementException exception
	/// </summary>
	public class CollectionElementException : Exception
	{
		private static readonly string message;
		private int _index;

		#region Constructors

		static CollectionElementException()
		{
      message = CollectionResources.Default.Strings[CollectionMessage.CollectionElementError];
		}

		public CollectionElementException(int index)
			: base(string.Format(message, index))
		{
			_index = index;
		}

		public CollectionElementException(string message, int index)
			: base(string.Format(message, index))
		{
			_index = index;
		}

		public CollectionElementException(Exception innerException, int index)
			: base(string.Format(message, index), innerException)
		{
			_index = index;
		}

		public CollectionElementException(string message, Exception innerException, int index)
			: base(string.Format(message, index), innerException)
		{
			_index = index;
		}

		protected CollectionElementException(SerializationInfo info, StreamingContext context)
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
