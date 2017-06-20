using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.SqlServer.Collections
{
  [SqlUserDefinedAggregate(Format.UserDefined, Name = "sfCollect", MaxByteSize = -1)]
  public sealed class SqlSingleCollector : IBinarySerialize
  {
    private List<Single?> _list;

    #region Methods

    public void Init()
    {
      if (_list == null)
        _list = new List<Single?>();
      else
        _list.Clear();
    }

    public void Accumulate(SqlSingle input)
    {
      if (_list != null)
        _list.Add(!input.IsNull ? input.Value : default(Single?));
    }

    public void Merge(SqlSingleCollector aggregator)
    {
      if (_list != null && aggregator._list != null)
        _list.AddRange(aggregator._list);
    }

    public SqlBytes Terminate()
    {
      if (_list == null)
        return SqlBytes.Null;
      var result = new SqlBytes(new MemoryStream());
      using (new NulSingleStreamedArray(result.Stream, SizeEncoding.B4, true, _list, true, false)) ;
      return result;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _list = new List<Single?>(new NulSingleStreamedArray(rd.BaseStream, true, false));
    }

    public void Write(BinaryWriter wr)
    {
      if (_list != null)
        using (var ms = new MemoryStream())
        using (var sa = new NulSingleStreamedArray(ms, SizeEncoding.B4, true, _list, true, false))
          wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
