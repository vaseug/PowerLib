using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PowerLib.System.Collections;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.Adapters;

namespace PowerLib.System.Data.Entity
{
  #region Regular expression rows

  [ComplexType]
  public sealed class RegexMatchRow
  {
    public int MatchNumber
    {
      get;
      set;
    }

    public int GroupNumber
    {
      get;
      set;
    }

    public int CaptureNumber
    {
      get;
      set;
    }

    public bool GroupSuccess
    {
      get;
      set;
    }

    public string GroupName
    {
      get;
      set;
    }

    public int Index
    {
      get;
      set;
    }

    public int Length
    {
      get;
      set;
    }

    public string Value
    {
      get;
      set;
    }
  }

  #endregion
  #region Compression rows

  [ComplexType]
  public sealed class ZipArchiveEntryRow
  {
    public string Name
    {
      get;
      set;
    }

    public string FullName
    {
      get;
      set;
    }

    public long Length
    {
      get;
      set;
    }

    public long CompressedLength
    {
      get;
      set;
    }

    public DateTimeOffset LastWriteTime
    {
      get;
      set;
    }
  }

  #endregion
  #region Base types rows

  [ComplexType]
  public sealed class BinaryRow
  {
    public Byte[] Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class StringRow
  {
    public String Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class BooleanRow
  {
    public Boolean? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class ByteRow
  {
    public Byte? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class Int16Row
  {
    public Int16? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class Int32Row
  {
    public Int32? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class Int64Row
  {
    public Int64? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class SingleRow
  {
    public Single? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class DoubleRow
  {
    public Double? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class DecimalRow
  {
    public Decimal? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class DateTimeRow
  {
    public DateTime? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class GuidRow
  {
    public Guid? Value
    {
      get;
      set;
    }
  }

  #endregion
  #region Base types indexed rows

  [ComplexType]
  public sealed class IndexedBinaryRow
  {
    public int Index
    {
      get;
      set;
    }

    public Byte[] Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class IndexedStringRow
  {
    public int Index
    {
      get;
      set;
    }

    public String Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class IndexedBooleanRow
  {
    public int Index
    {
      get;
      set;
    }

    public Boolean? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class IndexedByteRow
  {
    public int Index
    {
      get;
      set;
    }

    public Byte? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class IndexedInt16Row
  {
    public int Index
    {
      get;
      set;
    }

    public Int16? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class IndexedInt32Row
  {
    public int Index
    {
      get;
      set;
    }

    public Int32? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class IndexedInt64Row
  {
    public int Index
    {
      get;
      set;
    }

    public Int64? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class IndexedSingleRow
  {
    public int Index
    {
      get;
      set;
    }

    public Single? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class IndexedDoubleRow
  {
    public int Index
    {
      get;
      set;
    }

    public Double? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class IndexedDecimalRow
  {
    public int Index
    {
      get;
      set;
    }

    public Decimal? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class IndexedDateTimeRow
  {
    public int Index
    {
      get;
      set;
    }

    public DateTime? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class IndexedGuidRow
  {
    public int Index
    {
      get;
      set;
    }

    public Guid? Value
    {
      get;
      set;
    }
  }

  #endregion
  #region Base types regular indexed rows

  [ComplexType]
  public sealed class RegularIndexedBinaryRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(t => (Int32?)t).ToArray(), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Byte[] Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class RegularIndexedStringRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(t => (Int32?)t).ToArray(), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public String Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class RegularIndexedBooleanRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(t => (Int32?)t).ToArray(), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Boolean? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class RegularIndexedByteRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(t => (Int32?)t).ToArray(), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Byte? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class RegularIndexedInt16Row
  {
    [Column]
    public int FlatIndex
    {
      get;
      set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Int16? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class RegularIndexedInt32Row
  {
    [Column]
    public int FlatIndex
    {
      get;
      set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Int32? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class RegularIndexedInt64Row
  {
    [Column]
    public int FlatIndex
    {
      get;
      set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Int64? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class RegularIndexedSingleRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Single? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class RegularIndexedDoubleRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Double? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class RegularIndexedDecimalRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Decimal? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class RegularIndexedDateTimeRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public DateTime? Value
    {
      get;
      set;
    }
  }

  [ComplexType]
  public sealed class RegularIndexedGuidRow
  {
    [Column]
    public int FlatIndex
    {
      get;
      set;
    }

    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }

    [Column]
    public Guid? Value
    {
      get;
      set;
    }
  }

  #endregion
}
