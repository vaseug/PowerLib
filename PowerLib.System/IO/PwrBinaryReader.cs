using System;
using System.IO;
using System.Text;

namespace PowerLib.System.IO
{
	public class PwrBinaryReader : BinaryReader
	{
		private SizeEncoding _sizeEncoding;
		private Endian _endian;
		private Encoding _encoding;

		#region Constructors

		public PwrBinaryReader(Stream output)
			: this(output, Endian.Default, Encoding.UTF8, false)
		{
		}

		public PwrBinaryReader(Stream output, Encoding encoding)
			: this(output, Endian.Default, SizeEncoding.VB, Encoding.UTF8, false)
		{
		}

		public PwrBinaryReader(Stream output, Encoding encoding, bool leaveOpen)
			: this(output, Endian.Default, SizeEncoding.VB, encoding, leaveOpen)
		{
		}

		public PwrBinaryReader(Stream output, SizeEncoding sizeEncoding, Encoding encoding)
			: this(output, Endian.Default, sizeEncoding, Encoding.UTF8, false)
		{
		}

		public PwrBinaryReader(Stream output, SizeEncoding sizeEncoding, Encoding encoding, bool leaveOpen)
			: this(output, Endian.Default, sizeEncoding, encoding, leaveOpen)
		{
		}

		public PwrBinaryReader(Stream output, Endian endian)
			: this(output, endian, SizeEncoding.VB, Encoding.UTF8, false)
		{
		}

		public PwrBinaryReader(Stream output, Endian endian, Encoding encoding)
			: this(output, endian, SizeEncoding.VB, encoding, false)
		{
		}

		public PwrBinaryReader(Stream output, Endian endian, Encoding encoding, bool leaveOpen)
			: this(output, endian, SizeEncoding.VB, encoding, leaveOpen)
		{
		}

		public PwrBinaryReader(Stream output, Endian endian, SizeEncoding sizeEncoding)
			: this(output, endian, sizeEncoding, Encoding.UTF8, false)
		{
		}

		public PwrBinaryReader(Stream output, Endian endian, SizeEncoding sizeEncoding, Encoding encoding)
			: this(output, endian, sizeEncoding, encoding, false)
		{
		}

		public PwrBinaryReader(Stream output, Endian endian, SizeEncoding sizeEncoding, Encoding encoding, bool leaveOpen)
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
		#region Read integers

		public override Int16 ReadInt16()
		{
			return this.ReadInt16(Endian);
		}

		public override Int32 ReadInt32()
		{
			return this.ReadInt32(Endian);
		}

		public override Int64 ReadInt64()
		{
			return this.ReadInt64(Endian);
		}

		public override UInt16 ReadUInt16()
		{
			return this.ReadUInt16(Endian);
		}

		public override UInt32 ReadUInt32()
		{
			return this.ReadUInt32(Endian);
		}

		public override UInt64 ReadUInt64()
		{
			return this.ReadUInt64(Endian);
		}

    #endregion
    #region Read reals

    public override Single ReadSingle()
    {
      return this.ReadSingle(Endian);
    }

    public override Double ReadDouble()
    {
      return this.ReadDouble(Endian);
    }

    #endregion
    #region Read string

    public override String ReadString()
		{
			return BaseStream.ReadString(Encoding, SizeEncoding, Endian);
		}

		#endregion
		#endregion
	}
}
