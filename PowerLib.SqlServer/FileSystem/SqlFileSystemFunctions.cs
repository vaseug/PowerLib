using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Linq;
using System.Data.SqlTypes;
using System.Xml;
using System.ComponentModel;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.IO;
using PowerLib.System.Linq;
using PowerLib.System.Text;
using PowerLib.System.Data.SqlTypes.IO;

namespace PowerLib.SqlServer.FileSystem
{
	public static class SqlFileSystemFunctions
	{
    #region Xml file functions

    [SqlFunction(Name = "fileReadAllXml", IsDeterministic = false)]
    public static SqlXml ReadAllXml(SqlString path)
    {
      if (path.IsNull)
        return SqlXml.Null;

      using (var fs = File.OpenRead(path.Value))
      using (var xrd = XmlReader.Create(fs, new XmlReaderSettings()))
        return new SqlXml(xrd);
    }

    [SqlFunction(Name = "fileWriteAllXml", IsDeterministic = false)]
    public static SqlInt64 WriteAllXml(SqlString path, SqlXml xml)
    {
      if (path.IsNull || xml.IsNull)
        return SqlInt64.Null;

      using (var fs = File.Create(path.Value))
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

    [SqlFunction(Name = "fileReadAllBinary", IsDeterministic = false)]
    public static SqlBytes ReadAllBinary(SqlString path)
    {
      if (path.IsNull)
        return SqlBytes.Null;

      using (var fs = File.OpenRead(path.Value))
        return fs.ReadSqlBytes(fs.Length);
    }

    [SqlFunction(Name = "fileReadBinary", IsDeterministic = false)]
    public static SqlBytes ReadBinary(SqlString path, SqlInt64 offset, SqlInt64 count)
    {
      if (path.IsNull)
        return SqlBytes.Null;

      using (var fs = File.OpenRead(path.Value))
      {
        if (!offset.IsNull)
          fs.Position = offset.Value;
        return fs.ReadSqlBytes(!count.IsNull ? count.Value : fs.Length - fs.Position);
      }
    }

    [SqlFunction(Name = "fileReadSizedBlocks", IsDeterministic = false, FillRowMethodName = "BlockFillRow")]
    public static IEnumerable ReadSizedBlocks(SqlString path, SqlInt64 offset, SqlByte sizing, [DefaultValue("NULL")] SqlInt32 maxCount)
    {
      if (path.IsNull || sizing.IsNull)
        yield break;

      var sizeEncoding = (SizeEncoding)sizing.Value;
      if (sizeEncoding == SizeEncoding.NO)
        yield break;

      using (var fs = File.OpenRead(path.Value))
      {
        if (!offset.IsNull)
          fs.Position = offset.Value;
        var position = fs.Position;
        foreach (var block in fs.ReadCollection(s => s.TryReadBytes(sizeEncoding), maxCount.IsNull ? int.MaxValue : maxCount.Value))
        {
          yield return Tuple.Create(position, block);
          position = fs.Position;
        }
      }
    }

    [SqlFunction(Name = "fileReadTerminatedBlocks", IsDeterministic = false, FillRowMethodName = "BlockFillRow")]
    public static IEnumerable ReadTerminatedBlocks(SqlString path, SqlInt64 offset, SqlBytes searchTerminator, [DefaultValue("1")] SqlBoolean omitTerminator, [DefaultValue("NULL")] SqlInt32 maxCount)
    {
      if (path.IsNull || searchTerminator.IsNull)
        yield break;

      var terminator = searchTerminator.Value;
      using (var fs = File.OpenRead(path.Value))
      {
        if (!offset.IsNull)
          fs.Position = offset.Value;
        var position = fs.Position;
        foreach (var block in fs.ReadCollection(s => s.TryReadBytes(terminator, omitTerminator.IsTrue), maxCount.IsNull ? int.MaxValue : maxCount.Value))
        {
          yield return Tuple.Create(position, block);
          position = fs.Position;
        }
      }
    }

    [SqlFunction(Name = "fileWriteAllBinary", IsDeterministic = false)]
    public static SqlInt64 WriteAllBinary(SqlString path, SqlBytes bytes)
    {
      if (path.IsNull || bytes.IsNull)
        return SqlInt64.Null;

      using (var stream = File.Create(path.Value))
      {
        Int64 position = stream.Position;
        stream.WriteSqlBytes(bytes);
        stream.Flush();
        return stream.Position - position;
      }
    }

    [SqlFunction(Name = "fileWriteBinary", IsDeterministic = false)]
    public static SqlInt64 WriteBinary(SqlString path, SqlBytes bytes, SqlInt64 offset, SqlBoolean insert)
    {
      if (path.IsNull || bytes.IsNull)
        return SqlInt64.Null;

      using (var fs = File.Open(path.Value, offset.IsNull ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write))
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

    [SqlFunction(Name = "fileWriteSizedBlock", IsDeterministic = false)]
    public static SqlInt64 WriteSizedBlock(SqlString path, SqlBytes bytes, SqlByte sizing, SqlInt64 offset, SqlBoolean insert)
    {
      if (path.IsNull || bytes.IsNull)
        return SqlInt64.Null;

      var sizeEncoding = !sizing.IsNull ? (SizeEncoding)sizing.Value : SizeEncoding.NO;
      using (var fs = File.Open(path.Value, offset.IsNull ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write))
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

    [SqlFunction(Name = "fileWriteTerminatedBlock", IsDeterministic = false)]
    public static SqlInt64 WriteTerminatedBlock(SqlString path, SqlBytes bytes, SqlBinary terminator, SqlInt64 offset, SqlBoolean insert)
    {
      if (path.IsNull || bytes.IsNull)
        return SqlInt64.Null;

      using (var fs = File.Open(path.Value, offset.IsNull ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write))
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

    [SqlFunction(Name = "fileRemoveBlock", IsDeterministic = false)]
    public static SqlBoolean RemoveBlock(SqlString path, SqlInt64 offset, SqlInt64 length)
    {
      if (path.IsNull || offset.IsNull)
        return SqlBoolean.Null;

      using (var fs = File.Open(path.Value, FileMode.Open, FileAccess.ReadWrite))
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

    [SqlFunction(Name = "fileSearchBinary", IsDeterministic = false)]
    public static SqlInt64 SearchBinary(SqlString path, SqlInt64 offset, SqlBytes pattern)
    {
      if (path.IsNull || pattern.IsNull)
        return SqlInt64.Null;

      using (var fs = File.OpenRead(path.Value))
      {
        if (!offset.IsNull)
          fs.Position = offset.Value;
        long start = fs.Position;
        long found = fs.Find(long.MaxValue, (int)pattern.Length, (byte v, int i, int j) => pattern[i] == (j >= 0 ? pattern[j] : v));
        return found >= 0L ? start + found : found;
      }
    }

    [SqlFunction(Name = "fileSearchBinaryLast", IsDeterministic = false)]
    public static SqlInt64 SearchBinaryLast(SqlString path, SqlInt64 offset, SqlBytes pattern)
    {
      if (path.IsNull || pattern.IsNull)
        return SqlInt64.Null;

      using (var fs = File.OpenRead(path.Value))
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

    [SqlFunction(Name = "fileDetectCodePage", IsDeterministic = false)]
    public static SqlInt32 DetectCodePage(SqlString path)
    {
      if (path.IsNull)
        return SqlInt32.Null;

      using (var fs = File.OpenRead(path.Value))
      {
        var preamble = EncodingPreamble.Detect(fs.ReadBytesMost(1, 4) ?? new byte[0]);
        return preamble != null ? preamble.CodePage : SqlInt32.Null;
      }
    }

    [SqlFunction(Name = "fileReadAllText", IsDeterministic = false)]
    public static SqlChars ReadAllText(SqlString path, SqlBoolean detectEncoding)
    {
      if (path.IsNull)
        return SqlChars.Null;

      var fileEncoding = SqlRuntime.FileEncoding;
      using (var fs = File.OpenRead(path.Value))
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

    [SqlFunction(Name = "fileReadAllTextByCpId", IsDeterministic = false)]
    public static SqlChars ReadAllTextByCpId(SqlString path, SqlBoolean detectEncoding, SqlInt32 cpId)
    {
      if (path.IsNull)
        return SqlChars.Null;

      var fileEncoding = cpId.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpId.Value);
      using (var fs = File.OpenRead(path.Value))
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

    [SqlFunction(Name = "fileReadAllTextByCpName", IsDeterministic = false)]
    public static SqlChars ReadAllTextByCpName(SqlString path, SqlBoolean detectEncoding, [SqlFacet(MaxSize = 128)] SqlString cpName)
    {
      if (path.IsNull)
        return SqlChars.Null;

      var fileEncoding = cpName.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpName.Value);
      using (var fs = File.OpenRead(path.Value))
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

    [SqlFunction(Name = "fileReadText", IsDeterministic = false)]
    public static SqlChars ReadText(SqlString path, SqlInt64 offset, SqlInt64 count, SqlBoolean detectEncoding)
    {
      if (path.IsNull)
        return SqlChars.Null;

      var fileEncoding = SqlRuntime.FileEncoding;
      using (var fs = File.OpenRead(path.Value))
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

    [SqlFunction(Name = "fileReadTextByCpId", IsDeterministic = false)]
    public static SqlChars ReadTextByCpId(SqlString path, SqlInt64 offset, SqlInt64 count, SqlBoolean detectEncoding, SqlInt32 cpId)
    {
      if (path.IsNull)
        return SqlChars.Null;

      var fileEncoding = cpId.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpId.Value);
      using (var fs = File.OpenRead(path.Value))
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

    [SqlFunction(Name = "fileReadTextByCpName", IsDeterministic = false)]
    public static SqlChars ReadTextByCpName(SqlString path, SqlInt64 offset, SqlInt64 count, SqlBoolean detectEncoding, [SqlFacet(MaxSize = 128)] SqlString cpName)
    {
      if (path.IsNull)
        return SqlChars.Null;

      var fileEncoding = cpName.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpName.Value);
      using (var fs = File.OpenRead(path.Value))
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

    [SqlFunction(Name = "fileReadLines", IsDeterministic = false, FillRowMethodName = "LineFillRow")]
    public static IEnumerable ReadLines(SqlString path, SqlInt64 offset, SqlString searchTerminator, SqlString delimiter, [DefaultValue("''")] SqlString newTerminator,
      [DefaultValue("NULL")] SqlInt32 maxCount, SqlBoolean detectEncoding)
    {
      if (path.IsNull || searchTerminator.IsNull)
        yield break;

      var fileEncoding = SqlRuntime.FileEncoding;
      var terminatorsList = delimiter.IsNull || delimiter.Value.Length == 0 ? new[] { searchTerminator.Value } : searchTerminator.Value.Split(new[] { delimiter.Value }, StringSplitOptions.RemoveEmptyEntries);
      using (var fs = File.OpenRead(path.Value))
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
        var position = fs.Position;
        foreach (var line in fs.ReadLines(fileEncoding, terminatorsList, newTerminator.IsNull ? null : newTerminator.Value, maxCount.IsNull ? int.MaxValue : maxCount.Value))
        {
          yield return Tuple.Create(position, line);
          position = fs.Position;
        }
      }
    }

    [SqlFunction(Name = "fileReadLinesByCpId", IsDeterministic = false, FillRowMethodName = "LineFillRow")]
    public static IEnumerable ReadLinesByCpId(SqlString path, SqlInt64 offset, SqlString searchTerminator, SqlString delimiter, [DefaultValue("''")] SqlString newTerminator,
      [DefaultValue("NULL")] SqlInt32 maxCount, SqlBoolean detectEncoding, SqlInt32 cpId)
    {
      if (path.IsNull || searchTerminator.IsNull)
        yield break;

      var fileEncoding = cpId.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpId.Value);
      var terminatorsList = searchTerminator.IsNull || delimiter.Value.Length == 0 ? new[] { searchTerminator.Value } : searchTerminator.Value.Split(new[] { delimiter.Value }, StringSplitOptions.RemoveEmptyEntries);
      using (var fs = File.OpenRead(path.Value))
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
        var position = fs.Position;
        foreach (var line in fs.ReadLines(fileEncoding, terminatorsList, newTerminator.IsNull ? null : newTerminator.Value, maxCount.IsNull ? int.MaxValue : maxCount.Value))
        {
          yield return Tuple.Create(position, line);
          position = fs.Position;
        }
      }
    }

    [SqlFunction(Name = "fileReadLinesByCpName", IsDeterministic = false, FillRowMethodName = "LineFillRow")]
    public static IEnumerable ReadLinesByCpName(SqlString path, SqlInt64 offset, SqlString searchTerminator, SqlString delimiter, [DefaultValue("''")] SqlString newTerminator,
      [DefaultValue("NULL")] SqlInt32 maxCount, SqlBoolean detectEncoding, [SqlFacet(MaxSize = 128)] SqlString cpName)
    {
      if (path.IsNull || searchTerminator.IsNull)
        yield break;

      var fileEncoding = cpName.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpName.Value);
      var terminatorsList = searchTerminator.IsNull || delimiter.Value.Length == 0 ? new[] { searchTerminator.Value } : searchTerminator.Value.Split(new[] { delimiter.Value }, StringSplitOptions.RemoveEmptyEntries);
      using (var fs = File.OpenRead(path.Value))
      {
        if (detectEncoding.IsTrue)
        {
          var preamble = EncodingPreamble.Detect(fs.ReadAt(0L, s => s.ReadBytesMost(1, 4)) ?? new byte[0]);
          if (preamble != null)
            fileEncoding = Encoding.GetEncoding(preamble.CodePage);
        }
        if (!offset.IsNull)
          fs.Position = offset.Value;
        var position = fs.Position;
        foreach (var line in fs.ReadLines(fileEncoding, terminatorsList, newTerminator.IsNull ? null : newTerminator.Value, maxCount.IsNull ? int.MaxValue : maxCount.Value))
        {
          yield return Tuple.Create(position, line);
          position = fs.Position;
        }
      }
    }

    [SqlFunction(Name = "fileWriteAllText", IsDeterministic = false)]
    public static SqlInt64 WriteAllText(SqlString path, SqlChars chars, SqlBoolean writeEncoding)
    {
      if (path.IsNull || chars.IsNull)
        return SqlInt64.Null;

      var fileEncoding = SqlRuntime.FileEncoding;
      using (var fs = File.Open(path.Value, FileMode.Create, FileAccess.Write))
      {
        Int64 position = fs.Position;
        if (writeEncoding.IsTrue)
          fs.WriteBytes(fileEncoding.GetPreamble());
        fs.WriteSqlChars(chars, fileEncoding);
        fs.Flush();
        return fs.Position - position;
      }
    }

    [SqlFunction(Name = "fileWriteAllTextByCpId", IsDeterministic = false)]
    public static SqlInt64 WriteAllTextByCpId(SqlString path, SqlChars chars, SqlBoolean writeEncoding, SqlInt32 cpId)
    {
      if (path.IsNull || chars.IsNull)
        return SqlInt64.Null;

      var fileEncoding = cpId.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpId.Value);
      using (var fs = File.Open(path.Value, FileMode.Create, FileAccess.Write))
      {
        Int64 position = fs.Position;
        if (writeEncoding.IsTrue)
          fs.WriteBytes(fileEncoding.GetPreamble());
        fs.WriteSqlChars(chars, fileEncoding);
        fs.Flush();
        return fs.Position - position;
      }
    }

    [SqlFunction(Name = "fileWriteAllTextByCpName", IsDeterministic = false)]
    public static SqlInt64 WriteAllTextByCpName(SqlString path, SqlChars chars, SqlBoolean writeEncoding, [SqlFacet(MaxSize = 128)] SqlString cpName)
    {
      if (path.IsNull || chars.IsNull)
        return SqlInt64.Null;

      var fileEncoding = cpName.IsNull ? null : Encoding.GetEncoding(cpName.Value);
      using (var fs = File.Open(path.Value, FileMode.Create, FileAccess.Write))
      {
        Int64 position = fs.Position;
        if (writeEncoding.IsTrue)
          fs.WriteBytes(fileEncoding.GetPreamble());
        fs.WriteSqlChars(chars, fileEncoding);
        fs.Flush();
        return fs.Position - position;
      }
    }

    [SqlFunction(Name = "fileWriteText", IsDeterministic = false)]
    public static SqlInt64 WriteText(SqlString path, SqlChars chars, SqlString terminator, SqlInt64 offset, SqlBoolean insert, SqlByte useEncoding)
    {
      if (path.IsNull || chars.IsNull)
        return SqlInt64.Null;

      var fileEncoding = SqlRuntime.FileEncoding;
      var usePreamble = useEncoding.IsNull ? 0 : useEncoding.Value;
      using (var fs = File.Open(path.Value, FileMode.OpenOrCreate, usePreamble > 0 ? FileAccess.ReadWrite : FileAccess.Write))
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

    [SqlFunction(Name = "fileWriteTextByCpId", IsDeterministic = false)]
    public static SqlInt64 WriteTextByCpId(SqlString path, SqlChars chars, SqlString terminator, SqlInt64 offset, SqlBoolean insert, SqlByte useEncoding, SqlInt32 cpId)
    {
      if (path.IsNull || chars.IsNull)
        return SqlInt64.Null;

      var fileEncoding = cpId.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpId.Value);
      var usePreamble = useEncoding.IsNull ? 0 : useEncoding.Value;
      using (var fs = File.Open(path.Value, FileMode.OpenOrCreate, usePreamble > 0 ? FileAccess.ReadWrite : FileAccess.Write))
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

    [SqlFunction(Name = "fileWriteTextByCpName", IsDeterministic = false)]
    public static SqlInt64 WriteTextByCpName(SqlString path, SqlChars chars, SqlString terminator, SqlInt64 offset, SqlBoolean insert, SqlByte useEncoding, [SqlFacet(MaxSize = 128)] SqlString cpName)
    {
      if (path.IsNull || chars.IsNull)
        return SqlInt64.Null;

      var fileEncoding = cpName.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpName.Value);
      var usePreamble = useEncoding.IsNull ? 0 : useEncoding.Value;
      using (var fs = File.Open(path.Value, FileMode.OpenOrCreate, usePreamble > 0 ? FileAccess.ReadWrite : FileAccess.Write))
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

    [SqlFunction(Name = "fileSearchText", IsDeterministic = false)]
    public static SqlInt64 SearchText(SqlString path, SqlInt64 offset, SqlInt32 skip, SqlChars pattern, SqlBoolean detectEncoding)
    {
      if (path.IsNull || pattern.IsNull)
        return SqlInt64.Null;

      var fileEncoding = SqlRuntime.FileEncoding;
      using (var fs = File.OpenRead(path.Value))
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

    [SqlFunction(Name = "fileSearchTextByCpId", IsDeterministic = false)]
    public static SqlInt64 SearchTextByCpId(SqlString path, SqlInt64 offset, SqlInt32 skip, SqlChars pattern, SqlBoolean detectEncoding, SqlInt32 cpId)
    {
      if (path.IsNull || pattern.IsNull)
        return SqlInt64.Null;

      var fileEncoding = cpId.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpId.Value);
      using (var fs = File.OpenRead(path.Value))
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

    [SqlFunction(Name = "fileSearchTextByCpName", IsDeterministic = false)]
    public static SqlInt64 SearchTextByCpName(SqlString path, SqlInt64 offset, SqlInt32 skip, SqlChars pattern, SqlBoolean detectEncoding, [SqlFacet(MaxSize = 128)]SqlString cpName)
    {
      if (path.IsNull || pattern.IsNull)
        return SqlInt64.Null;

      var fileEncoding = cpName.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpName.Value);
      using (var fs = File.OpenRead(path.Value))
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
    #region Directory manipulation functions

    [SqlFunction(Name = "dirCreate", IsDeterministic = false)]
    public static SqlBoolean CreateDirectory(SqlString path)
    {
      if (path.IsNull)
        return SqlBoolean.Null;

      return Directory.CreateDirectory(path.Value).Exists;
    }

    [SqlFunction(Name = "dirDelete", IsDeterministic = false)]
    public static SqlBoolean DeleteDirectory(SqlString path, SqlBoolean recursive)
    {
      if (path.IsNull)
        return SqlBoolean.Null;

      if (!Directory.Exists(path.Value))
        return false;

      if (recursive.IsNull)
        Directory.Delete(path.Value);
      else
        Directory.Delete(path.Value, recursive.Value);
      return true;
    }

    [SqlFunction(Name = "dirMove", IsDeterministic = false)]
    public static SqlBoolean MoveDirectory(SqlString sourcePath, SqlString targetPath)
    {
      if (sourcePath.IsNull || targetPath.IsNull)
        return SqlBoolean.Null;

      if (!Directory.Exists(sourcePath.Value))
        return false;

      Directory.Move(sourcePath.Value, targetPath.Value);
      return true;
    }

    [SqlFunction(Name = "dirExists", IsDeterministic = false)]
    public static SqlBoolean ExistsDirectory(SqlString path)
    {
      if (path.IsNull)
        return SqlBoolean.Null;

      return Directory.Exists(path.Value);
    }

    [SqlFunction(Name = "dirEnumerate", IsDeterministic = false, FillRowMethodName = "FileSystemInfoFillRow")]
    public static IEnumerable Enumerate(SqlString path, SqlString searchPattern, SqlInt32 maxDepth, SqlInt32 traversalOptions)
    {
      if (path.IsNull)
        yield break;

      DirectoryInfo di = new DirectoryInfo(path.Value);
      if (!di.Exists)
        yield break;
      foreach (var hfsi in di.CreateHierarchicalDirectoryInfo(null).EnumerateHierarchicalFileSystemInfos(searchPattern.IsNull ? null : searchPattern.Value, maxDepth.IsNull ? int.MaxValue : maxDepth.Value,
        traversalOptions.IsNull ? 0 : (FileSystemTraversalOptions)traversalOptions.Value,
        default(Func<FileSystemInfo, bool>), default(Comparison<FileSystemInfo>)))
        yield return hfsi;
    }

    #endregion
    #region File manipulation function

    [SqlFunction(Name = "fileDelete", IsDeterministic = false)]
    public static SqlBoolean DeleteFile(SqlString path)
    {
      if (path.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(path.Value))
        return false;

      File.Delete(path.Value);
      return true;
    }

    [SqlFunction(Name = "fileCopy", IsDeterministic = false)]
    public static SqlBoolean CopyFile(SqlString sourcePath, SqlString targetPath, [DefaultValue("0")] SqlBoolean overwrite)
    {
      if (sourcePath.IsNull || targetPath.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(sourcePath.Value))
        return false;

      File.Copy(sourcePath.Value, targetPath.Value, overwrite.IsTrue);
      return true;
    }

    [SqlFunction(Name = "fileMove", IsDeterministic = false)]
    public static SqlBoolean MoveFile(SqlString sourcePath, SqlString targetPath)
    {
      if (sourcePath.IsNull || targetPath.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(sourcePath.Value))
        return false;

      File.Move(sourcePath.Value, targetPath.Value);
      return true;
    }

    [SqlFunction(Name = "fileReplace", IsDeterministic = false)]
    public static SqlBoolean ReplaceFile(SqlString sourcePath, SqlString targetPath, SqlString targetBackupFilename)
    {
      if (sourcePath.IsNull || targetPath.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(sourcePath.Value))
        return false;

      File.Replace(sourcePath.Value, targetPath.Value, targetBackupFilename.IsNull ? null : targetBackupFilename.Value);
      return true;
    }

    [SqlFunction(Name = "fileEncrypt", IsDeterministic = false)]
    public static SqlBoolean EncryptFile(SqlString path)
    {
      if (path.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(path.Value))
        return false;

      File.Encrypt(path.Value);
      return true;
    }

    [SqlFunction(Name = "fileDecrypt", IsDeterministic = false)]
    public static SqlBoolean DecryptFile(SqlString path)
    {
      if (path.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(path.Value))
        return false;

      File.Decrypt(path.Value);
      return true;
    }

    [SqlFunction(Name = "fileExists", IsDeterministic = false)]
    public static SqlBoolean ExistsFile(SqlString path)
    {
      if (path.IsNull)
        return SqlBoolean.Null;

      return File.Exists(path.Value);
    }

    [SqlFunction(Name = "fileTruncate", IsDeterministic = false)]
    public static SqlBoolean TruncateFile(SqlString path)
    {
      if (path.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(path.Value))
        return false;

      using (var fs = File.Open(path.Value, FileMode.Truncate, FileAccess.Write)) ;
      return true;
    }

    [SqlFunction(Name = "fileGetAttributes", IsDeterministic = false)]
    public static SqlInt32 GetFileAttributes(SqlString path)
    {
      if (path.IsNull)
        return SqlInt32.Null;

      return (Int32)File.GetAttributes(path.Value);
    }

    [SqlFunction(Name = "fileGetCreationTime", IsDeterministic = false)]
    public static SqlDateTime GetFileCreationTime(SqlString path)
    {
      if (path.IsNull)
        return SqlDateTime.Null;

      return File.GetCreationTime(path.Value);
    }

    [SqlFunction(Name = "fileGetCreationTimeUtc", IsDeterministic = false)]
    public static SqlDateTime GetFileCreationTimeUtc(SqlString path)
    {
      if (path.IsNull)
        return SqlDateTime.Null;

      return File.GetCreationTimeUtc(path.Value);
    }

    [SqlFunction(Name = "fileGetLastAccessTime", IsDeterministic = false)]
    public static SqlDateTime GetFileLastAccessTime(SqlString path)
    {
      if (path.IsNull)
        return SqlDateTime.Null;

      return File.GetLastAccessTime(path.Value);
    }

    [SqlFunction(Name = "fileGetLastAccessTimeUtc", IsDeterministic = false)]
    public static SqlDateTime GetFileLastAccessTimeUtc(SqlString path)
    {
      if (path.IsNull)
        return SqlDateTime.Null;

      return File.GetLastAccessTimeUtc(path.Value);
    }

    [SqlFunction(Name = "fileGetLastWriteTime", IsDeterministic = false)]
    public static SqlDateTime GetFileLastWriteTime(SqlString path)
    {
      if (path.IsNull)
        return SqlDateTime.Null;

      return File.GetLastWriteTime(path.Value);
    }

    [SqlFunction(Name = "fileGetLastWriteTimeUtc", IsDeterministic = false)]
    public static SqlDateTime GetFileLastWriteTimeUtc(SqlString path)
    {
      if (path.IsNull)
        return SqlDateTime.Null;

      return File.GetLastWriteTimeUtc(path.Value);
    }

    [SqlFunction(Name = "fileSetAttributes", IsDeterministic = false)]
    public static SqlBoolean SetFileAttributes(SqlString path, SqlInt32 attributes)
    {
      if (path.IsNull || attributes.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(path.Value))
        return false;

      File.SetAttributes(path.Value, (FileAttributes)attributes.Value);
      return true;
    }

    [SqlFunction(Name = "fileSetCreationTime", IsDeterministic = false)]
    public static SqlBoolean SetFileCreationTime(SqlString path, SqlDateTime creationTime)
    {
      if (path.IsNull || creationTime.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(path.Value))
        return false;

      File.SetCreationTime(path.Value, creationTime.Value);
      return true;
    }

    [SqlFunction(Name = "fileSetCreationTimeUtc", IsDeterministic = false)]
    public static SqlBoolean SetFileCreationTimeUtc(SqlString path, SqlDateTime creationTimeUtc)
    {
      if (path.IsNull || creationTimeUtc.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(path.Value))
        return false;

      File.SetCreationTimeUtc(path.Value, creationTimeUtc.Value);
      return true;
    }

    [SqlFunction(Name = "fileSetLastAccessTime", IsDeterministic = false)]
    public static SqlBoolean SetFileLastAccessTime(SqlString path, SqlDateTime lastAccessTime)
    {
      if (path.IsNull || lastAccessTime.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(path.Value))
        return false;

      File.SetLastAccessTime(path.Value, lastAccessTime.Value);
      return true;
    }

    [SqlFunction(Name = "fileSetLastAccessTimeUtc", IsDeterministic = false)]
    public static SqlBoolean SetFileLastAccessTimeUtc(SqlString path, SqlDateTime lastAccessTimeUtc)
    {
      if (path.IsNull || lastAccessTimeUtc.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(path.Value))
        return false;

      File.SetLastAccessTimeUtc(path.Value, lastAccessTimeUtc.Value);
      return true;
    }

    [SqlFunction(Name = "fileSetLastWriteTime", IsDeterministic = false)]
    public static SqlBoolean SetFileLastWriteTime(SqlString path, SqlDateTime lastWriteTime)
    {
      if (path.IsNull || lastWriteTime.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(path.Value))
        return false;

      File.SetLastWriteTime(path.Value, lastWriteTime.Value);
      return true;
    }

    [SqlFunction(Name = "fileSetLastWriteTimeUtc", IsDeterministic = false)]
    public static SqlBoolean SetFileLastWriteTimeUtc(SqlString path, SqlDateTime lastWriteTimeUtc)
    {
      if (path.IsNull || lastWriteTimeUtc.IsNull)
        return SqlBoolean.Null;

      if (!File.Exists(path.Value))
        return false;

      File.SetLastWriteTimeUtc(path.Value, lastWriteTimeUtc.Value);
      return true;
    }

    #endregion
    #region Path manipulation functions

    [SqlFunction(Name = "pathChangeExtension", IsDeterministic = false)]
    public static SqlString ChangeExtension(SqlString path, SqlString extension)
    {
      if (path.IsNull || extension.IsNull)
        return SqlString.Null;

      return Path.ChangeExtension(path.Value, extension.Value);
    }

    [SqlFunction(Name = "pathGetDirectoryName", IsDeterministic = false)]
    public static SqlString GetDirectoryName(SqlString path)
    {
      if (path.IsNull)
        return SqlString.Null;

      return Path.GetDirectoryName(path.Value);
    }

    [SqlFunction(Name = "pathGetExtension", IsDeterministic = false)]
    public static SqlString GetExtension(SqlString path)
    {
      if (path.IsNull)
        return SqlString.Null;

      return Path.GetExtension(path.Value);
    }

    [SqlFunction(Name = "pathGetFileName", IsDeterministic = false)]
    public static SqlString GetFileName(SqlString path)
    {
      if (path.IsNull)
        return SqlString.Null;

      return Path.GetFileName(path.Value);
    }

    [SqlFunction(Name = "pathGetFileNameWithoutExtension", IsDeterministic = false)]
    public static SqlString GetFileNameWithoutExtension(SqlString path)
    {
      if (path.IsNull)
        return SqlString.Null;

      return Path.GetFileNameWithoutExtension(path.Value);
    }

    [SqlFunction(Name = "pathGetRoot", IsDeterministic = false)]
    public static SqlString GetPathRoot(SqlString path)
    {
      if (path.IsNull)
        return SqlString.Null;

      return Path.GetPathRoot(path.Value);
    }

    [SqlFunction(Name = "pathGetRandomFileName", IsDeterministic = false)]
    public static SqlString GetRandomFileName()
    {
      return Path.GetRandomFileName();
    }

    [SqlFunction(Name = "pathHasExtension", IsDeterministic = false)]
    public static SqlBoolean HasExtension(SqlString path)
    {
      if (path.IsNull)
        return SqlBoolean.Null;

      return Path.HasExtension(path.Value);
    }

    [SqlFunction(Name = "pathIsRooted", IsDeterministic = false)]
    public static SqlBoolean IsPathRooted(SqlString path)
    {
      if (path.IsNull)
        return SqlBoolean.Null;

      return Path.IsPathRooted(path.Value);
    }

    #endregion
    #region FillRow functions

    private static void FileSystemInfoFillRow(object obj,
      out SqlBoolean IsDirectory, out SqlInt32 Depth,
      out SqlString FullName, out SqlString RelativeName, out SqlString ParentName,
      out SqlString Name, out SqlString Extension,
      out SqlInt32 Attributes, out SqlInt64 Length,
      out SqlDateTime CreationTime, out SqlDateTime CreationTimeUtc,
      out SqlDateTime LastWriteTime, out SqlDateTime LastWriteTimeUtc,
      out SqlDateTime LastAccessTime, out SqlDateTime LastAccessTimeUtc)
		{
      HierarchicalFileSystemInfo hfsi = (HierarchicalFileSystemInfo)obj;
      HierarchicalDirectoryInfo hdi = hfsi as HierarchicalDirectoryInfo;
      HierarchicalFileInfo hfi = hfsi as HierarchicalFileInfo;
      IsDirectory = hdi != null ? true : hfi != null ? false : SqlBoolean.Null;
      Depth = hfsi.Depth;
      ParentName = hfsi.Parent != null ? hfsi.Parent.Info.FullName : SqlString.Null;
      FullName = hfsi.Info.FullName;
      RelativeName = hfsi.RelativeName != null ? hfsi.RelativeName : SqlString.Null;
      Name = hfsi.Info.Name;
      Extension = hfsi.Info.Extension;
      CreationTime = hfsi.Info.CreationTime;
      CreationTimeUtc = hfsi.Info.CreationTimeUtc;
      LastWriteTime = hfsi.Info.LastWriteTime;
      LastWriteTimeUtc = hfsi.Info.LastWriteTimeUtc;
      LastAccessTime = hfsi.Info.LastAccessTime;
      LastAccessTimeUtc = hfsi.Info.LastAccessTimeUtc;
      Attributes = (int)hfsi.Info.Attributes;
      Length = hfi != null ? hfi.Info.Length : SqlInt64.Null;
    }

    private static void BlockFillRow(object obj, out SqlInt64 Offset, [SqlFacet(MaxSize = -1)] out SqlBinary Value)
    {
      var tuple = (Tuple<Int64, Byte[]>)obj;
      Offset = tuple.Item1;
      Value = tuple.Item2 ?? SqlBinary.Null;
    }

    private static void LineFillRow(object obj, out SqlInt64 Offset, [SqlFacet(MaxSize = -1)] out SqlString Value)
    {
      var tuple = (Tuple<Int64, String>)obj;
      Offset = tuple.Item1;
      Value = tuple.Item2 ?? SqlString.Null;
    }

    #endregion
  }
}
