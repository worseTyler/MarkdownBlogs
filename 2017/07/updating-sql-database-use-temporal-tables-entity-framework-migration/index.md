

In a couple of recent applications that IntelliTect developed for clients, we decided to use SQL Server Temporal Tables in order to track changes in the database over time. The apps we were working on were ASP.NET Core web applications, using Entity Framework Core (EF).  While we could have simply modified the databases using queries against the database server directly, we chose to perform an EF migration to add the system versioning.  This way, we could bring up a new database at any time using the standard EF migration pattern, without any extra external steps.

Adding temporal tables to a migration was very simple:

1. Create a new empty migration
    
    1. dotnet ef migrations add AddTemporalTables
    
    1. This will create a skeleton migration with an Up(MigrationBuilder migrationBuilder) and a Down(MigrationBuilder migrationBuilder) method, both of which contain no code
2. Modify the skeleton migration as shown below
3. Apply the migration
    1. dotnet ef database update

### Migration Code

```
public partial class AddTemporalTables : Migration
{
   List tablesToUpdate = new List
   {
      "Table1",
      "Table2",
      "Table3",
      "Table4"
   };
   
   protected override void Up(MigrationBuilder migrationBuilder)
   {
      migrationBuilder.Sql($"CREATE SCHEMA History");
      foreach (var table in tablesToUpdate)
      {
         string alterStatement = $@"ALTER TABLE {table} ADD SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START HIDDEN
         CONSTRAINT DF\_{table}\_SysStart DEFAULT GETDATE(), SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END HIDDEN
         CONSTRAINT DF\_{table}\_SysEnd DEFAULT CONVERT(datetime2 (0), '9999-12-31 23:59:59'),
         PERIOD FOR SYSTEM\_TIME (SysStartTime, SysEndTime)";
         migrationBuilder.Sql(alterStatement);
         alterStatement = $@"ALTER TABLE {table} SET (SYSTEM\_VERSIONING = ON (HISTORY\_TABLE = History.{table}));";
         migrationBuilder.Sql(alterStatement);
      }
   }

   protected override void Down(MigrationBuilder migrationBuilder)
   {
      foreach (var table in tablesToUpdate)
      {
         string alterStatement = $@"ALTER TABLE {table} SET (SYSTEM\_VERSIONING = OFF);";
         migrationBuilder.Sql(alterStatement);
         alterStatement = $@"ALTER TABLE {table} DROP PERIOD FOR SYSTEM\_TIME";
         migrationBuilder.Sql(alterStatement);
         alterStatement = $@"ALTER TABLE {table} DROP DF\_{table}\_SysStart, DF\_{table}\_SysEnd";
         migrationBuilder.Sql(alterStatement);
         alterStatement = $@"ALTER TABLE {table} DROP COLUMN SysStartTime, COLUMN SysEndTime";
         migrationBuilder.Sql(alterStatement);
         alterStatement = $@"DROP TABLE History.{table}";
         migrationBuilder.Sql(alterStatement);
      }
      migrationBuilder.Sql($"DROP SCHEMA History");
   }
}

```

_Written by Jason Peterson_
