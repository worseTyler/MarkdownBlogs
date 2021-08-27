

## Oracle Data Provider for .NET
#
Oracle Data Provider for .NET (ODP.NET) uses Object-relational mapping (ORM) to allow developers to write object-oriented code against a Model instead of writing direct queries into the database. With Entity Framework gaining full support from Microsoft and Microsoft developers, it is critical that there exists an ORM that works well with Entity Framework when required to work with an Oracle database.

In the past, the only viable option available was an application called dotConnect from DevArt. However, dotConnect had a major downfall in that running dotConnect version 8.4 on build server A cannot build an application using dotConnect version 8.5. I am currently in a role where we are supporting a multitude of applications ranging from VB6 to MVC 5 so having a different build server for each version of Devart can be quite painful.

A colleague of mine discovered ODP.NET and gave it a try. He had a positive experience, and I decided I would look into it. I was pleasantly surprised to see a NuGet package for ODP.NET and immediately spun up a new project with the goal of using ODP.NET to integrate between Entity Framework and Oracle using a database first. Below are the steps I used to accomplish my goal.

As a prerequisite for integration with Visual Studio, this walk-through requires Oracle Developer Tools for Visual Studio 2015 which you can download via:

https://www.oracle.com/technetwork/topics/dotnet/downloads/index.html

We can start by creating a console application in Visual Studio 2015. File -> New -> Project then under Windows choose Console Application and name ODP.NET.

![Screen Shot 2016-04-26 at 2.57.36 PM](https://intellitect.com/wp-content/uploads/2016/04/Screen-Shot-2016-04-26-at-2.57.36-PM.png "Oracle Data Provider for .NET")

There are two NuGet packages we will need to install, and the first will be Entity Framework. Right-click the Solution at the top of the Solution Explorer and navigate to Manage NuGet Packages for Solution. In the search bar, type “Entity Framework”. In the screenshot below, I’m grabbing the latest stable version 6.1.3 for this walk-through. Select EF and check ODP.NET the name of our project in the right-hand panel of the NuGet - Solution tab in Visual Studio, and click Install.

![Manage Packages](https://intellitect.com/wp-content/uploads/2016/04/Manage-Packages.png "Oracle Data Provider for .NET")

The second package that is needed is Oracle.ManagedDataAccess.EntityFramework by Oracle I will be getting version 12.1.2400.

![Oracle.ManagedDataAccess.EntityFramework](https://intellitect.com/wp-content/uploads/2016/04/Oracle.ManagedDataAccess.EntityFramework.png "Oracle Data Provider for .NET")

Nuget will gather dependencies and discover that Oracle.ManagedDataAccess.EntityFramework also needs Oracle.ManagedDataAccess and will install that package for you as well.

After installing our two NuGet packages, it is time to connect to our Oracle database with the help of the Oracle Developer tools we installed as a prerequisite of this walk-through. In VS go to Tools -> Connect to Database. Then, in the Add Connection form fill out the details to connect to your existing Oracle DB. In my case I need to fill out Username and Password, and then change Connection Type to EZ Connect and fill out Database host name, Port number, Database service name, and Data source name.

![Add Connection](https://intellitect.com/wp-content/uploads/2016/04/Add-Connection.png "Oracle Data Provider for .NET")

After adding the connection, we are ready to add the Entity Data Model. Right-click on the ODP.NET project in solution explorer, navigate to Add -> New Item, select ADO.NET Entity Data Model, and name it OracleModel.

![Add New Item](https://intellitect.com/wp-content/uploads/2016/04/Add-New-Item.png "Oracle Data Provider for .NET")

In the Entity Data Model wizard, select EF Designer from Database then hit next.

![Entity Data Model Wizard](https://intellitect.com/wp-content/uploads/2016/04/Entity-Data-Model-Wizard.png "Oracle Data Provider for .NET")

Then choose your connection. You should see the DB we connected to earlier via the Database Explorer. Select whether or not you want to include or exclude sensitive data in your connection string.

![Entity Data Model Wizard 2](https://intellitect.com/wp-content/uploads/2016/04/Entity-Data-Model-Wizard-2.png "Oracle Data Provider for .NET")

For the final step in the Entity Data Model Wizard, select the Tables/Views/Stored Procedures that you would like to include in your model then click Finish.

![Entity Data Model Wizard 3](https://intellitect.com/wp-content/uploads/2016/04/Entity-Data-Model-Wizard-3.png "Oracle Data Provider for .NET")

Your .edmx will generate, which includes your context.

![edmx](https://intellitect.com/wp-content/uploads/2016/04/edmx.png "Oracle Data Provider for .NET")

Now we are ready to connect to the context and make queries to our database. In your Program.cs you can add the following code and access your oracle database.

```csharp
using (Entities context = new Entities())
{
    var test = context.HLAs.Where(x => x.HOME_LOCATION ==
               "Spokane/CdA").FirstOrDefault();
}
﻿
```

After using ODP.NET in a few projects, I have also discovered that the error messages that bubble up from ODP.NET are cleaner and more descriptive than many of the errors generated through Devart. In one example, I went from an error in Devart stating metadata resources could not be loaded. After removing Devart and installing ODP.NET, the first error I encountered was “No connection string named 'DbEntities' could be found in the application config file”.

I hope you enjoyed going through this walk-through, and I hope it helps you in any future work that requires EF and Oracle.

_Written by Mark Salvino._
