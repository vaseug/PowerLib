using System;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Net;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.ComponentModel;
using PowerLib.System.Data.SqlTypes.Collections;

namespace PowerLib.SqlServer.Web
{
  public static class SqlWebFunctions
  {
    private const string ReaderPrefix = "reader:";
    private const string WriterPrefix = "writer:";
    private const string RequestPrefix = "request:";
    private const string ResponsePrefix = "response:";

    #region Internal methods

    private static string GetFieldValue(string text, string name, string itemDelimiters, string keyValueDelimiters, string trimmers)
    {
      if (text == null)
        throw new ArgumentNullException(nameof(text));
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", nameof(name));

      var item = text.SplitToKeyValue(itemDelimiters.ToCharArray(), keyValueDelimiters.ToCharArray(), trimmers.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        .FirstOrDefault(t => t.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase));
      return item.Key == null ? null : item.Value;
    }

    private static string SetFieldValue(string text, string name, string value, string itemDelimiters, string keyValueDelimiters, string trimmers)
    {
      if (text == null)
        throw new ArgumentNullException(nameof(text));
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Field name is not specified", nameof(name));

      return string.Join(itemDelimiters[0].ToString(),
        text.SplitToKeyValue(itemDelimiters.ToCharArray(), keyValueDelimiters.ToCharArray(), trimmers.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
          .Select(p => p.Key + (keyValueDelimiters[0] + (p.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase) ? value : p.Value)) ?? string.Empty));
    }

    private static NameValueCollection GetWithPrefix(NameValueCollection coll, string prefix)
    {
      if (prefix == null)
        prefix = string.Empty;
      NameValueCollection result = new NameValueCollection();
      foreach (string key in coll)
        if (key.StartsWith(prefix))
          result.Add(key.Remove(0, prefix.Length), coll[key]);
      return result;
    }

    private static void PutWithPrefix(NameValueCollection coll, string prefix, NameValueCollection items)
    {
      if (prefix == null)
        prefix = string.Empty;
      foreach (string key in items)
        coll[prefix + key] = items[key];
    }

    private static void GetProperties(object obj, NameValueCollection coll, string prefix)
    {
      var retrieve = GetWithPrefix(coll, ResponsePrefix);
      TypeInitializer.Default.Retrieve(obj, retrieve);
      PutWithPrefix(coll, ResponsePrefix, retrieve);
    }

    private static void SetProperties(object obj, NameValueCollection coll, string prefix)
    {
      TypeInitializer.Default.Initialize(obj, GetWithPrefix(coll, prefix));
    }

    private static Encoding GetContentEncoding(string contentType)
    {
      if (contentType == null)
        throw new ArgumentNullException(nameof(contentType));

      var charset = GetFieldValue(contentType, "charset", ";", "=", " ");
      if (charset == null)
        return null;
      try
      {
        return Encoding.GetEncoding(charset);
      }
      catch (Exception)
      {
        return null;
      }
    }

    private static SqlXml GetXml(WebResponse response, NameValueCollection attributes)
    {
      var stream = response.GetResponseStream();
      if (attributes != null)
        GetProperties(response, attributes, ResponsePrefix);
      if (stream == null || stream == Stream.Null)
        return SqlXml.Null;

      var rds = new XmlReaderSettings { CloseInput = true };
      if (attributes != null)
        SetProperties(rds, attributes, ReaderPrefix);
      using (var xrd = XmlReader.Create(stream, rds))
        return new SqlXml(xrd);
    }

    private static SqlChars GetText(WebResponse response, NameValueCollection attributes)
    {
      var stream = response.GetResponseStream();
      if (attributes != null)
        GetProperties(response, attributes, ResponsePrefix);
      if (stream == null || stream == Stream.Null)
        return SqlChars.Null;

      var encoding = response.ContentType != null ? GetContentEncoding(response.ContentType) : null;
      var sb = new StringBuilder();
      var buffer = new char[1024];
      using (var srd = encoding != null ? new StreamReader(stream, encoding) : new StreamReader(stream, true))
        while (!srd.EndOfStream)
          sb.Append(buffer, 0, srd.ReadBlock(buffer, 0, buffer.Length));
      var array = new char[sb.Length];
      sb.CopyTo(0, array, 0, sb.Length);
      return new SqlChars(array);
    }

    private static SqlBytes GetBinary(WebResponse response, NameValueCollection attributes)
    {
      using (var stream = response.GetResponseStream())
      {
        if (attributes != null)
          GetProperties(response, attributes, ResponsePrefix);
        if (stream == null || stream == Stream.Null)
          return SqlBytes.Null;

        var ms = new MemoryStream();
        try
        {
          stream.CopyTo(ms, SqlRuntime.IoBufferSize);
          ms.Flush();
        }
        catch (Exception)
        {
          ms.Dispose();
          throw;
        }
        return new SqlBytes(ms);
      }
    }

    private static void PutXml(WebRequest request, SqlXml xml, NameValueCollection attributes)
    {
      var wrs = new XmlWriterSettings()
      {
        Encoding = request.ContentType != null ? GetContentEncoding(request.ContentType) : SqlRuntime.WebEncoding,
        CloseOutput = true
      };
      if (attributes != null)
        SetProperties(wrs, attributes, WriterPrefix);
      using (var xrd = xml.CreateReader())
      using (var xwr = XmlWriter.Create(request.GetRequestStream(), wrs))
      {
        xwr.WriteNode(xrd, true);
        xwr.Flush();
      }
    }

    private static void PutText(WebRequest request, SqlChars chars, NameValueCollection attributes)
    {
      using (var swr = new StreamWriter(request.GetRequestStream(), request.ContentType != null ? GetContentEncoding(request.ContentType) : SqlRuntime.WebEncoding))
      {
        if (attributes != null)
          SetProperties(swr, attributes, WriterPrefix);
        long offset = 0L;
        var buffer = new char[1024];
        while (offset < chars.Length)
        {
          int read = (int)chars.Read(offset, buffer, 0, buffer.Length);
          swr.Write(buffer, 0, read);
          offset += read;
        }
        swr.Flush();
      }
    }

    private static void PutBinary(WebRequest request, SqlBytes bytes, NameValueCollection attributes)
    {
      using (var stream = request.GetRequestStream())
      {
        long offset = 0L;
        var buffer = new byte[SqlRuntime.IoBufferSize];
        while (offset < bytes.Length)
        {
          int read = (int)bytes.Read(offset, buffer, 0, buffer.Length);
          stream.Write(buffer, 0, read);
          offset += read;
        }
        stream.Flush();
      }
    }

    private static WebRequest Create(String uri, String method, NameValueCollection headers, NameValueCollection attributes)
    {
      var request = WebRequest.Create(uri);
      if (method != null)
        request.Method = method;
      if (headers != null)
        request.Headers.Add(headers);
      if (attributes != null)
        SetProperties(request, attributes, RequestPrefix);
      return request;
    }

    private static WebRequest Create(Uri uri, String method, NameValueCollection headers, NameValueCollection attributes)
    {
      var request = WebRequest.Create(uri);
      if (method != null)
        request.Method = method;
      if (headers != null)
        request.Headers.Add(headers);
      if (attributes != null)
        SetProperties(request, attributes, RequestPrefix);
      return request;
    }

    #endregion
    #region Web configuration methods
    #region Web error lengths

    [SqlProcedure(Name = "setDefaultMaximumErrorResponseLength")]
    public static void SetDefaultMaximumErrorResponseLength(SqlInt32 length)
    {
      HttpWebRequest.DefaultMaximumErrorResponseLength = length.IsNull ? -1 : length.Value;
    }

    [SqlProcedure(Name = "getDefaultMaximumErrorResponseLength")]
    public static void GetDefaultMaximumErrorResponseLength(out SqlInt32 length)
    {
      length = HttpWebRequest.DefaultMaximumErrorResponseLength;
    }

    [SqlProcedure(Name = "setDefaultMaximumResponseHeadersLength")]
    public static void SetDefaultMaximumResponseHeadersLength(SqlInt32 length)
    {
      HttpWebRequest.DefaultMaximumResponseHeadersLength = length.IsNull ? -1 : length.Value;
    }

    [SqlProcedure(Name = "getDefaultMaximumResponseHeadersLength")]
    public static void GetDefaultMaximumResponseHeadersLength(out SqlInt32 length)
    {
      length = HttpWebRequest.DefaultMaximumResponseHeadersLength;
    }

    #endregion
    #endregion
    #region Web Query methods

    [SqlFunction(Name = "webQueryGetXml", IsDeterministic = false)]
    public static SqlXml QueryGetXml(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlXml.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        return GetXml(response, attributes.IsNull ? null : attributes.Value);
    }

    [SqlFunction(Name = "webQueryGetText", IsDeterministic = false)]
    public static SqlChars QueryGetText(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlChars.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        return GetText(response, attributes.IsNull ? null : attributes.Value);
    }

    [SqlFunction(Name = "webQueryGetBinary", IsDeterministic = false)]
    public static SqlBytes QueryGetBinary(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlBytes.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        return GetBinary(response, attributes.IsNull ? null : attributes.Value);
    }

    [SqlFunction(Name = "webQueryPutXml", IsDeterministic = false)]
    public static SqlNameValueCollection QueryPutXml(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlXml xml, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlNameValueCollection.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      if (!xml.IsNull)
        PutXml(request, xml, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        if (attributes != null && !attributes.IsNull)
          GetProperties(response, attributes.Value, ResponsePrefix);
      return attributes;
    }

    [SqlFunction(Name = "webQueryPutText", IsDeterministic = false)]
    public static SqlNameValueCollection QueryPutText(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlChars chars, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlNameValueCollection.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      if (!chars.IsNull)
        PutText(request, chars, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        if (attributes != null && !attributes.IsNull)
          GetProperties(response, attributes.Value, ResponsePrefix);
      return attributes;
    }

    [SqlFunction(Name = "webQueryPutBinary", IsDeterministic = false)]
    public static SqlNameValueCollection QueryPutBinary(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlBytes bytes, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlNameValueCollection.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      if (!bytes.IsNull)
        PutBinary(request, bytes, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        if (attributes != null && !attributes.IsNull)
          GetProperties(response, attributes.Value, ResponsePrefix);
      return attributes;
    }

    [SqlFunction(Name = "webQueryPutXmlGetXml", IsDeterministic = false)]
    public static SqlXml QueryPutXmlGetXml(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlXml xml, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlXml.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      if (!xml.IsNull)
        PutXml(request, xml, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        return GetXml(response, attributes.IsNull ? null : attributes.Value);
    }

    [SqlFunction(Name = "webQueryPutXmlGetText", IsDeterministic = false)]
    public static SqlChars QueryPutXmlGetText(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlXml xml, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlChars.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      if (!xml.IsNull)
        PutXml(request, xml, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        return GetText(response, attributes.IsNull ? null : attributes.Value);
    }

    [SqlFunction(Name = "webQueryPutXmlGetBinary", IsDeterministic = false)]
    public static SqlBytes QueryPutXmlGetBinary(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlXml xml, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlBytes.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      if (!xml.IsNull)
        PutXml(request, xml, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        return GetBinary(response, attributes.IsNull ? null : attributes.Value);
    }

    [SqlFunction(Name = "webQueryPutTextGetXml", IsDeterministic = false)]
    public static SqlXml QueryPutTextGetXml(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlChars chars, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlXml.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      if (!chars.IsNull)
        PutText(request, chars, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        return GetXml(response, attributes.IsNull ? null : attributes.Value);
    }

    [SqlFunction(Name = "webQueryPutTextGetText", IsDeterministic = false)]
    public static SqlChars QueryPutTextGetText(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlChars chars, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlChars.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      if (!chars.IsNull)
        PutText(request, chars, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        return GetText(response, attributes.IsNull ? null : attributes.Value);
    }

    [SqlFunction(Name = "webQueryPutTextGetBinary", IsDeterministic = false)]
    public static SqlBytes QueryPutTextGetBinary(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlChars chars, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlBytes.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      if (!chars.IsNull)
        PutText(request, chars, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        return GetBinary(response, attributes.IsNull ? null : attributes.Value);
    }

    [SqlFunction(Name = "webQueryPutBinaryGetXml", IsDeterministic = false)]
    public static SqlXml QueryPutBinaryGetXml(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlBytes bytes, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlXml.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      if (!bytes.IsNull)
        PutBinary(request, bytes, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        return GetXml(response, attributes.IsNull ? null : attributes.Value);
    }

    [SqlFunction(Name = "webQueryPutBinaryGetText", IsDeterministic = false)]
    public static SqlChars QueryPutBinaryGetText(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlBytes bytes, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlChars.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      if (!bytes.IsNull)
        PutBinary(request, bytes, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        return GetText(response, attributes.IsNull ? null : attributes.Value);
    }

    [SqlFunction(Name = "webQueryPutBinaryGetBinary", IsDeterministic = false)]
    public static SqlBytes QueryPutBinaryGetBinary(SqlString uri, SqlString method, SqlNameValueCollection headers, SqlBytes bytes, SqlNameValueCollection attributes)
    {
      if (uri.IsNull || method.IsNull)
        return SqlBytes.Null;

      var request = Create(uri.Value, method.Value, headers.IsNull ? null : headers.Value, attributes.IsNull ? null : attributes.Value);
      if (!bytes.IsNull)
        PutBinary(request, bytes, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
        return GetBinary(response, attributes.IsNull ? null : attributes.Value);
    }

    [SqlProcedure(Name = "webQueryGetXmlExt")]
    public static void QueryGetXmlExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, out SqlNameValueCollection headersOut, out SqlXml xmlOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        xmlOut = GetXml(response, attributes.IsNull ? null : attributes.Value);
      }
    }

    [SqlProcedure(Name = "webQueryGetTextExt")]
    public static void QueryGetTextExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, out SqlNameValueCollection headersOut, out SqlChars charsOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        charsOut = GetText(response, attributes.IsNull ? null : attributes.Value);
      }
    }

    [SqlProcedure(Name = "webQueryGetBinaryExt")]
    public static void QueryGetBinaryExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, out SqlNameValueCollection headersOut, out SqlBytes bytesOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        bytesOut = GetBinary(response, attributes.IsNull ? null : attributes.Value);
      }
    }

    [SqlProcedure(Name = "webQueryPutXmlExt")]
    public static void QueryPutXmlExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, SqlXml xmlIn, out SqlNameValueCollection headersOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      if (!xmlIn.IsNull)
        PutXml(request, xmlIn, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        if (attributes != null && !attributes.IsNull)
          GetProperties(response, attributes.Value, ResponsePrefix);
      }
    }

    [SqlProcedure(Name = "webQueryPutTextExt")]
    public static void QueryPutTextExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, SqlChars charsIn, out SqlNameValueCollection headersOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      if (!charsIn.IsNull)
        PutText(request, charsIn, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        if (attributes != null && !attributes.IsNull)
          GetProperties(response, attributes.Value, ResponsePrefix);
      }
    }

    [SqlProcedure(Name = "webQueryPutBinaryExt")]
    public static void QueryPutBinaryExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, SqlBytes bytesIn, out SqlNameValueCollection headersOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      if (!bytesIn.IsNull)
        PutBinary(request, bytesIn, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        if (attributes != null && !attributes.IsNull)
          GetProperties(response, attributes.Value, ResponsePrefix);
      }
    }

    [SqlProcedure(Name = "webQueryPutXmlGetXmlExt")]
    public static void QueryPutXmlGetXmlExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, SqlXml xmlIn, out SqlNameValueCollection headersOut, out SqlXml xmlOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      if (!xmlIn.IsNull)
        PutXml(request, xmlIn, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        xmlOut = GetXml(response, attributes.IsNull ? null : attributes.Value);
      }
    }

    [SqlProcedure(Name = "webQueryPutXmlGetTextExt")]
    public static void QueryPutXmlGetTextExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, SqlXml xmlIn, out SqlNameValueCollection headersOut, out SqlChars charsOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      if (!xmlIn.IsNull)
        PutXml(request, xmlIn, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        charsOut = GetText(response, attributes.IsNull ? null : attributes.Value);
      }
    }

    [SqlProcedure(Name = "webQueryPutXmlGetBinaryExt")]
    public static void QueryPutXmlGetBinaryExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, SqlXml xmlIn, out SqlNameValueCollection headersOut, out SqlBytes bytesOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      if (!xmlIn.IsNull)
        PutXml(request, xmlIn, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        bytesOut = GetBinary(response, attributes.IsNull ? null : attributes.Value);
      }
    }

    [SqlProcedure(Name = "webQueryPutTextGetXmlExt")]
    public static void QueryPutTextGetXmlExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, SqlChars charsIn, out SqlNameValueCollection headersOut, out SqlXml xmlOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      if (!charsIn.IsNull)
        PutText(request, charsIn, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        xmlOut = GetXml(response, attributes.IsNull ? null : attributes.Value);
      }
    }

    [SqlProcedure(Name = "webQueryPutTextGetTextExt")]
    public static void QueryPutTextGetTextExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, SqlChars charsIn, out SqlNameValueCollection headersOut, out SqlChars charsOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      if (!charsIn.IsNull)
        PutText(request, charsIn, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        charsOut = GetText(response, attributes.IsNull ? null : attributes.Value);
      }
    }

    [SqlProcedure(Name = "webQueryPutTextGetBinaryExt")]
    public static void QueryPutTextGetBinaryExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, SqlChars charsIn, out SqlNameValueCollection headersOut, out SqlBytes bytesOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      if (!charsIn.IsNull)
        PutText(request, charsIn, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        bytesOut = GetBinary(response, attributes.IsNull ? null : attributes.Value);
      }
    }

    [SqlProcedure(Name = "webQueryPutBinaryGetXmlExt")]
    public static void QueryPutBinaryGetXmlExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, SqlBytes bytesIn, out SqlNameValueCollection headersOut, out SqlXml xmlOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      if (!bytesIn.IsNull)
        PutBinary(request, bytesIn, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        xmlOut = GetXml(response, attributes.IsNull ? null : attributes.Value);
      }
    }

    [SqlProcedure(Name = "webQueryPutBinaryGetTextExt")]
    public static void QueryPutBinaryGetTextExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, SqlBytes bytesIn, out SqlNameValueCollection headersOut, out SqlChars charsOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      if (!bytesIn.IsNull)
        PutBinary(request, bytesIn, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        charsOut = GetText(response, attributes.IsNull ? null : attributes.Value);
      }
    }

    [SqlProcedure(Name = "webQueryPutBinaryGetBinaryExt")]
    public static void QueryPutBinaryGetBinaryExt(SqlString uri, SqlString method, SqlNameValueCollection headersIn, SqlBytes bytesIn, out SqlNameValueCollection headersOut, out SqlBytes bytesOut, ref SqlNameValueCollection attributes)
    {
      var request = Create(uri.IsNull ? default(String) : uri.Value, method.IsNull ? default(String) : method.Value, headersIn.IsNull ? null : headersIn.Value, attributes.IsNull ? null : attributes.Value);
      if (!bytesIn.IsNull)
        PutBinary(request, bytesIn, attributes.IsNull ? null : attributes.Value);
      WebResponse response;
      try
      {
        response = request.GetResponse();
      }
      catch (WebException ex)
      {
        response = ex.Response;
      }
      using (response)
      {
        headersOut = new SqlNameValueCollection(new NameValueCollection(response.Headers));
        bytesOut = GetBinary(response, attributes.IsNull ? null : attributes.Value);
      }
    }

    #endregion
    #region Conversion methods

    [SqlFunction(Name = "webHtmlDecode", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString HtmlDecode(SqlString input)
    {
      return input.IsNull ? SqlString.Null : WebUtility.HtmlDecode(input.Value.ToString());
    }

    [SqlFunction(Name = "webHtmlEncode", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString HtmlEncode(SqlString input)
    {
      return input.IsNull ? SqlString.Null : new SqlString(WebUtility.HtmlEncode(input.Value.ToString()));
    }

    [SqlFunction(Name = "webUrlDecode", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString UrlDecode(SqlString input)
    {
      return input.IsNull ? SqlString.Null : WebUtility.UrlDecode(input.Value.ToString());
    }

    [SqlFunction(Name = "webUrlEncode", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString UrlEncode(SqlString input)
    {
      return input.IsNull ? SqlString.Null : new SqlString(WebUtility.UrlEncode(input.Value.ToString()));
    }

    [SqlFunction(Name = "webUrlDecodeBinary", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlBytes UrlDecodeBinary(SqlBytes input, SqlInt32 offset, SqlInt32 count)
    {
      if (input.IsNull)
        return SqlBytes.Null;
      if (!offset.IsNull && (offset.Value < 0 || offset.Value > input.Length))
        throw new ArgumentOutOfRangeException("offset");
      if (!count.IsNull && (count.Value < 0 || count.Value > input.Length - offset.Value))
        throw new ArgumentOutOfRangeException("count");

      int offsetValue = offset.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : offset.Value;
      int countValue = count.IsNull ? input.Value.Length - (offset.IsNull ? 0 : offset.Value) : count.Value;
      return new SqlBytes(WebUtility.UrlDecodeToBytes(input.Buffer, offsetValue, countValue));
    }

    [SqlFunction(Name = "webUrlEncodeBinary", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlBytes UrlEncodeBinary(SqlBytes input, SqlInt32 offset, SqlInt32 count)
    {
      if (input.IsNull)
        return SqlBytes.Null;
      if (!offset.IsNull && (offset.Value < 0 || offset.Value > input.Length))
        throw new ArgumentOutOfRangeException("offset");
      if (!count.IsNull && (count.Value < 0 || count.Value > input.Length - offset.Value))
        throw new ArgumentOutOfRangeException("count");

      int offsetValue = offset.IsNull ? input.Value.Length - (count.IsNull ? input.Value.Length : count.Value) : offset.Value;
      int countValue = count.IsNull ? input.Value.Length - (offset.IsNull ? 0 : offset.Value) : count.Value;
      return new SqlBytes(WebUtility.UrlEncodeToBytes(input.Buffer, offsetValue, countValue));
    }

    [SqlFunction(Name = "webDateTimeToString", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString DateTimeToString(SqlDateTime input)
    {
      return input.IsNull ? SqlString.Null : input.Value.ToUniversalTime().ToString("r");
    }

    #endregion
  }
}
