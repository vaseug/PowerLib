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

---
### PowerLib.System

Contains many classes, structures, interfaces and extension methods that expedite and optimize the development process.

**PowerLib.System.Collection.PwrList\<T>** - A list whose operation with an internal buffer is based on the round-robin algorithm. It implements the memory allocation and deallocation management interfaces and contains many methods for working with its elements.

**PowerLib.System.Collection.PwrSortedList\<T>** - Sorted list based on the comparator of elements represented by the interface *IComparer\<T>* or the delegate *Comparison\<T>*. The values of the elements participating in the comparison must be unchanged during the storage in the list. The list can be restricted by storing only unique values. There are also options that control the addition of duplicate values: to the beginning of the chain, to the end, or arbitrarily.

**PowerLib.System.Collection.PwrKeySortedList\<K, T>** - Sorted list based on the element key comparator represented by the *IComparer\<K>* interface or the *Comparison\<K>* delegate. The key extraction is specified by the delegate *Func\<T, K>*. The key values of the elements participating in the comparison must be unchanged during the storage in the list. The list can be restricted by storing only unique values. There are also options that control the addition of duplicate values: to the beginning of the chain, to the end, or arbitrarily.

**PowerLib.System.Collection.PwrStack\<T>** - A general stack with additional range operations.
**PowerLib.System.Collection.PwrSortedStack\<T>** - A sorted stack (priority stack).
**PowerLib.System.Collection.PwrKeySortedStack\<T>** - A key sorted stack (priority stack).

**PowerLib.System.Collection.PwrQueue\<T>** - A general queue with additional range operations.
**PowerLib.System.Collection.PwrSortedQueue\<T>** - A sorted queue (priority queue).
**PowerLib.System.Collection.PwrKeySortedQueue\<T>** - A key sorted queue (priority queue).

**PowerLib.System.Collection.PwrDeque\<T>** - A general deque with additional range operations.
**PowerLib.System.Collection.PwrSortedDeque\<T>** - A sorted deque (priority deque).
**PowerLib.System.Collection.PwrKeySortedDeque\<T>** - A key sorted deque (priority deque).

The **ListExtension** class contains a number of extension methods for working with lists.
There are many classes in namespace **PowerLib.System.Collection.Matching** for working with items matching and comparison. Also, in namespace **PowerLib.System.Linq.Builders** there are classes that allow you to build complex predicative expressions, comparison expressions, access expressions to fields, properties, and methods. Very useful for compiling predicative Queryable expressions depending on the current runtime filtering conditions. For example, predicate expression with anonymous type:
```csharp
      DateTime? birthday = new DateTime(1990, 1, 1);
      var predicate = PredicateBuilder.Matching(() => new { id = default(int), name = default(string), birthday = default(DateTime?) })
        .Match(t => t.name == "Mike");
      if (birthday.HasValue)
        predicate = predicate.And(t => t.birthday.HasValue && t.birthday.Value >= birthday.Value);
      dc.Persons
        .Select(t => new { id = t.Id, name = t.Name, birthday = t.Birthday })
        .Where(predicate.Expression)
        .ToArray();
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

For working with arrays jagged and regular (one-dimensional, multidimensional), there are many classes and methods of extensions.

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
A large section to working with linked and tree data (trees, graphs) includes many structures for storing tree elements and methods for working with them (including LINQ extensions) to be posted later.

Continued...

---
### PowerLib.System.Data

Contains code to working with data objects.

Continued...

---
### PowerLib.System.Data.Linq

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
			.Where(t => dc.regexIsMatch(
            	dc.xmlEvaluateAsString(t.AdditionalContactInfo,
					"/def:AdditionalContactInfo/crm:ContactRecord/act:telephoneNumber/act:number/text()", ns),
				@"555-\d{4}$", RegexOptions.None) == true)
			.ToArray();
	}

```
Continued...

---
### PowerLib.EntityFramework

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
			.Where(t => dc.regexIsMatch(
            	dc.xmlEvaluateAsString(t.AdditionalContactInfo,
					"/def:AdditionalContactInfo/crm:ContactRecord/act:telephoneNumber/act:number/text()", ns),
				@"555-\d{4}$", RegexOptions.None) == true)
			.ToArray();
	}

```

Continued...

---
### PowerLib.System.Data.SqlTypes

Contains several dozens of user-defined types (UDTs) and functions:

- __SqlRegex__ - Represents a regular expression (System.Text.RegularExpressions.Regex type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlBigInteger__ - Represents a big integer (System.Numerics.BigInteger type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlComplex__ - Represents a complex number (System.Numerics.Complex type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlGradAngle__ - Represents a grad angle (PowerLib.System.Numerics.GradAngle type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlHourAngle__ - Represents a hour angle (PowerLib.System.Numerics.HourAngle type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlSexagesimalAngle__ - Represents a sexagesimal angle (PowerLib.System.Numerics.SexagesimalAngle type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlZipArchive__ - Represents a zip archive (System.IO.Compression.ZipArchive type) to be stored in or retrieved from a database and and having its methods and properties.
- __SqlRange__ - Represents a range - integer index and count (PowerLib.System.Range type) to be stored in or retrieved from a database and and having its methods and properties.
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
### PowerLib.SqlServer

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
### PowerLib.SqlClr.Deploy

A library that examines the specified assembly for sql clr objects and generates an SQL script for their registration, modification and deletion.

Continued...

---
### PowerLib.SqlClr.Deploy.Utility

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

Continued...
