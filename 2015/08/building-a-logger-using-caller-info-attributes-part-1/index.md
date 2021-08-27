

## Logging and Tracing for NET Applications Part 1: Building a Logger Using Caller Info Attributes
#
Welcome to the first of a multi-part discussion on logging and tracing for .NET applications. In this post, we will discuss using a C# 5 feature to include detailed file, member, and line number information when tracing. In future posts, we’ll discuss integrating this method with NLog to output log information to various data sources, followed by a specific discussion of logging to Azure table storage, including a rich web based UI for viewing and filtering log messages.

When building a software component, one important decision that must be made is how to log information during system execution. This includes logging general system output as well as tracing which can be used to debug the system when something goes wrong. Spending a bit of time up front building in reliable tracing can save a ton of time when investigating system failures during development, as well as when the system goes live. In this blog post, I’ll show you how to build a lightweight, generic, logger that can easily be configured to output log data to a variety of data stores.

At IntelliTect, we find ourselves working on a myriad of different projects, using a variety of technologies. From on-premise standalone applications, client / server applications, as well as cloud-based solutions, they all have a common requirement: enable developers, users, and administrators to quickly identify and determine root-cause of defects in the system. To accomplish this, we generally build tracing into the software such that output can be read to diagnose issues. This includes tracing general program flow and exception details as they occur during program execution. If a user encounters unexpected behavior, taking a look at the logs is a great place to start. Even with great tracing in place, however, the task of investigating failures can still be quite difficult. If one is not familiar with the codebase of the solution, it can be a daunting task to figure out exactly where in the code the specific trace messages come from. To combat this in the past, we have deployed .pdb files for our components along with the production code, such that exception tracing can include stack traces including line number information. This does not, however, provide file, method, and line number information for non-exception events. Fortunately, C# 5 has provided us with a mechanism for doing exactly that.

[Caller Info Attributes](https://msdn.microsoft.com/en-us/library/hh534540(VS.110).aspx) is a C# feature introduced in C# 5. These attributes allow developers to create methods with optional parameters that tell the compiler to pass the caller’s file, line number, and member name to the method. This proves to be extremely valuable when building a logger. A simple example from MSDN shows how to use these attributes:

```csharp
// using System.Runtime.CompilerServices
// using System.Diagnostics;

public void DoProcessing()
{
   TraceMessage("Something happened.");
}

public void TraceMessage(string message,
       [CallerMemberName] string memberName = "",
       [CallerFilePath] string sourceFilePath = "",
       [CallerLineNumber] int sourceLineNumber = 0)
{
   Trace.WriteLine("message: " + message);
   Trace.WriteLine("member name: " + memberName);
   Trace.WriteLine("source file path: " + sourceFilePath);
   Trace.WriteLine("source line number: " + sourceLineNumber);
}

// Sample Output:
//  message: Something happened.
//  member name: DoProcessing
//  source file path: c:\\Users\\username\\Documents\\Visual Studio 2012\\Projects\\CallerInfoCS\\CallerInfoCS\\Form1.cs
//  source line number: 31
```

It’s as simple as that to include detailed information on where a log message comes from.  Please check back next month where we’ll discuss using this to build an NLog based logger that can use this information to create rich logs, which we will write to various forms of external storage.

Check out part two of this blog - _[Creating an NLog wrapper that logs to Azure Table Storage](/creating-an-nlog-wrapper-that-logs-to-azure-table-storage/)_.

_Written by Jason Peterson_
