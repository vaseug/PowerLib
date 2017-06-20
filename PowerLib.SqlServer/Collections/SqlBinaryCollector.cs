using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.IO.Streamed.Typed;

namespace PowerLib.SqlServer.Collections
{

  [SqlUserDefinedAggregate(Format.UserDefined, Name = "binCollect", MaxByteSize = -1)]
  public sealed class SqlBinaryCollector : IBinarySerialize
  {
    private List<Byte[]> _list;

    #region Methods

    public void Init()
    {
      if (_list == null)
        _list = new List<Byte[]>();
      else
        _list.Clear();
    }

    public void Accumulate([SqlFacet(MaxSize = -1)] SqlBinary input)
    {
      if (_list != null)
        _list.Add(!input.IsNull ? input.Value : null);
    }

    public void Merge(SqlBinaryCollector aggregator)
    {
      if (_list != null && aggregator._list != null)
        _list.AddRange(aggregator._list);
    }

    public SqlBytes Terminate()
    {
      if (_list == null)
        return SqlBytes.Null;
      var result = new SqlBytes(new MemoryStream());
      using (new BinaryStreamedArray(result.Stream, SizeEncoding.B4, SizeEncoding.B4, _list, true, false)) ;
      return result;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new BinaryStreamedArray(rd.BaseStream, true, false))
        _list = sa.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      if (_list != null)
        using (var ms = new MemoryStream())
        using (var sa = new BinaryStreamedArray(wr.BaseStream, SizeEncoding.B4, SizeEncoding.B4, _list, true, false))
          wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
