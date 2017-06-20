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
  [SqlUserDefinedAggregate(Format.UserDefined, Name = "strJoin", MaxByteSize = -1)]
  public class SqlStringJoinAggregator : IBinarySerialize
  {
    private State _state;
    private String _separator;
    private StringBuilder _sb;

    #region Methods

    public void Init()
    {
      _state = State.Empty;
      _separator = string.Empty;
      if (_sb == null)
        _sb = new StringBuilder();
      else
        _sb.Clear();
    }

    public void Accumulate([SqlFacet(MaxSize = -1)] SqlString input,  SqlString separator)
    {
      if (_state == State.IsNull)
        return;
      if (input.IsNull)
      {
        _separator = null;
        _sb.Clear();
        _state = State.IsNull;
      }
      else
      {
        if (!separator.IsNull)
          _separator = separator.Value ?? string.Empty;
        if (_state == State.Valued)
          _sb.Append(_separator);
        else
          _state = State.Valued;
        _sb.Append(input.Value ?? string.Empty);
        if (_state == State.Empty)
          _state = State.Valued;
      }
    }

    public void Merge(SqlStringJoinAggregator join)
    {
      if (_state == State.IsNull)
        return;
      if (join._state == State.IsNull)
      {
        _separator = null;
        _sb.Clear();
        _state = State.IsNull;
      }
      else if (join._state == State.Valued)
      {
        if (_state == State.Valued)
          _sb.Append(_separator);
        _sb.Append(join._sb.ToString());
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
      _separator = null;
      if (_sb == null)
        _sb = new StringBuilder();
      else
        _sb.Clear();
      switch (_state = rd.ReadEnum<State>())
      {
        case State.Valued:
          _separator = rd.ReadString(Encoding.Unicode, SizeEncoding.VB);
          _sb.Append(rd.ReadString(Encoding.Unicode, SizeEncoding.VB));
          break;
        case State.Empty:
          _separator = string.Empty;
          break;
      }
    }

    void IBinarySerialize.Write(BinaryWriter writer)
    {
      writer.WriteEnum(_state);
      if (_state == State.Valued)
      {
        writer.Write(_separator, Encoding.Unicode, SizeEncoding.VB);
        writer.Write(_sb.ToString(), Encoding.Unicode, SizeEncoding.VB);
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
