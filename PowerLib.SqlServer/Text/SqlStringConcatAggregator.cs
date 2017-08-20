using System;
using System.Text;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.IO;

namespace PowerLib.SqlServer.Text
{
  [Serializable]
  [SqlUserDefinedAggregate(Format.UserDefined, Name = "strConcat", MaxByteSize = -1)]
  public class SqlStringConcatAggregator : IBinarySerialize
  {
    private State _state;
    private StringBuilder _sb;

    #region Methods

    public void Init()
    {
      _state = State.Empty;
      if (_sb == null)
        _sb = new StringBuilder();
      else
        _sb.Clear();
    }

    public void Accumulate([SqlFacet(MaxSize = -1)] SqlString input)
    {
      if (_state == State.IsNull)
        return;
      if (input.IsNull)
      {
        _sb.Clear();
        _state = State.IsNull;
      }
      else
      {
        _sb.Append(input.Value ?? string.Empty);
        if (_state == State.Empty)
          _state = State.Valued;
      }
    }

    public void Merge(SqlStringConcatAggregator concat)
    {
      if (_state == State.IsNull)
        return;
      if (concat._state == State.IsNull)
      {
        _sb.Clear();
        _state = State.IsNull;
      }
      else if (concat._state == State.Valued)
      {
        _sb.Append(concat._sb.ToString());
        if (_state == State.Empty)
          _state = State.Valued;
      }
    }

    [return: SqlFacet(MaxSize = -1)]
    public SqlString Terminate()
    {
      return _state == State.Valued ? new SqlString(_sb.ToString()) : SqlString.Null;
    }

    #endregion
    #region IBinarySerialize implementation

    void IBinarySerialize.Read(BinaryReader rd)
    {
      if (_sb == null)
        _sb = new StringBuilder();
      else
        _sb.Clear();
      if ((_state = rd.ReadEnum<State>()) == State.Valued)
        _sb.Append(rd.ReadString(SqlRuntime.TextEncoding, SizeEncoding.VB));
    }

    void IBinarySerialize.Write(BinaryWriter writer)
    {
      writer.WriteEnum(_state);
      if (_state == State.Valued)
        writer.Write(_sb.ToString(), SqlRuntime.TextEncoding, SizeEncoding.VB);
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
