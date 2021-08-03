

Welcome back to part two of our discussion on logging and tracing for .NET applications. In [part one](/building-a-logger-using-caller-info-attributes-part-1/), we discussed a feature introduced in C# 5 that allows us to log detailed file, member, and line number information when tracing. Today, we’ll show how to build a wrapper for NLog that can utilize this functionality. We’ll also demonstrate how we can redirect our NLog output to Azure table storage. This will set us up for the final part of our blog, which will be building a log viewer for viewing logs stored in Azure table storage.

[NLog](https://www.nuget.org/packages/NLog/) is a logging platform for .NET with rich log routing and management capabilities. At Intellitect, we have used NLog for several large projects, and it has served us well. We have benefited from the configurable log routing to store our logs in the Windows Event Log, SQL Server, and simple text files. Today we will demonstrate adding another log storage destination, Windows Azure Table Storage. Before we can get to routing the logs, however, let’s build a logger in C# that wraps some of the common methods that are used when logging with NLog. Instead of wrapping NLog, we could of course fork the open source NLog project from GitHub and make our changes there, but to keep it simple, we will create a wrapper.

As you will see, our logging class is going to be very simple. Note that for simplicity, I’ve only included implementations for two of the numerous logging methods that NLog has. Specifically, I have code for the Trace method, which takes a message and up to four optional parameters that will be used to format the string using String.Format(...). This method logs using NLog level of Trace. I’ve also included code for the Error method, which takes an exception, a message, and up to four optional parameters that will be used to format the string using String.Format(...). This method logs using NLog level of Error. It is a trivial task to generate wrappers for any of the other NLog methods.

Note that in both of these cases, we can’t use a variable length params argument, because of the requirement that the CallerMemberName, CallerFilePath, and CallerLineNumber must be optional.

```csharp
using System;
using NLog;
using NLog.Config;
namespace Intellitect.Logging
{
   public static class Logger
   {
      private static readonly NLog.Logger _Logger = LogManager.GetCurrentClassLogger();
      private static string GetCallerInfoString(string memberName, string sourceFilePath, int sourceLineNumber)
      {
         return string.Format("{0}({1}):{2}", sourceFilePath, sourceLineNumber, memberName);
      }
      /\*
      Note that we cant use params string[] args here because the memberName, sourceFilePath, and sourceLineNumber
      \* arguments MUST be optional in order to get file, member, and line number information.
      \*/
      public static void Trace(
            string message,
            object object1 = null,
            object object2 = null,
            object object3 = null,
            object object4 = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
      {
         var messageString = string.Format(message, object1, object2, object3, object4);
         _Logger.Trace(string.Format("{0}:{1}", GetCallerInfoString(memberName, sourceFilePath, sourceLineNumber), messageString));
         System.Diagnostics.Trace.WriteLine(messageString);
      }

      public static void Error(
            Exception exception,
            string message,
            object object1 = null,
            object object2 = null,
            object object3 = null,
            object object4 = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
      {
         var messageString = string.Format(message, object1, object2, object3, object4);
         _Logger.Error(exception, string.Format("{0}:{1}", GetCallerInfoString(memberName, sourceFilePath, sourceLineNumber), messageString));
         System.Diagnostics.Trace.WriteLine(messageString);
      }
   }
}
```

As you can see from the above code, our wrapper is very simple. Our class is static, and has reference to a single static instance of the NLog logger. For each wrapped NLog method, we simply take the input and format it using string.Format(...), then log it using the NLog logger. The only thing we’re changing, is always outputting the caller, file, and line number where the logging method was called from, by using the Caller info attributes that we discussed in [part one](/building-a-logger-using-caller-info-attributes-part-1/) of our blog. Additionally, we are tracing the message using System.Diagnostics.Trace, such that the data can be seen when running from within Visual Studio.

Now that we’ve essentially built the logger, let’s take a look at how we can route the output of the logger to Azure table storage. Due to the simple configuration model that NLog uses, redirecting our log output to Azure table storage is very simple. I’ll describe the steps to configure the output, assuming of course that you already have an Azure account with a storage account setup.

**Step 1: Add the connection string for Azure storage to app.config**

In the project you are using the logger from, open app.confg (web.config if it’s a web project), and add the following, putting in the values for your storage account that you get from the Azure dashboard.

```csharp
<appSettings>
    <add key="AzureStorageConnectionString" 
         value="DefaultEndpointsProtocol=https;AccountName=AccountNameFromAzureDashboard;AccountKey=AccountKeyFromAzureDashboard" />
</appSettings>
```

**Step 2: Add NLog Azure Table Storage target to your project**

From Visual Studio, select Tools->NuGet package manager from the menu. From the package manager console that appears, type Install-Package AzureTableStorageNLogTarget. This will add NLog to your project.

**Step 3: Create NLog.config, and set it to log to Azure**

In the project you are using the logger from, create a file called NLog.config, with the following contents. Note that you may replace “TestLogs” and “SomeKey” with whatever values you’d like to use.

```csharp
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="https://www.nlog-project.org/schemas/NLog.xsd"
        xmlns:xsi="https://www.w3.org/2001/XMLSchema-instance"
        internalLogFile="${basedir}/logs/Log ${shortdate}.txt"
        autoReload="true" throwExceptions="true">
 <targets>
    <target type="AzureTableStorage"
        name="azure"
        ConnectionStringKey="AzureStorageConnectionString"
        TableName="TestLogs" PartitionKeyPrefixKey="SomeKey" />
 </targets>
 <rules>
    <logger name="\*" minlevel="Trace" writeTo="azure" />
 </rules>
</nlog>
```

That’s it for configuration. Run the project, and you will be logging to Azure table storage! Next month, we will show how to build a UI for viewing these logs, but in the meantime, check out [Azure Storage Explorer](https://azurestorageexplorer.codeplex.com/downloads/get/160100) to view your table storage. [Click here for the complete source code for this sample](https://intellitectsp.sharepoint.com/Marketing/_layouts/15/guestaccess.aspx?guestaccesstoken=BRFjINJE3rJXsHwCwii93So5hI%2bFNT%2frV2BOm7y%2bCnk%3d&docid=0f806055181f24e9db04597248583e041).

_Written by Jason Peterson_
