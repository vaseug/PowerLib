using System;
using System.Text;
using System.IO;

namespace PowerLib.System.IO
{
	public class PwrBinaryWriter : BinaryWriter
	{
		private SizeEncoding _sizeEncoding;
		private Endian _endian;
		private Encoding _encoding;

		#region Constructors

		protected PwrBinaryWriter()
			: this(Stream.Null, Endian.Default, SizeEncoding.VB, Encoding.UTF8, false)
		{
		}

		public PwrBinaryWriter(Stream output)
			: this(output, Endian.Default, Encoding.UTF8, false)
		{
		}

		public PwrBinaryWriter(Stream output, Encoding encoding)
			: this(output, Endian.Default, SizeEncoding.VB, Encoding.UTF8, false)
		{
		}

		public PwrBinaryWriter(Stream output, Encoding encoding, bool leaveOpen)
			: this(output, Endian.Default, SizeEncoding.VB, encoding, leaveOpen)
		{
		}

		public PwrBinaryWriter(Stream output, SizeEncoding sizeEncoding, Encoding encoding)
			: this(output, Endian.Default, sizeEncoding, Encoding.UTF8, false)
		{
		}

		public PwrBinaryWriter(Stream output, SizeEncoding sizeEncoding, Encoding encoding, bool leaveOpen)
			: this(output, Endian.Default, sizeEncoding, encoding, leaveOpen)
		{
		}

		public PwrBinaryWriter(Stream output, Endian endian)
			: this(output, endian, SizeEncoding.VB, Encoding.UTF8, false)
		{
		}

		public PwrBinaryWriter(Stream output, Endian endian, Encoding encoding)
			: this(output, endian, SizeEncoding.VB, encoding, false)
		{
		}

		public PwrBinaryWriter(Stream output, Endian endian, Encoding encoding, bool leaveOpen)
			: this(output, endian, SizeEncoding.VB, encoding, leaveOpen)
		{
		}

		public PwrBinaryWriter(Stream output, Endian endian, SizeEncoding sizeEncoding)
			: this(output, endian, sizeEncoding, Encoding.UTF8, false)
		{
		}

		public PwrBinaryWriter(Stream output, Endian endian, SizeEncoding sizeEncoding, Encoding encoding)
			: this(output, endian, sizeEncoding, encoding, false)
		{
		}

		public PwrBinaryWriter(Stream output, Endian endian, SizeEncoding sizeEncoding, Encoding encoding, bool leaveOpen)
			: base(output, encoding, leaveOpen)
		{
			_endian = endian;
			_sizeEncoding = sizeEncoding;
			_encoding = encoding;
		}

		#endregion
		#region Properties

		internal protected Endian Endian
		{
			get { return _endian; }
		}

		internal protected SizeEncoding SizeEncoding
		{
			get { return _sizeEncoding; }
		}

		internal protected Encoding Encoding
		{
			get { return _encoding; }
		}

		#endregion
		#region Methods
		#region Write size

		protected void WriteSize(int value)
		{
			BaseStream.WriteSize(value, SizeEncoding, Endian);
		}

		protected void WriteSize(long value)
		{
			BaseStream.WriteLongSize(value, SizeEncoding, Endian);
		}

		#endregion
		#region Write integers

		public override void Write(Int16 value)
		{
			BaseStream.WriteInt16(value, Endian);
		}

		public override void Write(Int32 value)
		{
			BaseStream.WriteInt32(value, Endian);
		}

		public override void Write(Int64 value)
		{
			BaseStream.WriteInt64(value, Endian);
		}

		public override void Write(UInt16 value)
		{
			BaseStream.WriteUInt16(value, Endian);
		}

		public override void Write(UInt32 value)
		{
			BaseStream.WriteUInt32(value, Endian);
		}

		public override void Write(UInt64 value)
		{
			BaseStream.WriteUInt64(value, Endian);
		}

    #endregion
    #region Write reals

    public override void Write(Single value)
    {
      BaseStream.WriteSingle(value, Endian);
    }

    public override void Write(Double value)
    {
      BaseStream.WriteDouble(value, Endian);
    }

    #endregion
    #region Write string

    public override void Write(String value)
		{
			if (value == null)
				throw new ArgumentNullException("value ");

			byte[] buffer = Encoding.GetBytes(value);
			WriteSize(buffer.Length);
			base.Write(buffer);
		}

		#endregion
		#endregion
	}
}
