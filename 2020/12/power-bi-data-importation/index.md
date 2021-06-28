
## Clarifying the Use of Queries and Methods for Data Importation with Power BI

Power BI is an industry leader in self-service analytics. This service delivers large-scale business intelligence into the hands of the end user by leveraging familiar tools found in Excel and Analysis Services into a single streamlined application. Power BI takes advantage of data importation and query optimization techniques such as query folding and VertiPaq compression.

However, the more Power BI tries to simplify the user experience, the less direct control users have on what happens behind the scenes. To help clarify the process of Power BI data importation, this blog will address three important areas of the service:

1. Explore the methods by which data is imported into Power BI
2. Explain what importation processes maximize efficiency
3. Reveal how these processes are relevant to query folding

For this exploration, I imported data directly from an Azure SQL Database into Power BI desktop.

### Question: At what point are queries executed against the source database from Power BI using Power Query?

There are three phases.

#### Phase 1

Phase one happens during the selection of database from “Get Data”

- This phase has the lowest computational cost of the three.
- During this phase, only schema and table descriptions are selected.
- SQL examples:

select r.\[ROUTINE\_SCHEMA\], r.\[ROUTINE\_NAME\], r.\[ROUTINE\_TYPE\], p.create\_date \[CREATED\_DATE\], p.modify\_date \[MODIFIED\_DATE\], cast(e.value as nvarchar(max)) \[DESCRIPTION\] from \[INFORMATION\_SCHEMA\].\[ROUTINES\] r join sys.schemas s on s.name = r.\[ROUTINE\_SCHEMA\] join sys.objects p on p.name = r.\[ROUTINE\_NAME\] and p.schema\_id = s.schema\_id and p.parent\_object\_id = 0 left outer join (select null major\_id, null minor\_id, null class, null name, null value) e on p.object\_id = e.major\_id and e.minor\_id = 0 and e.class = 1 and e.name = 'MS\_Description'

select t.\[TABLE\_CATALOG\], t.\[TABLE\_SCHEMA\], t.\[TABLE\_NAME\], t.\[TABLE\_TYPE\], tv.create\_date \[CREATED\_DATE\], tv.modify\_date \[MODIFIED\_DATE\], cast(e.value as nvarchar(max)) \[DESCRIPTION\] from \[INFORMATION\_SCHEMA\].\[TABLES\] t join sys.schemas s on s.name = t.\[TABLE\_SCHEMA\] join sys.objects tv on tv.name = t.\[TABLE\_NAME\] and tv.schema\_id = s.schema\_id and tv.parent\_object\_id = 0 left outer join (select null major\_id, null minor\_id, null class, null name, null value) e on tv.object\_id = e.major\_id and e.minor\_id = 0 and e.class = 1 and e.name = 'MS\_Description'

#### Phase 2

This phase occurs when entering Power Query Editor (Transform Data)

- Phase 2 has a more significant cost than phase one and includes selection of the top N (4096 here) values for each table as well as some specific filtered select statements.
- This phase samples the database for use in Power Query filtering and transformations.
- SQL examples:

select \[$Ordered\].\[SalesOrderID\], \[$Ordered\].\[RevisionNumber\], \[$Ordered\].\[OrderDate\], \[$Ordered\].\[DueDate\], \[$Ordered\].\[ShipDate\], \[$Ordered\].\[Status\], \[$Ordered\].\[OnlineOrderFlag\], \[$Ordered\].\[SalesOrderNumber\], \[$Ordered\].\[PurchaseOrderNumber\], \[$Ordered\].\[AccountNumber\], \[$Ordered\].\[CustomerID\], \[$Ordered\].\[ShipToAddressID\], \[$Ordered\].\[BillToAddressID\], \[$Ordered\].\[ShipMethod\], \[$Ordered\].\[CreditCardApprovalCode\], \[$Ordered\].\[SubTotal\], \[$Ordered\].\[TaxAmt\], \[$Ordered\].\[Freight\], \[$Ordered\].\[TotalDue\], \[$Ordered\].\[Comment\], \[$Ordered\].\[rowguid\], \[$Ordered\].\[ModifiedDate\] from ( select \[\_\].\[SalesOrderID\], \[\_\].\[RevisionNumber\], \[\_\].\[OrderDate\], \[\_\].\[DueDate\], \[\_\].\[ShipDate\], \[\_\].\[Status\], \[\_\].\[OnlineOrderFlag\], \[\_\].\[SalesOrderNumber\], \[\_\].\[PurchaseOrderNumber\], \[\_\].\[AccountNumber\], \[\_\].\[CustomerID\], \[\_\].\[ShipToAddressID\], \[\_\].\[BillToAddressID\], \[\_\].\[ShipMethod\], \[\_\].\[CreditCardApprovalCode\], \[\_\].\[SubTotal\], \[\_\].\[TaxAmt\], \[\_\].\[Freight\], \[\_\].\[TotalDue\], \[\_\].\[Comment\], \[\_\].\[rowguid\], \[\_\].\[ModifiedDate\] from \[SalesLT\].\[SalesOrderHeader\] as \[\_\] where \[\_\].\[SalesOrderID\] = 71946 ) as \[$Ordered\] order by \[$Ordered\].\[SalesOrderID\]

select top 4096 \[$Ordered\].\[SystemInformationID\], \[$Ordered\].\[Database Version\], \[$Ordered\].\[VersionDate\], \[$Ordered\].\[ModifiedDate\] from \[dbo\].\[BuildVersion\] as \[$Ordered\] order by \[$Ordered\].\[SystemInformationID\]

#### Phase 3

The third phase runs when closing Power Query and applying changes

- Of all the phases, it’s the most expensive operation because it includes the complete selection from all the tables.
- SQL examples:

Select \[$Ordered\].\[SalesOrderID\], \[$Ordered\].\[RevisionNumber\], \[$Ordered\].\[OrderDate\], \[$Ordered\].\[DueDate\], \[$Ordered\].\[ShipDate\], \[$Ordered\].\[Status\], \[$Ordered\].\[OnlineOrderFlag\], \[$Ordered\].\[SalesOrderNumber\], \[$Ordered\].\[PurchaseOrderNumber\], \[$Ordered\].\[AccountNumber\], \[$Ordered\].\[CustomerID\], \[$Ordered\].\[ShipToAddressID\], \[$Ordered\].\[BillToAddressID\], \[$Ordered\].\[ShipMethod\], \[$Ordered\].\[CreditCardApprovalCode\], \[$Ordered\].\[SubTotal\], \[$Ordered\].\[TaxAmt\], \[$Ordered\].\[Freight\], \[$Ordered\].\[TotalDue\], \[$Ordered\].\[Comment\], \[$Ordered\].\[rowguid\], \[$Ordered\].\[ModifiedDate\] from \[SalesLT\].\[SalesOrderHeader\] as \[$Ordered\] order by \[$Ordered\].\[SalesOrderID\]

Metrics: Database-Throughput-Unit (DTU) utilization graph on SQL database. DTU measures are a combination of CPU, I/O, and log flushes/second.

![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2020/12/power-bi-data-importation/images/image001.png)

Chart of DTU by Stage

### Question: Is there a noticeable difference between Import > Filter columns vs Filter columns > Import?

I believe that there is a slight improvement in simple query performance because users can specify what columns are imported into Power Query by tweaking the SQL statement in the advanced options. Unfortunately, doing so will **_disable Query Folding_** for any future transformations, which I will explain later.

Explored methods of importing data into Power BI:

1. I loaded only wanted tables, then filtered out columns. For advanced options: select \* from tables.
2. I selected wanted tables from PowerQuery but didn't use advanced options.
3. I loaded only a specific column from with an SQL select statement. For advanced options: select x from table.
4. I selected a specific column from PowerQuery without using advanced options.

#### DTI Utilization vs Methods of Import

![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2020/12/power-bi-data-importation/images/image002-1024x688.png)

Chart of DTU Utilization vs Methods of Import

From the above Cost of Imports and Methods graph, we see higher DTU utilization on methods two and four which pertain to not using custom SQL selection before import of data into Power Query. In contrast, methods one and three use specific SQL select statements in the advanced editor and result in fewer and cheaper queries to the source database.

This behavior is consistent with what we found earlier, because Power BI skips the first two query phases and jumps directly to data importation. Though specific SQL select statements may reduce the cost of the initial import on the source database in simple queries, they may result in a higher computational load on Power BI by disabling query folding.

<table><tbody><tr><td>Method 1</td><td><strong><em>query against server: </em></strong><em>select * from saleslt.customer</em></td></tr><tr><td>Method 2</td><td><strong><em>queries against server:</em></strong></td></tr><tr><td>2020-10-13T20:41:09.6600000</td><td>SELECT c.name AS column_name, t.Name AS column_type, CASE WHEN EXISTS ( SELECT NULL FROM sys.indexes INNER JOIN sys.index_columns ic ON ic.object_id = c.object_id LEFT OUTER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id WHERE i.is_primary_key = 1 AND ic.column_id = c.column_id ) THEN CAST(1 as bit) ELSE CAST(0 as bit) END AS is_primary_key, c.is_nullable, c.is_identity FROM sys.columns c INNER JOIN sys.types t ON c.user_type_id = t.user_type_id WHERE c.object_id = OBJECT_ID('[SalesLT].[Customer]') ORDER BY c.column_id</td></tr><tr><td>2020-10-13T20:35:59.1400000</td><td>select top 4096 [$Ordered].[CustomerID] from ( select [CustomerID] from [SalesLT].[Customer] as [$Table] ) as [$Ordered] order by [$Ordered].[CustomerID]</td></tr><tr><td>2020-10-13T20:35:35.0070000</td><td>select top 4096 [$Ordered].[CustomerID], [$Ordered].[NameStyle], [$Ordered].[Title], [$Ordered].[FirstName], [$Ordered].[MiddleName], [$Ordered].[LastName], [$Ordered].[Suffix], [$Ordered].[CompanyName], [$Ordered].[SalesPerson], [$Ordered].[EmailAddress], [$Ordered].[Phone], [$Ordered].[PasswordHash], [$Ordered].[PasswordSalt], [$Ordered].[rowguid], [$Ordered].[ModifiedDate] from [SalesLT].[Customer] as [$Ordered] order by [$Ordered].[CustomerID]</td></tr></tbody></table>

### Question: What is query folding?

Query folding is the process used by Power Query to send transformations to the source database. First, the database performs these transformations, then data is imported into Power BI. These transformations, where applicable, can be viewed in Power Query under “View Native Query.” Query folding is important for performance because it can drastically decrease the amount of data that is imported into Power BI.

![Query folding: screenshot showing view native query.](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2020/12/power-bi-data-importation/images/image003.png)

Query folding will be disabled and as a result “View Native Query” will no longer be an option under two conditions:

1. It is disabled when using unsupported sources, such as flat files.
2. It is disabled when applying certain transformations, such as “removing rows with errors.”

![screenshot showing query folding options.](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2020/12/power-bi-data-importation/images/image004.png)

It’s important to point out that using a custom SQL import statement **will prevent folding**. In his blog, [_Query Folding in Power Query to Improve Performance_](https://www.mssqltips.com/sqlservertip/3635/query-folding-in-power-query-to-improve-performance/), Koen Verbeeck, writes, “If you write your own SQL statement to fetch the data, any subsequent transformation on that data will not use query folding.”

**Important note**: If you intend to perform any further transformations on the data, I don't recommend writing your own SQL statement to initially fetch the data.

### Understanding the Data Importation of Power BI Is Only the Beginning

To sum up, Power BI offers a simplified experience to users. However, users need an explicit understanding of the processes that occur behind the scenes to more effectively utilize this powerful software.

Stay tuned for more tips and explorations regarding Power BI.

### Want More?

Here are some references that I’ve found useful in my quest to grow my understanding of Power BI’s powerful features.

- [https://docs.microsoft.com/en-us/power-bi/guidance/power-query-folding](https://docs.microsoft.com/en-us/power-bi/guidance/power-query-folding)
- [https://www.mssqltips.com/sqlservertip/4563/power-bi-native-query-and-query-folding/](https://www.mssqltips.com/sqlservertip/4563/power-bi-native-query-and-query-folding/)
- [https://www.sqlgene.com/2019/09/27/a-comprehensive-guide-to-power-bi-performance-tuning/](https://www.sqlgene.com/2019/09/27/a-comprehensive-guide-to-power-bi-performance-tuning/)
