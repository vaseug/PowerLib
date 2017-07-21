# **PowerLib**

This solution contains the following projects:

* **[PowerLib.System](#PowerLib.System.Data)**
* **[PowerLib.System.Data](#PowerLib.System.Data)**
* **[PowerLib.System.Data.Linq](#PowerLib.System.Data.Linq)**
* **[PowerLib.EntityFramework](#PowerLib.EntityFramework)**
* **[PowerLib.System.Data.SqlTypes](#PowerLib.System.Data.SqlTypes)**
* **[PowerLib.SqlServer](#PowerLib.SqlServer)**
* **[PowerLib.SqlClr.Deploy](#PowerLib.SqlClr.Deploy)**
* **[PowerLib.SqlClr.Deploy.Utility](#PowerLib.SqlClr.Deploy.Utility)**

After successfully building the solution, a folder with the configuration name will be located at its root. It will contains the following folders:
* **PWRLIB** - PowerLib general assemblies,
* **PWRSQL** - SQLCLR assemblies for deploying on MSSQL Server,
* **SQLCLRDU** - utility for deploying any SQLCLR assemblies on MSSQL Server.

---
## PowerLib.System

Contains many classes, structures, interfaces and extension methods that expedite and optimize the development process.

### Arrays

For working with arrays jagged and regular (one-dimensional, multidimensional), there are many classes and extension methods. For example, the following are the extension methods for working with regular arrays.
```csharp
// Creates regular array with specified lengths and lowerBounds.
public static Array CreateAsRegular<T>(int[] lengths, int[] lowerBounds = null);

// Enumerates regular array elements.
public static IEnumerable<T> EnumerateAsRegular<T>(this Array array, bool zeroBased = false, Range? range = null, params Range[] ranges);

// Creates a shallow copy of the regular array.
public static Array CloneAsRegular(this Array array);

// Returns a regular array from its string representation.
public static Array ParseAsRegular<T>(string s, Func<string, int, int[], T> itemParser, string itemPattern, char[] delimitChars, char[] spaceChars, char[] escapeChars, char[] openingChars, char[] closingChars);

// Converts a regular array to its string representation.
public static string FormatAsRegular<T>(this Array array, Func<T, int, int[], string> itemFormatter, Func<int, string> itemDelimiter, Func<int, string> openBracket, Func<int, string> closeBracket, int[] indices = null, bool zeroBased = false, Range? range = null, params Range[] ranges);

// Enumerates a regular array elements and projects each element of the array into a new form.
public static IEnumerable<TResult> SelectAsRegular<TSource, TResult>(this Array array, Func<TSource, int, int[], TResult> selector, int[] indices = null, bool zeroBased = false, Range? range = null, params Range[] ranges);

// Enumerates a regular array elements and filters elements based on a predicate.
public static IEnumerable<T> WhereAsRegular<T>(this Array array, Func<T, int, int[], bool> predicate, int[] indices = null, bool zeroBased = false, Range? range = null, params Range[] ranges);

// Apply action to elements of a regular array.
public static void ApplyAsRegular<T>(this Array array, Action<T, int, int[]> action, int[] indices = null, bool zeroBased = false, Range? range = null, params Range[] ranges);

// Initialize elements of a regular array.
public static void FillAsRegular<T>(this Array array, Func<int, int[], T> valuator, int[] indices = null, bool zeroBased = false, Range? range = null, params Range[] ranges);

// Converts one regular array to another with the possibility of applying transposition and the conversion of the values of the elements.
public static Array ConvertAsRegular<TSource, TResult>(this Array array, Func<TSource, int, int[], int, int[], TResult> converter, int[] transposition = null, int[] lowerBounds = null, int[] targetIndices = null, int[] sourceIndices = null, bool zeroBased = false, Range? range = null, params Range[] ranges);

// 
public static Array RangeAsRegular(this Array array, int[] transposition = null, int[] lowerBounds = null, bool zeroBased = false, Range? range = null, params Range[] ranges);
```
The following are the extension methods for working with jagged arrays.
```csharp
// Creates jagged array with specified structures.
public static Array CreateAsJagged<T>(int[] ranks, Func<int, int[], int[][], int[]> lensGetter, int[] bandedIndices = null, int[][] rankedIndices = null);

// Enumerate jagged array elements
public static IEnumerable<T> EnumerateAsJagged<T>(this Array array, int[] bandedIndices = null, int[][] rankedIndices = null, bool zeroBased = false, Func<int, int[], int[][], bool, Range[]> ranger = null);

// Converts a jagged array to its string representation.
public static string FormatAsJagged<T>(this Array array, Func<T, int, int[], int[][], string> itemFormatter, Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket, int[] bandedIndices = null, int[][] rankedIndices = null, bool zeroBased = false, Func<int, int[], int[][], bool, Range[]> ranger = null);

// Enumerates a jagged array elements and projects each element of the array into a new form.
public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, int, int[], int[][], TResult> selector, int[] bandedIndices = null, int[][] rankedIndices = null, bool zeroBased = false, Func<int, int[], int[][], bool, Range[]> ranger = null);

// Enumerates a jagged array elements and filters elements based on a predicate.
public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, int, int[], int[][], bool> predicate, int[] bandedIndices = null, int[][] rankedIndices = null, bool zeroBased = false, Func<int, int[], int[][], bool, Range[]> ranger = null);

// Initialize elements of a jagged array.
public static void FillAsJagged<T>(this Array array, Func<int, int[], int[][], T> valuator, int[] bandedIndices = null, int[][] rankedIndices = null, bool zeroBased = false, Func<int, int[], int[][], bool, Range[]> ranger = null);

// Apply action to elements of a jagged array.
public static void ApplyAsJagged<T>(this Array array, Action<T, int, int[], int[][]> action, int[] bandedIndices = null, int[][] rankedIndices = null, bool zeroBased = false, Func<int, int[], int[][], bool, Range[]> ranger = null);

// Converts one jagged array to another with conversion of the values of the elements.
public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, int, int[], int[][], TResult> converter, int[] bandedIndices = null, int[][] rankedIndices = null, bool zeroBased = false, Func<int, int[], int[][], bool, Range[]> ranger = null);
```
The following simple code shows the enumeration of the jagged array elements
```csharp
  var ja = new int[][][,]
  {
    new[]
    {
      new int[,]
      {
        { 1, 2 },
        { 3, 4 },
        { 5, 6 },
        { 7, 8 },
      },
      new int[,]
      {
        { 11, 12, 13, 14 },
        { 21, 22, 23, 24 },
        { 31, 32, 33, 34 },
      }
    },
    new[]
    {
      new int[,]
      {
        { 111, 112, 113, 114, 115 },
        { 121, 122, 123, 124, 125 },
        { 131, 132, 133, 134, 135 },
      }
    }
  };
  
  Console.WriteLine(string.Join(",", ja.EnumerateAsJagged<int>()));
```
and produces the following result:
```
1,2,3,4,5,6,7,8,11,12,13,14,21,22,23,24,31,32,33,34,111,112,113,114,115,121,122,123,124,125,131,132,133,134,135
```
### Collections

The **ListExtension** class contains a number of extension methods for working with lists.

#### Lists

**PowerLib.System.Collection.PwrList\<T>** - A list whose operation with an internal buffer is based on the round-robin algorithm. It implements the memory allocation and deallocation management interfaces and contains many methods for working with its elements.

**PowerLib.System.Collection.PwrSortedList\<T>** - Sorted list based on the comparator of elements represented by the interface *IComparer\<T>* or the delegate *Comparison\<T>*. The values of the elements participating in the comparison must be unchanged during the storage in the list. The list can be restricted by storing only unique values. There are also options that control the addition of duplicate values: to the beginning of the chain, to the end, or arbitrarily.

**PowerLib.System.Collection.PwrKeySortedList\<K, T>** - Sorted list based on the element key comparator represented by the *IComparer\<K>* interface or the *Comparison\<K>* delegate. The key extraction is specified by the delegate *Func\<T, K>*. The key values of the elements participating in the comparison must be unchanged during the storage in the list. The list can be restricted by storing only unique values. There are also options that control the addition of duplicate values: to the beginning of the chain, to the end, or arbitrarily.

#### Stacks

**PowerLib.System.Collection.PwrStack\<T>** - A general stack with additional range operations.
**PowerLib.System.Collection.PwrSortedStack\<T>** - A sorted stack (priority stack).
**PowerLib.System.Collection.PwrKeySortedStack\<T>** - A key sorted stack (priority stack).

#### Queues

**PowerLib.System.Collection.PwrQueue\<T>** - A general queue with additional range operations.
**PowerLib.System.Collection.PwrSortedQueue\<T>** - A sorted queue (priority queue).
**PowerLib.System.Collection.PwrKeySortedQueue\<T>** - A key sorted queue (priority queue).

#### Deques

**PowerLib.System.Collection.PwrDeque\<T>** - A general deque with additional range operations.
**PowerLib.System.Collection.PwrSortedDeque\<T>** - A sorted deque (priority deque).
**PowerLib.System.Collection.PwrKeySortedDeque\<T>** - A key sorted deque (priority deque).

#### Bitwise
...
#### Linked list
...
#### Trees
...
#### Graphs
...
#### Matching
There are many classes in namespace **PowerLib.System.Collection.Matching** for working with items matching and comparison.

### IO
#### Streaming
...
#### Filesystem

To work with the file system, the **PowerLib.System.IO.FileSystemInfoExtension** class exists, which allows you to display hierarchical information about the file structure using flexible filtering and sorting capabilities. Also, group operations for deleting and moving (renaming) files by condition are supported. For example, search files with max depth: 2 (0 - unrestricted depth), file extension: "\***.csproj**", directory starts with: "**PowerLib.**" and output by directory name *descending order* and file name *ascending order*.
```csharp
  foreach (var item in new DirectoryInfo(@"D:\Projects\Github\PowerLib\").EnumerateFiles("*", 2, false, 
    fi => fi.Extension == ".csproj", (x, y) => Comparable.Compare(x.Name, y.Name, false),
    di => di.Name.StartsWith(@"PowerLib."), (x, y) => Comparable.Compare(x.Name, y.Name, false) * -1))
    Console.WriteLine("{0}", Path.Combine(item.DirectoryName, item.Name));
```
Console output:
```
D:\Projects\Github\PowerLib\PowerLib.System.Data.SqlTypes\PowerLib.System.Data.SqlTypes.csproj
D:\Projects\Github\PowerLib\PowerLib.System.Data.Linq\PowerLib.System.Data.Linq.csproj
D:\Projects\Github\PowerLib\PowerLib.System.Data\PowerLib.System.Data.csproj
D:\Projects\Github\PowerLib\PowerLib.System.ComponentModel.DataAnnotations\PowerLib.System.ComponentModel.DataAnnotations.csproj
D:\Projects\Github\PowerLib\PowerLib.System\PowerLib.System.csproj
D:\Projects\Github\PowerLib\PowerLib.SqlServer\PowerLib.SqlServer.csproj
D:\Projects\Github\PowerLib\PowerLib.SqlClr.Deploy.Utility\PowerLib.SqlClr.Deploy.Utility.csproj
D:\Projects\Github\PowerLib\PowerLib.SqlClr.Deploy\PowerLib.SqlClr.Deploy.csproj
D:\Projects\Github\PowerLib\PowerLib.EntityFramework\PowerLib.EntityFramework.csproj
```
Below are the prototypes of functions for working with the elements of the file system with the most complete number of arguments:
```csharp
//	Enumerate abstract filesystem items
public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos<DI>(this DI diParent, int maxDepth, bool excludeEmpty, Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison, Func<DI, IEnumerable<FileSystemInfo>> getChildren, Func<FileSystemInfo, bool> hasChildren)
where DI : FileSystemInfo;

//	Enumerate filesystem items
public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty, Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison);

//	Enumerate files
public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction, Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison, Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison);

//	Enumerate directories
public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty, Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparer);

// Move (rename) filesystem items
public static int MoveTo(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,  Func<FileSystemInfo, bool> predicate, Func<FileSystemInfo, string> replacing);

// Delete filesytem items
public static int Delete(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty, Func<FileSystemInfo, bool> predicate, bool recursive);
```
There are also functions with a predicate, the parameter of which, together with the element, is the context of the hierarchy of type IHierarchicalContext\<DirectoryInfo\> containing the list of ancestors:
```csharp
//	Enumerate abstract filesystem items
public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos<DI>(this DI diParent, int maxDepth, bool excludeEmpty, HierarchicalContext<DI> context, Func<ElementContext<FileSystemInfo, IHierarchicalContext<DI>>, bool> predicate, Comparison<FileSystemInfo> comparison, Func<DI, IEnumerable<FileSystemInfo>> getChildren, Func<FileSystemInfo, bool> hasChildren) where DI : FileSystemInfo;

//	Enumerate files
public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction, Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer, Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison);

//	Enumerate directories
public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty, Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison);

// Move (rename) filesystem items
public static int MoveTo(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty, Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Func<FileSystemInfo, string> replacing);
            
// Delete filesytem items
public static int Delete(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty, Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, bool recursive);
```

### Builders

Also, in namespace **PowerLib.System.Linq.Builders** there are classes that allow you to build complex predicative expressions, comparison expressions, access (initializetion, copy, call) expressions to fields, properties, and methods. Very useful for compiling predicative Queryable expressions depending on the current runtime filtering conditions. For example, predicate expression with anonymous type:
```csharp
  DateTime? birthday = new DateTime(1990, 1, 1);
  var predicate = PredicateBuilder
    .Matching(() => new { id = default(int), name = default(string), birthday = default(DateTime?) })
    .Match(t => t.name == "Mike");
  if (birthday.HasValue)
    predicate = predicate.And(t => t.birthday.HasValue && t.birthday.Value >= birthday.Value);
  dc.Persons
    .Select(t => new { id = t.Id, name = t.Name, birthday = t.Birthday })
    .Where(predicate.Expression)
    .ToArray();
```
The following example demonstrates how to create complex comparer using ComparerBuilder class:
```csharp
  public class Person
  {
    private int id;
    public string Name { get; set; }
    public DateTime Birthday { get; set; }
    public double GetSalary(int year);
  }

  var comparer = ComparerBuilder.Comparison<Person>()
    .Descend(t => t.Birthday)
    .Ascend(t => t.Name)
    .Descend(ReflectionBuilder.InstancePropertyAccess<Person, int>(pi => pi.Name == "id"))
    .Comparer(); //	result is IComparer<Person> type

  int year;

  var comparison = ComparerBuilder.Comparison<Person>()
    .Descend(t => t.GetSalary(year))
    .Ascend(t => t.Name)
    .Comparison(); // Comparison<Person> with external variable parameter 
```
The following example demonstrates how to create an object and call its private method using the Reflection Builder class:
```csharp
  public class MyExpr
  {
    private int _factor;

    private MyExpr(int factor)
    {
      _factor = factor;
    }

    private string Method(int x, int y)
    {
      return ((x + y) * _factor).ToString();
    }
  }
  
  public string Test()
  {
    var arguments = Tuple.Create(7, 8, 3);

    var factory = ReflectionBuilder
      .Construct<MyExpr>(ci => ci.GetParameters().Length == 1, c => c.ByVal(pi => pi.Position == 0, arguments.Item3))
      .Compile();

    var action = ReflectionBuilder
      .InstanceMethodCall(mi => mi.Name == "Method" && mi.IsPrivate,
        Call<MyExpr>.Expression(call => call.ByVal(pi => pi.Position == 0, arguments.Item1).ByVal(pi => pi.Position == 1, arguments.Item2).Return<string>()))
        .Compile();

    return action(factory());
  }
```
Continued...

---
## PowerLib.System.Data

Contains code to working with data objects.
The DataTypeAdapter class is intended to organize the conversion between the storage type and the representation type of data object. The class is abstract and declares two public properties: StoreValue - for the value on the storage side and ViewValue - for the value on the view side. Conversion between values is carried out on demand. The following classes are derived from the DataTypeAdapter:
* __BytesBinaryFormatterAdapter__
* __BytesBinarySerializeAdapter\<T\>__
* __StreamedCollectionAdapter\<T, V\>__
* __StringXmlSerializableAdapter\<T\>__
* __StringXmlSerializerAdapter__
* __XElementXmlSerializableAdapter\<T\>__
* __XElementXmlSerializerAdapter__

The PowerLib.SqlServer library provides a functional that allows you to work with regular (one-dimensional or multidimensional) arrays or collections of simple data types stored in a binary data type. On the client side, work with these types of data can be done either by calling the functions of the PowerLib.SqlServer library through EntityFramework or LINQ2SQL, or by converting to the representing type and vice versa. The following example shows the use of the StreamedCollectionAdapter for these purposes.
```csharp
public class SampleEntity
{
    private StreamedCollectionAdapter<int?, int[]> _indicesAdapter = new StreamedCollectionAdapter<int?, int[]>(null, null, false,
      s => new NulInt32StreamedArray(s, false, false), c => c.Select(t => t.Value).ToArray(),
      (s, c) => new NulInt32StreamedArray(s, SizeEncoding.B1, true, c.Select(v => (int?)v).Counted(c.Length), false, false));

    [Column("DimIndices")]
    public Byte[] RawIndices
    {
      get { return _indicesAdapter.StoreValue; }
      set { _indicesAdapter.StoreValue = value; }
    }

    [NotMapped]
    public int[] DimIndices
    {
      get { return _indicesAdapter.ViewValue; }
      set { _indicesAdapter.ViewValue = value; }
    }
}
```

---
## PowerLib.System.Data.Linq

Extends LINQ to SQL for working with functions from the PowerLib.SqlServer assembly. Simply inherit your DataContext from PowerLib.System.Data.Linq.PwrDataContext.

Samples:

1. Getting an entity person who has a phone number in XML containing three digits `555` before the four end digits. All filtering actions are performed on the SQL server side.

```csharp
  using (var dc = new MyDataContext())
  {
    var ns = @"def=http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ContactInfo"
           + @";crm=http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ContactRecord"
           + @";act=http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ContactTypes";
    var persons = dc.Person
      .Where(t => dc.regexIsMatch(dc.xmlEvaluateAsString(t.AdditionalContactInfo, "/def:AdditionalContactInfo/crm:ContactRecord/act:telephoneNumber/act:number/text()", ns), @"555-\d{4}$", RegexOptions.None) == true)
      .ToArray();
    }
```
Continued...

---
## PowerLib.EntityFramework

Extends EntityFramework for working with MSSQL server functions and stored procedures, and for working with functions from the PowerLib.SqlServer assembly. In CodeFirst inherit your DbContext from PowerLib.System.Data.Entity.PwrDbContext with same name, but in other namespace. In method OnModelCreating call this method base class implementation.

Samples:

1. Getting an entity person who has a phone number in XML containing three digits `555` before the four end digits. All filtering actions are performed on the SQL server side.

```csharp
  using (var dc = new PwrDbContext())
  {
    var ns = @"def=http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ContactInfo"
           + @";crm=http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ContactRecord"
           + @";act=http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ContactTypes";
    var persons = dc.Person
      .Where(t => dc.regexIsMatch(dc.xmlEvaluateAsString(t.AdditionalContactInfo,  "/def:AdditionalContactInfo/crm:ContactRecord/act:telephoneNumber/act:number/text()", ns), @"555-\d{4}$", RegexOptions.None) == true)
      .ToArray();
  }
```

Continued...

---
## PowerLib.System.Data.SqlTypes

Contains several dozens of user-defined types (UDTs) and functions:

- __SqlRegex__ - Represents a regular expression (System.Text.RegularExpressions.Regex type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlBigInteger__ - Represents a big integer (System.Numerics.BigInteger type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlComplex__ - Represents a complex number (System.Numerics.Complex type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlGradAngle__ - Represents a grad angle (PowerLib.System.Numerics.GradAngle type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlHourAngle__ - Represents a hour angle (PowerLib.System.Numerics.HourAngle type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlSexagesimalAngle__ - Represents a sexagesimal angle (PowerLib.System.Numerics.SexagesimalAngle type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlZipArchive__ - Represents a zip archive (System.IO.Compression.ZipArchive type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlRange__ - Represents a range - integer index and count (PowerLib.System.Range type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlUri__ - Represents an uri - Uniform Resource Identifier (URI) value (System.Uri type) to be stored in or retrieved from a database and and having its methods and properties.
- __Sql__<*ClrType*>__Collection__ - Represents collection items of <*Type*> type to be stored in or retrieved from a database. In SQL server collection names as <*DbType*>__Collection__.
- __Sql__<*ClrType*>__Array__ - Represents single dimension array of <*Type*> item type to be stored in or retrieved from a database. In SQL server single dimension array names as <*DbType*>__Array__.
- __Sql__<*ClrType*>__RegularArray__ - Represents multiple dimensions array of <*Type*> item type to be stored in or retrieved from a database. In SQL server multiple dimensions array names as <*DbType*>__RegularArray__.

The following table shows the mapping of names in the assembly and names in the SQL Server.

*ClrType* | *DbType*
---|---
BigInteger|HugeInt
Binary|Binary
Boolean|Bit
Byte|TinyInt
Complex|Complex
DateTime|DateTime
Double|DoubleFloat
GradAngle|GradAngle
Guid|Uid
HourAngle|HourAngle
Int16|SmallInt
Int32|Int
Int64|BigInt
Range|Range
SexagesimalAngle|SexagesimalAngle
Single|SingleFloat
String|String


To deploy the assembly on an MSSQL server, use the sqlclrdu.exe utility described below. For example,
```
>sqlclrdu.exe create -permission:unsafe -script:script.sql -schema:pwrlib -connection:"Data Source=MASTER\MSSQLSERVER2016;Initial Catalog=AdventureWorks2016;Integrated Security=True;Connect Timeout=15;" "PowerLib.System.Data.SqlTypes.dll"
```
Dependent assemblies without sqlclr contents are installed with the -invisible option. Assemblies from GAC deploy by strong assembly name with -strong option. For example,
```
>sqlclrdu.exe create -invisible -strong -script:test.sql "System.Runtime.Serialization, version=4.0.0.0, culture=neutral, publickeytoken=b77a5c561934e089"
```

Continued...

---
## PowerLib.SqlServer

Contains more than a thousand functions that integrates into the SQL server. All functions can be divided into several categories: binary type manipulation, text manipulation (regular expression support), compression, cryptography, xml type manipulation (xpath query, xsl transformation, convert to and from json) and base types collections, single and multiple dimensions array support. All functions integrate into LINQ to SQL and EntityFramework.

To deploy the assembly on an MSSQL server, use the sqlclrdu.exe utility described below. For example,
```
sqlclrdu.exe create -permission:unsafe -script:script.sql -schema:pwrlib -map:"PowerLib.System.Data.map" -connection:"Data Source=MASTER\MSSQLSERVER2016;Initial Catalog=AdventureWorks2016;Integrated Security=True;Connect Timeout=15;" "PowerLib.SqlServer.dll"
```
Dependent assemblies without sqlclr contents are installed with the -invisible option. Assemblies from GAC deploy by strong assembly name with -strong option. For example,
```
>sqlclrdu.exe create -invisible -strong -script:test.sql "System.Runtime.Serialization, version=4.0.0.0, culture=neutral, publickeytoken=b77a5c561934e089"
```

Continued...

---
## PowerLib.SqlClr.Deploy

A library that examines the specified assembly for sql clr objects and generates an SQL script for their registration, modification and deletion.

---
## PowerLib.SqlClr.Deploy.Utility

The sqlclrdu.exe utility is designed to deploy any sql clr assemblies on the MSSQL server.

```
> sqlclrdu.exe

Microsoft SQL server CLR assembly deployment utility.

syntax:
  sqlclrdu.exe <command> <options> <assembly>

commands:
  create - Create assembly on SQL server database.
  alter - Alter assembly on SQL server database.
  drop - Drop assembly on SQL server database.
  manage - Manage database (-enable-clr, -trustworthy keys accepted).

options:
  -assembly:<assembly_name> - Assembly name in SQL server.
  -script:<script_file> - SQL script file path.
  -map:<map_file> - Map referenced types to SQL Server database objects.
  -encoding:<script_encoding> - SQL script file encoding.
  -connection:<connection_string> - SQL server connection string.
  -permission:[safe|ext_access|unsafe] - One of three different levels of security in which your code can run.
  -owner:<owner> - Database user or role.
  -schema:<schema> - Database schema.
  -unchecked - Applied with ALTER ASSEMBLY command and add UNCHECKED DATA option.
  -invisible - Only register assembly without its contents.
  -append - Append to SQL script file.
  -strong - Specifies that the assembly is defined by a strong name.
  -enable-clr - Sets 'clr enabled' option for SQL server.
  -trustworthy - Sets 'trustworthy' option for database.

comments:
  If string values contain spaces, they must be enclosed in double quotes.
  For example, -connection:"Data Source = MASTER\MSSQLSERVER2016;Initial Catalog = AdventureWorks2016; Integrated Security = True"
```
