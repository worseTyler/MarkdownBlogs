

## Upgrading SQL Server LocalDb
#
Estimated reading time: 3 minutes

### The Problem

The Microsoft SQL Server LocalDb has become a staple tool for our development teams. A number of years ago they removed the version number from the connection string, which was a great move.

I recently needed to upgrade to 2016 in order to import a .bacpac file from an Azure database. So I installed SQL Server Express 2016 upgrading my SQL instance. However, it didn't upgrade my LocalDb instance as well. I searched around, but didn't find many complete answers.

You might also get this error: Internal Error. The internal target platform type sqlAzureV12DatabaseSchemaProvider does not support schema file version '3.5'. This problem happens when you do not have the latest SSMS. Sometimes you even need a pre-release version of SSMS to get the .bacpac files to load. Here is a link to the [latest SSMS](https://msdn.microsoft.com/en-us/library/mt238290.aspx). Thanks to Derek Howard for pointing this one out.

Note that I could have just created a new instance, but I didn't want to change connection strings in my existing applications. You can also have several instances of LocalDb set up at the same time will with different names.

### The Process

There may be better ways, but this worked in my case. This process should work for just about any version of LocalDb, at least as of 2016. The basic process is as follows.

1. Install the latest version of SQL Server (Full or Express)
2. Stop LocalDb
3. Remove the existing LocalDb instance
4. Add a new LocalDb instance
5. Start the new instance

The first step is pretty self explanatory, and a web search will take you to the Microsoft download site.

### Warning!

The following approach will cause all your existing databases to disconnect from your instance. The instance name will be the same, but you will need to reconnect them if you are not connecting via a filename in your connection string. If you have your application set up to recreate the database, this will likely fail because the default filename for the database already exists, it just isn't attached to your LocalDb instance.

The default location for databases is: C:\\Users\\[account name]\\AppData\\Local\\Microsoft\\Microsoft SQL Server Local DB\\Instances\\[instance name]

You can just reattach the databases you need with SQL Server Management Studio.

### The Commands

These command should be run from a command window (cmd.exe). The location doesn't seem to matter. Replace MSSQLLocalDB with the name of your instance.

The following command will give you a list of instances.

```powershell
C:\\>sqllocaldb info
```

These are the commands to stop, delete, create, and start your instance. This will cause all your databases to detach, see note above.

```powershell
C:\\>sqllocaldb stop MSSQLLocalDB
LocalDB instance "MSSQLLocalDB" stopped.

C:\\>sqllocaldb delete MSSQLLocalDB
LocalDB instance "MSSQLLocalDB" deleted.

C:\\>sqllocaldb create MSSQLLocalDB
LocalDB instance "MSSQLLocalDB" created with version 13.0.1601.5.

C:\\>sqllocaldb start MSSQLLocalDB
LocalDB instance "MSSQLLocalDB" started.
```

Note: the create command will create a new instance of LocalDb with the latest version. You can specify the version number if you want a specific version.

### Want More?

Learn more about SQL and read about updating your SQL database [here](/updating-sql-database-use-temporal-tables-entity-framework-migration/)!

![](https://intellitect.com/wp-content/uploads/2021/04/blog-job-ad-2-1024x129.png)
