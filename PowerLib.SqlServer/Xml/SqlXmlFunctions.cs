using System;
using System.Collections;
using System.Linq;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Runtime.Serialization.Json;
using Microsoft.SqlServer.Server;
using PowerLib.System;
using PowerLib.System.Linq;

namespace PowerLib.SqlServer.Xml
{
  public static class SqlXmlFunctions
  {
    #region Internal functions

    private static IXmlNamespaceResolver GetNamespaceResolver(XmlNameTable nameTable, string nsMap)
    {
      var nsResolver = new XmlNamespaceManager(nameTable);
      if (nsMap != null)
        nsMap.SplitToKeyValue(new[] { ';' }, new[] { '=' })
          .ForEach(p => nsResolver.AddNamespace(p.Key, p.Value));
      return nsResolver;
    }

    private static XslCompiledTransform GetStylesheetTransformer(SqlXml stylesheet)
    {
      XslCompiledTransform transform = new XslCompiledTransform();
      using (var stylesheetReader = stylesheet.CreateReader())
        transform.Load(stylesheetReader);
      return transform;
    }

    #endregion
    #region Scalar functions

    [SqlFunction(Name = "xmlEvaluate", IsDeterministic = true)]
    public static SqlXml Evaluate([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        return SqlXml.Null;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver)) as XPathNodeIterator;
        if (iterator == null)
          return SqlXml.Null;
        var result = iterator.Cast<XPathNavigator>().SingleOrDefault();
        return result != null ? new SqlXml(result.ReadSubtree()) : SqlXml.Null;
      }
    }

    [SqlFunction(Name = "xmlEvaluateAsString", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString EvaluateAsString([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        return SqlString.Null;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var value = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver));
        if (value == null)
          return SqlString.Null;
        var iterator = value as XPathNodeIterator;
        if (iterator == null)
          return new SqlString((String)Convert.ChangeType(value, typeof(String)));
        var result = ((XPathNodeIterator)value).Cast<XPathNavigator>().FirstOrDefault();
        return result == null ? SqlString.Null : new SqlString(result.Value);
      }
    }

    [SqlFunction(Name = "xmlEvaluateAsBit", IsDeterministic = true)]
    public static SqlBoolean EvaluateAsBoolean([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        return SqlBoolean.Null;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var value = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver));
        if (value == null)
          return SqlBoolean.Null;
        var iterator = value as XPathNodeIterator;
        if (iterator == null)
          return new SqlBoolean((Boolean)Convert.ChangeType(value, typeof(Boolean)));
        var result = ((XPathNodeIterator)value).Cast<XPathNavigator>().FirstOrDefault();
        return result == null ? SqlBoolean.Null : new SqlBoolean((Boolean)result.ValueAs(typeof(Boolean), resolver));
      }
    }

    [SqlFunction(Name = "xmlEvaluateAsTinyInt", IsDeterministic = true)]
    public static SqlByte EvaluateAsByte([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        return SqlByte.Null;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var value = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver));
        if (value == null)
          return SqlByte.Null;
        var iterator = value as XPathNodeIterator;
        if (iterator == null)
          return new SqlByte((Byte)Convert.ChangeType(value, typeof(Byte)));
        var result = ((XPathNodeIterator)value).Cast<XPathNavigator>().FirstOrDefault();
        return result == null ? SqlByte.Null : new SqlByte((Byte)result.ValueAs(typeof(Byte), resolver));
      }
    }

    [SqlFunction(Name = "xmlEvaluateAsSmallInt", IsDeterministic = true)]
    public static SqlInt16 EvaluateAsInt16([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        return SqlInt16.Null;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var value = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver));
        if (value == null)
          return SqlInt16.Null;
        var iterator = value as XPathNodeIterator;
        if (iterator == null)
          return new SqlInt16((Int16)Convert.ChangeType(value, typeof(Int16)));
        var result = ((XPathNodeIterator)value).Cast<XPathNavigator>().FirstOrDefault();
        return result == null ? SqlInt16.Null : new SqlInt16((Int16)result.ValueAs(typeof(Int16), resolver));
      }
    }

    [SqlFunction(Name = "xmlEvaluateAsInt", IsDeterministic = true)]
    public static SqlInt32 EvaluateAsInt32([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        return SqlInt32.Null;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var value = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver));
        if (value == null)
          return SqlInt32.Null;
        var iterator = value as XPathNodeIterator;
        if (iterator == null)
          return new SqlInt32((Int32)Convert.ChangeType(value, typeof(Int32)));
        var result = ((XPathNodeIterator)value).Cast<XPathNavigator>().FirstOrDefault();
        return result == null ? SqlInt32.Null : new SqlInt32((Int32)result.ValueAs(typeof(Int32), resolver));
      }
    }

    [SqlFunction(Name = "xmlEvaluateAsBigInt", IsDeterministic = true)]
    public static SqlInt64 EvaluateAsInt64([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        return SqlInt64.Null;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var value = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver));
        if (value == null)
          return SqlInt64.Null;
        var iterator = value as XPathNodeIterator;
        if (iterator == null)
          return new SqlInt64((Int64)Convert.ChangeType(value, typeof(Int64)));
        var result = ((XPathNodeIterator)value).Cast<XPathNavigator>().FirstOrDefault();
        return result == null ? SqlInt64.Null : new SqlInt64((Int64)result.ValueAs(typeof(Int64), resolver));
      }
    }

    [SqlFunction(Name = "xmlEvaluateAsSingle", IsDeterministic = true)]
    public static SqlSingle EvaluateAsSingle([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        return SqlSingle.Null;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var value = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver));
        if (value == null)
          return SqlSingle.Null;
        var iterator = value as XPathNodeIterator;
        if (iterator == null)
          return new SqlSingle((Single)Convert.ChangeType(value, typeof(Single)));
        var result = ((XPathNodeIterator)value).Cast<XPathNavigator>().FirstOrDefault();
        return result == null ? SqlSingle.Null : new SqlSingle((Single)result.ValueAs(typeof(Single), resolver));
      }
    }

    [SqlFunction(Name = "xmlEvaluateAsDouble", IsDeterministic = true)]
    public static SqlDouble EvaluateAsDouble([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        return SqlDouble.Null;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var value = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver));
        if (value == null)
          return SqlDouble.Null;
        var iterator = value as XPathNodeIterator;
        if (iterator == null)
          return new SqlDouble((Double)Convert.ChangeType(value, typeof(Double)));
        var result = ((XPathNodeIterator)value).Cast<XPathNavigator>().FirstOrDefault();
        return result == null ? SqlDouble.Null : new SqlDouble((Double)result.ValueAs(typeof(Double), resolver));
      }
    }

    [SqlFunction(Name = "xmlEvaluateAsDateTime", IsDeterministic = true)]
    public static SqlDateTime EvaluateAsDateTime([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        return SqlDateTime.Null;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var value = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver));
        if (value == null)
          return SqlDateTime.Null;
        var iterator = value as XPathNodeIterator;
        if (iterator == null)
          return new SqlDateTime((DateTime)Convert.ChangeType(value, typeof(DateTime)));
        var result = ((XPathNodeIterator)value).Cast<XPathNavigator>().FirstOrDefault();
        return result == null ? SqlDateTime.Null : new SqlDateTime((DateTime)result.ValueAs(typeof(DateTime), resolver));
      }
    }

    [SqlFunction(Name = "xmlInsertAfter", IsDeterministic = true)]
    public static SqlXml InsertAfter(SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap, SqlXml value)
    {
      if (input.IsNull || path.IsNull || value.IsNull)
        return input;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Select(path.Value, resolver);
        if (iterator != null)
          while (iterator.MoveNext())
            using (var xrd = value.CreateReader())
              iterator.Current.InsertAfter(xrd);
        return input;
      }
    }

    [SqlFunction(Name = "xmlInsertBefore", IsDeterministic = true)]
    public static SqlXml InsertBefore(SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap, SqlXml value)
    {
      if (input.IsNull || path.IsNull || value.IsNull)
        return input;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Select(path.Value, resolver);
        if (iterator != null)
          while (iterator.MoveNext())
            using (var xrd = value.CreateReader())
              iterator.Current.InsertBefore(xrd);
        return input;
      }
    }

    [SqlFunction(Name = "xmlDelete", IsDeterministic = true)]
    public static SqlXml Delete(SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        return input;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Select(path.Value, resolver);
        if (iterator != null)
          while (iterator.MoveNext())
            iterator.Current.DeleteSelf();
        return input;
      }
    }

    [SqlFunction(Name = "xmlReplace", IsDeterministic = true)]
    public static SqlXml Replace(SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap, SqlXml replace)
    {
      if (input.IsNull || path.IsNull || replace.IsNull)
        return input;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Select(path.Value, resolver);
        if (iterator != null)
          while (iterator.MoveNext())
            using (var xrd = replace.CreateReader())
              iterator.Current.ReplaceSelf(xrd);
        return input;
      }
    }

    [SqlFunction(Name = "xmlTransform", IsDeterministic = true)]
    public static SqlXml Transform([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlXml stylesheet)
    {
      if (input.IsNull || stylesheet.IsNull)
        return SqlXml.Null;

      Encoding enc = new UnicodeEncoding(!BitConverter.IsLittleEndian, false);
      XslCompiledTransform tr = GetStylesheetTransformer(stylesheet);
      using (var ird = input.CreateReader())
      using (var ms = new MemoryStream())
      using (var owr = XmlWriter.Create(ms, new XmlWriterSettings() { Indent = false, Encoding = enc }))
      {
        tr.Transform(ird, owr);
        ms.Position = 0;
        using (var resultReader = XmlReader.Create(ms, new XmlReaderSettings() { IgnoreWhitespace = true }))
          return new SqlXml(resultReader);
      }
    }

    [SqlFunction(Name = "xmlToJson", IsDeterministic = true)]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ToJson([SqlFacet(MaxSize = -1)] SqlXml input)
    {
      if (input.IsNull)
        return SqlString.Null;

      Encoding enc = new UnicodeEncoding(!BitConverter.IsLittleEndian, false);
      var doc = new XmlDocument();
      using (var xrd = input.CreateReader())
        doc.Load(xrd);
      using (var ms = new MemoryStream())
      using (var jsw = JsonReaderWriterFactory.CreateJsonWriter(ms, enc, false, false))
      {
        doc.Save(jsw);
        ms.Position = 0L;
        using (var srd = new StreamReader(ms, enc))
          return srd.ReadToEnd();
      }
    }

    [SqlFunction(Name = "xmlFromJson", IsDeterministic = true)]
    public static SqlXml FromJson([SqlFacet(MaxSize = -1)] SqlString input)
    {
      if (input.IsNull)
        return SqlXml.Null;

      Encoding enc = new UnicodeEncoding(!BitConverter.IsLittleEndian, false);
      using (var mss = new MemoryStream())
      using (var swr = new StreamWriter(mss, enc) { AutoFlush = true })
      {
        swr.Write(input.Value);
        mss.Position = 0L;
        using (var jsr = JsonReaderWriterFactory.CreateJsonReader(mss, enc, XmlDictionaryReaderQuotas.Max, null))
        {
          var doc = new XmlDocument();
          doc.Load(jsr);
          using (var mst = new MemoryStream())
          using (var xwr = XmlWriter.Create(mst, new XmlWriterSettings() { Indent = false, Encoding = enc, CloseOutput = false }))
          {
            doc.Save(xwr);
            mst.Position = 0L;
            using (var xrd = XmlReader.Create(mst, new XmlReaderSettings() { IgnoreWhitespace = true, CloseInput = false }))
              return new SqlXml(xrd);
          }
        }
      }
    }

    #endregion
    #region Table-valued functions

    [SqlFunction(Name = "xmlSelect", IsDeterministic = true, FillRowMethodName = "ToXmlFillRow")]
    public static IEnumerable Select([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        yield break;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver)) as XPathNodeIterator;
        if (iterator != null)
          foreach (var item in navigator.Select(XPathExpression.Compile(path.Value, resolver)).Cast<XPathNavigator>())
            yield return item == null ? SqlXml.Null : new SqlXml(item.ReadSubtree());
      }
    }

    [SqlFunction(Name = "xmlSelectAsString", IsDeterministic = true, FillRowMethodName = "ToStringFillRow")]
    public static IEnumerable SelectAsString([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        yield break;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver)) as XPathNodeIterator;
        if (iterator != null)
          foreach (var item in iterator.Cast<XPathNavigator>())
            yield return item == null ? null : item.Value;
      }
    }

    [SqlFunction(Name = "xmlSelectAsBit", IsDeterministic = true, FillRowMethodName = "ToBooleanFillRow")]
    public static IEnumerable SelectAsBoolean([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        yield break;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver)) as XPathNodeIterator;
        if (iterator != null)
          foreach (var item in iterator.Cast<XPathNavigator>())
            yield return item == null ? (Boolean?)null : (Boolean)item.ValueAs(typeof(Boolean), item);
      }
    }

    [SqlFunction(Name = "xmlSelectAsTinyInt", IsDeterministic = true, FillRowMethodName = "ToByteFillRow")]
    public static IEnumerable SelectAsByte([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        yield break;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver)) as XPathNodeIterator;
        if (iterator != null)
          foreach (var item in iterator.Cast<XPathNavigator>())
            yield return item == null ? (Byte?)null : (Byte)item.ValueAs(typeof(Byte), item);
      }
    }

    [SqlFunction(Name = "xmlSelectAsSmallInt", IsDeterministic = true, FillRowMethodName = "ToInt16FillRow")]
    public static IEnumerable SelectAsInt16([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        yield break;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver)) as XPathNodeIterator;
        if (iterator != null)
          foreach (var item in iterator.Cast<XPathNavigator>())
            yield return item == null ? (Int16?)null : (Int16)item.ValueAs(typeof(Int16), item);
      }
    }

    [SqlFunction(Name = "xmlSelectAsInt", IsDeterministic = true, FillRowMethodName = "ToInt32FillRow")]
    public static IEnumerable SelectAsInt32([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        yield break;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver)) as XPathNodeIterator;
        if (iterator != null)
          foreach (var item in iterator.Cast<XPathNavigator>())
            yield return item == null ? (Int32?)null : (Int32)item.ValueAs(typeof(Int32), item);
      }
    }

    [SqlFunction(Name = "xmlSelectAsBigInt", IsDeterministic = true, FillRowMethodName = "ToInt64FillRow")]
    public static IEnumerable SelectAsInt64([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        yield break;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver)) as XPathNodeIterator;
        if (iterator != null)
          foreach (var item in iterator.Cast<XPathNavigator>())
            yield return item == null ? (Int64?)null : (Int64)item.ValueAs(typeof(Int64), item);
      }
    }

    [SqlFunction(Name = "xmlSelectAsSingle", IsDeterministic = true, FillRowMethodName = "ToSingleFillRow")]
    public static IEnumerable SelectAsSingle([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        yield break;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver)) as XPathNodeIterator;
        if (iterator != null)
          foreach (var item in iterator.Cast<XPathNavigator>())
            yield return item == null ? (Single?)null : (Single)item.ValueAs(typeof(Single), item);
      }
    }

    [SqlFunction(Name = "xmlSelectAsDouble", IsDeterministic = true, FillRowMethodName = "ToDoubleFillRow")]
    public static IEnumerable SelectAsDouble([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        yield break;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver)) as XPathNodeIterator;
        if (iterator != null)
          foreach (var item in iterator.Cast<XPathNavigator>())
            yield return item == null ? (Double?)null : (Double)item.ValueAs(typeof(Double), item);
      }
    }

    [SqlFunction(Name = "xmlSelectAsDateTime", IsDeterministic = true, FillRowMethodName = "ToDateTimeFillRow")]
    public static IEnumerable SelectAsDateTime([SqlFacet(MaxSize = -1)] SqlXml input, [SqlFacet(MaxSize = -1)] SqlString path, SqlString nsMap)
    {
      if (input.IsNull || path.IsNull)
        yield break;

      using (var reader = input.CreateReader())
      {
        var navigator = new XPathDocument(reader).CreateNavigator();
        var resolver = GetNamespaceResolver(navigator.NameTable, !nsMap.IsNull ? nsMap.Value : null);
        var iterator = navigator.Evaluate(XPathExpression.Compile(path.Value, resolver)) as XPathNodeIterator;
        if (iterator != null)
          foreach (var item in iterator.Cast<XPathNavigator>())
            yield return item == null ? (DateTime?)null : (DateTime)item.ValueAs(typeof(DateTime), item);
      }
    }

    #endregion
    #region FillRow functions

    private static void ToXmlFillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlXml Value)
    {
      Value = (SqlXml)obj;
    }

    private static void ToStringFillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlString Value)
    {
      Value = (String)obj;
    }

    private static void ToBooleanFillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlBoolean Value)
    {
      Boolean? value = (Boolean?)obj;
      Value = value.HasValue ? value.Value : SqlBoolean.Null;
    }

    private static void ToByteFillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlByte Value)
    {
      Byte? value = (Byte?)obj;
      Value = value.HasValue ? value.Value : SqlByte.Null;
    }

    private static void ToInt16FillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlInt16 Value)
    {
      Int16? value = (Int16?)obj;
      Value = value.HasValue ? value.Value : SqlInt16.Null;
    }

    private static void ToInt32FillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlInt32 Value)
    {
      Int32? value = (Int32?)obj;
      Value = value.HasValue ? value.Value : SqlInt32.Null;
    }

    private static void ToInt64FillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlInt64 Value)
    {
      Int64? value = (Int64?)obj;
      Value = value.HasValue ? value.Value : SqlInt64.Null;
    }

    private static void ToSingleFillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlSingle Value)
    {
      Single? value = (Single?)obj;
      Value = value.HasValue ? value.Value : SqlSingle.Null;
    }

    private static void ToDoubleFillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlDouble Value)
    {
      Double? value = (Double?)obj;
      Value = value.HasValue ? value.Value : SqlDouble.Null;
    }

    private static void ToDateTimeFillRow(object obj, [SqlFacet(MaxSize = -1)] out SqlDateTime Value)
    {
      DateTime? value = (DateTime?)obj;
      Value = value.HasValue ? value.Value : SqlDateTime.Null;
    }

    #endregion
  }
}
