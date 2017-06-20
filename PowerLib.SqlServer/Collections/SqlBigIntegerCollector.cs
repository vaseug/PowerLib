using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.SqlServer.Collections
{
  [SqlUserDefinedAggregate(Format.UserDefined, Name = "hiCollect", MaxByteSize = -1)]
  public sealed class SqlBigIntegerCollector : IBinarySerialize
  {
    private List<BigInteger?> _list;

    #region Methods

    public void Init()
    {
      if (_list == null)
        _list = new List<BigInteger?>();
      else
        _list.Clear();
    }

    public void Accumulate(SqlBigInteger input)
    {
      if (_list != null)
        _list.Add(!input.IsNull ? input.Value : default(BigInteger?));
    }

    public void Merge(SqlBigIntegerCollector aggregator)
    {
      if (_list != null && aggregator._list != null)
        _list.AddRange(aggregator._list);
    }

    public SqlBytes Terminate()
    {
      if (_list == null)
        return SqlBytes.Null;
      var result = new SqlBytes(new MemoryStream());
      using (new BinaryStreamedArray(result.Stream, SizeEncoding.B4, SizeEncoding.B4, _list.Select(t => t.HasValue ? t.Value.ToByteArray() : default(byte[])).Counted(_list.Count), true, false)) ;
      return result;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new BinaryStreamedArray(rd.BaseStream, true, false))
        _list = sa.Select(t => t != null ? new BigInteger(t) : default(BigInteger?)).ToList();
    }

    public void Write(BinaryWriter wr)
    {
      if (_list != null)
        using (var ms = new MemoryStream())
        using (var sa = new BinaryStreamedArray(wr.BaseStream, SizeEncoding.B4, SizeEncoding.B4, _list.Select(t => t.HasValue ? t.Value.ToByteArray() : default(byte[])).Counted(_list.Count), true, false))
          wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
