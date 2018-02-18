using System;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.SqlTypes.Collections;

namespace PowerLib.SqlServer.Collections
{
  [Serializable]
  [SqlUserDefinedAggregate(Format.UserDefined, Name = "nvCollect", MaxByteSize = -1)]
  public sealed class SqlNameValueCollector : IBinarySerialize
  {
    private NameValueCollection _coll;

    #region Methods

    public void Init()
    {
      if (_coll == null)
        _coll = new NameValueCollection();
      else
        _coll.Clear();
    }

    public void Accumulate([SqlFacet(MaxSize = -1)] SqlString name, [SqlFacet(MaxSize = -1)] SqlString value)
    {
      if (_coll != null)
        _coll.Add(!name.IsNull ? name.Value : null, !value.IsNull ? value.Value : null);
    }

    public void Merge(SqlNameValueCollector aggregator)
    {
      if (_coll != null && aggregator._coll != null)
        _coll.Add(aggregator._coll);
    }

    public SqlNameValueCollection Terminate()
    {
      return _coll != null ? new SqlNameValueCollection(_coll) : SqlNameValueCollection.Null;
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
