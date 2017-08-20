using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using System.Xml;
using Microsoft.SqlServer.Server;
using PowerLib.System.Linq;
using PowerLib.System.IO;
using PowerLib.System.Text;
using PowerLib.System.Data.SqlTypes.IO;
using PowerLib.SqlServer;

namespace PowerLib.System.Data.SqlTypes.FileSystem
{
  [SqlUserDefinedType(Format.UserDefined, Name = "FileInfo", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlFileInfo : INullable, IBinarySerialize
  {
    private static readonly SqlFileInfo @null = new SqlFileInfo();

    private FileInfo _fi;

    #region Constructor

    public SqlFileInfo()
    {
      _fi = null;
    }

    public SqlFileInfo(string path)
    {
      _fi = new FileInfo(path);
    }

    public SqlFileInfo(FileInfo fi)
    {
      _fi = fi;
    }

    #endregion
    #region Properties

    public FileInfo FileInfo
    {
      get { return _fi; }
    }

    public static SqlFileInfo Null
    {
      get { return @null; }
    }

    public bool IsNull
    {
      get { return _fi == null; }
    }

    #region File properties

    public SqlInt32 Attributes
    {
      get { return !IsNull ? (Int32)_fi.Attributes : SqlInt32.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _fi.Attributes = (FileAttributes)value.Value;
      }
    }

    public SqlDateTime CreationTime
    {
      get { return !IsNull ? _fi.CreationTime : SqlDateTime.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _fi.CreationTime = value.Value;
      }
    }

    public SqlDateTime CreationTimeUtc
    {
      get { return !IsNull ? _fi.CreationTimeUtc : SqlDateTime.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _fi.CreationTimeUtc = value.Value;
      }
    }

    public SqlString DirectoryName
    {
      get { return !IsNull ? _fi.DirectoryName : SqlString.Null; }
    }

    public SqlBoolean Exists
    {
      get { return !IsNull ? _fi.Exists : SqlBoolean.Null; }
    }

    public SqlString Extension
    {
      get { return !IsNull ? _fi.Extension : SqlString.Null; }
    }

    public SqlString FullName
    {
      get { return !IsNull ? _fi.FullName : SqlString.Null; }
    }

    public SqlBoolean IsReadOnly
    {
      get { return !IsNull ? _fi.IsReadOnly : SqlBoolean.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _fi.IsReadOnly = value.Value;
      }
    }

    public SqlDateTime LastAccessTime
    {
      get { return !IsNull ? _fi.LastAccessTime : SqlDateTime.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _fi.LastAccessTime = value.Value;
      }
    }

    public SqlDateTime LastAccessTimeUtc
    {
      get { return !IsNull ? _fi.LastAccessTimeUtc : SqlDateTime.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _fi.LastAccessTimeUtc = value.Value;
      }
    }

    public SqlDateTime LastWriteTime
    {
      get { return !IsNull ? _fi.LastWriteTime : SqlDateTime.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _fi.LastWriteTime = value.Value;
      }
    }

    public SqlDateTime LastWriteTimeUtc
    {
      get { return !IsNull ? _fi.LastWriteTimeUtc : SqlDateTime.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _fi.LastWriteTimeUtc = value.Value;
      }
    }

    public SqlInt64 Length
    {
      get { return !IsNull ? _fi.Length : SqlInt64.Null; }
    }

    public SqlString Name
    {
      get { return !IsNull ? _fi.Name : SqlString.Null; }
    }

    #endregion
    #endregion
    #region Methods

    public static SqlFileInfo Parse(SqlString s)
    {
      return !s.IsNull ? new SqlFileInfo(s.Value) : Null;
    }

    public override String ToString()
    {
      return _fi != null ? _fi.FullName : SqlFormatting.NullText;
    }

    #region Xml file functions

    [SqlMethod]
    public SqlXml ReadAllXml()
    {
      if (IsNull)
        return SqlXml.Null;

      using (var fs = _fi.OpenRead())
      using (var xrd = XmlReader.Create(fs, new XmlReaderSettings()))
        return new SqlXml(xrd);
    }

    [SqlMethod]
    public SqlInt64 WriteAllXml(SqlXml xml)
    {
      if (IsNull)
        return SqlInt64.Null;

      using (var fs = _fi.Create())
      using (var xwr = XmlWriter.Create(fs, new XmlWriterSettings()))
      using (var xrd = xml.CreateReader())
      {
        xwr.WriteNode(xrd, true);
        xwr.Flush();
        return fs.Length;
      }
    }

    #endregion
    #region Binary file functions

    [SqlMethod]
    public SqlBytes ReadAllBinary()
    {
      if (IsNull)
        return SqlBytes.Null;

      using (var fs = _fi.OpenRead())
        return fs.ReadSqlBytes(fs.Length);
    }

    [SqlMethod]
    public SqlBytes ReadBinary(SqlInt64 offset, SqlInt64 count)
    {
      if (IsNull)
        return SqlBytes.Null;

      using (var fs = _fi.OpenRead())
      {
        if (!offset.IsNull)
          fs.Position = offset.Value;
        return fs.ReadSqlBytes(!count.IsNull ? count.Value : fs.Length - fs.Position);
      }
    }

    [SqlMethod]
    public SqlInt64 WriteAllBinary([SqlFacet(MaxSize = -1)] SqlBytes bytes)
    {
      if (IsNull || bytes.IsNull)
        return SqlInt64.Null;

      using (var stream = _fi.Create())
      {
        Int64 position = stream.Position;
        stream.WriteSqlBytes(bytes);
        stream.Flush();
        return stream.Position - position;
      }
    }

    [SqlMethod]
    public SqlInt64 WriteBinary(SqlBytes bytes, SqlInt64 offset, SqlBoolean insert)
    {
      if (IsNull || bytes.IsNull)
        return SqlInt64.Null;

      using (var fs = _fi.Open(offset.IsNull ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write))
      {
        if (!offset.IsNull)
          fs.Position = offset.Value;
        Int64 position = fs.Position;
        if (!offset.IsNull && insert.IsTrue)
        {
          fs.Move(offset.Value + bytes.Length, bytes.Length, SqlRuntime.IoBufferSize);
          fs.Position = position;
        }
        fs.WriteSqlBytes(bytes);
        fs.Flush();
        return fs.Position - position;
      }
    }

    [SqlMethod]
    public SqlInt64 WriteSizedBlock(SqlBytes bytes, SqlByte sizing, SqlInt64 offset, SqlBoolean insert)
    {
      if (IsNull || bytes.IsNull)
        return SqlInt64.Null;

      var sizeEncoding = !sizing.IsNull ? (SizeEncoding)sizing.Value : SizeEncoding.NO;
      using (var fs = _fi.Open(offset.IsNull ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write))
      {
        if (!offset.IsNull)
          fs.Position = offset.Value;
        Int64 position = fs.Position;
        if (!offset.IsNull && insert.IsTrue)
        {
          long size = bytes.Length + PwrBitConverter.GetSizeEncodingSize(bytes.Length, sizeEncoding);
          fs.Move(offset.Value, size, SqlRuntime.IoBufferSize);
          fs.Position = position;
        }
        if (sizeEncoding != SizeEncoding.NO)
          fs.WriteLongSize(bytes.Length, sizeEncoding);
        fs.WriteSqlBytes(bytes);
        fs.Flush();
        return fs.Position - position;
      }
    }

    [SqlMethod]
    public SqlInt64 WriteTerminatedBlock(SqlBytes bytes, SqlBinary terminator, SqlInt64 offset, SqlBoolean insert)
    {
      if (IsNull || bytes.IsNull)
        return SqlInt64.Null;

      using (var fs = _fi.Open(offset.IsNull ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write))
      {
        if (!offset.IsNull)
          fs.Position = offset.Value;
        Int64 position = fs.Position;
        if (!offset.IsNull && insert.IsTrue)
        {
          long size = bytes.Length + (!terminator.IsNull ? terminator.Length : 0);
          fs.Move(offset.Value, size, SqlRuntime.IoBufferSize);
          fs.Position = position;
        }
        fs.WriteSqlBytes(bytes);
        if (!terminator.IsNull)
          fs.WriteBytes(terminator.Value);
        fs.Flush();
        return fs.Position - position;
      }
    }

    [SqlMethod]
    public SqlBoolean RemoveBlock(SqlInt64 offset, SqlInt64 length)
    {
      if (IsNull || offset.IsNull)
        return SqlBoolean.Null;

      using (var fs = _fi.Open(FileMode.Open, FileAccess.ReadWrite))
      {
        if (offset.Value >= fs.Length)
          return false;
        if (length.IsNull || length > fs.Length - offset.Value)
          fs.SetLength(offset.Value);
        else
        {
          fs.Locate(offset.Value + length.Value).Move(offset.Value, fs.Length - (offset.Value + length.Value), SqlRuntime.IoBufferSize);
          fs.SetLength(fs.Length - length.Value);
        }
        return true;
      }
    }

    [SqlMethod]
    public SqlInt64 SearchBinary(SqlInt64 offset, SqlBytes pattern)
    {
      if (pattern.IsNull)
        return SqlInt64.Null;

      using (var fs = _fi.OpenRead())
      {
        if (!offset.IsNull)
          fs.Position = offset.Value;
        long start = fs.Position;
        long found = fs.Find(long.MaxValue, (int)pattern.Length, (byte v, int i, int j) => pattern[i] == (j >= 0 ? pattern[j] : v));
        return found >= 0L ? start + found : found;
      }
    }

    [SqlMethod]
    public SqlInt64 SearchBinaryLast(SqlInt64 offset, SqlBytes pattern)
    {
      if (pattern.IsNull)
        return SqlInt64.Null;

      using (var fs = _fi.OpenRead())
      {
        if (!offset.IsNull)
          fs.Position = offset.Value;
        long start = fs.Position;
        long found = fs.FindLast(long.MaxValue, (int)pattern.Length, (byte v, int i, int j) => pattern.Buffer[pattern.Length - 1 - i] == (j >= 0 ? pattern[pattern.Length - 1 - j] : v));
        return found >= 0L ? start + found : found;
      }
    }

    #endregion
    #region Text file functions

    [SqlMethod]
    public SqlInt32 DetectCodePage()
    {
      if (IsNull)
        return SqlInt32.Null;

      using (var fs = _fi.OpenRead())
      {
        var preamble = EncodingPreamble.Detect(fs.ReadBytesMost(1, 4) ?? new byte[0]);
        return preamble != null ? preamble.CodePage : SqlInt32.Null;
      }
    }

    [SqlMethod]
    public SqlChars ReadAllText(SqlBoolean detectEncoding)
    {
      if (IsNull)
        return SqlChars.Null;

      var fileEncoding = SqlRuntime.FileEncoding;
      using (var fs = _fi.OpenRead())
      {
        if (detectEncoding.IsTrue)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadBytesMost(1, 4) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
          else
            fs.Position = 0;
        }
        return fs.ReadSqlChars(fileEncoding, (fs.Length - fs.Position).Yield(t => (int)Comparable.Min(t, int.MaxValue), (t, r) => t - r, t => t > 0).Aggregate(0L, (t, p) => t + fileEncoding.GetMaxCharCount(p)));
      }
    }

    [SqlMethod]
    public SqlChars ReadAllTextByCpId(SqlBoolean detectEncoding, SqlInt32 cpId)
    {
      if (IsNull)
        return SqlChars.Null;

      var fileEncoding = cpId.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpId.Value);
      using (var fs = _fi.OpenRead())
      {
        if (detectEncoding.IsTrue)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadBytesMost(1, 4) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
          else
            fs.Position = 0;
        }
        return fs.ReadSqlChars(fileEncoding, (fs.Length - fs.Position).Yield(t => (int)Comparable.Min(t, int.MaxValue), (t, r) => t - r, t => t > 0).Aggregate(0L, (t, p) => t + fileEncoding.GetMaxCharCount(p)));
      }
    }

    [SqlMethod]
    public SqlChars ReadAllTextByCpName(SqlBoolean detectEncoding, [SqlFacet(MaxSize = 128)] SqlString cpName)
    {
      if (IsNull)
        return SqlChars.Null;

      var fileEncoding = cpName.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpName.Value);
      using (var fs = _fi.OpenRead())
      {
        if (detectEncoding.IsTrue)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadBytesMost(1, 4) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
          else
            fs.Position = 0;
        }
        return fs.ReadSqlChars(fileEncoding, (fs.Length - fs.Position).Yield(t => (int)Comparable.Min(t, int.MaxValue), (t, r) => t - r, t => t > 0).Aggregate(0L, (t, p) => t + fileEncoding.GetMaxCharCount(p)));
      }
    }

    [SqlMethod]
    public SqlChars ReadText(SqlInt64 offset, SqlInt64 count, SqlBoolean detectEncoding)
    {
      if (IsNull)
        return SqlChars.Null;

      var fileEncoding = SqlRuntime.FileEncoding;
      using (var fs = _fi.OpenRead())
      {
        if (detectEncoding.IsTrue)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadBytesMost(1, 4) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
          else
            fs.Position = 0;
        }
        if (!offset.IsNull)
          fs.Position = offset.Value;
        return fs.ReadSqlChars(fileEncoding, !count.IsNull ? count.Value : (fs.Length - fs.Position).Yield(t => (int)Comparable.Min(t, int.MaxValue), (t, r) => t - r, t => t > 0).Aggregate(0L, (t, p) => t + fileEncoding.GetMaxCharCount(p)));
      }
    }

    [SqlMethod]
    public SqlChars ReadTextByCpId(SqlInt64 offset, SqlInt64 count, SqlBoolean detectEncoding, SqlInt32 cpId)
    {
      if (IsNull)
        return SqlChars.Null;

      var fileEncoding = cpId.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpId.Value);
      using (var fs = _fi.OpenRead())
      {
        if (detectEncoding.IsTrue)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadBytesMost(1, 4) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
          else
            fs.Position = 0;
        }
        if (!offset.IsNull)
          fs.Position = offset.Value;
        return fs.ReadSqlChars(fileEncoding, !count.IsNull ? count.Value : (fs.Length - fs.Position).Yield(t => (int)Comparable.Min(t, int.MaxValue), (t, r) => t - r, t => t > 0).Aggregate(0L, (t, p) => t + fileEncoding.GetMaxCharCount(p)));
      }
    }

    [SqlMethod]
    public SqlChars ReadTextByCpName(SqlInt64 offset, SqlInt64 count, SqlBoolean detectEncoding, [SqlFacet(MaxSize = 128)] SqlString cpName)
    {
      if (IsNull)
        return SqlChars.Null;

      var fileEncoding = cpName.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpName.Value);
      using (var fs = _fi.OpenRead())
      {
        if (detectEncoding.IsTrue)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadBytesMost(1, 4) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
          else
            fs.Position = 0;
        }
        if (!offset.IsNull)
          fs.Position = offset.Value;
        return fs.ReadSqlChars(fileEncoding, !count.IsNull ? count.Value : (fs.Length - fs.Position).Yield(t => (int)Comparable.Min(t, int.MaxValue), (t, r) => t - r, t => t > 0).Aggregate(0L, (t, p) => t + fileEncoding.GetMaxCharCount(p)));
      }
    }

    [SqlMethod]
    public SqlInt64 WriteAllText(SqlChars chars, SqlBoolean writeEncoding)
    {
      if (IsNull || chars.IsNull)
        return SqlInt64.Null;

      var fileEncoding = SqlRuntime.FileEncoding;
      using (var fs = _fi.Open(FileMode.Create, FileAccess.Write))
      {
        Int64 position = fs.Position;
        if (writeEncoding.IsTrue)
          fs.WriteBytes(fileEncoding.GetPreamble());
        fs.WriteSqlChars(chars, fileEncoding);
        fs.Flush();
        return fs.Position - position;
      }
    }

    [SqlMethod]
    public SqlInt64 WriteAllTextByCpId(SqlChars chars, SqlBoolean writeEncoding, SqlInt32 cpId)
    {
      if (IsNull || chars.IsNull)
        return SqlInt64.Null;

      var fileEncoding = cpId.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpId.Value);
      using (var fs = _fi.Open(FileMode.Create, FileAccess.Write))
      {
        Int64 position = fs.Position;
        if (writeEncoding.IsTrue)
          fs.WriteBytes(fileEncoding.GetPreamble());
        fs.WriteSqlChars(chars, fileEncoding);
        fs.Flush();
        return fs.Position - position;
      }
    }

    [SqlMethod]
    public SqlInt64 WriteAllTextByCpName(SqlChars chars, SqlBoolean writeEncoding, [SqlFacet(MaxSize = 128)] SqlString cpName)
    {
      if (IsNull || chars.IsNull)
        return SqlInt64.Null;

      var fileEncoding = cpName.IsNull ? null : Encoding.GetEncoding(cpName.Value);
      using (var fs = _fi.Open(FileMode.Create, FileAccess.Write))
      {
        Int64 position = fs.Position;
        if (writeEncoding.IsTrue)
          fs.WriteBytes(fileEncoding.GetPreamble());
        fs.WriteSqlChars(chars, fileEncoding);
        fs.Flush();
        return fs.Position - position;
      }
    }

    [SqlMethod]
    public SqlInt64 WriteText(SqlChars chars, SqlString terminator, SqlInt64 offset, SqlBoolean insert, SqlByte useEncoding)
    {
      if (IsNull || chars.IsNull)
        return SqlInt64.Null;

      var fileEncoding = SqlRuntime.FileEncoding;
      var usePreamble = useEncoding.IsNull ? 0 : useEncoding.Value;
      using (var fs = _fi.Open(FileMode.OpenOrCreate, usePreamble > 0 ? FileAccess.ReadWrite : FileAccess.Write))
      {
        if (usePreamble > 0)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadAt(0L, s => s.ReadBytesMost(1, 4)) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
          else if (usePreamble == 2)
          {
            var bom = fileEncoding.GetPreamble();
            if (bom != null && bom.Length > 0)
            {
              fs.Move(bom.Length, fs.Length, SqlRuntime.IoBufferSize);
              fs.Locate(0).WriteBytes(bom);
              if (!offset.IsNull)
                offset = new SqlInt64(offset.Value + bom.Length);
            }
          }
        }
        if (!offset.IsNull)
          fs.Position = offset.Value;
        else
          fs.Position = fs.Length;
        Int64 position = fs.Position;
        if (!offset.IsNull && insert.IsTrue)
        {
          long totalSize = fileEncoding.GetByteCount(chars.Buffer, 0, (int)chars.Length) + (!terminator.IsNull ? fileEncoding.GetByteCount(terminator.Value) : 0);
          fs.Move(offset.Value + totalSize, totalSize, SqlRuntime.IoBufferSize);
          fs.Position = position;
        }
        fs.WriteSqlChars(chars, fileEncoding);
        if (!terminator.IsNull)
          fs.WriteString(terminator.Value, fileEncoding);
        fs.Flush();
        return fs.Position - position;
      }
    }

    [SqlMethod]
    public SqlInt64 WriteTextByCpId(SqlChars chars, SqlString terminator, SqlInt64 offset, SqlBoolean insert, SqlByte useEncoding, SqlInt32 cpId)
    {
      if (IsNull || chars.IsNull)
        return SqlInt64.Null;

      var fileEncoding = cpId.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpId.Value);
      var usePreamble = useEncoding.IsNull ? 0 : useEncoding.Value;
      using (var fs = _fi.Open(FileMode.OpenOrCreate, usePreamble > 0 ? FileAccess.ReadWrite : FileAccess.Write))
      {
        if (usePreamble > 0)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadAt(0L, s => s.ReadBytesMost(1, 4)) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
          else if (usePreamble == 2)
          {
            var bom = fileEncoding.GetPreamble();
            if (bom != null && bom.Length > 0)
            {
              fs.Move(bom.Length, fs.Length, SqlRuntime.IoBufferSize);
              fs.Locate(0).WriteBytes(bom);
              if (!offset.IsNull)
                offset = new SqlInt64(offset.Value + bom.Length);
            }
          }
        }
        if (!offset.IsNull)
          fs.Position = offset.Value;
        else
          fs.Position = fs.Length;
        Int64 position = fs.Position;
        if (!offset.IsNull && insert.IsTrue)
        {
          long size = fileEncoding.GetByteCount(chars.Buffer, 0, (int)chars.Length) + (!terminator.IsNull ? fileEncoding.GetByteCount(terminator.Value) : 0);
          fs.Move(offset.Value + size, size, SqlRuntime.IoBufferSize);
          fs.Position = position;
        }
        fs.WriteSqlChars(chars, fileEncoding);
        if (!terminator.IsNull)
          fs.WriteString(terminator.Value, fileEncoding);
        fs.Flush();
        return fs.Position - position;
      }
    }

    [SqlMethod]
    public SqlInt64 WriteTextByCpName(SqlChars chars, SqlString terminator, SqlInt64 offset, SqlBoolean insert, SqlByte useEncoding, [SqlFacet(MaxSize = 128)] SqlString cpName)
    {
      if (IsNull || chars.IsNull)
        return SqlInt64.Null;

      var fileEncoding = cpName.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpName.Value);
      var usePreamble = useEncoding.IsNull ? 0 : useEncoding.Value;
      using (var fs = _fi.Open(FileMode.OpenOrCreate, usePreamble > 0 ? FileAccess.ReadWrite : FileAccess.Write))
      {
        if (usePreamble > 0)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadAt(0L, s => s.ReadBytesMost(1, 4)) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
          else if (usePreamble == 2)
          {
            var bom = fileEncoding.GetPreamble();
            if (bom != null && bom.Length > 0)
            {
              fs.Move(bom.Length, fs.Length, SqlRuntime.IoBufferSize);
              fs.Locate(0).WriteBytes(bom);
              if (!offset.IsNull)
                offset = new SqlInt64(offset.Value + bom.Length);
            }
          }
        }
        if (!offset.IsNull)
          fs.Position = offset.Value;
        else
          fs.Position = fs.Length;
        Int64 position = fs.Position;
        if (!offset.IsNull && insert.IsTrue)
        {
          long size = fileEncoding.GetByteCount(chars.Buffer, 0, (int)chars.Length) + (!terminator.IsNull ? fileEncoding.GetByteCount(terminator.Value) : 0);
          fs.Move(offset.Value + size, size, SqlRuntime.IoBufferSize);
          fs.Position = position;
        }
        fs.WriteSqlChars(chars, fileEncoding);
        if (!terminator.IsNull)
          fs.WriteString(terminator.Value, fileEncoding);
        fs.Flush();
        return fs.Position - position;
      }
    }

    [SqlMethod]
    public SqlInt64 SearchText(SqlInt64 offset, SqlInt32 skip, SqlChars pattern, SqlBoolean detectEncoding)
    {
      if (IsNull || pattern.IsNull)
        return SqlInt64.Null;

      var fileEncoding = SqlRuntime.FileEncoding;
      using (var fs = _fi.OpenRead())
      {
        if (detectEncoding.IsTrue)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadBytesMost(1, 4) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
          else
            fs.Position = 0L;
        }
        if (!offset.IsNull)
          fs.Position = offset.Value;
        if (!skip.IsNull)
          using (var e = fs.ReadChars(fileEncoding).GetEnumerator())
            for (int count = skip.Value; count > 0 && e.MoveNext(); count--) ;
        long start = fs.Position;
        long found = fs.Find(fileEncoding, long.MaxValue, (int)pattern.Length, (char v, int i, int j) => pattern[i] == (j >= 0 ? pattern[j] : v));
        return found >= 0L ? start + found : found;
      }
    }

    [SqlMethod]
    public SqlInt64 SearchTextByCpId(SqlInt64 offset, SqlInt32 skip, SqlChars pattern, SqlBoolean detectEncoding, SqlInt32 cpId)
    {
      if (IsNull || pattern.IsNull)
        return SqlInt64.Null;

      var fileEncoding = cpId.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpId.Value);
      using (var fs = _fi.OpenRead())
      {
        if (detectEncoding.IsTrue)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadBytesMost(1, 4) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
          else
            fs.Position = 0L;
        }
        if (!offset.IsNull)
          fs.Position = offset.Value;
        if (!skip.IsNull)
          using (var e = fs.ReadChars(fileEncoding).GetEnumerator())
            for (int count = skip.Value; count > 0 && e.MoveNext(); count--) ;
        long start = fs.Position;
        long found = fs.Find(fileEncoding, long.MaxValue, (int)pattern.Length, (char v, int i, int j) => pattern[i] == (j >= 0 ? pattern[j] : v));
        return found >= 0L ? start + found : found;
      }
    }

    [SqlMethod]
    public SqlInt64 SearchTextByCpName(SqlInt64 offset, SqlInt32 skip, SqlChars pattern, SqlBoolean detectEncoding, [SqlFacet(MaxSize = 128)]SqlString cpName)
    {
      if (IsNull || pattern.IsNull)
        return SqlInt64.Null;

      var fileEncoding = cpName.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpName.Value);
      using (var fs = _fi.OpenRead())
      {
        if (detectEncoding.IsTrue)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadBytesMost(1, 4) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
          else
            fs.Position = 0L;
        }
        if (!offset.IsNull)
          fs.Position = offset.Value;
        if (!skip.IsNull)
          using (var e = fs.ReadChars(fileEncoding).GetEnumerator())
            for (int count = skip.Value; count > 0 && e.MoveNext(); count--) ;
        long start = fs.Position;
        long found = fs.Find(fileEncoding, long.MaxValue, (int)pattern.Length, (char v, int i, int j) => pattern[i] == (j >= 0 ? pattern[j] : v));
        return found >= 0L ? start + found : found;
      }
    }

    #endregion
    #region File manipulation function

    [SqlMethod]
    public SqlBoolean Delete()
    {
      if (IsNull)
        return SqlBoolean.Null;

      if (!Exists)
        return false;

      _fi.Delete();
      return true;
    }

    [SqlMethod]
    public SqlBoolean CopyTo(SqlString targetPath, SqlBoolean overwrite)
    {
      if (IsNull || targetPath.IsNull)
        return SqlBoolean.Null;

      if (!Exists)
        return false;

      _fi.CopyTo(targetPath.Value, overwrite.IsTrue);
      return true;
    }

    [SqlMethod]
    public SqlBoolean MoveTo(SqlString targetPath)
    {
      if (IsNull || targetPath.IsNull)
        return SqlBoolean.Null;

      if (!Exists)
        return false;

      _fi.MoveTo(targetPath.Value);
      return true;
    }

    [SqlMethod]
    public SqlBoolean Replace(SqlString targetPath, SqlString targetBackupFilename)
    {
      if (IsNull || targetPath.IsNull)
        return SqlBoolean.Null;

      if (!Exists)
        return false;

      _fi.Replace(targetPath.Value, targetBackupFilename.IsNull ? null : targetBackupFilename.Value);
      return true;
    }

    [SqlMethod]
    public SqlBoolean Encrypt()
    {
      if (IsNull)
        return SqlBoolean.Null;

      if (!Exists)
        return false;

      _fi.Encrypt();
      return true;
    }

    [SqlMethod]
    public SqlBoolean Decrypt()
    {
      if (IsNull)
        return SqlBoolean.Null;

      if (!Exists)
        return false;

      _fi.Decrypt();
      return true;
    }

    [SqlMethod]
    public SqlBoolean Truncate()
    {
      if (IsNull)
        return SqlBoolean.Null;

      if (!Exists)
        return false;

      using (var fs = _fi.Open(FileMode.Truncate, FileAccess.Write)) ;
      return true;
    }

    #endregion
    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _fi = rd.BaseStream.ReadObject(s => new FileInfo(s.ReadString(Encoding.UTF8, SizeEncoding.VB)), s => s.ReadBoolean(TypeCode.Byte));
    }

    public void Write(BinaryWriter wr)
    {
      wr.BaseStream.WriteObject(_fi != null ? _fi.FullName : null, (s, t) => s.WriteString(t, Encoding.UTF8, SizeEncoding.VB), (s, f) => s.WriteBoolean(f, TypeCode.Byte, false));
    }

    #endregion
  }
}
