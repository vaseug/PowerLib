using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace PowerLib.System.Data.SqlTypes.Collections
{
  [SqlUserDefinedType(Format.UserDefined, Name = "NameValueCollection", IsByteOrdered = true, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlNameValueCollection : INullable, IBinarySerialize
  {
    private static readonly SqlNameValueCollection @null = new SqlNameValueCollection();

    private NameValueCollection _coll;

    #region Contructors

    public SqlNameValueCollection()
    {
      _coll = null;
    }

    public SqlNameValueCollection(NameValueCollection coll)
    {
      _coll = coll;
    }

    #endregion
    #region Properties

    public NameValueCollection Value
    {
      get { return _coll; }
      set { _coll = value; }
    }

    public static SqlNameValueCollection Null
    {
      get { return @null; }
    }

    public bool IsNull
    {
      get { return _coll == null; }
    }

    public SqlInt32 Count
    {
      get { return _coll != null ? _coll.Count : SqlInt32.Null; }
    }

    #endregion
    #region Methods

    public static SqlNameValueCollection Parse(SqlString s)
    {
      if (s.IsNull)
        return Null;

      throw new NotSupportedException();
    }

    public override String ToString()
    {
      return _coll.ToString();
    }

    [SqlMethod]
    public SqlNameValueCollection Clear()
    {
      _coll.Clear();
      return this;
    }

    [SqlMethod]
    public SqlNameValueCollection AddItem(SqlString name, SqlString value)
    {
      _coll.Add(name.IsNull ? default(String) : name.Value, value.IsNull ? default(String) : value.Value);
      return this;
    }

    [SqlMethod]
    public SqlNameValueCollection RemoveItem(SqlString name)
    {
      _coll.Remove(name.IsNull ? default(String) : name.Value);
      return this;
    }

    [SqlMethod]
    public SqlNameValueCollection SetItem(SqlString name, SqlString value)
    {
      _coll.Set(name.IsNull ? default(String) : name.Value, value.IsNull ? default(String) : value.Value);
      return this;
    }

    [SqlMethod]
    public SqlNameValueCollection AddRange(SqlNameValueCollection coll)
    {
      if (!coll.IsNull)
        _coll.Add(coll._coll);
      return this;
    }

    [SqlMethod]
    public SqlString GetItem(SqlString name)
    {
      return _coll[name.IsNull ? default(String) : name.Value] ?? SqlString.Null;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _coll = (NameValueCollection)new BinaryFormatter().Deserialize(rd.BaseStream);
    }

    public void Write(BinaryWriter wr)
    {
      new BinaryFormatter().Serialize(wr.BaseStream, _coll);
    }

    #endregion
  }
}
