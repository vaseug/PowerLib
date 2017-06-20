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

Continued...

---
### PowerLib.SqlServer

Contains more than a thousand functions that integrates into the SQL server. All functions can be divided into several categories: binary type manipulation, text manipulation (regular expression support), compression, cryptography, xml type manipulation (xpath query, xsl transformation, convert to and from json) and base types collections, single and multiple dimensions array support. All functions integrate into LINQ to SQL and EntityFramework.

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
