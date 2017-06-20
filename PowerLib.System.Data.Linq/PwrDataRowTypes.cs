using System;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Xml.Linq;
using PowerLib.System.Collections;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.Adapters;

namespace PowerLib.System.Data.Linq
{
  #region Regular expression rows

  [Table]
  public sealed class RegexMatchRow
  {
    [Column]
    public int MatchNumber
    {
      get;
      private set;
    }

    [Column]
    public int GroupNumber
    {
      get;
      private set;
    }

    [Column]
    public int CaptureNumber
    {
      get;
      private set;
    }

    [Column]
    public bool GroupSuccess
    {
      get;
      private set;
    }

    [Column]
    public string GroupName
    {
      get;
      private set;
    }

    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public int Length
    {
      get;
      private set;
    }

    [Column]
    public string Value
    {
      get;
      private set;
    }
  }

  #endregion
  #region Compression rows

  [Table]
  public sealed class ZipArchiveEntryRow
  {
    [Column]
    public string Name
    {
      get;
      private set;
    }

    [Column]
    public string FullName
    {
      get;
      private set;
    }

    [Column]
    public long Length
    {
      get;
      private set;
    }

    [Column]
    public long CompressedLength
    {
      get;
      private set;
    }

    [Column]
    public DateTimeOffset LastWriteTime
    {
      get;
      private set;
    }
  }

  #endregion
  #region Base types rows

  [Table]
  public sealed class BinaryRow
  {
    [Column]
    public Byte[] Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class StringRow
  {
    [Column]
    public String Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class BooleanRow
  {
    [Column]
    public Boolean? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class ByteRow
  {
    [Column]
    public Byte? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class Int16Row
  {
    [Column]
    public Int16? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class Int32Row
  {
    [Column]
    public Int32? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class Int64Row
  {
    [Column]
    public Int64? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class SingleRow
  {
    [Column]
    public Single? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class DoubleRow
  {
    [Column]
    public Double? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class DecimalRow
  {
    [Column]
    public Decimal? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class DateTimeRow
  {
    [Column]
    public DateTime? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class GuidRow
  {
    [Column]
    public Guid? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class XmlRow
  {
    [Column]
    public XElement Value
    {
      get;
      private set;
    }
  }

  #endregion
  #region Base types indexed rows

  [Table]
  public sealed class IndexedBinaryRow
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public Byte[] Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class IndexedStringRow
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public String Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class IndexedBooleanRow
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public Boolean? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class IndexedByteRow
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public Byte? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class IndexedInt16Row
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public Int16? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class IndexedInt32Row
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public Int32? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class IndexedInt64Row
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public Int64? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class IndexedSingleRow
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public Single? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class IndexedDoubleRow
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public Double? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class IndexedDecimalRow
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public Decimal? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class IndexedDateTimeRow
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public DateTime? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class IndexedGuidRow
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public Guid? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class IndexedXmlRow
  {
    [Column]
    public int Index
    {
      get;
      private set;
    }

    [Column]
    public XElement Value
    {
      get;
      private set;
    }
  }

  #endregion
  #region Base types regular indexed rows

  [Table]
  public sealed class RegularIndexedBinaryRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(t => (Int32?)t).ToArray(), false, false));

    [Column(Name = "DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Byte[] Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class RegularIndexedStringRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(t => (Int32?)t).ToArray(), false, false));

    [Column(Name = "DimIndices")]
    private Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public String Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class RegularIndexedBooleanRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(t => (Int32?)t).ToArray(), false, false));

    [Column(Name = "DimIndices")]
    private Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Boolean? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class RegularIndexedByteRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(t => (Int32?)t).ToArray(), false, false));

    [Column(Name = "DimIndices")]
    private Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Byte? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class RegularIndexedInt16Row
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column(Name = "DimIndices")]
    private Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Int16? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class RegularIndexedInt32Row
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column(Name = "DimIndices")]
    private Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Int32? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class RegularIndexedInt64Row
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column(Name = "DimIndices")]
    private Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Int64? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class RegularIndexedSingleRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column(Name = "DimIndices")]
    private Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Single? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class RegularIndexedDoubleRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column(Name = "DimIndices")]
    private Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Double? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class RegularIndexedDecimalRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column(Name = "DimIndices")]
    private Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Decimal? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class RegularIndexedDateTimeRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column(Name = "DimIndices")]
    private Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public DateTime? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class RegularIndexedGuidRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column(Name = "DimIndices")]
    private Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Guid? Value
    {
      get;
      private set;
    }
  }

  [Table]
  public sealed class RegularIndexedXmlRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      private set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column(Name = "DimIndices")]
    private Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public XElement Value
    {
      get;
      private set;
    }
  }

  #endregion
}
