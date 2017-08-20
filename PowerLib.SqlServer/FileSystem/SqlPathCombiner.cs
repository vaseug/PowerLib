using System;
using System.IO;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.IO;
using PowerLib.System.Data.SqlTypes.IO;

namespace PowerLib.SqlServer.FileSystem
{
  [Serializable]
  [SqlUserDefinedAggregate(Format.UserDefined, Name = "pathCombine", MaxByteSize = -1)]
  public sealed class SqlPathCombiner : IBinarySerialize
  {
    private string _path;

    #region Methods

    public void Init()
    {
      _path = null;
    }

    public void Accumulate(SqlString part)
    {
      _path = _path != null ? part.IsNull ? null : Path.Combine(_path, part.Value) : part.IsNull ? null : part.Value;
    }

    public void Merge(SqlPathCombiner combiner)
    {
      _path = _path != null && combiner._path != null ? Path.Combine(_path, combiner._path) : null;
    }

    public SqlString Terminate()
    {
      return _path ?? SqlString.Null;
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _path = rd.ReadObject(r => r.ReadString(Encoding.GetEncoding(rd.ReadUInt16()), SizeEncoding.VB), r => r.ReadBoolean(TypeCode.Byte));
    }

    public void Write(BinaryWriter wr)
    {
      wr.Write(_path, (w, t) =>
      {
        var encoding = SqlRuntime.TextEncoding;
        wr.Write((UInt16)encoding.CodePage);
        wr.Write(_path, encoding, SizeEncoding.VB);
      }, (w, f) => w.Write(f, TypeCode.Byte));
    }

    #endregion
  }
}
