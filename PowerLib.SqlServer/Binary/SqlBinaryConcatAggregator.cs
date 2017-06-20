using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.Collections;
using PowerLib.System.IO;

namespace PowerLib.SqlServer.Text
{
  [Serializable]
  [SqlUserDefinedAggregate(Format.UserDefined, Name = "binConcat", MaxByteSize = -1)]
  public class SqlBinaryConcatAggregator : IBinarySerialize
  {
    private State _state;
    private List<Byte> _list;

    #region Methods

    public void Init()
    {
      _state = State.Empty;
      if (_list == null)
        _list = new List<byte>();
      else
        _list.Clear();
    }

    public void Accumulate([SqlFacet(MaxSize = -1)] SqlBytes input)
    {
      if (_state == State.IsNull)
        return;
      if (input.IsNull)
      {
        _list.Clear();
        _state = State.IsNull;
      }
      else
      {
        _list.AddRange(input.Buffer.Enumerate(0, (int)input.Length));
        if (_state == State.Empty)
          _state = State.Valued;
      }
    }

    public void Merge(SqlBinaryConcatAggregator concat)
    {
      if (_state == State.IsNull)
        return;
      if (concat._state == State.IsNull)
      {
        _list.Clear();
        _state = State.IsNull;
      }
      else if (concat._state == State.Valued)
      {
        _list.AddRange(concat._list);
        if (_state == State.Empty)
          _state = State.Valued;
      }
    }

    [return: SqlFacet(MaxSize = -1)]
    public SqlBytes Terminate()
    {
      return _state == State.Valued ? new SqlBytes(_list.ToArray()) : SqlBytes.Null;
    }

    #endregion
    #region IBinarySerialize implementation

    void IBinarySerialize.Read(BinaryReader rd)
    {
      if (_list == null)
        _list = new List<Byte>();
      else
        _list.Clear();
      if ((_state = rd.ReadEnum<State>()) == State.Valued)
        _list.AddRange(rd.ReadCollection<Byte>(tr => tr.ReadByte(), rd.ReadInt32()));
    }

    void IBinarySerialize.Write(BinaryWriter wr)
    {
      wr.WriteEnum(_state);
      if (_state == State.Valued)
      {
        wr.Write(_list.Count);
        wr.WriteCollection(_list, (tw, t) => tw.Write(t));
      }
    }

    #endregion
    #region Embedded types

    private enum State : byte
    {
      Empty = 0,
      Valued = 1,
      IsNull = 2
    }

    #endregion
  }
}
