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
using PowerLib.SqlServer;

namespace PowerLib.System.Data.SqlTypes.FileSystem
{
	public static class SqlFileSystemFunctions
	{
    #region Binary file functions

    [SqlFunction(Name = "fiReadSizedBlocks", IsDeterministic = false, FillRowMethodName = "BlockFillRow")]
    public static IEnumerable ReadSizedBlocks(SqlFileInfo fileInfo, SqlInt64 offset, SqlByte sizing, [DefaultValue("NULL")] SqlInt32 maxCount)
    {
      if (fileInfo.IsNull || sizing.IsNull)
        yield break;

      var sizeEncoding = (SizeEncoding)sizing.Value;
      if (sizeEncoding == SizeEncoding.NO)
        yield break;

      using (var fs = fileInfo.FileInfo.OpenRead())
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

    [SqlFunction(Name = "fiReadTerminatedBlocks", IsDeterministic = false, FillRowMethodName = "BlockFillRow")]
    public static IEnumerable ReadTerminatedBlocks(SqlFileInfo fileInfo, SqlInt64 offset, SqlBytes searchTerminator, [DefaultValue("1")] SqlBoolean omitTerminator, [DefaultValue("NULL")] SqlInt32 maxCount)
    {
      if (fileInfo.IsNull || searchTerminator.IsNull)
        yield break;

      var terminator = searchTerminator.Value;
      using (var fs = fileInfo.FileInfo.OpenRead())
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

    #endregion
    #region Text file functions

    [SqlFunction(Name = "fiReadLines", IsDeterministic = false, FillRowMethodName = "LineFillRow")]
    public static IEnumerable ReadLines(SqlFileInfo fileInfo, SqlInt64 offset, SqlString searchTerminator, SqlString delimiter, [DefaultValue("''")] SqlString newTerminator,
      [DefaultValue("NULL")] SqlInt32 maxCount, SqlBoolean detectEncoding)
    {
      if (fileInfo.IsNull || searchTerminator.IsNull)
        yield break;

      var fileEncoding = SqlRuntime.FileEncoding;
      var terminatorsList = delimiter.IsNull || delimiter.Value.Length == 0 ? new[] { searchTerminator.Value } : searchTerminator.Value.Split(new[] { delimiter.Value }, StringSplitOptions.RemoveEmptyEntries);
      using (var fs = fileInfo.FileInfo.OpenRead())
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

    [SqlFunction(Name = "fiReadLinesByCpId", IsDeterministic = false, FillRowMethodName = "LineFillRow")]
    public static IEnumerable ReadLinesByCpId(SqlFileInfo fileInfo, SqlInt64 offset, SqlString searchTerminator, SqlString delimiter, [DefaultValue("''")] SqlString newTerminator,
      [DefaultValue("NULL")] SqlInt32 maxCount, SqlBoolean detectEncoding, SqlInt32 cpId)
    {
      if (fileInfo.IsNull || searchTerminator.IsNull)
        yield break;

      var fileEncoding = cpId.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpId.Value);
      var terminatorsList = searchTerminator.IsNull || delimiter.Value.Length == 0 ? new[] { searchTerminator.Value } : searchTerminator.Value.Split(new[] { delimiter.Value }, StringSplitOptions.RemoveEmptyEntries);
      using (var fs = fileInfo.FileInfo.OpenRead())
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

    [SqlFunction(Name = "fiReadLinesByCpName", IsDeterministic = false, FillRowMethodName = "LineFillRow")]
    public static IEnumerable ReadLinesByCpName(SqlFileInfo fileInfo, SqlInt64 offset, SqlString searchTerminator, SqlString delimiter, [DefaultValue("''")] SqlString newTerminator,
      [DefaultValue("NULL")] SqlInt32 maxCount, SqlBoolean detectEncoding, [SqlFacet(MaxSize = 128)] SqlString cpName)
    {
      if (fileInfo.IsNull || searchTerminator.IsNull)
        yield break;

      var fileEncoding = cpName.IsNull ? SqlRuntime.FileEncoding : Encoding.GetEncoding(cpName.Value);
      var terminatorsList = searchTerminator.IsNull || delimiter.Value.Length == 0 ? new[] { searchTerminator.Value } : searchTerminator.Value.Split(new[] { delimiter.Value }, StringSplitOptions.RemoveEmptyEntries);
      using (var fs = fileInfo.FileInfo.OpenRead())
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

    #endregion
    #region Directory functions

    [SqlFunction(Name = "diEnumerate", IsDeterministic = false, FillRowMethodName = "FileSystemInfoFillRow")]
    public static IEnumerable Enumerate(SqlDirectoryInfo dirInfo, SqlString searchPattern, SqlInt32 maxDepth, SqlInt32 traversalOptions)
    {
      if (dirInfo.IsNull)
        yield break;

      DirectoryInfo di = dirInfo.DirectoryInfo;
      if (!di.Exists)
        yield break;
      foreach (var hfsi in di.CreateHierarchicalDirectoryInfo(null).EnumerateHierarchicalFileSystemInfos(searchPattern.IsNull ? null : searchPattern.Value, maxDepth.IsNull ? int.MaxValue : maxDepth.Value,
        traversalOptions.IsNull ? 0 : (FileSystemTraversalOptions)traversalOptions.Value,
        default(Func<FileSystemInfo, bool>), default(Comparison<FileSystemInfo>)))
        yield return hfsi;
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
