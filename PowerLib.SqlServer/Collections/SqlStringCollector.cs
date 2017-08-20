using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.SqlServer.Collections
{
  [Serializable]
  [SqlUserDefinedAggregate(Format.UserDefined, Name = "strCollect", MaxByteSize = -1)]
  public sealed class SqlStringCollector : IBinarySerialize
  {
    private List<String> _list;

    #region Methods

    public void Init()
    {
      if (_list == null)
        _list = new List<String>();
      else
        _list.Clear();
    }

    public void Accumulate([SqlFacet(MaxSize = -1)] SqlString input)
    {
      if (_list != null)
        _list.Add(!input.IsNull ? input.Value : null);
    }

    public void Merge(SqlStringCollector aggregator)
    {
      if (_list != null && aggregator._list != null)
        _list.AddRange(aggregator._list);
    }

    public SqlBytes Terminate()
    {
      if (_list == null)
        return SqlBytes.Null;
      var result = new SqlBytes(new MemoryStream());
      using (new StringStreamedArray(result.Stream, SizeEncoding.B4, SizeEncoding.B4, SqlRuntime.TextEncoding, null, _list, true, false)) ;
      return result;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new StringStreamedArray(rd.BaseStream, null, true, false))
        _list = sa.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      if (_list != null)
        using (var ms = new MemoryStream())
        using (var sa = new StringStreamedArray(ms, SizeEncoding.B4, SizeEncoding.B4, SqlRuntime.TextEncoding, null, _list, true, false))
          wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
