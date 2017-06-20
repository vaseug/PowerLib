﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Collections;
using PowerLib.System.IO.Streamed.Typed;
using PowerLib.System.Numerics;
using PowerLib.System.Data.SqlTypes.Numerics;

namespace PowerLib.SqlServer.Collections
{
  [SqlUserDefinedAggregate(Format.UserDefined, Name = "saCollect", MaxByteSize = -1)]
  public sealed class SqlSexagesimalAngleCollector : IBinarySerialize
  {
    private List<SexagesimalAngle?> _list;

    #region Methods

    public void Init()
    {
      if (_list == null)
        _list = new List<SexagesimalAngle?>();
      else
        _list.Clear();
    }

    public void Accumulate(SqlSexagesimalAngle input)
    {
      if (_list != null)
        _list.Add(!input.IsNull ? input.Value : default(SexagesimalAngle?));
    }

    public void Merge(SqlSexagesimalAngleCollector aggregator)
    {
      if (_list != null && aggregator._list != null)
        _list.AddRange(aggregator._list);
    }

    public SqlBytes Terminate()
    {
      if (_list == null)
        return SqlBytes.Null;
      var result = new SqlBytes(new MemoryStream());
      using (new NulInt32StreamedArray(result.Stream, SizeEncoding.B4, true, _list.Select(t => t.HasValue ? t.Value.Units : default(Int32?)).Counted(_list.Count), true, false)) ;
      return result;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      using (var sa = new NulInt32StreamedArray(rd.BaseStream, true, false))
        _list = sa.Select(t => t.HasValue ? new SexagesimalAngle(t.Value) : default(SexagesimalAngle?)).ToList();
    }

    public void Write(BinaryWriter wr)
    {
      if (_list != null)
        using (var ms = new MemoryStream())
        using (var sa = new NulInt32StreamedArray(ms, SizeEncoding.B4, true, _list.Select(t => t.HasValue ? t.Value.Units : default(Int32?)).Counted(_list.Count), true, false))
          wr.Write(ms.GetBuffer(), 0, (int)ms.Length);
    }

    #endregion
  }
}
