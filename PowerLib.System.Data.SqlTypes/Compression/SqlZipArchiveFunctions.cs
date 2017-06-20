using System;
using System.Collections;
using System.IO.Compression;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace PowerLib.System.Data.SqlTypes.Compression
{
  public static class SqlZipArchiveFunctions
  {
    /// <summary>
    /// Create empty zip archive.
    /// </summary>
    /// <returns>Empty zip archive.</returns>
    [SqlFunction(Name = "zaCreate")]
    public static SqlZipArchive ZipArchiveCreate()
    {
      return new SqlZipArchive();
    }

    /// <summary>
    /// Select zip archive entries.
    /// </summary>
    /// <param name="ziparch">Zip archive.</param>
    /// <returns>Zip archive entry rows collection.</returns>
    [SqlFunction(Name = "zaGetEntries", FillRowMethodName = "ZipArchiveEntryFillRow")]
    public static IEnumerable ZipArchiveGetEntries(SqlZipArchive ziparch)
    {
      if (ziparch == null)
        throw new ArgumentNullException("ziparch");

      if (ziparch.IsNull)
        yield break;

      using (ZipArchive zipArchive = ziparch.GetZipArchive(ZipArchiveMode.Read))
        if (zipArchive != null)
          foreach (var entry in zipArchive.Entries)
            yield return entry;
    }

    private static void ZipArchiveEntryFillRow(object obj, out SqlString Name, out SqlString FullName, out SqlInt64 Length, out SqlInt64 CompressedLength, out SqlDateTime LastWriteTime)
    {
      ZipArchiveEntry entry = (ZipArchiveEntry)obj;
      Name = entry.Name;
      FullName = entry.FullName;
      Length = entry.Length;
      CompressedLength = entry.CompressedLength;
      LastWriteTime = entry.LastWriteTime.DateTime;
    }
  }
}
