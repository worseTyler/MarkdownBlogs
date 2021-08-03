

## Developers, Start Your Migrations! .NET 5 Paves the Way for a Great Development Experience!

For the past several years, Microsoft has invested heavily in .NET Core, their open-source, cross-platform development solution. As a result, those working with the original .NET Framework have been receiving fewer updates. While .NET Core and .NET Framework are language compatible, there have been significant gaps in Core’s feature set. With .NET 5, Microsoft unified these frameworks and charted a clear path ahead for developers.

![Graphic showing the evolution of Microsoft's various .NET frameworks, including .NET 5.](https://intellitect.com/wp-content/uploads/2021/04/dotnet5.png ".NET 5 VIDEO: A New Era in .NET")

This diagram shows how .NET Framework, .NET Standard, C#, and .NET Core align and how .NET Core has evolved through three versions to .NET 5 and beyond. During this time, .NET Standard provided a compatibility layer between .NET Framework and .NET Core. Note that .NET 4 was skipped to avoid confusion with the current .NET Framework 4.x versions.

.NET Standard provided a base for building libraries that were compatible with each runtime by specifying a core set of library functionality. However, this led to significant confusion as none of the version numbers aligned, and the concepts behind the functionality were not simple to understand.

.NET 5 is a single product with a uniform set of capabilities and APIs used for Windows desktop apps, cross-platform apps, console apps, cloud services, and websites.

### What's New in .NET 5

With the release of .NET 5, [Microsoft has dropped the Core moniker and has clearly defined the path ahead for various applications](https://docs.microsoft.com/en-us/dotnet/core/dotnet-five#net-50-doesnt-replace-net-standard). Additionally, .NET 5 will support most full framework and .NET Core application types, also called workloads. While not all project types are cross-platform like WinForms, many other project types are now available.

https://youtu.be/jELkzoS_TT0

.NET 5 has a shorter support lifecycle than other versions of .NET. Some versions, such as .NET 6, will be marked as Long-Term Support (LTS) and have support for three years after release. The quality of all releases is equal, but only even-numbered versions will have LTS.

After the next major release, .NET will only support former releases for three months. This limited-support lifecycle means developers will need to perform framework upgrades periodically to stay on a supported .NET version. Most importantly, .NET 5 has the same quality standards as the LTS releases.

Many of the changes for .NET 5 occurred behind the scenes. One significant effort was consolidating the various .NET repositories to ensure a simple and straightforward path ahead for development.  

- **Blazor** **WebAssembly and Blazor Server** are supported for production and allow developers to use C# code to build robust client-side applications. While not an exact replacement, Blazor fills some compelling use cases left by Silverlight and Web Forms.
- **C# 9 and F# 5** are the default languages for .NET 5. The current release cadence is a new .NET version each November. The plan is for this to include a new version of C# as well. 
- **Source Generators** allow for generating additional code during compilation by using the familiar Roselyn analyzer API.
- **Windows Forms (WinForms)** support has full support in the latest versions of Visual Studio. Now is an ideal time to port your WinForms applications.

### To Migrate or Not To Migrate, That is the Question

So, should you migrate my code?

For most application types, the answer is yes. But, how hard will it be?

Well, it depends. [Check out the .NET Portability Analyzer.](https://docs.microsoft.com/en-us/dotnet/standard/analyzers/portability-analyzer) Some apps will migrate very easily.

The largest area of change is around the infrastructure for ASP.NET. Migrating your code may require rework, but in most cases, I have found that the migration improves the quality of the code by supporting more modern coding approaches. For WebForms, Blazor or Razor Pages are good alternatives, and Windows Workflows can be done using an open-source product like CoreWF or Elsa-Workflow.

If you are in an unsupported case like Web Forms, you have two options. One, migrate to a regular ASP.NET web application which will mean a new architecture. Two, you could use Blazor on the server and have a similar paradigm to Web Forms.

### What Are You Waiting For?

If you have been waiting to migrate to .NET 5, the time is now! Check out [Mark Michaelis’ article on reasons to migrate to ASP.NET Core.](/intellitect-today-migrate-asp-net-core/)

[![](https://intellitect.com/wp-content/uploads/2021/04/blog-job-ad-2-1024x129.png)](/join-our-team/ ".NET 5 VIDEO: A New Era in .NET")
