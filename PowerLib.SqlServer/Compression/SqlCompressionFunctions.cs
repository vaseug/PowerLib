using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.IO;

namespace PowerLib.SqlServer.Compression
{
  public static class SqlCompressionFunctions
  {
    #region Stream functions

    [SqlFunction(Name = "comprDeflateCompress", IsDeterministic = true)]
    public static SqlBytes DeflateCompress(SqlBytes input, SqlInt32 compressionLevel)
    {
      if (input.IsNull)
        return SqlBytes.Null;

      MemoryStream ms = new MemoryStream();
      try
      {
        using (var cs = compressionLevel.IsNull ? new DeflateStream(ms, CompressionMode.Compress, true) :
          new DeflateStream(ms, (CompressionLevel)Enum.ToObject(typeof(CompressionLevel), compressionLevel.Value), true))
            input.Stream.CopyTo(cs);
      }
      catch (Exception)
      {
        ms.Dispose();
        throw;
      }
      return new SqlBytes(ms);
    }

    [SqlFunction(Name = "comprDeflateDecompress", IsDeterministic = true)]
    public static SqlBytes DeflateDecompress(SqlBytes input)
    {
      if (input.IsNull)
        return SqlBytes.Null;

      using (var cs = new DeflateStream(input.Stream, CompressionMode.Decompress, true))
      {
        MemoryStream ms = new MemoryStream();
        try
        {
          cs.CopyTo(ms);
        }
        catch (Exception)
        {
          ms.Dispose();
          throw;
        }
        return new SqlBytes(ms);
      }
    }

    [SqlFunction(Name = "comprGZipCompress", IsDeterministic = true)]
    public static SqlBytes GZipCompress(SqlBytes input, SqlInt32 compressionLevel)
    {
      if (input.IsNull)
        return SqlBytes.Null;

      MemoryStream ms = new MemoryStream();
      try
      {
        using (var cs = compressionLevel.IsNull ? new GZipStream(ms, CompressionMode.Compress, true) :
          new GZipStream(ms, (CompressionLevel)Enum.ToObject(typeof(CompressionLevel), compressionLevel.Value), true))
          input.Stream.CopyTo(cs);
      }
      catch (Exception)
      {
        ms.Dispose();
        throw;
      }
      return new SqlBytes(ms);
    }

    [SqlFunction(Name = "comprGZipDecompress", IsDeterministic = true)]
    public static SqlBytes GZipDecompress(SqlBytes input)
    {
      if (input.IsNull)
        return SqlBytes.Null;

      using (var cs = new GZipStream(input.Stream, CompressionMode.Decompress, true))
      {
        MemoryStream ms = new MemoryStream();
        try
        {
          cs.CopyTo(ms);
        }
        catch (Exception)
        {
          ms.Dispose();
          throw;
        }
        return new SqlBytes(ms);
      }
    }

    #endregion
    #region ZipArchive functions

    [SqlFunction(Name = "zipArchiveAddEntry", IsDeterministic = false)]
    public static SqlBytes ZipArchiveAddEntry(SqlBytes input, SqlString entryName, SqlBytes entryData, [DefaultValue("NULL")] SqlInt32 compressionLevel)
    {
      if (input == null)
        throw new ArgumentNullException("input");

      if (input.IsNull || entryData.IsNull || entryName.IsNull)
        return input;

      using (ZipArchive archive = new ZipArchive(input.Stream, ZipArchiveMode.Create, true))
      {
        ZipArchiveEntry entry = compressionLevel.IsNull ? archive.CreateEntry(entryName.Value) :
          archive.CreateEntry(entryName.Value, (CompressionLevel)Enum.ToObject(typeof(CompressionLevel), compressionLevel.Value));
        using (Stream es = entry.Open())
          entryData.Stream.Locate(0L).CopyTo(es);
      }
      return input;
    }

    [SqlFunction(Name = "zipArchiveDeleteEntry", IsDeterministic = false)]
    public static SqlBytes ZipArchiveDeleteEntry(SqlBytes input, SqlString entryName)
    {
      if (input == null)
        throw new ArgumentNullException("input");

      if (input.IsNull || entryName.IsNull)
        return input;

      using (ZipArchive zipArchive = new ZipArchive(input.Stream, ZipArchiveMode.Update, true))
      {
        ZipArchiveEntry zipEntry = zipArchive.GetEntry(entryName.Value);
        zipEntry.Delete();
      }
      return input;
    }

    [SqlFunction(Name = "zipArchiveGetEntry", IsDeterministic = false)]
    public static SqlBytes ZipArchiveGetEntry(SqlBytes input,  SqlString entryName)
    {
      if (input == null)
        throw new ArgumentNullException("input");

      if (input.IsNull || entryName.IsNull)
        return SqlBytes.Null;

      using (ZipArchive zipArchive = new ZipArchive(input.Stream, ZipArchiveMode.Read, true))
      {
        ZipArchiveEntry zipEntry = zipArchive.GetEntry(entryName.Value);
        if (zipEntry == null)
          return SqlBytes.Null;
        using (Stream es = zipEntry.Open())
        {
          MemoryStream ms = new MemoryStream();
          try
          {
            es.CopyTo(ms);
          }
          catch (Exception)
          {
            ms.Dispose();
            throw;
          }
          return new SqlBytes(ms);
        }
      }
    }

    [SqlFunction(Name = "zipArchiveGetEntries", IsDeterministic = false, FillRowMethodName = "ZipArchiveEntryFillRow")]
    public static IEnumerable ZipArchiveGetEntries(SqlBytes input)
    {
      if (input.IsNull || input.Length == 0)
        yield break;

      using (ZipArchive arch = new ZipArchive(input.Stream, ZipArchiveMode.Read))
        foreach (ZipArchiveEntry entry in arch.Entries)
          yield return entry;
    }

    private static void ZipArchiveEntryFillRow(object obj,  out SqlString Name,  out SqlString FullName, out SqlInt64 Length, out SqlInt64 CompressedLength, out SqlDateTime LastWriteTime)
    {
      ZipArchiveEntry entry = (ZipArchiveEntry)obj;
      Name = entry.Name;
      FullName = entry.FullName;
      Length = entry.Length;
      CompressedLength = entry.CompressedLength;
      LastWriteTime = entry.LastWriteTime.DateTime;
    }

    #endregion
  }
}
