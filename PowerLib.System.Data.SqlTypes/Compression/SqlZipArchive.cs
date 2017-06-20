using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.IO;

namespace PowerLib.System.Data.SqlTypes.Compression
{
  /// <summary>
  /// SqlZipArchive type.
  /// </summary>
  [SqlUserDefinedType(Format.UserDefined, Name = "ZipArchive", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlZipArchive : IDisposable, INullable, IBinarySerialize
  {
    private static readonly SqlZipArchive @null = new SqlZipArchive((Stream)null);

    private Stream _stream;

    #region Constructor

    public SqlZipArchive()
      : this(new MemoryStream())
    {
    }

    public SqlZipArchive(byte[] buffer)
      : this(new MemoryStream(buffer))
    {
    }

    private SqlZipArchive(Stream stream)
    {
      _stream = stream;
    }

    #endregion
    #region Properties

    public static SqlZipArchive Null
    {
      get { return @null; }
    }

    public bool IsNull
    {
      get { return _stream == null; }
    }

    #endregion
    #region Methods

    public void Dispose()
    {
      if (_stream != null)
        _stream.Dispose();
    }

    public static SqlZipArchive Parse(SqlString s)
    {
      throw new NotSupportedException("Conversion from string is not supported.");
    }

    public override String ToString()
    {
      throw new NotSupportedException("Conversion to string is not supported.");
    }

    public ZipArchive GetZipArchive(ZipArchiveMode mode)
    {
      return _stream != null && (_stream.Length > 0 || mode != ZipArchiveMode.Read) ? new ZipArchive(_stream, mode, true) : null;
    }

    [SqlMethod(IsMutator = true)]
    public void AddEntry([SqlFacet(MaxSize = 255)]SqlString entryName, [SqlFacet(MaxSize = -1)] SqlBytes entryData, [DefaultValue("NULL")] SqlInt32 compressionLevel)
    {
      if (entryName.IsNull || entryData.IsNull)
        return;

      using (ZipArchive za = new ZipArchive(_stream, ZipArchiveMode.Update, true))
      {
        var zipEntry = compressionLevel.IsNull ? za.CreateEntry(entryName.Value) : za.CreateEntry(entryName.Value, (CompressionLevel)compressionLevel.Value);
        using (var es = zipEntry.Open())
        {
          if (entryData.Storage == StorageState.Buffer)
            es.WriteBytes(entryData.Buffer);
          else
            entryData.Stream.Locate(0L).CopyTo(es);
        }
      }
    }

    [SqlMethod(IsMutator = true)]
    public void DeleteEntry(SqlString entryName)
    {
      if (entryName.IsNull)
        return;

      using (ZipArchive za = new ZipArchive(_stream, ZipArchiveMode.Update, true))
      {
        var zipEntry = za.GetEntry(entryName.Value);
        if (zipEntry != null)
          zipEntry.Delete();
      }
    }

    [SqlMethod]
    public SqlBytes GetEntry(SqlString entryName)
    {
      if (entryName.IsNull)
        return SqlBytes.Null;

      using (ZipArchive za = new ZipArchive(_stream, ZipArchiveMode.Update, true))
      {
        var zipEntry = za.GetEntry(entryName.Value);
        if (zipEntry == null)
          return SqlBytes.Null;

        var ms = new MemoryStream();
        try
        {
          using (var es = zipEntry.Open())
          {
            es.CopyTo(ms);
            return new SqlBytes(ms);
          }
        }
        catch (Exception)
        {
          ms.Dispose();
          throw;
        }
      }
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      rd.BaseStream.Locate(0L).CopyTo(_stream.Resize(0L));
    }

    public void Write(BinaryWriter wr)
    {
      _stream.Locate(0L).CopyTo(wr.BaseStream);
    }

    #endregion
  }
}
