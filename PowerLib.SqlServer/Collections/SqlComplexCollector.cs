using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Numerics;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.SqlServer.Collections
{
  [SqlUserDefinedAggregate(Format.UserDefined, Name = "cxCollect", MaxByteSize = -1)]
  public sealed class SqlComplexCollector : IBinarySerialize
  {
    private List<Complex?> _list;

    #region Methods

    public void Init()
    {
      if (_list == null)
        _list = new List<Complex?>();
      else
        _list.Clear();
    }

    public void Accumulate(SqlComplex input)
    {
      if (_list != null)
        _list.Add(!input.IsNull ? input.Value : default(Complex?));
    }

    public void Merge(SqlComplexCollector aggregator)
    {
      if (_list != null && aggregator._list != null)
        _list.AddRange(aggregator._list);
    }

    public SqlBytes Terminate()
    {
      if (_list == null)
        return SqlBytes.Null;
      var result = new SqlBytes(new MemoryStream());
      using (new NulComplexStreamedArray(result.Stream, SizeEncoding.B4, true, _list, true, false)) ;
      return result;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulComplexStreamedArray(rd.BaseStream, true, true))
        _list = sa.ToList();
    }

    public void Write(BinaryWriter wr)
    {
      if (_list != null)
        using (var ms = new MemoryStream())
        using (var sa = new NulComplexStreamedArray(ms, SizeEncoding.B4, true, _list, true, false))
          wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
