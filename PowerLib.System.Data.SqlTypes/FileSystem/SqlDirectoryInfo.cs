using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using System.Xml;
using Microsoft.SqlServer.Server;
using PowerLib.System.Linq;
using PowerLib.System.IO;
using PowerLib.System.Text;
using PowerLib.System.Data.SqlTypes.IO;
using PowerLib.SqlServer;

namespace PowerLib.System.Data.SqlTypes.FileSystem
{
  [SqlUserDefinedType(Format.UserDefined, Name = "DirectoryInfo", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlDirectoryInfo : INullable, IBinarySerialize
  {
    private static readonly SqlDirectoryInfo @null = new SqlDirectoryInfo();

    private DirectoryInfo _di;

    #region Constructor

    public SqlDirectoryInfo()
    {
      _di = null;
    }

    public SqlDirectoryInfo(string path)
    {
      _di = new DirectoryInfo(path);
    }

    public SqlDirectoryInfo(DirectoryInfo fi)
    {
      _di = fi;
    }

    #endregion
    #region Properties

    public DirectoryInfo DirectoryInfo
    {
      get { return _di; }
    }

    public static SqlDirectoryInfo Null
    {
      get { return @null; }
    }

    public bool IsNull
    {
      get { return _di == null; }
    }

    #region Directory properties

    public SqlInt32 Attributes
    {
      get { return !IsNull ? (Int32)_di.Attributes : SqlInt32.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _di.Attributes = (FileAttributes)value.Value;
      }
    }

    public SqlDateTime CreationTime
    {
      get { return !IsNull ? _di.CreationTime : SqlDateTime.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _di.CreationTime = value.Value;
      }
    }

    public SqlDateTime CreationTimeUtc
    {
      get { return !IsNull ? _di.CreationTimeUtc : SqlDateTime.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _di.CreationTimeUtc = value.Value;
      }
    }

    public SqlBoolean Exists
    {
      get { return !IsNull ? _di.Exists : SqlBoolean.Null; }
    }

    public SqlString Extension
    {
      get { return !IsNull ? _di.Extension : SqlString.Null; }
    }

    public SqlString FullName
    {
      get { return !IsNull ? _di.FullName : SqlString.Null; }
    }

    public SqlDateTime LastAccessTime
    {
      get { return !IsNull ? _di.LastAccessTime : SqlDateTime.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _di.LastAccessTime = value.Value;
      }
    }

    public SqlDateTime LastAccessTimeUtc
    {
      get { return !IsNull ? _di.LastAccessTimeUtc : SqlDateTime.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _di.LastAccessTimeUtc = value.Value;
      }
    }

    public SqlDateTime LastWriteTime
    {
      get { return !IsNull ? _di.LastWriteTime : SqlDateTime.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _di.LastWriteTime = value.Value;
      }
    }

    public SqlDateTime LastWriteTimeUtc
    {
      get { return !IsNull ? _di.LastWriteTimeUtc : SqlDateTime.Null; }
      set
      {
        if (IsNull || value.IsNull)
          return;

        _di.LastWriteTimeUtc = value.Value;
      }
    }

    public SqlString Name
    {
      get { return !IsNull ? _di.Name : SqlString.Null; }
    }

    public SqlDirectoryInfo Parent
    {
      get
      {
        if (IsNull)
          return SqlDirectoryInfo.Null;

        var diParent = _di.Parent;
        return diParent != null ? new SqlDirectoryInfo(diParent) : SqlDirectoryInfo.Null;
      }
    }

    public SqlDirectoryInfo Root
    {
      get
      {
        if (IsNull)
          return SqlDirectoryInfo.Null;

        var diRoot = _di.Root;
        return diRoot != null ? new SqlDirectoryInfo(diRoot) : SqlDirectoryInfo.Null;
      }
    }

    #endregion
    #endregion
    #region Methods

    public static SqlDirectoryInfo Parse(SqlString s)
    {
      return !s.IsNull ? new SqlDirectoryInfo(s.Value) : Null;
    }

    public override String ToString()
    {
      return _di != null ? _di.FullName : SqlFormatting.NullText;
    }

    #region Directory manipulation functions

    [SqlMethod]
    public SqlBoolean Create()
    {
      if (IsNull)
        return SqlBoolean.Null;

      _di.Create();
      return _di.Exists;
    }

    [SqlMethod]
    public SqlDirectoryInfo CreateSubdirectory(SqlString path)
    {
      if (IsNull || path.IsNull)
        return SqlDirectoryInfo.Null;

      return new SqlDirectoryInfo(_di.CreateSubdirectory(path.Value));
    }

    [SqlMethod]
    public SqlBoolean Delete(SqlBoolean recursive)
    {
      if (IsNull)
        return SqlBoolean.Null;

      if (!Exists)
        return false;

      if (recursive.IsNull)
        _di.Delete();
      else
        _di.Delete(recursive.Value);
      return true;
    }

    [SqlMethod]
    public SqlBoolean MoveTo(SqlString targetPath)
    {
      if (IsNull || targetPath.IsNull)
        return SqlBoolean.Null;

      if (!Exists)
        return false;

      _di.MoveTo(targetPath.Value);
      return true;
    }

    #endregion
    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _di = rd.BaseStream.ReadObject(s => new DirectoryInfo(s.ReadString(Encoding.UTF8, SizeEncoding.VB)), s => s.ReadBoolean(TypeCode.Byte));
    }

    public void Write(BinaryWriter wr)
    {
      wr.BaseStream.WriteObject(_di != null ? _di.FullName : null, (s, t) => s.WriteString(t, Encoding.UTF8, SizeEncoding.VB), (s, f) => s.WriteBoolean(f, TypeCode.Byte, false));
    }

    #endregion
  }
}
