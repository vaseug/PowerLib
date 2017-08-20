using System;
using System.IO;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System.IO;

namespace PowerLib.System.Data.SqlTypes.Net
{
  [SqlUserDefinedType(Format.UserDefined, Name = "Uri", IsByteOrdered = false, IsFixedLength = false, MaxByteSize = -1)]
  public sealed class SqlUri : INullable, IBinarySerialize
  {
    private static readonly SqlUri @null = new SqlUri();

    private Uri _uri;

    #region Constructor

    public SqlUri()
    {
      _uri = null;
    }

    public SqlUri(string uri)
    {
      _uri = new Uri(uri);
    }

    public SqlUri(Uri uri)
    {
      _uri = uri;
    }

    #endregion
    #region Properties

    public Uri Uri
    {
      get { return _uri; }
    }

    public static SqlUri Null
    {
      get { return @null; }
    }

    public bool IsNull
    {
      get { return _uri == null; }
    }

    [SqlFacet(IsFixedLength = false, MaxSize = -1)]
    public SqlString AbsolutePath
    {
      get { return IsNull ? SqlString.Null : _uri.AbsolutePath; }
    }

    [SqlFacet(IsFixedLength = false, MaxSize = -1)]
    public SqlString AbsoluteUri
    {
      get { return IsNull ? SqlString.Null : _uri.AbsoluteUri; }
    }

    [SqlFacet(IsFixedLength = false, MaxSize = -1)]
    public SqlString Authority
    {
      get { return IsNull ? SqlString.Null : _uri.Authority; }
    }

    [SqlFacet(IsFixedLength = false, MaxSize = -1)]
    public SqlString DnsSafeHost
    {
      get { return IsNull ? SqlString.Null : _uri.DnsSafeHost; }
    }

    [SqlFacet(IsFixedLength = false, MaxSize = -1)]
    public SqlString Fragment
    {
      get { return IsNull ? SqlString.Null : _uri.Fragment; }
    }

    [SqlFacet(IsFixedLength = false, MaxSize = -1)]
    public SqlString Host
    {
      get { return IsNull ? SqlString.Null : _uri.Host; }
    }

    public SqlInt32 HostNameType
    {
      get { return IsNull ? SqlInt32.Null : (Int32)_uri.HostNameType; }
    }

    public SqlBoolean IsAbsoluteUri
    {
      get { return IsNull ? SqlBoolean.Null : _uri.IsAbsoluteUri; }
    }

    public SqlBoolean IsDefaultPort
    {
      get { return IsNull ? SqlBoolean.Null : _uri.IsDefaultPort; }
    }

    public SqlBoolean IsFile
    {
      get { return IsNull ? SqlBoolean.Null : _uri.IsFile; }
    }

    public SqlBoolean IsLoopback
    {
      get { return IsNull ? SqlBoolean.Null : _uri.IsLoopback; }
    }

    public SqlBoolean IsUnc
    {
      get { return IsNull ? SqlBoolean.Null : _uri.IsUnc; }
    }

    [SqlFacet(MaxSize = -1)]
    public SqlString LocalPath
    {
      get { return IsNull ? SqlString.Null : _uri.LocalPath; }
    }

    [SqlFacet(MaxSize = -1)]
    public SqlString OriginalString
    {
      get { return IsNull ? SqlString.Null : _uri.OriginalString; }
    }

    [SqlFacet(MaxSize = -1)]
    public SqlString PathAndQuery
    {
      get { return IsNull ? SqlString.Null : _uri.PathAndQuery; }
    }

    public SqlInt32 Port
    {
      get { return IsNull ? SqlInt32.Null : _uri.Port; }
    }

    [SqlFacet(MaxSize = -1)]
    public SqlString Query
    {
      get { return IsNull ? SqlString.Null : _uri.Query; }
    }

    [SqlFacet(MaxSize = -1)]
    public SqlString Scheme
    {
      get { return IsNull ? SqlString.Null : _uri.Scheme; }
    }

    public SqlBoolean UserEscaped
    {
      get { return IsNull ? SqlBoolean.Null : _uri.UserEscaped; }
    }

    [SqlFacet(MaxSize = -1)]
    public SqlString UserInfo
    {
      get { return IsNull ? SqlString.Null : _uri.UserInfo; }
    }

    public SqlBoolean IsWellFormedOriginalString
    {
      get { return IsNull ? SqlBoolean.Null : _uri.IsWellFormedOriginalString(); }
    }

    #endregion
    #region Methods

    public static SqlUri Parse(SqlString s)
    {
      return !s.IsNull ? new SqlUri(s.Value) : Null;
    }

    public override String ToString()
    {
      return _uri != null ? _uri.ToString() : SqlFormatting.NullText;
    }

    [SqlMethod(IsDeterministic = true, OnNullCall = false)]
    [return:SqlFacet(MaxSize = -1)]
    public SqlString GetComponents(SqlInt32 components, SqlInt32 format)
    {
      if (IsNull || components.IsNull || format.IsNull)
        return SqlString.Null;

      return _uri.GetComponents((UriComponents)components.Value, (UriFormat)format.Value);
    }

    [SqlMethod(IsDeterministic = true, OnNullCall = false)]
    [return: SqlFacet(MaxSize = -1)]
    public SqlString GetLeftPart(SqlInt32 part)
    {
      if (IsNull || part.IsNull)
        return SqlString.Null;

      return _uri.GetLeftPart((UriPartial)part.Value);
    }

    [SqlMethod(IsDeterministic = true, OnNullCall = false)]
    public SqlBoolean IsBaseOf(SqlUri uri)
    {
      if (IsNull || uri.IsNull)
        return SqlBoolean.Null;

      return _uri.IsBaseOf(uri._uri);
    }

    [SqlMethod(IsDeterministic = true, OnNullCall = false)]
    public SqlUri MakeRelativeUri(SqlUri uri)
    {
      if (IsNull || uri.IsNull)
        return SqlUri.Null;

      return new SqlUri(_uri.MakeRelativeUri(uri._uri));
    }

    #endregion
    #region IBinarySerialize implementation

    public void Read(BinaryReader rd)
    {
      _uri = new Uri(rd.BaseStream.ReadString(Encoding.UTF8, SizeEncoding.VB));
    }

    public void Write(BinaryWriter wr)
    {
      wr.BaseStream.WriteString(_uri.ToString(), Encoding.UTF8, SizeEncoding.VB);
    }

    #endregion
  }
}
