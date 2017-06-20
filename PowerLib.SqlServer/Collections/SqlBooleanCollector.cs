using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.SqlServer.Collections
{
  [SqlUserDefinedAggregate(Format.UserDefined, Name = "bCollect", MaxByteSize = -1)]
  public sealed class SqlBooleanCollector : IBinarySerialize
  {
    private List<Boolean?> _list;

    #region Methods

    public void Init()
    {
      if (_list == null)
        _list = new List<Boolean?>();
      else
        _list.Clear();
    }

    public void Accumulate(SqlBoolean input)
    {
      if (_list != null)
        _list.Add(!input.IsNull ? input.Value : default(Boolean?));
    }

    public void Merge(SqlBooleanCollector aggregator)
    {
      if (_list != null && aggregator._list != null)
        _list.AddRange(aggregator._list);
    }

    public SqlBytes Terminate()
    {
      if (_list == null)
        return SqlBytes.Null;
      var result = new SqlBytes(new MemoryStream());
      using (new NulBooleanStreamedArray(result.Stream, SizeEncoding.B4, _list, true, false)) ;
      return result;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulBooleanStreamedArray(rd.BaseStream, true, true))
        _list = sa.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      if (_list != null)
        using (var ms = new MemoryStream())
        using (var sa = new NulBooleanStreamedArray(ms, SizeEncoding.B4, _list, true, false))
          wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
