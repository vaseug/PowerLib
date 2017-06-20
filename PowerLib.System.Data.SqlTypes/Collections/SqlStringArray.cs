using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "StringArray", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlStringArray : INullable, IBinarySerialize
  {
    private String[] _array;
    private Encoding _encoding;

    #region Contructors

    public SqlStringArray()
    {
      _array = null;
      _encoding = null;
    }

    public SqlStringArray(int length, Encoding encoding)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      _array = new String[length];
      _encoding = encoding;
    }

    public SqlStringArray(IEnumerable<String> coll, Encoding encoding)
    {
      _array = coll != null ? coll.ToArray() : null;
      _encoding = encoding;
    }

    private SqlStringArray(String[] array, Encoding encoding)
    {
      _array = array;
      _encoding = encoding;
    }

    #endregion
    #region Properties

    public String[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public static SqlStringArray Null
    {
      get { return new SqlStringArray(); }
    }

    public bool IsNull
    {
      get { return _array == null; }
    }

    public SqlInt32 Length
    {
      get { return _array != null ? _array.Length : SqlInt32.Null; }
    }

    public SqlInt32 Codepage
    {
      get { return _encoding != null ? _encoding.CodePage : SqlInt32.Null; }
    }

    #endregion
    #region Methods

    public static SqlStringArray Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      return new SqlStringArray(SqlFormatting.ParseArray<String>(s.Value,
        t => !t.Equals(SqlFormatting.NullText, StringComparison.InvariantCultureIgnoreCase) ? SqlFormatting.Unquote(t) : default(String)), Encoding.Unicode);
    }

    public override String ToString()
    {
      return SqlFormatting.Format(_array, t => t != null ? SqlFormatting.Quote(t) : SqlString.Null.ToString());
    }

    [SqlMethod]
    [return: SqlFacet(MaxSize = -1)]
    public SqlString GetItem(SqlInt32 index)
    {
      return !index.IsNull && _array[index.Value] != null ? _array[index.Value] : SqlString.Null;
    }

    [SqlMethod(IsMutator = true)]
    public void SetItem([SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 index)
    {
      if (!index.IsNull)
        _array[index.Value] = !value.IsNull ? value.Value : null;
    }

    [SqlMethod]
    public SqlStringArray GetRange(SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      return new SqlStringArray(_array.Range(indexValue, countValue), _encoding);
    }

    [SqlMethod(IsMutator = true)]
    public void SetRange(SqlStringArray array, SqlInt32 index)
    {
      if (array.IsNull)
        return;

      int indexValue = index.IsNull ? _array.Length - Comparable.Min(_array.Length, array._array.Length) : index.Value;
      _array.Fill(i => array._array[i - indexValue]);
    }

    [SqlMethod(IsMutator = true)]
    public void FillRange([SqlFacet(MaxSize = -1)] SqlString value, SqlInt32 index, SqlInt32 count)
    {
      int indexValue = !index.IsNull ? index.Value : count.IsNull ? 0 : _array.Length - count.Value;
      int countValue = !count.IsNull ? count.Value : index.IsNull ? 0 : _array.Length - index.Value;
      _array.Fill(!value.IsNull ? value.Value : null, indexValue, countValue);
    }

    [SqlMethod]
    public SqlStringCollection ToCollection()
    {
      return new SqlStringCollection(_array, _encoding);
    }

    #endregion
    #region Operators

    public static implicit operator byte[] (SqlStringArray array)
    {
      using (var ms = new MemoryStream())
      using (new StringStreamedArray(ms, SizeEncoding.B4, SizeEncoding.B4, Encoding.Unicode, null, array._array, true, false))
        return ms.ToArray();
    }

    public static explicit operator SqlStringArray(byte[] buffer)
    {
      using (var ms = new MemoryStream(buffer))
      using (var sa = new StringStreamedArray(ms, null, true, false))
        return new SqlStringArray(sa.ToArray(), sa.Encoding);
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new StringStreamedArray(rd.BaseStream, null, true, false))
      {
        _array = sa.ToArray();
        _encoding = sa.Encoding;
      }
    }

    public void Write(BinaryWriter wr)
    {
      using (var ms = new MemoryStream())
      using (var sa = new StringStreamedArray(ms, SizeEncoding.B4, SizeEncoding.B4, _encoding, null, _array, true, false))
        wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
