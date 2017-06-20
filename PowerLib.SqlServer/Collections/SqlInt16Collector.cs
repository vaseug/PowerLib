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
  [SqlUserDefinedAggregate(Format.UserDefined, Name = "siCollect", MaxByteSize = -1)]
  public sealed class SqlInt16Collector : IBinarySerialize
  {
    private List<Int16?> _list;

    #region Methods

    public void Init()
    {
      if (_list == null)
        _list = new List<Int16?>();
      else
        _list.Clear();
    }

    public void Accumulate(SqlInt16 input)
    {
      if (_list != null)
        _list.Add(!input.IsNull ? input.Value : default(Int16?));
    }

    public void Merge(SqlInt16Collector aggregator)
    {
      if (_list != null && aggregator._list != null)
        _list.AddRange(aggregator._list);
    }

    public SqlBytes Terminate()
    {
      if (_list == null)
        return SqlBytes.Null;
      var result = new SqlBytes(new MemoryStream());
      using (new NulInt16StreamedArray(result.Stream, SizeEncoding.B4, true, _list, true, false)) ;
      return result;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulInt16StreamedArray(rd.BaseStream, true, false))
        _list = sa.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      if (_list != null)
        using (var ms = new MemoryStream())
        using (var sa = new NulInt16StreamedArray(ms, SizeEncoding.B4, true, _list, true, false))
          wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
